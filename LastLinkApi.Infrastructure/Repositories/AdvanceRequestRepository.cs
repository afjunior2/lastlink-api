using Microsoft.EntityFrameworkCore;
using LastLinkApi.Domain.Entities;
using LastLinkApi.Domain.Repositories;
using LastLinkApi.Domain.ValueObjects;
using LastLinkApi.Infrastructure.Data;

namespace LastLinkApi.Infrastructure.Repositories;

public class AdvanceRequestRepository : IAdvanceRequestRepository
{
    private readonly ApplicationDbContext _context;

    public AdvanceRequestRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<AdvanceRequest> SaveAsync(AdvanceRequest request)
    {
        if (request.Id == 0)
        {
            _context.AdvanceRequests.Add(request);
        }
        else
        {
            _context.AdvanceRequests.Update(request);
        }

        await _context.SaveChangesAsync();
        return request;
    }

    public async Task<AdvanceRequest?> GetByIdAsync(int id)
    {
        return await _context.AdvanceRequests
            .FirstOrDefaultAsync(r => r.Id == id);
    }

    public async Task<IEnumerable<AdvanceRequest>> GetByCreatorAsync(string creatorId)
    {
        return await _context.AdvanceRequests
            .Where(r => r.CreatorId == creatorId)
            .OrderByDescending(r => r.RequestDate)
            .ToListAsync();
    }

    public async Task<bool> HasPendingRequestAsync(string creatorId)
    {
        return await _context.AdvanceRequests
            .AnyAsync(r => r.CreatorId == creatorId && r.Status == RequestStatus.Pending);
    }

    public async Task<IEnumerable<AdvanceRequest>> GetAllAsync()
    {
        return await _context.AdvanceRequests
            .OrderByDescending(r => r.RequestDate)
            .ToListAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var request = await GetByIdAsync(id);
        if (request != null)
        {
            _context.AdvanceRequests.Remove(request);
            await _context.SaveChangesAsync();
        }
    }
    
    public async Task UpdateAsync(AdvanceRequest request)
    {
        _context.AdvanceRequests.Update(request);
        await _context.SaveChangesAsync();
    }
}