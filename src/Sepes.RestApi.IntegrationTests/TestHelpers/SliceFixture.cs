using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Respawn;
using Sepes.Infrastructure.Extensions;
using Sepes.Infrastructure.Model;
using Sepes.Infrastructure.Model.Context;
using Sepes.RestApi.IntegrationTests.Setup;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Sepes.RestApi.IntegrationTests.TestHelpers
{
    //From https://github.com/jbogard/ContosoUniversityDotNetCore/blob/master/ContosoUniversity.IntegrationTests/SliceFixture.cs
    public class SliceFixture
    {
        private static readonly IConfigurationRoot _configuration;
        private static readonly IServiceScopeFactory _scopeFactory;

        static SliceFixture()
        {
            Trace.WriteLine("SliceFixture Constructor");

            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())               
                .AddUserSecrets<IntegrationTestsCollection>()
                .AddEnvironmentVariables();
            _configuration = builder.Build();

            Trace.WriteLine("SliceFixture Create Factory");
            var factory = new CustomWebApplicationFactory<Startup>();

            Trace.WriteLine("SliceFixture Get Scope Factory");
            _scopeFactory = factory.Services.GetService<IServiceScopeFactory>();
        }

        private readonly static Checkpoint _checkpoint = new Checkpoint
        {
            TablesToIgnore = new[]
            {
                "__EFMigrationsHistory"
            },
            WithReseed = true
        };

        public static Task ResetCheckpoint() => _checkpoint.Reset(IntegrationTestConnectionStringUtil.GetDatabaseConnectionString(_configuration));

        public static async Task ExecuteScopeAsync(Func<IServiceProvider, Task> action)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetService<SepesDbContext>();

                try
                {
                    //await dbContext.BeginTransactionAsync().ConfigureAwait(false);

                    await action(scope.ServiceProvider).ConfigureAwait(false);

                    //await dbContext.CommitTransactionAsync().ConfigureAwait(false);
                }
                catch (Exception)
                {
                    //dbContext.RollbackTransaction();
                    throw;
                }
            }
        }

        public async static Task<TEntity> InsertAsync<TEntity>(TEntity entity) where TEntity : class
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetService<SepesDbContext>();
               
                    dbContext.Set<TEntity>().Add(entity);

                    await dbContext.SaveChangesAsync();

                    return entity;             
            }
        }

        public static async Task<T> ExecuteScopeAsync<T>(Func<IServiceProvider, Task<T>> action)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetService<SepesDbContext>();

                try
                {
                    //await dbContext.BeginTransactionAsync().ConfigureAwait(false);

                    var result = await action(scope.ServiceProvider).ConfigureAwait(false);

                    //await dbContext.CommitTransactionAsync().ConfigureAwait(false);

                    return result;
                }
                catch (Exception)
                {
                    //dbContext.RollbackTransaction();
                    throw;
                }
            }
        }

        public static Task ExecuteDbContextAsync(Func<SepesDbContext, Task> action)
            => ExecuteScopeAsync(sp => action(sp.GetService<SepesDbContext>()));

        public static Task<T> ExecuteDbContextAsync<T>(Func<SepesDbContext, Task<T>> action)
            => ExecuteScopeAsync(sp => action(sp.GetService<SepesDbContext>()));

        public static Task<T> FindAsync<T>(int id)
          where T : class, IHasNummericalId
        {
            return ExecuteDbContextAsync<T>(db => db.Set<T>().FindAsync(id).AsTask());
        }

        public static Task<CloudResource> GetResource(int resourceId = 0, int studyId = 0, int sandboxId = 0)
        {
            return ExecuteDbContextAsync<CloudResource>
                (db =>
                db.CloudResources
                .Include(r => r.ChildResources)
                .Include(r => r.Operations)
                .If(resourceId > 0, x => x.Where(r => r.Id == resourceId))
                .If(studyId > 0, x => x.Where(r => r.StudyId == studyId))
                .If(sandboxId > 0, x => x.Where(r => r.SandboxId == sandboxId))
                .AsNoTracking()
                .FirstOrDefaultAsync());
        }

        public static Task InsertAsync<T>(params T[] entities) where T : class
        {
            return ExecuteDbContextAsync(db =>
            {
                foreach (var entity in entities)
                {
                    db.Set<T>().Add(entity);
                }

                return db.SaveChangesAsync();
            });
        }


        public static Task InsertAsync<TEntity, TEntity2>(TEntity entity, TEntity2 entity2)
            where TEntity : class
            where TEntity2 : class
        {
            return ExecuteDbContextAsync(db =>
            {
                db.Set<TEntity>().Add(entity);
                db.Set<TEntity2>().Add(entity2);

                return db.SaveChangesAsync();
            });
        }

        public static Task InsertAsync<TEntity, TEntity2, TEntity3>(TEntity entity, TEntity2 entity2, TEntity3 entity3)
            where TEntity : class
            where TEntity2 : class
            where TEntity3 : class
        {
            return ExecuteDbContextAsync(db =>
            {
                db.Set<TEntity>().Add(entity);
                db.Set<TEntity2>().Add(entity2);
                db.Set<TEntity3>().Add(entity3);

                return db.SaveChangesAsync();
            });
        }

        public static Task InsertAsync<TEntity, TEntity2, TEntity3, TEntity4>(TEntity entity, TEntity2 entity2, TEntity3 entity3, TEntity4 entity4)
            where TEntity : class
            where TEntity2 : class
            where TEntity3 : class
            where TEntity4 : class
        {
            return ExecuteDbContextAsync(db =>
            {
                db.Set<TEntity>().Add(entity);
                db.Set<TEntity2>().Add(entity2);
                db.Set<TEntity3>().Add(entity3);
                db.Set<TEntity4>().Add(entity4);

                return db.SaveChangesAsync();
            });
        }
    }
}
