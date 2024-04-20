using DynaMod.Serialization;
using DynaMod.Serialization.Attributes;
using DynaMod.Serialization.Document;
using DynaMod.ThirdParty.Json.LitJson;
using Xunit;

namespace DynaMod.Tests.DynamoDocumentTests
{
    public class SerializationTests
    {
        [Fact]
        public void TopLevelAttribute_Serializes_To_Correct_DynamoJsonType()
        {
            var bigOJson = @"{
  ""PK"": {
    ""S"": ""PK_Value""
  },
  ""SK"": {
    ""S"": ""SK_Value""
  },
  ""Negative_Number"": {
    ""N"": ""-10.05""
  },
  ""Positive Number"": {
    ""N"": ""0""
  },
  ""True BOOL"": {
    ""BOOL"": true
  },
  ""False BOOL"": {
    ""BOOL"": false
  },
  ""Éçã"": {
    ""S"": ""~;?., ""
  },
  ""Null_Field"": {
    ""NULL"": true
  },
  ""__StringSET"": {
    ""SS"": [
      ""STRING"",
      ""STRING"",
      ""STRING2""
    ]
  },
  ""      NUMBER-SET"": {
    ""NS"": [
      ""1"",
      ""2"",
      ""-1"",
      ""-2.0"",
      "".5555"",
      ""-.123""
    ]
  },
  ""BinarySET_"": {
    ""BS"": [
      ""VVRGOCBURVhUIEVOQ09ERUQ="",
      ""QU5PVEhFUiBVVEY4IFRFWFQgRU5DT0RFRA==""
    ]
  },
  ""LIST"": {
    ""L"": [
      {
        ""M"": {
          ""STRING_MAP_INSIDE_LIST"": {
            ""S"": ""AAAA""
          }
        }
      }
    ]
  },
  ""BinaryField"": {
    ""B"": ""QU5PVEhFUiBVVEY4IFRFWFQgRU5DT0RFRA==""
  },
  ""MAP"": {
    ""M"": {
      ""LIST_INSIDE_MAP"": {
        ""L"": [
          {
            ""L"": [
              {
                ""S"": ""E1""
              },
              {
                ""S"": ""E2""
              },
              {
                ""N"": ""20""
              },
              {
                ""M"": {
                  ""NNN"": {
                    ""N"": ""0""
                  }
                }
              }
            ]
          }
        ]
      }
    }
  }
}";
            var serializer = new DocumentSerializer();
            var deserialized = serializer.FromDynamoJson(bigOJson);

        }

        [Fact]
        public void HighlyNestedDocument_DeserializesCorrecly()
        {
            var bigOJson = @"{
  ""PK"": {
    ""S"": ""PK_Value""
  },
  ""SK"": {
    ""S"": ""SK_Value""
  },
  ""Negative_Number"": {
    ""N"": ""-10.05""
  },
  ""Positive Number"": {
    ""N"": ""0""
  },
  ""True BOOL"": {
    ""BOOL"": true
  },
  ""False BOOL"": {
    ""BOOL"": false
  },
  ""Éçã"": {
    ""S"": ""~;?., ""
  },
  ""Null_Field"": {
    ""NULL"": true
  },
  ""__StringSET"": {
    ""SS"": [
      ""STRING"",
      ""STRING"",
      ""STRING2""
    ]
  },
  ""      NUMBER-SET"": {
    ""NS"": [
      ""1"",
      ""2"",
      ""-1"",
      ""-2.0"",
      "".5555"",
      ""-.123""
    ]
  },
  ""BinarySET_"": {
    ""BS"": [
      ""VVRGOCBURVhUIEVOQ09ERUQ="",
      ""QU5PVEhFUiBVVEY4IFRFWFQgRU5DT0RFRA==""
    ]
  },
  ""LIST"": {
    ""L"": [
      {
        ""M"": {
          ""STRING_MAP_INSIDE_LIST"": {
            ""S"": ""AAAA""
          }
        }
      }
    ]
  },
  ""BinaryField"": {
    ""B"": ""QU5PVEhFUiBVVEY4IFRFWFQgRU5DT0RFRA==""
  },
  ""MAP"": {
    ""M"": {
      ""LIST_INSIDE_MAP"": {
        ""L"": [
          {
            ""L"": [
              {
                ""S"": ""E1""
              },
              {
                ""S"": ""E2""
              },
              {
                ""N"": ""20""
              },
              {
                ""M"": {
                  ""NNN"": {
                    ""N"": ""0""
                  }
                }
              }
            ]
          }
        ]
      }
    }
  }
}";
            var serializer = new DocumentSerializer();
            var deserialized = serializer.FromDynamoJson(bigOJson);

            var json_writer = new JsonWriter();
            DocumentSerializer.WriteMapValue(json_writer, deserialized);

            Assert.NotNull(deserialized);
            Assert.NotEmpty(json_writer.ToString());
        }
    }
}
