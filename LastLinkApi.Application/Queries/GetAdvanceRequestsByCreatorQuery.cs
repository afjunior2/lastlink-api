using MediatR;
using LastLinkApi.Domain.Entities;

namespace LastLinkApi.Application.Queries;

public record GetAdvanceRequestsByCreatorQuery(string CreatorId) : IRequest<IEnumerable<AdvanceRequest>>;