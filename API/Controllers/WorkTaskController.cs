using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class WorkTaskController : BaseApiController
    {
        private readonly DataContext _context;

        public WorkTaskController(DataContext context) 
        {
            _context= context;

        }

        [HttpPost("AddTaskWithEmployee")]
        public async Task<ActionResult> AddTaskWithEmployee(WorkTask task, int employeeId)
        {
            try
            {
                var employee = _context.Employees.Find(employeeId);
                if (employee == null) return BadRequest("Employee doen't exist");

                if(task.DueDate == default(DateTime)) return BadRequest("Provide due date for the task");
                task.Assignee=employee;
                await _context.Tasks.AddAsync(task);
                await _context.SaveChangesAsync();
                return Ok(task);
            }
            catch(Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        [HttpPost("AddTask")]
        public async Task<ActionResult> AddTask(string title, string description, DateTime dueDate)
        {
            try
            {
                WorkTask task = new WorkTask{
                    Title=title == null || title == "" ? throw new Exception() : title,
                    Description=description == null || title == "" ? throw new Exception() : description,
                    DueDate=dueDate == default(DateTime) ? throw new Exception() : dueDate
                };
                task.Assignee=null;
                await _context.Tasks.AddAsync(task);
                await _context.SaveChangesAsync();
                return Ok(task);
            }
            catch(Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpDelete("DeleteTask")]
        public async Task<ActionResult> DeleteTask(int taskId)
        {
            try
            {
                var task = _context.Tasks.Find(taskId);
                if(task == null) return BadRequest("Task doesn't exist");
                _context.Tasks.Remove(task);
                await _context.SaveChangesAsync();
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        


        [HttpPatch("ChangeTask")]
        public async Task<ActionResult> ChangeTask(int taskid, string title = "", string description = "")
        {
            try{
                var t = _context.Tasks.Find(taskid);
                if (t==null) return BadRequest("Task doesn't exist");
                t.Title = title == "" ? t.Title : title;
                t.Description = description == "" ? t.Description : description;
                
                _context.Tasks.Update(t);
                await _context.SaveChangesAsync();
                return Ok(t);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPatch("UpdateDueDate")]
        public async Task<ActionResult> UpdateDueDate(int taskid, DateTime date)
        {
            try{
                var t = _context.Tasks.Find(taskid);
                if (t==null) return BadRequest("Task doesn't exist");
            
                t.DueDate = date;
                
                _context.Tasks.Update(t);
                await _context.SaveChangesAsync();
                return Ok(t);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        [HttpPatch("ChangeAssignee")]
        public async Task<ActionResult> ChangeAssignee(int taskid, int employeeId)
        {
            try{
                var task = _context.Tasks.Find(taskid);
                var employee = _context.Employees.Find(employeeId);
                if (task==null) return BadRequest("Task doesn't exist");
                if (employee==null) return BadRequest("Employee doesn't exist");

            
                task.Assignee = employee;
                
                _context.Tasks.Update(task);
                await _context.SaveChangesAsync();
                return Ok(task);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPut("UpdateTask")]
        public async Task<ActionResult> UpdateTask(int taskid, WorkTask newTask)
        {
            try{
                var task = _context.Tasks.Find(taskid);
                if (task==null) return BadRequest("Task doesn't exist");
            
                task.Title = newTask.Title == "" ? task.Title : newTask.Title;
                task.Description = newTask.Description == "" ? task.Description : newTask.Description;
                task.DueDate = newTask.DueDate == default(DateTime) ? task.DueDate : newTask.DueDate;
                task.Assignee = newTask.Assignee == null ? task.Assignee : newTask.Assignee;
                
                _context.Tasks.Update(task);
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