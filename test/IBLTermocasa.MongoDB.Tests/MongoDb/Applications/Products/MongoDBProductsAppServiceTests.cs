using IBLTermocasa.Products;
using Xunit;

namespace IBLTermocasa.MongoDB.Applications.Products;

[Collection(IBLTermocasaTestConsts.CollectionDefinitionName)]
public class MongoDBProductsAppServiceTests : ProductsAppServiceTests<IBLTermocasaMongoDbTestModule>
{
}