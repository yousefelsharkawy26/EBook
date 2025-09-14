using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Digital_Library.Core.Models
{
	public class UserBookAccess
	{
		[Key]
		public string Id { get; set; } = Guid.NewGuid().ToString();

		[ForeignKey(nameof(User))]
		[Required]
		public string UserId { get; set; }

		[ForeignKey(nameof(Book))]
		[Required]
		public string BookId { get; set; }

		public User? User { get; set; }
		public Book? Book { get; set; }

		// المسار للملف المشفر بالمستخدم
		public string? FilePath { get; set; }

		// Envelope Encryption fields
		[Column(TypeName = "varbinary(32)")]
		public byte[]? EncryptedDEK { get; set; }

		[Column(TypeName = "varbinary(16)")]
		public byte[]? IV { get; set; }

		[Column(TypeName = "varbinary(16)")]
		public byte[]? Tag { get; set; }

		// التواريخ
		public DateTime AssignedDate { get; set; } = DateTime.UtcNow;

		// إذا كانت استعارة
		public DateTime? BorrowDate { get; set; }
		public DateTime? DueDate { get; set; }
	}
}
