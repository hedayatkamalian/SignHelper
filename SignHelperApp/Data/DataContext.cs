using Microsoft.EntityFrameworkCore;
using SignHelperApp.Entities;

namespace SignHelperApp.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<SignRequest>().HasKey(p => p.Id);

            modelBuilder.Entity<SignRequest>()
                .HasOne(p => p.Template)
                .WithMany()
                .HasForeignKey(p => p.TemplateId)
                .OnDelete(DeleteBehavior.NoAction);


            modelBuilder.Entity<Template>().HasKey(p => p.Id);

            modelBuilder.Entity<Template>()
                .HasMany(p => SignRequests)
                .WithOne()
                .HasForeignKey(p => p.TemplateId)
                .OnDelete(DeleteBehavior.NoAction);
        }


        public DbSet<SignRequest> SignRequests { get; set; }
        public DbSet<Template> Templates { get; set; }


    }
}
