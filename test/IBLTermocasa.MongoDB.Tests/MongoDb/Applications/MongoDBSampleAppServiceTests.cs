using IBLTermocasa.MongoDB;
using IBLTermocasa.Samples;
using Xunit;

namespace IBLTermocasa.MongoDb.Applications;

[Collection(IBLTermocasaTestConsts.CollectionDefinitionName)]
public class MongoDBSampleAppServiceTests : SampleAppServiceTests<IBLTermocasaMongoDbTestModule>
{

}
