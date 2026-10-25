using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.IO;
using System.Web;
using System.Web.SessionState;

namespace netcreative.ca
{
    // The anti-spam picture of the contact form: a small sum (for example "7 + 5 =") drawn fresh for
    // every visitor, with each character tilted and some noise behind it, so there is no fixed set of
    // pictures a robot could learn. The sum itself is kept in the visitor's session by Contact.aspx.
    public class ContactCaptcha : IHttpHandler, IReadOnlySessionState
    {
        public const string SessionTextKey = "Contact.AntiSpam.Text";

        private const int Width = 220;
        private const int Height = 64;

        public bool IsReusable
        {
            get { return false; }
        }

        public void ProcessRequest(HttpContext context)
        {
            context.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            context.Response.Cache.SetNoStore();

            string text = context.Session == null ? null : context.Session[SessionTextKey] as string;
            if (string.IsNullOrEmpty(text))
            {
                context.Response.StatusCode = 404;
                return;
            }

            context.Response.ContentType = "image/png";
            context.Response.BinaryWrite(Render(text));
        }

        // the picture of one sum as PNG bytes
        public static byte[] Render(string text)
        {
            var rnd = new Random(Guid.NewGuid().GetHashCode());

            using (var bmp = new Bitmap(Width, Height))
            using (var g = Graphics.FromImage(bmp))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;

                using (var bg = new LinearGradientBrush(new Rectangle(0, 0, Width, Height), Color.FromArgb(246, 246, 246), Color.FromArgb(226, 230, 238), 20f))
                {
                    g.FillRectangle(bg, 0, 0, Width, Height);
                }

                // noise behind the text
                for (int i = 0; i < 90; i++)
                {
                    using (var dot = new SolidBrush(Color.FromArgb(70, rnd.Next(60, 200), rnd.Next(60, 200), rnd.Next(60, 200))))
                    {
                        g.FillEllipse(dot, rnd.Next(Width), rnd.Next(Height), rnd.Next(1, 4), rnd.Next(1, 4));
                    }
                }

                for (int i = 0; i < 5; i++)
                {
                    using (var pen = new Pen(Color.FromArgb(90, rnd.Next(80, 210), rnd.Next(80, 210), rnd.Next(80, 210)), 1.5f))
                    {
                        g.DrawLine(pen, rnd.Next(Width), rnd.Next(Height), rnd.Next(Width), rnd.Next(Height));
                    }
                }

                // the characters, one by one, each with its own tilt, height and colour
                using (var font = new Font(FontFamily.GenericSansSerif, 30f, FontStyle.Bold, GraphicsUnit.Pixel))
                using (var format = StringFormat.GenericTypographic)
                {
                    float x = 14f;

                    foreach (char c in text)
                    {
                        string s = c.ToString();
                        SizeF size = g.MeasureString(s, font, 200, format);

                        GraphicsState state = g.Save();
                        g.TranslateTransform(x + size.Width / 2f, Height / 2f + rnd.Next(-5, 6));
                        g.RotateTransform(rnd.Next(-14, 15));

                        using (var brush = new SolidBrush(Color.FromArgb(rnd.Next(20, 90), rnd.Next(20, 90), rnd.Next(20, 90))))
                        {
                            g.DrawString(s, font, brush, -size.Width / 2f, -size.Height / 2f, format);
                        }

                        g.Restore(state);
                        x += size.Width + 9f;
                    }
                }

                // a line across the text
                using (var pen = new Pen(Color.FromArgb(150, 90, 90, 90), 1.5f))
                {
                    g.DrawLine(pen, 0, rnd.Next(18, 46), Width, rnd.Next(18, 46));
                }

                using (var ms = new MemoryStream())
                {
                    bmp.Save(ms, ImageFormat.Png);
                    return ms.ToArray();
                }
            }
        }
    }
}
