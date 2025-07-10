namespace LessAutoEquipping
{
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    public sealed class Preferences : INotifyPropertyChanged
    {
        private bool preventAutoEquip;

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

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
            =>
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
