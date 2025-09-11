using MediatR;
using LastLinkApi.Domain.ValueObjects;

namespace LastLinkApi.Application.Queries;

public record SimulateAdvanceRequestQuery(decimal RequestedAmount) : IRequest<RequestSimulation>;