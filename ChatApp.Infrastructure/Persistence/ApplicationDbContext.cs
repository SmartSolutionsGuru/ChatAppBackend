using ChatApp.Domain.Entities;
using ChatApp.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatApp.Infrastructure.Persistence
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<Chat> Chats => Set<Chat>();
        public DbSet<Message> Messages => Set<Message>();
        public DbSet<ChatRequest> ChatRequests => Set<ChatRequest>();
        public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Chat>(e =>
            {
                e.HasKey(x => x.Id);
                e.Property(x => x.User1Id).IsRequired();
                e.Property(x => x.User2Id).IsRequired();
                e.HasIndex(x => new { x.User1Id, x.User2Id }).IsUnique(false);
            });

            builder.Entity<Message>(e =>
            {
                e.HasKey(x => x.Id);
                e.Property(x => x.Content).IsRequired().HasMaxLength(4000);
                e.Property(x => x.SenderId).IsRequired();
                e.Property(x => x.ReceiverId).IsRequired();

                e.HasOne(x => x.Chat)
                 .WithMany(c => c.Messages)
                 .HasForeignKey(x => x.ChatId)
                 .OnDelete(DeleteBehavior.Cascade);

                e.HasIndex(x => new { x.ChatId, x.CreatedAt });
            });

            builder.Entity<ChatRequest>(e =>
            {
                e.HasKey(x => x.Id);
                e.Property(x => x.FromUserId).IsRequired();
                e.Property(x => x.ToUserId).IsRequired();
                e.HasIndex(x => new { x.FromUserId, x.ToUserId });
            });

            builder.Entity<RefreshToken>(e =>
            {
                e.HasKey(x => x.Id);
                e.Property(x => x.UserId).IsRequired();
                e.Property(x => x.Token).IsRequired().HasMaxLength(200);
                e.HasIndex(x => x.Token).IsUnique();
            });


        }
    }
}
