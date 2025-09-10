using MediatR;
using LastLinkApi.Application.Commands;
using LastLinkApi.Domain.Entities;
using LastLinkApi.Domain.Repositories;

namespace LastLinkApi.Application.Handlers;

public class CreateAdvanceRequestHandler : IRequestHandler<CreateAdvanceRequestCommand, AdvanceRequest>
{
    private readonly IAdvanceRequestRepository _repository;

    public CreateAdvanceRequestHandler(IAdvanceRequestRepository repository)
    {
        _repository = repository;
    }

    public async Task<AdvanceRequest> Handle(CreateAdvanceRequestCommand request, CancellationToken cancellationToken)
    {
        // Application rule validation
        var hasPendingRequest = await _repository.HasPendingRequestAsync(request.CreatorId);
        if (hasPendingRequest)
        {
            throw new InvalidOperationException("Creator already has a pending request");
        }

        // Entity creation (domain rules applied)
        var advanceRequest = new AdvanceRequest(request.CreatorId, request.RequestedAmount, request.RequestDate);

        // Persistence
        return await _repository.SaveAsync(advanceRequest);
    }
}