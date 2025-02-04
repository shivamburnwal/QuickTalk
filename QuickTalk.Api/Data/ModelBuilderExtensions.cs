﻿using Microsoft.EntityFrameworkCore;
using QuickTalk.Api.Models;

namespace QuickTalk.Api.Data
{
    public static class ModelBuilderExtensions
    {
        public static void ConfigureRelationships (this ModelBuilder modelBuilder)
        {
            // Usernames must be unique
            modelBuilder.Entity<User>()
            .HasIndex(u => u.Username)
            .IsUnique();

            // One-to-many: User -> Message
            modelBuilder.Entity<Message>()
                .HasOne(m => m.User)
                .WithMany(u => u.Messages)
                .HasForeignKey(m => m.UserID)
                .OnDelete(DeleteBehavior.SetNull);

            #region Conversation RelationShips
            /*
            // One-to-many: User -> Conversation (User1)
            modelBuilder.Entity<Conversation>()
                .HasOne(c => c.User1)
                .WithMany(u => u.UserConversations)
                .HasForeignKey(c => c.User1ID)
                .OnDelete(DeleteBehavior.Restrict);

            // One-to-many: User -> Conversation (User2)
            modelBuilder.Entity<Conversation>()
                .HasOne(c => c.User2)
                .WithMany(u => u.UserConversations)
                .HasForeignKey(c => c.User2ID)
                .OnDelete(DeleteBehavior.Restrict);

            // One-to-many: Conversation -> Message
            modelBuilder.Entity<Message>()
                .HasOne(m => m.Conversation)
                .WithMany(c => c.Messages)
                .HasForeignKey(m => m.ConversationID)
                .OnDelete(DeleteBehavior.Cascade);
            */
            #endregion

            // Store room type as string instead of int
            modelBuilder.Entity<Chatroom>()
            .Property(c => c.RoomType)
            .HasConversion<string>();

            // One-to-many: Chatroom -> Message
            modelBuilder.Entity<Message>()
                .HasOne(m => m.Chatroom)
                .WithMany(c => c.Messages)
                .HasForeignKey(m => m.ChatroomID)
                .OnDelete(DeleteBehavior.Cascade);

            // Many-to-many: User -> Chatroom (via UserChatroom)
            modelBuilder.Entity<UserChatroom>()
                .HasKey(uc => new { uc.UserID, uc.ChatroomID });

            modelBuilder.Entity<UserChatroom>()
                .HasOne(uc => uc.User)
                .WithMany(u => u.UserChatrooms)
                .HasForeignKey(uc => uc.UserID);

            modelBuilder.Entity<UserChatroom>()
                .HasOne(uc => uc.Chatroom)
                .WithMany(c => c.UserChatrooms)
                .HasForeignKey(uc => uc.ChatroomID);

            // One-to-many: Message -> MessageReaction
            modelBuilder.Entity<MessageReaction>()
                .HasOne(mr => mr.Message)
                .WithMany(m => m.MessageReactions)
                .HasForeignKey(mr => mr.MessageID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<MessageReaction>()
                .HasOne(mr => mr.User)
                .WithMany(u => u.MessageReactions)
                .HasForeignKey(mr => mr.UserID)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
