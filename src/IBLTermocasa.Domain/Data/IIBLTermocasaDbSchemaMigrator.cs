using System.Threading.Tasks;

namespace IBLTermocasa.Data;

public interface IIBLTermocasaDbSchemaMigrator
{
    Task MigrateAsync();
}
