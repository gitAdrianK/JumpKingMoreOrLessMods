namespace LessAutoEquipping
{
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    public class Preferences : INotifyPropertyChanged
    {
        private bool preventAutoEquip = false;

        public bool ShouldPreventAutoEquip
        {
            get => this.preventAutoEquip;
            set
            {
                this.preventAutoEquip = value;
                this.OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
