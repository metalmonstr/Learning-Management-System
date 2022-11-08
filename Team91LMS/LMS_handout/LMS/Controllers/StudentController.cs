using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LMS.Models.LMSModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LMS.Controllers
{
  [Authorize(Roles = "Student")]
  public class StudentController : CommonController
  {

    public IActionResult Index()
    {
      return View();
    }

    public IActionResult Catalog()
    {
      return View();
    }

    public IActionResult Class(string subject, string num, string season, string year)
    {
      ViewData["subject"] = subject;
      ViewData["num"] = num;
      ViewData["season"] = season;
      ViewData["year"] = year;
      return View();
    }

    public IActionResult Assignment(string subject, string num, string season, string year, string cat, string aname)
    {
      ViewData["subject"] = subject;
      ViewData["num"] = num;
      ViewData["season"] = season;
      ViewData["year"] = year;
      ViewData["cat"] = cat;
      ViewData["aname"] = aname;
      return View();
    }


    public IActionResult ClassListings(string subject, string num)
    {
      System.Diagnostics.Debug.WriteLine(subject + num);
      ViewData["subject"] = subject;
      ViewData["num"] = num;
      return View();
    }


    /*******Begin code to modify********/

    /// <summary>
    /// Returns a JSON array of the classes the given student is enrolled in.
    /// Each object in the array should have the following fields:
    /// "subject" - The subject abbreviation of the class (such as "CS")
    /// "number" - The course number (such as 5530)
    /// "name" - The course name
    /// "season" - The season part of the semester
    /// "year" - The year part of the semester
    /// "grade" - The grade earned in the class, or "--" if one hasn't been assigned
    /// </summary>
    /// <param name="uid">The uid of the student</param>
    /// <returns>The JSON array</returns>
    public IActionResult GetMyClasses(string uid)
    {
            using (Team91LMSContext db = new Team91LMSContext())
            {
                var query = from p in db.Students
                            join j1 in db.EnrolledIn
                            on p.UId equals j1.UId
                            where p.UId == uid
                            join j2 in db.Classes
                            on j1.ClassId equals j2.ClassId
                            join j3 in db.Courses
                            on j2.CourseId equals j3.CourseId
                            select new {subject = j3.SubjectAbv, number = j3.CourseNumber, name = j3.CourseName, season = j2.SemesterSeason, year = j2.SemesterYear, grade = j1.Grade };
                return Json(query.ToArray());

            }
            //return Json(null);
    }

    /// <summary>
    /// Returns a JSON array of all the assignments in the given class that the given student is enrolled in.
    /// Each object in the array should have the following fields:
    /// "aname" - The assignment name
    /// "cname" - The category name that the assignment belongs to
    /// "due" - The due Date/Time
    /// "score" - The score earned by the student, or null if the student has not submitted to this assignment.
    /// </summary>
    /// <param name="subject">The course subject abbreviation</param>
    /// <param name="num">The course number</param>
    /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
    /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
    /// <param name="uid"></param>
    /// <returns>The JSON array</returns>
    public IActionResult GetAssignmentsInClass(string subject, int num, string season, int year, string uid)
    {
            using (Team91LMSContext db = new Team91LMSContext())
            {
                var query = from p in db.Courses
                            join j1 in db.Classes
                            on p.CourseId equals j1.CourseId
                            where p.CourseNumber == num.ToString() && p.SubjectAbv == subject && j1.SemesterSeason == season && j1.SemesterYear == year
                            join j2 in db.AssignmentCategories
                            on j1.ClassId equals j2.ClassId
                            join j3 in db.Assignments
                            on j2.Acid equals j3.Acid
                            join j4 in db.EnrolledIn
                            on j1.ClassId equals j4.ClassId
                            join j5 in db.Students
                            on j4.UId equals j5.UId
                            where uid == j5.UId
                            join j6 in db.Submissions
                            on new { j3.Aid, j5.UId } equals new { j6.Aid, j6.UId }
                            into getScore
                            from t in getScore.DefaultIfEmpty()
                            //where t.UId == uid
                            select new { aname = j3.AssignmentName, cname = j2.AssignmentCategoryName, due = j3.DueDate, score = t == null ? (uint?)null : t.SubmissionScore};
                return Json(query.ToArray());

            }
            //return Json(null);
    }



    /// <summary>
    /// Adds a submission to the given assignment for the given student
    /// The submission should use the current time as its DateTime
    /// You can get the current time with DateTime.Now
    /// The score of the submission should start as 0 until a Professor grades it
    /// If a Student submits to an assignment again, it should replace the submission contents
    /// and the submission time (the score should remain the same).
	/// Does *not* automatically reject late submissions.
    /// </summary>
    /// <param name="subject">The course subject abbreviation</param>
    /// <param name="num">The course number</param>
    /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
    /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
    /// <param name="category">The name of the assignment category in the class</param>
    /// <param name="asgname">The new assignment name</param>
    /// <param name="uid">The student submitting the assignment</param>
    /// <param name="contents">The text contents of the student's submission</param>
    /// <returns>A JSON object containing {success = true/false}.</returns>
    public IActionResult SubmitAssignmentText(string subject, int num, string season, int year, 
      string category, string asgname, string uid, string contents)
    {
            using (Team91LMSContext db = new Team91LMSContext())
            {
                var query = from p in db.Students
                            join j1 in db.EnrolledIn
                            on p.UId equals j1.UId
                            where p.UId == uid
                            join j2 in db.Classes
                            on j1.ClassId equals j2.ClassId
                            where j2.SemesterSeason == season && j2.SemesterYear == year
                            join j3 in db.Courses
                            on j2.CourseId equals j3.CourseId
                            where j3.SubjectAbv == subject && j3.CourseNumber == num.ToString()
                            join j4 in db.AssignmentCategories
                            on j2.ClassId equals j4.ClassId
                            where j4.AssignmentCategoryName == category
                            join j5 in db.Assignments
                            on j4.Acid equals j5.Acid
                            where j5.AssignmentName == asgname
                            select j5.Aid;
                if (query.Any())
                {
                    //create the object and add it to the table
                    Submissions submit = new Submissions();
                    submit.SubmissionContents = contents;
                    submit.SubmissionTime = DateTime.Now;
                    submit.UId = uid;
                    submit.Aid = query.First();
                    //check if update or add
                    var existQuery = from p in db.Submissions
                            where p.UId == uid && p.Aid == submit.Aid
                            select p;
                    //update
                    if (existQuery.Any())
                    {
                        var toChange = existQuery.First();
                        toChange.SubmissionContents = contents;
                        toChange.SubmissionTime = DateTime.Now;
                    }
                    //add
                    else
                    {
                        submit.SubmissionScore = 0;
                        db.Submissions.Add(submit);
                    }
                    //made at least one change
                    try
                    {
                        if (db.SaveChanges() > 0)
                        {
                            return Json(new { success = true });
                        }
                    }
                    catch
                    {
                        return Json(new { success = false });
                    }

                }
            }
                return Json(new { success = false });
    }

    
    /// <summary>
    /// Enrolls a student in a class.
    /// </summary>
    /// <param name="subject">The department subject abbreviation</param>
    /// <param name="num">The course number</param>
    /// <param name="season">The season part of the semester</param>
    /// <param name="year">The year part of the semester</param>
    /// <param name="uid">The uid of the student</param>
    /// <returns>A JSON object containing {success = {true/false},
	/// false if the student is already enrolled in the Class.</returns>
    public IActionResult Enroll(string subject, int num, string season, int year, string uid)
    {
            using (Team91LMSContext db = new Team91LMSContext())
            {
                //get the classID
                var query = from p in db.Courses
                            join j1 in db.Classes
                            on p.CourseId equals j1.CourseId
                            where p.CourseNumber == num.ToString() && p.SubjectAbv == subject && j1.SemesterSeason == season && j1.SemesterYear == year
                            select j1.ClassId;
                if(query.Any())
                {
                    //create the object and add it to the table
                    EnrolledIn enroll = new EnrolledIn();
                    enroll.ClassId = query.First();
                    enroll.UId = uid;
                    enroll.Grade = "--";
                    db.EnrolledIn.Add(enroll);
                    //made at least one change
                    try
                    {
                        if (db.SaveChanges() > 0)
                        {
                            return Json(new { success = true });
                        }
                    }
                    catch
                    {
                        return Json(new { success = false });
                    }
                    
                }
                
            }
      return Json(new { success = false });
    }



    /// <summary>
    /// Calculates a student's GPA
    /// A student's GPA is determined by the grade-point representation of the average grade in all their classes.
    /// Assume all classes are 4 credit hours.
    /// If a student does not have a grade in a class ("--"), that class is not counted in the average.
    /// If a student does not have any grades, they have a GPA of 0.0.
    /// Otherwise, the point-value of a letter grade is determined by the table on this page:
    /// https://advising.utah.edu/academic-standards/gpa-calculator-new.php
    /// </summary>
    /// <param name="uid">The uid of the student</param>
    /// <returns>A JSON object containing a single field called "gpa" with the number value</returns>
    public IActionResult GetGPA(string uid)
    {
            using (Team91LMSContext db = new Team91LMSContext())
            {
                var query = from p in db.Students
                            where p.UId == uid
                            join j1 in db.EnrolledIn
                            on p.UId equals j1.UId
                            select j1.Grade;
                double gpa = 0.0;
                int numClasses = query.Count();
                //count up grades
                foreach (var grade in query)
                {
                    //assigns value based on grade
                    if(grade.Equals("A"))
                    {
                        gpa += 4.0;
                    }
                    else if(grade.Equals("A-"))
                    {
                        gpa += 3.7;
                    }
                    else if (grade.Equals("B+"))
                    {
                        gpa += 3.3;
                    }
                    else if (grade.Equals("B"))
                    {
                        gpa += 3.0;
                    }
                    else if (grade.Equals("B-"))
                    {
                        gpa += 2.7;
                    }
                    else if (grade.Equals("C+"))
                    {
                        gpa += 2.3;
                    }
                    else if (grade.Equals("C"))
                    {
                        gpa += 2.0;
                    }
                    else if (grade.Equals("C-"))
                    {
                        gpa += 1.7;
                    }
                    else if (grade.Equals("D+"))
                    {
                        gpa += 1.3;
                    }
                    else if (grade.Equals("D"))
                    {
                        gpa += 1.0;
                    }
                    else if (grade.Equals("D-"))
                    {
                        gpa += 0.7;
                    }
                    else if (grade.Equals("--"))
                    {
                        numClasses -= 1;
                    }
                }
                //check num classes > 0
                if(numClasses > 0)
                {
                    gpa = gpa / numClasses;
                }
                return Json(new { gpa = gpa });
            }
            //return Json(null);
    }

    /*******End code to modify********/

  }
}