using System;
using System.ComponentModel.DataAnnotations;

namespace HealthNet.DTOs.UserDTO;

public class ForgotPasswordDto
{
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string NewPassword { get; set; }

        [Required]
        public string ConfirmPassword { get; set; }
}
