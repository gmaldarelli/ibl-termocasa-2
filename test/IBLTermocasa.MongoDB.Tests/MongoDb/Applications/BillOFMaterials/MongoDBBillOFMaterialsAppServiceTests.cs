using IBLTermocasa.BillOfMaterials;
using Xunit;

namespace IBLTermocasa.MongoDB.Applications.BillOfMaterials;

[Collection(IBLTermocasaTestConsts.CollectionDefinitionName)]
public class MongoDBBillOfMaterialsAppServiceTests : BillOfMaterialsAppServiceTests<IBLTermocasaMongoDbTestModule>
{
}