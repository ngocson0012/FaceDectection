using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DXApplication1.lib
{
    class ArraySort
    {
        public void SortByDescending(List<double> A,out List<double> Sorted,out List<int> index)
        {
            //List<int> A = new List<int>() { 4,5,3, 2, 1 };

            var sorted = A
                .Select((x, i) => new KeyValuePair<double, int>(x, i))
                .OrderByDescending(x => x.Key)
                .ToList();

            Sorted = sorted.Select(x => x.Key).ToList();
            index = sorted.Select(x => x.Value).ToList();

            
        }
    }
}
