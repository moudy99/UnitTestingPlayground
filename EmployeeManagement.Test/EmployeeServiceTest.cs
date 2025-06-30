using EmployeeManagement.Business;
using EmployeeManagement.Business.Exceptions;
using EmployeeManagement.Test.Services;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeManagement.Test
{
    public class EmployeeServiceTest
    {
  
        [Fact]
        public void CreateInternalEmployee_CreationOfInternalEmployee_MustAttenFirstObligatoryCours()
        {
            // Arrage
            var DummyTestRepo = new EmployeeManagementTestDataRepository();
            var employeeService  = new EmployeeService(DummyTestRepo,new EmployeeFactory());

            // Act
            var Emp = employeeService.CreateInternalEmployee("Moudy", "Rasmy");
            var Cours = DummyTestRepo.GetCourse(new Guid("37e03ca7-c730-4351-834c-b66f280cdb01"));

            // Assert

            Assert.Contains(Cours,Emp.AttendedCourses);
        }

        [Fact]
        public void CreateInternalEmployee_CreationOfInternalEmployee_AllAttendedCourseAreNotNew()
        {
            var Repo = new EmployeeManagementTestDataRepository();
            var empService = new EmployeeService(
                repository:Repo,employeeFactory:new EmployeeFactory());

            var employee = empService.CreateInternalEmployee("MOUDY", "RASMY");

            //foreach(var cr  in employee.AttendedCourses)
            //{
            //    Assert.False(cr.IsNew);
            //}

            Assert.All(employee.AttendedCourses,(cr)=>Assert.False(cr.IsNew));
        }
    
        [Fact]
        public async Task CreateInternalEmployee_CreationOfInternalEmployee_MustAttenAllObligatoryCourses_Async()
        {
            // Arrage
            var DummyTestRepo = new EmployeeManagementTestDataRepository();
            var employeeService  = new EmployeeService(DummyTestRepo,new EmployeeFactory());
            var Cours = await DummyTestRepo.GetCoursesAsync(
                Guid.Parse("37e03ca7-c730-4351-834c-b66f280cdb01"),
                     Guid.Parse("1fd115cf-f44c-4982-86bc-a8fe2e4ff83e"));


            // Act
            var Emp = await employeeService.CreateInternalEmployeeAsync("Moudy", "Rasmy");
            // Assert

            Assert.Equal(Cours,Emp.AttendedCourses);
        }


        [Fact]
        public async Task CreateInternalEmployee_RaiseLowMinimumRange_ThrowInvalidRasieException_Async()
        {
            // Arrage
            var DummyTestRepo = new EmployeeManagementTestDataRepository();
            var employeeService = new EmployeeService(DummyTestRepo, new EmployeeFactory());
            var Emp = await employeeService.CreateInternalEmployeeAsync("Moudy", "Rasmy");


            // Act & Assert

            await Assert.ThrowsAsync<EmployeeInvalidRaiseException>(
                async () =>
             await employeeService.GiveRaiseAsync(employee: Emp, raise: 33)
                );
        }

    }
}
