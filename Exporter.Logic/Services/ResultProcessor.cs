using Newtonsoft.Json.Linq;

namespace Exporter.Logic.Services
{
    public class ResultProcessor
    {
        private readonly HttpClient _httpClient;
        private readonly DataPersistenceService _dataPersistenceService;

        public ResultProcessor(HttpClient httpClient, DataPersistenceService dataPersistenceService)
        {
            _httpClient = httpClient;
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("apiKey", "dd");
            _dataPersistenceService = dataPersistenceService;
        }

        public async Task ProcessTransactionsAsync(string url, Guid guid)
        {
            var processingStatus = NextCallService.GetProcessingStatus();

            try
            {
                processingStatus[guid] = "processing";

                while (!string.IsNullOrEmpty(url))
                {
                    HttpResponseMessage response = await _httpClient.GetAsync(url);
                    response.EnsureSuccessStatusCode();

                    string json = await response.Content.ReadAsStringAsync();
                    var jsonObj = JObject.Parse(json);

                    bool hasNextPage = (bool)jsonObj["hasNextPage"];
                    url = hasNextPage ? (string)jsonObj["nextPageUrl"] : null;
                    var itemsJson = jsonObj["items"];


                    string itemsJsonString = itemsJson.ToString();
                    await _dataPersistenceService.SaveResultAsync(guid, itemsJsonString);
                }

                processingStatus[guid] = "completed";
            }
            catch (Exception)
            {
                //TODO: Log errors
                processingStatus[guid] = "error";
                throw;
            }
        }

        public async Task<object> GetResult(Guid guid)
        {
            return await _dataPersistenceService.GetResultAsync(guid);
        }
    }
}
