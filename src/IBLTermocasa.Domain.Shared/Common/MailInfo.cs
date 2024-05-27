using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace IBLTermocasa.Common;

public class MailInfo
{
    public List<MailItem> MailItems { get; set; } = new();
}

public class MailItem
{
    public MailType Type { get; set; }
    [EmailAddress]
    [Required]
    public string Email { get; set; }
}

public enum MailType
{
    EMAIL_HOME = 0,
    EMAIL_WORK = 1,
    EMAIL_NEWSLETTER = 2,
    OTHER = 3
}