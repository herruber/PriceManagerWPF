using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PriceManagerWPF
{
    public class MainViewModel
    {
        public int materialId { get; set; }
        public Material material { get; set; }
        public ModelData Item { get; set; }

    }
}
