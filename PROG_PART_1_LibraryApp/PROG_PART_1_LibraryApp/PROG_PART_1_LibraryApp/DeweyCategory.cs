using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PROG_PART_1_LibraryApp
{
    public class DeweyCategory
    {
        public string Description { get; set; }
        public Dictionary<string, DeweyCategory> Children { get; set; }
        public DeweyCategory Parent { get; set; }
    }
}
