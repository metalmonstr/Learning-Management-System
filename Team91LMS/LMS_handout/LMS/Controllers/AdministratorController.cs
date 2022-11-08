using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LMS.Models.LMSModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LMS.Controllers
{
  [Authorize(Roles = "Administrator")]
  public class AdministratorController : CommonController
  {
    public IActionResult Index()
    {
      return View();
    }

    public IActionResult Department(string subject)
    {
      ViewData["subject"] = subject;
      return View();
    }

    public IActionResult Course(string subject, string num)
    {
      ViewData["subject"] = subject;
      ViewData["num"] = num;
      return View();
    }

    /*******Begin code to modify********/

    /// <summary>
    /// Returns a JSON array of all the courses in the given department.
    /// Each object in the array should have the following fields:
    /// "number" - The course number (as in 5530)
    /// "name" - The course name (as in "Database Systems")
    /// </summary>
    /// <param name="subject">The department subject abbreviation (as in "CS")</param>
    /// <returns>The JSON result</returns>
    public IActionResult GetCourses(string subject)
    {
            using (Team91LMSContext db = new Team91LMSContext())
            {
                var query = from p in db.Courses
                            where p.SubjectAbv.Equals(subject)
                            select new {number = p.CourseNumber, name = p.CourseName};
                if (query.Any())
                {
                    return Json(query.ToArray());
                }
            }
            return Json(null);
    }


    


    /// <summary>
    /// Returns a JSON array of all the professors working in a given department.
    /// Each object in the array should have the following fields:
    /// "lname" - The professor's last name
    /// "fname" - The professor's first name
    /// "uid" - The professor's uid
    /// </summary>
    /// <param name="subject">The department subject abbreviation</param>
    /// <returns>The JSON result</returns>
    public IActionResult GetProfessors(string subject)
    {
            using (Team91LMSContext db = new Team91LMSContext())
            {
                var query = from p in db.Professors
                            where p.SubjectAbv.Equals(subject)
                        select new { lname = p.LastName, fname = p.FirstName, uid = p.UId};
                if (query.Any())
                {
                    return Json(query.ToArray());
                }
            }
            return Json(null);
    }



    /// <summary>
    /// Creates a course.
    /// A course is uniquely identified by its number + the subject to which it belongs
    /// </summary>
    /// <param name="subject">The subject abbreviation for the department in which the course will be added</param>
    /// <param name="number">The course number</param>
    /// <param name="name">The course name</param>
    /// <returns>A JSON object containing {success = true/false},
	/// false if the Course already exists.</returns>
    public IActionResult CreateCourse(string subject, int number, string name)
    {
            Courses course = new Courses();
            course.CourseName = name;
            course.SubjectAbv = subject;
            course.CourseNumber = number.ToString();
            for(int i = course.CourseNumber.Length; i < 4; i++)
            {
                course.CourseNumber = "0" + course.CourseNumber;
            }
            using (Team91LMSContext db = new Team91LMSContext())
            {
                db.Courses.Add(course);
                try
                {
                    db.SaveChanges();
                    return Json(new { success = true });
                }
                catch
                {
                    return Json(new { success = false });
                }
            }

    }



    /// <summary>
    /// Creates a class offering of a given course.
    /// </summary>
    /// <param name="subject">The department subject abbreviation</param>
    /// <param name="number">The course number</param>
    /// <param name="season">The season part of the semester</param>
    /// <param name="year">The year part of the semester</param>
    /// <param name="start">The start time</param>
    /// <param name="end">The end time</param>
    /// <param name="location">The location</param>
    /// <param name="instructor">The uid of the professor</param>
    /// <returns>A JSON object containing {success = true/false}. 
    /// false if another class occupies the same location during any time 
    /// within the start-end range in the same semester, or if there is already
    /// a Class offering of the same Course in the same Semester.</returns>
    public IActionResult CreateClass(string subject, int number, string season, int year, DateTime start, DateTime end, string location, string instructor)
    {
            Classes classToAdd = new Classes();
            classToAdd.StartTime = start;
            classToAdd.EndTime = end;
            classToAdd.Location = location;
            classToAdd.UId = instructor;
            classToAdd.SemesterSeason = season;
            classToAdd.SemesterYear = Convert.ToUInt16(year);
            //find the courseID
            using (Team91LMSContext db = new Team91LMSContext())
            {
                //get class id
                var IDquery = from p in db.Courses
                        where p.SubjectAbv.Equals(subject) && int.Parse(p.CourseNumber) == number
                        select p.CourseId;
                classToAdd.CourseId = IDquery.First();
                //check for invalid class
                var query = from p in db.Classes
                            where (p.SemesterSeason == season && p.SemesterYear == Convert.ToUInt16(year)) //they are in same semester and
                            && ((p.Location.Equals(location) && classesOverlap((DateTime)p.StartTime, (DateTime)p.EndTime, start, end)) //at same location with overlapping times OR
                            || p.CourseId == classToAdd.CourseId) //same course
                            select p;
                if (!query.Any())
                {

                    db.Classes.Add(classToAdd);
                    try 
                    {
                        db.SaveChanges();
                    }
                    catch
                    {
                        return Json(new { success = false });
                    }
                        
                        return Json(new { success = true });
                }

            }
            return Json(new { success = false });
    }
        //checks the start and end times of class a to see if it overlaps with the start and end time of class b and vice versa
        //overlaps if one starts before the other ends and ends before the other starts
        private bool classesOverlap(DateTime aStart, DateTime aEnd, DateTime bStart, DateTime bEnd)
        {
            //A overlaps B
            //if a starts before b ends
            if(aStart.CompareTo(bEnd) < 0)
            {
                //and ends after b starts
                if(aEnd.CompareTo(bStart) > 0)
                {
                    return true;
                }
            }
            //B overlaps A
            //if b starts before a ends
            if (bStart.CompareTo(aEnd) < 0)
            {
                //and ends after a starts
                if (bEnd.CompareTo(aStart) > 0)
                {
                    return true;
                }
            }
            return false;
        }

    /*******End code to modify********/

  }
}