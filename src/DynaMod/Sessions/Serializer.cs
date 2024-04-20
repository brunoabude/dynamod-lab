using DynaMod.Mapping;
using DynaMod.Mapping.Fluent.Fields;
using DynaMod.Mapping.Models;
using DynaMod.Serialization.Attributes;
using DynaMod.Serialization.DataMapping;
using System;
using System.Collections.Generic;

namespace DynaMod.Sessions
{
    internal class Serializer
    {
        internal readonly Queue<object> Input;
        internal readonly Queue<(object, MapValue)> Output;

        internal Serializer()
        {
            Input = new Queue<object>();
            Output = new Queue<(object, MapValue)>();
        }

        internal void Flush()
        {
            int batchSize = Input.Count;

            for (int i = 0; i < batchSize; i++)
            {
                object entity = Input.Dequeue();
                Type entityType = entity.GetType();

                EntityMap map = EntityMap.Get(entityType);
                DataMapper dataMapper = DataMapper.GetMapper(entityType);

                HashSet<string> ignoredMemberNames = new HashSet<string>();

                if (map.partition_key_expression.NodeType == FluentExpressionType.MemberAccess)
                {
                    ignoredMemberNames.Add(((FluentMemberAccessExpression)map.partition_key_expression).MemberName);
                }

                foreach ((string name, MemberDataDefinition def) in dataMapper.GetAllMemberDataDefinitions())
                {
                    if (ignoredMemberNames.Contains(name))
                        continue;

                    // TODO: ignore members that are relationships
                    // TODO: add members that are entities the ignore list
                    // TODO: add members that are entities to the queue (should also iterate over collections)
                }

                ModelMapper modelMapper = new ModelMapper();

                MapValue document = modelMapper.Map(entity);

                Output.Enqueue((entity, document));
            }
        }

        internal void Clear()
        {
            Input.Clear();
            Output.Clear();
        }
    }
}
