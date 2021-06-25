using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Sepes.Common.Exceptions;
using Sepes.Infrastructure.Extensions;
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

        public async Task<WbsCodeCache> Get(string wbsCode, CancellationToken cancellation = default)
        {
            _logger.LogInformation($"Wbs Cache - Get: {wbsCode}");

            try
            {
                var wbsFromDbQueryable = GetItemQueryable(wbsCode, true).Where(w => w.Expires > DateTime.UtcNow);
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
            _logger.LogInformation($"Wbs Cache - Add: {wbsCode}, valid: {valid}");

            try
            {
                //Executed as NoTracking, because it ensures we hit the database
                var queryable = GetItemQueryable(wbsCode, true);

                if (queryable.Any())
                {
                    _logger.LogInformation($"Wbs Cache - Add: {wbsCode}, valid: {valid}, item exists allready");
                    //Re-running the query, but now as a trackable, because we might be updating the entry.

                    queryable = GetItemQueryable(wbsCode);
                    var existingItem = await queryable.SingleOrDefaultAsync();
                    existingItem.Valid = valid;

                    if (valid)
                    {
                        existingItem.Expires = GetNewExpires(valid);
                    }
                }
                else
                {
                    _logger.LogInformation($"Wbs Cache - Add: {wbsCode}, valid: {valid}, item does not exist, adding!");
                    _sepesDbContext.WbsCodeCache.Add(new WbsCodeCache(wbsCode.ToLowerInvariant(), valid, GetNewExpires(valid)));
                }

                await _sepesDbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new CustomUserMessageException($"Unable to add wbs cache entry {wbsCode}, valid: {valid}", ex, $"Failed to update WBS cache");
            }           
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

        IQueryable<WbsCodeCache> GetItemQueryable(string wbsCode, bool asNoTracking = false)
        {
            return _sepesDbContext.WbsCodeCache
                 .If(asNoTracking, x => x.AsNoTracking())
                .Where(w => w.WbsCode == wbsCode.ToLowerInvariant());
        }      
    }
}