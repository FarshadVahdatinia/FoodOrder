namespace Core.Models
{
    public class Order
    {
        public int FoodId { get; set; }
        public string Name { get; set; }
        public string PersonId { get; set; }
        public DateTime OrderDate { get; set; } = DateTime.Now;
    }
}
