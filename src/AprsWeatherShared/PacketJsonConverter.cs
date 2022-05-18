using System.Text.Json;
using System.Text.Json.Serialization;
using AprsSharp.Parsers.Aprs;

namespace AprsWeather.Shared;

public class PacketJsonConverter : JsonConverter<Packet>
{
    public override Packet? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var value = reader.GetString();
        return value == null ? null : new Packet(value);
    }

    public override void Write(Utf8JsonWriter writer, Packet value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.Encode());
    }
}
