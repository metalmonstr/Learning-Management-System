using System;
using System.Collections.Generic;

namespace LMS.Models.LMSModels
{
    public partial class Courses
    {
        public Courses()
        {
            Classes = new HashSet<Classes>();
        }

        public string CourseName { get; set; }
        public string CourseNumber { get; set; }
        public string SubjectAbv { get; set; }
        public uint CourseId { get; set; }

        public virtual Departments SubjectAbvNavigation { get; set; }
        public virtual ICollection<Classes> Classes { get; set; }
    }
}
