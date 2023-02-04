using System.Net;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SwitchBoardApi.Core.Service;

namespace SwitchBoardApi.API;

[ApiController]
[Route("api/SwitchBoard")]
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
    [Route("")]
    public async Task<ActionResult<string>> GetAllContainerStatus()
    {
        try
        {
            var containerStatus = await _dockerService.MonitorContainer();
            return Ok(JsonConvert.SerializeObject(containerStatus));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get container status");

            return StatusCode(StatusCodes.Status500InternalServerError, new ProblemDetails()
            {
                Type = "https://tools.ietf.org/html/rfc7807",
                Title = "Error",
                Detail = "Unable to get container status",
                Status = (int)HttpStatusCode.InternalServerError
            });
        }
    }

    [HttpPost]
    [Route("")]
    public async Task<ActionResult> CreateContiner(string image, string containerName, CancellationToken ct = default)
    {
        try
        {
            await _dockerService.StartContainer(image, containerName, ct);
            return StatusCode(201, $"Container started - {containerName}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to start container");

            return StatusCode(StatusCodes.Status500InternalServerError, new ProblemDetails()
            {
                Type = "https://tools.ietf.org/html/rfc7807",
                Title = "Error",
                Detail = "Unable to start container",
                Status = (int)HttpStatusCode.InternalServerError
            });
        }
    }

    [HttpDelete]
    [Route("")]
    public async Task<ActionResult<string>> DeleteContainer(string containerId)
    {
        try
        {
            await _dockerService.DeleteContainer(containerId);
            return Ok("Deleted");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete container");

            return StatusCode(StatusCodes.Status500InternalServerError, new ProblemDetails()
            {
                Type = "https://tools.ietf.org/html/rfc7807",
                Title = "Error",
                Detail = "Unable to delete container",
                Status = (int)HttpStatusCode.InternalServerError
            });
        }
    }
}