using Newtonsoft.Json;
using Microsoft.AspNetCore.Http;
using System.Diagnostics.CodeAnalysis;

namespace DemoWebApplication.Utils;

[ExcludeFromCodeCoverage]
public static class SessionExtensions
{
    public static void SetObjectAsJson(this ISession session, string key, object obj)
    {
        session.SetString(key, JsonConvert.SerializeObject(obj));
    }
    public static T? GetObjectFromJson<T>(this ISession session, string key)
    {
        var value = session.GetString(key);
        return value == null ? default : JsonConvert.DeserializeObject<T>(value);
    }
}
