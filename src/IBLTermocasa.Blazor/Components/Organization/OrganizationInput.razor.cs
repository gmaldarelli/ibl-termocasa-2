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
    [Parameter] public OrganizationDto OrganizationParameter { get; set; }
    private OrganizationDto InternalOrganization; 
    [Parameter] public bool IsNew { get; set; }

    [Parameter] public List<OrganizationType> OrganizationTypes { get; set; }
    [Parameter] public List<IndustryDto> Industries { get; set; }

    [Parameter] public EventCallback<OrganizationDto> OnOrganizationSaved { get; set; }
    
    [Parameter] public EventCallback<OrganizationDto> OnOrganizationCancel { get; set; }
    public MudForm MudFormInternalOrganization;


    private bool _isComponentRendered = false;
    private string[] Errors = [];
    private object Validations;
    private bool InternalOrganizationIsValid;
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
                NewOrganization = _mapper.Map<OrganizationDto, OrganizationCreateDto>(InternalOrganization);
                var result = await OrganizationsAppService.CreateAsync(NewOrganization);
                OrganizationParameter = _mapper.Map<OrganizationDto>(result);
                InternalOrganization = OrganizationParameter.DeepClone();
            }
            else
            {
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
        if (OrganizationParameter == null)
        {
            OrganizationParameter = new OrganizationDto();
        }
        InternalOrganization = OrganizationParameter.DeepClone();
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