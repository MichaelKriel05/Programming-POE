using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace PROG_PART_1_LibraryApp
{
    public class DeweyCategories
    {
        public Dictionary<string, string> deweyDictionary = new Dictionary<string, string>
        {
            { "000", "Comp. Science, Information, General" },
            { "100", "Philosophy + Psychology" },
            { "200", "Religion" },
            { "300", "Social Sciences" },
            { "400", "Language" },
            { "500", "Natural Sciences + Mathematics" },
            { "600", "Technology" },
            { "700", "Arts + Recreation" },
            { "800", "Literature" },
            { "900", "History + Geography" }
        };
    }
}
