using IBLTermocasa.Samples;
using Xunit;

namespace IBLTermocasa.MongoDB.Domains;

[Collection(IBLTermocasaTestConsts.CollectionDefinitionName)]
public class MongoDBSampleDomainTests : SampleDomainTests<IBLTermocasaMongoDbTestModule>
{

}
