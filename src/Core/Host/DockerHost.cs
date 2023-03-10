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

        /// <summary>
        /// Pulls the image from docker
        /// </summary>
        /// <param name="image"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Create a container
        /// </summary>
        /// <param name="createContainerParameters"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Start a container
        /// </summary>
        /// <param name="containerId"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
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

        /// <summary>
        /// List all containers in the docker daemon
        /// </summary>
        /// <returns></returns>
        public async Task<IList<ContainerListResponse>> ListContainers()
        {
            IList<ContainerListResponse> containers = await _dockerClient.Containers.ListContainersAsync(
                                                     new ContainersListParameters()
                                                     {
                                                         All = true,
                                                     });
            return containers;
        }

        /// <summary>
        /// Remove a container
        /// </summary>
        /// <param name="containerId"></param>
        /// <returns></returns>
        public async Task RemoveContainer(string containerId)
        {
            await _dockerClient.Containers.RemoveContainerAsync(
                                                    containerId,
                                                    new ContainerRemoveParameters(),
                                                    CancellationToken.None);
        }

        /// <summary>
        /// Stop a container
        /// </summary>
        /// <param name="containerId"></param>
        /// <returns></returns>
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

        /// <summary>
        /// List all images
        /// </summary>
        /// <returns></returns>
        private async Task<IList<ImagesListResponse>> ListImages()
        {
            var images = await _dockerClient.Images.ListImagesAsync(new ImagesListParameters() { All = true });
            return images;
        }

        /// <summary>
        /// Get the docker uri based on os
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
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