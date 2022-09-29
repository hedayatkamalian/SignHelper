using Microsoft.EntityFrameworkCore;
using SignHelperApp.Entities;

namespace SignHelperApp.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);

            modelBuilder.Entity<SignRequest>()
                .HasKey(p => p.Id);

            modelBuilder.Entity<SignRequest>()
                .Property(p => p.Id)
                .ValueGeneratedNever();

            modelBuilder.Entity<SignRequest>()
                .HasOne(p => p.Template)
                .WithMany(p => p.SignRequests)
                .HasForeignKey(p => p.TemplateId)
                .OnDelete(DeleteBehavior.Cascade);



            modelBuilder.Entity<Template>()
                .HasKey(p => p.Id);

            modelBuilder.Entity<Template>()
                .Property(p => p.Id)
                .ValueGeneratedNever();

            modelBuilder.Entity<Template>()
                .HasMany(p => p.SignRequests)
                .WithOne(p => p.Template)
                .HasForeignKey(p => p.TemplateId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Template>()
                .OwnsMany(p => p.SignPoints).ToTable("Template_SingPoints");
        }


        public DbSet<SignRequest> SignRequests { get; set; }
        public DbSet<Template> Templates { get; set; }


    }
}
