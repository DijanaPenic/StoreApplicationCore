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
    [Migration("20210612114605_RemovedDataSeed")]
    partial class RemovedDataSeed
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .UseIdentityByDefaultColumns()
                .HasAnnotation("Relational:MaxIdentifierLength", 63)
                .HasAnnotation("ProductVersion", "5.0.1");

            modelBuilder.Entity("Store.Entities.BookEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<string>("Author")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)")
                        .HasColumnName("author");

                    b.Property<Guid>("BookstoreId")
                        .HasColumnType("uuid")
                        .HasColumnName("bookstore_id");

                    b.Property<DateTime>("DateCreatedUtc")
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("date_created_utc");

                    b.Property<DateTime>("DateUpdatedUtc")
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("date_updated_utc");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)")
                        .HasColumnName("name");

                    b.HasKey("Id")
                        .HasName("pk_books");

                    b.HasIndex("BookstoreId")
                        .HasDatabaseName("ix_books_bookstore_id");

                    b.ToTable("book");
                });

            modelBuilder.Entity("Store.Entities.BookstoreEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<DateTime>("DateCreatedUtc")
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("date_created_utc");

                    b.Property<DateTime>("DateUpdatedUtc")
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("date_updated_utc");

                    b.Property<string>("Location")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)")
                        .HasColumnName("location");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)")
                        .HasColumnName("name");

                    b.HasKey("Id")
                        .HasName("pk_bookstores");

                    b.ToTable("bookstore");
                });

            modelBuilder.Entity("Store.Entities.EmailTemplateEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<Guid>("ClientId")
                        .HasColumnType("uuid")
                        .HasColumnName("client_id");

                    b.Property<DateTime>("DateCreatedUtc")
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("date_created_utc");

                    b.Property<DateTime>("DateUpdatedUtc")
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("date_updated_utc");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)")
                        .HasColumnName("name");

                    b.Property<string>("Path")
                        .IsRequired()
                        .HasMaxLength(250)
                        .HasColumnType("character varying(250)")
                        .HasColumnName("path");

                    b.Property<int>("Type")
                        .HasColumnType("integer")
                        .HasColumnName("type");

                    b.HasKey("Id")
                        .HasName("pk_email_templates");

                    b.HasIndex("ClientId")
                        .HasDatabaseName("ix_email_templates_client_id");

                    b.ToTable("email_template");
                });

            modelBuilder.Entity("Store.Entities.Identity.ClientEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<int>("AccessTokenLifeTime")
                        .HasColumnType("integer")
                        .HasColumnName("access_token_life_time");

                    b.Property<bool>("Active")
                        .HasColumnType("boolean")
                        .HasColumnName("active");

                    b.Property<string>("AllowedOrigin")
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)")
                        .HasColumnName("allowed_origin");

                    b.Property<int>("ApplicationType")
                        .HasColumnType("integer")
                        .HasColumnName("application_type");

                    b.Property<DateTime>("DateCreatedUtc")
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("date_created_utc");

                    b.Property<DateTime>("DateUpdatedUtc")
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("date_updated_utc");

                    b.Property<string>("Description")
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)")
                        .HasColumnName("description");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)")
                        .HasColumnName("name");

                    b.Property<int>("RefreshTokenLifeTime")
                        .HasColumnType("integer")
                        .HasColumnName("refresh_token_life_time");

                    b.Property<string>("Secret")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("secret");

                    b.HasKey("Id")
                        .HasName("pk_clients");

                    b.HasIndex("Name")
                        .IsUnique()
                        .HasDatabaseName("NameIndex");

                    b.ToTable("client", "identity");

                    b.HasData(
                        new
                        {
                            Id = new Guid("5c52160a-4ab4-49c6-ba5f-56df9c5730b6"),
                            AccessTokenLifeTime = 20,
                            Active = true,
                            AllowedOrigin = "*",
                            ApplicationType = 1,
                            DateCreatedUtc = new DateTime(2020, 10, 25, 16, 44, 0, 0, DateTimeKind.Unspecified),
                            DateUpdatedUtc = new DateTime(2020, 10, 25, 16, 44, 0, 0, DateTimeKind.Unspecified),
                            Description = "Web API Application",
                            Name = "WebApiApplication",
                            RefreshTokenLifeTime = 60,
                            Secret = "PX23zsV/7nm6+ZI9LmrKXSBf2O47cYtiJGk2WJ/G/PdU2eD7Y929MZeItkGpBY2v6a2tXhGINq8bAQYz1bQC6A=="
                        });
                });

            modelBuilder.Entity("Store.Entities.Identity.RoleClaimEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<string>("ClaimType")
                        .HasColumnType("text")
                        .HasColumnName("claim_type");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("text")
                        .HasColumnName("claim_value");

                    b.Property<DateTime>("DateCreatedUtc")
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("date_created_utc");

                    b.Property<DateTime>("DateUpdatedUtc")
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("date_updated_utc");

                    b.Property<Guid>("RoleId")
                        .HasColumnType("uuid")
                        .HasColumnName("role_id");

                    b.HasKey("Id")
                        .HasName("pk_role_claims");

                    b.HasIndex("RoleId")
                        .HasDatabaseName("ix_role_claims_role_id");

                    b.ToTable("role_claim", "identity");
                });

            modelBuilder.Entity("Store.Entities.Identity.RoleEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("text")
                        .HasColumnName("concurrency_stamp");

                    b.Property<DateTime>("DateCreatedUtc")
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("date_created_utc");

                    b.Property<DateTime>("DateUpdatedUtc")
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("date_updated_utc");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)")
                        .HasColumnName("name");

                    b.Property<string>("NormalizedName")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)")
                        .HasColumnName("normalized_name");

                    b.Property<bool>("Stackable")
                        .HasColumnType("boolean")
                        .HasColumnName("stackable");

                    b.HasKey("Id")
                        .HasName("pk_roles");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasDatabaseName("RoleNameIndex");

                    b.ToTable("role", "identity");

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
                        },
                        new
                        {
                            Id = new Guid("9621c09c-06b1-45fb-8baf-38e0757e2f59"),
                            DateCreatedUtc = new DateTime(2020, 10, 30, 18, 44, 11, 0, DateTimeKind.Unspecified),
                            DateUpdatedUtc = new DateTime(2020, 10, 30, 18, 44, 11, 0, DateTimeKind.Unspecified),
                            Name = "Guest",
                            NormalizedName = "GUEST",
                            Stackable = false
                        });
                });

            modelBuilder.Entity("Store.Entities.Identity.UserClaimEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<string>("ClaimType")
                        .HasColumnType("text")
                        .HasColumnName("claim_type");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("text")
                        .HasColumnName("claim_value");

                    b.Property<DateTime>("DateCreatedUtc")
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("date_created_utc");

                    b.Property<DateTime>("DateUpdatedUtc")
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("date_updated_utc");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid")
                        .HasColumnName("user_id");

                    b.HasKey("Id")
                        .HasName("pk_user_claims");

                    b.HasIndex("UserId")
                        .HasDatabaseName("ix_user_claims_user_id");

                    b.ToTable("user_claim", "identity");
                });

            modelBuilder.Entity("Store.Entities.Identity.UserEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<int>("AccessFailedCount")
                        .HasColumnType("integer")
                        .HasColumnName("access_failed_count");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("text")
                        .HasColumnName("concurrency_stamp");

                    b.Property<DateTime>("DateCreatedUtc")
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("date_created_utc");

                    b.Property<DateTime>("DateUpdatedUtc")
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("date_updated_utc");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)")
                        .HasColumnName("email");

                    b.Property<bool>("EmailConfirmed")
                        .HasColumnType("boolean")
                        .HasColumnName("email_confirmed");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)")
                        .HasColumnName("first_name");

                    b.Property<bool>("IsApproved")
                        .HasColumnType("boolean")
                        .HasColumnName("is_approved");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)")
                        .HasColumnName("last_name");

                    b.Property<bool>("LockoutEnabled")
                        .HasColumnType("boolean")
                        .HasColumnName("lockout_enabled");

                    b.Property<DateTime?>("LockoutEndDateUtc")
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("lockout_end_date_utc");

                    b.Property<string>("NormalizedEmail")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)")
                        .HasColumnName("normalized_email");

                    b.Property<string>("NormalizedUserName")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)")
                        .HasColumnName("normalized_user_name");

                    b.Property<string>("PasswordHash")
                        .HasColumnType("text")
                        .HasColumnName("password_hash");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("text")
                        .HasColumnName("phone_number");

                    b.Property<bool>("PhoneNumberConfirmed")
                        .HasColumnType("boolean")
                        .HasColumnName("phone_number_confirmed");

                    b.Property<string>("SecurityStamp")
                        .HasColumnType("text")
                        .HasColumnName("security_stamp");

                    b.Property<bool>("TwoFactorEnabled")
                        .HasColumnType("boolean")
                        .HasColumnName("two_factor_enabled");

                    b.Property<string>("UserName")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)")
                        .HasColumnName("user_name");

                    b.HasKey("Id")
                        .HasName("pk_users");

                    b.HasIndex("NormalizedEmail")
                        .HasDatabaseName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasDatabaseName("UserNameIndex");

                    b.ToTable("user", "identity");
                });

            modelBuilder.Entity("Store.Entities.Identity.UserLoginEntity", b =>
                {
                    b.Property<string>("LoginProvider")
                        .HasMaxLength(128)
                        .HasColumnType("character varying(128)")
                        .HasColumnName("login_provider");

                    b.Property<string>("ProviderKey")
                        .HasMaxLength(128)
                        .HasColumnType("character varying(128)")
                        .HasColumnName("provider_key");

                    b.Property<DateTime>("DateCreatedUtc")
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("date_created_utc");

                    b.Property<DateTime>("DateUpdatedUtc")
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("date_updated_utc");

                    b.Property<bool>("IsConfirmed")
                        .HasColumnType("boolean")
                        .HasColumnName("is_confirmed");

                    b.Property<string>("ProviderDisplayName")
                        .HasColumnType("text")
                        .HasColumnName("provider_display_name");

                    b.Property<string>("Token")
                        .HasMaxLength(300)
                        .HasColumnType("character varying(300)")
                        .HasColumnName("token");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid")
                        .HasColumnName("user_id");

                    b.HasKey("LoginProvider", "ProviderKey")
                        .HasName("pk_user_login");

                    b.HasIndex("UserId")
                        .HasDatabaseName("ix_user_logins_user_id");

                    b.ToTable("user_login", "identity");
                });

            modelBuilder.Entity("Store.Entities.Identity.UserRefreshTokenEntity", b =>
                {
                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid")
                        .HasColumnName("user_id");

                    b.Property<Guid>("ClientId")
                        .HasColumnType("uuid")
                        .HasColumnName("client_id");

                    b.Property<DateTime>("DateCreatedUtc")
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("date_created_utc");

                    b.Property<DateTime>("DateExpiresUtc")
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("date_expires_utc");

                    b.Property<DateTime>("DateUpdatedUtc")
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("date_updated_utc");

                    b.Property<string>("Value")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)")
                        .HasColumnName("value");

                    b.HasKey("UserId", "ClientId")
                        .HasName("pk_user_refresh_token");

                    b.HasIndex("ClientId")
                        .HasDatabaseName("ix_user_refresh_tokens_client_id");

                    b.ToTable("user_refresh_token", "identity");
                });

            modelBuilder.Entity("Store.Entities.Identity.UserTokenEntity", b =>
                {
                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid")
                        .HasColumnName("user_id");

                    b.Property<string>("LoginProvider")
                        .HasMaxLength(128)
                        .HasColumnType("character varying(128)")
                        .HasColumnName("login_provider");

                    b.Property<string>("Name")
                        .HasMaxLength(128)
                        .HasColumnType("character varying(128)")
                        .HasColumnName("name");

                    b.Property<DateTime>("DateCreatedUtc")
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("date_created_utc");

                    b.Property<DateTime>("DateUpdatedUtc")
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("date_updated_utc");

                    b.Property<string>("Value")
                        .HasColumnType("text")
                        .HasColumnName("value");

                    b.HasKey("UserId", "LoginProvider", "Name")
                        .HasName("pk_user_token");

                    b.ToTable("user_token", "identity");
                });

            modelBuilder.Entity("user_role", b =>
                {
                    b.Property<Guid>("RoleId")
                        .HasColumnType("uuid")
                        .HasColumnName("role_id");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid")
                        .HasColumnName("user_id");

                    b.Property<DateTime>("DateCreatedUtc")
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("date_created_utc");

                    b.HasKey("RoleId", "UserId")
                        .HasName("pk_user_role");

                    b.HasIndex("UserId")
                        .HasDatabaseName("ix_user_role_user_id");

                    b.ToTable("user_role", "identity");
                });

            modelBuilder.Entity("Store.Entities.BookEntity", b =>
                {
                    b.HasOne("Store.Entities.BookstoreEntity", "Bookstore")
                        .WithMany("Books")
                        .HasForeignKey("BookstoreId")
                        .HasConstraintName("fk_books_bookstores_bookstore_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Bookstore");
                });

            modelBuilder.Entity("Store.Entities.EmailTemplateEntity", b =>
                {
                    b.HasOne("Store.Entities.Identity.ClientEntity", "Client")
                        .WithMany("EmailTemplates")
                        .HasForeignKey("ClientId")
                        .HasConstraintName("fk_email_templates_clients_client_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Client");
                });

            modelBuilder.Entity("Store.Entities.Identity.RoleClaimEntity", b =>
                {
                    b.HasOne("Store.Entities.Identity.RoleEntity", "Role")
                        .WithMany("Claims")
                        .HasForeignKey("RoleId")
                        .HasConstraintName("fk_role_claims_roles_role_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Role");
                });

            modelBuilder.Entity("Store.Entities.Identity.UserClaimEntity", b =>
                {
                    b.HasOne("Store.Entities.Identity.UserEntity", "User")
                        .WithMany("Claims")
                        .HasForeignKey("UserId")
                        .HasConstraintName("fk_user_claims_users_user_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("Store.Entities.Identity.UserLoginEntity", b =>
                {
                    b.HasOne("Store.Entities.Identity.UserEntity", "User")
                        .WithMany("Logins")
                        .HasForeignKey("UserId")
                        .HasConstraintName("fk_user_logins_users_user_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("Store.Entities.Identity.UserRefreshTokenEntity", b =>
                {
                    b.HasOne("Store.Entities.Identity.ClientEntity", "Client")
                        .WithMany("RefreshTokens")
                        .HasForeignKey("ClientId")
                        .HasConstraintName("fk_user_refresh_tokens_clients_client_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Store.Entities.Identity.UserEntity", "User")
                        .WithMany("RefreshTokens")
                        .HasForeignKey("UserId")
                        .HasConstraintName("fk_user_refresh_tokens_users_user_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Client");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Store.Entities.Identity.UserTokenEntity", b =>
                {
                    b.HasOne("Store.Entities.Identity.UserEntity", "User")
                        .WithMany("UserTokens")
                        .HasForeignKey("UserId")
                        .HasConstraintName("fk_user_tokens_users_user_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("user_role", b =>
                {
                    b.HasOne("Store.Entities.Identity.RoleEntity", "Role")
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .HasConstraintName("fk_user_role_roles_role_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Store.Entities.Identity.UserEntity", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .HasConstraintName("fk_user_role_users_user_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Role");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Store.Entities.BookstoreEntity", b =>
                {
                    b.Navigation("Books");
                });

            modelBuilder.Entity("Store.Entities.Identity.ClientEntity", b =>
                {
                    b.Navigation("EmailTemplates");

                    b.Navigation("RefreshTokens");
                });

            modelBuilder.Entity("Store.Entities.Identity.RoleEntity", b =>
                {
                    b.Navigation("Claims");
                });

            modelBuilder.Entity("Store.Entities.Identity.UserEntity", b =>
                {
                    b.Navigation("Claims");

                    b.Navigation("Logins");

                    b.Navigation("RefreshTokens");

                    b.Navigation("UserTokens");
                });
#pragma warning restore 612, 618
        }
    }
}