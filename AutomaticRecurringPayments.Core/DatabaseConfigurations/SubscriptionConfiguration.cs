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
using AutomaticRecurringPayment.Model.Entities.Subscriptions;

namespace AutomaticRecurringPayments.Core.DatabaseConfigurations
{
    public class SubscriptionConfiguration : BaseEntityTypeConfiguration<Subscription>
    {

        protected override void ConfigureEntity(EntityTypeBuilder<Subscription> builder)
        {
            builder.Property(x => x.StatusId)
                   .IsRequired();

            builder.Property(x => x.ClientId)
                   .IsRequired();

            builder.Property(x => x.LastBraintreeTransactionId);

            builder.ToTable("Subscriptions");

            Relationships(builder);
        }

        private void Relationships(EntityTypeBuilder<Subscription> builder)
        {
            builder.HasOne(x => x.Client)
                   .WithMany(x => x.Subscriptions)
                   .OnDelete(DeleteBehavior.Restrict)
                   .HasForeignKey(x => x.ClientId);

            builder.HasOne(x => x.LastBraintreeTransaction)
                   .WithOne(x => x.LastSubscription)
                   .OnDelete(DeleteBehavior.Restrict)
                   .HasForeignKey<Subscription>(x => x.LastBraintreeTransactionId);
        }
    }
}
