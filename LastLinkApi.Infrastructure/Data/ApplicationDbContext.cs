using Microsoft.EntityFrameworkCore;
using LastLinkApi.Domain.Entities;
using LastLinkApi.Domain.ValueObjects;

namespace LastLinkApi.Infrastructure.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<AdvanceRequest> AdvanceRequests { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<AdvanceRequest>(entity =>
        {
            entity.HasKey(e => e.Id);
            
            entity.Property(e => e.CreatorId)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(e => e.RequestedAmount)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            entity.Property(e => e.RequestDate)
                .IsRequired();

            entity.Property(e => e.Status)
                .HasConversion(
                    v => v.ToString(),
                    v => Enum.Parse<RequestStatus>(v))
                .IsRequired();

            entity.HasIndex(e => e.CreatorId);
            entity.HasIndex(e => e.Status);
            entity.HasIndex(e => new { e.CreatorId, e.Status });
        });
    }
}