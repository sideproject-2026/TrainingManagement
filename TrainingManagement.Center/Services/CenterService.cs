using BuildingBlock.Util.Commons.Parameters;
using BuildingBlock.Util.Commons.Results;
using Microsoft.EntityFrameworkCore;
using TrainingManagement.Center.Contracts;
using TrainingManagement.Center.Models;
using TrainingManagement.Center.Persistence;

namespace TrainingManagement.Center.Services;

internal class CenterService(CenterDbContext context) : ICenterService
{
    public async Task<Result<Guid>> CreateAsync(TrainingCenter trainingCenter, CancellationToken ct = default)
    {
        
        context.TrainingCenters.Add(trainingCenter);
        await context.SaveChangesAsync(ct);
        return Result<Guid>.Success(trainingCenter.Id);


    }

    public async Task<Result> DeleteAsync(Guid trainingCenterId, CancellationToken ct = default)
    {
        context.Database.BeginTransaction();

        await context.TrainingCenters
                .Where(c => c.Id == trainingCenterId)
                .ExecuteDeleteAsync(ct);

        context.Database.CommitTransaction();

        return Result.Success();
    }

    public async Task<QueryResult<TrainingCenter>> GetAllAsync(FilterParam filterParam, CancellationToken ct = default)
    {
        var query = context.TrainingCenters.AsQueryable();

        if (filterParam.Filters != null)
        {
            foreach (var filter in filterParam.Filters)
            {
                // Apply filters to the query based on the filter key and value
                // This is a simplified example, you may need to handle different data types and operators
                query = query.Where(tc => EF.Property<string>(tc, filter.Key).Contains(filter.Value.ToString()));
            }
        }

        if (filterParam.IsPaginate)
        {
            var totalItems = await query.CountAsync(ct);
            var pageCount = (int)Math.Ceiling(totalItems / (double)filterParam.PageSize);
            var data = await query.Skip((filterParam.PageNumber - 1) * filterParam.PageSize)
                                  .Take(filterParam.PageSize)
                                  .ToListAsync(ct);

            return new QueryResult<TrainingCenter>(data, pageCount);
           

        }
        else
        {
            var data = await query.ToListAsync(ct);
            return new QueryResult<TrainingCenter>(data, data.Count);
            
        }
    }
}
