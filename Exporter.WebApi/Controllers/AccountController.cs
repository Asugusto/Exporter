using Microsoft.AspNetCore.Mvc;
using System.Text;

[ApiController]
[Route("[controller]")]
public class AccountController : ControllerBase
{
    private readonly NextCallService _nextCallService;

    public AccountController(NextCallService nextCallService)
    {
        _nextCallService = nextCallService;
    }

    [HttpPost]
    public IActionResult Process(string accountAddress, string chain, Int64 startBlock, Int64 endBlock, Int64 startTimestamp, Int64 endTimestamp)
    {

        StringBuilder urlBuilder = new StringBuilder($"https://translate.dev.noves.fi/evm/{chain}/txs/{accountAddress}?sort=desc&pageNumber=1&pageSize=10&liveData=false");

        if (startBlock != 0)
        {
            urlBuilder.Append($"&startBlock={startBlock}");
        }

        if (endBlock != 0)
        {
            urlBuilder.Append($"&endBlock={endBlock}");
        }

        if (startTimestamp != 0)
        {
            urlBuilder.Append($"&startTimestamp={startTimestamp}");
        }

        if (endTimestamp != 0)
        {
            urlBuilder.Append($"&endTimestamp={endTimestamp}");
        }

        Guid guid = Guid.NewGuid();

        //Do not await this call so it's processed in background
        Task.Run(() =>
        {
            _nextCallService.ProcessTransactionsAsync(urlBuilder.ToString(), guid);
        });

        return Accepted(guid);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetResult(Guid id)
    {
        try
        {
            var result = await _nextCallService.GetResultAsync(id);
            return Ok(result);
        }
        catch (FileNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpGet("all")]
    public IActionResult GetAllResults()
    {
        var processingStatus = NextCallService.GetProcessingStatus();
        return Ok(processingStatus);
    }
}
