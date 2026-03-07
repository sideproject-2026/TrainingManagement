using BuildingBlock.Util.Commons.Parameters;
using BuildingBlock.Util.Commons.Results;
using TrainingManagement.Center.Models;

namespace TrainingManagement.Center.Contracts;

public interface ICenterService
{
    Task<Result<Guid>> CreateAsync(TrainingCenter trainingCenter, CancellationToken ct = default);

    Task<Result> DeleteAsync(Guid trainingCenterId, CancellationToken ct = default);

    Task<QueryResult<TrainingCenter>> GetAllAsync(FilterParam filterParam, CancellationToken ct = default);
}
