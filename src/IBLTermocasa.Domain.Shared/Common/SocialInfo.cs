using System.Collections.Generic;

namespace IBLTermocasa.Common;

public class SocialInfo
{
    public List<SocialItem> SocialItems { get; set; } = new();
}

public class SocialItem
{
    public SocialType SocialType { get; set; }
    public string Url { get; set; }
}

public enum SocialType
{
    LINKEDIN,
    TWITTER,
    FACEBOOK,
    INSTAGRAM,
    WEBSITE,
    OTHER
}