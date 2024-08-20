using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IBLTermocasa.Common;
using IBLTermocasa.Contacts;
using IBLTermocasa.Materials;
using IBLTermocasa.Organizations;
using IBLTermocasa.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor;
using NUglify.Helpers;

namespace IBLTermocasa.Blazor.Components.Component;

public partial class AddContactToOrganization

{
    [CascadingParameter] private MudDialogInstance MudDialog { get; set; }
    [Parameter] public List<Guid> ExclusionContacts { get; set; } = new();
    [Parameter] public OrganizationDto OrganizationDto { get; set; } = new();
    [Inject] private IContactsAppService ContactsAppService { get; set; }
    private List<LookupDto<Guid>> ContactsCollection { get; set; } = new();
    

    private MudForm NewComponentForm { get; set; }
    private bool IsFormValid { get; set; }
    private bool HasDuplicateContact { get; set; }
    private string DuplicateContactErrorMessage { get; set; }
    private LookupDto<Guid> selectedContact;
    
    protected override async Task OnParametersSetAsync() {
        ContactsCollection = (await ContactsAppService.GetContactLookupAsync(new LookupRequestDto()))
            .Items.ToList();
    }
    
    
    private async Task ValidateForm() {
        await NewComponentForm.Validate();
        IsFormValid = NewComponentForm.IsValid && !HasDuplicateContact;
    }
    
    private async Task CheckForDuplicateContact() {
        HasDuplicateContact = ExclusionContacts
            .Any(contact => selectedContact.Id == contact);
        DuplicateContactErrorMessage = HasDuplicateContact ? L["TheContactAlreadyExists"] : string.Empty;
        await ValidateForm();
    }

    private void Cancel() {
        MudDialog.Cancel();
    }

    private async void Submit() {
        await NewComponentForm.Validate();
        await CheckForDuplicateContact();
        if (IsFormValid) {
            OrganizationDto.ListContacts.Add(new ContactPropertyDto(selectedContact.Id, selectedContact.DisplayName));
            MudDialog.Close(DialogResult.Ok(OrganizationDto));
        }
    }
}