using IBLTermocasa.Components;
using Xunit;

namespace IBLTermocasa.MongoDB.Applications.Components;

[Collection(IBLTermocasaTestConsts.CollectionDefinitionName)]
public class MongoDBComponentsAppServiceTests : ComponentsAppServiceTests<IBLTermocasaMongoDbTestModule>
{
}