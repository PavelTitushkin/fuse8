﻿// <auto-generated />
using System;
using API_DataBase;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace API_DataBase.Migrations
{
    [DbContext(typeof(CurrencyRateContext))]
    partial class CurrencyRateContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasDefaultSchema("cur")
                .HasAnnotation("ProductVersion", "7.0.10")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("API_DataBase.Entities.Currencies", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("Date")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("date");

                    b.HasKey("Id")
                        .HasName("pk_currencies_list");

                    b.ToTable("currencies_list", "cur");
                });

            modelBuilder.Entity("API_DataBase.Entities.CurrencyEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Code")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("code");

                    b.Property<int?>("CurrenciesId")
                        .HasColumnType("integer")
                        .HasColumnName("currencies_id");

                    b.Property<decimal>("Value")
                        .HasColumnType("numeric")
                        .HasColumnName("value");

                    b.HasKey("Id")
                        .HasName("pk_currency_entity");

                    b.HasIndex("CurrenciesId")
                        .HasDatabaseName("ix_currency_entity_currencies_id");

                    b.ToTable("currency_entity", "cur");
                });

            modelBuilder.Entity("API_DataBase.Entities.CurrencyEntity", b =>
                {
                    b.HasOne("API_DataBase.Entities.Currencies", null)
                        .WithMany("CurrenciesList")
                        .HasForeignKey("CurrenciesId")
                        .HasConstraintName("fk_currency_entity_currencies_list_currencies_id");
                });

            modelBuilder.Entity("API_DataBase.Entities.Currencies", b =>
                {
                    b.Navigation("CurrenciesList");
                });
#pragma warning restore 612, 618
        }
    }
}
