using MediatR;
using LastLinkApi.Application.Queries;
using LastLinkApi.Domain.Entities;
using LastLinkApi.Domain.Repositories;

namespace LastLinkApi.Application.Handlers;

public class GetAdvanceRequestsByCreatorHandler : IRequestHandler<GetAdvanceRequestsByCreatorQuery, IEnumerable<AdvanceRequest>>
{
    private readonly IAdvanceRequestRepository _repository;

    public GetAdvanceRequestsByCreatorHandler(IAdvanceRequestRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<AdvanceRequest>> Handle(GetAdvanceRequestsByCreatorQuery request, CancellationToken cancellationToken)
    {
        return await _repository.GetByCreatorAsync(request.CreatorId);
    }
}