using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AskMeDraft.Models.ViewModels
{
    public class ChangePassword
    {
        [Required(ErrorMessage = "Old Password can not be empty")]
        [DataType(DataType.Password)]
        public string OldPassword { get; set; }

        [Required(ErrorMessage = "New Password can not be empty")]
        [DataType(DataType.Password)]
        [MinLength(5, ErrorMessage = "Password must be atleast 5 characters long")]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = "Confirm Password can not be empty")]
        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "New Password and Confirm Password do not match")]
        public string ConfirmPassword { get; set; }
    }
}