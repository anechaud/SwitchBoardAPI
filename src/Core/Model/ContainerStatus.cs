using System;
namespace SwitchBoardApi.Core.Model
{
    public class ContainerCondition
    {
        public string? ContainerId { get; set; }
        public string? Status { get; set; }
        public string? ContainerName { get; set; }

        public override string ToString()
        {
            return $"{ContainerId} - {ContainerName} - {Status}";
        }
    }
}