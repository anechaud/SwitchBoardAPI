using Microsoft.AspNetCore.Mvc;

namespace SwitchBoardApi.API;

[ApiController]
[Route("[api/SwitchBoard]")]
public class SwitchBoardApi: ControllerBase
{
    private readonly ILogger<SwitchBoardApi> _logger;

    public SwitchBoardApi(ILogger<SwitchBoardApi> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<string>> GetAllContainerStatus()
    {
        return Ok("sdfds");
    }

    [HttpPost]
    public async Task<ActionResult> CreateContiner()
    {
        return StatusCode(201);
    }

    [HttpDelete]
    public async Task<ActionResult<string>> DeleteContainer()
    {
        return Ok("Deleted");
    }
}