﻿using IBLTermocasa.BillOFMaterials;
using Xunit;

namespace IBLTermocasa.MongoDB.Applications.BillOFMaterials;

[Collection(IBLTermocasaTestConsts.CollectionDefinitionName)]
public class MongoDBBillOFMaterialsAppServiceTests : BillOFMaterialsAppServiceTests<IBLTermocasaMongoDbTestModule>
{
}