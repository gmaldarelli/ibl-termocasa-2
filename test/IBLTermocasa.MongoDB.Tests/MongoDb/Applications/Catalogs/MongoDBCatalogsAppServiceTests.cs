using IBLTermocasa.Catalogs;
using Xunit;

namespace IBLTermocasa.MongoDB.Applications.Catalogs;

[Collection(IBLTermocasaTestConsts.CollectionDefinitionName)]
public class MongoDBCatalogsAppServiceTests : CatalogsAppServiceTests<IBLTermocasaMongoDbTestModule>
{
}