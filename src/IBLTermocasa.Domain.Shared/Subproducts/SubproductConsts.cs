namespace IBLTermocasa.Subproducts
{
    public static class SubproductConsts
    {
        private const string DefaultSorting = "{0}Order asc,{0}Name asc";

        public static string GetDefaultSorting(bool withEntityName)
        {
            return string.Format(DefaultSorting, withEntityName ? "Subproduct." : string.Empty);
        }

    }
}