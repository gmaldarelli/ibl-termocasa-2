namespace IBLTermocasa.Industries
{
    public static class IndustryConsts
    {
        private const string DefaultSorting = "{0}Code asc";

        public static string GetDefaultSorting(bool withEntityName)
        {
            return string.Format(DefaultSorting, withEntityName ? "Industry." : string.Empty);
        }

        public const int CodeMaxLength = 255;
        public const int DescriptionMaxLength = 500;
    }
}