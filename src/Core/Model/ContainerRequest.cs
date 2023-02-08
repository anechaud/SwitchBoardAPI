using System;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace SwitchBoardApi.Core.Model
{
    public class ContainerRequest
    {
        [Required(ErrorMessage = "Please provide the image")]
        public string Image { get; set; } = string.Empty;
        public string? ContainerName { get; set; } = string.Empty;
        [IsValidDirectory("Please enter a valid source")]
        public string MountSource { get; set; } = string.Empty;
        [IsValidDirectory("Please enter a valid target")]
        public string MountTarget { get; set; } = string.Empty;
        public IList<string>? Enviorment { get; set; }
    }

    /// <summary>
    /// Custom arrtribute to validate if a string is in valid directory format
    /// </summary>
    public class IsValidDirectoryAttribute : ValidationAttribute
    {
        string errMsg;
        public IsValidDirectoryAttribute(string errorMessage):base(errorMessage)
        {
            errMsg = errorMessage;
        }

        public string GetErrorMessage() =>
            $"Please enter a valid path";

        protected override ValidationResult? IsValid(
            object? value, ValidationContext validationContext)
        {
            var path = Convert.ToString(value)??string.Empty;
            string pattern = "^(?=([/\\\\]))(?:\\1[^\\\\/\"<>|\\u0000-\\u001f]+)+\\1?\\r?$";
            var success = Regex.IsMatch(path, pattern);
            if (!success && !string.IsNullOrEmpty(path))
            {
                return new ValidationResult(errMsg);
            }

            return ValidationResult.Success;
        }
    }
}