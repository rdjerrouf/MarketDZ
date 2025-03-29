using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MarketDZ.Models;
using MarketDZ.Services;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;

namespace MarketDZ.ViewModels
{
    [QueryProperty(nameof(ItemId), "ItemId")]
    public partial class ReportItemViewModel : ObservableObject
    {
        private readonly FirebaseSecurityService _securityService;
        private readonly IAuthService _authService;

        private int _itemId;
        public int ItemId
        {
            get => _itemId;
            set => SetProperty(ref _itemId, value);
        }

        private string _title = "Report Item";
        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        private string _selectedReason = string.Empty;
        public string SelectedReason
        {
            get => _selectedReason;
            set => SetProperty(ref _selectedReason, value);
        }

        private string _additionalComments = string.Empty;
        public string AdditionalComments
        {
            get => _additionalComments;
            set => SetProperty(ref _additionalComments, value);
        }

        private ObservableCollection<string> _reportReasons = new ObservableCollection<string>();
        public ObservableCollection<string> ReportReasons
        {
            get => _reportReasons;
            set => SetProperty(ref _reportReasons, value);
        }

        private bool _isBusy;
        public bool IsBusy
        {
            get => _isBusy;
            set => SetProperty(ref _isBusy, value);
        }

        private bool _hasAlreadyReported;
        public bool HasAlreadyReported
        {
            get => _hasAlreadyReported;
            set => SetProperty(ref _hasAlreadyReported, value);
        }

        public ReportItemViewModel(FirebaseSecurityService securityService, IAuthService authService)
        {
            _securityService = securityService;
            _authService = authService;

            ReportReasons = new ObservableCollection<string>
    {
        "Prohibited content",
        "Incorrect category",
        "Scam or fraud",
        "Offensive content",
        "Duplicate listing",
        "Copyright violation",
        "Other"
    };
        }
        private async Task CheckIfAlreadyReportedAsync()
        {
            if (ItemId <= 0)
                return;

            try
            {
                IsBusy = true;
                // Get current user ID
                var currentUser = await _authService.GetCurrentUserAsync();
                if (currentUser == null)
                {
                    await Shell.Current.DisplayAlert("Error", "You must be logged in to report items", "OK");
                    await Shell.Current.GoToAsync("..");
                    return;
                }

                HasAlreadyReported = await _securityService.HasUserReportedItemAsync(currentUser.Id, ItemId);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error checking report status: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task SubmitReport()
        {
            if (ItemId <= 0 || string.IsNullOrWhiteSpace(SelectedReason))
            {
                await Shell.Current.DisplayAlert("Error", "Please select a reason for reporting", "OK");
                return;
            }

            try
            {
                IsBusy = true;

                // Get current user ID
                var currentUser = await _authService.GetCurrentUserAsync();
                if (currentUser == null)
                {
                    await Shell.Current.DisplayAlert("Error", "You must be logged in to report items", "OK");
                    return;
                }

                await _securityService.ReportItemAsync(
                    ItemId,
                    currentUser.Id,
                    SelectedReason,
                    AdditionalComments);

                await Shell.Current.DisplayAlert(
                    "Thank You",
                    "Your report has been submitted and will be reviewed by our team.",
                    "OK");

                await Shell.Current.GoToAsync("..");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error submitting report: {ex.Message}");
                await Shell.Current.DisplayAlert("Error", "Failed to submit report. Please try again.", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task Cancel()
        {
            await Shell.Current.GoToAsync("..");
        }

        public async Task InitializeAsync()
        {
            await CheckIfAlreadyReportedAsync();
        }
    }
}