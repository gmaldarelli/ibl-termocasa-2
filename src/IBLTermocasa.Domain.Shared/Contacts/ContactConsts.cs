namespace IBLTermocasa.Contacts
{
    public static class ContactConsts
    {
        private const string DefaultSorting = "{0}Name asc,{0}JobRole asc,{0}Surname asc,{0}MailInfo asc,{0}ConfidentialName asc,{0}PhoneInfo asc,{0}Tag asc";

        public static string GetDefaultSorting(bool withEntityName)
        {
            return string.Format(DefaultSorting, withEntityName ? "Contact." : string.Empty);
        }

    }
}