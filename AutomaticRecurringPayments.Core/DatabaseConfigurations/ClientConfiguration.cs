using AutomaticRecurringPayment.Model.Entities.Clients;
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

namespace AutomaticRecurringPayments.Core.DatabaseConfigurations
{
    public class ClientConfiguration : BaseEntityTypeConfiguration<Client>
    {
        protected override void ConfigureEntity(EntityTypeBuilder<Client> builder)
        {
            builder.Property(x => x.FirstName)
                .IsRequired();

            builder.Property(x => x.LastName)
                .IsRequired();

            builder.Property(x => x.Email)
                .IsRequired();

            builder.Property(x => x.PhoneNumber);

            builder.ToTable("Clients");

            Relationships(builder);
        }

        private void Relationships(EntityTypeBuilder<Client> builder)
        {
        }
    }
}
