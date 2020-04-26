using System.ComponentModel.DataAnnotations;
using System.Linq;
using RxdSolutions.FusionScript.ViewModels;

namespace RxdSolutions.FusionScript.Validation
{
    public class UniqueMacroName : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var name = (string)value;
            var item = (ScriptViewModel)validationContext.ObjectInstance;

            var allModels = Enumerable.Empty<ScriptViewModel>(); // Main.ModelCache.GetAll();
            if (allModels.Where(x => x.Id != item.Id).Any(x => x.Name.Equals(name, System.StringComparison.InvariantCultureIgnoreCase)))
            {
                return new ValidationResult(ErrorMessage, new[] { validationContext.MemberName });
            }

            return ValidationResult.Success;
        }
    }
}
