﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using Repository;

#nullable disable

namespace TelegramNewsAggregator.Migrations
{
    [DbContext(typeof(ApplicationContext))]
    [Migration("20240817085355_AddUsersAndUserChannelsTables")]
    partial class AddUsersAndUserChannelsTables
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.4")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Entities.MessageTag", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid>("MessageId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("TagId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("TagId");

                    b.HasIndex("MessageId", "TagId")
                        .IsUnique();

                    b.ToTable("MessagesTags");
                });

            modelBuilder.Entity("Entities.Models.BufferedBlock", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.ToTable("BufferedBlocks");
                });

            modelBuilder.Entity("Entities.Models.BufferedMessage", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid>("BlockId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("MessageId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("BlockId");

                    b.HasIndex("MessageId")
                        .IsUnique();

                    b.ToTable("BufferedMessages");
                });

            modelBuilder.Entity("Entities.Models.Channel", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<bool>("IsPrivate")
                        .HasColumnType("boolean");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(32)
                        .HasColumnType("character varying(32)");

                    b.Property<long>("TelegramId")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.ToTable("Channels");
                });

            modelBuilder.Entity("Entities.Models.Message", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid>("ChannelId")
                        .HasColumnType("uuid");

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("SendedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("Status")
                        .HasColumnType("integer");

                    b.Property<long>("TelegramId")
                        .HasColumnType("bigint");

                    b.Property<string>("Uri")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("ChannelId");

                    b.ToTable("Messages");
                });

            modelBuilder.Entity("Entities.Models.Summary", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Summaries");
                });

            modelBuilder.Entity("Entities.Models.SummaryBlock", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid>("MessageId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("SummaryId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("MessageId")
                        .IsUnique();

                    b.HasIndex("SummaryId");

                    b.ToTable("SummaryBlocks");
                });

            modelBuilder.Entity("Entities.Models.User", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime>("SubscribtionsUpdatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<long>("TelegramId")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.HasIndex("TelegramId")
                        .IsUnique();

                    b.ToTable("Users");
                });

            modelBuilder.Entity("Entities.Models.UserChannel", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid>("ChannelId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("ChannelId");

                    b.HasIndex("UserId", "ChannelId")
                        .IsUnique();

                    b.ToTable("UserChannels");
                });

            modelBuilder.Entity("Entities.Tag", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.ToTable("Tags");
                });

            modelBuilder.Entity("Entities.MessageTag", b =>
                {
                    b.HasOne("Entities.Models.Message", "Message")
                        .WithMany()
                        .HasForeignKey("MessageId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Entities.Tag", "Tag")
                        .WithMany()
                        .HasForeignKey("TagId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Message");

                    b.Navigation("Tag");
                });

            modelBuilder.Entity("Entities.Models.BufferedMessage", b =>
                {
                    b.HasOne("Entities.Models.BufferedBlock", "Block")
                        .WithMany()
                        .HasForeignKey("BlockId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Entities.Models.Message", "Message")
                        .WithMany()
                        .HasForeignKey("MessageId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Block");

                    b.Navigation("Message");
                });

            modelBuilder.Entity("Entities.Models.Message", b =>
                {
                    b.HasOne("Entities.Models.Channel", "Channel")
                        .WithMany()
                        .HasForeignKey("ChannelId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Channel");
                });

            modelBuilder.Entity("Entities.Models.SummaryBlock", b =>
                {
                    b.HasOne("Entities.Models.Message", "Message")
                        .WithMany()
                        .HasForeignKey("MessageId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Entities.Models.Summary", "Summary")
                        .WithMany()
                        .HasForeignKey("SummaryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Message");

                    b.Navigation("Summary");
                });

            modelBuilder.Entity("Entities.Models.UserChannel", b =>
                {
                    b.HasOne("Entities.Models.Channel", "Channel")
                        .WithMany()
                        .HasForeignKey("ChannelId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Entities.Models.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Channel");

                    b.Navigation("User");
                });
#pragma warning restore 612, 618
        }
    }
}
