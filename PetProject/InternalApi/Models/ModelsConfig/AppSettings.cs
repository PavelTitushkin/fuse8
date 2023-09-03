namespace InternalApi.Models.ModelsConfig
{
    public class AppSettings
    {
        public string Default { get; set; }
        public string Base { get; set; }
        public int Round { get; set; }
        public string APIKey { get; set; }
        public string BasePath { get; set; }
        public string PathFile { get; set; }
        public int CacheLifetime { get; set; }
        public int WaitTimeTaskExecution { get; set; }
    }
}
