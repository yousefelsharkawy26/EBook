namespace Digital_Library.Core.Model
{
    public class Category
    {
        public Guid CategroyID { get; set; }
        public string CategroyName { get; set; }
        public string Description { get; set; }
        public bool IsApproved { get; set; }

        public ICollection<Book> Books { get; set; }
    }
}
