using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;

namespace IBLTermocasa.Web.Public.Pages;

public class IndexModel : IBLTermocasaPublicPageModel
{
    public void OnGet()
    {

    }

    public async Task OnPostLoginAsync()
    {
        await HttpContext.ChallengeAsync("oidc");
    }
}
