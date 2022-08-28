using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AskMeDraft.Models.ViewModels
{
    public class AnswerReportView
    {
        public int ID { get; set; }
        public int AnswerID { get; set; }
        public int ReporterID { get; set; }
        public string ReporterName { get; set; }
        public Nullable<int> ReportHandlerID { get; set; }
        public string ReportHandlerName { get; set; }
        public string Reason { get; set; }
        public string ReportDatetime { get; set; }
        public bool Status { get; set; }
    }
}