using System;
using System.Linq.Expressions;
using System.Text;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc.ViewFeatures.Internal;
using Microsoft.AspNetCore.Routing;

namespace ChessPlatform.Components
{
    public static class HtmlExtensionsInput
    {
        /// <summary>
        ///     Not working for enumerable types.
        /// </summary>
        public static HtmlString BootstrapTextBoxFor<TModel, TValue>(this IHtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, TValue>> expression, HtmlExtensionsCommon.InputType inputType, object htmlAttributes = null)
        {
            var builder = new StringBuilder();
            var routeValueDictionary = new RouteValueDictionary(HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
            var labelText = htmlHelper.GetDisplayName(expression);
            var labelHtmlAttributes = new { @class = "col-sm-5 control-label" };
            var validationHtmlAttributes = new { @class = "text-danger" };

            routeValueDictionary.Add("type", inputType.ToString());
            routeValueDictionary.Add("class", "form-control");
            routeValueDictionary.Add("placeholder", labelText);

            builder.AppendLine("<div class='form-group'>");
            builder.AppendLine(HtmlExtensionsCommon.FormatLabel(htmlHelper.LabelFor(expression, labelHtmlAttributes).GetString(),
                labelText));
            builder.AppendLine("<div class='col-sm-7'>");
            builder.AppendLine(htmlHelper.TextBoxFor(expression, routeValueDictionary).GetString());
            builder.AppendLine(htmlHelper.ValidationMessageFor(expression, null, validationHtmlAttributes).GetString());
            builder.AppendLine("</div>");
            builder.AppendLine("</div>");

            return new HtmlString(builder.ToString());
        }

        public static HtmlString BootstrapCheckBoxFor<TModel>(this IHtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, bool>> expression, object htmlAttributes = null)
        {
            var builder = new StringBuilder();
            var routeValueDictionary = new RouteValueDictionary(HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
            var labelText = htmlHelper.GetDisplayName(expression);

            routeValueDictionary.Add("type", "checkbox");
            routeValueDictionary.Add("class", "input-assumpte");

            builder.AppendLine("<div class='form-group'>");
            builder.AppendLine("<div class='checkboxx col-sm-offset-5 col-sm-7'>");
            builder.Append(htmlHelper.CheckBox(ExpressionHelper.GetExpressionText(expression), routeValueDictionary).GetString());
            builder.AppendLine(HtmlExtensionsCommon.FormatLabel(htmlHelper.LabelFor(expression).GetString(), labelText));
            builder.AppendLine("</div>");
            builder.AppendLine("</div>");

            return new HtmlString(builder.ToString());
        }
    }
}