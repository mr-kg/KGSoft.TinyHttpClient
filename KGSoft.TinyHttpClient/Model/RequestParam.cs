using static KGSoft.TinyHttpClient.Enums;

namespace KGSoft.TinyHttpClient.Model;

public class RequestParam
{
    public RequestParam(string key, string value, RequestParamType type)
    {
        Key = key;
        Value = value;
        Type = type;
    }

    public string Key { get; set; }
    public string Value { get; set; }
    public RequestParamType Type { get; set; }
}
