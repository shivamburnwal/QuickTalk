﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using QuickTalk.Api.Data;

#nullable disable

namespace QuickTalk.Api.Data.Migrations
{
    [DbContext(typeof(ChatDbContext))]
    [Migration("20250204085121_UpdateRelationForUserDeletion")]
    partial class UpdateRelationForUserDeletion
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.36")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("QuickTalk.Api.Models.Chatroom", b =>
                {
                    b.Property<int>("ChatroomID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ChatroomID"), 1L, 1);

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsPrivate")
                        .HasColumnType("bit");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("RoomType")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("ChatroomID");

                    b.ToTable("Chatrooms");
                });

            modelBuilder.Entity("QuickTalk.Api.Models.Message", b =>
                {
                    b.Property<int>("MessageID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("MessageID"), 1L, 1);

                    b.Property<int>("ChatroomID")
                        .HasColumnType("int");

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("SentAt")
                        .HasColumnType("datetime2");

                    b.Property<int?>("UserID")
                        .HasColumnType("int");

                    b.HasKey("MessageID");

                    b.HasIndex("ChatroomID");

                    b.HasIndex("UserID");

                    b.ToTable("Messages");
                });

            modelBuilder.Entity("QuickTalk.Api.Models.MessageReaction", b =>
                {
                    b.Property<int>("ReactionID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ReactionID"), 1L, 1);

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<int>("MessageID")
                        .HasColumnType("int");

                    b.Property<string>("ReactionType")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("UserID")
                        .HasColumnType("int");

                    b.HasKey("ReactionID");

                    b.HasIndex("MessageID");

                    b.HasIndex("UserID");

                    b.ToTable("MessageReactions");
                });

            modelBuilder.Entity("QuickTalk.Api.Models.User", b =>
                {
                    b.Property<int>("UserID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("UserID"), 1L, 1);

                    b.Property<string>("AvatarUrl")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("DisplayName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("LastLogin")
                        .HasColumnType("datetime2");

                    b.Property<string>("PasswordHash")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Role")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("UserID");

                    b.HasIndex("Username")
                        .IsUnique();

                    b.ToTable("Users");

                    b.HasData(
                        new
                        {
                            UserID = 1,
                            CreatedAt = new DateTime(2025, 2, 4, 8, 51, 20, 434, DateTimeKind.Utc).AddTicks(8082),
                            Email = "admin@quicktalk.com",
                            PasswordHash = "e86f78a8a3caf0b60d8e74e5942aa6d86dc150cd3c03338aef25b7d2d7e3acc7",
                            Role = "Admin",
                            Username = "admin"
                        });
                });

            modelBuilder.Entity("QuickTalk.Api.Models.UserChatroom", b =>
                {
                    b.Property<int>("UserID")
                        .HasColumnType("int");

                    b.Property<int>("ChatroomID")
                        .HasColumnType("int");

                    b.Property<DateTime>("JoinDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("Role")
                        .HasColumnType("int");

                    b.HasKey("UserID", "ChatroomID");

                    b.HasIndex("ChatroomID");

                    b.ToTable("UserChatrooms");
                });

            modelBuilder.Entity("QuickTalk.Api.Models.Message", b =>
                {
                    b.HasOne("QuickTalk.Api.Models.Chatroom", "Chatroom")
                        .WithMany("Messages")
                        .HasForeignKey("ChatroomID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("QuickTalk.Api.Models.User", "User")
                        .WithMany("Messages")
                        .HasForeignKey("UserID")
                        .OnDelete(DeleteBehavior.SetNull);

                    b.Navigation("Chatroom");

                    b.Navigation("User");
                });

            modelBuilder.Entity("QuickTalk.Api.Models.MessageReaction", b =>
                {
                    b.HasOne("QuickTalk.Api.Models.Message", "Message")
                        .WithMany("MessageReactions")
                        .HasForeignKey("MessageID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("QuickTalk.Api.Models.User", "User")
                        .WithMany("MessageReactions")
                        .HasForeignKey("UserID")
                        .OnDelete(DeleteBehavior.SetNull);

                    b.Navigation("Message");

                    b.Navigation("User");
                });

            modelBuilder.Entity("QuickTalk.Api.Models.UserChatroom", b =>
                {
                    b.HasOne("QuickTalk.Api.Models.Chatroom", "Chatroom")
                        .WithMany("UserChatrooms")
                        .HasForeignKey("ChatroomID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("QuickTalk.Api.Models.User", "User")
                        .WithMany("UserChatrooms")
                        .HasForeignKey("UserID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Chatroom");

                    b.Navigation("User");
                });

            modelBuilder.Entity("QuickTalk.Api.Models.Chatroom", b =>
                {
                    b.Navigation("Messages");

                    b.Navigation("UserChatrooms");
                });

            modelBuilder.Entity("QuickTalk.Api.Models.Message", b =>
                {
                    b.Navigation("MessageReactions");
                });

            modelBuilder.Entity("QuickTalk.Api.Models.User", b =>
                {
                    b.Navigation("MessageReactions");

                    b.Navigation("Messages");

                    b.Navigation("UserChatrooms");
                });
#pragma warning restore 612, 618
        }
    }
}
