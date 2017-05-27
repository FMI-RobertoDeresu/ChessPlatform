using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures.Internal;

namespace ChessPlatform.Components
{
    public static class HtmlExtensionsCommon
    {
        public static string GetString(this IHtmlContent htmlContent)
        {
            using (var writer = new StringWriter())
            {
                htmlContent.WriteTo(writer, HtmlEncoder.Default);

                return writer.ToString();
            }
        }

        public static string GetDisplayName<TModel, TValue>(this IHtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, TValue>> expression)
        {
            var displayAttribute =
                htmlHelper.ViewData.GetViewDataInfo(ExpressionHelper.GetExpressionText(expression))
                    .PropertyInfo.GetCustomAttributes(typeof(DisplayAttribute), false);

            return displayAttribute.Any() ? ((DisplayAttribute) displayAttribute.First()).Name : null;
        }

        public static string FormatLabel(string label, string labelText)
        {
            if (!string.IsNullOrWhiteSpace(label) && !string.IsNullOrWhiteSpace(labelText))
            {
                var startIndex = label.IndexOf('>') + 1;
                var endIndex = label.IndexOf("</", StringComparison.Ordinal);

                if (startIndex * endIndex > 0)
                {
                    return label.Remove(startIndex, endIndex - startIndex).Insert(startIndex, labelText);
                }
            }

            return label;
        }

        #region Enums

        public enum InputType
        {
            Text,
            Number,
            Password,
            Range,
            Email,
            Tel,
            Date,
            Time,
            Datetime,
            Month,
            Week,
            Color,
            Search,
            Url
        }

        public enum ButtonType
        {
            Default,
            Primary,
            Success,
            Info,
            Warning,
            Danger
        }

        public enum ButtonSize
        {
            Large,
            Default,
            Small,
            ExtraSmall
        }

        public enum ButtonWidth
        {
            Normal,
            Full
        }

        #endregion
    }
}