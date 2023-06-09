using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class CompletedTaskController : BaseApiController
    {
        private readonly DataContext _context;
        public CompletedTaskController(DataContext context)
        {
            _context = context;
        }

        [HttpPost("AddCompletedTask")]
        public async Task<ActionResult> AddCompletedTask(int taskId, DateTime dateCompleted)
        {
            try
            {
                var task = _context.Tasks.Include(X => X.Assignee).Where(x => x.Id == taskId).FirstOrDefault();
                if (task == null) return BadRequest("Task doesn't exist");

                if(task.Assignee == null) return NotFound("This task doesn't have an assignee. Please provide this information by changing the assigne for this task");

                Completed completedTask = new Completed
                {
                    Title = task.Title,
                    Description = task.Description,
                    Assignee = task.Assignee,
                    DateCompleted = dateCompleted
                };

                await _context.CompletedTasks.AddAsync(completedTask);

                _context.Tasks.Remove(task);

                await _context.SaveChangesAsync();
                return Ok(completedTask);

            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet("GetAllCompletedTasks")]
        public async Task<ActionResult> GetAllCompletedTasks()
        {
            try
            {
                var completedtasks = await _context.CompletedTasks
                                .Include(x => x.Assignee)
                                .ToListAsync();

                if(completedtasks.Count() == 0) return Ok("No completed tasks yet");

                return Ok(completedtasks);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet("TopFiveEmployees")]
        public ActionResult TopFiveEmployees()
        {
            try
            {
                var employee = _context.Employees
                            .Include(x => x.Completed)
                            .Where(x => x.Completed!.Count() > 0);


                if (employee == null) return BadRequest("No Employees");
                Dictionary<string, int> empl = new Dictionary<string, int>();

                foreach (Employee e in employee)
                {
                    var c = e.Completed!.Where(x => x.DateCompleted.Month == DateTime.Now.Month - 1).Count();
                    string fullname = string.Concat(e.Name + " ", e.LastName);
                    empl.Add(fullname, c);

                }
                empl = empl.OrderByDescending(x => x.Value).ToDictionary(x => x.Key, x => x.Value);
                var topFive = empl.Take(5);

                if(topFive.Count() == 0) return Ok("No information");

                return Ok(topFive);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet("EmployeeWithMostCompletedTasks")]
        public ActionResult EmployeeWithMostCompletedTasks()
        {
            try
            {
                var employee = _context.Employees
                        .Include(x => x.Completed)
                        .Where(x => (x.Completed != null && x.Completed.Count() > 0));
                        
                if (employee == null || employee.Count() == 0) return BadRequest("No Employees");
                Dictionary<int, int> empl = new Dictionary<int, int>();

                foreach (Employee e in employee)
                {
                    var c = e.Completed!.Count();
                    empl.Add(e.Id, c);
                }
                empl = empl.OrderByDescending(x => x.Value).ToDictionary(x => x.Key, x => x.Value);
                List<Employee> mostefficientempls = new List<Employee>();
                int max = empl.ElementAt(0).Value;
                foreach(var e in empl)
                {
                    if (e.Value < max) empl.Remove(e.Key);
                    else
                    mostefficientempls.Add(employee.Where(x => x.Id == e.Key).First());
                }

                List<Object> list = new List<Object>();
                foreach (var e in mostefficientempls)
                {
                    list.Add( new {
                        Name=e.Name,
                        LastName=e.LastName,
                        e.Email,
                        TasksCompleted = max
                    });
                }
                
                return Ok(list);
            }
            catch(Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpDelete("DeleteCompletedTask")]
        public async Task<ActionResult> DeleteCompletedTask(int completedTaskId)
        {
            try
            {
                var completedTask = await _context.CompletedTasks.FindAsync(completedTaskId);

                if (completedTask == null) return BadRequest("Task isn't completed yet");

                _context.Remove<Completed>(completedTask);

                await _context.SaveChangesAsync();

                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPut("UpdateCompletedTask")]
        public async Task<ActionResult> UpdateCompleetedTask(Completed completedTask)
        {
            try
            {
                var task = _context.CompletedTasks.Find(completedTask.Id);

                if(task == null) return BadRequest("Task doesn't exist");

                task.Title=completedTask.Title == "" ? 
                                    task.Title : completedTask.Title;
                task.Description=completedTask.Description == "" ? 
                                    task.Description : completedTask.Description;
                task.DateCompleted=completedTask.DateCompleted == null ? 
                                    task.DateCompleted : completedTask.DateCompleted;
                task.Assignee=completedTask.Assignee == null ? 
                                    task.Assignee : completedTask.Assignee;


                _context.CompletedTasks.Update(task);
                await _context.SaveChangesAsync();

                return Ok(task);
                
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

        }
    }
}