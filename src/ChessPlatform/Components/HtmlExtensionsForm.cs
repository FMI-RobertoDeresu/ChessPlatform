using Microsoft.AspNetCore.Mvc.Rendering;

namespace ChessPlatform.Components
{
    public static class HtmlExtensionsForm
    {
        public static MvcForm BootstrapBeginHorizontalForm(this IHtmlHelper htmlHelper, string action, string controller,
            bool centered = true)
        {
            var style = centered ? "float: none; margin: 30px auto;" : "margin: 30px";
            var htmlClass = "form-horizontal col-md-5 unselectable";

            return htmlHelper.BeginForm(action, controller, FormMethod.Post, new { @class = htmlClass, style = style });
        }
    }
}