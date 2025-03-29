using MarketDZ.ViewModels.AddItem;

namespace MarketDZ.Views.AddItem
{
    public partial class AddItemPage : ContentPage
    {
        public AddItemPage(AddItemViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }

        // If you don't want to use dependency injection:
        /*
        public AddItemPage()
        {
            InitializeComponent();
            BindingContext = new AddItemViewModel();
        }
        */
    }
}