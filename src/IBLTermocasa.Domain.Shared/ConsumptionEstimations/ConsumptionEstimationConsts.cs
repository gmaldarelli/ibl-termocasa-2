namespace IBLTermocasa.ConsumptionEstimations
{
    public static class ConsumptionEstimationConsts
    {
        private const string DefaultSorting = "{0}ConsumptionProduct asc";

        public static string GetDefaultSorting(bool withEntityName)
        {
            return string.Format(DefaultSorting, withEntityName ? "ConsumptionEstimation." : string.Empty);
        }

    }
}