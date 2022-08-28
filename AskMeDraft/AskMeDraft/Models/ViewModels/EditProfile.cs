using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AskMeDraft.Models.ViewModels
{
    public class EditProfile
    {
        public int ID { get; set; }

        [Required(ErrorMessage = "Name can not be empty")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Email can not be empty")]
        [EmailAddress(ErrorMessage = "Please enter a correct email")]
        public string Email { get; set; }
        public string Role { get; set; }
        public bool Status { get; set; }
    }
}