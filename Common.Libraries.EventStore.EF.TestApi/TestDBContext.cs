using Common.Libraries.EventStore.Projection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Common.Libraries.EventStore.EF.TestApi
{
    public class TestContextFactory : IDesignTimeDbContextFactory<TestDBContext>
    {
        public TestDBContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<TestDBContext>();
            optionsBuilder.UseMySql("server=localhost;database=TestDb;uid=root;pwd=Nova@2025", ServerVersion.AutoDetect("server=localhost;database=TestDb;uid=root;pwd=Nova@2025"));

            return new TestDBContext(optionsBuilder.Options);
        }
    }
    public class TestDBContext : DbContext
    {
        public TestDBContext(DbContextOptions options) : base(options)
        {
        }

        protected TestDBContext()
        {
        }
        public DbSet<UserDetails> UserDetails { get; set; }
        public DbSet<Event> Events { get; set; }

        public DbSet<Checkpoint> CheckPoints { get; set; }

        public DbSet<UserSnapshot> UserSnapshots { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder builder)
        {
            if (!builder.IsConfigured)
            {


            }

            base.OnConfiguring(builder);
        }
    }
}
