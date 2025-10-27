using Microsoft.AspNetCore.Mvc;
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
        public async Task<IActionResult> Put(int id, [FromBody] Task value)
        {
            if (id != value.Id) return BadRequest("Id mismatch");

            var ok = await TaskBusiness.SaveTaskAsync(value);
            if (!ok)
                return BadRequest("No se puede APROBAR una solicitud denegada con más de 24h desde su creación (UTC).");

            return Ok(value);
        }

        // DELETE api/<TasksController>/5
        [HttpDelete("{id}")]
        public async Task<bool> Delete(int id)
        {
            return await TaskBusiness.DeleteTaskAsync(id);
        }
    }
}

