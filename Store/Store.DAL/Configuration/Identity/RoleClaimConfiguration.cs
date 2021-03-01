using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Store.Entities.Identity;

namespace Store.DAL.Configuration.Identity
{
    internal class RoleClaimConfiguration : IEntityTypeConfiguration<RoleClaimEntity> 
    {
        public void Configure(EntityTypeBuilder<RoleClaimEntity> builder)
        {
            // Maps to the RoleClaim table
            builder.ToTable("role_claim", "identity");

            // Primary key
            builder.HasKey(rc => rc.Id);

            // Seed data
            builder.HasData
            (
                // ADMIN

                new RoleClaimEntity
                {
                    Id = Guid.Parse("31fc1523-8653-4bbc-acab-dee44688439a"),
                    RoleId = Guid.Parse("d72ef5e5-f08a-4173-b83a-74618893891b"),
                    ClaimType = "Permission",
                    ClaimValue = "Book.Read",
                    DateCreatedUtc = DateTime.Parse("01-Mar-21 8:39:00 PM"),
                    DateUpdatedUtc = DateTime.Parse("01-Mar-21 8:39:00 PM")
                },
                new RoleClaimEntity
                {
                    Id = Guid.Parse("d7cca48d-8c8c-4e58-9354-526b1bbcd801"),
                    RoleId = Guid.Parse("d72ef5e5-f08a-4173-b83a-74618893891b"),
                    ClaimType = "Permission",
                    ClaimValue = "Book.Update",
                    DateCreatedUtc = DateTime.Parse("01-Mar-21 8:39:00 PM"),
                    DateUpdatedUtc = DateTime.Parse("01-Mar-21 8:39:00 PM")
                },
                new RoleClaimEntity
                {
                    Id = Guid.Parse("75a033b5-074d-4a50-b2dd-d934dfc35018"),
                    RoleId = Guid.Parse("d72ef5e5-f08a-4173-b83a-74618893891b"),
                    ClaimType = "Permission",
                    ClaimValue = "Book.Create",
                    DateCreatedUtc = DateTime.Parse("01-Mar-21 8:39:00 PM"),
                    DateUpdatedUtc = DateTime.Parse("01-Mar-21 8:39:00 PM")
                },
                new RoleClaimEntity
                {
                    Id = Guid.Parse("71e14223-8ceb-4a37-8696-3dedc476fb8e"),
                    RoleId = Guid.Parse("d72ef5e5-f08a-4173-b83a-74618893891b"),
                    ClaimType = "Permission",
                    ClaimValue = "Book.Delete",
                    DateCreatedUtc = DateTime.Parse("01-Mar-21 8:39:00 PM"),
                    DateUpdatedUtc = DateTime.Parse("01-Mar-21 8:39:00 PM")
                },
                new RoleClaimEntity
                {
                    Id = Guid.Parse("2cc7acb8-0b62-4cd4-9139-b0ad220678e4"),
                    RoleId = Guid.Parse("d72ef5e5-f08a-4173-b83a-74618893891b"),
                    ClaimType = "Permission",
                    ClaimValue = "Book.Full",
                    DateCreatedUtc = DateTime.Parse("01-Mar-21 8:39:00 PM"),
                    DateUpdatedUtc = DateTime.Parse("01-Mar-21 8:39:00 PM")
                },
                new RoleClaimEntity
                {
                    Id = Guid.Parse("28fc2b87-eee6-4cbd-847b-91c0da1751a2"),
                    RoleId = Guid.Parse("d72ef5e5-f08a-4173-b83a-74618893891b"),
                    ClaimType = "Permission",
                    ClaimValue = "Bookstore.Read",
                    DateCreatedUtc = DateTime.Parse("01-Mar-21 8:39:00 PM"),
                    DateUpdatedUtc = DateTime.Parse("01-Mar-21 8:39:00 PM")
                },
                new RoleClaimEntity
                {
                    Id = Guid.Parse("3e7a5a9f-efd5-41c9-97bc-db0e438da4d5"),
                    RoleId = Guid.Parse("d72ef5e5-f08a-4173-b83a-74618893891b"),
                    ClaimType = "Permission",
                    ClaimValue = "Bookstore.Update",
                    DateCreatedUtc = DateTime.Parse("01-Mar-21 8:39:00 PM"),
                    DateUpdatedUtc = DateTime.Parse("01-Mar-21 8:39:00 PM")
                },
                new RoleClaimEntity
                {
                    Id = Guid.Parse("62589436-f138-4375-a4b0-fb19048efc6c"),
                    RoleId = Guid.Parse("d72ef5e5-f08a-4173-b83a-74618893891b"),
                    ClaimType = "Permission",
                    ClaimValue = "Bookstore.Create",
                    DateCreatedUtc = DateTime.Parse("01-Mar-21 8:39:00 PM"),
                    DateUpdatedUtc = DateTime.Parse("01-Mar-21 8:39:00 PM")
                },
                new RoleClaimEntity
                {
                    Id = Guid.Parse("c2810def-d14f-451c-8dde-cc9a0e2ad04c"),
                    RoleId = Guid.Parse("d72ef5e5-f08a-4173-b83a-74618893891b"),
                    ClaimType = "Permission",
                    ClaimValue = "Bookstore.Delete",
                    DateCreatedUtc = DateTime.Parse("01-Mar-21 8:39:00 PM"),
                    DateUpdatedUtc = DateTime.Parse("01-Mar-21 8:39:00 PM")
                },
                new RoleClaimEntity
                {
                    Id = Guid.Parse("9a72cefd-056b-4507-9130-06e680c6bbc7"),
                    RoleId = Guid.Parse("d72ef5e5-f08a-4173-b83a-74618893891b"),
                    ClaimType = "Permission",
                    ClaimValue = "Bookstore.Full",
                    DateCreatedUtc = DateTime.Parse("01-Mar-21 8:39:00 PM"),
                    DateUpdatedUtc = DateTime.Parse("01-Mar-21 8:39:00 PM")
                },
                new RoleClaimEntity
                {
                    Id = Guid.Parse("83cf48ff-6a60-4565-bf5c-6470870f7efd"),
                    RoleId = Guid.Parse("d72ef5e5-f08a-4173-b83a-74618893891b"),
                    ClaimType = "Permission",
                    ClaimValue = "User.Read",
                    DateCreatedUtc = DateTime.Parse("01-Mar-21 8:39:00 PM"),
                    DateUpdatedUtc = DateTime.Parse("01-Mar-21 8:39:00 PM")
                },
                new RoleClaimEntity
                {
                    Id = Guid.Parse("ec34092b-38fa-4df7-a2e4-d313d79f0a3d"),
                    RoleId = Guid.Parse("d72ef5e5-f08a-4173-b83a-74618893891b"),
                    ClaimType = "Permission",
                    ClaimValue = "User.Update",
                    DateCreatedUtc = DateTime.Parse("01-Mar-21 8:39:00 PM"),
                    DateUpdatedUtc = DateTime.Parse("01-Mar-21 8:39:00 PM")
                },
                new RoleClaimEntity
                {
                    Id = Guid.Parse("a6bf0136-feb7-480b-b518-64f65ed70fa5"),
                    RoleId = Guid.Parse("d72ef5e5-f08a-4173-b83a-74618893891b"),
                    ClaimType = "Permission",
                    ClaimValue = "User.Create",
                    DateCreatedUtc = DateTime.Parse("01-Mar-21 8:39:00 PM"),
                    DateUpdatedUtc = DateTime.Parse("01-Mar-21 8:39:00 PM")
                },
                new RoleClaimEntity
                {
                    Id = Guid.Parse("0fd82be4-fad9-44a6-bf1e-5369a5f7ce0d"),
                    RoleId = Guid.Parse("d72ef5e5-f08a-4173-b83a-74618893891b"),
                    ClaimType = "Permission",
                    ClaimValue = "User.Delete",
                    DateCreatedUtc = DateTime.Parse("01-Mar-21 8:39:00 PM"),
                    DateUpdatedUtc = DateTime.Parse("01-Mar-21 8:39:00 PM")
                },
                new RoleClaimEntity
                {
                    Id = Guid.Parse("b236e988-e237-4367-823b-55d53e9d591e"),
                    RoleId = Guid.Parse("d72ef5e5-f08a-4173-b83a-74618893891b"),
                    ClaimType = "Permission",
                    ClaimValue = "User.Full",
                    DateCreatedUtc = DateTime.Parse("01-Mar-21 8:39:00 PM"),
                    DateUpdatedUtc = DateTime.Parse("01-Mar-21 8:39:00 PM")
                },
                new RoleClaimEntity
                {
                    Id = Guid.Parse("ee04a7d9-ad9c-4f4a-9820-d55fa540c632"),
                    RoleId = Guid.Parse("d72ef5e5-f08a-4173-b83a-74618893891b"),
                    ClaimType = "Permission",
                    ClaimValue = "Role.Read",
                    DateCreatedUtc = DateTime.Parse("01-Mar-21 8:39:00 PM"),
                    DateUpdatedUtc = DateTime.Parse("01-Mar-21 8:39:00 PM")
                },
                new RoleClaimEntity
                {
                    Id = Guid.Parse("1d2674ad-f820-4222-897b-4e48bba43d3d"),
                    RoleId = Guid.Parse("d72ef5e5-f08a-4173-b83a-74618893891b"),
                    ClaimType = "Permission",
                    ClaimValue = "Role.Update",
                    DateCreatedUtc = DateTime.Parse("01-Mar-21 8:39:00 PM"),
                    DateUpdatedUtc = DateTime.Parse("01-Mar-21 8:39:00 PM")
                },
                new RoleClaimEntity
                {
                    Id = Guid.Parse("5bfb886c-da71-40ae-893e-aba0f5fca319"),
                    RoleId = Guid.Parse("d72ef5e5-f08a-4173-b83a-74618893891b"),
                    ClaimType = "Permission",
                    ClaimValue = "Role.Create",
                    DateCreatedUtc = DateTime.Parse("01-Mar-21 8:39:00 PM"),
                    DateUpdatedUtc = DateTime.Parse("01-Mar-21 8:39:00 PM")
                },
                new RoleClaimEntity
                {
                    Id = Guid.Parse("fd843c83-ddeb-4ca9-957f-333ee739402c"),
                    RoleId = Guid.Parse("d72ef5e5-f08a-4173-b83a-74618893891b"),
                    ClaimType = "Permission",
                    ClaimValue = "Role.Delete",
                    DateCreatedUtc = DateTime.Parse("01-Mar-21 8:39:00 PM"),
                    DateUpdatedUtc = DateTime.Parse("01-Mar-21 8:39:00 PM")
                },
                new RoleClaimEntity
                {
                    Id = Guid.Parse("171c4701-98e8-4b74-af5e-eda937dbdb24"),
                    RoleId = Guid.Parse("d72ef5e5-f08a-4173-b83a-74618893891b"),
                    ClaimType = "Permission",
                    ClaimValue = "Role.Full",
                    DateCreatedUtc = DateTime.Parse("01-Mar-21 8:39:00 PM"),
                    DateUpdatedUtc = DateTime.Parse("01-Mar-21 8:39:00 PM")
                },
                new RoleClaimEntity
                {
                    Id = Guid.Parse("c0aada02-252d-4231-ae31-4445736c39ea"),
                    RoleId = Guid.Parse("d72ef5e5-f08a-4173-b83a-74618893891b"),
                    ClaimType = "Permission",
                    ClaimValue = "EmailTemplate.Read",
                    DateCreatedUtc = DateTime.Parse("01-Mar-21 8:39:00 PM"),
                    DateUpdatedUtc = DateTime.Parse("01-Mar-21 8:39:00 PM")
                },
                new RoleClaimEntity
                {
                    Id = Guid.Parse("4cc53412-561a-4d29-ac1b-22dd3cf18c32"),
                    RoleId = Guid.Parse("d72ef5e5-f08a-4173-b83a-74618893891b"),
                    ClaimType = "Permission",
                    ClaimValue = "EmailTemplate.Update",
                    DateCreatedUtc = DateTime.Parse("01-Mar-21 8:39:00 PM"),
                    DateUpdatedUtc = DateTime.Parse("01-Mar-21 8:39:00 PM")
                },
                new RoleClaimEntity
                {
                    Id = Guid.Parse("b5695386-1b56-4e77-87ab-6b58c16f5594"),
                    RoleId = Guid.Parse("d72ef5e5-f08a-4173-b83a-74618893891b"),
                    ClaimType = "Permission",
                    ClaimValue = "EmailTemplate.Create",
                    DateCreatedUtc = DateTime.Parse("01-Mar-21 8:39:00 PM"),
                    DateUpdatedUtc = DateTime.Parse("01-Mar-21 8:39:00 PM")
                },
                new RoleClaimEntity
                {
                    Id = Guid.Parse("f6534a40-cb9a-43a9-b00f-01471fff8c73"),
                    RoleId = Guid.Parse("d72ef5e5-f08a-4173-b83a-74618893891b"),
                    ClaimType = "Permission",
                    ClaimValue = "EmailTemplate.Delete",
                    DateCreatedUtc = DateTime.Parse("01-Mar-21 8:39:00 PM"),
                    DateUpdatedUtc = DateTime.Parse("01-Mar-21 8:39:00 PM")
                },
                new RoleClaimEntity
                {
                    Id = Guid.Parse("ee777566-7a40-4aa1-9923-2758a38e777a"),
                    RoleId = Guid.Parse("d72ef5e5-f08a-4173-b83a-74618893891b"),
                    ClaimType = "Permission",
                    ClaimValue = "EmailTemplate.Full",
                    DateCreatedUtc = DateTime.Parse("01-Mar-21 8:39:00 PM"),
                    DateUpdatedUtc = DateTime.Parse("01-Mar-21 8:39:00 PM")
                },

                // CUSTOMER

                new RoleClaimEntity
                {
                    Id = Guid.Parse("3f3b40be-8f86-4e79-adf1-8ba429f73c89"),
                    RoleId = Guid.Parse("d82ef5e5-f08a-4173-b83a-74618893891b"),
                    ClaimType = "Permission",
                    ClaimValue = "Book.Read",
                    DateCreatedUtc = DateTime.Parse("01-Mar-21 8:39:00 PM"),
                    DateUpdatedUtc = DateTime.Parse("01-Mar-21 8:39:00 PM")
                },
                new RoleClaimEntity
                {
                    Id = Guid.Parse("399fc9fb-bd70-4bf9-8dd4-a45e25630d8a"),
                    RoleId = Guid.Parse("d82ef5e5-f08a-4173-b83a-74618893891b"),
                    ClaimType = "Permission",
                    ClaimValue = "Bookstore.Read",
                    DateCreatedUtc = DateTime.Parse("01-Mar-21 8:39:00 PM"),
                    DateUpdatedUtc = DateTime.Parse("01-Mar-21 8:39:00 PM")
                },

                // STORE MANAGER

                new RoleClaimEntity
                {
                    Id = Guid.Parse("c8ad649a-2019-43e7-8a33-786d2963c4cc"),
                    RoleId = Guid.Parse("d92ef5e5-f08a-4173-b83a-74618893891b"),
                    ClaimType = "Permission",
                    ClaimValue = "Book.Read",
                    DateCreatedUtc = DateTime.Parse("01-Mar-21 8:39:00 PM"),
                    DateUpdatedUtc = DateTime.Parse("01-Mar-21 8:39:00 PM")
                },
                new RoleClaimEntity
                {
                    Id = Guid.Parse("6775734b-677b-4430-bbb9-1495d8991419"),
                    RoleId = Guid.Parse("d92ef5e5-f08a-4173-b83a-74618893891b"),
                    ClaimType = "Permission",
                    ClaimValue = "Book.Update",
                    DateCreatedUtc = DateTime.Parse("01-Mar-21 8:39:00 PM"),
                    DateUpdatedUtc = DateTime.Parse("01-Mar-21 8:39:00 PM")
                },
                new RoleClaimEntity
                {
                    Id = Guid.Parse("43f588d9-5a6c-48c3-900b-5a9d6a643637"),
                    RoleId = Guid.Parse("d92ef5e5-f08a-4173-b83a-74618893891b"),
                    ClaimType = "Permission",
                    ClaimValue = "Book.Create",
                    DateCreatedUtc = DateTime.Parse("01-Mar-21 8:39:00 PM"),
                    DateUpdatedUtc = DateTime.Parse("01-Mar-21 8:39:00 PM")
                },
                new RoleClaimEntity
                {
                    Id = Guid.Parse("b1e0cbc5-f04f-4a43-bab6-2448d144dbfc"),
                    RoleId = Guid.Parse("d92ef5e5-f08a-4173-b83a-74618893891b"),
                    ClaimType = "Permission",
                    ClaimValue = "Book.Delete",
                    DateCreatedUtc = DateTime.Parse("01-Mar-21 8:39:00 PM"),
                    DateUpdatedUtc = DateTime.Parse("01-Mar-21 8:39:00 PM")
                },
                new RoleClaimEntity
                {
                    Id = Guid.Parse("18a4fabe-2e70-4f51-a38a-30fa12332216"),
                    RoleId = Guid.Parse("d92ef5e5-f08a-4173-b83a-74618893891b"),
                    ClaimType = "Permission",
                    ClaimValue = "Bookstore.Read",
                    DateCreatedUtc = DateTime.Parse("01-Mar-21 8:39:00 PM"),
                    DateUpdatedUtc = DateTime.Parse("01-Mar-21 8:39:00 PM")
                },
                new RoleClaimEntity
                {
                    Id = Guid.Parse("f753b22d-bcaf-4545-b9b4-37d1bdedde67"),
                    RoleId = Guid.Parse("d92ef5e5-f08a-4173-b83a-74618893891b"),
                    ClaimType = "Permission",
                    ClaimValue = "Bookstore.Update",
                    DateCreatedUtc = DateTime.Parse("01-Mar-21 8:39:00 PM"),
                    DateUpdatedUtc = DateTime.Parse("01-Mar-21 8:39:00 PM")
                },
                new RoleClaimEntity
                {
                    Id = Guid.Parse("1633728a-d42a-4ee3-98af-74db1970224e"),
                    RoleId = Guid.Parse("d92ef5e5-f08a-4173-b83a-74618893891b"),
                    ClaimType = "Permission",
                    ClaimValue = "Bookstore.Create",
                    DateCreatedUtc = DateTime.Parse("01-Mar-21 8:39:00 PM"),
                    DateUpdatedUtc = DateTime.Parse("01-Mar-21 8:39:00 PM")
                },
                new RoleClaimEntity
                {
                    Id = Guid.Parse("edf7e607-426d-4cc5-b6ac-321f11518520"),
                    RoleId = Guid.Parse("d92ef5e5-f08a-4173-b83a-74618893891b"),
                    ClaimType = "Permission",
                    ClaimValue = "Bookstore.Delete",
                    DateCreatedUtc = DateTime.Parse("01-Mar-21 8:39:00 PM"),
                    DateUpdatedUtc = DateTime.Parse("01-Mar-21 8:39:00 PM")
                }
            );
        }
    }
}