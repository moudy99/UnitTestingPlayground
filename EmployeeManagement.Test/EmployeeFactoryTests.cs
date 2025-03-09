using EmployeeManagement.Business;
using EmployeeManagement.DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeManagement.Test
{
    public class EmployeeFactoryTests
    {
        [Fact]
        public void CreateEmployee_WithOnlyFirstnameAndLastname_ReturnsInternalEmployeeWith2500Salay()
        {
            // Arrange
            var SUT = new EmployeeFactory();

            // Act
            var employee = (InternalEmployee)SUT.CreateEmployee("Moudy", "Rasmy");
            
            // Assert

            Assert.Equal(2500,employee.Salary);
        }
    }
}
