using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Windows;

namespace YMTEditor
{
    public partial class RenameWindow : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private string _prefix;

        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private string _input;
        public string input
        {
            get { return _input; }
            set { _input = value; OnPropertyChanged("input"); NewfullName = _prefix + input; }
        }

        private string _CurrentfullName;
        public string CurrentfullName
        {
            get { return _CurrentfullName; }
            set { _CurrentfullName = value; OnPropertyChanged("CurrentfullName"); }
        }

        private string _NewfullName;
        public string NewfullName
        {
            get { return _NewfullName; }
            set { _NewfullName = value; OnPropertyChanged("NewfullName"); }
        }

        public RenameWindow(string _curFullName, string _curDlcName)
        {
            InitializeComponent();
            this.DataContext = this;

            CurrentfullName = _curFullName;
            input = _curDlcName;

            _prefix = _curFullName.Substring(0, _curFullName.Length - _curDlcName.Length);
            NewfullName = _prefix + input;

        }

        private void ButtonSubmit_Click(object sender, RoutedEventArgs e)
        {
            string userInput = NewfullName;

            if (input == null || input.Length < 1)
            {
                MessageBox.Show("Your input is empty!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            Regex r = new Regex("^[a-zA-Z_0-9]*$");
            if (r.IsMatch(userInput))
            {
                if (userInput.Length < 60)
                {
                    Hide();
                }
                else
                {
                    MessageBox.Show("Your input is too long!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Your input contains not allowed characters!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }
    }
}
