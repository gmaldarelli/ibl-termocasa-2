using System;
using System.Threading.Tasks;
using AutoMapper;
using Blazorise;
using Force.DeepCloner;
using IBLTermocasa.Contacts;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Volo.Abp;

namespace IBLTermocasa.Blazor.Components.Contact;

public partial class ContactInput
{
    private Validations ContactValidations { get; set; } = new();
    private ContactCreateDto NewContact { get; set; }

    private ContactUpdateDto EditingContact { get; set; }
    
    private string _contactImageString = "/images/no-photo.jpeg";
    [Inject] private IMapper _mapper { get; set; }
    
    [Inject] private IContactsAppService ContactsAppService { get; set; }
    [Parameter] public ContactDto Contact { get; set; }
    
    private ContactDto InternalContact; 
    [Parameter] public bool IsNew { get; set; }
    
    [Parameter] public EventCallback<ContactDto> OnContactSaved { get; set; }
    
    [Parameter] public EventCallback<ContactDto> OnContactCancel { get; set; }

    private bool _isComponentRendered = false;
    private async Task HandleValidSubmit()
    {
        if (await ContactValidations.ValidateAll() == false)
        {
            return;
        }
        Console.WriteLine("Entro nel metodo HandleValidSubmit()");
        try
        {
            // copy Contact properties to EditingContact with mapper
            if (IsNew)
            {
                InternalContact.ConcurrencyStamp = Guid.NewGuid().ToString();
                NewContact = _mapper.Map<ContactDto, ContactCreateDto>(InternalContact);
                var result = await ContactsAppService.CreateAsync(NewContact);
                Contact = _mapper.Map<ContactDto>(result);
                IsNew = false;
            }
            else
            {
                EditingContact = _mapper.Map<ContactDto, ContactUpdateDto>(InternalContact);
                var result = await ContactsAppService.UpdateAsync(InternalContact.Id, EditingContact);
                Contact = _mapper.Map<ContactDto>(result);
                InternalContact = Contact.DeepClone();
            }
            if (_isComponentRendered)
            {
                await OnContactSaved.InvokeAsync(Contact);
            }
            StateHasChanged();
        }
        catch (Exception ex)
        {
            throw new UserFriendlyException(ex.Message);
        }
    }

    private async Task HandleCancel()
    {
        var message = L["ConfirmCancelMessage"];
        if (!await UiMessageService.Confirm(message))
        {
            return;
        }
        else
        {
            InternalContact = null;
            await OnContactCancel.InvokeAsync(Contact);
        }
    }
    
    
    protected override async Task OnParametersSetAsync()
    {
        if (Contact == null)
        {
            Contact = new ContactDto();
        }

        InternalContact = Contact.DeepClone();

        if (Contact.ImageId != null && Contact.ImageId != Guid.Empty)
        { 
            Console.WriteLine($">>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>Contact.ImageId: {Contact.ImageId}");
            var remoteService = await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("IBLTermocasa") ?? await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Default");
            var token = await FileDescriptorAppService.GetDownloadTokenAsync(Contact.ImageId!.Value);
            if (token is { Token: not null })
            {
                _contactImageString = $"{remoteService?.BaseUrl.EnsureEndsWith('/') ?? string.Empty}api/file-management/file-descriptor/download/{Contact.ImageId}?token={token.Token}";
                Console.WriteLine($">>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>_contactImageString {_contactImageString}");
            }
        }
        
        StateHasChanged();
    }


    protected override void OnParametersSet()
    {
        StateHasChanged();
    }

    protected override void OnAfterRender(bool firstRender)
    {
        if (firstRender)
        {
            _isComponentRendered = true;
        }
    }
}