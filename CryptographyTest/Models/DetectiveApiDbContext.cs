using CryptographyTest.Services;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace CryptographyTest.Models
{
    public class DetectiveApiDbContext : IdentityDbContext<User, IdentityRole<Guid>, Guid>  // Extends IdentityDbContext for IdentityUser<Guid>
    {
        public DbSet<Case> Cases { get; set; }
        public DbSet<Tip> Tips { get; set; }
        public DbSet<ContactPerson> ContactPersons { get; set; }

        public DetectiveApiDbContext(DbContextOptions<DetectiveApiDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>()
                .Property(u => u.Email)
                .IsConcurrencyToken(false)
                .HasConversion(
                    v => RsaService.Encrypt(v),
                    v => RsaService.Decrypt(v)
                );

            modelBuilder.Entity<User>()
                .Property(u => u.UserName)
                .IsConcurrencyToken(false)
                .HasConversion(
                    v => RsaService.Encrypt(v),
                    v => RsaService.Decrypt(v)
                );

            modelBuilder.Entity<Case>()
                .Property(u => u.Name)
                .IsConcurrencyToken(false)
                .HasConversion(
                    v => RsaService.Encrypt(v),
                    v => RsaService.Decrypt(v)
                );

            modelBuilder.Entity<Case>()
                .Property(u => u.Description)
                .IsConcurrencyToken(false)
                .HasConversion(
                    v => RsaService.Encrypt(v),
                    v => RsaService.Decrypt(v)
                );

            modelBuilder.Entity<Tip>()
                .Property(u => u.Description)
                .IsConcurrencyToken(false)
                .HasConversion(
                    v => RsaService.Encrypt(v),
                    v => RsaService.Decrypt(v)
                );

            modelBuilder.Entity<ContactPerson>()
                .Property(u => u.Name)
                .IsConcurrencyToken(false)
                .HasConversion(
                    v => RsaService.Encrypt(v),
                    v => RsaService.Decrypt(v)
                );

            modelBuilder.Entity<ContactPerson>()
                .Property(u => u.Phone)
                .IsConcurrencyToken(false)
                .HasConversion(
                    v => RsaService.Encrypt(v),
                    v => RsaService.Decrypt(v)
                );
        }

        private void ApplyEncryption()
        {
            var entries = ChangeTracker.Entries().Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);
            foreach (var entry in entries)
            {
                if (entry.Entity is User user)
                {
                    if (!string.IsNullOrWhiteSpace(user.UserName))
                    {
                        user.UserName = RsaService.Encrypt(user.UserName);
                    }
                    if (!string.IsNullOrWhiteSpace(user.Email))
                    {
                        user.Email = RsaService.Encrypt(user.Email);
                    }
                }

                if (entry.Entity is Case cas)
                {
                    if (!string.IsNullOrWhiteSpace(cas.Name))
                    {
                        cas.Name = RsaService.Encrypt(cas.Name);
                    }

                    if (!string.IsNullOrWhiteSpace(cas.Description))
                    {
                        cas.Description = RsaService.Encrypt(cas.Description);
                    }
                }

                if (entry.Entity is Tip tip)
                {
                    if (!string.IsNullOrWhiteSpace(tip.Description))
                    {
                        tip.Description = RsaService.Encrypt(tip.Description);
                    }
                }

                if (entry.Entity is ContactPerson person)
                {
                    if (!string.IsNullOrWhiteSpace(person.Name))
                    {
                        person.Name = RsaService.Encrypt(person.Name);
                    }
                    if (!string.IsNullOrWhiteSpace(person.Phone))
                    {
                        person.Phone = RsaService.Encrypt(person.Phone);
                    }
                }
            }
        }
    }
}