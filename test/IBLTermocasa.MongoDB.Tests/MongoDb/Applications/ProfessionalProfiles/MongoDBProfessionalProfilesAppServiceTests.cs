using IBLTermocasa.ProfessionalProfiles;
using Xunit;

namespace IBLTermocasa.MongoDB.Applications.ProfessionalProfiles;

[Collection(IBLTermocasaTestConsts.CollectionDefinitionName)]
public class MongoDBProfessionalProfilesAppServiceTests : ProfessionalProfilesAppServiceTests<IBLTermocasaMongoDbTestModule>
{
}