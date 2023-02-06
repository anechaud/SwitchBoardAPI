using System;
using System.ComponentModel.DataAnnotations;

namespace SwitchBoardApi.Core.Model
{
	public class ContainerRequest
	{
		[Required(ErrorMessage ="Please provide the image")]
		public string Image { get; set; }
		[Required(ErrorMessage ="Please give a containername")]
        public string?  ContainerName { get; set; }
        public string? MountSource { get; set; }
		public string? MountTarget { get; set; }
		public IList<string>? Enviorment { get; set; }

		public static bool Validate(ContainerRequest request)
		{
			bool isValid = false;
			if (!String.IsNullOrEmpty(request.Image) && !String.IsNullOrEmpty(request.ContainerName))
				isValid = true;
			if (!String.IsNullOrEmpty(request.MountSource) || !String.IsNullOrEmpty(request.MountTarget))
				isValid = false;

			return isValid;
		}
	}
}