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
    [Migration("20201223220351_UpdatedEmailTemplateTableWithClientFK")]
    partial class UpdatedEmailTemplateTableWithClientFK
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
                        .HasColumnName("id")
                        .HasColumnType("uuid");

                    b.Property<string>("Author")
                        .IsRequired()
                        .HasColumnName("author")
                        .HasColumnType("character varying(50)")
                        .HasMaxLength(50);

                    b.Property<Guid>("BookstoreId")
                        .HasColumnName("bookstore_id")
                        .HasColumnType("uuid");

                    b.Property<DateTime>("DateCreatedUtc")
                        .HasColumnName("date_created_utc")
                        .HasColumnType("timestamp without time zone");

                    b.Property<DateTime>("DateUpdatedUtc")
                        .HasColumnName("date_updated_utc")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnName("name")
                        .HasColumnType("character varying(50)")
                        .HasMaxLength(50);

                    b.HasKey("Id")
                        .HasName("pk_books");

                    b.HasIndex("BookstoreId")
                        .HasName("ix_books_bookstore_id");

                    b.ToTable("book");

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
                        .HasColumnName("id")
                        .HasColumnType("uuid");

                    b.Property<DateTime>("DateCreatedUtc")
                        .HasColumnName("date_created_utc")
                        .HasColumnType("timestamp without time zone");

                    b.Property<DateTime>("DateUpdatedUtc")
                        .HasColumnName("date_updated_utc")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("Location")
                        .IsRequired()
                        .HasColumnName("location")
                        .HasColumnType("character varying(100)")
                        .HasMaxLength(100);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnName("name")
                        .HasColumnType("character varying(50)")
                        .HasMaxLength(50);

                    b.HasKey("Id")
                        .HasName("pk_bookstores");

                    b.ToTable("bookstore");

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

            modelBuilder.Entity("Store.Entities.EmailTemplateEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("id")
                        .HasColumnType("uuid");

                    b.Property<Guid>("ClientId")
                        .HasColumnName("client_id")
                        .HasColumnType("uuid");

                    b.Property<DateTime>("DateCreatedUtc")
                        .HasColumnName("date_created_utc")
                        .HasColumnType("timestamp without time zone");

                    b.Property<DateTime>("DateUpdatedUtc")
                        .HasColumnName("date_updated_utc")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("Path")
                        .IsRequired()
                        .HasColumnName("path")
                        .HasColumnType("character varying(250)")
                        .HasMaxLength(250);

                    b.Property<int>("Type")
                        .HasColumnName("type")
                        .HasColumnType("integer");

                    b.HasKey("Id")
                        .HasName("pk_email_templates");

                    b.HasIndex("ClientId")
                        .HasName("ix_email_template_client_id");

                    b.ToTable("email_template");
                });

            modelBuilder.Entity("Store.Entities.Identity.ClientEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("id")
                        .HasColumnType("uuid");

                    b.Property<int>("AccessTokenLifeTime")
                        .HasColumnName("access_token_life_time")
                        .HasColumnType("integer");

                    b.Property<bool>("Active")
                        .HasColumnName("active")
                        .HasColumnType("boolean");

                    b.Property<string>("AllowedOrigin")
                        .HasColumnName("allowed_origin")
                        .HasColumnType("character varying(100)")
                        .HasMaxLength(100);

                    b.Property<int>("ApplicationType")
                        .HasColumnName("application_type")
                        .HasColumnType("integer");

                    b.Property<DateTime>("DateCreatedUtc")
                        .HasColumnName("date_created_utc")
                        .HasColumnType("timestamp without time zone");

                    b.Property<DateTime>("DateUpdatedUtc")
                        .HasColumnName("date_updated_utc")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("Description")
                        .HasColumnName("description")
                        .HasColumnType("character varying(100)")
                        .HasMaxLength(100);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnName("name")
                        .HasColumnType("character varying(50)")
                        .HasMaxLength(50);

                    b.Property<int>("RefreshTokenLifeTime")
                        .HasColumnName("refresh_token_life_time")
                        .HasColumnType("integer");

                    b.Property<string>("Secret")
                        .IsRequired()
                        .HasColumnName("secret")
                        .HasColumnType("text");

                    b.HasKey("Id")
                        .HasName("pk_clients");

                    b.HasIndex("Name")
                        .IsUnique()
                        .HasName("NameIndex");

                    b.ToTable("client","identity");

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
                        .HasColumnName("id")
                        .HasColumnType("uuid");

                    b.Property<string>("ClaimType")
                        .HasColumnName("claim_type")
                        .HasColumnType("text");

                    b.Property<string>("ClaimValue")
                        .HasColumnName("claim_value")
                        .HasColumnType("text");

                    b.Property<DateTime>("DateCreatedUtc")
                        .HasColumnName("date_created_utc")
                        .HasColumnType("timestamp without time zone");

                    b.Property<DateTime>("DateUpdatedUtc")
                        .HasColumnName("date_updated_utc")
                        .HasColumnType("timestamp without time zone");

                    b.Property<Guid>("RoleId")
                        .HasColumnName("role_id")
                        .HasColumnType("uuid");

                    b.HasKey("Id")
                        .HasName("pk_role_claims");

                    b.HasIndex("RoleId")
                        .HasName("ix_role_claim_role_id");

                    b.ToTable("role_claim","identity");
                });

            modelBuilder.Entity("Store.Entities.Identity.RoleEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("id")
                        .HasColumnType("uuid");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnName("concurrency_stamp")
                        .HasColumnType("text");

                    b.Property<DateTime>("DateCreatedUtc")
                        .HasColumnName("date_created_utc")
                        .HasColumnType("timestamp without time zone");

                    b.Property<DateTime>("DateUpdatedUtc")
                        .HasColumnName("date_updated_utc")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnName("name")
                        .HasColumnType("character varying(256)")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedName")
                        .IsRequired()
                        .HasColumnName("normalized_name")
                        .HasColumnType("character varying(256)")
                        .HasMaxLength(256);

                    b.Property<bool>("Stackable")
                        .HasColumnName("stackable")
                        .HasColumnType("boolean");

                    b.HasKey("Id")
                        .HasName("pk_roles");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasName("RoleNameIndex");

                    b.ToTable("role","identity");

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
                        .HasColumnName("id")
                        .HasColumnType("uuid");

                    b.Property<string>("ClaimType")
                        .HasColumnName("claim_type")
                        .HasColumnType("text");

                    b.Property<string>("ClaimValue")
                        .HasColumnName("claim_value")
                        .HasColumnType("text");

                    b.Property<DateTime>("DateCreatedUtc")
                        .HasColumnName("date_created_utc")
                        .HasColumnType("timestamp without time zone");

                    b.Property<DateTime>("DateUpdatedUtc")
                        .HasColumnName("date_updated_utc")
                        .HasColumnType("timestamp without time zone");

                    b.Property<Guid>("UserId")
                        .HasColumnName("user_id")
                        .HasColumnType("uuid");

                    b.HasKey("Id")
                        .HasName("pk_user_claims");

                    b.HasIndex("UserId")
                        .HasName("ix_user_claim_user_id");

                    b.ToTable("user_claim","identity");
                });

            modelBuilder.Entity("Store.Entities.Identity.UserEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("id")
                        .HasColumnType("uuid");

                    b.Property<int>("AccessFailedCount")
                        .HasColumnName("access_failed_count")
                        .HasColumnType("integer");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnName("concurrency_stamp")
                        .HasColumnType("text");

                    b.Property<DateTime>("DateCreatedUtc")
                        .HasColumnName("date_created_utc")
                        .HasColumnType("timestamp without time zone");

                    b.Property<DateTime>("DateUpdatedUtc")
                        .HasColumnName("date_updated_utc")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnName("email")
                        .HasColumnType("character varying(256)")
                        .HasMaxLength(256);

                    b.Property<bool>("EmailConfirmed")
                        .HasColumnName("email_confirmed")
                        .HasColumnType("boolean");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasColumnName("first_name")
                        .HasColumnType("character varying(50)")
                        .HasMaxLength(50);

                    b.Property<bool>("IsApproved")
                        .HasColumnName("is_approved")
                        .HasColumnType("boolean");

                    b.Property<bool>("IsDeleted")
                        .HasColumnName("is_deleted")
                        .HasColumnType("boolean");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasColumnName("last_name")
                        .HasColumnType("character varying(50)")
                        .HasMaxLength(50);

                    b.Property<bool>("LockoutEnabled")
                        .HasColumnName("lockout_enabled")
                        .HasColumnType("boolean");

                    b.Property<DateTime?>("LockoutEndDateUtc")
                        .HasColumnName("lockout_end_date_utc")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("NormalizedEmail")
                        .IsRequired()
                        .HasColumnName("normalized_email")
                        .HasColumnType("character varying(256)")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedUserName")
                        .IsRequired()
                        .HasColumnName("normalized_user_name")
                        .HasColumnType("character varying(256)")
                        .HasMaxLength(256);

                    b.Property<string>("PasswordHash")
                        .HasColumnName("password_hash")
                        .HasColumnType("text");

                    b.Property<string>("PhoneNumber")
                        .HasColumnName("phone_number")
                        .HasColumnType("text");

                    b.Property<bool>("PhoneNumberConfirmed")
                        .HasColumnName("phone_number_confirmed")
                        .HasColumnType("boolean");

                    b.Property<string>("SecurityStamp")
                        .HasColumnName("security_stamp")
                        .HasColumnType("text");

                    b.Property<bool>("TwoFactorEnabled")
                        .HasColumnName("two_factor_enabled")
                        .HasColumnType("boolean");

                    b.Property<string>("UserName")
                        .IsRequired()
                        .HasColumnName("user_name")
                        .HasColumnType("character varying(256)")
                        .HasMaxLength(256);

                    b.HasKey("Id")
                        .HasName("pk_users");

                    b.HasIndex("NormalizedEmail")
                        .HasName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasName("UserNameIndex");

                    b.ToTable("user","identity");
                });

            modelBuilder.Entity("Store.Entities.Identity.UserLoginEntity", b =>
                {
                    b.Property<string>("LoginProvider")
                        .HasColumnName("login_provider")
                        .HasColumnType("character varying(128)")
                        .HasMaxLength(128);

                    b.Property<string>("ProviderKey")
                        .HasColumnName("provider_key")
                        .HasColumnType("character varying(128)")
                        .HasMaxLength(128);

                    b.Property<DateTime>("DateCreatedUtc")
                        .HasColumnName("date_created_utc")
                        .HasColumnType("timestamp without time zone");

                    b.Property<DateTime>("DateUpdatedUtc")
                        .HasColumnName("date_updated_utc")
                        .HasColumnType("timestamp without time zone");

                    b.Property<bool>("IsConfirmed")
                        .HasColumnName("is_confirmed")
                        .HasColumnType("boolean");

                    b.Property<string>("ProviderDisplayName")
                        .HasColumnName("provider_display_name")
                        .HasColumnType("text");

                    b.Property<string>("Token")
                        .HasColumnName("token")
                        .HasColumnType("character varying(300)")
                        .HasMaxLength(300);

                    b.Property<Guid>("UserId")
                        .HasColumnName("user_id")
                        .HasColumnType("uuid");

                    b.HasKey("LoginProvider", "ProviderKey")
                        .HasName("pk_user_login");

                    b.HasIndex("UserId")
                        .HasName("ix_user_logins_user_id");

                    b.ToTable("user_login","identity");
                });

            modelBuilder.Entity("Store.Entities.Identity.UserRefreshTokenEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("id")
                        .HasColumnType("uuid");

                    b.Property<Guid>("ClientId")
                        .HasColumnName("client_id")
                        .HasColumnType("uuid");

                    b.Property<DateTime>("DateCreatedUtc")
                        .HasColumnName("date_created_utc")
                        .HasColumnType("timestamp without time zone");

                    b.Property<DateTime>("DateExpiresUtc")
                        .HasColumnName("date_expires_utc")
                        .HasColumnType("timestamp without time zone");

                    b.Property<DateTime>("DateUpdatedUtc")
                        .HasColumnName("date_updated_utc")
                        .HasColumnType("timestamp without time zone");

                    b.Property<Guid>("UserId")
                        .HasColumnName("user_id")
                        .HasColumnType("uuid");

                    b.Property<string>("Value")
                        .IsRequired()
                        .HasColumnName("value")
                        .HasColumnType("character varying(256)")
                        .HasMaxLength(256);

                    b.HasKey("Id")
                        .HasName("pk_user_refresh_tokens");

                    b.HasIndex("ClientId")
                        .HasName("ix_user_refresh_token_client_id");

                    b.HasIndex("UserId")
                        .HasName("ix_user_refresh_token_user_id");

                    b.ToTable("user_refresh_token","identity");
                });

            modelBuilder.Entity("Store.Entities.Identity.UserRoleEntity", b =>
                {
                    b.Property<Guid>("UserId")
                        .HasColumnName("user_id")
                        .HasColumnType("uuid");

                    b.Property<Guid>("RoleId")
                        .HasColumnName("role_id")
                        .HasColumnType("uuid");

                    b.Property<DateTime>("DateCreatedUtc")
                        .HasColumnName("date_created_utc")
                        .HasColumnType("timestamp without time zone");

                    b.HasKey("UserId", "RoleId")
                        .HasName("pk_user_role");

                    b.HasIndex("RoleId")
                        .HasName("ix_user_roles_role_id");

                    b.ToTable("user_role","identity");
                });

            modelBuilder.Entity("Store.Entities.Identity.UserTokenEntity", b =>
                {
                    b.Property<Guid>("UserId")
                        .HasColumnName("user_id")
                        .HasColumnType("uuid");

                    b.Property<string>("LoginProvider")
                        .HasColumnName("login_provider")
                        .HasColumnType("character varying(128)")
                        .HasMaxLength(128);

                    b.Property<string>("Name")
                        .HasColumnName("name")
                        .HasColumnType("character varying(128)")
                        .HasMaxLength(128);

                    b.Property<DateTime>("DateCreatedUtc")
                        .HasColumnName("date_created_utc")
                        .HasColumnType("timestamp without time zone");

                    b.Property<DateTime>("DateUpdatedUtc")
                        .HasColumnName("date_updated_utc")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("Value")
                        .HasColumnName("value")
                        .HasColumnType("text");

                    b.HasKey("UserId", "LoginProvider", "Name")
                        .HasName("pk_user_token");

                    b.ToTable("user_token","identity");
                });

            modelBuilder.Entity("Store.Entities.BookEntity", b =>
                {
                    b.HasOne("Store.Entities.BookstoreEntity", "Bookstore")
                        .WithMany("Books")
                        .HasForeignKey("BookstoreId")
                        .HasConstraintName("fk_books_bookstores_bookstore_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Store.Entities.EmailTemplateEntity", b =>
                {
                    b.HasOne("Store.Entities.Identity.ClientEntity", null)
                        .WithMany()
                        .HasForeignKey("ClientId")
                        .HasConstraintName("fk_email_template_client_client_entity_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Store.Entities.Identity.RoleClaimEntity", b =>
                {
                    b.HasOne("Store.Entities.Identity.RoleEntity", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .HasConstraintName("fk_role_claim_role_role_entity_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Store.Entities.Identity.UserClaimEntity", b =>
                {
                    b.HasOne("Store.Entities.Identity.UserEntity", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .HasConstraintName("fk_user_claim_user_user_entity_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Store.Entities.Identity.UserLoginEntity", b =>
                {
                    b.HasOne("Store.Entities.Identity.UserEntity", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .HasConstraintName("fk_user_logins_user_user_entity_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Store.Entities.Identity.UserRefreshTokenEntity", b =>
                {
                    b.HasOne("Store.Entities.Identity.ClientEntity", null)
                        .WithMany()
                        .HasForeignKey("ClientId")
                        .HasConstraintName("fk_user_refresh_token_clients_client_entity_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Store.Entities.Identity.UserEntity", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .HasConstraintName("fk_user_refresh_token_user_user_entity_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Store.Entities.Identity.UserRoleEntity", b =>
                {
                    b.HasOne("Store.Entities.Identity.RoleEntity", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .HasConstraintName("fk_user_roles_role_role_entity_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Store.Entities.Identity.UserEntity", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .HasConstraintName("fk_user_roles_user_user_entity_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Store.Entities.Identity.UserTokenEntity", b =>
                {
                    b.HasOne("Store.Entities.Identity.UserEntity", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .HasConstraintName("fk_user_tokens_user_user_entity_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
