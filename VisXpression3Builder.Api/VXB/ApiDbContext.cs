using SQLite.CodeFirst;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace VisXpression3Builder.Api.VXB
{
    public class ApiDbContext : DbContext
    {
        public ApiDbContext() : base("DefaultConnection") { }
        public DbSet<UserDefinedFunction> UserDefinedFunctions { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            var sqliteConnectionInitializer = new SqliteCreateDatabaseIfNotExists<ApiDbContext>(modelBuilder);
            Database.SetInitializer(sqliteConnectionInitializer);
        }
    }
}