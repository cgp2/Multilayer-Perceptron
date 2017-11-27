using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace СНС
{
    interface ILayers
    {
        int NeuronsCount { get; }
        int Type { get; set; } // 0 - входной, 1 - скрытый, 2 - выходной
        void SetWeights(string[] input);
        void SetWeights(List<List<double>> input);
        void AddLinks(int number);
        List<INeurons> NeuronsList { get; set; }

        List<double> Result(List<double> input);
    }
}
