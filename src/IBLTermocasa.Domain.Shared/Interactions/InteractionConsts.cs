namespace IBLTermocasa.Interactions
{
    public static class InteractionConsts
    {
        private const string DefaultSorting = "{0}InteractionType asc";

        public static string GetDefaultSorting(bool withEntityName)
        {
            return string.Format(DefaultSorting, withEntityName ? "Interaction." : string.Empty);
        }

        public const int ContentMaxLength = 500;
    }
}