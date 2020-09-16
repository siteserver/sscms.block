using Datory.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace SSCMS.Block.Models
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum AreaType
    {
        [DataEnum(DisplayName = "不拦截区域")] None,
        [DataEnum(DisplayName = "拦截以下区域")] Includes,
        [DataEnum(DisplayName = "除以下区域外，拦截所有其他区域")] Excludes
    }
}
