using HotChocolate;
using HotChocolate.Data;
using HotChocolate.Types;
using System.Linq;

namespace ContosoUniversity
{
    public class Query
    {
        [UseFirstOrDefault]
        [UseProjection]
        public IQueryable<Student> GetStudentById([Service] SchoolContext context, int studentId) =>
            context.Students.Where(t => t.Id == studentId);

        [UsePaging]
        [UseProjection]
        [UseFiltering]
        [UseSorting]
        public IQueryable<Student> GetStudents([Service] SchoolContext context) =>
            context.Students;

        [UsePaging]
        [UseProjection]
        [UseFiltering]
        [UseSorting]
        public IQueryable<Course> GetCourses([Service] SchoolContext context) =>
            context.Courses;
    }
}