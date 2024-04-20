﻿using DynaMod.ThirdParty.Json.LitJson;
using System.Collections.Generic;
using static DynaMod.Common.Assertions;

namespace DynaMod.Client.Contracts
{
    public partial class TableClassSummary
    {
        public static partial TableClassSummary Load(JsonReader json_reader)
        {
            TableClassSummary instance = new TableClassSummary();
            Assert(json_reader.Read() && json_reader.Token == JsonToken.ObjectStart);
            while (json_reader.Read() && json_reader.Token != JsonToken.ObjectEnd)
            {
                Assert(json_reader.Token == JsonToken.PropertyName);
                switch (json_reader.Value.ToString())
                {
                    case "LastUpdateDateTime":
                        Assert(json_reader.Read() && json_reader.Token == JsonToken.Double);
                        instance.LastUpdateDateTime = (float)json_reader.Value;
                        break;
                    case "TableClass":
                        Assert(json_reader.Read() && json_reader.Token == JsonToken.String);
                        instance.TableClass = (string)json_reader.Value;
                        break;
                }
            }

            return instance;
        }
    }
}