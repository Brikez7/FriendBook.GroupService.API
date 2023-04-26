﻿// <auto-generated />
using System;
using FriendBook.GroupService.API.DAL;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace FriendBook.GroupService.API.DAL.Migrations
{
    [DbContext(typeof(GroupAppDBContext))]
    [Migration("20230426224250_Add_AccountStatusGroup")]
    partial class Add_AccountStatusGroup
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.5")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("FriendBook.GroupService.API.Domain.Entities.AccountStatusGroup", b =>
                {
                    b.Property<Guid?>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("pk_account_status_groups_id");

                    b.Property<Guid>("IdAccount")
                        .HasColumnType("uuid")
                        .HasColumnName("account_id");

                    b.Property<Guid>("IdGroup")
                        .HasColumnType("uuid")
                        .HasColumnName("group_id");

                    b.Property<short>("RoleAccount")
                        .HasColumnType("smallint")
                        .HasColumnName("role_account");

                    b.HasKey("Id");

                    b.HasIndex("IdGroup");

                    b.ToTable(" account_status_groups", (string)null);
                });

            modelBuilder.Entity("FriendBook.GroupService.API.Domain.Entities.Group", b =>
                {
                    b.Property<Guid?>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("pk_group_id");

                    b.Property<Guid>("AccountId")
                        .HasColumnType("uuid")
                        .HasColumnName("account_id");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("create_date");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("character varying")
                        .HasColumnName("name");

                    b.HasKey("Id");

                    b.HasIndex("AccountId");

                    b.ToTable("groups", (string)null);
                });

            modelBuilder.Entity("FriendBook.GroupService.API.Domain.Entities.AccountStatusGroup", b =>
                {
                    b.HasOne("FriendBook.GroupService.API.Domain.Entities.Group", "Group")
                        .WithMany("AccountStatusGroups")
                        .HasForeignKey("IdGroup")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Group");
                });

            modelBuilder.Entity("FriendBook.GroupService.API.Domain.Entities.Group", b =>
                {
                    b.Navigation("AccountStatusGroups");
                });
#pragma warning restore 612, 618
        }
    }
}
