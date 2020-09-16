using Datory.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace SSCMS.Block.Models
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum BlockMethod
    {
        [DataEnum(DisplayName = "转至指定网址")] RedirectUrl,
        [DataEnum(DisplayName = "显示拦截信息")] Warning,
        [DataEnum(DisplayName = "输入密码验证")] Password,
    }
}
