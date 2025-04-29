using CommunityToolkit.Mvvm.Input;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Input;

namespace Market
{
    public partial class ProductDetailWindow : Window
    {
        public AutoPart Product { get; }
        public ICommand EditCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand CloseCommand { get; }

        public ProductDetailWindow(AutoPart product, Action<AutoPart> editAction, Action<string> deleteAction)
        {
            InitializeComponent();
            if (App.CurrentUser?.IsAdmin != true)
            {
                EditButton.Visibility = Visibility.Collapsed;
                DeleteButton.Visibility = Visibility.Collapsed;
            }
            Product = product;
            EditCommand = new RelayCommand(() => editAction?.Invoke(product));
            DeleteCommand = new RelayCommand(() =>
            {
                deleteAction?.Invoke(product.Id);
                Close();
            });
            CloseCommand = new RelayCommand(Close);
            DataContext = this;
        }
        private void LanguageButton_Click(object sender, RoutedEventArgs e)
        {
            var currentCulture = CultureInfo.CurrentUICulture.Name;
            var newCulture = currentCulture == "ru-RU" ? "en-US" : "ru-RU";
            LocalizationManager.SetLanguage(newCulture);
        }
    }
}