using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;

namespace SignalRTemplate.Extensions;

public static class ObjectExtension
{
    public static string SerializeToJson(this object source, bool writeIndented = true, bool proepertyNameCaseInsensitive = true, JavaScriptEncoder encoder = null)
    {
        string retValue = string.Empty;
        if (encoder == null)
            encoder = JavaScriptEncoder.Create(UnicodeRanges.All);

        var options = new JsonSerializerOptions()
        {
            PropertyNameCaseInsensitive = proepertyNameCaseInsensitive,
            WriteIndented = writeIndented,
        };
        retValue = JsonSerializer.Serialize(source, options);

        return retValue;
    }

    public static T? DeserializeFromJson<T>(this string value, bool writeIndented = true, bool proepertyNameCaseInsensitive = true, JavaScriptEncoder encoder = null)
    {
        if (encoder == null)
            encoder = JavaScriptEncoder.Create(UnicodeRanges.All);

        var options = new JsonSerializerOptions()
        {
            PropertyNameCaseInsensitive = proepertyNameCaseInsensitive,
            WriteIndented = writeIndented,
        };
        var retValue = JsonSerializer.Deserialize<T>(value, options);

        return retValue;
    }

    public static dynamic? DeserializeFromJson(this string value, bool writeIndented = true, bool proepertyNameCaseInsensitive = true, JavaScriptEncoder encoder = null)
    {
        if (encoder == null)
            encoder = JavaScriptEncoder.Create(UnicodeRanges.All);

        var options = new JsonSerializerOptions()
        {
            PropertyNameCaseInsensitive = proepertyNameCaseInsensitive,
            WriteIndented = writeIndented,
        };
        var retValue = JsonSerializer.Deserialize<dynamic>(value, options);

        return retValue;
    }

    public static async Task<T?> ReadAsAsync<T>(this HttpContent content, bool writeIndented = true, bool proepertyNameCaseInsensitive = true)
    {
        var options = new JsonSerializerOptions()
        {
            PropertyNameCaseInsensitive = proepertyNameCaseInsensitive,
            WriteIndented = writeIndented,
        };

        return await JsonSerializer.DeserializeAsync<T>(await content.ReadAsStreamAsync(), options);
    }

}
