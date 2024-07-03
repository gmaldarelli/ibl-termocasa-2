namespace IBLTermocasa.ProfessionalProfiles
{
    public static class ProfessionalProfileConsts
    {
        private const string DefaultSorting = "{0}Name asc";

        public static string GetDefaultSorting(bool withEntityName)
        {
            return string.Format(DefaultSorting, withEntityName ? "ProfessionalProfile." : string.Empty);
        }

    }
}