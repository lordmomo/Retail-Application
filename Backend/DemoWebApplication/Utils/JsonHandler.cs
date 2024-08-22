//using DemoWebApplication.Models;
//using Newtonsoft.Json;
//using System.Diagnostics.CodeAnalysis;

//namespace DemoWebApplication.Utils;

//[ExcludeFromCodeCoverage]

//public static class JsonHandler
//{
//    public static List<T> GetDataFromJson<T>(string filePath)
//    {
//        var file = File.ReadAllText(filePath);
//        return JsonConvert.DeserializeObject<List<T>>(file) ?? new List<T>();
//    }

//    public static void StoreDataInJson<T>(string filePath, List<T> userData)
//    {
//        var json = JsonConvert.SerializeObject(userData, Formatting.Indented);
//        File.WriteAllText(filePath, json);
//    }
//    public static List<Ledger> GetDataFromLedger(string filePath)
//    {
//        var json = File.ReadAllText(filePath);
//        return JsonConvert.DeserializeObject<List<Ledger>>(json) ?? new List<Ledger>();
//    }

//    public static void StoreDataInLedger(string filePath, List<Ledger> data)
//    {
//        var json = JsonConvert.SerializeObject(data, Formatting.Indented);
//        File.WriteAllText(filePath, json);
//    }
//}
