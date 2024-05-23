using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace IBLTermocasa.Data;

/* This is used if database provider does't define
 * IIBLTermocasaDbSchemaMigrator implementation.
 */
public class NullIBLTermocasaDbSchemaMigrator : IIBLTermocasaDbSchemaMigrator, ITransientDependency
{
    public Task MigrateAsync()
    {
        return Task.CompletedTask;
    }
}
