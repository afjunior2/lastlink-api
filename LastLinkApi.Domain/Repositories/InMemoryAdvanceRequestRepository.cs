using LastLinkApi.Domain.Entities;
using LastLinkApi.Domain.ValueObjects;

namespace LastLinkApi.Domain.Repositories;

public class InMemoryAdvanceRequestRepository : IAdvanceRequestRepository
{
    private readonly List<AdvanceRequest> _requests = new();

    public Task<AdvanceRequest> SaveAsync(AdvanceRequest request)
    {
        var existing = _requests.FirstOrDefault(r => r.Id == request.Id);
        if (existing == null)
        {
            request.Id = _requests.Count + 1; // auto-incremento simples
            _requests.Add(request);
        }
        else
        {
            var index = _requests.IndexOf(existing);
            _requests[index] = request;
        }

        // Agora retornamos a entidade salva
        return Task.FromResult(request);
    }

    public Task<AdvanceRequest?> GetByIdAsync(int id)
    {
        var result = _requests.FirstOrDefault(r => r.Id == id);
        return Task.FromResult(result);
    }

    public Task<IEnumerable<AdvanceRequest>> GetByCreatorAsync(string creatorId)
    {
        var result = _requests.Where(r => r.CreatorId == creatorId);
        return Task.FromResult(result.AsEnumerable());
    }

    public Task<bool> HasPendingRequestAsync(string creatorId)
    {
        var hasPending = _requests.Any(r => r.CreatorId == creatorId && r.Status == RequestStatus.Pending);
        return Task.FromResult(hasPending);
    }

    public Task<IEnumerable<AdvanceRequest>> GetAllAsync()
    {
        return Task.FromResult(_requests.AsEnumerable());
    }

    public Task DeleteAsync(int id)
    {
        var request = _requests.FirstOrDefault(r => r.Id == id);
        if (request != null)
        {
            _requests.Remove(request);
        }

        return Task.CompletedTask;
    }
}