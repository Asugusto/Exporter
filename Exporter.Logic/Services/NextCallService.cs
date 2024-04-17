using Exporter.Logic.Services;

public class NextCallService
{
    private static readonly Dictionary<Guid, string> _processingStatus = new Dictionary<Guid, string>();

    private readonly ResultProcessor _resultProcessor;

    public NextCallService(ResultProcessor resultProcessor)
    {
        _resultProcessor = resultProcessor;
    }

    public async Task ProcessTransactionsAsync(string url, Guid guid)
    {
        await _resultProcessor.ProcessTransactionsAsync(url, guid);
    }

    public static Dictionary<Guid, string> GetProcessingStatus()
    {
        return _processingStatus;
    }

    public async Task<object> GetResultAsync(Guid guid)
    {
        return await _resultProcessor.GetResult(guid);
    }
}
