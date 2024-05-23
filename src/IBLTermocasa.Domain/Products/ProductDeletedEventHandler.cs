using IBLTermocasa.Subproducts;

using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Entities.Events;
using Volo.Abp.EventBus;

namespace IBLTermocasa.Products;

public class ProductDeletedEventHandler : ILocalEventHandler<EntityDeletedEventData<Product>>, ITransientDependency
{
    private readonly ISubproductRepository _subproductRepository;

    public ProductDeletedEventHandler(ISubproductRepository subproductRepository)
    {
        _subproductRepository = subproductRepository;

    }

    public async Task HandleEventAsync(EntityDeletedEventData<Product> eventData)
    {
        if (eventData.Entity is not ISoftDelete softDeletedEntity)
        {
            return;
        }

        if (!softDeletedEntity.IsDeleted)
        {
            return;
        }

        try
        {
            await _subproductRepository.DeleteManyAsync(await _subproductRepository.GetListByProductIdAsync(eventData.Entity.Id));

        }
        catch
        {
            //...
        }
    }
}