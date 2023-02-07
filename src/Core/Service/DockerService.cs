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

        /// <summary>
        /// Deletes a container
        /// </summary>
        /// <param name="containerId"></param>
        /// <returns></returns>
        public async Task<bool> DeleteContainer(string containerId)
        {
            await _dockerHost.StopContainer(containerId);
            await _dockerHost.RemoveContainer(containerId);
            return true;
        }

        /// <summary>
        /// Get status of all the container in a docker daemon
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<ContainerCondition>> MonitorContainer()
        {
            var containers = await _dockerHost.ListContainers();
            var statusList = new List<ContainerCondition>();
            foreach (var item in containers)
            {
                var continer = new ContainerCondition
                {
                    ContainerId = item.ID,
                    Status = item.State,
                    ContainerName = item.Names.Select(x => x).FirstOrDefault()
                };
                statusList.Add(continer);
            }
            return statusList;
        }

        /// <summary>
        /// Get containers for the requested page and limit
        /// </summary>
        /// <param name="page"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        public async Task<PaginationMetadata<ContainerCondition>> MonitorContainer(int page = 1, int limit = 10)
        {
            var containers = await _dockerHost.ListContainers();
            var pagedResult = containers.Skip((page - 1) * limit).Take(limit).ToList();
            var statusList = new List<ContainerCondition>();
            foreach (var item in pagedResult)
            {
                var continer = new ContainerCondition
                {
                    ContainerId = item.ID,
                    Status = item.State,
                    ContainerName = item.Names.Select(x => x).FirstOrDefault()
                };
                statusList.Add(continer);
            }
            var totalPages = (int)Math.Ceiling(containers.Count / (double)limit);
            return new PaginationMetadata<ContainerCondition>
            {
                TotalCount=containers.Count,
                CurrentPage=page,
                ListOfItems=statusList,
                PageSize=limit,
                TotalPages= totalPages,
                NextPage = page < totalPages,
                PreviousPage = page>1
            };
        }

        /// <summary>
        /// Start a container
        /// </summary>
        /// <param name="containerRequest"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Build a container request object
        /// </summary>
        /// <param name="containerRequest"></param>
        /// <returns></returns>
        private CreateContainerParameters FormContainerParamObject(ContainerRequest containerRequest)
        {

            var requestObj = new CreateContainerParameters
            {
                Image = containerRequest.Image.Trim(),
                Name = containerRequest.ContainerName.Trim(),
                HostConfig = new HostConfig
                {
                    PublishAllPorts = true,
                },
                Env = containerRequest.Enviorment
            };

            if (!string.IsNullOrEmpty(containerRequest.MountSource) && !string.IsNullOrEmpty(containerRequest.MountTarget))
                requestObj.HostConfig.Mounts = new List<Mount>
                            {
                                new Mount
                                {
                                    Source =containerRequest.MountSource,
                                    Target = containerRequest.MountTarget,
                                    Type = "bind"
                                }
                            };
            return requestObj;
        }
    }
}