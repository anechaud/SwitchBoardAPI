using Microsoft.AspNetCore.Mvc;

namespace SwitchBoardApi.Controllers;

[ApiController]
[Route("[controller]")]
public class SwitchBoardController : ControllerBase
{
    private readonly ILogger<SwitchBoardController> _logger;

    public SwitchBoardController(ILogger<SwitchBoardController> logger)
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

