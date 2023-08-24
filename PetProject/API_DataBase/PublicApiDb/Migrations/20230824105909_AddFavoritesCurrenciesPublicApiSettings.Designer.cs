﻿// <auto-generated />
using DataStore.PublicApiDb;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DataStore.PublicApiDb.Migrations
{
    [DbContext(typeof(PublicApiContext))]
    [Migration("20230824105909_AddFavoritesCurrenciesPublicApiSettings")]
    partial class AddFavoritesCurrenciesPublicApiSettings
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasDefaultSchema("user")
                .HasAnnotation("ProductVersion", "7.0.10")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("DataStore.PublicApiDb.Entities.FavoritesCurrencies", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("BaseCurrency")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("base_currency");

                    b.Property<string>("Currency")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("currency");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("name");

                    b.HasKey("Id")
                        .HasName("pk_favorites_currencies");

                    b.HasIndex("Name")
                        .IsUnique()
                        .HasDatabaseName("ix_favorites_currencies_name");

                    b.HasIndex("Currency", "BaseCurrency")
                        .IsUnique()
                        .HasDatabaseName("ix_favorites_currencies_currency_base_currency");

                    b.ToTable("favorites_currencies", "user");
                });

            modelBuilder.Entity("DataStore.PublicApiDb.Entities.PublicApiSettings", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("CurrencyRoundCount")
                        .HasColumnType("integer")
                        .HasColumnName("currency_round_count");

                    b.Property<string>("DefaultCurrency")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("default_currency");

                    b.HasKey("Id")
                        .HasName("pk_public_api_settings");

                    b.ToTable("public_api_settings", "user", t =>
                        {
                            t.HasCheckConstraint("rounding_cannot_be_less_than_zero", "currency_round_count >= 0");
                        });
                });
#pragma warning restore 612, 618
        }
    }
}
