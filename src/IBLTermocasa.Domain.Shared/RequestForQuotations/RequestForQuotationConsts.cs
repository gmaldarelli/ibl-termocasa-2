namespace IBLTermocasa.RequestForQuotations
{
    public static class RequestForQuotationConsts
    {
        private const string DefaultSorting = "{0}QuoteNumber asc,{0}WorkSite asc,{0}City asc,{0}OrganizationProperty asc,{0}ContactProperty asc,{0}PhoneInfo asc,{0}MailInfo asc,{0}Status asc";

        public static string GetDefaultSorting(bool withEntityName)
        {
            return string.Format(DefaultSorting, withEntityName ? "RequestForQuotation." : string.Empty);
        }

    }
}