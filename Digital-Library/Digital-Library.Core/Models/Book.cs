using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.SymbolStore;

namespace Digital_Library.Core.Model
{
    public class Book
    {
        [Key]
        public Guid BookId { get; set; }
        [StringLength(200)]
        public string Title { get; set; }   
        [StringLength(50)]
        public string Author { get; set; }
        public double PricePhysical { get; set; }
        public double PricePDFPerDay { get; set; }
        [StringLength(400)]
        public string Description { get; set; }
        public int Stock { get; set; }
        public bool HasPDF { get; set; }
        public bool isBorrowable { get; set; }
        [StringLength(150)]
        public string PDF_FilePath { get; set; }
        public DateTime UploadDate { get; set; }

        public Category Category { get; set; }
        [ForeignKey(nameof(Category))]
        public Guid CategoryID { get; set; }

        /*public Vendor Vendor { get; set; }
         [ForeignKey(nameof(Vendor))]
        public Guid VendorID { get; set; }*/

        //public ICollection<OrderDetail> OrderDetails { get; set; }
        //public ICollection<CartDetail> CartDetails { get; set; }
        //public ICollection<Borrowing> Borrowing { get; set; }





    }
}
