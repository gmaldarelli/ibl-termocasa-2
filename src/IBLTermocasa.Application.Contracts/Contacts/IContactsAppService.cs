using System;
using System.Threading.Tasks;
using IBLTermocasa.Shared;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Content;

namespace IBLTermocasa.Contacts
{
    public interface IContactsAppService : IApplicationService
    {

        Task<PagedResultDto<ContactDto>> GetListAsync(GetContactsInput input);

        Task<ContactDto> GetAsync(Guid id);

        Task DeleteAsync(Guid id);

        Task<ContactDto> CreateAsync(ContactCreateDto input);

        Task<ContactDto> UpdateAsync(Guid id, ContactUpdateDto input);

        Task<IRemoteStreamContent> GetListAsExcelFileAsync(ContactExcelDownloadDto input);

        Task<DownloadTokenResultDto> GetDownloadTokenAsync();
    }
}