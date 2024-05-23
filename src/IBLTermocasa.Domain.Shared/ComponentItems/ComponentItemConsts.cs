namespace IBLTermocasa.ComponentItems
{
    public static class ComponentItemConsts
    {
        private const string DefaultSorting = "{0}IsDefault asc";

        public static string GetDefaultSorting(bool withEntityName)
        {
            return string.Format(DefaultSorting, withEntityName ? "ComponentItem." : string.Empty);
        }

    }
}