using API.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class ShowTasksController : BaseApiController
    {

        private readonly DataContext _context;
        public ShowTasksController(DataContext context)
        {
            _context = context;
        }

        [HttpGet("GetAllTasks")]
        public async Task<ActionResult> GetAllTasks()
        {
            try
            {
                return Ok(await _context.Tasks.Include(x => x.Assignee).ToListAsync());
            }
            catch(Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet("GetTaskWithId")]
        public async Task<ActionResult> GetTaskWithId(int taskId)
        {
            try
            {
                var task = await _context.Tasks
                            .Include(x => x.Assignee)
                            .Where(x => x.Id == taskId)
                            .FirstOrDefaultAsync();
                if(task == null) return BadRequest("Task doesn't exist");
                return Ok(new {
                    taskId,
                    task.Title,
                    task.Description,
                    task.DueDate,
                    EmployeeName = task.Assignee!.Name,
                    EmployeeSurname = task.Assignee.LastName
                });
            }
            catch(Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet("GetTaskEmployeeId")]
        public async Task<ActionResult> GetTaskForEmoloyee(int employeeId)
        {
            try
            {
                var empl = await _context.Employees.FindAsync(employeeId);
                
                if(empl == null) return BadRequest("Employee doesn't exist");

                var task = await _context.Tasks
                        .Include(x => x.Assignee)
                        .Where(x => x.Assignee!.Id == employeeId)
                        .ToListAsync();
                if(task == null) return BadRequest("Task doesn't exist");
                
                return Ok(task);
            }
            catch(Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet("TaskWithEarliestDueDate")]
        public async Task<ActionResult> TaskWithEarliestDueDate()
        {
            try{
                var task = await _context.Tasks
                                .Where(x => (x.DueDate > DateTime.Now && x.DueDate < DateTime.Now.AddDays(7)))
                                .Include(x => x.Assignee)
                                .ToListAsync();

                if(task == null) return BadRequest("No tasks");

                return Ok(task);
            }
            catch(Exception e)
            { 
                return BadRequest(e.Message);
            }
        }
        
        [HttpGet("GetUnassignedTasks")]
        public async Task<ActionResult> GetUnassignedTasks()
        {
            try{

                var tasks = await _context.Tasks.Where(x => x.Assignee == null).ToListAsync();

                //if(tasks == null) return BadRequest("All tasks are assigned");

                if(tasks.Count() == 0) return Ok("All tasks are assigned");

                return Ok(tasks);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}