namespace Desktop.ViewModels;

public class MainViewModel(InvoiceInputViewModels invoiceViewModel) : BaseViewModel
{
    public InvoiceInputViewModels InvoiceCreationVM { get; } = invoiceViewModel;

    public async Task InitAsyncTask()
    {
        await InvoiceCreationVM.InitAsync();
    }
}