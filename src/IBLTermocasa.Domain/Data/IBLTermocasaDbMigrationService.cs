using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using IBLTermocasa.Domain;
using IBLTermocasa.Industries;
using IBLTermocasa.Materials;
using IBLTermocasa.Organizations;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using MongoDB.Bson.Serialization;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Identity;
using Volo.Abp.MultiTenancy;
using Volo.Saas.Tenants;

namespace IBLTermocasa.Data;

public class IBLTermocasaDbMigrationService : ITransientDependency
{
    public ILogger<IBLTermocasaDbMigrationService> Logger { get; set; }

    private readonly IDataSeeder _dataSeeder;
    private readonly IEnumerable<IIBLTermocasaDbSchemaMigrator> _dbSchemaMigrators;
    private readonly ITenantRepository _tenantRepository;
    private readonly ICurrentTenant _currentTenant;
    private readonly IOrganizationRepository _organizationRepository;
    private readonly IIndustryRepository _industryRepository;
    private readonly IIdentityUserRepository _identityUserRepository;
    private readonly IMaterialRepository _materialRepository;

    public IBLTermocasaDbMigrationService(
        IDataSeeder dataSeeder,
        ITenantRepository tenantRepository,
        ICurrentTenant currentTenant,
        IOrganizationRepository organizationRepository,
        IEnumerable<IIBLTermocasaDbSchemaMigrator> dbSchemaMigrators,
        IIdentityUserRepository identityUserRepository,
        IIndustryRepository industryRepository,
        IMaterialRepository materialRepository)
    {
        _dataSeeder = dataSeeder;
        _tenantRepository = tenantRepository;
        _currentTenant = currentTenant;
        _dbSchemaMigrators = dbSchemaMigrators;
        _organizationRepository = organizationRepository;
        _identityUserRepository = identityUserRepository;
        _industryRepository = industryRepository;
        _materialRepository = materialRepository;

        Logger = NullLogger<IBLTermocasaDbMigrationService>.Instance;
    }

    public async Task MigrateAsync()
    {

        Logger.LogInformation("Started database migrations...");

        await MigrateDatabaseSchemaAsync();
        await SeedDataAsync();

        Logger.LogInformation($"Successfully completed host database migrations.");

        var tenants = await _tenantRepository.GetListAsync(includeDetails: true);

        var migratedDatabaseSchemas = new HashSet<string>();
        foreach (var tenant in tenants)
        {
            using (_currentTenant.Change(tenant.Id))
            {
                if (tenant.ConnectionStrings.Any())
                {
                    var tenantConnectionStrings = tenant.ConnectionStrings
                        .Select(x => x.Value)
                        .ToList();

                    if (!migratedDatabaseSchemas.IsSupersetOf(tenantConnectionStrings))
                    {
                        await MigrateDatabaseSchemaAsync(tenant);

                        migratedDatabaseSchemas.AddIfNotContains(tenantConnectionStrings);
                    }
                }

                await SeedDataAsync(tenant);
            }

            Logger.LogInformation($"Successfully completed {tenant.Name} tenant database migrations.");
        }

        Logger.LogInformation("Successfully completed all database migrations.");
        Logger.LogInformation("You can safely end this process...");
    }

    private async Task MigrateDatabaseSchemaAsync(Tenant? tenant = null)
    {
        Logger.LogInformation(
            $"Migrating schema for {(tenant == null ? "host" : tenant.Name + " tenant")} database...");

        foreach (var migrator in _dbSchemaMigrators)
        {
            await migrator.MigrateAsync();
        }
    }

    private async Task SeedDataAsync(Tenant? tenant = null)
    {
        Logger.LogInformation($"Executing {(tenant == null ? "host" : tenant.Name + " tenant")} database seed...");

        await _dataSeeder.SeedAsync(new DataSeedContext(tenant?.Id)
            .WithProperty(IdentityDataSeedContributor.AdminEmailPropertyName,
                IBLTermocasaConsts.AdminEmailDefaultValue)
            .WithProperty(IdentityDataSeedContributor.AdminPasswordPropertyName,
                IBLTermocasaConsts.AdminPasswordDefaultValue)
        );
        
        //Codice da commentare se è stato eseguito la migrazione già una volta
        
        //await SeedCustomerDataAsync(tenant: tenant);
        //await SeedMaterialDataAsync(tenant: tenant);
    }

    private async Task SeedCustomerDataAsync(Tenant? tenant = null)
    {
        Logger.LogInformation($"Executing {(tenant == null ? "host" : tenant.Name + " tenant")} database CustomerData seed...");
        string filePath = Path.Combine(Directory.GetCurrentDirectory(), "..\\..\\..\\..\\..\\etc\\Import\\", "customers.xlsx");
        BsonSerializer.RegisterSerializationProvider( new CustomGuidSerializationProvider());
        var importer = new DataImporter(_organizationRepository, _identityUserRepository, _industryRepository, _materialRepository);
        importer.ImportCustomerDataFromExcel(filePath);
    }
    private async Task SeedMaterialDataAsync(Tenant? tenant = null)
    {
        Logger.LogInformation($"Executing {(tenant == null ? "host" : tenant.Name + " tenant")} database CustomerData seed...");
        string filePath = Path.Combine(Directory.GetCurrentDirectory(), "..\\..\\..\\..\\..\\etc\\Import\\", "materials.xlsx");
        BsonSerializer.RegisterSerializationProvider( new CustomGuidSerializationProvider());
        var importer = new DataImporter(_organizationRepository, _identityUserRepository, _industryRepository, _materialRepository);
        await importer.ImportMaterialDataFromExcel(filePath);
    }
}
