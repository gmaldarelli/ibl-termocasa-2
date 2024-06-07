namespace IBLTermocasa.Shared
{
    public class LookupDto<TKey>
    {
        public TKey Id { get; set; }

        public string DisplayName { get; set; } = null!;
        
        public LookupDto(TKey id, string displayName)
        {
            Id = id;
            DisplayName = displayName;
        }
        public LookupDto()
        {
        }
    }
}