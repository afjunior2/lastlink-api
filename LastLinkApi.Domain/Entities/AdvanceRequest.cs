using LastLinkApi.Domain.ValueObjects;

namespace LastLinkApi.Domain.Entities;

public class AdvanceRequest
{
    public int Id { get; private set; }
    public string CreatorId { get; private set; } = string.Empty;
    public decimal RequestedAmount { get; private set; }
    public DateTime RequestDate { get; private set; }
    public RequestStatus Status { get; private set; }
    public decimal FeeAmount => RequestedAmount * 0.05m; // taxa fixa de 5%
    public decimal NetAmount => RequestedAmount - FeeAmount;
    public void SetId(int id)
    {
        if (Id != 0) 
            throw new InvalidOperationException("O Id já foi definido e não pode ser alterado.");
    
        Id = id;
    }
    private AdvanceRequest() { }
    
    public AdvanceRequest(string creatorId, decimal requestedAmount, DateTime? requestDate = null)
    {
        ValidateCreatorId(creatorId);
        ValidateRequestedAmount(requestedAmount);

        CreatorId = creatorId;
        RequestedAmount = requestedAmount;
        RequestDate = requestDate ?? DateTime.UtcNow;
        Status = RequestStatus.Pending;
    }

    // Método de atualização controlada
    public void Update(string creatorId, decimal requestedAmount, DateTime requestDate, RequestStatus status)
    {
        ValidateCreatorId(creatorId);
        ValidateRequestedAmount(requestedAmount);

        CreatorId = creatorId;
        RequestedAmount = requestedAmount;
        RequestDate = requestDate;
        Status = status;
    }

    // Regras de domínio
    public void Approve()
    {
        if (Status != RequestStatus.Pending)
            throw new InvalidOperationException($"Não é possível aprovar uma solicitação que já está como '{Status}'.");

        Status = RequestStatus.Approved;
    }

    public void Reject()
    {
        if (Status != RequestStatus.Pending)
            throw new InvalidOperationException($"Não é possível rejeitar uma solicitação que já está como '{Status}'.");

        Status = RequestStatus.Rejected;
    }

    // Método de simulação
    public static RequestSimulation Simulate(decimal requestedAmount)
    {
        ValidateRequestedAmount(requestedAmount);

        var feeAmount = requestedAmount * 0.05m;
        var netAmount = requestedAmount - feeAmount;

        return new RequestSimulation(requestedAmount, feeAmount, netAmount, 5.0m);
    }

    // Validações
    private static void ValidateCreatorId(string creatorId)
    {
        if (string.IsNullOrWhiteSpace(creatorId))
            throw new ArgumentException("O identificador do solicitante é obrigatório.", nameof(creatorId));
    }

    private static void ValidateRequestedAmount(decimal requestedAmount)
    {
        if (requestedAmount <= 100)
            throw new ArgumentException("O valor solicitado deve ser maior que R$100,00.", nameof(requestedAmount));
    }
}
