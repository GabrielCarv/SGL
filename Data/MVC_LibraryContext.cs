using Microsoft.EntityFrameworkCore;
using MVC_Library.Models;


namespace MVC_Library.Data
{
    public class MVC_LibraryContext : DbContext
    {
        public MVC_LibraryContext(DbContextOptions<MVC_LibraryContext> options)
            : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<BookCategory>()
                   .HasKey(b => new { b.BookId, b.CategoryId });

            modelBuilder.Entity<Phone>()
                   .HasKey(d => new { d.Number});

            modelBuilder.Entity<Book>()
                .Property(v => v.BasePrice).HasPrecision(4, 4);

        }

        public DbSet<Rent> Rents { get; set; }

        public DbSet<Person> Peoples { get; set; }

        public DbSet<Book> Books { get; set; }

        public DbSet<Inventory> Inventories { get; set; }

        public DbSet<Phone> Phones { get; set; }

        public DbSet<PublishingCompany> PublishingCompanies { get; set; }
        public DbSet<Category> Categories { get; set; }

        public DbSet<BookCategory> BookCategories { get; set; }

    }
}
