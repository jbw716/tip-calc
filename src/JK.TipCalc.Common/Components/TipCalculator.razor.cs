﻿using JK.TipCalc.Common.Models;
using Microsoft.AspNetCore.Components;

namespace JK.TipCalc.Common.Components;
public partial class TipCalculator
{
    protected TipCalculatorViewModel ViewModel { get; private set; }

    protected override void OnInitialized()
    {
        const int customTipPercent = 25;
        var percents = new int[] { 15, 18, 20 };
        this.ViewModel = TipCalculatorViewModel.Create(customTipPercent, percents);
    }

    protected void HandleCustomTipAmountChanged(ChangeEventArgs e)
    {
        try
        {
            this.ViewModel.Errors.Clear();
            var candidate = e?.Value?.ToString()?.Trim() ?? string.Empty;
            this.ViewModel.ChangeCustomTip(candidate);
        }
        catch (InvalidPercentException ex)
        {
            var message = ex.Message.Remove(ex.Message.IndexOf('('));
            this.ViewModel.Errors.Add(message);
        }
    }

    protected void AmountChanged(ChangeEventArgs e)
    {
        this.ViewModel.Amount = e?.Value?.ToString() ?? "";
    }

    protected void DiscountChanged(ChangeEventArgs e)
    {
        this.ViewModel.Discount = e?.Value?.ToString() ?? "";
    }
}
