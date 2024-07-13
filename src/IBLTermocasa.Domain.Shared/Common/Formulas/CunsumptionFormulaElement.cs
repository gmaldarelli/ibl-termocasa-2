using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.IdentityModel.Tokens;

namespace IBLTermocasa.Common.Formulas;

public class ProductComponentCunsumptionFormulaElement
{
    public string PlaceHolder { get; set; }
    public Guid ProductItemId { get; set; }
    public Guid ProducComponentId { get; set; }
    public Guid ComponentId { get; set; }
    public string Code { get; set; }
    public string ConsumptionCalculation { get; set; }
    public List<string> Variables { get; }
    public Dictionary<string, object> VariablesValues { get; }
    
    public object? Value => CalculateValue();
    
    public bool IsValueAvailable => ChckeCalculated();
    
    public void SetVariableValue(string variable, object value)
    {
        if (Variables.Contains(variable))
        {
            VariablesValues[variable] = value;
        }
    }

    protected ProductComponentCunsumptionFormulaElement()
    {
        Variables = new List<string>();
        VariablesValues = new Dictionary<string, object>();
    }

    public ProductComponentCunsumptionFormulaElement(Guid productItemId, Guid producComponentId, Guid componentId , string placeHolder, string code, string consumptionCalculation)
    {
        ProductItemId = productItemId;
        ComponentId = componentId;
        PlaceHolder = placeHolder;
        ProducComponentId = producComponentId;
        Code = code;
        ConsumptionCalculation = consumptionCalculation;
        VariablesValues = new Dictionary<string, object>();
        Variables = ExtractVariables(consumptionCalculation);
    }

    private List<string> ExtractVariables(string formula)
    {
        var matches = Regex.Matches(formula, @"\{([^}]+)\}");
        var variables = new List<string>();
        foreach (Match match in matches)
        {
            variables.Add($"{{{match.Groups[1].Value}}}");
        }

        return variables;
    }

    private bool ChckeCalculated()
    {
        return VariablesValues.Count == Variables.Count;
    }
    private object CalculateValue()
    {
        if (CollectionUtilities.IsNullOrEmpty(ConsumptionCalculation))
        {
            return null;
        }

        if (VariablesValues.Count < Variables.Count)
        {
            throw new Exception("Not all variables have been set");
        }

        var formula = ConsumptionCalculation;
        VariablesValues.ToList().ForEach(x =>
        {
            var value = x.Value switch
            {
                decimal d => d.ToString(CultureInfo.CreateSpecificCulture("en")),
                string s => s.Replace(",", "."),
                int i => i.ToString(),
                double b => b.ToString().Replace(",", "."),
                bool bl => bl ? "1" : "0",
                _ => x.Value.ToString()
            };
            formula = formula.Replace(x.Key , value);
        });
        Console.WriteLine($"Extracted formula: {formula}");
        DataTable dt = new DataTable();
        var result = dt.Compute(formula, "");
        return result;
    }
}