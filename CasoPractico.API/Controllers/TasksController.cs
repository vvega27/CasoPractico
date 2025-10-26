using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net.Sockets;
using CasoPractico.Core.BusinessLogic;
using Task = CasoPractico.Data.Models.Task;

namespace CasoPractico.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TasksController(ITaskBusiness TaskBusiness) : ControllerBase
    {
        // GET: api/<TasksController>
        [HttpGet]
        public async Task<IEnumerable<Task>> Get()
        {
            return await TaskBusiness.GetTasks(id: null);
        }

        // GET api/<TasksController>/5
        [HttpGet("{id}")]
        public async Task<IEnumerable<Task>> Get(int id)
        {
            return await TaskBusiness.GetTasks(id);
        }

        // POST api/<TasksController>
        [HttpPost]
        public async Task<bool> Post([FromBody] Task Task)
        {
            return await TaskBusiness.CreateTasksAsync(Task);
        }

        // PUT api/<TasksController>/5
        [HttpPut("{id}")]
        public async Task<bool> Put(int id, [FromBody] Task value)
        {
            return await TaskBusiness.SaveTaskAsync(value);
        }

        // DELETE api/<TasksController>/5
        [HttpDelete("{id}")]
        public async Task<bool> Delete(int id)
        {
            return await TaskBusiness.DeleteTaskAsync(id);
        }
    }
}

