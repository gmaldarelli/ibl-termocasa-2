namespace IBLTermocasa.Catalogs
{
    public static class CatalogConsts
    {
        private const string DefaultSorting = "{0}Name asc";

        public static string GetDefaultSorting(bool withEntityName)
        {
            return string.Format(DefaultSorting, withEntityName ? "Catalog." : string.Empty);
        }

    }
}