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
            var entityType = context.Model.FindEntityType(typeof(TrainingCenter));

            foreach (var filter in filterParam.Filters)
            {
                if (string.IsNullOrWhiteSpace(filter.Key))
                {
                    // Skip filters with no valid key
                    continue;
                }

                var property = entityType?.FindProperty(filter.Key);
                if (property == null || property.ClrType != typeof(string))
                {
                    // Skip unknown properties or non-string properties to avoid runtime exceptions
                    continue;
                }

                var filterValue = filter.Value?.ToString();
                if (string.IsNullOrEmpty(filterValue))
                {
                    // Skip filters with null or empty values to avoid Contains(null)
                    continue;
                }

                var filterKey = filter.Key;
                query = query.Where(tc => EF.Property<string>(tc, filterKey).Contains(filterValue));
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
            return new QueryResult<TrainingCenter>(data, 1);
            
        }
    }
}
