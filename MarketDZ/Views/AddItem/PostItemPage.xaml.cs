using MarketDZ.ViewModels.AddItem; // Update namespace

namespace MarketDZ.Views.AddItem
{
    public partial class PostItemPage : ContentPage
    {
        public PostItemPage(AddItemViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }
}