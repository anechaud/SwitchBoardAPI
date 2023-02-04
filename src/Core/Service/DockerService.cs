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
                    Status = item.State,
                    ContainerName = item.Names.Select(x=>x).FirstOrDefault()
                };
                statusList.Add(continer.ToString());
            }
            return statusList;
        }

        public async Task StartContainer(string image, string containerName, CancellationToken ct = default)
        {
            var containers = await _dockerHost.ListContainers();
            var existingContainer = containers?.Where(x => x.Names.FirstOrDefault().Contains(containerName))?.FirstOrDefault();
            string? containerId;
            if (existingContainer != null)
            {
                containerId = existingContainer.ID;
            }
            else
            {
                containerId = await _dockerHost.CreateContainer(image, containerName, ct);
            }

            var started = await _dockerHost.StartContainer(containerId, ct);
        }
    }
}