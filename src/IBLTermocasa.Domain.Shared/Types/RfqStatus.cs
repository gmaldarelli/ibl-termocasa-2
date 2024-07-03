namespace IBLTermocasa.Types;

public enum RfqStatus
{
    DRAFT,
    NEW,
    IN_PROGRESS_BOM,
    ON_HOLD,
    PENDING_REVIEW,
    COMPLETED,
    CANCELED
}

public static class EnumExtensionsRfqStatus
{
    public static string GetDisplayName(this RfqStatus status)
    {
        return status switch
        {
            RfqStatus.DRAFT => "DRAFT",
            RfqStatus.NEW => "NEW",
            RfqStatus.IN_PROGRESS_BOM => "IN_PROGRESS_BOM",
            RfqStatus.ON_HOLD => "ON_HOLD",
            RfqStatus.PENDING_REVIEW => "PENDING_REVIEW",
            RfqStatus.COMPLETED => "COMPLETED",
            RfqStatus.CANCELED => "CANCELED",
            _ => status.ToString()
        };
    }
}