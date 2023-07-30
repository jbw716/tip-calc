using System.Text.RegularExpressions;

namespace JK.TipCalc.Common.Models;
public partial class TipCalculatorViewModel
{
    private string amount;
    private string discount;

    private TipCalculatorViewModel(int customTipPercent, IEnumerable<int> percentList)
    {
        this.State = CalculatorState.Default;
        var fullList = percentList.Union(new int[] { customTipPercent });
        this.Tips = new TipCollection(fullList);
    }

    public CalculatorState State { get; private set; }
    public TipCollection Tips { get; private set; }
    public Tip CustomTip => this.Tips.Items.Last();
    public List<string> Errors { get; private set; } = new();
    public bool HasErrors => this.Errors.Any();

    public string Amount
    {
        get => this.amount;
        set
        {
            value = value.Replace(".", "");
            decimal decimalValue;
            if (Regex.Match(value, "^0*$").Success)
            {
                decimalValue = 0;
            }
            else if (decimal.TryParse(value, out decimal number))
            {
                decimalValue = number / 100;
            }
            else
            {
                value = this.amount;
                this.amount = $"{value:0.00}";
                return;
            }

            if (decimalValue <= 9999999999.99m)
            {
                value = $"{decimalValue:0.00}";
                this.amount = value;
                this.Tips.SetMealValues(this.AmountDecimal, this.DiscountDecimal);
            }
        }
    }

    public string Discount
    {
        get => this.discount;
        set
        {
            value = value.Replace(".", "");
            decimal decimalValue;
            if (Regex.Match(value, "^0*$").Success)
            {
                decimalValue = 0;
            }
            else if (decimal.TryParse(value, out decimal number))
            {
                decimalValue = number / 100;
            }
            else
            {
                value = this.discount;
                this.discount = $"{value:0.00}";
                return;
            }

            if (decimalValue <= 9999999999.99m)
            {
                value = $"{decimalValue:0.00}";
                this.discount = value;
                this.Tips.SetMealValues(this.AmountDecimal, this.DiscountDecimal);
            }
        }
    }

    public decimal AmountDecimal => decimal.TryParse(this.Amount, out decimal number) ? number : 0;
    public decimal DiscountDecimal => decimal.TryParse(this.Discount, out decimal number) ? number : 0;

    public static TipCalculatorViewModel Create(int customTipPercent, int[] percentList)
        => new(customTipPercent, percentList);

    public void ChangeCustomTip(string value)
    {
        var newValue = int.TryParse(value, out int intValue) ? intValue : 0;
        if (newValue < 0 || newValue > 100)
        {
            throw new InvalidPercentException(nameof(newValue));
        }

        this.CustomTip.Percent.SetValue(newValue);
        this.CustomTip.SetMealValues(this.AmountDecimal, this.DiscountDecimal);
        switch (this.State)
        {
            case CalculatorState.RoundDown:
                this.CustomTip.RoundDown();
                break;
            case CalculatorState.RoundUp:
                this.CustomTip.RoundUp();
                break;
                //case CalculatorState.Default:
                //default:
        }
    }

    public void Reset()
    {
        this.Tips.SetMealValues(this.AmountDecimal, this.DiscountDecimal);
        this.State = CalculatorState.Default;
    }

    public void RoundDown()
    {
        if (this.State == CalculatorState.RoundUp)
        {
            Reset();
        }
        this.Tips.RoundDown();
        this.State = CalculatorState.RoundDown;
    }

    public void RoundUp()
    {
        if (this.State == CalculatorState.RoundDown)
        {
            Reset();
        }
        this.Tips.RoundUp();
        this.State = CalculatorState.RoundUp;
    }
}