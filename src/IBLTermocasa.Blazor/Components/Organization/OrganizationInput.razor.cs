using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Blazorise;
using Force.DeepCloner;
using IBLTermocasa.Industries;
using IBLTermocasa.Organizations;
using IBLTermocasa.Types;
using Microsoft.AspNetCore.Components;
using Volo.Abp;

namespace IBLTermocasa.Blazor.Components.Organization;

public partial class OrganizationInput
{
    private Validations OrganizationValidations { get; set; } = new();

    private OrganizationCreateDto NewOrganization { get; set; }
    private OrganizationUpdateDto EditingOrganization { get; set; }

    private string _organizaionImageString = "/images/no-photo.jpeg";

    [Inject] private IMapper _mapper { get; set; }
    [Parameter] public OrganizationDto Organization { get; set; }
    private OrganizationDto InternalOrganization; 
    [Parameter] public bool IsNew { get; set; }

    [Parameter] public List<OrganizationType> OrganizationTypes { get; set; }
    [Parameter] public List<IndustryDto> Industries { get; set; }

    [Parameter] public EventCallback<OrganizationDto> OnOrganizationSaved { get; set; }
    
    [Parameter] public EventCallback<OrganizationDto> OnOrganizationCancel { get; set; }


    private bool _isComponentRendered = false;
    private async Task HandleValidSubmit()
    {
        if(IsNew)
        {
            InternalOrganization.ConcurrencyStamp = Guid.NewGuid().ToString();
        }
        if (await OrganizationValidations.ValidateAll() == false)
        {
            return;
        }
        try
        {
            // copy organization properties to EditingOrganization with mapper
            if (IsNew)
            {
                NewOrganization = _mapper.Map<OrganizationDto, OrganizationCreateDto>(InternalOrganization);
                var result = await OrganizationsAppService.CreateAsync(NewOrganization);
                Organization = _mapper.Map<OrganizationDto>(result);
                InternalOrganization = Organization.DeepClone();
            }
            else
            {
                EditingOrganization = _mapper.Map<OrganizationDto, OrganizationUpdateDto>(InternalOrganization);
                var result = await OrganizationsAppService.UpdateAsync(InternalOrganization.Id, EditingOrganization);
                Organization = _mapper.Map<OrganizationDto>(result);
                InternalOrganization = Organization.DeepClone();

            }
            if (_isComponentRendered)
            {
                await OnOrganizationSaved.InvokeAsync(Organization);
            }

        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.StackTrace);
            throw new UserFriendlyException(ex.Message);
        }
    }

    private async  Task HandleCancel()
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
    
    
    protected override async Task OnParametersSetAsync()
    {
        if (Organization == null)
        {
            Organization = new OrganizationDto();
        }
        InternalOrganization = Organization.DeepClone();
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