using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using IBLTermocasa.Permissions;
using IBLTermocasa.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Caching.Distributed;
using MiniExcelLibs;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Authorization;
using Volo.Abp.Caching;
using Volo.Abp.Content;

namespace IBLTermocasa.ProfessionalProfiles
{
    [RemoteService(IsEnabled = false)]
    [Authorize(IBLTermocasaPermissions.ProfessionalProfiles.Default)]
    public class ProfessionalProfilesAppService : IBLTermocasaAppService, IProfessionalProfilesAppService
    {
        protected IDistributedCache<ProfessionalProfileExcelDownloadTokenCacheItem, string> _excelDownloadTokenCache;
        protected IProfessionalProfileRepository _professionalProfileRepository;
        protected ProfessionalProfileManager _professionalProfileManager;

        public ProfessionalProfilesAppService(IProfessionalProfileRepository professionalProfileRepository,
            ProfessionalProfileManager professionalProfileManager,
            IDistributedCache<ProfessionalProfileExcelDownloadTokenCacheItem, string> excelDownloadTokenCache)
        {
            _excelDownloadTokenCache = excelDownloadTokenCache;
            _professionalProfileRepository = professionalProfileRepository;
            _professionalProfileManager = professionalProfileManager;
        }

        public virtual async Task<PagedResultDto<ProfessionalProfileDto>> GetListAsync(
            GetProfessionalProfilesInput input)
        {
            var totalCount = await _professionalProfileRepository.GetCountAsync(input.FilterText, input.Name, input.StandardPrice,
                input.StandardPriceMin, input.StandardPriceMax);
            var items = await _professionalProfileRepository.GetListAsync(input.FilterText, input.Name, input.StandardPrice,
                input.StandardPriceMin, input.StandardPriceMax, input.Sorting, input.MaxResultCount, input.SkipCount);

            return new PagedResultDto<ProfessionalProfileDto>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<ProfessionalProfile>, List<ProfessionalProfileDto>>(items)
            };
        }

        public virtual async Task<ProfessionalProfileDto> GetAsync(Guid id)
        {
            return ObjectMapper.Map<ProfessionalProfile, ProfessionalProfileDto>(
                await _professionalProfileRepository.GetAsync(id));
        }

        [Authorize(IBLTermocasaPermissions.ProfessionalProfiles.Delete)]
        public virtual async Task DeleteAsync(Guid id)
        {
            await _professionalProfileRepository.DeleteAsync(id);
        }
        
        [Authorize(IBLTermocasaPermissions.ProfessionalProfiles.Create)]
        public virtual async Task<ProfessionalProfileDto> CreateAsync(ProfessionalProfileCreateDto input)
        {
            var professionalProfile = ObjectMapper.Map<ProfessionalProfileCreateDto, ProfessionalProfile>(input);
            return ObjectMapper.Map<ProfessionalProfile, ProfessionalProfileDto>(
                await _professionalProfileManager.CreateAsync(professionalProfile));
        }

        [Authorize(IBLTermocasaPermissions.ProfessionalProfiles.Edit)]
        public virtual async Task<ProfessionalProfileDto> UpdateAsync(Guid id, ProfessionalProfileUpdateDto input)
        {
            var professionalProfile = await _professionalProfileRepository.GetAsync(id);
            ObjectMapper.Map(input, professionalProfile);
            return ObjectMapper.Map<ProfessionalProfile, ProfessionalProfileDto>(
                await _professionalProfileManager.UpdateAsync(id, professionalProfile));
        }

        [AllowAnonymous]
        public virtual async Task<IRemoteStreamContent> GetListAsExcelFileAsync(
            ProfessionalProfileExcelDownloadDto input)
        {
            var downloadToken = await _excelDownloadTokenCache.GetAsync(input.DownloadToken);
            if (downloadToken == null || input.DownloadToken != downloadToken.Token)
            {
                throw new AbpAuthorizationException("Invalid download token: " + input.DownloadToken);
            }

            var items = await _professionalProfileRepository.GetListAsync(input.FilterText, input.Name,
                input.StandardPriceMin, input.StandardPriceMax);

            var memoryStream = new MemoryStream();
            await memoryStream.SaveAsAsync(
                ObjectMapper.Map<List<ProfessionalProfile>, List<ProfessionalProfileExcelDto>>(items));
            memoryStream.Seek(0, SeekOrigin.Begin);

            return new RemoteStreamContent(memoryStream, "ProfessionalProfiles.xlsx",
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        }

        public virtual async Task<DownloadTokenResultDto> GetDownloadTokenAsync()
        {
            var token = Guid.NewGuid().ToString("N");

            await _excelDownloadTokenCache.SetAsync(
                token,
                new ProfessionalProfileExcelDownloadTokenCacheItem { Token = token },
                new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(30)
                });

            return new DownloadTokenResultDto
            {
                Token = token
            };
        }

        [Authorize(IBLTermocasaPermissions.ProfessionalProfiles.Delete)]
        public virtual async Task DeleteByIdsAsync(List<Guid> professionalprofileIds)
        {
            await _professionalProfileRepository.DeleteManyAsync(professionalprofileIds);
        }

        [Authorize(IBLTermocasaPermissions.ProfessionalProfiles.Delete)]
        public virtual async Task DeleteAllAsync(GetProfessionalProfilesInput input)
        {
            await _professionalProfileRepository.DeleteAllAsync(input.FilterText, input.Name, input.StandardPriceMin,
                input.StandardPriceMax);
        }
    }
}