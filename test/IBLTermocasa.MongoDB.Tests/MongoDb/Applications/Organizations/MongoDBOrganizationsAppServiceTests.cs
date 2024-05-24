using IBLTermocasa.Organizations;
using Xunit;

namespace IBLTermocasa.MongoDB.Applications.Organizations;

[Collection(IBLTermocasaTestConsts.CollectionDefinitionName)]
public class MongoDBOrganizationsAppServiceTests : OrganizationsAppServiceTests<IBLTermocasaMongoDbTestModule>
{
}