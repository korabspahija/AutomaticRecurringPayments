using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using AutomaticRecurringPayment.Model.Entities;

namespace Bisko.Visualizer.Edge.Core.DatabaseConfigurations
{
    public abstract class BaseEntityTypeConfiguration<T> : IEntityTypeConfiguration<T> where T : BaseEntity
    {
        public void Configure(EntityTypeBuilder<T> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .HasColumnName("Id")
                .ValueGeneratedOnAdd()
                .IsRequired();

            builder.Property(x => x.InsertDateTime)
                .HasColumnName("InsertDateTime")
                .HasDefaultValueSql("GETDATE()")
                .ValueGeneratedOnAdd()
                .IsRequired();

            builder.Property(x => x.InsertedBy)
                .HasColumnName("InsertedBy")
                .HasDefaultValue(null);

            builder.Property(x => x.UpdateDateTime)
                .HasColumnName("UpdatedAt");

            builder.Property(x => x.UpdatedBy)
                .HasColumnName("UpdatedBy")
                .HasDefaultValue(null);

            builder.Property(x => x.IsDeleted)
                .HasColumnName("IsDeleted")
                .HasDefaultValue(false)
                .ValueGeneratedOnAdd()
                .IsRequired();

            builder.HasQueryFilter(x => !x.IsDeleted);

            ConfigureEntity(builder);
        }

        protected abstract void ConfigureEntity(EntityTypeBuilder<T> builder);
    }
}
