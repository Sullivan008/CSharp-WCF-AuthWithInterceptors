using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Controls;

namespace ClientForAuthenticationAndAuthorization.Validations
{
    public class OnlyDigitsValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            ValidationResult validationResult = new ValidationResult(true, null);

            if (value != null && !string.IsNullOrEmpty(value.ToString()))
            {
                Regex regex = new Regex("[^0-9.-]+");
                bool parsingOk = !regex.IsMatch(value.ToString());

                if (!parsingOk)
                {
                    validationResult = new ValidationResult(false, "Illegal Characters, Please Enter Numeric Value");
                }
            }

            return validationResult;
        }
    }
}
