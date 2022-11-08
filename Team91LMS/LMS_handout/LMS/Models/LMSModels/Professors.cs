using System;
using System.Collections.Generic;

namespace LMS.Models.LMSModels
{
    public partial class Professors
    {
        public Professors()
        {
            Classes = new HashSet<Classes>();
        }

        public string UId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime Dob { get; set; }
        public string SubjectAbv { get; set; }

        public virtual Departments SubjectAbvNavigation { get; set; }
        public virtual ICollection<Classes> Classes { get; set; }
    }
}
