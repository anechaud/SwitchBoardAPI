using System.Net;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SwitchBoardApi.Core.Model;
using SwitchBoardApi.Core.Service;

namespace SwitchBoardApi.API;

[Route("api/SwitchBoard")]
public class SwitchBoardApi : ControllerBase
{
    private readonly ILogger<SwitchBoardApi> _logger;
    private readonly IDockerService _dockerService;

    public SwitchBoardApi(IDockerService dockerService, ILogger<SwitchBoardApi> logger)
    {
        _logger = logger;
        _dockerService = dockerService;
    }

    /// <summary>
    /// Get all container status
    /// </summary>
    /// <returns>A model that has information about all the containers state</returns>
    [HttpGet]
    [Route("")]
    public async Task<ActionResult<IEnumerable<ContainerCondition>>> GetAllContainerStatus()
    {
        var containerStatus = await _dockerService.MonitorContainer();
        return Ok(containerStatus);
    }

    /// <summary>
    /// Starts a container
    /// </summary>
    /// <param name="containerRequest"></param>
    /// <param name="ct"></param>
    /// <returns>Name of the started container</returns>
    /// <exception cref="BadHttpRequestException"></exception>
    [HttpPost]
    [Route("")]
    public async Task<ActionResult> CreateContiner([FromBody] ContainerRequest containerRequest, CancellationToken ct = default)
    {
        if (!ModelState.IsValid)
        {
            var message = string.Join(" , ", ModelState.Values
                                .SelectMany(v => v.Errors)
                                .Select(e => e.ErrorMessage));
            throw new BadHttpRequestException(message);
        }
        await _dockerService.StartContainer(containerRequest, ct);
        return StatusCode(201, $"Container started - {containerRequest.ContainerName}");
    }

    /// <summary>
    /// Stops and removes a container
    /// </summary>
    /// <param name="containerId"></param>
    /// <returns></returns>
    [HttpDelete]
    [Route("")]
    public async Task<ActionResult<string>> DeleteContainer(string containerId)
    {
        await _dockerService.DeleteContainer(containerId);
        return Ok("Deleted");
    }
}