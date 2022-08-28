//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace AskMeDraft.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Answer
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Answer()
        {
            this.AnswerReports = new HashSet<AnswerReport>();
            this.Votes = new HashSet<Vote>();
        }
    
        public int ID { get; set; }
        public string Content { get; set; }
        public int CreatorID { get; set; }
        public int QuestionID { get; set; }
        public System.DateTime PostingDatetime { get; set; }
        public System.DateTime ModificationDatetime { get; set; }
        public int UpvoteCount { get; set; }
        public int DownvoteCount { get; set; }
        public bool Status { get; set; }
    
        public virtual User User { get; set; }
        public virtual Question Question { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<AnswerReport> AnswerReports { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Vote> Votes { get; set; }
    }
}
