using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;

namespace evenotify_v2.models
{
    public class eve : DbContext
    {
        public DbSet<MsgIds> MsgIds { get; set; }
        public DbSet<Users> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Server=.;Database=eve;Trusted_Connection=True;");
        }
    }

    public class MsgIds
    {
        [Key]
        [MaxLength(50)]
        public string Id { get; set; }
    }

    public class Users
    {
        [Key]
        public int Id { get; set; }
        [MaxLength(50)]
        public string ApiId { get; set; }
        [MaxLength(100)]
        public string ApiKey { get; set; }
        [MaxLength(50)]
        public string Email { get; set; }

        public bool HadFirst { get; set; }

        public int fails { get; set; }

        public bool verified { get; set; }


        public Guid verifyUrl { get; set; }

        [MaxLength(50)]
        public string character { get; set; }

        public int SendCount { get; set; } = 0;
    }
}