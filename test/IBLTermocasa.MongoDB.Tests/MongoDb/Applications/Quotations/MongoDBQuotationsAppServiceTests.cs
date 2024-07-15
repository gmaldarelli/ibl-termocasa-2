using IBLTermocasa.Quotations;
using Xunit;

namespace IBLTermocasa.MongoDB.Applications.Quotations;

[Collection(IBLTermocasaTestConsts.CollectionDefinitionName)]
public class MongoDBQuotationsAppServiceTests : QuotationsAppServiceTests<IBLTermocasaMongoDbTestModule>
{
}