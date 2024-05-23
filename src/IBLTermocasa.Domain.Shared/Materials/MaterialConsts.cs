namespace IBLTermocasa.Materials
{
    public static class MaterialConsts
    {
        private const string DefaultSorting = "{0}Code asc,{0}Name asc,{0}MeasureUnit asc,{0}Quantity asc,{0}Lifo asc,{0}StandardPrice asc,{0}AveragePrice asc,{0}LastPrice asc,{0}AveragePriceSecond asc";

        public static string GetDefaultSorting(bool withEntityName)
        {
            return string.Format(DefaultSorting, withEntityName ? "Material." : string.Empty);
        }

    }
}