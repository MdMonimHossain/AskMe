using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AskMeDraft.Models.ViewModels
{
    public class PostQuestion
    {
        [Required(ErrorMessage = "Title can not be empty")]
        public string Title { get; set; }
        public string Description { get; set; }
        public string Tags { get; set; }
    }
}