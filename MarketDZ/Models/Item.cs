using System.ComponentModel.DataAnnotations;
using MarketDZ.Converters;
using Newtonsoft.Json;

namespace MarketDZ.Models
{
    public class Item
    {
        public int Id { get; set; }

        public required string Title { get; set; }

        public required string Description { get; set; }
        public string? PhotoUrl { get; set; }

        [Range(0, 999999.99)]
        public required decimal Price { get; set; } = 0.00M;

        public DateTime ListedDate { get; set; } = DateTime.UtcNow;
        public required string Category { get; set; }
        public string? JobType { get; set; }
        public string? ServiceType { get; set; }
        public string? RentalPeriod { get; set; }
        public DateTime? AvailableFrom { get; set; }
        public DateTime? AvailableTo { get; set; }

        // Job-specific properties
        public JobCategory? JobCategory { get; set; }
        public string? CompanyName { get; set; }
        public string? JobLocation { get; set; }
        public ApplyMethod? ApplyMethod { get; set; }
        public string? ApplyContact { get; set; }

        // Service-specific properties
        public ServiceCategory? ServiceCategory { get; set; }
        public ServiceAvailability? ServiceAvailability { get; set; }
        public int? YearsOfExperience { get; set; }
        public int? NumberOfEmployees { get; set; }
        public string? ServiceLocation { get; set; }
        public double? AverageRating { get; set; }

        // Location properties
        public AlState? State { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }

        // Category-specific properties
        public ForSaleCategory? ForSaleCategory { get; set; }
        public ForRentCategory? ForRentCategory { get; set; }

        // User relationship
        [JsonProperty("postedByUserId")]
        public int PostedByUserId { get; set; }

        [JsonProperty("postedByUser")]
        public required User PostedByUser { get; set; }

        // Status and collections
        public ItemStatus Status { get; set; } = ItemStatus.Active;
        public ICollection<User> FavoritedByUsers { get; set; } = [];
        public ICollection<ItemPhoto> Photos { get; set; } = [];

        // Navigation property for location
        public ItemLocation? ItemLocation { get; set; }

        // Helper property to check if item has a location
        public bool HasLocation => ItemLocation != null;

        // Additional properties
        public string? ImageUrl { get; set; }
        public string? ImagePath { get; set; }
    }
}