using LastLinkApi.Application.Commands;
using LastLinkApi.Domain.Entities;
using LastLinkApi.Domain.Repositories;
using MediatR;

namespace LastLinkApi.Application.Handlers
{
    /// <summary>
    /// Handles the rejection of advance requests.
    /// </summary>
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
                throw new KeyNotFoundException($"Solicitação de antecipação com Id {request.RequestId} não foi encontrada.");

            advanceRequest.Reject();

            await _repository.UpdateAsync(advanceRequest);

            return advanceRequest;
        }
    }
}