using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ProjectManager.Data
{
    public class ApplicationDbContext : IdentityDbContext<UserProfile>
    {
        public DbSet<Project> Projects => Set<Project>();
        public DbSet<ProjectStates> ProjectStates => Set<ProjectStates>();
        public DbSet<ProjectTask> ProjectTasks => Set<ProjectTask>();

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<UserProfile>()
                .Property(e => e.FirstName)
                .HasMaxLength(20);

            modelBuilder.Entity<UserProfile>()
                .Property(e => e.LastName)
                .HasMaxLength(20);

            //TODO: Optimise
            modelBuilder.Entity<Project>().Navigation(p => p.States).AutoInclude();
            modelBuilder.Entity<Project>().Navigation(p => p.Tasks).AutoInclude();

            modelBuilder.Entity<ProjectStates>().Navigation(ps => ps.Project).AutoInclude();
            modelBuilder.Entity<ProjectStates>().Navigation(ps => ps.User).AutoInclude();
        }
    }
}