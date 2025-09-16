namespace Digital_Library.PdfViewer.Models;
public class JwtResponse
{
	public string Token { get; set; }
	public DateTime Expiration { get; set; }
	public string UserId { get; set; }
	public string Email { get; set; }
	public List<string> Roles { get; set; }
}
