namespace LessLocationText
{
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    public sealed class Preferences : INotifyPropertyChanged
    {
        private bool shouldHideDiscover;
        private bool shouldHideEnter;

        public bool ShouldHideDiscover
        {
            get => this.shouldHideDiscover;
            set
            {
                this.shouldHideDiscover = value;
                this.OnPropertyChanged();
            }
        }

        public bool ShouldHideEnter
        {
            get => this.shouldHideEnter;
            set
            {
                this.shouldHideEnter = value;
                this.OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
            =>
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
