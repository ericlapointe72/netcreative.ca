using System.Collections.Generic;
using System.Web.Script.Serialization;
using netcreative.ca.Resources;

namespace netcreative.ca
{
    // The questions and answers of a "Questions fréquentes" section as the FAQPage of schema.org (what Google reads to show them under
    // the result). The texts are the ones the page shows: keys "<prefix>_Q1" / "<prefix>_A1" ... of the resources.
    public static class FaqSchema
    {
        // "<" is written as < so that nothing in a text can close the script block that holds the JSON
        public static string Json(string prefix, int count)
        {
            var questions = new List<object>();

            for (int i = 1; i <= count; i++)
            {
                questions.Add(new Dictionary<string, object>
                {
                    { "@type", "Question" },
                    { "name", Global.Text(prefix + "_Q" + i) },
                    { "acceptedAnswer", new Dictionary<string, object> { { "@type", "Answer" }, { "text", Global.Text(prefix + "_A" + i) } } }
                });
            }

            var page = new Dictionary<string, object>
            {
                { "@context", "https://schema.org" },
                { "@type", "FAQPage" },
                { "mainEntity", questions }
            };

            return new JavaScriptSerializer().Serialize(page).Replace("<", "\\u003c");
        }
    }
}
