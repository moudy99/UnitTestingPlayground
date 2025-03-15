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

    }
}
