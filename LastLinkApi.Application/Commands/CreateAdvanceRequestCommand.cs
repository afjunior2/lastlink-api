using MediatR;
using LastLinkApi.Domain.Entities;

namespace LastLinkApi.Application.Commands;

public record CreateAdvanceRequestCommand(
    string CreatorId,
    decimal RequestedAmount,
    DateTime? RequestDate = null
) : IRequest<AdvanceRequest>;