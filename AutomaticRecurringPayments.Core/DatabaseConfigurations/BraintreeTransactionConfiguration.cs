using Bisko.Visualizer.Edge.Core.DatabaseConfigurations;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutomaticRecurringPayment.Model.Entities.BraintreeTransactions;

namespace AutomaticRecurringPayments.Core.DatabaseConfigurations
{
    public class BraintreeTransactionConfiguration : BaseEntityTypeConfiguration<BraintreeTransaction>
    {

        protected override void ConfigureEntity(EntityTypeBuilder<BraintreeTransaction> builder)
        {
            builder.Property(x => x.BraintreeCustomerId)
                .IsRequired();

            builder.Property(x => x.TransactionId);

            builder.Property(x => x.BraintreeTransactionStatusId);
            builder.Property(x => x.Amount);

            builder.Property(x => x.PaymentMethodToken)
                .IsRequired();

            builder.Property(x => x.ClientId)
                .IsRequired();

            builder.Property(x => x.ParentBraintreeTransactionId);

            builder.ToTable("BraintreeTransactions");

            Relationships(builder);
        }

        private void Relationships(EntityTypeBuilder<BraintreeTransaction> builder)
        {
            builder.HasOne(x => x.Client)
                .WithMany(x => x.BraintreeTransactions)
                .OnDelete(DeleteBehavior.Restrict)
                .HasForeignKey(x => x.ClientId);

            builder.HasOne(x => x.Subscription)
                .WithMany(x => x.BraintreeTransactions)
                .OnDelete(DeleteBehavior.Restrict)
                .HasForeignKey(x => x.SubscriptionId);

            builder.HasOne(x => x.ParentBraintreeTransaction)
                .WithMany(x => x.ChildBraintreeTransactions)
                .OnDelete(DeleteBehavior.Restrict)
                .HasForeignKey(x => x.ParentBraintreeTransactionId);
        }
    }
}

