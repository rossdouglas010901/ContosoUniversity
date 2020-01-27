using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ContosoUniversity.Data;
using ContosoUniversity.Models;
using ContosoUniversity.Models.SchoolViewModels;

namespace ContosoUniversity.Controllers
{
    public class InstructorController : Controller
    {
        private readonly SchoolContext _context;

        public InstructorController(SchoolContext context)
        {
            _context = context;
        }

        // GET: Instructor

        //public async Task<IActionResult> Index()
        public async Task<IActionResult> Index(int? id, int? CourseID)
        {
            var viewModel = new InstrucotrIndexData();
            viewModel.Instructors = await _context.Instructors
                .Include(i => i.OfficeAssignment)
                .Include(i => i.Courses)
                    .ThenInclude(i => i.Course)
                    .ThenInclude(i => i.Department)
                .ToListAsync();

            if (id != null)
            {
                Instructor instructor = viewModel.Instructors.Where(
                i => i.ID == id.Value).SingleOrDefault();
                if (instructor == null)
                {
                    return NotFound();
                }
                viewModel.Courses = instructor.Courses.Select(s => s.Course);

                ViewData["InstructorName"] = instructor.FullName;

                ViewData["InstrutorID"] = id.Value;

            }

            if (CourseID != null)
            {

                _context.Enrollments.Include(i => i.Student)
                    .Where(c=>c.CourseID == CourseID.Value).Load();

                var enrollemnts = viewModel.Courses
                    .Where(x => x.CourseID == CourseID).SingleOrDefault();

                if (enrollemnts == null)
                {
                    return NotFound();
                }

                viewModel.Enrollments = enrollemnts.Enrollments;
                ViewData["CourseID"] = CourseID.Value;

            }

                return View(viewModel);
        }

        //public async Task<IActionResult> Index()
        //{
        //    return View(await _context.Instructors.ToListAsync());
        //}

        // GET: Instructor/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var instructor = await _context.Instructors
                .FirstOrDefaultAsync(m => m.ID == id);
            if (instructor == null)
            {
                return NotFound();
            }

            return View(instructor);
        }

        // GET: Instructor/Create
        public IActionResult Create()
        {
            var instructor = new Instructor();
            instructor.Courses = new List<CourseAssignment>();
            PopulateAssignedCourseDate(instructor);
            return View();
        }

        // POST: Instructor/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("HireDate,OfficeAssignment,FirstName,LastName,Email,Address,City,Province,PostalCode")] Instructor instructor, string[] selectedCourse)
        {

            if (selectedCourse != null)
            {
                instructor.Courses = new List<CourseAssignment> ();
                foreach(var course in selectedCourse)
                {
                    var courseToAdd = new CourseAssignment
                    {
                        InstructorID = instructor.ID,
                        CourseID = int.Parse(course)
                    };
                    instructor.Courses.Add(courseToAdd);
                }
            }

            if (ModelState.IsValid)
            {
                _context.Add(instructor);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(instructor);
        }

        // GET: Instructor/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            //var instructor = await _context.Instructors.FindAsync(id);
            var instructor = await _context.Instructors
                .Include(i => i.OfficeAssignment)
                .Include(i => i.Courses)
                .SingleOrDefaultAsync(i => i.ID == id.Value);
            if (instructor == null)
            {
                return NotFound();
            }

            PopulateAssignedCourseDate(instructor);

            return View(instructor);
        }

        private void PopulateAssignedCourseDate(Instructor instructor)
        {
            var allCourses = _context.Courses;
            var instructorCourses = new HashSet<int>(instructor.Courses.Select(c => c.CourseID));
            var viewModel = new List<AssignedCourseData>();
            foreach(var course in allCourses)
            {
                viewModel.Add(new AssignedCourseData
                {
                    CourseID = course.CourseID,
                    Title = course.Title,
                    Assigned = instructorCourses.Contains(course.CourseID)
                });

            }
            ViewData["Courses"] = viewModel;

        }

        // POST: Instructor/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(int id, [Bind("HireDate,ID,FirstName,LastName,Email,Address,City,Province,PostalCode")] Instructor instructor)
        public async Task<IActionResult> Edit(int? id,string[] selectedCourse)

        {
            if (id==null)
            {
                return NotFound();
            }

            var instructorToUpdate = await _context.Instructors
                .Include(i => i.OfficeAssignment)
                .Include(i=>i.Courses)
                .ThenInclude(i=>i.Course)
                .SingleOrDefaultAsync(i => i.ID == id.Value);
            if(await TryUpdateModelAsync<Instructor>(
                instructorToUpdate,"",
                i=>i.FirstName,i=>i.LastName,i=>i.Address,
                i=>i.City, i => i.Province, i => i.PostalCode,
                i => i.Email, i => i.HireDate,
                i => i.OfficeAssignment)
                )
            {
                if(string.IsNullOrWhiteSpace(instructorToUpdate.OfficeAssignment.Location))
                {
                    instructorToUpdate.OfficeAssignment = null;
                }
                UpdateInstructorCourses(selectedCourse, instructorToUpdate);
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateException)
                {
                    ModelState.AddModelError("", "Unable to Save changes");
                }
                return RedirectToAction(nameof(Index));
            }
            return View(instructorToUpdate);
        }

        //Part 2:  Update Related Data (Instructor Assigned Courses)
        private void UpdateInstructorCourses(string[] selectedCourse, Instructor instructorToUpdate)
        {
            if (selectedCourse == null)
            {
                //If no checkboxes were selected, initialize the Courses navigation property
                //with an empty collection and return
                instructorToUpdate.Courses = new List<CourseAssignment>();
                return;
            }

            //To facilitate efficient lookups, 2 collections will be stored in HashSet objects
            //: selectedCourseHS ->  selected course (hashset of checkboxe selections)
            //: instructorCourses -> instructor courses (hashset of courses assigned to instructor)
            var selectedCourseHS = new HashSet<string>(selectedCourse);
            var instructorCourses = new HashSet<int>
                (instructorToUpdate.Courses.Select(c => c.Course.CourseID));

            //Loop through all courses in the database and check each course against the ones
            //currently assigned to the instructor versus the ones that were selected in the
            //view
            foreach (var course in _context.Courses)//Loop all courses
            {
                //CONDITION 1:
                //If the checkbox for a course was selected but the course isn't in the 
                //Instructor.Courses navigation property, the course is added to the collection
                //in the navigation property
                if (selectedCourseHS.Contains(course.CourseID.ToString()))
                {
                    if (!instructorCourses.Contains(course.CourseID))
                    {
                        instructorToUpdate.Courses.Add(new CourseAssignment
                        {
                            InstructorID = instructorToUpdate.ID,
                            CourseID = course.CourseID
                        });
                    }
                }
                //CONDITION 2:
                //If the check box for a course wasn't selected, but the course is in the 
                //Instructor.Courses navigation property, the course is removed 
                //from the navigation property.
                else
                {
                    if (instructorCourses.Contains(course.CourseID))
                    {
                        CourseAssignment courseToRemove =
                            instructorToUpdate.Courses
                            .SingleOrDefault(i => i.CourseID == course.CourseID);
                        _context.Remove(courseToRemove);
                    }
                }

            }//end foreach
        }

        // GET: Instructor/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var instructor = await _context.Instructors
                .FirstOrDefaultAsync(m => m.ID == id);
            if (instructor == null)
            {
                return NotFound();
            }

            return View(instructor);
        }

        // POST: Instructor/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var instructor = await _context.Instructors.FindAsync(id);
            try
            {
                _context.Instructors.Remove(instructor);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException ex)
            {
                if (((Microsoft.Data.SqlClient.SqlException)ex.InnerException).Number == 547)
                {
                    ModelState.AddModelError("", "Unable to delete instructor due to related records");
                }
                else
                {
                    ModelState.AddModelError("", "Unable to delete instructor due to a system error");
                }
            }
            return View(instructor);
        }

        private bool InstructorExists(int id)
        {
            return _context.Instructors.Any(e => e.ID == id);
        }
    }
}
