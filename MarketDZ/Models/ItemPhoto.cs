namespace MarketDZ.Models
{
    public class ItemPhoto
    {
        public int Id { get; set; }
        public int ItemId { get; set; }
        public required Item Item { get; set; }
        public required string PhotoUrl { get; set; }
        public bool IsPrimaryPhoto { get; set; }
        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
        public int DisplayOrder { get; set; }
    }
}
