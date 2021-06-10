using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace YMTEditor
{

    public partial class NewYMTWindow : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private string _gender = "mp_m_freemode_01_";

        private string _input;
        public string input
        {
            get { return _input; }
            set { _input = value; OnPropertyChanged("input"); fullName = _gender + input; }
        }

        private string _fullName;
        public string fullName
        {
            get { return _fullName; }
            set { _fullName = value; OnPropertyChanged("fullName"); }
        }

        public NewYMTWindow()
        {
            InitializeComponent();
            this.DataContext = this;

            fullName = _gender + input;
        }

        private void ButtonSubmit_Click(object sender, RoutedEventArgs e)
        {
            string userInput = fullName;

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

        private void ButtonHelp_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("1. Select for which MP character will your YMT work \n2. Type name of your choice, for example 'custom_clothes' \n3. Your full YMT is combined with gender and input" +
                " \n\nIf you are going to stream clothes to FiveM it should be prefixed with your full YMT name, for example: \n\n(IT IS ONLY EXAMPLE, DONT COPY IT, USE YOUR FULL YMT NAME)\n\nmp_m_freemode_01_custom_clothes^jbib_000_u.ydd\nmp_m_freemode_01_custom_clothes^jbib_diff_000_a_uni.ytd \netc...", "Help", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void radio_click(object sender, RoutedEventArgs e)
        {

            if(male.IsChecked == true)
            {
                _gender = "mp_m_freemode_01_";
            }
            else if(female.IsChecked == true)
            {
                _gender = "mp_f_freemode_01_";
            }

            fullName = _gender + input;
        }
    }
}
