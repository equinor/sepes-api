using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Sepes.Infrastructure.Model;
using Sepes.Infrastructure.Model.Context;
using Sepes.Infrastructure.Service.DataModelService.Interface;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service.DataModelService
{
    public class WbsCodeCacheModelService : IWbsCodeCacheModelService
    {
        readonly ILogger _logger;
        readonly SepesDbContext _sepesDbContext;

        public WbsCodeCacheModelService(ILogger<WbsCodeCacheModelService> logger, SepesDbContext sepesDbContext)
        {
            _logger = logger;
            _sepesDbContext = sepesDbContext;
        }

        //public async Task<bool> ExistsAndValid(string wbsCode, CancellationToken cancellation = default)
        //{
        //    try
        //    {
        //        var wbsFromDbQueryable = GetItemQueryable(wbsCode).Where(w => w.Valid && w.Expires > DateTime.UtcNow);

        //        return await wbsFromDbQueryable.AnyAsync(cancellation);

        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, $"WbsCode cache lookup failed for code {wbsCode}");
        //    }

        //    return false;
        //}

        public async Task<WbsCodeCache> Get(string wbsCode, CancellationToken cancellation = default)
        {
            try
            {
                var wbsFromDbQueryable = GetItemQueryable(wbsCode).Where(w => w.Expires > DateTime.UtcNow);

                return await wbsFromDbQueryable.SingleOrDefaultAsync(cancellation);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"WbsCode cache lookup failed for code {wbsCode}");
            }

            return null;
        }

        public async Task Add(string wbsCode, bool valid)
        {
            var queryable = GetItemQueryable(wbsCode);

            if (queryable.Any())
            {
                var existingItem = await queryable.SingleOrDefaultAsync();
                existingItem.Valid = valid;

                if (valid)
                {
                    existingItem.Expires = GetNewExpires(valid);
                }            
            }
            else
            {
                _sepesDbContext.WbsCodeCache.Add(new WbsCodeCache(wbsCode.ToLowerInvariant(), valid, GetNewExpires(valid)));
            }

            await _sepesDbContext.SaveChangesAsync();
        }

        public async Task Clean()
        {
            var itemsToRemove = await _sepesDbContext.WbsCodeCache.Where(w => w.Expires <= DateTime.UtcNow).ToListAsync();
            _sepesDbContext.WbsCodeCache.RemoveRange(itemsToRemove);
            await _sepesDbContext.SaveChangesAsync();
        }

        DateTime GetNewExpires(bool valid)
        {
            return DateTime.UtcNow.AddMinutes(valid ? 20 : 3);
        }

        IQueryable<WbsCodeCache> GetItemQueryable(string wbsCode)
        {
            return _sepesDbContext.WbsCodeCache.Where(w => w.WbsCode == wbsCode.ToLower());
        }      
    }
}