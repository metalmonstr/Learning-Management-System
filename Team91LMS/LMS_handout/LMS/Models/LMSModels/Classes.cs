using System;
using System.Collections.Generic;

namespace LMS.Models.LMSModels
{
    public partial class Classes
    {
        public Classes()
        {
            AssignmentCategories = new HashSet<AssignmentCategories>();
            EnrolledIn = new HashSet<EnrolledIn>();
        }

        public uint ClassId { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public string Location { get; set; }
        public uint SemesterYear { get; set; }
        public string SemesterSeason { get; set; }
        public uint CourseId { get; set; }
        public string UId { get; set; }

        public virtual Courses Course { get; set; }
        public virtual Professors U { get; set; }
        public virtual ICollection<AssignmentCategories> AssignmentCategories { get; set; }
        public virtual ICollection<EnrolledIn> EnrolledIn { get; set; }
    }
}
