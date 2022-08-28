using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AskMeDraft.Models.ViewModels
{
    public class AnswerCard
    {
        public int ID { get; set; }
        public int QuestionID { get; set; }
        public int CreatorID { get; set; }
        public string UserName { get; set; }
        public string Content { get; set; }
        public string Datetime { get; set; }
        public int UpvoteCount { get; set; }
        public int DownvoteCount { get; set; }
        public bool Status { get; set; }
    }
}