using MHWApp.Library.Model;
using System.ComponentModel.DataAnnotations;

namespace MHWApp.Client.Pages.MainMenu.ShowAndEditSession
{
    public class EditSessionFormModel : IValidatableObject
    {
        public string CurrentName { get; set; }
        public string NewName { get; set; }

        public int CurrentDayCount { get; set; }
        public int NewDayCount { get; set; }

        public int CurrentMaxDays { get; set; }
        public int NewMaxDays { get; set; }

        public EditSessionFormModel(Session session)
        {
            NewName = CurrentName = session.Name;
            NewDayCount = CurrentDayCount = session.DayCount;
            NewMaxDays = CurrentMaxDays = session.MaxDays;
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            throw new NotImplementedException();
        }
    }
}
