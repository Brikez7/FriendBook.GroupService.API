﻿using FriendBook.CommentServer.API.DAL.Configuration.DataType;
using FriendBook.CommentServer.API.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FriendBook.CommentServer.API.DAL.Configuration
{
    public class GroupConfig : IEntityTypeConfiguration<Group>
    {
        public const string Table_name = "comments";

        public void Configure(EntityTypeBuilder<Group> builder)
        {
            builder.ToTable(Table_name);

            builder.HasKey(e => new { e.Id });

            builder.HasIndex(e => e.AccountId);

            builder.Property(e => e.Id)
                   .HasColumnType(EntityDataTypes.Guid)
                   .HasColumnName("pk_group_id");

            builder.Property(e => e.AccountId)
                   .HasColumnType(EntityDataTypes.Character_varying)
                   .HasColumnName("account_id");

            builder.Property(e => e.Name)
                   .HasColumnType(EntityDataTypes.Character_varying)
                   .HasColumnName("name");

            builder.Property(e => e.CreatedDate)
                   .HasColumnName("create_date");
        }
    }
}