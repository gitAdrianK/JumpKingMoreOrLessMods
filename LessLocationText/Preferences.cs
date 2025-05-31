namespace LessLocationText
{
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    public class Preferences : INotifyPropertyChanged
    {
        private bool shouldHideDiscover = false;
        private bool shouldHideEnter = false;

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

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
