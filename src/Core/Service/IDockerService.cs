using System;
using SwitchBoardApi.Core.Model;

namespace SwitchBoardApi.Core.Service
{
	public interface IDockerService
	{
		public Task StartContainer(string image, string containerName, CancellationToken ct = default);
		public Task<bool> DeleteContainer(string containerId);
		public Task<IEnumerable<string>> MonitorContainer(string containerId);
	}
}