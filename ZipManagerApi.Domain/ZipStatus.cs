using System;
using System.Text.Json.Serialization;

namespace ZipManagerApi.Domain;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ZipStatus
{
    Processing,
    Finished,
}
