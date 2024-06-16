namespace IBLTermocasa.BillOfMaterials
{
    public static class BillOfMaterialConsts
    {
        private const string DefaultSorting = "{0}BomNumber asc";

        public static string GetDefaultSorting(bool withEntityName)
        {
            return string.Format(DefaultSorting, withEntityName ? "BillOfMaterial." : string.Empty);
        }

    }
}