using MediatR;
using LastLinkApi.Domain.Entities;

namespace LastLinkApi.Application.Commands;
public record RejectAdvanceRequestCommand(int RequestId) : IRequest<AdvanceRequest>;