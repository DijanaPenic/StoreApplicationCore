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
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20201009213416_UpdateLockoutEndDateFieldInUserTable")]
    partial class UpdateLockoutEndDateFieldInUserTable
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

                    b.HasData(
                        new
                        {
                            Id = new Guid("304e51fa-e2dc-4114-bf3f-08d86b04996d"),
                            Author = "J. R. R. Tolkien",
                            BookstoreId = new Guid("61c048ca-028d-4466-b7fd-4a05f0dad647"),
                            DateCreatedUtc = new DateTime(2020, 10, 8, 18, 44, 11, 0, DateTimeKind.Unspecified),
                            DateUpdatedUtc = new DateTime(2020, 10, 8, 18, 44, 11, 0, DateTimeKind.Unspecified),
                            Name = "The Lord of the Rings"
                        },
                        new
                        {
                            Id = new Guid("2d59e1d6-05e8-47a4-bf40-08d86b04996d"),
                            Author = "Paulo Coelho",
                            BookstoreId = new Guid("61c048ca-028d-4466-b7fd-4a05f0dad647"),
                            DateCreatedUtc = new DateTime(2020, 10, 8, 18, 44, 11, 0, DateTimeKind.Unspecified),
                            DateUpdatedUtc = new DateTime(2020, 10, 8, 18, 44, 11, 0, DateTimeKind.Unspecified),
                            Name = "The Alchemist"
                        },
                        new
                        {
                            Id = new Guid("53b9c986-1857-4878-bf41-08d86b04996d"),
                            Author = "Antoine de Saint-Exupéry",
                            BookstoreId = new Guid("61c048ca-028d-4466-b7fd-4a05f0dad647"),
                            DateCreatedUtc = new DateTime(2020, 10, 8, 18, 44, 11, 0, DateTimeKind.Unspecified),
                            DateUpdatedUtc = new DateTime(2020, 10, 8, 18, 44, 11, 0, DateTimeKind.Unspecified),
                            Name = "The Little Prince"
                        });
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

                    b.HasData(
                        new
                        {
                            Id = new Guid("61c048ca-028d-4466-b7fd-4a05f0dad647"),
                            DateCreatedUtc = new DateTime(2020, 10, 8, 18, 44, 11, 0, DateTimeKind.Unspecified),
                            DateUpdatedUtc = new DateTime(2020, 10, 8, 18, 44, 11, 0, DateTimeKind.Unspecified),
                            Location = "2526 E Colfax Ave, Denver, WA",
                            Name = "Strand Book Store"
                        },
                        new
                        {
                            Id = new Guid("a4b57c3c-4c09-4b8c-b2f8-e88ce049f30c"),
                            DateCreatedUtc = new DateTime(2020, 10, 8, 18, 44, 11, 0, DateTimeKind.Unspecified),
                            DateUpdatedUtc = new DateTime(2020, 10, 8, 18, 44, 11, 0, DateTimeKind.Unspecified),
                            Location = "18325 Campus Way NE, Bothell, WA",
                            Name = "Powell's City of Books"
                        },
                        new
                        {
                            Id = new Guid("fa588725-0c60-4554-8eb9-20520038ee87"),
                            DateCreatedUtc = new DateTime(2020, 10, 8, 18, 44, 11, 0, DateTimeKind.Unspecified),
                            DateUpdatedUtc = new DateTime(2020, 10, 8, 18, 44, 11, 0, DateTimeKind.Unspecified),
                            Location = "3415 SW Cedar Hills Blvd, Beaverton, OR",
                            Name = "Shakespeare & Co"
                        });
                });

            modelBuilder.Entity("Store.Entities.Identity.RoleClaimEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("ClaimType")
                        .HasColumnType("text");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("text");

                    b.Property<DateTime>("DateCreatedUtc")
                        .HasColumnType("timestamp without time zone");

                    b.Property<DateTime>("DateUpdatedUtc")
                        .HasColumnType("timestamp without time zone");

                    b.Property<Guid>("RoleId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("RoleClaim");
                });

            modelBuilder.Entity("Store.Entities.Identity.RoleEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("text");

                    b.Property<DateTime>("DateCreatedUtc")
                        .HasColumnType("timestamp without time zone");

                    b.Property<DateTime>("DateUpdatedUtc")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("character varying(256)")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedName")
                        .IsRequired()
                        .HasColumnType("character varying(256)")
                        .HasMaxLength(256);

                    b.Property<bool>("Stackable")
                        .HasColumnType("boolean");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasName("RoleNameIndex");

                    b.ToTable("Role");

                    b.HasData(
                        new
                        {
                            Id = new Guid("d72ef5e5-f08a-4173-b83a-74618893891b"),
                            DateCreatedUtc = new DateTime(2020, 10, 8, 18, 44, 11, 0, DateTimeKind.Unspecified),
                            DateUpdatedUtc = new DateTime(2020, 10, 8, 18, 44, 11, 0, DateTimeKind.Unspecified),
                            Name = "Admin",
                            NormalizedName = "ADMIN",
                            Stackable = false
                        },
                        new
                        {
                            Id = new Guid("d82ef5e5-f08a-4173-b83a-74618893891b"),
                            DateCreatedUtc = new DateTime(2020, 10, 8, 18, 44, 11, 0, DateTimeKind.Unspecified),
                            DateUpdatedUtc = new DateTime(2020, 10, 8, 18, 44, 11, 0, DateTimeKind.Unspecified),
                            Name = "Customer",
                            NormalizedName = "CUSTOMER",
                            Stackable = true
                        },
                        new
                        {
                            Id = new Guid("d92ef5e5-f08a-4173-b83a-74618893891b"),
                            DateCreatedUtc = new DateTime(2020, 10, 8, 18, 44, 11, 0, DateTimeKind.Unspecified),
                            DateUpdatedUtc = new DateTime(2020, 10, 8, 18, 44, 11, 0, DateTimeKind.Unspecified),
                            Name = "Store Manager",
                            NormalizedName = "STORE MANAGER",
                            Stackable = true
                        });
                });

            modelBuilder.Entity("Store.Entities.Identity.UserClaimEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("ClaimType")
                        .HasColumnType("text");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("text");

                    b.Property<DateTime>("DateCreatedUtc")
                        .HasColumnType("timestamp without time zone");

                    b.Property<DateTime>("DateUpdatedUtc")
                        .HasColumnType("timestamp without time zone");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("UserClaim");
                });

            modelBuilder.Entity("Store.Entities.Identity.UserEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<int>("AccessFailedCount")
                        .HasColumnType("integer");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("text");

                    b.Property<DateTime>("DateCreatedUtc")
                        .HasColumnType("timestamp without time zone");

                    b.Property<DateTime>("DateUpdatedUtc")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("character varying(256)")
                        .HasMaxLength(256);

                    b.Property<bool>("EmailConfirmed")
                        .HasColumnType("boolean");

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

                    b.Property<string>("NormalizedEmail")
                        .IsRequired()
                        .HasColumnType("character varying(256)")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedUserName")
                        .IsRequired()
                        .HasColumnType("character varying(256)")
                        .HasMaxLength(256);

                    b.Property<string>("PasswordHash")
                        .HasColumnType("text");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("text");

                    b.Property<bool>("PhoneNumberConfirmed")
                        .HasColumnType("boolean");

                    b.Property<string>("SecurityStamp")
                        .HasColumnType("text");

                    b.Property<bool>("TwoFactorEnabled")
                        .HasColumnType("boolean");

                    b.Property<string>("UserName")
                        .IsRequired()
                        .HasColumnType("character varying(256)")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasName("UserNameIndex");

                    b.ToTable("User");
                });

            modelBuilder.Entity("Store.Entities.Identity.UserLoginEntity", b =>
                {
                    b.Property<string>("LoginProvider")
                        .HasColumnType("character varying(128)")
                        .HasMaxLength(128);

                    b.Property<string>("ProviderKey")
                        .HasColumnType("character varying(128)")
                        .HasMaxLength(128);

                    b.Property<DateTime>("DateCreatedUtc")
                        .HasColumnType("timestamp without time zone");

                    b.Property<DateTime>("DateUpdatedUtc")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("ProviderDisplayName")
                        .HasColumnType("text");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid");

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("UserLogin");
                });

            modelBuilder.Entity("Store.Entities.Identity.UserRoleEntity", b =>
                {
                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("RoleId")
                        .HasColumnType("uuid");

                    b.Property<DateTime>("DateCreatedUtc")
                        .HasColumnType("timestamp without time zone");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("UserRole");
                });

            modelBuilder.Entity("Store.Entities.Identity.UserTokenEntity", b =>
                {
                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid");

                    b.Property<string>("LoginProvider")
                        .HasColumnType("character varying(128)")
                        .HasMaxLength(128);

                    b.Property<string>("Name")
                        .HasColumnType("character varying(128)")
                        .HasMaxLength(128);

                    b.Property<DateTime>("DateCreatedUtc")
                        .HasColumnType("timestamp without time zone");

                    b.Property<DateTime>("DateUpdatedUtc")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("Value")
                        .HasColumnType("text");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("UserToken");
                });

            modelBuilder.Entity("Store.Entities.BookEntity", b =>
                {
                    b.HasOne("Store.Entities.BookstoreEntity", "Bookstore")
                        .WithMany("Books")
                        .HasForeignKey("BookstoreId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Store.Entities.Identity.RoleClaimEntity", b =>
                {
                    b.HasOne("Store.Entities.Identity.RoleEntity", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Store.Entities.Identity.UserClaimEntity", b =>
                {
                    b.HasOne("Store.Entities.Identity.UserEntity", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Store.Entities.Identity.UserLoginEntity", b =>
                {
                    b.HasOne("Store.Entities.Identity.UserEntity", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Store.Entities.Identity.UserRoleEntity", b =>
                {
                    b.HasOne("Store.Entities.Identity.RoleEntity", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Store.Entities.Identity.UserEntity", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Store.Entities.Identity.UserTokenEntity", b =>
                {
                    b.HasOne("Store.Entities.Identity.UserEntity", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
