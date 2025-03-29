// Views/ReportItemPage.xaml.cs
using MarketDZ.ViewModels;

namespace MarketDZ.Views
{
    public partial class ReportItemPage : ContentPage
    {
        private readonly ReportItemViewModel _viewModel;

        public ReportItemPage(ReportItemViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            BindingContext = _viewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await _viewModel.InitializeAsync();
        }
    }
}