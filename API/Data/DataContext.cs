using API.Entities;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class DataContext : DbContext
    {
        public required DbSet<Employee> Employees { get; set; }
        public required DbSet<WorkTask> Tasks { get; set; }
        public required DbSet<Completed> CompletedTasks { get; set; }
        public DataContext(DbContextOptions options) : base(options)
        {
        }

        // protected override void onModelCreating(ModelBuilder modelBuilder)
        // {
        //     modelBuilder    
        //         .Entity<Employee>()
        //         .HasMany(x => x.Completed)
        //         .WithOne(x => x.Assignee)
        //         .OnDelete(DeleteBehavior.ClientSetNull);
        // }
    }

}