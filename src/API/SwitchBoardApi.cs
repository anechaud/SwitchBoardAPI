using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SwitchBoardApi.Core.Service;

namespace SwitchBoardApi.API;

[ApiController]
[Route("[api/SwitchBoard]")]
public class SwitchBoardApi: ControllerBase
{
    private readonly ILogger<SwitchBoardApi> _logger;
    private readonly IDockerService _dockerService;

    public SwitchBoardApi(IDockerService dockerService, ILogger<SwitchBoardApi> logger)
    {
        _logger = logger;
        _dockerService = dockerService;
    }

    [HttpGet]
    public async Task<ActionResult<string>> GetAllContainerStatus()
    {
        var containerStatus = await _dockerService.MonitorContainer();
        return Ok(JsonConvert.SerializeObject(containerStatus));
    }

    [HttpPost]
    public async Task<ActionResult> CreateContiner(string image, string containerName, CancellationToken ct = default)
    {
        await _dockerService.StartContainer(image, containerName, ct);
        return StatusCode(201,$"Container created {containerName}");
    }

    [HttpDelete]
    public async Task<ActionResult<string>> DeleteContainer(string containerId)
    {
        await _dockerService.DeleteContainer(containerId);
        return Ok("Deleted");
    }
}