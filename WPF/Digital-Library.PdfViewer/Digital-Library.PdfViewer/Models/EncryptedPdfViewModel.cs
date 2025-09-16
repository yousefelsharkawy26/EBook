namespace Digital_Library.PdfViewer.Models;

public class EncryptedPdfViewModel
{
	public byte[] EncryptedFile { get; set; }
	public byte[] IV { get; set; }              
	public byte[] Tag { get; set; }            
	public byte[] EncryptedDEK { get; set; }    
	public string FileName { get; set; }

	public string? type { get; set; }

	public string? Email { get; set; }
}
