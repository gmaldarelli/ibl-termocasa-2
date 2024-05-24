using IBLTermocasa.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;
using Volo.Abp.Data;

namespace IBLTermocasa.Interactions
{
    public class InteractionManager : DomainService
    {
        protected IInteractionRepository _interactionRepository;

        public InteractionManager(IInteractionRepository interactionRepository)
        {
            _interactionRepository = interactionRepository;
        }

        public virtual async Task<Interaction> CreateAsync(
        Guid writerUserId, Guid? notificationOrganizationUnitId, Guid? identityUserId, InteractionType interactionType, DateTime interactionDate, string? content = null, string? referenceObject = null, string? writerNotes = null)
        {
            Check.NotNull(writerUserId, nameof(writerUserId));
            Check.NotNull(interactionType, nameof(interactionType));
            Check.NotNull(interactionDate, nameof(interactionDate));
            Check.Length(content, nameof(content), InteractionConsts.ContentMaxLength);

            var interaction = new Interaction(
             GuidGenerator.Create(),
             writerUserId, notificationOrganizationUnitId, identityUserId, interactionType, interactionDate, content, referenceObject, writerNotes
             );

            return await _interactionRepository.InsertAsync(interaction);
        }

        public virtual async Task<Interaction> UpdateAsync(
            Guid id,
            Guid writerUserId, Guid? notificationOrganizationUnitId, Guid? identityUserId, InteractionType interactionType, DateTime interactionDate, string? content = null, string? referenceObject = null, string? writerNotes = null, [CanBeNull] string? concurrencyStamp = null
        )
        {
            Check.NotNull(writerUserId, nameof(writerUserId));
            Check.NotNull(interactionType, nameof(interactionType));
            Check.NotNull(interactionDate, nameof(interactionDate));
            Check.Length(content, nameof(content), InteractionConsts.ContentMaxLength);

            var interaction = await _interactionRepository.GetAsync(id);

            interaction.WriterUserId = writerUserId;
            interaction.NotificationOrganizationUnitId = notificationOrganizationUnitId;
            interaction.IdentityUserId = identityUserId;
            interaction.InteractionType = interactionType;
            interaction.InteractionDate = interactionDate;
            interaction.Content = content;
            interaction.ReferenceObject = referenceObject;
            interaction.WriterNotes = writerNotes;

            interaction.SetConcurrencyStampIfNotNull(concurrencyStamp);
            return await _interactionRepository.UpdateAsync(interaction);
        }

    }
}