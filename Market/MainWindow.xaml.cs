using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Market
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainWindowViewModel();
            Loaded += MainWindow_Loaded;
            this.Cursor = (Cursor)Application.Current.FindResource("DefaultCursor");
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is MainWindowViewModel viewModel)
            {
                PartsItemsControl.ItemsSource = viewModel.AutoParts;
            }
        }
        private void LanguageRu_Click(object sender, RoutedEventArgs e)
        {
            LocalizationManager.SetLanguage("ru-RU");
        }

        private void LanguageEn_Click(object sender, RoutedEventArgs e)
        {
            LocalizationManager.SetLanguage("en-US");
        }
        public void RefreshItems()
        {
            PartsItemsControl.ItemsSource = null;
            PartsItemsControl.ItemsSource = (DataContext as MainWindowViewModel)?.AutoParts;
        }

    }
}