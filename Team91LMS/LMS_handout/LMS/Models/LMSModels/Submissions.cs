using System;
using System.Collections.Generic;

namespace LMS.Models.LMSModels
{
    public partial class Submissions
    {
        public uint Aid { get; set; }
        public string UId { get; set; }
        public DateTime SubmissionTime { get; set; }
        public string SubmissionContents { get; set; }
        public uint SubmissionScore { get; set; }

        public virtual Assignments A { get; set; }
        public virtual Students U { get; set; }
    }
}
