using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LMS.Models.LMSModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LMS.Controllers
{
  public class CommonController : Controller
  {

    /*******Begin code to modify********/

    // TODO: Uncomment and change 'X' after you have scaffoled

    
    protected Team91LMSContext db;

    public CommonController()
    {
      db = new Team91LMSContext();
    }
    

    /*
     * WARNING: This is the quick and easy way to make the controller
     *          use a different LibraryContext - good enough for our purposes.
     *          The "right" way is through Dependency Injection via the constructor 
     *          (look this up if interested).
    */

    // TODO: Uncomment and change 'X' after you have scaffoled
    
    public void UseLMSContext(Team91LMSContext ctx)
    {
      db = ctx;
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing)
      {
        db.Dispose();
      }
      base.Dispose(disposing);
    }
    



    /// <summary>
    /// Retreive a JSON array of all departments from the database.
    /// Each object in the array should have a field called "name" and "subject",
    /// where "name" is the department name and "subject" is the subject abbreviation.
    /// </summary>
    /// <returns>The JSON array</returns>
    public IActionResult GetDepartments()
    {
            using (Team91LMSContext db = new Team91LMSContext())
            {
                var query = from p in db.Departments
                            select new {name = p.DepartmentName, subject = p.SubjectAbv };
                return Json(query.ToArray());
            }
                // TODO: Do not return this hard-coded array.
                //return Json(new[] { new { name = "None", subject = "NONE" } });
    }



    /// <summary>
    /// Returns a JSON array representing the course catalog.
    /// Each object in the array should have the following fields:
    /// "subject": The subject abbreviation, (e.g. "CS")
    /// "dname": The department name, as in "Computer Science"
    /// "courses": An array of JSON objects representing the courses in the department.
    ///            Each field in this inner-array should have the following fields:
    ///            "number": The course number (e.g. 5530)
    ///            "cname": The course name (e.g. "Database Systems")
    /// </summary>
    /// <returns>The JSON array</returns>
    public IActionResult GetCatalog()
    {
            using (Team91LMSContext db = new Team91LMSContext())
            {
                var query = from p in db.Departments
                            select new { subject = p.SubjectAbv, dname = p.DepartmentName, courses = from i in db.Courses
                                                                                                     where i.SubjectAbv == p.SubjectAbv
                                                                                                     select new {number = i.CourseNumber, cname = i.CourseName } };
                return Json(query.ToArray());
            }
            //return Json(null);
    }

    /// <summary>
    /// Returns a JSON array of all class offerings of a specific course.
    /// Each object in the array should have the following fields:
    /// "season": the season part of the semester, such as "Fall"
    /// "year": the year part of the semester
    /// "location": the location of the class
    /// "start": the start time in format "hh:mm:ss"
    /// "end": the end time in format "hh:mm:ss"
    /// "fname": the first name of the professor
    /// "lname": the last name of the professor
    /// </summary>
    /// <param name="subject">The subject abbreviation, as in "CS"</param>
    /// <param name="number">The course number, as in 5530</param>
    /// <returns>The JSON array</returns>
    public IActionResult GetClassOfferings(string subject, int number)
    {
            using (Team91LMSContext db = new Team91LMSContext())
            {
                var query = from p in db.Courses
                            join j1 in db.Classes
                            on p.CourseId equals j1.CourseId
                            join j2 in db.Professors
                            on j1.UId equals j2.UId
                            where p.CourseNumber == number.ToString() && p.SubjectAbv == subject
                            select new { season = j1.SemesterSeason, year = j1.SemesterYear, location = j1.Location, start = j1.StartTime, end = j1.EndTime, fname = j2.FirstName, lname = j2.LastName };
                return Json(query.ToArray());
            }
            //return Json(null);
    }

    /// <summary>
    /// This method does NOT return JSON. It returns plain text (containing html).
    /// Use "return Content(...)" to return plain text.
    /// Returns the contents of an assignment.
    /// </summary>
    /// <param name="subject">The course subject abbreviation</param>
    /// <param name="num">The course number</param>
    /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
    /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
    /// <param name="category">The name of the assignment category in the class</param>
    /// <param name="asgname">The name of the assignment in the category</param>
    /// <returns>The assignment contents</returns>
    public IActionResult GetAssignmentContents(string subject, int num, string season, int year, string category, string asgname)
    {
            using (Team91LMSContext db = new Team91LMSContext())
            {
                var query = from p in db.Courses
                            join j1 in db.Classes
                            on p.CourseId equals j1.CourseId
                            where j1.SemesterSeason == season && j1.SemesterYear == year && p.SubjectAbv == subject && p.CourseNumber == num.ToString()
                            join j2 in db.AssignmentCategories
                            on j1.ClassId equals j2.ClassId
                            where j2.AssignmentCategoryName == category
                            join j3 in db.Assignments
                            on j2.Acid equals j3.Acid
                            where j3.AssignmentName == asgname
                            select j3.AssignmentContents;
                return Content(query.FirstOrDefault());
            }
                    //return Content("");
    }


    /// <summary>
    /// This method does NOT return JSON. It returns plain text (containing html).
    /// Use "return Content(...)" to return plain text.
    /// Returns the contents of an assignment submission.
    /// Returns the empty string ("") if there is no submission.
    /// </summary>
    /// <param name="subject">The course subject abbreviation</param>
    /// <param name="num">The course number</param>
    /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
    /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
    /// <param name="category">The name of the assignment category in the class</param>
    /// <param name="asgname">The name of the assignment in the category</param>
    /// <param name="uid">The uid of the student who submitted it</param>
    /// <returns>The submission text</returns>
    public IActionResult GetSubmissionText(string subject, int num, string season, int year, string category, string asgname, string uid)
    {
            using (Team91LMSContext db = new Team91LMSContext())
            {
                var query = from p in db.Courses
                            join j1 in db.Classes
                            on p.CourseId equals j1.CourseId
                            where j1.SemesterSeason == season && j1.SemesterYear == year && p.SubjectAbv == subject && p.CourseNumber == num.ToString()
                            join j2 in db.AssignmentCategories
                            on j1.ClassId equals j2.ClassId
                            where j2.AssignmentCategoryName == category
                            join j3 in db.Assignments
                            on j2.Acid equals j3.Acid
                            where j3.AssignmentName == asgname
                            join j4 in db.Submissions
                            on j3.Aid equals j4.Aid
                            where j4.UId == uid
                            select j4.SubmissionContents;
                return Content(query.FirstOrDefault());
            }
            //return Content("");
    }


    /// <summary>
    /// Gets information about a user as a single JSON object.
    /// The object should have the following fields:
    /// "fname": the user's first name
    /// "lname": the user's last name
    /// "uid": the user's uid
    /// "department": (professors and students only) the name (such as "Computer Science") of the department for the user. 
    ///               If the user is a Professor, this is the department they work in.
    ///               If the user is a Student, this is the department they major in.    
    ///               If the user is an Administrator, this field is not present in the returned JSON
    /// </summary>
    /// <param name="uid">The ID of the user</param>
    /// <returns>
    /// The user JSON object 
    /// or an object containing {success: false} if the user doesn't exist
    /// </returns>
    public IActionResult GetUser(string uid)
    {
            using (Team91LMSContext db = new Team91LMSContext())
            {
                //students
                var query = from p in db.Students
                            where p.UId.Equals(uid)
                            join c in db.Departments
                            on p.SubjectAbv equals c.SubjectAbv
                            select new { fname = p.FirstName, lname = p.LastName, uid = p.UId, department = c.DepartmentName};
                if (query.Any())
                {
                    return Json(query.First());
                }
                //professors
                query = from p in db.Professors
                            where p.UId.Equals(uid)
                            join c in db.Departments
                            on p.SubjectAbv equals c.SubjectAbv
                            select new { fname = p.FirstName, lname = p.LastName, uid = p.UId, department = c.DepartmentName };
                if (query.Any())
                {
                    return Json(query.First());
                }
                //administrators
                var adminQuery = from p in db.Administrators
                        where p.UId.Equals(uid)
                        select new { fname = p.FirstName, lname = p.LastName, uid = p.UId };
                if (adminQuery.Any())
                {
                    return Json(adminQuery.First());
                }
            }
            return Json(new { success = false } );
    }


    /*******End code to modify********/

  }
}