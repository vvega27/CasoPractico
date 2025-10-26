using Microsoft.EntityFrameworkCore;
using Task = CasoPractico.MinimalAPI.Model.Task;

namespace CasoPractico.MinimalAPI.Context
{
    public class TaskDbContext : DbContext
    {
        public TaskDbContext(DbContextOptions<TaskDbContext> options) : base(options)
        {
        }

        public DbSet<Task> Task => Set<Task>();
    }
}
