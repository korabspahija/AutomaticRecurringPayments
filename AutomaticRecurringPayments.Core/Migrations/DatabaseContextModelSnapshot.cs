﻿// <auto-generated />
using System;
using AutomaticRecurringPayments.Core.DatabaseContexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace AutomaticRecurringPayments.Core.Migrations
{
    [DbContext(typeof(DatabaseContext))]
    partial class DatabaseContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.5")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("AutomaticRecurringPayment.Model.Entities.BraintreeTransactions.BraintreeTransaction", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("Id");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<long>("Amount")
                        .HasColumnType("bigint");

                    b.Property<int>("ApplicationId")
                        .HasColumnType("int");

                    b.Property<string>("BraintreeCustomerId")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("BraintreeTransactionStatusId")
                        .HasColumnType("int");

                    b.Property<int>("ClientId")
                        .HasColumnType("int");

                    b.Property<DateTime?>("InsertDateTime")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime2")
                        .HasColumnName("InsertDateTime")
                        .HasDefaultValueSql("GETDATE()");

                    b.Property<string>("InsertedBy")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("InsertedBy");

                    b.Property<bool>("IsDeleted")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bit")
                        .HasDefaultValue(false)
                        .HasColumnName("IsDeleted");

                    b.Property<int?>("OnDemandStatus")
                        .HasColumnType("int");

                    b.Property<int?>("ParentBraintreeTransactionId")
                        .HasColumnType("int");

                    b.Property<string>("PaymentMethodToken")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("ProductAccessProviderId")
                        .HasColumnType("int");

                    b.Property<bool?>("RecurringCanceled")
                        .HasColumnType("bit");

                    b.Property<int>("SubscriptionId")
                        .HasColumnType("int");

                    b.Property<string>("TransactionId")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("UpdateDateTime")
                        .HasColumnType("datetime2")
                        .HasColumnName("UpdatedAt");

                    b.Property<string>("UpdatedBy")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("UpdatedBy");

                    b.HasKey("Id");

                    b.HasIndex("ClientId");

                    b.HasIndex("ParentBraintreeTransactionId");

                    b.HasIndex("SubscriptionId");

                    b.ToTable("BraintreeTransactions", (string)null);
                });

            modelBuilder.Entity("AutomaticRecurringPayment.Model.Entities.Clients.Client", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("Id");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("BraintreeCustomerId")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("ConsumerCode")
                        .HasColumnType("int");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("InsertDateTime")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime2")
                        .HasColumnName("InsertDateTime")
                        .HasDefaultValueSql("GETDATE()");

                    b.Property<string>("InsertedBy")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("InsertedBy");

                    b.Property<bool>("IsDeleted")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bit")
                        .HasDefaultValue(false)
                        .HasColumnName("IsDeleted");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("UpdateDateTime")
                        .HasColumnType("datetime2")
                        .HasColumnName("UpdatedAt");

                    b.Property<string>("UpdatedBy")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("UpdatedBy");

                    b.HasKey("Id");

                    b.ToTable("Clients", (string)null);
                });

            modelBuilder.Entity("AutomaticRecurringPayment.Model.Entities.Subscriptions.Subscription", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("Id");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("ClientId")
                        .HasColumnType("int");

                    b.Property<DateTime?>("InsertDateTime")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime2")
                        .HasColumnName("InsertDateTime")
                        .HasDefaultValueSql("GETDATE()");

                    b.Property<string>("InsertedBy")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("InsertedBy");

                    b.Property<bool>("IsDeleted")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bit")
                        .HasDefaultValue(false)
                        .HasColumnName("IsDeleted");

                    b.Property<int?>("LastBraintreeTransactionId")
                        .HasColumnType("int");

                    b.Property<int>("StatusId")
                        .HasColumnType("int");

                    b.Property<DateTime?>("UpdateDateTime")
                        .HasColumnType("datetime2")
                        .HasColumnName("UpdatedAt");

                    b.Property<string>("UpdatedBy")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("UpdatedBy");

                    b.HasKey("Id");

                    b.HasIndex("ClientId");

                    b.HasIndex("LastBraintreeTransactionId")
                        .IsUnique()
                        .HasFilter("[LastBraintreeTransactionId] IS NOT NULL");

                    b.ToTable("Subscriptions", (string)null);
                });

            modelBuilder.Entity("AutomaticRecurringPayment.Model.Entities.BraintreeTransactions.BraintreeTransaction", b =>
                {
                    b.HasOne("AutomaticRecurringPayment.Model.Entities.Clients.Client", "Client")
                        .WithMany("BraintreeTransactions")
                        .HasForeignKey("ClientId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("AutomaticRecurringPayment.Model.Entities.BraintreeTransactions.BraintreeTransaction", "ParentBraintreeTransaction")
                        .WithMany("ChildBraintreeTransactions")
                        .HasForeignKey("ParentBraintreeTransactionId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("AutomaticRecurringPayment.Model.Entities.Subscriptions.Subscription", "Subscription")
                        .WithMany("BraintreeTransactions")
                        .HasForeignKey("SubscriptionId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Client");

                    b.Navigation("ParentBraintreeTransaction");

                    b.Navigation("Subscription");
                });

            modelBuilder.Entity("AutomaticRecurringPayment.Model.Entities.Subscriptions.Subscription", b =>
                {
                    b.HasOne("AutomaticRecurringPayment.Model.Entities.Clients.Client", "Client")
                        .WithMany("Subscriptions")
                        .HasForeignKey("ClientId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("AutomaticRecurringPayment.Model.Entities.BraintreeTransactions.BraintreeTransaction", "LastBraintreeTransaction")
                        .WithOne("LastSubscription")
                        .HasForeignKey("AutomaticRecurringPayment.Model.Entities.Subscriptions.Subscription", "LastBraintreeTransactionId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.Navigation("Client");

                    b.Navigation("LastBraintreeTransaction");
                });

            modelBuilder.Entity("AutomaticRecurringPayment.Model.Entities.BraintreeTransactions.BraintreeTransaction", b =>
                {
                    b.Navigation("ChildBraintreeTransactions");

                    b.Navigation("LastSubscription");
                });

            modelBuilder.Entity("AutomaticRecurringPayment.Model.Entities.Clients.Client", b =>
                {
                    b.Navigation("BraintreeTransactions");

                    b.Navigation("Subscriptions");
                });

            modelBuilder.Entity("AutomaticRecurringPayment.Model.Entities.Subscriptions.Subscription", b =>
                {
                    b.Navigation("BraintreeTransactions");
                });
#pragma warning restore 612, 618
        }
    }
}
