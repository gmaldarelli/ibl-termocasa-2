using IBLTermocasa.ComponentItems;

using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Entities.Events;
using Volo.Abp.EventBus;

namespace IBLTermocasa.Components;

public class ComponentDeletedEventHandler : ILocalEventHandler<EntityDeletedEventData<Component>>, ITransientDependency
{
    private readonly IComponentItemRepository _componentItemRepository;

    public ComponentDeletedEventHandler(IComponentItemRepository componentItemRepository)
    {
        _componentItemRepository = componentItemRepository;

    }

    public async Task HandleEventAsync(EntityDeletedEventData<Component> eventData)
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
            await _componentItemRepository.DeleteManyAsync(await _componentItemRepository.GetListByComponentIdAsync(eventData.Entity.Id));

        }
        catch
        {
            //...
        }
    }
}