using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Blazorise;
using Force.DeepCloner;
using IBLTermocasa.Common;
using IBLTermocasa.Contacts;
using IBLTermocasa.Industries;
using IBLTermocasa.Organizations;
using IBLTermocasa.Permissions;
using IBLTermocasa.Shared;
using IBLTermocasa.Types;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Volo.Abp;

namespace IBLTermocasa.Blazor.Components.Organization;

public partial class OrganizationInput
{
    private Validations OrganizationValidations { get; set; } = new();

    private OrganizationCreateDto NewOrganization { get; set; }
    private OrganizationUpdateDto EditingOrganization { get; set; }

    private string _organizaionImageString = "/images/no-photo.jpeg";

    [Inject] private IMapper _mapper { get; set; }
    [Inject] public IContactsAppService ContactsAppService { get; set; }
    [Parameter] public OrganizationDto OrganizationParameter { get; set; }
    private OrganizationDto InternalOrganization; 
    [Parameter] public bool IsNew { get; set; }

    [Parameter] public List<OrganizationType> OrganizationTypes { get; set; }
    [Parameter] public List<IndustryDto> Industries { get; set; }

    [Parameter] public EventCallback<OrganizationDto> OnOrganizationSaved { get; set; }
    
    [Parameter] public EventCallback<OrganizationDto> OnOrganizationCancel { get; set; }
    public MudForm MudFormInternalOrganization;
    
    private List<LookupDto<Guid>> ContactsCollection { get; set; } = new();

    private bool _isComponentRendered = false;
    private string[] Errors = [];
    private object Validations;
    private bool InternalOrganizationIsValid;
    private bool _isLoading = true;
    
    
    protected override async Task OnInitializedAsync()
    {
        _isLoading = true;
        ContactsCollection = (await ContactsAppService.GetContactLookupAsync(new LookupRequestDto())).Items.ToList();
        await base.OnInitializedAsync();
        _isLoading = false;
    }
    
    protected override async Task OnParametersSetAsync()
    {
        if (!ContactsCollection.IsNullOrEmpty())
        {
            ListAllContacts = ContactsCollection.Select(x => new ContactPropertyDto
            {
                Id = x.Id,
                Name = x.DisplayName
            }).ToList();
            ListContactGuids = ListAllContacts.Select(x => x.Id).ToList();
        }
        
        if (OrganizationParameter == null)
        {
            OrganizationParameter = new OrganizationDto();
        }
        InternalOrganization = OrganizationParameter.DeepClone();
        if (InternalOrganization is not null && InternalOrganization.ListContacts.Count != 0)
        {
            SelectedContactGuids = InternalOrganization.ListContacts.Select(x => x.Id).ToList();
        }
    }
    
    
    
    private async Task HandleValidSubmit()
    {
        if(IsNew)
        {
            InternalOrganization.ConcurrencyStamp = Guid.NewGuid().ToString();
        }
        MudFormInternalOrganization.Validate();
        if(Errors.Length > 0)
        {
            return;
        }
        try
        {
            if (IsNew)
            {
                var contactProperties = ContactsCollection
                    .Where(x => SelectedContactGuids.Any(sc => sc == x.Id))
                    .ToList();
                InternalOrganization.ListContacts = contactProperties.Select(x => new ContactPropertyDto
                {
                    Id = x.Id,
                    Name = x.DisplayName
                }).ToList();
                NewOrganization = _mapper.Map<OrganizationDto, OrganizationCreateDto>(InternalOrganization);
                var result = await OrganizationsAppService.CreateAsync(NewOrganization);
                OrganizationParameter = _mapper.Map<OrganizationDto>(result);
                InternalOrganization = OrganizationParameter.DeepClone();
            }
            else
            {
                var contactProperties = ContactsCollection
                    .Where(x => SelectedContactGuids.Any(sc => sc == x.Id))
                    .ToList();
                InternalOrganization.ListContacts = contactProperties.Select(x => new ContactPropertyDto
                {
                    Id = x.Id,
                    Name = x.DisplayName
                }).ToList();
                EditingOrganization = _mapper.Map<OrganizationDto, OrganizationUpdateDto>(InternalOrganization);
                var result = await OrganizationsAppService.UpdateAsync(InternalOrganization.Id, EditingOrganization);
                InternalOrganization = result.DeepClone();

            }
            if (_isComponentRendered)
            {
                await OnOrganizationSaved.InvokeAsync(OrganizationParameter);
            }

        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.StackTrace);
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
            InternalOrganization = null;
            await OnOrganizationCancel.InvokeAsync();
        }
    }
    
    
    protected override void OnAfterRender(bool firstRender)
    {
        if (firstRender)
        {
            _isComponentRendered = true;
        }
    }

    
}