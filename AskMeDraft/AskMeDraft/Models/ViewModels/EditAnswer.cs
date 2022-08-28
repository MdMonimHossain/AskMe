using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AskMeDraft.Models.ViewModels
{
    public class EditAnswer
    {
        public int ID { get; set; }

        [Required(ErrorMessage = "Answer field empty")]
        public string Content { get; set; }
    }
}