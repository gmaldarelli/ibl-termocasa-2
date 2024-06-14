﻿namespace IBLTermocasa.BillOfMaterials
{
    public static class BillOFMaterialConsts
    {
        private const string DefaultSorting = "{0}Name asc";

        public static string GetDefaultSorting(bool withEntityName)
        {
            return string.Format(DefaultSorting, withEntityName ? "BillOFMaterial." : string.Empty);
        }

    }
}