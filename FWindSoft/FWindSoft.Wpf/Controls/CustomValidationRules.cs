using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows.Controls;

namespace FWindSoft.Wpf
{
    public class ValidateRules : Collection<ValidationRule>
    {
    }

    public class ValidateInteger : ValidationRule
    {
        public int MaxValue { set; get; }
        public int MinValue { set; get; }

        public ValidateInteger()
        {
            this.MaxValue = Int32.MaxValue;
            this.MinValue = Int32.MinValue;
        }

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            Int32 tempValue;
            if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
                return new ValidationResult(false, "输入值不能为空");
            if (Int32.TryParse(value.ToString(), out tempValue))
            {
                if (tempValue > this.MinValue && tempValue < this.MaxValue)
                {
                    return new ValidationResult(true,null);
                }
                else
                {
                    return new ValidationResult(false,string.Format("请输入正确的范围[{0}--{1}]",this.MinValue,this.MaxValue));
                }

            }
            else
            {
                return new ValidationResult(false,"请输入整数");
            }
        }
    }
    public class ValidateDecimal : ValidationRule
    {
        public double MaxValue { set; get; }
        public double MinValue { set; get; }

        public ValidateDecimal()
        {
            this.MaxValue = double.MaxValue;
            this.MinValue = double.MinValue;
        }

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            double tempValue;
            if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
                return new ValidationResult(false, "输入值不能为空");
            if (double.TryParse(value.ToString(), out tempValue))
            {
                if (tempValue >= this.MinValue && tempValue <= this.MaxValue)
                {
                    return new ValidationResult(true, null);
                }
                else
                {
                    return new ValidationResult(false, string.Format("请输入正确的范围[{0}--{1}]", this.MinValue, this.MaxValue));
                }

            }
            else
            {
                return new ValidationResult(false, "请输入正确数字");
            }
        }
    }
}
