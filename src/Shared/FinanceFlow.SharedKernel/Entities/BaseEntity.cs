using FinanceFlow.SharedKernel.Extensions;

namespace FinanceFlow.SharedKernel.Entities;

/// <summary>
/// Classe base para entidades.
/// </summary>
public abstract class BaseEntity
{
    public Guid Id { get; set; }

    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Prepara a entidade para inserção.
    /// </summary>
    public virtual void PrepareInsert()
    {
        Id = Guid.NewGuid();
        CreatedAt = DateTime.UtcNow.ToBrasiliaTime();
    }
}