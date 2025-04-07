using EmployeeManagement.Business;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeManagement.Test
{
    public class ExceptionsTests
    {
        [Fact]
        public async Task TestThrow_testing()
        {
           await Assert.ThrowsAnyAsync<Exception>(() => throw new InvalidEnumArgumentException("Invalid"));
        }

        [Fact]
        public  void CreateInternalEmployee_TryToCreateInternalEmployeeWihtoutFirstName_ThrowAnException()
        {
            var sut = new EmployeeFactory();

            Assert.ThrowsAny<Exception>(() =>
             {
                 sut.CreateEmployee("", "Doe");
             });


        }
    }
}
