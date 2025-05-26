using System.Data;
using Kids.Models;
using Microsoft.EntityFrameworkCore;


namespace Kids.Context
{
    public class KidsContext:DbContext
    {
        public KidsContext(DbContextOptions<KidsContext> options) : base(options)
        {
            


        }
        public DbSet<Comment> Comments { get; set; }
   
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<ContactUs> ContactUs { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Master> Masters { get; set; }
        public DbSet<Factor> Factors { get; set; }
        public DbSet<DiscountCode> DiscountCodes { get; set; }
        public DbSet<CourseGroup> CourseGroups { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<UserCourse> UserCourses { get; set; }
        public DbSet<Episode> Episodes { get; set; }
        public DbSet<Rate> Rates { get; set; }
        public DbSet<Article> Articles { get; set; }
        public DbSet<ArticleGroup> ArticleGroups { get; set; }
        public DbSet<AgeGroup> AgeGroups { get; set; }
        public DbSet<CourseGallery> CourseGalleries { get; set; }
        public DbSet<ErrorLog> ErrorLogs { get; set; }
        public DbSet<Slider> Sliders { get; set; }
        public DbSet<Log> Logs { get; set; }

    }
}

