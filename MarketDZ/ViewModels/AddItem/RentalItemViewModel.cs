using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MarketDZ.Services;
using MarketDZ.Models;
using System.Diagnostics;
using System.Text;

namespace MarketDZ.ViewModels.AddItem
{
    /// <summary>
    /// ViewModel for managing rental items.
    /// </summary>
    public partial class RentalItemViewModel : ObservableObject
    {
        #region Fields and Constants

        private readonly IItemService _itemService;
        private readonly IAuthService _authService;

        // Validation constants
        private const int TITLE_MIN_LENGTH = 2;
        private const int DESCRIPTION_MIN_LENGTH = 4;
        private const decimal MIN_PRICE = 0.01m;
        private const decimal MAX_PRICE = 999999.99m;
        private const int MAX_PHOTOS = 3;
        private const int MIN_PHOTOS = 1;
        private const long MAX_PHOTO_SIZE = 5 * 1024 * 1024; // 5MB

        // List of available rental periods
        public List<string> RentalPeriods { get; } = new()
        {
            "per Day",
            "per Week",
            "per Month"
        };

        // Collection to store selected photo paths
        private List<string> _selectedPhotos = new();

        #endregion

        #region Observable Properties

        private string _title = string.Empty;
        public string Title
        {
            get => _title;
            set
            {
                if (SetProperty(ref _title, value))
                {
                    // Only validate if more than one character or empty
                    if (string.IsNullOrEmpty(value) || value.Length > 1)
                    {
                        ValidateTitle();
                        OnPropertyChanged(nameof(CanSave));
                        SaveCommand.NotifyCanExecuteChanged();
                    }
                }
            }
        }

        private string _description = string.Empty;
        public string Description
        {
            get => _description;
            set
            {
                Debug.WriteLine($"Description setter called with value: '{value}'");
                if (SetProperty(ref _description, value))
                {
                    Debug.WriteLine($"Description changed to: '{_description}'");
                    ValidateDescription();
                    OnPropertyChanged(nameof(CanSave));
                    SaveCommand.NotifyCanExecuteChanged();
                }
            }
        }

        // String-based category properties for the picker
        public List<string> CategoryNames => Enum.GetNames<ForRentCategory>().ToList();

        private string _selectedCategoryName = string.Empty;
        public string SelectedCategoryName
        {
            get => _selectedCategoryName;
            set
            {
                if (SetProperty(ref _selectedCategoryName, value) && !string.IsNullOrEmpty(value))
                {
                    // Convert string to enum
                    if (Enum.TryParse<ForRentCategory>(value, out var category))
                    {
                        SelectedCategory = category;
                        Debug.WriteLine($"Category set from string to enum: {category}");
                    }
                }
            }
        }

        private ForRentCategory? _selectedCategory;
        public ForRentCategory? SelectedCategory
        {
            get => _selectedCategory;
            set
            {
                Debug.WriteLine($"Setting SelectedCategory: {value}");
                if (SetProperty(ref _selectedCategory, value))
                {
                    Debug.WriteLine($"SelectedCategory changed to: {_selectedCategory}");
                    // Update the string representation
                    if (value.HasValue && _selectedCategoryName != value.ToString())
                    {
                        _selectedCategoryName = value?.ToString() ?? string.Empty;
                        OnPropertyChanged(nameof(SelectedCategoryName));
                    }
                    ValidateCategory();
                    OnPropertyChanged(nameof(CanSave));
                }
            }
        }

        private string? _categoryError;
        public string? CategoryError
        {
            get => _categoryError;
            set => SetProperty(ref _categoryError, value);
        }

        public bool HasCategoryError => !string.IsNullOrEmpty(CategoryError);

        // String-based state properties for the picker
        public List<string> StateNames => Enum.GetNames<AlState>().ToList();

        private string _selectedStateName = string.Empty;
        public string SelectedStateName
        {
            get => _selectedStateName;
            set
            {
                if (SetProperty(ref _selectedStateName, value) && !string.IsNullOrEmpty(value))
                {
                    // Convert string to enum
                    if (Enum.TryParse<AlState>(value, out var state))
                    {
                        SelectedState = state;
                        Debug.WriteLine($"State set from string to enum: {state}");
                    }
                }
            }
        }

        private AlState? _selectedState;
        public AlState? SelectedState
        {
            get => _selectedState;
            set
            {
                Debug.WriteLine($"Setting SelectedState: {value}");
                if (SetProperty(ref _selectedState, value))
                {
                    Debug.WriteLine($"SelectedState changed to: {_selectedState}");
                    // Update the string representation
                    if (value.HasValue && _selectedStateName != value.ToString())
                    {
                        _selectedStateName = value?.ToString() ?? string.Empty;
                        OnPropertyChanged(nameof(SelectedStateName));
                    }
                    ValidateState();
                    OnPropertyChanged(nameof(CanSave));
                }
            }
        }

        private string? _stateError;
        public string? StateError
        {
            get => _stateError;
            set => SetProperty(ref _stateError, value);
        }

        private decimal _price;
        public decimal Price
        {
            get => _price;
            set
            {
                if (SetProperty(ref _price, value))
                {
                    ValidatePrice();
                    OnPropertyChanged(nameof(CanSave));
                    SaveCommand.NotifyCanExecuteChanged();
                }
            }
        }

        private string _rentalPeriod = string.Empty;
        public string RentalPeriod
        {
            get => _rentalPeriod;
            set
            {
                SetProperty(ref _rentalPeriod, value);
            }
        }

        private DateTime _availableFrom = DateTime.Today;
        public DateTime AvailableFrom
        {
            get => _availableFrom;
            set
            {
                if (SetProperty(ref _availableFrom, value))
                {
                    ValidateDates();
                    OnPropertyChanged(nameof(CanSave));
                }
            }
        }

        private DateTime _availableTo = DateTime.Today;
        public DateTime AvailableTo
        {
            get => _availableTo;
            set
            {
                if (SetProperty(ref _availableTo, value))
                {
                    ValidateDates();
                    OnPropertyChanged(nameof(CanSave));
                }
            }
        }

        public List<string> SelectedPhotos
        {
            get => _selectedPhotos;
            set
            {
                if (SetProperty(ref _selectedPhotos, value))
                {
                    ValidatePhotos();
                    OnPropertyChanged(nameof(CanSave));
                    OnPropertyChanged(nameof(PhotoCount));
                    OnPropertyChanged(nameof(CanAddPhoto));
                }
            }
        }

        public int PhotoCount => SelectedPhotos.Count;
        public bool CanAddPhoto => PhotoCount < MAX_PHOTOS;

        #region Error Properties

        private string? _titleError;
        public string? TitleError
        {
            get => _titleError;
            set => SetProperty(ref _titleError, value);
        }

        public bool HasStateError => !string.IsNullOrEmpty(StateError);
        public bool HasTitleError => !string.IsNullOrEmpty(TitleError);

        private string? _descriptionError;
        public string? DescriptionError
        {
            get => _descriptionError;
            set
            {
                if (SetProperty(ref _descriptionError, value))
                {
                    OnPropertyChanged(nameof(HasDescriptionError));
                    OnPropertyChanged(nameof(CanSave));
                }
            }
        }

        public bool HasDescriptionError => !string.IsNullOrEmpty(DescriptionError);

        private string? _priceError;
        public string? PriceError
        {
            get => _priceError;
            set => SetProperty(ref _priceError, value);
        }

        public bool HasPriceError => !string.IsNullOrEmpty(PriceError);

        private string? _dateError;
        public string? DateError
        {
            get => _dateError;
            set => SetProperty(ref _dateError, value);
        }

        public bool HasDateError => !string.IsNullOrEmpty(DateError);

        private string? _photoError;
        public string? PhotoError
        {
            get => _photoError;
            set => SetProperty(ref _photoError, value);
        }

        public bool HasPhotoError => !string.IsNullOrEmpty(PhotoError);

        private bool _isBusy;
        public bool IsBusy
        {
            get => _isBusy;
            set => SetProperty(ref _isBusy, value);
        }

        #endregion

        #region Computed Properties

        public bool CanSave
        {
            get
            {
                bool isBusy = IsBusy;
                var canSave = !isBusy &&
                     !HasTitleError &&
                     !HasDescriptionError &&
                     !HasPriceError &&
                     !HasDateError &&
                     !HasPhotoError &&
                     !HasStateError &&
                     !HasCategoryError &&
                     SelectedState.HasValue &&
                     SelectedCategory.HasValue &&
                     PhotoCount >= MIN_PHOTOS;

                Debug.WriteLine("CanSave evaluation:");
                Debug.WriteLine($"- IsBusy: {IsBusy}");
                Debug.WriteLine($"- HasTitleError: {HasTitleError}");
                Debug.WriteLine($"- HasDescriptionError: {HasDescriptionError}");
                Debug.WriteLine($"- HasPriceError: {HasPriceError}");
                Debug.WriteLine($"- HasDateError: {HasDateError}");
                Debug.WriteLine($"- HasPhotoError: {HasPhotoError}");
                Debug.WriteLine($"- HasStateError: {HasStateError}");
                Debug.WriteLine($"- HasCategoryError: {HasCategoryError}");
                Debug.WriteLine($"- PhotoCount: {PhotoCount} (minimum: {MIN_PHOTOS})");
                Debug.WriteLine($"- SelectedState: {SelectedState}");
                Debug.WriteLine($"- SelectedCategory: {SelectedCategory}");
                Debug.WriteLine($"Final CanSave value: {canSave}");

                return canSave;
            }
        }

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="RentalItemViewModel"/> class.
        /// </summary>
        /// <param name="itemService">The item service.</param>
        /// <param name="authService">The authentication service.</param>
        public RentalItemViewModel(IItemService itemService, IAuthService authService)
        {
            try
            {
                Debug.WriteLine("Starting RentalItemViewModel initialization");

                _itemService = itemService ?? throw new ArgumentNullException(nameof(itemService));
                _authService = authService ?? throw new ArgumentNullException(nameof(authService));

                if (RentalPeriods.Count > 0)
                {
                    RentalPeriod = RentalPeriods[0];
                }

                // Perform initial validation
                ValidateTitle();
                ValidateDescription();
                ValidatePrice();
                ValidateDates();
                ValidatePhotos();
                ValidateState();
                ValidateCategory();

                Debug.WriteLine("RentalItemViewModel initialization completed");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in RentalItemViewModel constructor: {ex}");
                throw;
            }
        }

        #region Validation Methods

        private bool ValidateCategory()
        {
            if (!SelectedCategory.HasValue)
            {
                CategoryError = "Please select a category";
                return false;
            }

            CategoryError = null;
            Debug.WriteLine("Category validated");
            return true;
        }

        private bool ValidateState()
        {
            if (!SelectedState.HasValue)
            {
                StateError = "Please select a state";
                return false;
            }

            StateError = null;
            Debug.WriteLine("State validated");
            return true;
        }

        private bool ValidateTitle()
        {
            if (string.IsNullOrWhiteSpace(Title))
            {
                TitleError = "Please enter an item title";
                return false;
            }

            if (Title.Length < TITLE_MIN_LENGTH)
            {
                TitleError = $"Title must be at least {TITLE_MIN_LENGTH} characters";
                return false;
            }

            TitleError = null;
            return true;
        }

        private bool ValidateDescription()
        {
            Debug.WriteLine($"Validating description. Current value: '{Description}'");
            Debug.WriteLine($"Description length before trim: {Description?.Length ?? 0}");
            Debug.WriteLine($"Description length after trim: {Description?.Trim().Length ?? 0}");

            if (string.IsNullOrWhiteSpace(Description))
            {
                DescriptionError = "Please enter an item description";
                Debug.WriteLine("Validation failed: Description is empty or whitespace");
                return false;
            }

            var trimmedLength = Description.Trim().Length;
            if (trimmedLength < DESCRIPTION_MIN_LENGTH)
            {
                DescriptionError = $"Description must be at least {DESCRIPTION_MIN_LENGTH} characters";
                Debug.WriteLine($"Validation failed: Description length {trimmedLength} is less than minimum {DESCRIPTION_MIN_LENGTH}");
                return false;
            }

            DescriptionError = null;
            Debug.WriteLine("Description validation passed");
            return true;
        }

        private bool ValidatePrice()
        {
            Debug.WriteLine($"Validating price: {Price}");

            if (Price < MIN_PRICE)
            {
                Debug.WriteLine($"Price {Price} is below minimum {MIN_PRICE}");
                PriceError = "Price must be greater than zero";
                return false;
            }

            if (Price > MAX_PRICE)
            {
                Debug.WriteLine($"Price {Price} exceeds maximum {MAX_PRICE}");
                PriceError = $"Price cannot exceed {MAX_PRICE:C}";
                return false;
            }

            Debug.WriteLine("Price validation passed");
            PriceError = null;
            return true;
        }

        private bool ValidateDates()
        {
            if (AvailableFrom < DateTime.Today)
            {
                DateError = "Start date cannot be in the past";
                return false;
            }

            if (AvailableTo < AvailableFrom)
            {
                DateError = "End date must be after start date";
                return false;
            }

            DateError = null;
            return true;
        }

        private bool ValidatePhotos()
        {
            if (PhotoCount < MIN_PHOTOS)
            {
                PhotoError = $"Please add at least {MIN_PHOTOS} photo";
                return false;
            }

            if (PhotoCount > MAX_PHOTOS)
            {
                PhotoError = $"Maximum {MAX_PHOTOS} photos allowed";
                return false;
            }

            PhotoError = null;
            return true;
        }

        private static async Task<bool> ValidatePhoto(FileResult photo)
        {
            try
            {
                using var stream = await photo.OpenReadAsync();
                if (stream.Length > MAX_PHOTO_SIZE)
                {
                    Debug.WriteLine($"Photo size ({stream.Length / 1024 / 1024}MB) exceeds limit of {MAX_PHOTO_SIZE / 1024 / 1024}MB");
                    await Shell.Current.DisplayAlert("Error", "Photo must be less than 5MB", "OK");
                    return false;
                }

                var extension = Path.GetExtension(photo.FileName).ToLowerInvariant();
                if (extension != ".jpg" && extension != ".jpeg" && extension != ".png")
                {
                    Debug.WriteLine($"Invalid file extension: {extension}");
                    await Shell.Current.DisplayAlert("Error", "Please select a JPG or PNG file", "OK");
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Photo validation error: {ex.Message}");
                await Shell.Current.DisplayAlert("Error", "Failed to validate photo", "OK");
                return false;
            }
        }

        private bool ValidateAll()
        {
            Debug.WriteLine("Running ValidateAll");

            var titleValid = ValidateTitle();
            Debug.WriteLine($"- Title validation: {titleValid}");

            var descriptionValid = ValidateDescription();
            Debug.WriteLine($"- Description validation: {descriptionValid}");

            var priceValid = ValidatePrice();
            Debug.WriteLine($"- Price validation: {priceValid}");

            var datesValid = ValidateDates();
            Debug.WriteLine($"- Dates validation: {datesValid}");

            var photosValid = ValidatePhotos();
            Debug.WriteLine($"- Photos validation: {photosValid}");

            var stateValid = ValidateState();
            Debug.WriteLine($"- State validation: {stateValid}");

            var categoryValid = ValidateCategory();
            Debug.WriteLine($"- Category validation: {categoryValid}");

            var allValid = titleValid &&
                           descriptionValid &&
                           priceValid &&
                           datesValid &&
                           photosValid &&
                           categoryValid &&
                           stateValid;
            Debug.WriteLine($"ValidateAll result: {allValid}");

            return allValid;
        }

        #endregion

        #region Commands

        [RelayCommand]
        public async Task AddPhoto()
        {
            if (IsBusy) return;
            try
            {
                IsBusy = true;
                Debug.WriteLine("Starting photo upload process");

                var result = await MediaPicker.PickPhotoAsync(new MediaPickerOptions
                {
                    Title = "Select a photo"
                });

                if (result == null)
                {
                    Debug.WriteLine("No photo selected");
                    return;
                }

                Debug.WriteLine($"Photo selected: {result.FileName}");

                if (!await ValidatePhoto(result))
                {
                    Debug.WriteLine("Photo validation failed");
                    return;
                }

                var tempPhotoPath = await SavePhoto(result);
                if (tempPhotoPath != null)
                {
                    // Store the photo in permanent storage
                    var permanentPhotoPath = await SavePhotoToPermanentStorage(tempPhotoPath);

                    var newPhotos = new List<string>(SelectedPhotos) { permanentPhotoPath };
                    SelectedPhotos = newPhotos;
                    Debug.WriteLine($"Photo saved permanently at: {permanentPhotoPath}");
                    await Shell.Current.DisplayAlert("Success", "Photo uploaded successfully", "OK");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Photo upload error: {ex.Message}");
                Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                await Shell.Current.DisplayAlert("Error",
                    "Failed to upload photo. Please try again.", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private void RemovePhoto(string photo)
        {
            if (SelectedPhotos.Contains(photo))
            {
                var newList = new List<string>(SelectedPhotos);
                newList.Remove(photo);
                SelectedPhotos = newList;
                Debug.WriteLine($"Photo removed: {photo}");
            }
        }

        private async Task<string> SavePhotoToPermanentStorage(string tempPath)
        {
            try
            {
                var fileName = Path.GetFileName(tempPath);
                var appDataDir = FileSystem.AppDataDirectory;
                var permanentDir = Path.Combine(appDataDir, "Photos");

                if (!Directory.Exists(permanentDir))
                    Directory.CreateDirectory(permanentDir);

                var permanentPath = Path.Combine(permanentDir, fileName);

                // Copy the file to permanent storage asynchronously
                using (var sourceStream = File.OpenRead(tempPath))
                using (var destStream = File.Create(permanentPath))
                {
                    await sourceStream.CopyToAsync(destStream);
                }

                return permanentPath;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error saving photo to permanent storage: {ex.Message}");
                return tempPath; // Fall back to the temporary path
            }
        }
        private static async Task<string?> SavePhoto(FileResult photo)
        {
            try
            {
                var photosDir = Path.Combine(FileSystem.CacheDirectory, "Photos");
                if (!Directory.Exists(photosDir))
                    Directory.CreateDirectory(photosDir);

                var fileName = $"rental_photo_{Guid.NewGuid()}{Path.GetExtension(photo.FileName)}";
                var localPath = Path.Combine(photosDir, fileName);

                Debug.WriteLine($"Saving photo to: {localPath}");

                using (var sourceStream = await photo.OpenReadAsync())
                using (var destinationStream = File.Create(localPath))
                {
                    await sourceStream.CopyToAsync(destinationStream);
                }

                Debug.WriteLine("Photo saved successfully");
                return localPath;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Photo save error: {ex.Message}");
                Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                throw;
            }
        }
        [RelayCommand(CanExecute = nameof(CanSave))]
        private async Task Save()
        {
            if (IsBusy || !CanSave) return;

            try
            {
                IsBusy = true;
                Debug.WriteLine("Starting rental listing save process");

                // Get current user first
                var user = await _authService.GetCurrentUserAsync();
                if (user == null)
                {
                    await Shell.Current.DisplayAlert("Error", "Please sign in to post rentals", "OK");
                    await Shell.Current.GoToAsync("///SignInPage");
                    return;
                }

                if (!ValidateAll()) return;

                var rentalItem = new Item
                {
                    Title = Title,
                    Description = BuildFullDescription(),
                    Price = Price,
                    Category = "For Rent",
                    RentalPeriod = RentalPeriod,
                    AvailableFrom = AvailableFrom,
                    AvailableTo = AvailableTo,
                    ListedDate = DateTime.UtcNow,
                    PostedByUserId = user.Id,
                    PostedByUser = user,
                    ForRentCategory = SelectedCategory,
                    State = SelectedState ?? throw new InvalidOperationException("State must be selected")
                };

                // Save the item
                var result = await _itemService.AddItemAsync(rentalItem);

                if (result)
                {
                    // Now add photos one by one
                    bool allPhotosAdded = true;

                    foreach (var photoPath in SelectedPhotos)
                    {
                        var photoAdded = await _itemService.AddItemPhotoAsync(user.Id, rentalItem.Id, photoPath);
                        if (!photoAdded)
                        {
                            Debug.WriteLine($"Failed to add photo: {photoPath}");
                            allPhotosAdded = false;
                        }
                        else
                        {
                            Debug.WriteLine($"Successfully added photo: {photoPath}");
                        }
                    }

                    if (!allPhotosAdded)
                    {
                        await Shell.Current.DisplayAlert("Warning", "Your rental has been posted, but some photos couldn't be added.", "OK");
                    }
                    else
                    {
                        await Shell.Current.DisplayAlert("Success", "Your rental has been posted!", "OK");
                    }

                    await Shell.Current.GoToAsync("//MainPage");
                }
                else
                {
                    await Shell.Current.DisplayAlert("Error", "Failed to post rental", "OK");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Save error: {ex.Message}");
                await Shell.Current.DisplayAlert("Error", "An error occurred while saving", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        #endregion

        #region Helper Methods

        private string BuildFullDescription()
        {
            var fullDescription = new StringBuilder();
            fullDescription.AppendLine(Description);
            fullDescription.AppendLine();
            fullDescription.AppendLine($"Category: {SelectedCategory?.ToString() ?? "Not specified"}");
            fullDescription.AppendLine($"Location: {SelectedState?.ToString() ?? "Not specified"}");
            fullDescription.AppendLine($"Available From: {AvailableFrom:d}");
            fullDescription.AppendLine($"Available To: {AvailableTo:d}");
            fullDescription.AppendLine($"Rate: {Price:C} {RentalPeriod}");

            return fullDescription.ToString();
        }

        #endregion
    }
}
#endregion
