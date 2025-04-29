using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;

namespace Market
{
    public class AutoPart
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Title { get; set; }
        public string ShortTitle { get; set; }
        public string FullTitle { get; set; }
        public string Description { get; set; }
        public string ImagePath { get; set; }
        public string Category { get; set; }
        public double Rating { get; set; }
        public decimal Price { get; set; }

        [NotMapped]
        public string PriceDisplay => Price > 0 ? Price.ToString("C0", new CultureInfo("en-US")) : "Стоимость уточняйте";

        public int Quantity { get; set; }
        public string Color { get; set; }
        public string Size { get; set; }
        public string DeliveryCountry { get; set; }
        public decimal Discount { get; set; }
        public bool IsAvailable { get; set; }

        [NotMapped]
        public List<string> RelatedProductsIds { get; set; } = new List<string>();

        public virtual ICollection<RelatedProduct> RelatedProducts { get; set; } = new List<RelatedProduct>();
        public int PurchasedCount { get; set; }
        public string Manufacturer { get; set; }
        public DateTime ProductionDate { get; set; }
        public string Year { get; set; }
        public string EngineType { get; set; }
        public string Marking { get; set; }
        public string EngineNumber { get; set; }
        public string DeliveryDate { get; set; }
        public string CheckPeriod { get; set; }
        public string DeliveryArea { get; set; }
        public string Installment { get; set; }

        public List<string> ImagePaths { get; set; } = new List<string>();
        public virtual ICollection<ColorOption> Colors { get; set; } = new List<ColorOption>();
        public virtual ICollection<SizeOption> Sizes { get; set; } = new List<SizeOption>();
        public DateTime AddedDate { get; set; } = DateTime.Now;

        [NotMapped]
        public bool IsNew => (DateTime.Now - AddedDate).TotalDays < 30;
    }
}