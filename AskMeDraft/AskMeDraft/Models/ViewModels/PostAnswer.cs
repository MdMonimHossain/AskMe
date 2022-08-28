using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AskMeDraft.Models.ViewModels
{
    public class PostAnswer
    {
        [Required(ErrorMessage = "Answer field empty")]
        public string Content { get; set; }
        public int QuestionID { get; set; }
    }
}