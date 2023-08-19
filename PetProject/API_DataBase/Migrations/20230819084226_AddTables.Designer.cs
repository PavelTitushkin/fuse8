﻿// <auto-generated />
using System;
using API_DataBase;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace API_DataBase.Migrations
{
    [DbContext(typeof(CurrencyRateContext))]
    [Migration("20230819084226_AddTables")]
    partial class AddTables
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
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

                    b.Property<DateOnly>("Date")
                        .HasColumnType("date")
                        .HasColumnName("date");

                    b.HasKey("Id")
                        .HasName("pk_currencies_list");

                    b.ToTable("currencies_list", "cur");
                });

            modelBuilder.Entity("API_DataBase.Entities.Currency", b =>
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
                        .HasName("pk_currencies");

                    b.HasIndex("CurrenciesId")
                        .HasDatabaseName("ix_currencies_currencies_id");

                    b.ToTable("currencies", "cur");
                });

            modelBuilder.Entity("API_DataBase.Entities.Currency", b =>
                {
                    b.HasOne("API_DataBase.Entities.Currencies", null)
                        .WithMany("CurrenciesList")
                        .HasForeignKey("CurrenciesId")
                        .HasConstraintName("fk_currencies_currencies_list_currencies_id");
                });

            modelBuilder.Entity("API_DataBase.Entities.Currencies", b =>
                {
                    b.Navigation("CurrenciesList");
                });
#pragma warning restore 612, 618
        }
    }
}
