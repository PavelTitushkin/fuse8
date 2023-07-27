using System.Text.Json;

namespace Fuse8_ByteMinds.SummerSchool.PublicApi.Models
{
    public class LowerCaseNamingPolicy : JsonNamingPolicy
    {
        public override string ConvertName(string name) => name.ToLower();
    }
}
