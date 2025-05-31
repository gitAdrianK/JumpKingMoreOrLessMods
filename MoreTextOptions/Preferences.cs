namespace MoreTextOptions
{
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    public class Preferences : INotifyPropertyChanged
    {
        private bool isCustomTextColor = false;
        private int textRed = 255;
        private int textGreen = 255;
        private int textBlue = 255;
        private bool isOutlineDisabled = false;
        private bool isCustomOutline = false;
        private int outlineRed = 0;
        private int outlineGreen = 0;
        private int outlineBlue = 0;

        public bool IsCustomTextColor
        {
            get => this.isCustomTextColor;
            set
            {
                this.isCustomTextColor = value;
                this.OnPropertyChanged();
            }
        }

        public int TextRed
        {
            get => this.textRed;
            set
            {
                this.textRed = value;
                this.OnPropertyChanged();
            }
        }

        public int TextGreen
        {
            get => this.textGreen;
            set
            {
                this.textGreen = value;
                this.OnPropertyChanged();
            }
        }

        public int TextBlue
        {
            get => this.textBlue;
            set
            {
                this.textBlue = value;
                this.OnPropertyChanged();
            }
        }

        public bool IsOutlineDisabled
        {
            get => this.isOutlineDisabled;
            set
            {
                this.isOutlineDisabled = value;
                this.OnPropertyChanged();
            }
        }

        public bool IsCustomOutline
        {
            get => this.isCustomOutline;
            set
            {
                this.isCustomOutline = value;
                this.OnPropertyChanged();
            }
        }

        public int OutlineRed
        {
            get => this.outlineRed;
            set
            {
                this.outlineRed = value;
                this.OnPropertyChanged();
            }
        }

        public int OutlineGreen
        {
            get => this.outlineGreen;
            set
            {
                this.outlineGreen = value;
                this.OnPropertyChanged();
            }
        }

        public int OutlineBlue
        {
            get => this.outlineBlue;
            set
            {
                this.outlineBlue = value;
                this.OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
