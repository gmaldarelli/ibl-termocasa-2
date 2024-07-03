using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;
using Volo.Abp.Data;

namespace IBLTermocasa.ProfessionalProfiles
{
    public class ProfessionalProfileManager : DomainService
    {
        protected IProfessionalProfileRepository _professionalProfileRepository;

        public ProfessionalProfileManager(IProfessionalProfileRepository professionalProfileRepository)
        {
            _professionalProfileRepository = professionalProfileRepository;
        }

        public virtual async Task<ProfessionalProfile> CreateAsync(ProfessionalProfile professionalProfile)
        {
            Check.NotNull(professionalProfile, nameof(professionalProfile));
            return await _professionalProfileRepository.InsertAsync(professionalProfile);
        }

        public virtual async Task<ProfessionalProfile> UpdateAsync(Guid id, ProfessionalProfile professionalProfile)
        {
            Check.NotNull(professionalProfile, nameof(professionalProfile));
            var existingProfessionalProfile = await _professionalProfileRepository.GetAsync(id);
            ProfessionalProfile.FillPropertiesForUpdate(professionalProfile, existingProfessionalProfile);
            return await _professionalProfileRepository.UpdateAsync(existingProfessionalProfile);
        }
    }
}