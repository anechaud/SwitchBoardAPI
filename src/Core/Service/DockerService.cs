using System;
using System.Linq;
using Docker.DotNet.Models;
using SwitchBoardApi.Core.Host;
using SwitchBoardApi.Core.Model;
using static System.Net.Mime.MediaTypeNames;

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
                var continer = new ContainerCondition
                {
                    ContainerId = item.ID,
                    Status = item.State,
                    ContainerName = item.Names.Select(x => x).FirstOrDefault()
                };
                statusList.Add(continer.ToString());
            }
            return statusList;
        }

        public async Task StartContainer(ContainerRequest containerRequest, CancellationToken ct = default)
        {
            var containerParameter = FormContainerParamObject(containerRequest);

            var containers = await _dockerHost.ListContainers();
            var existingContainer = containers?.Where(x => x.Names.FirstOrDefault().Contains(containerParameter.Name))?.FirstOrDefault();
            string? containerId;
            if (existingContainer != null)
            {
                containerId = existingContainer.ID;
            }
            else
            {
                containerId = await _dockerHost.CreateContainer(containerParameter, ct);
            }

            var started = await _dockerHost.StartContainer(containerId, ct);
        }

        private CreateContainerParameters FormContainerParamObject(ContainerRequest containerRequest)
        {
            return new CreateContainerParameters
            {
                Image = containerRequest.Image,
                Name = containerRequest.ContainerName,
                Env=containerRequest.Enviorment,
                HostConfig = new HostConfig
                {
                    PublishAllPorts = true,
                    Mounts = new List<Mount>
                            {
                                new Mount
                                {
                                    Source =containerRequest.MountSource,
                                    Target = containerRequest.MountTarget,
                                    Type = "bind"
                                }
                            },
                    //Binds = new List<string>
                    //        {
                    //            $"vol_{containerName}:/data"
                    //        }
                }
            };
        }
    }
}