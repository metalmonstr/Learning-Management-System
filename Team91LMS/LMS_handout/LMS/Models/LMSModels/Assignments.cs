using System;
using System.Collections.Generic;

namespace LMS.Models.LMSModels
{
    public partial class Assignments
    {
        public Assignments()
        {
            Submissions = new HashSet<Submissions>();
        }

        public uint Aid { get; set; }
        public uint Acid { get; set; }
        public string AssignmentName { get; set; }
        public DateTime DueDate { get; set; }
        public string AssignmentContents { get; set; }
        public uint MaxPointValue { get; set; }

        public virtual AssignmentCategories Ac { get; set; }
        public virtual ICollection<Submissions> Submissions { get; set; }
    }
}
