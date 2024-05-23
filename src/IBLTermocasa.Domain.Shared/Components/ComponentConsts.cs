namespace IBLTermocasa.Components
{
    public static class ComponentConsts
    {
        private const string DefaultSorting = "{0}Name asc";

        public static string GetDefaultSorting(bool withEntityName)
        {
            return string.Format(DefaultSorting, withEntityName ? "Component." : string.Empty);
        }

    }
}