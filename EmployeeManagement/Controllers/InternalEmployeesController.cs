using AutoMapper;
using EmployeeManagement.Business;
using EmployeeManagement.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeManagement.Controllers
{
    [Route("api/internalemployees")]
    [ApiController]
    public class InternalEmployeesController : ControllerBase
    {
        private readonly IEmployeeService _employeeService;
        private readonly HttpClient _httpClient;
        private readonly IMapper _mapper;
        private readonly ILogger<InternalEmployeesController> _logger;

        public InternalEmployeesController(
            IEmployeeService employeeService,
            HttpClient httpClient,
            IMapper mapper,
            ILogger<InternalEmployeesController> logger)
        {
            _employeeService = employeeService;
            _httpClient = httpClient;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<InternalEmployeeDto>>> GetInternalEmployees()
        {
            _logger.LogInformation("Fetching all internal employees");

            try
            {
                var internalEmployees = await _employeeService.FetchInternalEmployeesAsync();

                _logger.LogInformation("Retrieved {Count} internal employees", internalEmployees.Count());

                var internalEmployeeDtos = internalEmployees.Select(e => new InternalEmployeeDto()
                {
                    Id = e.Id,
                    FirstName = e.FirstName,
                    LastName = e.LastName,
                    Salary = e.Salary,
                    SuggestedBonus = e.SuggestedBonus,
                    YearsInService = e.YearsInService
                });

                return Ok(internalEmployeeDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving internal employees");
                return StatusCode(500, "An error occurred while retrieving employees.");
            }
        }

        [HttpGet("{employeeId}", Name = "GetInternalEmployee")]
        public async Task<ActionResult<InternalEmployeeDto>> GetInternalEmployee(Guid? employeeId)
        {
            _logger.LogInformation("GetInternalEmployee request received with employeeId: {EmployeeId}", employeeId);
            _logger.LogError("+++++++++++==============================================");
            if (!employeeId.HasValue)
            {
                _logger.LogWarning("GetInternalEmployee request received without employeeId");
                return BadRequest("Employee ID is required.");
            }

            _logger.LogInformation("Fetching employee with ID {EmployeeId}", employeeId.Value);

            var internalEmployee = await _employeeService.FetchInternalEmployeeAsync(employeeId.Value);
            if (internalEmployee == null)
            {
                _logger.LogWarning("Employee with ID {EmployeeId} not found", employeeId.Value);
                return NotFound();
            }

            return Ok(_mapper.Map<InternalEmployeeDto>(internalEmployee));
        }

        [HttpPost]
        public async Task<ActionResult<InternalEmployeeDto>> CreateInternalEmployee(
            InternalEmployeeForCreationDto internalEmployeeForCreation)
        {
            _logger.LogInformation("Creating new internal employee: {FirstName} {LastName}",
                internalEmployeeForCreation.FirstName, internalEmployeeForCreation.LastName);

            try
            {
                var internalEmployee = await _employeeService.CreateInternalEmployeeAsync(
                    internalEmployeeForCreation.FirstName, internalEmployeeForCreation.LastName);

                await _employeeService.AddInternalEmployeeAsync(internalEmployee);

                _logger.LogInformation("Successfully created internal employee with ID {EmployeeId}", internalEmployee.Id);

                return CreatedAtAction(nameof(GetInternalEmployee), new { employeeId = internalEmployee.Id },
                    _mapper.Map<InternalEmployeeDto>(internalEmployee));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating internal employee");
                return StatusCode(500, "An error occurred while creating the employee.");
            }
        }
    }
}
