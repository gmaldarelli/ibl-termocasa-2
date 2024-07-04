using System;
using System.Threading.Tasks;
using AutoMapper;
using Force.DeepCloner;
using IBLTermocasa.ProfessionalProfiles;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Volo.Abp.Http.Client;

namespace IBLTermocasa.Blazor.Components.ProfessionalProfile;

public partial class ProfessionalProfileInput
{
    [CascadingParameter] MudDialogInstance Dialog { get; set; }
    [Parameter] public ProfessionalProfileDto ProfessionalProfile { get; set; } = new();
    [Parameter] public EventCallback<ProfessionalProfileDto> OnProfessionalProfileSaved { get; set; }
    [Parameter] public EventCallback<ProfessionalProfileDto> OnProfessionalProfileCancel { get; set; }
    [Parameter] public bool DisplayReadOnly { get; set; }
    [Parameter] public bool IsNew { get; set; }
    [Inject] private IMapper _mapper { get; set; }
    [Inject] public IProfessionalProfilesAppService ProfessionalProfilesAppService { get; set; }
    [Inject] public IDialogService DialogService { get; set; }
    
    private bool success;
    private bool _isComponentRendered;
    private ProfessionalProfileDto InternalProfessionalProfile = new();

    //è importante sapere che OnParametersSetAsync viene chiamato prima di tutti, anche di OnInitializedAsync()
    
    private string GetButtonText()
    {
        return DisplayReadOnly ? "Back" : "Cancel";
    }
    protected override async Task OnParametersSetAsync()
    {
        InternalProfessionalProfile = IsNew ? new ProfessionalProfileDto() : ProfessionalProfile.DeepClone();
        StateHasChanged();
    }

    private async Task HandleValidSubmit()
    {
        try
        {
            ProfessionalProfileDto result;
            if (IsNew)
            {
                result = await ProfessionalProfilesAppService.CreateAsync(_mapper.Map<ProfessionalProfileCreateDto>(InternalProfessionalProfile));
            }
            else
            {
                result = await ProfessionalProfilesAppService.UpdateAsync(InternalProfessionalProfile.Id, _mapper.Map<ProfessionalProfileUpdateDto>(InternalProfessionalProfile));
            }

            ProfessionalProfile = _mapper.Map<ProfessionalProfileDto>(result);
            InternalProfessionalProfile = ProfessionalProfile.DeepClone();
            await OnProfessionalProfileSaved.InvokeAsync(InternalProfessionalProfile);
            Dialog.Close(DialogResult.Ok(result));
        }
        catch (AbpRemoteCallException ex)
        {
            // Log the exception and show a message to the user
            Console.WriteLine($"Errore durante la chiamata remota: {ex.Message}");
            // Optionally, you can use a MudBlazor Snackbar to show an error message to the user
            // _snackbar.Add("Errore durante il salvataggio del profilo. Per favore riprova.", Severity.Error);
        }
        catch (Exception ex)
        {
            // Handle other exceptions
            Console.WriteLine($"Errore generico: {ex.Message}");
            // Optionally, show a message to the user
            // _snackbar.Add("Si è verificato un errore inaspettato. Per favore riprova.", Severity.Error);
        }
        finally
        {
            StateHasChanged();
        }
    }

    private void HandleCancel()
    { 
        Dialog.Cancel();
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