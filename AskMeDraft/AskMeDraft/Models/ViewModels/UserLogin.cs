using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AskMeDraft.Models.ViewModels
{
    public class UserLogin
    {
        [Required(ErrorMessage = "Username can not be empty")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Password can not be empty")]
        public string Password { get; set; }
    }
}