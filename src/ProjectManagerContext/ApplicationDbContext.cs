using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ProjectManagerContext.Data;

namespace ProjectManager.Data
{
    public class ApplicationDbContext : IdentityDbContext<UserProfile>
    {
        public DbSet<Project> Projects => Set<Project>();
        public DbSet<ProjectStates> ProjectStates => Set<ProjectStates>();
        public DbSet<ProjectTask> ProjectTasks => Set<ProjectTask>();

        public DbSet<Quote> Quotes => Set<Quote>();

        public DbSet<DrawingIssue> DrawingIssues => Set<DrawingIssue>();

        public DbSet<Variation> Variations => Set<Variation>();

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            configurationBuilder.Properties<decimal>().HavePrecision(10, 2);
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

            modelBuilder.Entity<Variation>()
                .HasOne(v => v.Originator)
                .WithMany(c => c.VariationsIssued)
                .HasForeignKey(v => v.OriginatorId)
                .IsRequired(false);

            modelBuilder.Entity<Variation>()
                .HasOne(v => v.Acceptor)
                .WithMany(c => c.VariationsAccepted)
                .HasForeignKey(v => v.AcceptorId)
                .IsRequired(false);

            modelBuilder.Entity<Variation>()
                .HasOne(v => v.RaisedBy)
                .WithMany(u => u.VariationsRaised)
                .HasForeignKey(v => v.RaisedById)
                .IsRequired(false);
                

            //TODO: Optimise
            modelBuilder.Entity<Project>().Navigation(p => p.States).AutoInclude();
            modelBuilder.Entity<Project>().Navigation(p => p.Tasks).AutoInclude();

            modelBuilder.Entity<ProjectStates>().Navigation(ps => ps.Project).AutoInclude();
            modelBuilder.Entity<ProjectStates>().Navigation(ps => ps.User).AutoInclude();

            modelBuilder.Entity<DrawingIssue>().Navigation(di => di.Contacts).AutoInclude();
            modelBuilder.Entity<DrawingIssue>().Navigation(di => di.Entries).AutoInclude();

            //modelBuilder.Entity<UserProfile>().Navigation(p => p.LineManager).AutoInclude();
            //modelBuilder.Entity<UserProfile>().Navigation(p => p.Reports).AutoInclude();
        }
    }
}