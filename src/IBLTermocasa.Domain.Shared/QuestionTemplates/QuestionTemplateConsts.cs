namespace IBLTermocasa.QuestionTemplates
{
    public static class QuestionTemplateConsts
    {
        private const string DefaultSorting = "{0}Code asc";

        public static string GetDefaultSorting(bool withEntityName)
        {
            return string.Format(DefaultSorting, withEntityName ? "QuestionTemplate." : string.Empty);
        }

    }
}