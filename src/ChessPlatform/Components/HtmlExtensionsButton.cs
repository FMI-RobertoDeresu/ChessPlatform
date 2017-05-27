using System.Text;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ChessPlatform.Components
{
    public static class HtmlExtensionsButton
    {
        public static HtmlString BootstrapBotton(this IHtmlHelper htmlHelper, string buttonText,
            HtmlExtensionsCommon.ButtonType buttonType = HtmlExtensionsCommon.ButtonType.Primary,
            HtmlExtensionsCommon.ButtonSize buttonSize = HtmlExtensionsCommon.ButtonSize.Default,
            HtmlExtensionsCommon.ButtonWidth buttonWidth = HtmlExtensionsCommon.ButtonWidth.Normal, bool centered = false)
        {
            var buttonClass = "btn" + GetButtonClassForType(buttonType) + GetButtonClassForSize(buttonSize);
            var style = GetButtonStyle(buttonType, buttonWidth, centered);

            return
                new HtmlString($"<button type='{buttonType}' class='{buttonClass}' {style}>{buttonText}</button>");
        }

        public static string GetButtonClassForType(HtmlExtensionsCommon.ButtonType buttonType)
        {
            switch (buttonType)
            {
                case HtmlExtensionsCommon.ButtonType.Primary:
                    return " btn-primary";
                case HtmlExtensionsCommon.ButtonType.Success:
                    return " btn-success";
                case HtmlExtensionsCommon.ButtonType.Info:
                    return " btn-info";
                case HtmlExtensionsCommon.ButtonType.Warning:
                    return " btn-warning";
                case HtmlExtensionsCommon.ButtonType.Danger:
                    return " btn-danger";
                default:
                    return " btn-default";
            }
        }

        public static string GetButtonClassForSize(HtmlExtensionsCommon.ButtonSize buttonSize)
        {
            switch (buttonSize)
            {
                case HtmlExtensionsCommon.ButtonSize.Large:
                    return " btn-lg";
                case HtmlExtensionsCommon.ButtonSize.Small:
                    return " btn-sm";
                case HtmlExtensionsCommon.ButtonSize.ExtraSmall:
                    return " btn-xs";
                default:
                    return string.Empty;
            }
        }

        public static string GetButtonStyle(HtmlExtensionsCommon.ButtonType buttonType,
            HtmlExtensionsCommon.ButtonWidth buttonWidth, bool centered)
        {
            var styleBuilder = new StringBuilder("style='");

            if (buttonWidth == HtmlExtensionsCommon.ButtonWidth.Full)
            {
                styleBuilder.Append("width: 100%;");
            }

            if (centered)
            {
                styleBuilder.Append("margin: auto; display: block;");
            }

            styleBuilder.Append("'");

            return styleBuilder.ToString();
        }
    }
}