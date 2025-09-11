using MediatR;
using LastLinkApi.Application.Queries;
using LastLinkApi.Domain.Entities;
using LastLinkApi.Domain.ValueObjects;

namespace LastLinkApi.Application.Handlers;

public class SimulateAdvanceRequestHandler : IRequestHandler<SimulateAdvanceRequestQuery, RequestSimulation>
{
    public Task<RequestSimulation> Handle(SimulateAdvanceRequestQuery request, CancellationToken cancellationToken)
    {
        var simulation = AdvanceRequest.Simulate(request.RequestedAmount);
        return Task.FromResult(simulation);
    }
}