using Microsoft.EntityFrameworkCore;

namespace ReviewGenerator.DataModels
{
    public class Context: DbContext
    {
        public Context(DbContextOptions options)
        : base(options)
        {
        }

        public DbSet<Review> Reviews { get; set; }


        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    optionsBuilder.UseSqlServer(@"Server=(localdb)\MSSQLLocalDB;Database=ReviewGenerator;Trusted_Connection=True;");
        //}

    }
}
