namespace MarketDZ.Models
{
    public class UserProfileStatistics
    {
        public int UserId { get; set; }
        public int PostedItemsCount { get; set; }
        public int FavoriteItemsCount { get; set; }
        public double AverageRating { get; set; }
    }
}
