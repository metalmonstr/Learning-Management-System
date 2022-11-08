using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LMS.Models.LMSModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LMS.Controllers
{
  [Authorize(Roles = "Professor")]
  public class ProfessorController : CommonController
  {
    public IActionResult Index()
    {
      return View();
    }

    public IActionResult Students(string subject, string num, string season, string year)
    {
      ViewData["subject"] = subject;
      ViewData["num"] = num;
      ViewData["season"] = season;
      ViewData["year"] = year;
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

    public IActionResult Categories(string subject, string num, string season, string year)
    {
      ViewData["subject"] = subject;
      ViewData["num"] = num;
      ViewData["season"] = season;
      ViewData["year"] = year;
      return View();
    }

    public IActionResult CatAssignments(string subject, string num, string season, string year, string cat)
    {
      ViewData["subject"] = subject;
      ViewData["num"] = num;
      ViewData["season"] = season;
      ViewData["year"] = year;
      ViewData["cat"] = cat;
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

    public IActionResult Submissions(string subject, string num, string season, string year, string cat, string aname)
    {
      ViewData["subject"] = subject;
      ViewData["num"] = num;
      ViewData["season"] = season;
      ViewData["year"] = year;
      ViewData["cat"] = cat;
      ViewData["aname"] = aname;
      return View();
    }

    public IActionResult Grade(string subject, string num, string season, string year, string cat, string aname, string uid)
    {
      ViewData["subject"] = subject;
      ViewData["num"] = num;
      ViewData["season"] = season;
      ViewData["year"] = year;
      ViewData["cat"] = cat;
      ViewData["aname"] = aname;
      ViewData["uid"] = uid;
      return View();
    }

    /*******Begin code to modify********/


    /// <summary>
    /// Returns a JSON array of all the students in a class.
    /// Each object in the array should have the following fields:
    /// "fname" - first name
    /// "lname" - last name
    /// "uid" - user ID
    /// "dob" - date of birth
    /// "grade" - the student's grade in this class
    /// </summary>
    /// <param name="subject">The course subject abbreviation</param>
    /// <param name="num">The course number</param>
    /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
    /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
    /// <returns>The JSON array</returns>
    public IActionResult GetStudentsInClass(string subject, int num, string season, int year)
    {
            using (Team91LMSContext db = new Team91LMSContext())
            {
                var query = from p in db.Courses
                            join j1 in db.Classes
                            on p.CourseId equals j1.CourseId
                            where p.CourseNumber == num.ToString() && p.SubjectAbv == subject && j1.SemesterSeason == season && j1.SemesterYear == year
                            join j2 in db.EnrolledIn
                            on j1.ClassId equals j2.ClassId
                            join j3 in db.Students
                            on j2.UId equals j3.UId
                            select new {fname = j3.FirstName, lname = j3.LastName, uid = j3.UId, dob = j3.Dob, grade = j2.Grade};
                return Json(query.ToArray());

            }
            //return Json(null);
    }



    /// <summary>
    /// Returns a JSON array with all the assignments in an assignment category for a class.
    /// If the "category" parameter is null, return all assignments in the class.
    /// Each object in the array should have the following fields:
    /// "aname" - The assignment name
    /// "cname" - The assignment category name.
    /// "due" - The due DateTime
    /// "submissions" - The number of submissions to the assignment
    /// </summary>
    /// <param name="subject">The course subject abbreviation</param>
    /// <param name="num">The course number</param>
    /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
    /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
    /// <param name="category">The name of the assignment category in the class, 
    /// or null to return assignments from all categories</param>
    /// <returns>The JSON array</returns>
    public IActionResult GetAssignmentsInCategory(string subject, int num, string season, int year, string category)
    {
            using (Team91LMSContext db = new Team91LMSContext())
            {
                //gets assignments from catagory
                if (category != null)
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
                                select new {aname = j3.AssignmentName, cname = j2.AssignmentCategoryName, due = j3.DueDate, submissions =  from i in db.Submissions
                                                                                                                                           where i.Aid == j3.Aid
                                                                                                                                           group i by i.Aid into numSubmissions
                                                                                                                                           select numSubmissions.Count()};
                    return Json(query.ToArray());
                }
                //get all assignments 
                else {
                    var query = from p in db.Courses
                                join j1 in db.Classes
                                on p.CourseId equals j1.CourseId
                                where j1.SemesterSeason == season && j1.SemesterYear == year && p.SubjectAbv == subject && p.CourseNumber == num.ToString()
                                join j2 in db.AssignmentCategories
                                on j1.ClassId equals j2.ClassId
                                join j3 in db.Assignments
                                on j2.Acid equals j3.Acid
                                select new
                                {
                                    aname = j3.AssignmentName,
                                    cname = j2.AssignmentCategoryName,
                                    due = j3.DueDate,
                                    submissions = from i in db.Submissions
                                                  where i.Aid == j3.Aid
                                                  group i by i.Aid into numSubmissions
                                                  select numSubmissions.Count()
                                };
                    return Json(query.ToArray());
                }
                
            }
      //return Json(null);
    }


    /// <summary>
    /// Returns a JSON array of the assignment categories for a certain class.
    /// Each object in the array should have the folling fields:
    /// "name" - The category name
    /// "weight" - The category weight
    /// </summary>
    /// <param name="subject">The course subject abbreviation</param>
    /// <param name="num">The course number</param>
    /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
    /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
    /// <param name="category">The name of the assignment category in the class</param>
    /// <returns>The JSON array</returns>
    public IActionResult GetAssignmentCategories(string subject, int num, string season, int year)
    {
            using (Team91LMSContext db = new Team91LMSContext())
            {
                //querry for the class ID, then get vals
                var query = from p in db.Courses
                            join j1 in db.Classes
                            on p.CourseId equals j1.CourseId
                            where j1.SemesterSeason == season && j1.SemesterYear == year && p.SubjectAbv == subject && p.CourseNumber == num.ToString()
                            join j2 in db.AssignmentCategories
                            on j1.ClassId equals j2.ClassId
                            select new {name = j2.AssignmentCategoryName, weight = j2.GradeWeight };
                            return Json(query.ToArray());
            }
      //return Json(null);
    }

    /// <summary>
    /// Creates a new assignment category for the specified class.
    /// </summary>
    /// <param name="subject">The course subject abbreviation</param>
    /// <param name="num">The course number</param>
    /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
    /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
    /// <param name="category">The new category name</param>
    /// <param name="catweight">The new category weight</param>
    /// <returns>A JSON object containing {success = true/false},
    ///	false if an assignment category with the same name already exists in the same class.</returns>
    public IActionResult CreateAssignmentCategory(string subject, int num, string season, int year, string category, int catweight)
    {
            
           
            using (Team91LMSContext db = new Team91LMSContext())
            {
                //querry for the class ID
                var query = from p in db.Courses
                            join j1 in db.Classes
                            on p.CourseId equals j1.CourseId
                            where j1.SemesterSeason == season && j1.SemesterYear == year && p.SubjectAbv == subject && p.CourseNumber == num.ToString()
                            select j1.ClassId;
                //build the assignment
                if (query.Any())
                {
                    AssignmentCategories cat = new AssignmentCategories();
                    cat.AssignmentCategoryName = category;
                    cat.GradeWeight = Convert.ToUInt16(catweight);
                    cat.ClassId = query.First();
                    db.AssignmentCategories.Add(cat);
                    
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
            return Json(new { success = false });
    }

    /// <summary>
    /// Creates a new assignment for the given class and category.
    /// </summary>
    /// <param name="subject">The course subject abbreviation</param>
    /// <param name="num">The course number</param>
    /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
    /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
    /// <param name="category">The name of the assignment category in the class</param>
    /// <param name="asgname">The new assignment name</param>
    /// <param name="asgpoints">The max point value for the new assignment</param>
    /// <param name="asgdue">The due DateTime for the new assignment</param>
    /// <param name="asgcontents">The contents of the new assignment</param>
    /// <returns>A JSON object containing success = true/false,
	/// false if an assignment with the same name already exists in the same assignment category.</returns>
    public IActionResult CreateAssignment(string subject, int num, string season, int year, string category, string asgname, int asgpoints, DateTime asgdue, string asgcontents)
    {
            using (Team91LMSContext db = new Team91LMSContext())
            {
                //querry for the class ID
                var query = from p in db.Courses
                            join j1 in db.Classes
                            on p.CourseId equals j1.CourseId
                            where j1.SemesterSeason == season && j1.SemesterYear == year && p.SubjectAbv == subject && p.CourseNumber == num.ToString()
                            join j2 in db.AssignmentCategories
                            on j1.ClassId equals j2.ClassId
                            where j2.AssignmentCategoryName == category
                            select j2.Acid;
                //build the assignment
                if (query.Any())
                {
                    Assignments assignment = new Assignments();
                    assignment.Acid = query.First();
                    assignment.AssignmentContents = asgcontents;
                    assignment.MaxPointValue = Convert.ToUInt16(asgpoints);
                    assignment.AssignmentName = asgname;
                    assignment.DueDate = asgdue;
                    db.Assignments.Add(assignment);

                    try
                    {
                        db.SaveChanges();
                        UpdateStudentsGrades(subject, num, season, year);
                        return Json(new { success = true });
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
    /// Gets a JSON array of all the submissions to a certain assignment.
    /// Each object in the array should have the following fields:
    /// "fname" - first name
    /// "lname" - last name
    /// "uid" - user ID
    /// "time" - DateTime of the submission
    /// "score" - The score given to the submission
    /// 
    /// </summary>
    /// <param name="subject">The course subject abbreviation</param>
    /// <param name="num">The course number</param>
    /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
    /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
    /// <param name="category">The name of the assignment category in the class</param>
    /// <param name="asgname">The name of the assignment</param>
    /// <returns>The JSON array</returns>
    public IActionResult GetSubmissionsToAssignment(string subject, int num, string season, int year, string category, string asgname)
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
                            join j5 in db.Students
                            on j4.UId equals j5.UId
                            select new
                            {
                                fname = j5.FirstName,
                                lname = j5.LastName,
                                uid = j5.UId,
                                time = j4.SubmissionTime,
                                score = j4.SubmissionScore
                            };
                return Json(query.ToArray());
            }
                //return Json(null);
    }


    /// <summary>
    /// Set the score of an assignment submission
    /// </summary>
    /// <param name="subject">The course subject abbreviation</param>
    /// <param name="num">The course number</param>
    /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
    /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
    /// <param name="category">The name of the assignment category in the class</param>
    /// <param name="asgname">The name of the assignment</param>
    /// <param name="uid">The uid of the student who's submission is being graded</param>
    /// <param name="score">The new score for the submission</param>
    /// <returns>A JSON object containing success = true/false</returns>
    public IActionResult GradeSubmission(string subject, int num, string season, int year, string category, string asgname, string uid, int score)
    {
            using (Team91LMSContext db = new Team91LMSContext())
            {
                //get the submission
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
                            select j4;
                if (query.Any())
                {
                    query.First().SubmissionScore = Convert.ToUInt16(score);
                    try
                    {
                        db.SaveChanges();
                        UpdateStudentsGrades(subject,num,season,year,uid);
                        return Json(new { success = true });
                    }
                    catch
                    {
                        return Json(new { success = false });
                    }
                }
            }
            return Json(new { success = false });
        }

        //goes through  the student with uid and updates their grade for this class
        private void UpdateStudentsGrades(string subject, int num, string season, int year, string uid)
        {
            var query = from p in db.Courses
                        join j1 in db.Classes
                        on p.CourseId equals j1.CourseId
                        where p.CourseNumber == num.ToString() && p.SubjectAbv == subject && j1.SemesterSeason == season && j1.SemesterYear == year
                        join j2 in db.EnrolledIn
                        on j1.ClassId equals j2.ClassId
                        join j3 in db.Students
                        on j2.UId equals j3.UId
                        where j3.UId == uid
                        select new
                        {
                            studentToGrade = j2,
                            maxScores = from i in db.Assignments
                                        join j4 in db.AssignmentCategories
                                        on i.Acid equals j4.Acid
                                        join j5 in db.Classes
                                        on j4.ClassId equals j5.ClassId
                                        join j7 in db.Courses
                                        on j5.CourseId equals j7.CourseId
                                        where j7.CourseNumber == num.ToString() && j7.SubjectAbv == subject && j5.SemesterSeason == season && j5.SemesterYear == year
                                        join j6 in db.EnrolledIn
                                        on j5.ClassId equals j6.ClassId
                                        where j6.UId == j3.UId
                                        group i by i.Acid into catagoryMaxes
                                        select new
                                        {
                                            catagory = catagoryMaxes.Key,
                                            TotalMaxes = catagoryMaxes.Sum(x => x.MaxPointValue)
                                        },
                            totalScores = from i in db.Courses
                                          join j4 in db.Classes
                                          on i.CourseId equals j4.CourseId
                                          where i.CourseNumber == num.ToString() && i.SubjectAbv == subject && j4.SemesterSeason == season && j4.SemesterYear == year
                                          join j5 in db.AssignmentCategories
                                          on j4.ClassId equals j5.ClassId
                                          join j6 in db.Assignments
                                          on j5.Acid equals j6.Acid
                                          join j7 in db.EnrolledIn
                                          on j4.ClassId equals j7.ClassId
                                          join j8 in db.Students
                                          on j7.UId equals j8.UId
                                          where j3.UId == j8.UId
                                          join j9 in db.Submissions
                                          on new { j6.Aid, j8.UId } equals new { j9.Aid, j9.UId }
                                          into getScore
                                          from t in getScore.DefaultIfEmpty()
                                          group t by j5.Acid into catagoryMaxes
                                          select new
                                          {
                                              catagory = catagoryMaxes.Key,
                                              TotalScores = catagoryMaxes.Sum(x => x == null ? 0 : x.SubmissionScore)
                                          },
                            gradeWeights = from i in db.Classes
                                           join j4 in db.Courses
                                           on i.CourseId equals j4.CourseId
                                           where j4.CourseNumber == num.ToString() && j4.SubjectAbv == subject && i.SemesterSeason == season && i.SemesterYear == year
                                           join j5 in db.AssignmentCategories
                                           on i.ClassId equals j5.ClassId
                                           join j6 in db.Assignments
                                           on j5.Acid equals j6.Acid
                                           group j5 by j5.Acid into weights
                                           select new
                                           {
                                               catagory = weights.First().Acid,
                                               Weight = weights.First().GradeWeight
                                           }
                        };
            //each array is the same assignment category at any index
            //query.First().studentGrade = "";
            foreach (var student in query)
            {
                //calculate letter grade
                uint sumGradeWeights = 0;
                //for each category, running sum += totalScores/maxScores * gradeWeights
                SortedDictionary<uint, long> calculateSum = new SortedDictionary<uint, long>();
                double runningSum = 0;
                foreach (var weight in student.gradeWeights)
                {
                    sumGradeWeights += weight.Weight;
                    calculateSum.Add(weight.catagory, weight.Weight);
                }
                foreach (var totalScore in student.totalScores)
                {
                    calculateSum[totalScore.catagory] *= totalScore.TotalScores;
                }
                foreach (var maxScore in student.maxScores)
                {
                    calculateSum[maxScore.catagory] /= maxScore.TotalMaxes;
                }
                foreach (var calculate in calculateSum)
                {
                    runningSum += calculate.Value;
                }
                double scalingFactor = 100.0 / Convert.ToInt32(sumGradeWeights);

                // running sum *= scalingFactor
                runningSum *= scalingFactor;
                //convert runningsum to letter grade (syllabus)
                if (runningSum >= 93)
                {
                    student.studentToGrade.Grade = "A";
                }
                else if (runningSum >= 90)
                {
                    student.studentToGrade.Grade = "A-";
                }
                else if (runningSum >= 87)
                {
                    student.studentToGrade.Grade = "B+";
                }
                else if (runningSum >= 83)
                {
                    student.studentToGrade.Grade = "B";
                }
                else if (runningSum >= 80)
                {
                    student.studentToGrade.Grade = "B-";
                }
                else if (runningSum >= 77)
                {
                    student.studentToGrade.Grade = "C+";
                }
                else if (runningSum >= 73)
                {
                    student.studentToGrade.Grade = "C";
                }
                else if (runningSum >= 70)
                {
                    student.studentToGrade.Grade = "C-";
                }
                else if (runningSum >= 67)
                {
                    student.studentToGrade.Grade = "D+";
                }
                else if (runningSum >= 63)
                {
                    student.studentToGrade.Grade = "D";
                }
                else if (runningSum >= 60)
                {
                    student.studentToGrade.Grade = "D-";
                }
                else
                {
                    student.studentToGrade.Grade = "E";
                }
            }

            try
            {
                db.SaveChanges();
            }
            catch
            {
            }
        }


        /// <summary>
        /// Returns a JSON array of the classes taught by the specified professor
        /// Each object in the array should have the following fields:
        /// "subject" - The subject abbreviation of the class (such as "CS")
        /// "number" - The course number (such as 5530)
        /// "name" - The course name
        /// "season" - The season part of the semester in which the class is taught
        /// "year" - The year part of the semester in which the class is taught
        /// </summary>
        /// <param name="uid">The professor's uid</param>
        /// <returns>The JSON array</returns>
        public IActionResult GetMyClasses(string uid)
    {
            using (Team91LMSContext db = new Team91LMSContext())
            {
                var query = from p in db.Classes
                            where p.UId == uid
                            join j1 in db.Courses
                            on p.CourseId equals j1.CourseId
                            select new { subject = j1.SubjectAbv, number = j1.CourseNumber, name = j1.CourseName, season = p.SemesterSeason, year = p.SemesterYear };
                return Json(query.ToArray());
            }
                //return Json(null);
    }

        //goes through all students in the course and update's their grades for this class
        public void UpdateStudentsGrades(string subject, int num, string season, int year)
        {
            using (Team91LMSContext db = new Team91LMSContext())
            {
                    var query = from p in db.Courses
                                join j1 in db.Classes
                                on p.CourseId equals j1.CourseId
                                where p.CourseNumber == num.ToString() && p.SubjectAbv == subject && j1.SemesterSeason == season && j1.SemesterYear == year
                                join j2 in db.EnrolledIn
                                on j1.ClassId equals j2.ClassId
                                join j3 in db.Students
                                on j2.UId equals j3.UId
                                select new
                                {
                                    studentToGrade = j2,
                                    maxScores = from i in db.Assignments
                                                join j4 in db.AssignmentCategories
                                                on i.Acid equals j4.Acid
                                                join j5 in db.Classes
                                                on j4.ClassId equals j5.ClassId
                                                join j7 in db.Courses
                                                on j5.CourseId equals j7.CourseId
                                                where j7.CourseNumber == num.ToString() && j7.SubjectAbv == subject && j5.SemesterSeason == season && j5.SemesterYear == year
                                                join j6 in db.EnrolledIn
                                                on j5.ClassId equals j6.ClassId
                                                where j6.UId == j3.UId
                                                group i by i.Acid into catagoryMaxes
                                                select new
                                                {
                                                    catagory = catagoryMaxes.Key,
                                                    TotalMaxes = catagoryMaxes.Sum(x => x.MaxPointValue)
                                                },
                                    totalScores = from i in db.Courses
                                                  join j4 in db.Classes
                                                  on i.CourseId equals j4.CourseId
                                                  where i.CourseNumber == num.ToString() && i.SubjectAbv == subject && j4.SemesterSeason == season && j4.SemesterYear == year
                                                  join j5 in db.AssignmentCategories
                                                  on j4.ClassId equals j5.ClassId
                                                  join j6 in db.Assignments
                                                  on j5.Acid equals j6.Acid
                                                  join j7 in db.EnrolledIn
                                                  on j4.ClassId equals j7.ClassId
                                                  join j8 in db.Students
                                                  on j7.UId equals j8.UId
                                                  where j3.UId == j8.UId
                                                  join j9 in db.Submissions
                                                  on new { j6.Aid, j8.UId } equals new { j9.Aid, j9.UId }
                                                  into getScore
                                                  from t in getScore.DefaultIfEmpty()
                                                  group t by j5.Acid into catagoryMaxes
                                                  select new
                                                  {
                                                      catagory = catagoryMaxes.Key,
                                                      TotalScores = catagoryMaxes.Sum(x => x == null ? 0 : x.SubmissionScore)
                                                  },
                                    gradeWeights = from i in db.Classes
                                                   join j4 in db.Courses
                                                   on i.CourseId equals j4.CourseId
                                                   where j4.CourseNumber == num.ToString() && j4.SubjectAbv == subject && i.SemesterSeason == season && i.SemesterYear == year
                                                   join j5 in db.AssignmentCategories
                                                   on i.ClassId equals j5.ClassId
                                                   join j6 in db.Assignments
                                                   on j5.Acid equals j6.Acid
                                                   group j5 by j5.Acid into weights
                                                   select new
                                                   {
                                                       catagory = weights.First().Acid,
                                                       Weight = weights.First().GradeWeight
                                                   }

                                };

                   
                //each array is the same assignment category at any index
                //query.First().studentGrade = "";
                foreach (var student in query)
                {
                    //calculate letter grade
                    uint sumGradeWeights = 0;
                    //for each category, running sum += totalScores/maxScores * gradeWeights
                    SortedDictionary<uint, long> calculateSum = new SortedDictionary<uint, long>();
                    double runningSum = 0;
                    foreach (var weight in student.gradeWeights)
                    {
                        sumGradeWeights += weight.Weight;
                        calculateSum.Add(weight.catagory, weight.Weight);
                    }
                    foreach (var totalScore in student.totalScores)
                    {
                        calculateSum[totalScore.catagory] *=  totalScore.TotalScores;
                    }
                    foreach (var maxScore in student.maxScores)
                    {
                        calculateSum[maxScore.catagory] /= maxScore.TotalMaxes;
                    }
                    foreach (var calculate in calculateSum)
                    {
                        runningSum += calculate.Value;
                    }
                    double scalingFactor = 100.0 / Convert.ToInt32(sumGradeWeights);

                    // running sum *= scalingFactor
                    runningSum *= scalingFactor;
                    //convert runningsum to letter grade (syllabus)
                    if(runningSum >= 93)
                    {
                        student.studentToGrade.Grade = "A";
                    }
                    else if (runningSum >= 90)
                    {
                        student.studentToGrade.Grade = "A-";
                    }
                    else if (runningSum >= 87)
                    {
                        student.studentToGrade.Grade = "B+";
                    }
                    else if (runningSum >= 83)
                    {
                        student.studentToGrade.Grade = "B";
                    }
                    else if (runningSum >= 80)
                    {
                        student.studentToGrade.Grade = "B-";
                    }
                    else if (runningSum >= 77)
                    {
                        student.studentToGrade.Grade = "C+";
                    }
                    else if (runningSum >= 73)
                    {
                        student.studentToGrade.Grade = "C";
                    }
                    else if (runningSum >= 70)
                    {
                        student.studentToGrade.Grade = "C-";
                    }
                    else if (runningSum >= 67)
                    {
                        student.studentToGrade.Grade = "D+";
                    }
                    else if (runningSum >= 63)
                    {
                        student.studentToGrade.Grade = "D";
                    }
                    else if (runningSum >= 60)
                    {
                        student.studentToGrade.Grade = "D-";
                    }
                    else
                    {
                        student.studentToGrade.Grade = "E";
                    }
                }

                try
                {
                    db.SaveChanges();
                }
                catch
                {

                }
             }
        }



    /*******End code to modify********/

  }
}