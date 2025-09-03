using Digital_Library.Core.Enum;
using Digital_Library.Core.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Digital_Library.Core.Models
{
    public class Transaction
    {
        [Key]
        public Guid PaymentId { get; set; }

        public Status TransactionStatus { get; set; }

        public int Amount { get; set; }

        public PaymentMethod PaymentMethod { get; set; }

        public DateTime TransactionDate { get; set; }

        public string ReferenceCode { get; set; }


        public Guid OrderId { get; set; }

        [ForeignKey(nameof(Order.OrderId))]
        public Order Order { get; set; }



    }
}
