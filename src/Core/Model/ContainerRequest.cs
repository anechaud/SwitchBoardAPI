using System;
using System.ComponentModel.DataAnnotations;

namespace SwitchBoardApi.Core.Model
{
	public class ContainerRequest
	{
		[Required]
		public string? Image { get; set; }
        [Required]
        public string?  ContainerName { get; set; }
        public string? MountSource { get; set; }
		public string? MountTarget { get; set; }
		public IList<string>? Enviorment { get; set; }
	}
}