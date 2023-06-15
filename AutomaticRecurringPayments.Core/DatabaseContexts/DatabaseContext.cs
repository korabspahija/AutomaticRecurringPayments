using AutomaticRecurringPayment.Model.Entities.BraintreeTransactions;
using AutomaticRecurringPayment.Model.Entities.Clients;
using AutomaticRecurringPayment.Model.Entities.Subscriptions;
using AutomaticRecurringPayments.Core.Extensions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace AutomaticRecurringPayments.Core.DatabaseContexts
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext(string connectionString) : base(GetOptions(connectionString))
        {
        }

        public DatabaseContext(DbContextOptions<DatabaseContext> options)
        : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyEntityTypes(typeof(DatabaseContext).Assembly, typeof(IEntityTypeConfiguration<>));
        }

        private static DbContextOptions GetOptions(string connectionString)
        {
            return SqlServerDbContextOptionsExtensions.UseSqlServer(new DbContextOptionsBuilder(), connectionString).Options;
        }

        public DbSet<BraintreeTransaction> BraintreeTransactions { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<Subscription> Subscriptions { get; set; }
    }
}
