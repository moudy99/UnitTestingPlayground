using EmployeeManagement.Business;
using EmployeeManagement.DataAccess.Entities;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeManagement.Test
{
    public class EmployeeFactoryTests
    {
        private EmployeeFactory _sub;
        [Fact]
        public void CreateEmployee_WithOnlyFirstnameAndLastname_ReturnsInternalEmployeeWith2500Salay()
        {
            // Arrange
             _sub= new EmployeeFactory();

            // Act
            var employee = (InternalEmployee)_sub.CreateEmployee("Moudy", "Rasmy");
            
            // Assert

            Assert.Equal(2500,employee.Salary);
        }
    }
}
