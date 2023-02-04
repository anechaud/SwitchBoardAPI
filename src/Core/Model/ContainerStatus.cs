using System;
namespace SwitchBoardApi.Core.Model
{
	public class ContainerStatus
	{
		public string? ContainerId { get; set; }
		public string? Status { get; set; }

        public override string ToString()
        {
            return $"{ContainerId} - {Status}";
        }
    }
}