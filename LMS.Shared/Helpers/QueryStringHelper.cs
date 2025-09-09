using System.Collections;
using System.Web;

namespace LMS.Shared.Helpers;
public static class QueryStringHelper
{
    public static string ObjectToQueryString(object obj)
    {
        var properties = new List<string>();

        foreach (var prop in obj.GetType().GetProperties())
        {
            var value = prop.GetValue(obj);
            if (value == null) continue;

            if (value is ICollection collection)
            {
                foreach (var item in collection)
                {
                    if (item != null)
                    {
                        properties.Add($"{prop.Name}={HttpUtility.UrlEncode(item.ToString())}");
                    }
                }
            }
            else
            {
                properties.Add($"{prop.Name}={HttpUtility.UrlEncode(value.ToString())}");
            }
        }

        return "?" + string.Join("&", properties);
    }
}
