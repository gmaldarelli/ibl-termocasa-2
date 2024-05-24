using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Content;
using IBLTermocasa.Shared;

namespace IBLTermocasa.Contacts
{
    public partial interface IContactsAppService : IApplicationService
    {

        Task<PagedResultDto<ContactDto>> GetListAsync(GetContactsInput input);

        Task<ContactDto> GetAsync(Guid id);

        Task DeleteAsync(Guid id);

        Task<ContactDto> CreateAsync(ContactCreateDto input);

        Task<ContactDto> UpdateAsync(Guid id, ContactUpdateDto input);

        Task<IRemoteStreamContent> GetListAsExcelFileAsync(ContactExcelDownloadDto input);

        Task<IBLTermocasa.Shared.DownloadTokenResultDto> GetDownloadTokenAsync();
    }
}