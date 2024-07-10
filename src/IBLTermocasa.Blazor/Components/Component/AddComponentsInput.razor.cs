using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IBLTermocasa.Components;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using NUglify.Helpers;

namespace IBLTermocasa.Blazor.Components.Component;

public partial class AddComponentsInput
{
    [CascadingParameter] private MudDialogInstance MudDialog { get; set; }
    [Parameter] public List<string> ExclusionCodes { get; set; } = new();

    private MudForm NewComponentForm { get; set; }
    private ComponentDto NewComponent { get; set; } = new();
    private bool IsFormValid { get; set; }
    private bool HasDuplicateCode { get; set; }
    private string DuplicateCodeErrorMessage { get; set; }
    
    
    private async Task ValidateForm() {
        await NewComponentForm.Validate();
        IsFormValid = NewComponentForm.IsValid && !HasDuplicateCode;
    }
    
    private async Task CheckForDuplicateCode() {
        var codesLowerCase = ExclusionCodes.Select(x => x.ToLowerInvariant()).ToList();
        HasDuplicateCode = codesLowerCase.Contains(NewComponent.Code.ToLowerInvariant());
        DuplicateCodeErrorMessage = HasDuplicateCode ? L["TheCodeAlreadyExists"] : string.Empty;
        await ValidateForm();
    }

    private void Cancel() {
        MudDialog.Cancel();
    }

    private async void Submit() {
        await NewComponentForm.Validate();
        await CheckForDuplicateCode();
        if (IsFormValid) {
            MudDialog.Close(DialogResult.Ok(NewComponent));
        }
    }
}