namespace IBLTermocasa.Quotations
{
    public static class QuotationConsts
    {
        private const string DefaultSorting = "{0}IdRFQ asc,{0}IdBOM asc,{0}Code asc,{0}Name asc,{0}SentDate asc,{0}QuotationValidDate asc,{0}ConfirmedDate asc,{0}Status asc,{0}DepositRequired asc,{0}DepositRequiredValue asc,{0}QuotationItems asc";

        public static string GetDefaultSorting(bool withEntityName)
        {
            return string.Format(DefaultSorting, withEntityName ? "Quotation." : string.Empty);
        }

    }
}