using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace EFNullableSeedBug
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var db = new BloggingContext())
            {
                IQueryable<Blog> blogQueryable = db.Set<Blog>().AsNoTracking().Where(b => b.Url.StartsWith("http"));

                foreach (var b in blogQueryable)
                    Console.WriteLine(b.Url);
            }
        }
    }

    public class BloggingContext : DbContext
    {
        public static readonly ILoggerFactory MyLoggerFactory = LoggerFactory.Create(builder =>
        {
            builder
            .AddFilter((category, level) =>
                category == DbLoggerCategory.Database.Command.Name
                && level == LogLevel.Information)
            .AddConsole();
        });

        public DbSet<Blog> Blogs { get; set; }
        public DbSet<Post> Posts { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlite("Data Source=blogging.db").UseLoggerFactory(MyLoggerFactory);
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Blog>().HasData(new Blog { BlogId = 1, Url = "http://blogs.msdn.com/adonet" });
            builder.Entity<Post>().HasData(new Post
            {
                PostId = 1,
                Title = "Hello World",
                Content = "Hello World: I wrote an app using EF Core!",
                BlogId = 1
            });
        }
    }

    public class Blog
    {
        public int BlogId { get; set; }
        public string Url { get; set; }

        public List<Post> Posts { get; set; } = new List<Post>();
    }

    public class Post
    {
        public int PostId { get; set; }

        public string Title { get; set; }

        public string Content { get; set; }

        public int BlogId { get; set; }

        public Blog Blog { get; set; }

        public int? ReplyToId { get; set; }

        public Post ReplyTo { get; set; }

        public List<Post> Replies { set; get; } = new List<Post>();
    }
}