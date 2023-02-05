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
        private static readonly bool IsMac = RuntimeInformation.IsOSPlatform(OSPlatform.OSX);
        private readonly IConfiguration Configuration;
        private readonly DockerClient _dockerClient;
        public DockerHost(IConfiguration configuration)
        {
            _dockerClient = new DockerClientConfiguration(new Uri(DockerApiUri())).CreateClient();
            Configuration = configuration;
        }

        private async Task PullImageIfNotExist(string image, CancellationToken ct = default)
        {
            var existingContainers = await ListContainers();

            var exists = existingContainers.Any(x => x.Image == image);
            Progress<JSONMessage> progress = new Progress<JSONMessage>();
            progress.ProgressChanged += (sender, message) =>
            {
                Console.WriteLine(message.Status);
            };
            if (!exists)
            {
                await _dockerClient.Images.CreateImageAsync(new ImagesCreateParameters
                {
                    FromImage = image,
                    Tag = "latest"
                }, null, progress, ct);
            }
        }

        public async Task<string> CreateContainer(CreateContainerParameters createContainerParameters, CancellationToken ct = default)
        {
            await PullImageIfNotExist(createContainerParameters.Image, ct);

            //var volumeList = await _dockerClient.Volumes.ListAsync();
            //var volumeCount = volumeList.Volumes.Where(v => v.Name == containerName).Count();
            //if (volumeCount <= 0)
            //{
            //    await _dockerClient.Volumes.CreateAsync(new VolumesCreateParameters
            //    {
            //        Name = $"vol_{containerName}",
            //    });
            //};

            return (await _dockerClient.Containers.CreateContainerAsync(createContainerParameters, ct)).ID;
        }

        public async Task<bool> StartContainer(string containerId, CancellationToken ct = default)
        {
            var isStarted = await _dockerClient.Containers.StartContainerAsync(containerId, new ContainerStartParameters(), ct);
            return isStarted;
        }

        public ValueTask DisposeAsync()
        {
            _dockerClient.Dispose();
            return new ValueTask();
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

        private async Task<IList<ImagesListResponse>> ListImages()
        {
            var images = await _dockerClient.Images.ListImagesAsync(new ImagesListParameters() { All = true });
            return images;
        }

        private static string DockerApiUri()
        {
            if (IsWindows)
                return "npipe://./pipe/docker_engine";

            if (IsLinux || IsMac)
                return "unix:///var/run/docker.sock";

            throw new Exception(
                "Was unable to determine what OS this is running on, does not appear to be Windows or Linux!?");
        }
    }
}