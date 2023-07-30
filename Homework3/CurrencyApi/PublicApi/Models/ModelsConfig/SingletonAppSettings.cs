namespace Fuse8_ByteMinds.SummerSchool.PublicApi.Models.ModelsConfig
{
    public class SingletonAppSettings
    {
        public AppSettings appSettings;
        private static readonly Lazy<SingletonAppSettings> lazy = new Lazy<SingletonAppSettings>(() => new SingletonAppSettings());
        private SingletonAppSettings()
        { }
        public static SingletonAppSettings Instance => lazy.Value;
    }
}
