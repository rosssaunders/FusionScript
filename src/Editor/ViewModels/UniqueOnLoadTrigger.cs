using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using RxdSolutions.FusionScript.Model;
using RxdSolutions.FusionScript.ViewModels;

namespace RxdSolutions.FusionScript.Validation
{
    public class UniqueOnLoadTrigger : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var tmpCollection = (ObservableCollection<TriggerViewModel>)value;

            if (tmpCollection.Count == 0)
                return ValidationResult.Success;

            var count = tmpCollection.Count(x => x.Trigger == Trigger.Load);

            if (count <= 1)
                return ValidationResult.Success;

            return new ValidationResult(ErrorMessage, new[] { validationContext.MemberName });
        }
    }
}
