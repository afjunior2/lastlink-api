using LastLinkApi.Domain.Entities;

namespace LastLinkApi.Domain.Repositories;

/// <summary>
/// Repository interface for advance requests
/// 
/// Defines the contract for advance request persistence following Repository pattern.
/// </summary>
public interface IAdvanceRequestRepository
{
    Task<AdvanceRequest> SaveAsync(AdvanceRequest request);
    Task<AdvanceRequest?> GetByIdAsync(int id);
    Task<IEnumerable<AdvanceRequest>> GetByCreatorAsync(string creatorId);
    Task<bool> HasPendingRequestAsync(string creatorId);
    Task<IEnumerable<AdvanceRequest>> GetAllAsync();
    Task DeleteAsync(int id);
    Task UpdateAsync(AdvanceRequest request);
}