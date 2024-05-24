using IBLTermocasa.Contacts;
using Xunit;

namespace IBLTermocasa.MongoDB.Applications.Contacts;

[Collection(IBLTermocasaTestConsts.CollectionDefinitionName)]
public class MongoDBContactsAppServiceTests : ContactsAppServiceTests<IBLTermocasaMongoDbTestModule>
{
}