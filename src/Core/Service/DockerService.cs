using System;
using SwitchBoardApi.Core.Host;
using SwitchBoardApi.Core.Model;

namespace SwitchBoardApi.Core.Service
{
	public class DockerService : IDockerService
    {
        private readonly IDockerHost _dockerHost;
		public DockerService(IDockerHost dockerHost)
		{
            _dockerHost = dockerHost;
		}

        public async Task<bool> DeleteContainer(string containerId)
        {
            return await _dockerHost.StopContainer(containerId);
        }

        public async Task<IEnumerable<string>> MonitorContainer()
        {
            var containers = await _dockerHost.ListContainers();
            var statusList = new List<string>();
            foreach (var item in containers)
            {
                var continer = new ContainerStatus
                {
                    ContainerId = item.ID,
                    Status = item.State
                };
                statusList.Add(continer.ToString());
            }
            return statusList;
        }

        public async Task StartContainer(string image, string containerName, CancellationToken ct = default)
        {
            await _dockerHost.StartContainer(image, containerName, ct);
        }
    }
}