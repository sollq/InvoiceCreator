using System.Windows;
using Desktop.ViewModels;

namespace Desktop.Views;

public partial class ProductDialog : Window
{
    public ProductDialogViewModel ViewModel { get; }

    public ProductDialog(ProductDialogViewModel? viewModel = null)
    {
        InitializeComponent();
        ViewModel = viewModel ?? new ProductDialogViewModel();
        DataContext = ViewModel;
        ViewModel.PropertyChanged += ViewModel_PropertyChanged;
    }

    private void ViewModel_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(ViewModel.DialogResult) && ViewModel.DialogResult)
        {
            DialogResult = true;
            Close();
        }
        else if (e.PropertyName == nameof(ViewModel.IsCancelled) && ViewModel.IsCancelled)
        {
            DialogResult = false;
            Close();
        }
    }

    public static ProductDialogResult ShowDialog(Window owner, ProductDialogViewModel viewModel)
    {
        var dialog = new ProductDialog(viewModel) { Owner = owner };
        var result = dialog.ShowDialog();
        return new ProductDialogResult
        {
            IsOk = result == true && viewModel.DialogResult,
            Product = viewModel.DialogResult ? viewModel.ToProduct() : null
        };
    }
}

public class ProductDialogResult
{
    public bool IsOk { get; set; }
    public Core.Models.Product? Product { get; set; }
}