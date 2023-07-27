namespace Fuse8_ByteMinds.SummerSchool.PublicApi.Abstractions
{
    public interface ISendMessage
    {
        Task<string> SendMessageAsync();
    }
}
