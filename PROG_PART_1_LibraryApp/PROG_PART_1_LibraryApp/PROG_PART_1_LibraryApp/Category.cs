using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PROG_PART_1_LibraryApp
{
    public class Category
    {
        public string Code { get; set; }
        public string Title { get; set; }
        public List<Category> Subcategories { get; set; }
    }
}
