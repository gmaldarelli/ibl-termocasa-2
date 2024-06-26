namespace IBLTermocasa.Common;

public class ExtendedLookUpDto <TItem> 
{
    public TItem Id { get; set; }
    public string DisplayName { get; set; }
    public ViewElementDto ViewElementDto { get; set; } = new ViewElementDto();
}