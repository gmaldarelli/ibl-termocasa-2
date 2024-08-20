using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Blazorise;
using IBLTermocasa.Blazor.Components.Contact;
using IBLTermocasa.Contacts;
using IBLTermocasa.Permissions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Volo.Abp.AspNetCore.Components.Messages;
using Volo.Abp.AspNetCore.Components.Web.Theming.PageToolbars;
using Volo.Abp.Http.Client;
using BreadcrumbItem = Volo.Abp.BlazoriseUI.BreadcrumbItem;
namespace IBLTermocasa.Blazor.Pages.Crm;

public partial class ContactDetails
{

    [Inject] private IContactsAppService ContactsAppService { get; set; }
    [Inject] private NavigationManager NavigationManager { get; set; }
    [Inject] private IUiMessageService UiMessageService { get; set; }
    [Inject] private IRemoteServiceConfigurationProvider RemoteServiceConfigurationProvider { get; set; }
    
    [Parameter] public string ContactId { get; set; }
    protected List<BreadcrumbItem> BreadcrumbItems = new List<BreadcrumbItem>();
    protected PageToolbar Toolbar { get; } = new PageToolbar();
    private ContactInput ContactInputComponent { get; set; }
    private ContactDto ContactInput { get; set; }

    private bool CanCreateContact { get; set; }
    private bool CanEditContact { get; set; }
    private bool CanDeleteContact { get; set; }


    protected override async Task OnInitializedAsync()
    {
        Guid _tmpGuid = Guid.NewGuid();
        if (Guid.TryParse(ContactId, out _tmpGuid) && _tmpGuid != Guid.Empty)
        {
            Guid id = checkContactId(ContactId);
            await tryToLoadContact(id);
        }
        else
        {
            IsNew = true;
            ContactInput = new ContactDto();
        }

        await SetBreadcrumbItemsAsync();
        await SetPermissionsAsync();
    }

    public bool IsNew { get; set; } = false;

    private async Task SetPermissionsAsync()
    {
        CanCreateContact = await AuthorizationService
            .IsGrantedAsync(IBLTermocasaPermissions.Contacts.Create);
        CanEditContact = await AuthorizationService
            .IsGrantedAsync(IBLTermocasaPermissions.Contacts.Edit);
        CanDeleteContact = await AuthorizationService
            .IsGrantedAsync(IBLTermocasaPermissions.Contacts.Delete);
    }

    protected virtual ValueTask SetBreadcrumbItemsAsync()
    {
        BreadcrumbItems.Add(new BreadcrumbItem(L["Menu:Contacts"], "/contacts")
        );
        BreadcrumbItems.Add(new BreadcrumbItem($"{L["Menu:Contact"]} - {ContactInput.Name} {ContactInput.Surname}", $"/contact/{ContactInput.Id}"));

        return ValueTask.CompletedTask;
    }

    private async Task tryToLoadContact(Guid id)
    {
        try
        {
            ContactInput = await ContactsAppService.GetAsync(id);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            NavigationManager.NavigateTo("/error");
        }
    }

    private Guid checkContactId(string contactId)
    {
        if (!Guid.TryParse(contactId, out Guid id))
        {
            NavigationManager.NavigateTo("/error");
        }

        return id;
    }

    private async Task HandleContactSaved(ContactDto savedContact)
    {
        ContactInput = savedContact;
        ContactId = savedContact.Id.ToString();
        IsNew = false;
        
        BreadcrumbItems.Add(new BreadcrumbItem($"{ContactInput.Name}", $"/contact/{ContactInput.Id}"));
        var message = L["ContactSavedMessage"];
        if (!await UiMessageService.Confirm(message))
        {
            NavigationManager.NavigateTo($"/contact/{ContactInput.Id}");
        }
        else
        {
            NavigationManager.NavigateTo("/contacts");
        }
    }
    private async Task HandleContactCancel()
    {
        NavigationManager.NavigateTo("/contacts");
    }
}