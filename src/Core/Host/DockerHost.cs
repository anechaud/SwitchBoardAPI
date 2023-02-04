using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Docker.DotNet;
using Docker.DotNet.Models;

namespace SwitchBoardApi.Core.Host
{
    public class DockerHost : IDockerHost, IAsyncDisposable
    {
        private static readonly bool IsWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
        private static readonly bool IsLinux = RuntimeInformation.IsOSPlatform(OSPlatform.Linux);

        private readonly DockerClient _dockerClient;
        public DockerHost()
        {
            _dockerClient = new DockerClientConfiguration(new Uri(DockerApiUri())).CreateClient();
        }

        private async Task PullImageIfNotExist(string image, CancellationToken ct = default)
        {
            var existingContainers = await ListContainers();

            var exists = existingContainers.Any(x => x.Image == image);

            if (!exists)
            {
                await _dockerClient.Images.CreateImageAsync(new ImagesCreateParameters
                {
                    FromImage = image,
                }, null, null, ct);
            }
        }

        public async Task<string> CreateContainer(string image, string containerName, CancellationToken ct = default)
        {
            await PullImageIfNotExist(image, ct);

            return (await _dockerClient.Containers.CreateContainerAsync(new CreateContainerParameters
            {
                Image = image,
                Name = containerName,
                HostConfig = new HostConfig
                {
                    PublishAllPorts = true,
                    AutoRemove = true
                }
            }, ct)).ID;
        }

        public async Task StartContainer(string image, string containerName, CancellationToken ct = default)
        {
            var containerId = await CreateContainer(image, containerName, ct);
            await _dockerClient.Containers.StartContainerAsync(containerId, new ContainerStartParameters(), ct);
        }

        public ValueTask DisposeAsync()
        {
            _dockerClient.Dispose();
            return new ValueTask();
        }

        private static string DockerApiUri()
        {
            if (IsWindows)
                return "npipe://./pipe/docker_engine";

            if (IsLinux)
                return "unix:///var/run/docker.sock";

            throw new Exception(
                "Was unable to determine what OS this is running on, does not appear to be Windows or Linux!?");
        }

        public async Task<IList<ContainerListResponse>> ListContainers()
        {
            IList<ContainerListResponse> containers = await _dockerClient.Containers.ListContainersAsync(
                                                     new ContainersListParameters()
                                                     {
                                                         All = true,
                                                     });
            return containers;
        }

        public async Task KillContainer(string containerId)
        {
            await _dockerClient.Containers.KillContainerAsync(
                                                    containerId,
                                                    new ContainerKillParameters(),
                                                    CancellationToken.None);
        }

        public async Task<bool> StopContainer(string containerId)
        {
            var stopped = await _dockerClient.Containers.StopContainerAsync(
                                                    containerId,
                                                    new ContainerStopParameters
                                                    {
                                                        WaitBeforeKillSeconds = 30
                                                    },
                                                    CancellationToken.None);

            return stopped;
        }
    }
}