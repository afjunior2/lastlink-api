using MediatR;
using LastLinkApi.Domain.Entities;

namespace LastLinkApi.Application.Commands;
public record ApproveAdvanceRequestCommand(int RequestId) : IRequest<AdvanceRequest>;