using IBLTermocasa.Industries;
using Xunit;

namespace IBLTermocasa.MongoDB.Applications.Industries;

[Collection(IBLTermocasaTestConsts.CollectionDefinitionName)]
public class MongoDBIndustriesAppServiceTests : IndustriesAppServiceTests<IBLTermocasaMongoDbTestModule>
{
}