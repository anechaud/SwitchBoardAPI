using System;
using Docker.DotNet.Models;

namespace SwitchBoardApi.Core.Host
{
	public interface IDockerHost
	{
        public Task<string> CreateContainer(string image, string containerName, CancellationToken ct = default);
        public Task<bool> StartContainer(string containerId, CancellationToken ct = default);
        public Task<IList<ContainerListResponse>> ListContainers();
        public Task KillContainer(string containerId);
        public Task<bool> StopContainer(string containerId);
        //public Task GetContainerStatus();
    }
}