namespace Base.Models;

public class Document
{
    public Guid Id { get; set; }
    public required string FileName { get; set; }
    public required string SaveTo { get; set; }
    public required string Domain { get; set; }
    public string Url => $"{SaveTo}/{FileName}";
}
