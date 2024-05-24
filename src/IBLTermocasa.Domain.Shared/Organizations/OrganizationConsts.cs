namespace IBLTermocasa.Organizations
{
    public static class OrganizationConsts
    {
        private const string DefaultSorting = "{0}Code asc,{0}Name asc,{0}OrganizationType asc,{0}MailInfo asc,{0}PhoneInfo asc";

        public static string GetDefaultSorting(bool withEntityName)
        {
            return string.Format(DefaultSorting, withEntityName ? "Organization." : string.Empty);
        }

    }
}