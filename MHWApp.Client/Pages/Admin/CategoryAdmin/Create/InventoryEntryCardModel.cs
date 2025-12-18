using MHWApp.Library.Model;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MHWApp.Client.Pages.Admin.CategoryAdmin.Create
{
    public class InventoryEntryCardModel : INotifyPropertyChanged
    {
        private int key;
        private string name = string.Empty;
        private string extendedName = string.Empty;
        private GameExtension? gameExtension;

        public int Key 
        { 
            get => key; 
            set => SetField(ref key, value); 
        }

        public string Name 
        { 
            get => name; 
            set => SetField(ref name, value); 
        }
        public string ExtendedName 
        { 
            get => extendedName; 
            set => SetField(ref extendedName, value); 
        }
        public GameExtension? GameExtension 
        { 
            get => gameExtension; 
            set => SetField(ref gameExtension, value); 
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName!);
            return true;
        }

        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
