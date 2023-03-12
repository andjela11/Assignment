using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class EmployeeController : BaseApiController
    {
        private readonly DataContext _context;
        public EmployeeController(DataContext context)
        {
            _context = context;
        }

        [HttpPost("addEmployee")]
        public async Task<ActionResult> AddEmployee(Employee employee)
        {
            try
            {
                await _context.Employees.AddAsync(employee);
                await _context.SaveChangesAsync();
                return Ok(employee);

            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet("GetEmployee")]
        public ActionResult<Employee> GetEmployee(int employeeId)
        {
            try
            {
                var employee = _context.Employees.Find(employeeId);
                if (employee == null) return BadRequest("Employee doesn't exist!");

                return Ok(employee);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet("GetAllEmployee")]
        public async Task<ActionResult> GetAllEmployee()
        {
            try
            {
                var employee = await _context.Employees.ToListAsync();
                if (employee == null) return BadRequest("No employees");

                if(employee.Count() == 0) return Ok("No emoployees");

                return Ok(employee);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet("GetEmoloyeeName")]
        public async Task<ActionResult<List<Employee>>> GetEmployee(string name = "", string lastName ="")
        {
            try{
                if(name == "" && lastName == "")
                {
                    return BadRequest("Enter at least one parameter for search");
                }
                else 
                {
                    var employees = await ReturnEmployees(name, lastName);
                    if (employees == null || employees.Count == 0) 
                            return BadRequest("No employees were found");
                    return Ok(employees);
                }

            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        private async Task<List<Employee>> ReturnEmployees(string name, string lastName)
        {
            List<Employee>? employees = null;
            if(lastName == "")
            {
                employees = await _context.Employees.Where(x => x.Name == name).ToListAsync();
            }
            else if (name == "")
            {
                employees = await _context.Employees.Where(x => x.LastName == lastName).ToListAsync();
            }
            else
            {
                employees = await _context.Employees.Where(x => x.Name == name && x.LastName == lastName).ToListAsync();
            }
            return employees;
        }

        [HttpDelete("DeleteEmployee")]
        public async Task<ActionResult> DeleteEmployee(int employeeId)
        {
            try
            {
                var employee = _context.Employees.Find(employeeId);
                if (employee == null) return BadRequest("Employee doesn't exist");


                var tasks = _context.Tasks.Where(x => x.Assignee == employee).ToList();
                
                if(tasks != null && tasks.Count() > 0)
                {
                    foreach (var task in tasks!)
                    {
                        task.Assignee= null;
                    }
                }
                _context.Employees.Remove(employee);
                await _context.SaveChangesAsync();
                return Ok(tasks);
            }
            catch(Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        
        // UPDATE 

        [HttpPut("UpdateEmployee")]
        public async Task<ActionResult> UpdateEmployee(int empId, Employee employee)
        {
            try{
                var empl = await _context.Employees.FindAsync(empId);
                if(empl == null) return BadRequest("Employee doesn't exist!");

                empl.Name= employee.Name == "" ? empl.Name : employee.Name;
                empl.LastName=employee.LastName == "" ? empl.LastName : employee.LastName;
                empl.DateOfBirth=employee.DateOfBirth == default(DateTime) ? empl.DateOfBirth : employee.DateOfBirth;
                empl.Email=employee.Email == "" ? empl.Email : employee.Email;
                empl.MonthlySalary=employee.MonthlySalary;
                empl.PhoneNumber=employee.PhoneNumber == "" ? empl.PhoneNumber : employee.PhoneNumber;;

                _context.Employees.Update(empl);
                await _context.SaveChangesAsync();
                return Ok(empl);

            }
            catch (Exception e) 
            {
                return BadRequest(e.Message);
            }
        }
    }
}