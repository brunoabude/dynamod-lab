using DynaMod.Client;
using DynaMod.Client.Contracts;
using DynaMod.Mapping;
using DynaMod.Mapping.Models;
using DynaMod.Querying;
using DynaMod.Serialization.Attributes;
using DynaMod.Serialization.DataMapping;
using DynaMod.Serialization.Document;
using DynaMod.ThirdParty.Json.LitJson;
using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using static DynaMod.Common.Assertions;

namespace DynaMod.Sessions
{
    internal struct EntityIdentity
    {
        internal readonly string table_name;
        internal readonly AttributeValue partition_key;
        internal readonly AttributeValue sort_key;

        public EntityIdentity(string table_name, AttributeValue partition_key, AttributeValue sort_key)
        {
            this.table_name = table_name;
            this.partition_key = partition_key;
            this.sort_key = sort_key;
        }

        public override bool Equals(object obj)
        {
            return obj is EntityIdentity id &&
                   table_name == id.table_name &&
                   EqualityComparer<AttributeValue>.Default.Equals(partition_key, id.partition_key) &&
                   EqualityComparer<AttributeValue>.Default.Equals(sort_key, id.sort_key);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(table_name, partition_key, sort_key);
        }

        public static bool operator ==(EntityIdentity left, EntityIdentity right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(EntityIdentity left, EntityIdentity right)
        {
            return !(left == right);
        }
    }

    public class DynamoSession
    {
        private readonly DynamoClient                  _client;
        private readonly Serializer                    _serializer;
        private readonly QueryHandler                  _query_handler;

        // Contains all the transient entities (that are newly created)
        private HashSet<object>                        _transient;
        // Contains all the entities that are already persisted (loaded from database)
        private HashSet<object>                        _persisted;
        private Dictionary<EntityIdentity, object>     _identity_table;

        // Contains the snapshots of every loaded entity so it can be used to check for updates later.
        // This is not ideal, the best approach would be to actually implement a proxy that can intercept
        // accesses to the entity data and also observe collections. Unfortunately, this feature will not be
        // implemented any time soon.
        private Dictionary<object, MapValue>           _initial_states;
        private TableSchemaCache                       _tables;
        private static Dictionary<Type, EntityMap>     _maps;

        internal DynamoSession(DynamoClient client)
        {
            _client = client;
            _query_handler = new QueryHandler(this);
            _serializer = new Serializer();
            _transient = new HashSet<object>();
            _persisted = new HashSet<object>();
            _tables = new TableSchemaCache(client);
            _maps = new Dictionary<Type, EntityMap>();
            _identity_table = new Dictionary<EntityIdentity, object>();
            _initial_states = new Dictionary<object, MapValue>();
        }

        public void Add(object entity)
        {
            _transient.Add(entity);
        }
        
        public async Task<TEntity> GetAsync<TEntity>(AttributeValue partition_key, AttributeValue sort_key = null) where TEntity : class
        {
            if (partition_key == null)
                throw new ArgumentException();

            var map = EntityMap.Get(typeof(TEntity));
            var table = _tables[map.table_name];

            if (table.HasSortKey)
            {
                if (sort_key == null && map.sort_key_expression != null && map.sort_key_expression.NodeType == Mapping.Fluent.Fields.FluentExpressionType.ConstantExpression)
                {
                    object value = map.sort_key_expression.Apply(null);
                    sort_key = DataMapper.Primitive(value);
                }

                if (sort_key == null)
                    throw new ArgumentException($"The table {table.TableName} requires a sort key, but none was provided and a default value could not be found in the map.");
            }

            var identity = new EntityIdentity(table.TableName, partition_key, sort_key);

            if (_identity_table.ContainsKey(identity))
            {
                if (_identity_table[identity].GetType() != typeof(TEntity))
                {
                    throw new InvalidOperationException($"The request entity is already at session, but it's type is {_identity_table[identity].GetType().Name} which differs from the requested {typeof(TEntity).Name}");
                }

                return (TEntity) _identity_table[identity];
            }

            var query_request = new QueryRequest
            {
                TableName = map.table_name,
                KeyConditionExpression = "#pk = :pk",
                ExpressionAttributeNames = new Dictionary<string, string>
                {
                    ["#pk"] = table.PartitionKeyFieldName
                },
                ExpressionAttributeValues = new MapValue
                {
                    [":pk"] = partition_key
                },
                Limit = 1
            };


            if (table.HasSortKey)
            {
                query_request.KeyConditionExpression += " AND #sk = :sk";
                query_request.ExpressionAttributeNames["#sk"] = table.SortKeyFieldName;
                query_request.ExpressionAttributeValues[":sk"] = sort_key;
            }

            var query_response = await _client.QueryAsync(query_request);

            if (query_response.Count == 0)
                return null;
            
            if (_identity_table.ContainsKey(identity))
            {
                throw new InvalidOperationException($"Identity is already in the session, it should be queried, something went wrong.");
            }

            var entity = new ModelLoader().Load(typeof(TEntity), query_response.Items[0]);
            _identity_table[identity] = entity;
            _persisted.Add(entity);
            _initial_states[entity] = query_response.Items[0];
            return (TEntity)entity;
        }

        private void ClearSession()
        {
            _serializer.Clear();
            _transient.Clear();
            _persisted.Clear();
            _identity_table.Clear();
            _initial_states.Clear();
        }

        public async Task CommitAsync()
        {
            /* Handle The Transient Entities First */

            foreach (object entity in _transient)
            {
                _serializer.Input.Enqueue(entity);
            }

            TransactWriteItemRequest transaction = new TransactWriteItemRequest
            {
                TransactItems = new List<Transactitem>()
            };

            _serializer.Flush();

            while (_serializer.Output.Count > 0)
            {
                (object entity, MapValue document) = _serializer.Output.Dequeue();
                var map = EntityMap.Get(entity.GetType());

                TableSchema table = _tables[map.table_name];

                object pk = map.partition_key_expression.Apply(entity);
                object sk = null;

                AttributeValue pk_primitive = DataMapper.Primitive(pk);
                AttributeValue sk_primitive = null;

                Assert(pk != null, "Invalid PartitionKey");
                Assert(DataMapper.GetDataTypeDescriptor(pk.GetType()) == table.PartitionKeyAttributeType, "InvalidPartition Key Type");
                document[table.PartitionKeyFieldName] = DataMapper.Primitive(pk);

                if (table.HasSortKey)
                {
                    sk = map.sort_key_expression.Apply(entity);
                    Assert(sk != null, "Invalid PartitionKey");
                    Assert(DataMapper.GetDataTypeDescriptor(sk.GetType()) == table.SortKeyAttributeType, "InvalidPartition Sortkey Type");
                    document[table.SortKeyFieldName] = DataMapper.Primitive(sk);
                    sk_primitive = DataMapper.Primitive(sk);
                }

                var identity = new EntityIdentity(table.TableName, pk_primitive, sk_primitive);

                if (_identity_table.ContainsKey(identity))
                {
                    throw new InvalidOperationException($"A transient entity identity conflicts with an identity already in the database.");
                }
                // TODO. We should be able to rollback it to the original session state pre-commit
                // so we can't just pluck the transient entity into the identity_table and go on because it will considered
                // a persisted entity from now on. Leving it here now only to check for conflicts between transient entities.
                _identity_table[identity] = entity;
                
                transaction.TransactItems.Add(new Transactitem
                {
                    Put = new Put
                    {
                        TableName = table.TableName,
                        Item = document
                    }
                });

                //Now, process the constraints of the transient entities.

                var member_data_definitions = DataMapper.GetMapper(entity.GetType()).GetAllMemberDataDefinitions();

                if (map.field_constraints.Count > 0)
                {
                    if (!table.HasSortKey)
                        throw new InvalidOperationException($"To define any kind of constraint, the table is required to have a sort key.");

                    if (table.SortKeyAttributeType != DataTypeDescriptor.S)
                        throw new InvalidOperationException($"To define any kind of constraint, the table is required to have a string partition_key");
                }

                foreach ((string field_name, List<Mapping.Constraints.Constraint> constraints) in map.field_constraints)
                {
                    MemberDataDefinition member_data = member_data_definitions[field_name];

                    foreach(var constraint in constraints)
                    {
                        switch (constraint)
                        {
                            case Mapping.Constraints.Constraint.Unique:
                                {
                                    var put_constraint = new Put
                                    {
                                        ExpressionAttributeNames = new Dictionary<string, string>(),
                                        ExpressionAttributeValues = null,
                                        TableName = table.TableName
                                    };

                                    put_constraint.ExpressionAttributeNames["#pk"] = table.PartitionKeyFieldName;

                                    object field_value = member_data.GetMethod.Invoke(entity, null);

                                    if (field_value == null)
                                        throw new InvalidOperationException($"The field {field_name} cannot be null because it has a unique constraint on it.");

                                    put_constraint.Item = new MapValue
                                    {
                                        [table.PartitionKeyFieldName] = new StringValue($"unique#{entity.GetType().Name}#{field_name}#{field_value}"),
                                        [table.SortKeyFieldName] = new StringValue("unique-constraint"),
                                        ["Key"] = new MapValue()
                                        {
                                            [table.PartitionKeyFieldName] = pk_primitive,
                                            [table.SortKeyFieldName] = sk_primitive
                                        }
                                    };

                                    put_constraint.ConditionExpression = "attribute_not_exists(#pk)";

                                    transaction.TransactItems.Add(new Transactitem
                                    {
                                        Put = put_constraint
                                    });
                                }
                                break;
                            default:
                                throw new NotImplementedException($"Constraint of type {constraint} is not implemented yet.");
                        }
                    }
                }
            }

            /* Handle The Persisted Entities */

            if (_serializer.Input.Count > 0 || _serializer.Output.Count > 0)
                throw new NotImplementedException();

            foreach (object entity in _persisted)
            {
                _serializer.Input.Enqueue(entity);
            }

            _serializer.Flush();

            while (_serializer.Output.Count > 0)
            {
                (object entity, MapValue document) = _serializer.Output.Dequeue();
                var map = EntityMap.Get(entity.GetType());

                TableSchema table = _tables[map.table_name];

                object pk = map.partition_key_expression.Apply(entity);
                object sk = null;

                AttributeValue pk_primitive = DataMapper.Primitive(pk);
                AttributeValue sk_primitive = null;

                Assert(pk != null, "Invalid PartitionKey");
                Assert(DataMapper.GetDataTypeDescriptor(pk.GetType()) == table.PartitionKeyAttributeType, "InvalidPartition Key Type");
                document[table.PartitionKeyFieldName] = DataMapper.Primitive(pk);

                if (table.HasSortKey)
                {
                    sk = map.sort_key_expression.Apply(entity);
                    Assert(sk != null, "Invalid PartitionKey");
                    Assert(DataMapper.GetDataTypeDescriptor(sk.GetType()) == table.SortKeyAttributeType, "InvalidPartition Sortkey Type");
                    document[table.SortKeyFieldName] = DataMapper.Primitive(sk);
                    sk_primitive = DataMapper.Primitive(sk);
                }

                var identity = new EntityIdentity(table.TableName, pk_primitive, sk_primitive);

                if (!_initial_states.ContainsKey(entity))
                {
                    throw new InvalidOperationException($"Where did the initial state for this entity go?");
                }

                var json_writer_initial = new JsonWriter();
                var json_writer_new = new JsonWriter();

                // TODO Implement Equals correctly in all attributeValues, including map and list
                DocumentSerializer.WriteMapValue(json_writer_initial, _initial_states[entity]);
                DocumentSerializer.WriteMapValue(json_writer_new, document);

                var initial = json_writer_initial.ToString();
                var final = json_writer_new.ToString();

                bool has_changed = json_writer_initial.ToString() != json_writer_new.ToString();

                if (has_changed)
                {
                    transaction.TransactItems.Add(new Transactitem
                    {
                        Put = new Put
                        {
                            TableName = table.TableName,
                            Item = document
                        }
                    });
                }
            }

            if (transaction.TransactItems.Count > 0)
            {
                await _client.CommitTransaction(transaction);
            }

            ClearSession();
        }
    }
}
