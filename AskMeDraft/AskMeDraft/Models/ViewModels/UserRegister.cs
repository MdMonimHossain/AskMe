using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AskMeDraft.Models.ViewModels
{
    public class UserRegister
    {
        [Required(ErrorMessage = "Name can not be empty")]
        public string Name { get; set; }
        
        [Required(ErrorMessage = "Username can not be empty")]
        [RegularExpression("^[a-zA-Z][a-zA-Z0-9]{4,19}$", ErrorMessage = "Username must be 5 to 20 characters long and must start with a letter")]
        public string Username { get; set; }
        
        [Required(ErrorMessage = "Email can not be empty")]
        [EmailAddress(ErrorMessage = "Please enter a correct email")]
        public string Email { get; set; }
        
        [Required(ErrorMessage = "Password can not be empty")]
        [DataType(DataType.Password)]
        [MinLength(5, ErrorMessage = "Password must be atleast 5 characters long")]
        public string Password { get; set; }
        
        [Required(ErrorMessage = "Confirm Password can not be empty")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Password and Confirm Password do not match")]
        public string ConfirmPassword { get; set; }
    }
}