using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using System.ComponentModel;
using System.Collections.Generic;
using System;
using System.Windows;
using Market.Windows;
using System.Globalization;
using System.Diagnostics;
using System.Windows.Data;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Market
{
    public class BoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (value is bool && (bool)value) ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private readonly AppDbContext _dbContext;
        private ObservableCollection<AutoPart> _autoParts = new ObservableCollection<AutoPart>();
        public ObservableCollection<AutoPart> AutoParts
        {
            get => _autoParts;
            set
            {
                _autoParts = value;
                OnPropertyChanged(nameof(AutoParts));
                Application.Current.Dispatcher.Invoke(() =>
                {
                    var window = Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();
                    window?.RefreshItems();
                });
            }
        }

        private List<AutoPart> _allParts;
        public List<string> AllColors => _allParts.SelectMany(p => p.Colors).Distinct().ToList();
        public List<string> AllSizes => _allParts.SelectMany(p => p.Sizes).Distinct().ToList();

        private string _searchText;
        public string SearchText
        {
            get => _searchText;
            set
            {
                if (_searchText != value)
                {
                    _searchText = value;
                    OnPropertyChanged(nameof(SearchText));
                    ApplyFilters();
                }
            }
        }

        private string _selectedColor;
        public string SelectedColor
        {
            get => _selectedColor;
            set
            {
                if (_selectedColor != value)
                {
                    _selectedColor = value;
                    OnPropertyChanged(nameof(SelectedColor));
                    ApplyFilters();
                }
            }
        }

        private string _selectedSize;
        public string SelectedSize
        {
            get => _selectedSize;
            set
            {
                if (_selectedSize != value)
                {
                    _selectedSize = value;
                    OnPropertyChanged(nameof(SelectedSize));
                    ApplyFilters();
                }
            }
        }

        private string _selectedCategory;
        public string SelectedCategory
        {
            get => _selectedCategory;
            set
            {
                _selectedCategory = value;
                OnPropertyChanged(nameof(SelectedCategory));
                ApplyFilters();
            }
        }

        public List<string> Categories { get; set; }
        public ICommand OpenProductCommand { get; }
        public ICommand ApplyFiltersCommand { get; }
        public ICommand ResetFiltersCommand { get; }
        public ICommand AddProductCommand { get; }
        public ICommand EditProductCommand { get; }
        public ICommand DeleteProductCommand { get; }

        public event PropertyChangedEventHandler PropertyChanged;

        public MainWindowViewModel()
        {
            _dbContext = new AppDbContext();
            OpenProductCommand = new RelayCommand<string>(OpenProduct);
            ApplyFiltersCommand = new RelayCommand(ApplyFilters);
            ResetFiltersCommand = new RelayCommand(ResetFilters);
            AddProductCommand = new RelayCommand(AddProduct);
            EditProductCommand = new RelayCommand<AutoPart>(EditProduct);
            DeleteProductCommand = new RelayCommand<string>(DeleteProduct);
            LoadAutoPartsAsync().ConfigureAwait(false);

            PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(SearchText) ||
                    e.PropertyName == nameof(SelectedColor) ||
                    e.PropertyName == nameof(SelectedSize))
                {
                    ApplyFilters();
                }
            };
        }

        private async Task LoadAutoPartsAsync()
        {
            try
            {
                _allParts = await _dbContext.AutoParts
                    .Include(p => p.Colors)
                    .Include(p => p.Sizes)
                    .Include(p => p.RelatedProducts)
                    .ToListAsync();

                AutoParts = new ObservableCollection<AutoPart>(_allParts);
                Categories = _allParts
                    .Select(p => p.Category)
                    .Distinct()
                    .OrderBy(c => c)
                    .ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}");
                AutoParts = new ObservableCollection<AutoPart>();
                Categories = new List<string>();
            }
        }

        private void ApplyFilters()
        {
            try
            {
                var mainWindow = Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();
                if (mainWindow == null) return;

                decimal? priceFrom = GetDecimalValue(mainWindow.PriceFromTextBox.Text);
                decimal? priceTo = GetDecimalValue(mainWindow.PriceToTextBox.Text);
                string selectedCategory = mainWindow.CategoryComboBox.SelectedItem?.ToString();
                bool inStockOnly = mainWindow.InStockCheckBox.IsChecked ?? false;

                var filtered = _allParts
                    .Where(p => FilterBySearchText(p))
                    .Where(p => FilterByCategory(p, selectedCategory))
                    .Where(p => FilterByPrice(p, priceFrom, priceTo))
                    .Where(p => FilterByAvailability(p, inStockOnly))
                    .Where(p => FilterByColor(p, SelectedColor))
                    .Where(p => FilterBySize(p, SelectedSize))
                    .ToList();

                AutoParts.Clear();
                foreach (var item in filtered)
                {
                    AutoParts.Add(item);
                }

                mainWindow.PartsItemsControl.ItemsSource = null;
                mainWindow.PartsItemsControl.ItemsSource = AutoParts;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Ошибка фильтрации: {ex.Message}");
            }
        }

        private decimal? GetDecimalValue(string text)
        {
            if (decimal.TryParse(text, out var result))
                return result;
            return null;
        }

        private bool FilterBySearchText(AutoPart part)
        {
            if (string.IsNullOrWhiteSpace(SearchText)) return true;

            return part.Title.IndexOf(SearchText, StringComparison.OrdinalIgnoreCase) >= 0 ||
                   part.Description.IndexOf(SearchText, StringComparison.OrdinalIgnoreCase) >= 0 ||
                   (part.Marking != null && part.Marking.IndexOf(SearchText, StringComparison.OrdinalIgnoreCase) >= 0) ||
                   (part.Manufacturer != null && part.Manufacturer.IndexOf(SearchText, StringComparison.OrdinalIgnoreCase) >= 0);
        }

        private bool FilterByCategory(AutoPart part, string category)
        {
            return string.IsNullOrEmpty(category) ||
                   (part.Category != null && part.Category.Equals(category, StringComparison.OrdinalIgnoreCase));
        }

        private bool FilterByPrice(AutoPart part, decimal? priceFrom, decimal? priceTo)
        {
            return (!priceFrom.HasValue || part.Price >= priceFrom.Value) &&
                   (!priceTo.HasValue || part.Price <= priceTo.Value);
        }

        private bool FilterByAvailability(AutoPart part, bool inStockOnly)
        {
            return !inStockOnly || part.IsAvailable;
        }

        private bool FilterByColor(AutoPart part, string color)
        {
            return string.IsNullOrEmpty(color) ||
                   (part.Color != null && part.Color.Equals(color, StringComparison.OrdinalIgnoreCase)) ||
                   (part.Colors != null && part.Colors.Any(c => c.Equals(color, StringComparison.OrdinalIgnoreCase)));
        }

        private bool FilterBySize(AutoPart part, string size)
        {
            return string.IsNullOrEmpty(size) ||
                   (part.Size != null && part.Size.Equals(size, StringComparison.OrdinalIgnoreCase)) ||
                   (part.Sizes != null && part.Sizes.Any(s => s.Equals(size, StringComparison.OrdinalIgnoreCase)));
        }

        private void ResetFilters()
        {
            if (Application.Current.MainWindow is MainWindow mainWindow)
            {
                mainWindow.SearchTextBox.Text = "";
                mainWindow.CategoryComboBox.SelectedIndex = -1;
                mainWindow.PriceFromTextBox.Text = "";
                mainWindow.PriceToTextBox.Text = "";
                mainWindow.InStockCheckBox.IsChecked = true;

                AutoParts = new ObservableCollection<AutoPart>(_allParts);
            }
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private async Task SaveAutoPartsAsync(AutoPart part)
        {
            try
            {
                if (part.Id == null)
                {
                    part.Id = Guid.NewGuid().ToString();
                    await _dbContext.AutoParts.AddAsync(part);
                }
                else
                {
                    _dbContext.AutoParts.Update(part);
                }

                await _dbContext.SaveChangesAsync();
                await LoadAutoPartsAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения данных: {ex.Message}");
            }
        }

        public async void AddProduct()
        {
            if (App.CurrentUser?.IsAdmin != true)
            {
                MessageBox.Show("Недостаточно прав для выполнения этой операции");
                return;
            }

            var window = new AddProductWindow();
            if (window.ShowDialog() == true)
            {
                await SaveAutoPartsAsync(window.NewProduct);
                ApplyFilters();
            }
        }

        public async void EditProduct(AutoPart product)
        {
            if (App.CurrentUser?.IsAdmin != true)
            {
                MessageBox.Show("Недостаточно прав для выполнения этой операции");
                return;
            }

            var window = new AddProductWindow(product);
            if (window.ShowDialog() == true)
            {
                await SaveAutoPartsAsync(window.NewProduct);
                ApplyFilters();
            }
        }

        public async void DeleteProduct(string productId)
        {
            if (App.CurrentUser?.IsAdmin != true)
            {
                MessageBox.Show("Недостаточно прав для выполнения этой операции");
                return;
            }

            if (MessageBox.Show("Удалить этот товар?", "Подтверждение", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                var product = await _dbContext.AutoParts.FirstOrDefaultAsync(p => p.Id == productId);
                if (product != null)
                {
                    _dbContext.AutoParts.Remove(product);
                    await _dbContext.SaveChangesAsync();
                    await LoadAutoPartsAsync();
                    ApplyFilters();
                }
            }
        }

        private void OpenProduct(string productId)
        {
            App.NavigateToProductDetail(productId, this);
        }
    }
}