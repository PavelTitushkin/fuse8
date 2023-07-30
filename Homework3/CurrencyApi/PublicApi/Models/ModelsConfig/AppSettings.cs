namespace Fuse8_ByteMinds.SummerSchool.PublicApi.Models.ModelsConfig
{
    public class AppSettings
    {
        public string Default { get; set; }
        public string Base { get; set; }
        public int Round { get; set; }
        public string APIKey { get; set; }
        public string BasePath { get; set; }

        public ClientConfig clientConfig;
        public void ClientConfigBuild()
        {
            clientConfig = new ClientConfig(new ClientConfigOptions()
            {
                Default = Default,
                Base = Base,
                Round = Round,
                APIKey = APIKey,
                BasePath = BasePath,
            });
        }
    }

    public class ClientConfig
    {
        private string _default;
        private string _base;
        private int _round;
        private string _apiKey;
        private string _basePath;

        public ClientConfig(ClientConfigOptions configOptions)
        {
            _default = configOptions.Default;
            _base = configOptions.Base;
            _round = configOptions.Round;
            _apiKey = configOptions.APIKey;
            _basePath = configOptions.BasePath;
        }
    }

    public class ClientConfigOptions
    {
        public string Default;
        public string Base;
        public int Round;
        public string APIKey;
        public string BasePath;
    }
}
