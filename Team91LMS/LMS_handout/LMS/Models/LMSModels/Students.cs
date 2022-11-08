using System;
using System.Collections.Generic;

namespace LMS.Models.LMSModels
{
    public partial class Students
    {
        public Students()
        {
            EnrolledIn = new HashSet<EnrolledIn>();
            Submissions = new HashSet<Submissions>();
        }

        public string UId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime Dob { get; set; }
        public string SubjectAbv { get; set; }

        public virtual Departments SubjectAbvNavigation { get; set; }
        public virtual ICollection<EnrolledIn> EnrolledIn { get; set; }
        public virtual ICollection<Submissions> Submissions { get; set; }
    }
}
