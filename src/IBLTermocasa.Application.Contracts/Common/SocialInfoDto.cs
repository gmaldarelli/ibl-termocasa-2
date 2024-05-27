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
}

public class SocialItemDto
{
    public SocialType SocialType { get; set; }
    public string Url { get; set; }
}
