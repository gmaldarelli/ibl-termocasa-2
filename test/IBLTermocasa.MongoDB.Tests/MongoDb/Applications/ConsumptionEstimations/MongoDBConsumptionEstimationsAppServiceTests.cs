using IBLTermocasa.ConsumptionEstimations;
using Xunit;

namespace IBLTermocasa.MongoDB.Applications.ConsumptionEstimations;

[Collection(IBLTermocasaTestConsts.CollectionDefinitionName)]
public class MongoDBConsumptionEstimationsAppServiceTests : ConsumptionEstimationsAppServiceTests<IBLTermocasaMongoDbTestModule>
{
}