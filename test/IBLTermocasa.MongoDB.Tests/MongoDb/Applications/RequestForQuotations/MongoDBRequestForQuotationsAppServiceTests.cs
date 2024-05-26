using IBLTermocasa.RequestForQuotations;
using Xunit;

namespace IBLTermocasa.MongoDB.Applications.RequestForQuotations;

[Collection(IBLTermocasaTestConsts.CollectionDefinitionName)]
public class MongoDBRequestForQuotationsAppServiceTests : RequestForQuotationsAppServiceTests<IBLTermocasaMongoDbTestModule>
{
}