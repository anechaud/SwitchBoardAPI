using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SwitchBoardApi.Core.Model;
using SwitchBoardApi.Core.Service;

namespace SwitchBoardApi.API;

[Authorize]
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
    [Route("/containers")]
    public async Task<ActionResult<IEnumerable<ContainerCondition>>> GetAllContainerStatus()
    {
        var containerStatus = await _dockerService.MonitorContainer();
        return Ok(containerStatus);
    }

    /// <summary>
    /// Get all container status - possibility for a pages result
    /// </summary>
    /// <param name="page"></param>
    /// <param name="limit"></param>
    /// <returns></returns>
    [HttpGet]
    [Route("/containers/{page}/{limit}")]
    public async Task<ActionResult<IEnumerable<ContainerCondition>>> GetPagegContainerStatus(int page = 1, int limit = 10)
    {
        var containerStatus = await _dockerService.MonitorContainer(page, limit);
        HttpContext.Response.Headers.Add("Paging-Headers-CurrentPage", JsonConvert.SerializeObject(containerStatus.CurrentPage));
        HttpContext.Response.Headers.Add("Paging-Headers-NextPage", JsonConvert.SerializeObject(containerStatus.NextPage));
        HttpContext.Response.Headers.Add("Paging-Headers-PageSize", JsonConvert.SerializeObject(containerStatus.PageSize));
        HttpContext.Response.Headers.Add("Paging-Headers-PreviousPage", JsonConvert.SerializeObject(containerStatus.PreviousPage));
        HttpContext.Response.Headers.Add("Paging-Headers-TotalCount", JsonConvert.SerializeObject(containerStatus.TotalCount));
        HttpContext.Response.Headers.Add("Paging-Headers-TotalPages", JsonConvert.SerializeObject(containerStatus.TotalPages));

        return Ok(containerStatus.ListOfItems);
    }

    /// <summary>
    /// Starts a container
    /// </summary>
    /// <param name="containerRequest"></param>
    /// <param name="ct"></param>
    /// <returns>Name of the started container</returns>
    /// <exception cref="BadHttpRequestException"></exception>
    [HttpPost]
    [Route("/computation")]
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
    [Route("/containers/{containerId}")]
    public async Task<ActionResult<string>> DeleteContainer(string containerId)
    {
        await _dockerService.DeleteContainer(containerId);
        return Ok("Deleted");
    }
}