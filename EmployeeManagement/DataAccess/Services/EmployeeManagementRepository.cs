using EmployeeManagement.DataAccess.DbContexts;
using EmployeeManagement.DataAccess.Entities;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace EmployeeManagement.DataAccess.Services
{
    public class EmployeeManagementRepository : IEmployeeManagementRepository
    {
        private readonly EmployeeDbContext _context;
        private readonly IMemoryCache memoryCache;

        public EmployeeManagementRepository(EmployeeDbContext context , IMemoryCache memoryCache)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            this.memoryCache = memoryCache;
        }

        public async Task<IEnumerable<InternalEmployee>> GetInternalEmployeesAsync()
        {
            if(memoryCache.TryGetValue("InternalEmployees", out IEnumerable<InternalEmployee> internalEmployees))
            {
                return internalEmployees;
            }
            else
            {

            var result = await _context.InternalEmployees
                .Include(e => e.AttendedCourses)
                .ToListAsync();
                memoryCache.Set("InternalEmployees", result);
                return result;
            }
        }

        public async Task<InternalEmployee?> GetInternalEmployeeAsync(Guid employeeId)
        {
            return await _context.InternalEmployees
                .Include(e => e.AttendedCourses)
                .FirstOrDefaultAsync(e => e.Id == employeeId);
        }

        public InternalEmployee? GetInternalEmployee(Guid employeeId)
        {
            return _context.InternalEmployees
                .Include(e => e.AttendedCourses)
                .FirstOrDefault(e => e.Id == employeeId);
        }

        public async Task<Course?> GetCourseAsync(Guid courseId)
        {
            return await _context.Courses.FirstOrDefaultAsync(e => e.Id == courseId);
        }

        public Course? GetCourse(Guid courseId)
        {
            return _context.Courses.FirstOrDefault(e => e.Id == courseId);
        }

        public List<Course> GetCourses(params Guid[] courseIds)
        {
            List<Course> coursesToReturn = new();
            foreach (var courseId in courseIds)
            {
                var course = GetCourse(courseId);
                if (course != null)
                {
                    coursesToReturn.Add(course);
                }
            }
            return coursesToReturn;
        }

        public async Task<List<Course>> GetCoursesAsync(params Guid[] courseIds)
        {
            List<Course> coursesToReturn = new();
            foreach (var courseId in courseIds)
            {
                var course = await GetCourseAsync(courseId);
                if (course != null)
                {
                    coursesToReturn.Add(course);
                }
            }
            return coursesToReturn;
        }

        public void AddInternalEmployee(InternalEmployee internalEmployee)
        {
            _context.InternalEmployees.Add(internalEmployee);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public bool DeleteInternelEmployee(Guid employeeId)
        {
            var result =  _context.InternalEmployees.Where(e => e.Id == employeeId).ExecuteDelete();
            if (result == 1) return true;
            return false;
        }
    }
}
