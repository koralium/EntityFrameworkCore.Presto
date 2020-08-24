using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication1.Models;

namespace WebApplication1.Database
{
    public class Context : DbContext
    {
        public Context(DbContextOptions<Context> options) : base(options)
        {
        }

        public DbSet<Customer> Customers { get; set; }

        public DbSet<Order> Orders { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Customer>(o =>
            {
                o.ToTable("customer")
                    .HasKey(x => x.Custkey);
            });

            modelBuilder.Entity<Order>(o =>
            {
                o.ToTable("orders")
                    .HasKey(x => x.Orderkey);
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}
