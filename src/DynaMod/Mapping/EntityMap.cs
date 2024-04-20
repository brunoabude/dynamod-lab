using DynaMod.Mapping.Constraints;
using DynaMod.Mapping.Fluent.Declarative;
using DynaMod.Mapping.Fluent.Fields;
using System;
using System.Collections.Generic;
using static DynaMod.Common.Assertions;

namespace DynaMod.Mapping
{
    public class EntityMap
    {
        static private Dictionary<Type, EntityMap>    _maps;
        static private HashSet<string>                _loaded_assemblies;
        internal Type                                 mapped_entity_type;
        internal FluentExpression                     partition_key_expression;
        internal FluentExpression                     sort_key_expression;
        internal string                               table_name;
        internal Dictionary<string, List<Constraint>> field_constraints = new Dictionary<string, List<Constraint>>();

        internal protected virtual void EnsureInitialized()
        {
        }

        internal void SetTableName(string tableName)
        {
            AssertArgument(!string.IsNullOrWhiteSpace(tableName), "TableName cannot be empty", nameof(tableName));
            table_name = tableName;
        }

        internal void AddConstraint(string field_name, Constraint constraint)
        {
            if (!field_constraints.ContainsKey(field_name))
                field_constraints[field_name] = new List<Constraint>();

            field_constraints[field_name].Add(constraint);
        }

        internal static EntityMap Get(Type type)
        {
            if (_maps == null)
            {
                _maps = new Dictionary<Type, EntityMap>();
            }

            if (_loaded_assemblies == null)
            {
                _loaded_assemblies = new HashSet<string>();
            }

            if (!_loaded_assemblies.Contains(type.Assembly.FullName))
            {
                foreach (Type t in type.Assembly.GetTypes())
                {
                    var _t = t;

                    if (!t.IsClass || t.IsAbstract || t.IsInterface)
                        continue;

                    while (_t != null)
                    {
                        if (_t.IsConstructedGenericType && _t.GetGenericTypeDefinition() == typeof(FluentEntityMap<>))
                        {
                            _maps[_t.GetGenericArguments()[0]] = (EntityMap)Activator.CreateInstance(t);
                            break;
                        }

                        _t = _t.BaseType;
                    }
                }

                _loaded_assemblies.Add(type.Assembly.FullName);
            }

            return _maps.GetValueOrDefault(type);
        }
    }
}
