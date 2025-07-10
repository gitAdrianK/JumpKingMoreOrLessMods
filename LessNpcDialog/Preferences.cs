namespace LessNpcDialog
{
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    public sealed class Preferences : INotifyPropertyChanged
    {
        private bool isEnabled = true;

        public bool IsEnabled
        {
            get => this.isEnabled;
            set
            {
                this.isEnabled = value;
                this.OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
            => this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
