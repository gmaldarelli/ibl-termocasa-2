using IBLTermocasa.Interactions;
using Xunit;

namespace IBLTermocasa.MongoDB.Applications.Interactions;

[Collection(IBLTermocasaTestConsts.CollectionDefinitionName)]
public class MongoDBInteractionsAppServiceTests : InteractionsAppServiceTests<IBLTermocasaMongoDbTestModule>
{
}