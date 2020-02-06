namespace CartService.Models 
{
    public class Promotion {

        public string ItemId { get; set; }
        public double PercentOff { get; set; }

        public Promotion() {
            
        }
        
        public Promotion(string itemId, double percentOff) {
            this.ItemId = itemId;
            this.PercentOff = percentOff;
        }
    }
}
