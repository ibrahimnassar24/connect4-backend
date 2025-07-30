using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using connect4_backend.Data.Models;

namespace connect4_backend.Data;

public partial class Connect4Context : DbContext
{
    public Connect4Context()
    {
    }

    public Connect4Context(DbContextOptions<Connect4Context> options)
        : base(options)
    {
    }

    public virtual DbSet<Friend> Friends { get; set; }

    public virtual DbSet<Match> Matches { get; set; }

    public virtual DbSet<Notification> Notifications { get; set; }

    public virtual DbSet<Profile> Profiles { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {

    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Friend>(entity =>
        {
            entity.HasKey(e => new { e.Sender, e.Receiver }).HasName("PK__friends__E805F0FA29B8F55C");

            entity.ToTable("friends");

            entity.Property(e => e.Sender)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("sender");
            entity.Property(e => e.Receiver)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("receiver");
            entity.Property(e => e.ClosedAt).HasColumnName("closedAt");
            entity.Property(e => e.CreatedAt).HasColumnName("createdAt");
            entity.Property(e => e.Status)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("status");

            entity.HasOne(d => d.ReceiverNavigation).WithMany(p => p.FriendReceiverNavigations)
                .HasForeignKey(d => d.Receiver)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__friends__receive__3B75D760");

            entity.HasOne(d => d.SenderNavigation).WithMany(p => p.FriendSenderNavigations)
                .HasForeignKey(d => d.Sender)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__friends__sender__3A81B327");
        });

        modelBuilder.Entity<Match>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__matches__3213E83FBB0E952B");

            entity.ToTable("matches");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt).HasColumnName("createdAt");
            entity.Property(e => e.FirstPlayer)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("firstPlayer");
            entity.Property(e => e.SecondPlayer)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("secondPlayer");
            entity.Property(e => e.Status)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("status");
            entity.Property(e => e.UpdatedAt).HasColumnName("updatedAt");
            entity.Property(e => e.Winner)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("winner");

            entity.HasOne(d => d.FirstPlayerNavigation).WithMany(p => p.MatchFirstPlayerNavigations)
                .HasForeignKey(d => d.FirstPlayer)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_firstPlayer");

            entity.HasOne(d => d.SecondPlayerNavigation).WithMany(p => p.MatchSecondPlayerNavigations)
                .HasForeignKey(d => d.SecondPlayer)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_secondPlayer");

            entity.HasOne(d => d.WinnerNavigation).WithMany(p => p.MatchWinnerNavigations)
                .HasForeignKey(d => d.Winner)
                .HasConstraintName("fk_winner");
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__notifica__3213E83F725FFC79");

            entity.ToTable("notifications");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt).HasColumnName("createdAt");
            entity.Property(e => e.Link)
                .IsUnicode(false)
                .HasColumnName("link");
            entity.Property(e => e.Message)
                .IsUnicode(false)
                .HasColumnName("message");
            entity.Property(e => e.Receiver)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("receiver");
            entity.Property(e => e.Type)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("type");

            entity.HasOne(d => d.ReceiverNavigation).WithMany(p => p.Notifications)
                .HasForeignKey(d => d.Receiver)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_notifications_receiver");
        });

        modelBuilder.Entity<Profile>(entity =>
        {
            entity.HasKey(e => e.Email).HasName("PK__profiles__AB6E6165731325AE");

            entity.ToTable("profiles");

            entity.Property(e => e.Email)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("email");
            entity.Property(e => e.AvatarUrl)
                .IsUnicode(false)
                .HasColumnName("avatarUrl");
            entity.Property(e => e.CoverUrl)
            .IsUnicode(false)
            .HasColumnName("coverUrl");
            entity.Property(e => e.Bio).HasColumnName("bio");
            entity.Property(e => e.CreatedAt).HasColumnName("createdAt");
            entity.Property(e => e.FirstName)
                .HasMaxLength(25)
                .HasColumnName("firstName");
            entity.Property(e => e.LastName)
                .HasMaxLength(25)
                .HasColumnName("lastName");
            entity.Property(e => e.UpdatedAt).HasColumnName("updatedAt");
        });

    }

    
}
