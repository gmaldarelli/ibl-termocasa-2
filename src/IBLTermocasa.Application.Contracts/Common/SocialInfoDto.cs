using System.Collections.Generic;

namespace IBLTermocasa.Common;

public class SocialInfoDto
{
    public List<SocialItemDto> SocialItems { get; set; } = new();
    public string ResolveSocialUrl(SocialType socialType)
    {
        var socialItem = SocialItems.Find(x => x.SocialType == socialType);
        return socialItem?.Url;
    }

    public string ResolveSocialIcon(SocialType socialSocialType)
    {
        return socialSocialType switch
        {
            SocialType.LINKEDIN => "fab fa-linkedin",
            SocialType.TWITTER => "fab fa-twitter",
            SocialType.FACEBOOK => "fab fa-facebook",
            SocialType.INSTAGRAM => "fab fa-instagram",
            SocialType.WEBSITE => "fab fa-globe",
            _ => "fab fa-link"
        };
    }
}

public class SocialItemDto
{
    public SocialType SocialType { get; set; }
    public string Url { get; set; }
}
