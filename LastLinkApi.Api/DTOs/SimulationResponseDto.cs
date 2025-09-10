using LastLinkApi.Domain.ValueObjects;

namespace LastLinkApi.Api.DTOs;

public class SimulationResponseDto
{
    public decimal RequestedAmount { get; set; }
    
    public decimal FeeAmount { get; set; }
    
    public decimal NetAmount { get; set; }
    
    public decimal FeePercentage { get; set; }

    public static SimulationResponseDto FromValueObject(RequestSimulation simulation)
    {
        return new SimulationResponseDto
        {
            RequestedAmount = simulation.RequestedAmount,
            FeeAmount = simulation.FeeAmount,
            NetAmount = simulation.NetAmount,
            FeePercentage = simulation.FeePercentage
        };
    }
}