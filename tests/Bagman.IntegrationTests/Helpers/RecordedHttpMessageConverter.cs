using Argon;

namespace Bagman.IntegrationTests.Helpers;

public class RecordedHttpMessageConverter : JsonConverter
{
    public override bool CanConvert(Type objectType)
    {
        return objectType.Name == "LoggedSend";
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        throw new NotImplementedException();
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        if (value == null)
        {
            writer.WriteNull();
            return;
        }

        var type = value.GetType();
        var properties = type.GetProperties();

        writer.WriteStartObject();

        foreach (var prop in properties)
        {
            var propValue = prop.GetValue(value);
            writer.WritePropertyName(prop.Name);

            if (prop.Name == "RequestContent" || prop.Name == "ResponseContent")
                WriteJsonProperty(writer, serializer, propValue as string);
            else
                serializer.Serialize(writer, propValue);
        }

        writer.WriteEndObject();
    }

    private void WriteJsonProperty(JsonWriter writer, JsonSerializer serializer, string content)
    {
        if (string.IsNullOrWhiteSpace(content))
        {
            writer.WriteNull();
            return;
        }

        try
        {
            var obj = JsonConvert.DeserializeObject(content);
            serializer.Serialize(writer, obj);
        }
        catch
        {
            writer.WriteValue(content);
        }
    }
}
