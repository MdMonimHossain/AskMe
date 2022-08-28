using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AskMeDraft.Models.ViewModels
{
    public class QuestionCard
    {
        public int ID { get; set; }
        public int CreatorID { get; set; }
        public string UserName { get; set; }
        public string QuestionTitle { get; set; }
        public string Datetime { get; set; }
        public Nullable<int> ViewCount { get; set; }
        public int AnswerCount { get; set; }
    }
}