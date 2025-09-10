using MediatR;
using LastLinkApi.Application.Commands;
using LastLinkApi.Domain.Entities;
using LastLinkApi.Domain.Repositories;

namespace LastLinkApi.Application.Handlers;

public class ApproveAdvanceRequestHandler : IRequestHandler<ApproveAdvanceRequestCommand, AdvanceRequest>
{
    private readonly IAdvanceRequestRepository _repository;

    public ApproveAdvanceRequestHandler(IAdvanceRequestRepository repository)
    {
        _repository = repository;
    }

    public async Task<AdvanceRequest> Handle(ApproveAdvanceRequestCommand request, CancellationToken cancellationToken)
    {
        var advanceRequest = await _repository.GetByIdAsync(request.RequestId);
        
        if (advanceRequest == null)
        {
            throw new ArgumentException("Advance request not found");
        }

        advanceRequest.Approve();
        return await _repository.SaveAsync(advanceRequest);
    }
}