using LastLinkApi.Application.Commands;
using LastLinkApi.Domain.Entities;
using LastLinkApi.Domain.Repositories;
using MediatR;

namespace LastLinkApi.Application.Handlers
{
    /// <summary>
    /// Handles the approval of advance requests.
    /// </summary>
    public class ApproveAdvanceRequestHandler(IAdvanceRequestRepository repository)
        : IRequestHandler<ApproveAdvanceRequestCommand, AdvanceRequest>
    {
        public async Task<AdvanceRequest> Handle(ApproveAdvanceRequestCommand request, CancellationToken cancellationToken)
        {
            var advanceRequest = await repository.GetByIdAsync(request.Id);

            if (advanceRequest == null)
                throw new KeyNotFoundException($"Solicitação de antecipação com Id {request.Id} não foi encontrada.");

            advanceRequest.Approve();

            await repository.UpdateAsync(advanceRequest);

            return advanceRequest;
        }
    }
}