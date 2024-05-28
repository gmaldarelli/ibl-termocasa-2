using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazorise;
using DocumentFormat.OpenXml.Office2010.ExcelAc;
using IBLTermocasa.RequestForQuotations;
using Microsoft.AspNetCore.Components;
using Volo.Abp;

namespace IBLTermocasa.Blazor.Components.RequestForQuotation;

public partial class RequestForQuotationItemInput
{
    private Validations RequestForQuotationItemValidations { get; set; } = new();
    
    //private QuestionTemplate SelectedQuestionTemplate = new QuestionTemplate();
    private bool _isComponentRendered;
    [Parameter] public RequestForQuotationItemDto SelectedRequestForQuotationItem { get; set; }
    [Parameter] public bool IsNew { get; set; }
    [Parameter] public List<(Guid,string)> QuestionTemplateNames { get; set; }
    [Parameter] public EventCallback<RequestForQuotationItemDto> OnRequestForQuotationItemSaved { get; set; }
    [Parameter] public EventCallback OnRequestForQuotationItemCancel { get; set; }
    private Modal EditQuestionTemplateModal { get; set; } = new();
    
    protected override void OnAfterRender(bool firstRender)
    {
        if (firstRender)
        {
            _isComponentRendered = true;
        }
    }
    private async Task CloseEditRequestForQuotationItemModalAsync()
    {
        await EditQuestionTemplateModal.Hide();
    }
    /*private async Task OpenEditQuestionTemplateModalAsync(QuestionTemplate input)
    {
        if (input == null)
        {
            SelectedQuestionTemplate = new QuestionTemplate();
        }
        else
        {
            SelectedQuestionTemplate = input;
        }

        EditQuestionTemplateModal.Show();
    }*/
    
    private async Task CloseQuestionTemplateModalAsync()
    {
        await EditQuestionTemplateModal.Hide();
    }
    private async Task CreateRequestForQuotationAsync()
    {
        await EditQuestionTemplateModal.Hide();
    }
    private async Task HandleValidSubmit()
    {
        try
        {
            if (await RequestForQuotationItemValidations.ValidateAll() == false)
            {
                return;
            }
            if (_isComponentRendered)
            {
                await OnRequestForQuotationItemSaved.InvokeAsync(SelectedRequestForQuotationItem);
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
        await OnRequestForQuotationItemCancel.InvokeAsync();
    }
    
    
    /*private Task OnQuestionTemplateSaved(QuestionTemplate questionTemplate)
    {
        if (questionTemplate.Id == Guid.Empty)
        {
            questionTemplate.Id = Guid.NewGuid();
        }
        var list = SelectedRequestForQuotationItem.QuestionTemplates.ToList();
        list.Add(questionTemplate);
        SelectedRequestForQuotationItem.QuestionTemplates = list;
        EditQuestionTemplateModal.Hide();
        StateHasChanged();
        return Task.CompletedTask;
    }
    private Task OnQuestionTemplateCancel()
    {
        SelectedQuestionTemplate = new QuestionTemplate();
        EditQuestionTemplateModal.Hide();
        return Task.CompletedTask;
    }*/
    
    
    private string GetQuestionName(Guid questionId)
    {
        var question = QuestionTemplateNames.FirstOrDefault(item => item.Item1 == questionId);
        return question.Item2;
    }
}