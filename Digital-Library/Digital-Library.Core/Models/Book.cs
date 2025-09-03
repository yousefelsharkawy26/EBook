<<<<<<< HEAD
﻿using System.ComponentModel.DataAnnotations;
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
=======
﻿using System;
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
>>>>>>> bccd10592d5cce44920d9ac3ae85ad6b6612283a
        public string Description { get; set; }
        public int Stock { get; set; }
        public bool HasPDF { get; set; }
        public bool isBorrowable { get; set; }
<<<<<<< HEAD
        [StringLength(150)]
        public string PDF_FilePath { get; set; }
        public DateTime UploadDate { get; set; }

        public Category Category { get; set; }
        [ForeignKey(nameof(Category))]
        public Guid CategoryID { get; set; }

        /*public Vendor Vendor { get; set; }
         [ForeignKey(nameof(Vendor))]
=======
        public string PDF_FilePath { get; set; }
        public DateTime UploadDate { get; set; }

        //public Category Category { get; set; }
        //[ForeignKey(nameof(Category))]
        public Guid CategoryID { get; set; }

        public ICollection<Borrowing> Borrowing { get; set; }


        /*public Vendor Vendor { get; set; }
        [ForeignKey(nameof(Vendor))]
>>>>>>> bccd10592d5cce44920d9ac3ae85ad6b6612283a
        public Guid VendorID { get; set; }*/

        //public ICollection<OrderDetail> OrderDetails { get; set; }
        //public ICollection<CartDetail> CartDetails { get; set; }
<<<<<<< HEAD
        //public ICollection<Borrowing> Borrowing { get; set; }



=======
>>>>>>> bccd10592d5cce44920d9ac3ae85ad6b6612283a


    }
}
