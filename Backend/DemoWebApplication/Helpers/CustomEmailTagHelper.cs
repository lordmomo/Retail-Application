using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Diagnostics.CodeAnalysis;

namespace DemoWebApplication.Helpers
{
    [ExcludeFromCodeCoverage]

    public class CustomEmailTagHelper : TagHelper
    {
        public string MyEmail { get; set; }
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "a";
            output.Attributes.SetAttribute("href", MyEmail);
            output.Attributes.Add("id", "my-email-id");
            output.Content.SetContent("my-email");
        }
    }
}
