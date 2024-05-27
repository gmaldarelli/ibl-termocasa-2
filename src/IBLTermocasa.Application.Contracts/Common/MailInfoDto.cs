using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace IBLTermocasa.Common;

public class MailInfoDto
{
    public List<MailItemDto> MailItems { get; set; } = new();
}

public class MailItemDto
{
    public MailType Type { get; set; }
    [EmailAddress]
    [Required]
    public string Email { get; set; }
    public override string ToString()
    {
        return Email;
    }
}
