using CryptographyTest.Services;
using Microsoft.EntityFrameworkCore;

namespace CryptographyTest.Models
{
    public class DetectiveApiDbContext : DbContext
    {
        public DbSet<Case> Cases { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Tip> Tips { get; set; }
        public DbSet<ContactPerson> ContactPersons { get; set; }

        public DetectiveApiDbContext(DbContextOptions<DetectiveApiDbContext> options) : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .Property(u => u.Email)
                 .IsConcurrencyToken(false)
                .HasConversion(
                    v => AesService.Encrypt(v),  // Encrypt when writing to the database
                    v => AesService.Decrypt(v)   // Decrypt when reading from the database
                );

            modelBuilder.Entity<User>()
                .Property(u => u.Name)
                 .IsConcurrencyToken(false)
                .HasConversion(
                    v => AesService.Encrypt(v),  // Encrypt when writing to the database
                    v => AesService.Decrypt(v)   // Decrypt when reading from the database
                );

            modelBuilder.Entity<Case>()
                .Property(u => u.Name)
                 .IsConcurrencyToken(false)
                .HasConversion(
                    v => AesService.Encrypt(v),  // Encrypt when writing to the database
                    v => AesService.Decrypt(v)   // Decrypt when reading from the database
                );
            modelBuilder.Entity<Case>()
                .Property(u => u.Description)
                 .IsConcurrencyToken(false)
                .HasConversion(
                    v => AesService.Encrypt(v),  // Encrypt when writing to the database
                    v => AesService.Decrypt(v)   // Decrypt when reading from the database
                );
            modelBuilder.Entity<Tip>()
                .Property(u => u.Description)
                 .IsConcurrencyToken(false)
                .HasConversion(
                    v => AesService.Encrypt(v),  // Encrypt when writing to the database
                    v => AesService.Decrypt(v)   // Decrypt when reading from the database
                );
            modelBuilder.Entity<ContactPerson>()
                .Property(u => u.Name)
                 .IsConcurrencyToken(false)
                .HasConversion(
                    v => AesService.Encrypt(v),  // Encrypt when writing to the database
                    v => AesService.Decrypt(v)   // Decrypt when reading from the database
                );
            modelBuilder.Entity<ContactPerson>()
                .Property(u => u.Phone)
                 .IsConcurrencyToken(false)
                .HasConversion(
                    v => AesService.Encrypt(v),  // Encrypt when writing to the database
                    v => AesService.Decrypt(v)   // Decrypt when reading from the database
                );
        }


        //public override int SaveChanges()
        //{
        //    ApplyEncryption();
        //    return base.SaveChanges();
        //}

        //public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        //{
        //    ApplyEncryption();
        //    return await base.SaveChangesAsync(cancellationToken);
        //}

        private void ApplyEncryption()
        {
            var entries = ChangeTracker.Entries().Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);
            foreach (var entry in entries)
            {
                if (entry.State == EntityState.Added || entry.State == EntityState.Modified)
                {
                    if (entry.Entity is User user)
                    {
                        if (!string.IsNullOrWhiteSpace(user.Name))
                        {
                            user.Name = AesService.Encrypt(user.Name);
                        }
                        if (!string.IsNullOrWhiteSpace(user.Email))
                        {
                            user.Email = AesService.Encrypt(user.Email);
                        }
                    }
                }

                if (entry.State == EntityState.Added || entry.State == EntityState.Modified)
                {
                    if (entry.Entity is Case cas)
                    {
                        if (!string.IsNullOrWhiteSpace(cas.Name))
                        {
                            cas.Name = AesService.Encrypt(cas.Name);
                        }

                        if (!string.IsNullOrWhiteSpace(cas.Description))
                        {
                            cas.Description = AesService.Encrypt(cas.Description);
                        }
                    }
                }

                if (entry.State == EntityState.Added || entry.State == EntityState.Modified)
                {
                    if (entry.Entity is Tip tip)
                    {
                        if (!string.IsNullOrWhiteSpace(tip.Description))
                        {
                            tip.Description = AesService.Encrypt(tip.Description);
                        }
                    }
                }

                if (entry.State == EntityState.Added || entry.State == EntityState.Modified)
                {
                    if (entry.Entity is ContactPerson person)
                    {
                        if (!string.IsNullOrWhiteSpace(person.Name))
                        {
                            person.Name = AesService.Encrypt(person.Name);
                        }
                        if (!string.IsNullOrWhiteSpace(person.Phone))
                        {
                            person.Phone = AesService.Encrypt(person.Phone);
                        }
                    }
                }
            }
        }
    }
}
