using Microsoft.AspNetCore.Razor.TagHelpers;
using hongWenAPP.Services;

namespace hongWenAPP.TagHelpers
{
    [HtmlTargetElement("*", Attributes = "asp-permission")]
    [HtmlTargetElement("*", Attributes = "asp-permissions")]
    public class PermissionTagHelper : TagHelper
    {
        private readonly AuthenticationService _authService;

        public PermissionTagHelper(AuthenticationService authService)
        {
            _authService = authService;
        }

        [HtmlAttributeName("asp-permission")]
        public string Permission { get; set; }

        [HtmlAttributeName("asp-permissions")]
        public string Permissions { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            bool hasAccess = false;

            if (!string.IsNullOrEmpty(Permission))
            {
                hasAccess = _authService.HasPermission(Permission);
            }
            else if (!string.IsNullOrEmpty(Permissions))
            {
                var permissionList = Permissions.Split(',').Select(p => p.Trim()).ToArray();
                hasAccess = _authService.HasAnyPermission(permissionList);
            }

            if (!hasAccess)
            {
                output.SuppressOutput();
            }
        }
    }
} 