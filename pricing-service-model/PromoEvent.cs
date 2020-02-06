using System;

namespace PricingServiceModel
{
    public class PromoEvent
    {
        // ? @org.kie.api.definition.type.Position(0)
        public string ItemId { get; set; }

        // ? @org.kie.api.definition.type.Position(1)
        public double PercentOff { get; set; }

        public PromoEvent()
        {
        }

        public PromoEvent(string itemId, double percentOff)
        {
            this.ItemId = itemId;
            this.PercentOff = percentOff;
        }
    }
}