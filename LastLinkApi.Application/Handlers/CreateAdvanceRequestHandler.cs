using MediatR;
using LastLinkApi.Application.Commands;
using LastLinkApi.Domain.Entities;

namespace LastLinkApi.Application.Handlers;
public class CreateAdvanceRequestHandler : IRequestHandler<CreateAdvanceRequestCommand, AdvanceRequest>
{
    private static int _nextId = 1;
    private static readonly List<AdvanceRequest> _mockDatabase = new(); 
    public static List<AdvanceRequest> GetMockDatabase() => _mockDatabase;

    public Task<AdvanceRequest> Handle(CreateAdvanceRequestCommand request, CancellationToken cancellationToken)
    {
        var hasPendingRequest = _mockDatabase.Any(r => 
            r.CreatorId == request.CreatorId && 
            r.Status == Domain.ValueObjects.RequestStatus.Pending);
            
        if (hasPendingRequest)
        {
            throw new InvalidOperationException("Creator already has a pending request");
        }

        var advanceRequest = new AdvanceRequest(request.CreatorId, request.RequestedAmount, request.RequestDate);
        var idProperty = typeof(AdvanceRequest).GetProperty("Id");
        idProperty?.SetValue(advanceRequest, _nextId++);
        
        _mockDatabase.Add(advanceRequest);

        return Task.FromResult(advanceRequest);
    }
}