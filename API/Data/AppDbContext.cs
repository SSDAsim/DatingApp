using System;
using API.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace API.Data;

public class AppDbContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<AppUser> Users { get; set; }

    public DbSet<Member> Members { get; set; }

    public DbSet<Photo> Photos { get; set; }
    public DbSet<MemberLike> Likes { get; set; }

    public DbSet<Message> Messages { get; set; }

    // 'DbSet<T> <table_name>' represents a database table

    // change the date format 
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // --- Configure many-to-many relationship between members and messages 
        // one recipient has many messages, deleting a recipient should not delete the messages
        modelBuilder.Entity<Message>()
            .HasOne(r => r.Recipient)
            .WithMany(m => m.MessagesReceived)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Message>()
            .HasOne(x => x.Sender)
            .WithMany(m => m.MessagesSent)
            .OnDelete(DeleteBehavior.Restrict);

        // -- many-to-many relationship between members and likes
        // declare a primary key as combination of both SourceMemberId and TargetMemberId
        // since the many-to-many relationship between User models based on likes is something we have to configure ourselves so that's why we are doing it heree
        modelBuilder.Entity<MemberLike>().HasKey(x => new { x.SourceMemberId, x.TargetMemberId });

        // configure individual entities 
        modelBuilder.Entity<MemberLike>()
               .HasOne(s => s.SourceMember)
               .WithMany(t => t.LikedMembers)
               .HasForeignKey(s => s.SourceMemberId)
               .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<MemberLike>()
               .HasOne(s => s.TargetMember)
               .WithMany(t => t.LikedByMembers)
               .HasForeignKey(s => s.TargetMemberId)
               .OnDelete(DeleteBehavior.NoAction);

        var dateTimeConverter = new ValueConverter<DateTime, DateTime>(
            v => v.ToUniversalTime(),
            v => DateTime.SpecifyKind(v, DateTimeKind.Utc)
        );

        // value converter for optional datetime
        var optionalDateTimeConverter = new ValueConverter<DateTime?, DateTime?>(
            v => v.HasValue ? v.Value.ToUniversalTime() : null,
            v => v.HasValue ? DateTime.SpecifyKind(v.Value, DateTimeKind.Utc) : null
        );

        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            foreach (var property in entityType.GetProperties())
            {
                if(property.ClrType == typeof(DateTime))
                {
                    property.SetValueConverter(dateTimeConverter);
                } else if (property.ClrType == typeof(DateTime?))
                {
                    property.SetValueConverter(optionalDateTimeConverter);
                }
            }
        }
    }
}
