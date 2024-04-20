using DynaMod.Serialization.Attributes;
using DynaMod.ThirdParty.Json.LitJson;
using System.Collections.Generic;
using static DynaMod.Common.Assertions;

namespace DynaMod.Client.Contracts
{
    public partial class Transactitem
    {
        public static partial Transactitem Load(JsonReader json_reader)
        {
            Transactitem instance = new Transactitem();
            Assert(json_reader.Read() && json_reader.Token == JsonToken.ObjectStart);
            while (json_reader.Read() && json_reader.Token != JsonToken.ObjectEnd)
            {
                Assert(json_reader.Token == JsonToken.PropertyName);
                switch (json_reader.Value.ToString())
                {
                    case "ConditionCheck":
                        instance.ConditionCheck = Conditioncheck.Load(json_reader);
                        break;
                    case "Delete":
                        instance.Delete = Delete.Load(json_reader);
                        break;
                    case "Put":
                        instance.Put = Put.Load(json_reader);
                        break;
                    case "Update":
                        instance.Update = Update.Load(json_reader);
                        break;
                }
            }

            return instance;
        }
    }
}