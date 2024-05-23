namespace IBLTermocasa.Products
{
    public static class ProductConsts
    {
        private const string DefaultSorting = "{0}Code asc";

        public static string GetDefaultSorting(bool withEntityName)
        {
            return string.Format(DefaultSorting, withEntityName ? "Product." : string.Empty);
        }

    }
}