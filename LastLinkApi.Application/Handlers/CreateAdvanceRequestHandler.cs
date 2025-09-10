using MediatR;
using LastLinkApi.Application.Commands;
using LastLinkApi.Domain.Entities;
using LastLinkApi.Domain.Repositories;

namespace LastLinkApi.Application.Handlers;

public class CreateAdvanceRequestHandler(IAdvanceRequestRepository repository)
    : IRequestHandler<CreateAdvanceRequestCommand, AdvanceRequest>
{
    public async Task<AdvanceRequest> Handle(CreateAdvanceRequestCommand request, CancellationToken cancellationToken)
    {
        // Validação de regra de aplicação
        var hasPendingRequest = await repository.HasPendingRequestAsync(request.CreatorId);
        if (hasPendingRequest)
        {
            throw new InvalidOperationException("Já existe uma solicitação de antecipação pendente.");
        }

        // Criação da entidade (regras de domínio aplicadas)
        var advanceRequest = new AdvanceRequest(request.CreatorId, request.RequestedAmount, request.RequestDate);

        // Persistence
        return await repository.SaveAsync(advanceRequest);
    }
}