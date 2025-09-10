using MediatR;
using LastLinkApi.Application.Commands;
using LastLinkApi.Domain.Entities;
using LastLinkApi.Domain.Repositories;

namespace LastLinkApi.Application.Handlers;

public class RejectAdvanceRequestHandler : IRequestHandler<RejectAdvanceRequestCommand, AdvanceRequest>
{
    private readonly IAdvanceRequestRepository _repository;

    public RejectAdvanceRequestHandler(IAdvanceRequestRepository repository)
    {
        _repository = repository;
    }

    public async Task<AdvanceRequest> Handle(RejectAdvanceRequestCommand request, CancellationToken cancellationToken)
    {
        var advanceRequest = await _repository.GetByIdAsync(request.RequestId);
        
        if (advanceRequest == null)
        {
            throw new ArgumentException("Solicitação não encontrada");
        }

        advanceRequest.Reject();
        return await _repository.SaveAsync(advanceRequest);
    }
}