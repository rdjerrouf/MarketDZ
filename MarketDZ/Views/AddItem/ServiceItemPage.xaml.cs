using System.Diagnostics;
using MarketDZ.ViewModels.AddItem;

namespace MarketDZ.Views.AddItem;

public partial class ServiceItemPage : ContentPage
{
    public ServiceItemPage(ServiceItemViewModel viewModel)
    {
        try
        {
            Console.WriteLine("ServiceItemPage: Constructor called");
            Debug.WriteLine("ServiceItemPage: Constructor called");

            InitializeComponent();
            BindingContext = viewModel;

            Console.WriteLine("ServiceItemPage: Initialization complete");
            Debug.WriteLine("ServiceItemPage: Initialization complete");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"ServiceItemPage Constructor Error: {ex}");
            Debug.WriteLine($"ServiceItemPage Constructor Error: {ex}");
            throw;
        }
    }
}