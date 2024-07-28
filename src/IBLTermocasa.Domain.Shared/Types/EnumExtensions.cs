using System;
using IBLTermocasa.Common;

namespace IBLTermocasa.Types
{
    public static class EnumExtensions
    {
        public static string GetDisplayName(Enum value)
        {
            return value switch
            {
                RfqStatus.DRAFT => "DRAFT",
                RfqStatus.NEW => "NEW",
                RfqStatus.IN_PROGRESS_BOM => "IN_PROGRESS_BOM",
                RfqStatus.ON_HOLD => "ON_HOLD",
                RfqStatus.PENDING_REVIEW => "PENDING_REVIEW",
                RfqStatus.COMPLETED => "COMPLETED",
                RfqStatus.CANCELED => "CANCELED",
                QuotationStatus.NEW => "NEW",
                QuotationStatus.DRAFT => "DRAFT",
                QuotationStatus.AWAITING_APPROVAL => "AWAITING_APPROVAL",
                QuotationStatus.APPROVED => "APPROVED",
                QuotationStatus.SENT_WAITING => "SENT_WAITING",
                QuotationStatus.SENT => "SENT",
                OrganizationType.LEAD => "LEAD",
                OrganizationType.CUSTOMER => "CUSTOMER",
                OrganizationType.PARTNER => "PARTNER",
                OrganizationType.SUPPLIER => "SUPPLIER",
                OrganizationType.COMPETITOR => "COMPETITOR",
                MeasureUnit.CF => "CF",
                MeasureUnit.CP => "CP",
                MeasureUnit.KG => "KG",
                MeasureUnit.M2 => "M2",
                MeasureUnit.M3 => "M3",
                MeasureUnit.ML => "ML",
                MeasureUnit.MT => "MT",
                MeasureUnit.PZ => "PZ",
                MeasureUnit.RT => "RT",
                InteractionType.MESSAGE => "MESSAGE",
                InteractionType.EMAIL => "EMAIL",
                InteractionType.PHONE => "PHONE",
                InteractionType.MEETING => "MEETING",
                InteractionType.INTERNAL_MESSAGE => "INTERNAL_MESSAGE",
                InteractionType.INTERNAL_EMAIL => "INTERNAL_EMAIL",
                InteractionType.INTERNAL_PHONE => "INTERNAL_PHONE",
                InteractionType.INTERNAL_MEETING => "INTERNAL_MEETING",
                InteractionType.OTHER => "OTHER",
                EntityType.RFQ => "RFQ",
                EntityType.QUOTE => "QUOTE",
                EntityType.WORKINGSHEET => "WORKINGSHEET",
                EntityType.TECHNICALSHEET => "TECHNICALSHEET",
                EntityType.BOM => "BOM",
                EntityType.WORKORDER => "WORKORDER",
                BomStatusType.CREATED => "CREATED",
                BomStatusType.TO_BE_BILLED => "TO_BE_BILLED",
                BomStatusType.MATERIALS_BILLED => "MATERIALS_BILLED",
                BomStatusType.WORKS_BILLED => "WORKS_BILLED",
                BomStatusType.COMPLETED => "COMPLETED",
                BomStatusType.RFP_GENERATED => "RFP_GENERATED",
                AnswerType.TEXT => "TEXT",
                AnswerType.NUMBER => "NUMBER",
                AnswerType.DATE => "DATE",
                AnswerType.BOOLEAN => "BOOLEAN",
                AnswerType.LARGE_TEXT => "LARGE_TEXT",
                AnswerType.CHOICE => "CHOICE",
                MailType.EMAIL_HOME => "EMAIL_HOME",
                MailType.EMAIL_WORK => "EMAIL_WORK",
                MailType.EMAIL_NEWSLETTER => "EMAIL_NEWSLETTER",
                MailType.OTHER => "OTHER",
                SourceType.ExternalSystem => "ExternalSystem",
                SourceType.MassImport => "MassImport",
                SourceType.Manual => "Manual",
                SourceType.System => "System",
                SourceType.Unknown => "Unknown",
                CostType.CALCULATED_FOR_PRODUCT => "CALCULATED_FOR_PRODUCT",
                CostType.FIXED_FOR_WORK => "FIXED_FOR_WORK",
                CostType.FIXED_FOR_PRODUCT => "FIXED_FOR_PRODUCT",
                _ => value.ToString()
            };
        }
    }
}