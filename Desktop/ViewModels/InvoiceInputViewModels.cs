using System.Windows.Controls;
using System.Windows.Input;

namespace Desktop.ViewModels;

public class InvoiceInputViewModels
{
    public ICommand CreateInvoiceCommand { get; }
    public async Task InitAsync()
    {
        
    }
    private void DatePicker_SelectedDateChanged(object? sender, SelectionChangedEventArgs e)
    {
        throw new NotImplementedException();
    }

    private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        throw new NotImplementedException();
    }
}