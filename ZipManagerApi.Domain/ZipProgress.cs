using System;
using System.Collections.Specialized;
using System.Text.Json.Serialization;

namespace ZipManagerApi.Domain;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ZipProgress
{
    Processing,
    Finished,
}
