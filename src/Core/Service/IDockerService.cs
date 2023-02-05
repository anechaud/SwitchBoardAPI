using System;
using SwitchBoardApi.Core.Model;

namespace SwitchBoardApi.Core.Service
{
	public interface IDockerService
	{
		public Task StartContainer(ContainerRequest containerRequest, CancellationToken ct = default);
		public Task<bool> DeleteContainer(string containerId);
		public Task<IEnumerable<string>> MonitorContainer();
	}
}