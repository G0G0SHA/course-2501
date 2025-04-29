using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using Market;

namespace Market.Windows
{
    public partial class AddProductWindow : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public AutoPart NewProduct { get; private set; }

        private TextBox ColorTextBox;
        private TextBox SizeTextBox;

        private void InitializeComponent()
        {
            // Пустой метод для обхода ошибки компиляции
        }

        public AddProductWindow()
        {
            InitializeComponent();
            NewProduct = new AutoPart();
            DataContext = this;
        }
        
        public AddProductWindow(AutoPart productToEdit)
        {
            InitializeComponent();
            NewProduct = productToEdit;
            DataContext = this;
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void LanguageButton_Click(object sender, RoutedEventArgs e)
        {
            var currentCulture = CultureInfo.CurrentUICulture.Name;
            var newCulture = currentCulture == "ru-RU" ? "en-US" : "ru-RU";
            LocalizationManager.SetLanguage(newCulture);
        }

        private void AddImage_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "Image files (*.jpg, *.png)|*.jpg;*.png",
                Multiselect = true
            };

            if (openFileDialog.ShowDialog() == true)
            {
                foreach (var filename in openFileDialog.FileNames)
                {
                    var destPath = System.IO.Path.Combine("Images", System.IO.Path.GetFileName(filename));
                    System.IO.File.Copy(filename, destPath, true);
                    NewProduct.ImagePaths.Add(destPath);
                }
                OnPropertyChanged(nameof(NewProduct));
            }
        }

        private void AddColor_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(ColorTextBox.Text))
            {
                NewProduct.Colors.Add(ColorTextBox.Text);
                ColorTextBox.Clear();
                OnPropertyChanged(nameof(NewProduct));
            }
        }

        private void AddSize_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(SizeTextBox.Text))
            {
                NewProduct.Sizes.Add(SizeTextBox.Text);
                SizeTextBox.Clear();
                OnPropertyChanged(nameof(NewProduct));
            }
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}