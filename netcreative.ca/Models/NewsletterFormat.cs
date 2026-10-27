using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace netcreative.ca
{
    // The text typed in the "Message" box of the newsletter page, as the HTML of the e-mail. One place makes it, for the e-mail AND for
    // the preview of the page (NewsletterPreview.ashx), so what is previewed is what is sent.
    //
    // What can be typed (a few simple marks, nothing else becomes HTML: every other character is written as text):
    //   a blank line               a new paragraph
    //   ## Title                   a title
    //   - item                     a list (one "- " or "* " at the start of each line)
    //   **bold**                   bold
    //   [text](https://address)    a link (also mailto:)
    //   https://address            a link, by itself
    //   > [Text](https://address)  a button (alone on its line)
    public static class NewsletterFormat
    {
        private const string Blue = "#1f78c1";
        private const string LinkBlue = "#4473c5";

        private static readonly Regex Heading = new Regex(@"^#{1,2}\s+(?<t>.+)$", RegexOptions.Compiled);
        private static readonly Regex Item = new Regex(@"^[-*]\s+(?<t>.+)$", RegexOptions.Compiled);
        private static readonly Regex Button = new Regex(@"^>\s*\[(?<t>[^\]]+)\]\((?<u>https?://[^\s)]+)\)\s*$", RegexOptions.Compiled);

        // on text that was already HTML-encoded: a link [text](address), **bold**, or an address written as it is
        private static readonly Regex Inline = new Regex(
            @"\[(?<t>[^\]]+)\]\((?<u>(?:https?://|mailto:)[^\s)]+)\)"
            + @"|\*\*(?<b>.+?)\*\*"
            + @"|(?<a>https?://(?:(?!&quot;|&#39;|&lt;|&gt;)[^\s<])+)",
            RegexOptions.Compiled | RegexOptions.Singleline);

        public const int MaxLength = 20000;

        // the message, then the closing line (a paragraph of its own, when there is one)
        public static string Html(string message, string closing)
        {
            string html = Html(message);
            string end = Html(closing);

            return html + end;
        }

        public static string Html(string text)
        {
            text = (text ?? string.Empty).Replace("\r\n", "\n").Replace('\r', '\n');

            if (text.Length > MaxLength)
            {
                text = text.Substring(0, MaxLength);
            }

            var html = new StringBuilder();
            var paragraph = new List<string>();
            var items = new List<string>();

            foreach (string raw in text.Split('\n'))
            {
                string line = raw.Trim();

                if (line.Length == 0)
                {
                    EndParagraph(html, paragraph);
                    EndList(html, items);
                    continue;
                }

                Match m = Heading.Match(line);
                if (m.Success)
                {
                    EndParagraph(html, paragraph);
                    EndList(html, items);
                    html.Append("<h2 style=\"margin:0 0 12px;font-family:Arial,Helvetica,sans-serif;font-size:20px;line-height:1.3;color:" + Blue + "\">")
                        .Append(Inline_(m.Groups["t"].Value)).Append("</h2>");
                    continue;
                }

                m = Button.Match(line);
                if (m.Success)
                {
                    EndParagraph(html, paragraph);
                    EndList(html, items);
                    html.Append("<table role=\"presentation\" cellpadding=\"0\" cellspacing=\"0\" border=\"0\" style=\"border-collapse:separate;margin:0 0 16px\"><tr>")
                        .Append("<td style=\"background-color:" + Blue + ";border-radius:4px\"><a href=\"").Append(HttpUtility.HtmlEncode(m.Groups["u"].Value))
                        .Append("\" style=\"display:inline-block;padding:12px 22px;font-family:Arial,Helvetica,sans-serif;font-size:15px;font-weight:bold;color:#ffffff;text-decoration:none\">")
                        .Append(HttpUtility.HtmlEncode(m.Groups["t"].Value)).Append("</a></td></tr></table>");
                    continue;
                }

                m = Item.Match(line);
                if (m.Success)
                {
                    EndParagraph(html, paragraph);
                    items.Add(m.Groups["t"].Value);
                    continue;
                }

                EndList(html, items);
                paragraph.Add(line);
            }

            EndParagraph(html, paragraph);
            EndList(html, items);

            return html.ToString();
        }

        private static void EndParagraph(StringBuilder html, List<string> lines)
        {
            if (lines.Count == 0)
            {
                return;
            }

            html.Append("<p style=\"margin:0 0 16px\">");

            for (int i = 0; i < lines.Count; i++)
            {
                if (i > 0)
                {
                    html.Append("<br>");
                }

                html.Append(Inline_(lines[i]));
            }

            html.Append("</p>");
            lines.Clear();
        }

        private static void EndList(StringBuilder html, List<string> items)
        {
            if (items.Count == 0)
            {
                return;
            }

            html.Append("<ul style=\"margin:0 0 16px;padding-left:22px\">");

            foreach (string item in items)
            {
                html.Append("<li style=\"margin:0 0 6px\">").Append(Inline_(item)).Append("</li>");
            }

            html.Append("</ul>");
            items.Clear();
        }

        // a line of typed text: encoded first (so nothing typed can be HTML), then the marks above are turned into HTML
        private static string Inline_(string typed)
        {
            return Marks(HttpUtility.HtmlEncode(typed));
        }

        private static string Marks(string encoded)
        {
            return Inline.Replace(encoded, m =>
            {
                if (m.Groups["u"].Success)
                {
                    return Link(m.Groups["u"].Value, m.Groups["t"].Value);
                }

                if (m.Groups["b"].Success)
                {
                    return "<strong>" + Marks(m.Groups["b"].Value) + "</strong>";
                }

                // an address written as it is: the punctuation of the sentence that follows it is not part of it
                string address = m.Groups["a"].Value;
                string tail = string.Empty;

                while (address.Length > 0 && ".,;:!?)".IndexOf(address[address.Length - 1]) >= 0 && !address.EndsWith("&#39;") && !address.EndsWith("&quot;"))
                {
                    tail = address[address.Length - 1] + tail;
                    address = address.Substring(0, address.Length - 1);
                }

                return Link(address, address) + tail;
            });
        }

        private static string Link(string address, string text)
        {
            return "<a href=\"" + address + "\" style=\"color:" + LinkBlue + "\">" + text + "</a>";
        }
    }
}
