﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using Store.DAL.Context;

namespace Store.DAL.Migrations
{
    [DbContext(typeof(StoreDbContext))]
    [Migration("20200909214358_CreateIdentityTables")]
    partial class CreateIdentityTables
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                .HasAnnotation("ProductVersion", "3.1.8")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            modelBuilder.Entity("Store.Entities.BookEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("Author")
                        .IsRequired()
                        .HasColumnType("character varying(50)")
                        .HasMaxLength(50);

                    b.Property<Guid>("BookstoreId")
                        .HasColumnType("uuid");

                    b.Property<DateTime>("DateCreatedUtc")
                        .HasColumnType("timestamp without time zone");

                    b.Property<DateTime>("DateUpdatedUtc")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("character varying(50)")
                        .HasMaxLength(50);

                    b.HasKey("Id");

                    b.HasIndex("BookstoreId");

                    b.ToTable("Book");
                });

            modelBuilder.Entity("Store.Entities.BookstoreEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime>("DateCreatedUtc")
                        .HasColumnType("timestamp without time zone");

                    b.Property<DateTime>("DateUpdatedUtc")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("Location")
                        .IsRequired()
                        .HasColumnType("character varying(100)")
                        .HasMaxLength(100);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("character varying(50)")
                        .HasMaxLength(50);

                    b.HasKey("Id");

                    b.ToTable("Bookstore");
                });

            modelBuilder.Entity("Store.Entities.Identity.ClaimEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("ClaimType")
                        .HasColumnType("varchar")
                        .HasMaxLength(150);

                    b.Property<string>("ClaimValue")
                        .HasColumnType("varchar");

                    b.Property<DateTime>("DateCreatedUtc")
                        .HasColumnType("timestamp without time zone");

                    b.Property<DateTime>("DateUpdatedUtc")
                        .HasColumnType("timestamp without time zone");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("Claim");
                });

            modelBuilder.Entity("Store.Entities.Identity.ClientEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<bool>("Active")
                        .HasColumnType("boolean");

                    b.Property<string>("AllowedOrigin")
                        .HasColumnType("character varying(100)")
                        .HasMaxLength(100);

                    b.Property<int>("ApplicationType")
                        .HasColumnType("integer");

                    b.Property<DateTime>("DateCreatedUtc")
                        .HasColumnType("timestamp without time zone");

                    b.Property<DateTime>("DateUpdatedUtc")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("character varying(100)")
                        .HasMaxLength(100);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("character varying(50)")
                        .HasMaxLength(50);

                    b.Property<int>("RefreshTokenLifeTime")
                        .HasColumnType("integer");

                    b.Property<string>("Secret")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Client");
                });

            modelBuilder.Entity("Store.Entities.Identity.ExternalLoginEntity", b =>
                {
                    b.Property<string>("LoginProvider")
                        .HasColumnType("varchar")
                        .HasMaxLength(128);

                    b.Property<string>("ProviderKey")
                        .HasColumnType("varchar")
                        .HasMaxLength(128);

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid");

                    b.Property<DateTime>("DateCreatedUtc")
                        .HasColumnType("timestamp without time zone");

                    b.Property<DateTime>("DateUpdatedUtc")
                        .HasColumnType("timestamp without time zone");

                    b.HasKey("LoginProvider", "ProviderKey", "UserId");

                    b.HasIndex("UserId");

                    b.ToTable("ExternalLogin");
                });

            modelBuilder.Entity("Store.Entities.Identity.RefreshTokenEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid>("ClientId")
                        .HasColumnType("uuid");

                    b.Property<DateTime>("DateCreatedUtc")
                        .HasColumnType("timestamp without time zone");

                    b.Property<DateTime>("DateUpdatedUtc")
                        .HasColumnType("timestamp without time zone");

                    b.Property<DateTime>("ExpiresUtc")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("ProtectedTicket")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid");

                    b.Property<string>("Value")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("ClientId");

                    b.HasIndex("UserId");

                    b.ToTable("RefreshToken");
                });

            modelBuilder.Entity("Store.Entities.Identity.RoleEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime>("DateCreatedUtc")
                        .HasColumnType("timestamp without time zone");

                    b.Property<DateTime>("DateUpdatedUtc")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("varchar")
                        .HasMaxLength(256);

                    b.Property<bool>("Stackable")
                        .HasColumnType("boolean");

                    b.HasKey("Id");

                    b.ToTable("Role");
                });

            modelBuilder.Entity("Store.Entities.Identity.UserEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<int>("AccessFailedCount")
                        .HasColumnType("integer");

                    b.Property<DateTime>("DateCreatedUtc")
                        .HasColumnType("timestamp without time zone");

                    b.Property<DateTime>("DateUpdatedUtc")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasColumnType("character varying(50)")
                        .HasMaxLength(50);

                    b.Property<bool>("IsApproved")
                        .HasColumnType("boolean");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("boolean");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasColumnType("character varying(50)")
                        .HasMaxLength(50);

                    b.Property<bool>("LockoutEnabled")
                        .HasColumnType("boolean");

                    b.Property<DateTime?>("LockoutEndDateUtc")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("PasswordHash")
                        .HasColumnType("varchar");

                    b.Property<string>("SecurityStamp")
                        .HasColumnType("varchar");

                    b.Property<string>("UserName")
                        .IsRequired()
                        .HasColumnType("varchar")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.ToTable("User");
                });

            modelBuilder.Entity("Store.Entities.Identity.UserRoleEntity", b =>
                {
                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("RoleId")
                        .HasColumnType("uuid");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("UserRole");
                });

            modelBuilder.Entity("Store.Entities.BookEntity", b =>
                {
                    b.HasOne("Store.Entities.BookstoreEntity", "Bookstore")
                        .WithMany("Books")
                        .HasForeignKey("BookstoreId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Store.Entities.Identity.ClaimEntity", b =>
                {
                    b.HasOne("Store.Entities.Identity.UserEntity", "User")
                        .WithMany("Claims")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Store.Entities.Identity.ExternalLoginEntity", b =>
                {
                    b.HasOne("Store.Entities.Identity.UserEntity", "User")
                        .WithMany("Logins")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Store.Entities.Identity.RefreshTokenEntity", b =>
                {
                    b.HasOne("Store.Entities.Identity.ClientEntity", "Client")
                        .WithMany()
                        .HasForeignKey("ClientId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Store.Entities.Identity.UserEntity", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Store.Entities.Identity.UserRoleEntity", b =>
                {
                    b.HasOne("Store.Entities.Identity.RoleEntity", "Role")
                        .WithMany("Users")
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Store.Entities.Identity.UserEntity", "User")
                        .WithMany("Roles")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}