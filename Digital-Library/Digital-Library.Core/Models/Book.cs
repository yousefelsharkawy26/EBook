using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Digital_Library.Core.Models
{
    internal class Book
    {
        [Key]
        public Guid BookId { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public double PricePhysical { get; set; }
        public double PricePDFPerDay { get; set; }
        public string Description { get; set; }
        public int Stock { get; set; }
        public bool HasPDF { get; set; }
        public bool isBorrowable { get; set; }
        public string PDF_FilePath { get; set; }
        public DateTime UploadDate { get; set; }

        //public Category Category { get; set; }
        //[ForeignKey(nameof(Category))]
        public Guid CategoryID { get; set; }

        public ICollection<Borrowing> Borrowing { get; set; }


        /*public Vendor Vendor { get; set; }
        [ForeignKey(nameof(Vendor))]
        public Guid VendorID { get; set; }*/

        //public ICollection<OrderDetail> OrderDetails { get; set; }
        //public ICollection<CartDetail> CartDetails { get; set; }


    }
}
