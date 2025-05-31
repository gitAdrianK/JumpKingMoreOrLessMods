namespace LessNpcDialog
{
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    public class Preferences : INotifyPropertyChanged
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

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
