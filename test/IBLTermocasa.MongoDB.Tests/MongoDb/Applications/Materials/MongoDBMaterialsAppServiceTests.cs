using IBLTermocasa.Materials;
using Xunit;

namespace IBLTermocasa.MongoDB.Applications.Materials;

[Collection(IBLTermocasaTestConsts.CollectionDefinitionName)]
public class MongoDBMaterialsAppServiceTests : MaterialsAppServiceTests<IBLTermocasaMongoDbTestModule>
{
}