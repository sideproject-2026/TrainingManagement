using System;

namespace BuildingBlock.Domain.Core;

public interface IDomainEvent
{
    DateTime OccurredOn { get; }
}
