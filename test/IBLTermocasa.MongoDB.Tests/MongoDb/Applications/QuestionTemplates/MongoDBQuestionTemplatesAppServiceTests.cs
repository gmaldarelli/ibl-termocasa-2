using IBLTermocasa.QuestionTemplates;
using Xunit;

namespace IBLTermocasa.MongoDB.Applications.QuestionTemplates;

[Collection(IBLTermocasaTestConsts.CollectionDefinitionName)]
public class MongoDBQuestionTemplatesAppServiceTests : QuestionTemplatesAppServiceTests<IBLTermocasaMongoDbTestModule>
{
}