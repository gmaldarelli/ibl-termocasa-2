using IBLTermocasa.Types;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace IBLTermocasa.Interactions
{
    public interface IInteractionRepository : IRepository<Interaction, Guid>
    {
        Task<InteractionWithNavigationProperties> GetWithNavigationPropertiesAsync(
    Guid id,
    CancellationToken cancellationToken = default
);

        Task<List<InteractionWithNavigationProperties>> GetListWithNavigationPropertiesAsync(
            string? filterText = null,
            InteractionType? interactionType = null,
            DateTime? interactionDateMin = null,
            DateTime? interactionDateMax = null,
            string? content = null,
            string? referenceObject = null,
            string? writerNotes = null,
            Guid? writerUserId = null,
            Guid? identityUserId = null,
            string? sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default
        );

        Task<List<Interaction>> GetListAsync(
                    string? filterText = null,
                    InteractionType? interactionType = null,
                    DateTime? interactionDateMin = null,
                    DateTime? interactionDateMax = null,
                    string? content = null,
                    string? referenceObject = null,
                    string? writerNotes = null,
                    string? sorting = null,
                    int maxResultCount = int.MaxValue,
                    int skipCount = 0,
                    CancellationToken cancellationToken = default
                );

        Task<long> GetCountAsync(
            string? filterText = null,
            InteractionType? interactionType = null,
            DateTime? interactionDateMin = null,
            DateTime? interactionDateMax = null,
            string? content = null,
            string? referenceObject = null,
            string? writerNotes = null,
            Guid? writerUserId = null,
            Guid? identityUserId = null,
            CancellationToken cancellationToken = default);
    }
}