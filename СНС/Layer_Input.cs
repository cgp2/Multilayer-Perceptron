using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace СНС
{
    class Layer_Input : ILayers
    {
        private List<INeurons> neuronsList = new List<INeurons>(); // Подумать над логикой
        private int neuronsCount = 0;
        private int type = 0; // 0 - входной слой, 1 - скрытые слои, 2 - выходной

        public Layer_Input(int n)
        {
            neuronsCount = n + 1;
            neuronsList.Add(new Neuron_Input(0, 0));
            for (int i = 1; i < neuronsCount; i++)
                neuronsList.Add(new Neuron_Input(i, 0));
        }

        public void SetWeights(string[] input)
        { }

        public void SetWeights(List<List<double>> w)
        {
        }

        public void AddLinks(int n)
        {
        }

        public List<double> Result(List<double> input)
        {
            neuronsList[0].Result = 1;
            for (int i = 1; i < neuronsCount; i++)
                neuronsList[i].Result = input[i - 1];

            List<double> res = new List<double>();
            res.Add(1);
            res.AddRange(input);

            return res;
        }

        public int NeuronsCount
        {
            get
            {
                return neuronsCount;
            }
        }
        public int Type
        {
            get
            {
                return type;
            }
            set
            {
                type = value;
            }
        }
        public List<INeurons> NeuronsList
        {
            get
            {
                return neuronsList;
            }
            set
            {
                neuronsList = value;
            }
        }
    }

    class Neuron_Input : INeurons
    {
        List<Link> incomingLinksList = new List<Link>();
        int incomingLinksCount = 0;
        int type = 0;
        double result, sum = 1;
        int id;

        public Neuron_Input(int id, int type)
        {
            this.id = id;
            this.type = type;
        }

        public void SetLinks(List<double> w)
        {
            incomingLinksCount = 0;
        }

        public void AddIncomingLinks(int n)
        {
            incomingLinksCount = 0;
        }

        public double Res(List<double> input)
        {
            return result;
        }

        public double Derivate(double x)
        {
            return 0;
        }

        public List<Link> IncomingLinksList
        {
            get
            {
                return incomingLinksList;
            }
            set
            {
                incomingLinksList = value;
            }
        }
        public int IncomingLinksCount
        {
            get
            {
                return incomingLinksCount;
            }
            set
            {
                incomingLinksCount = value;
            }
        }
        public int Type
        {
            get
            {
                return type;
            }
            set
            {
                type = value;
            }
        }
        public double Result
        {
            get
            {
                return result;
            }
            set
            {
                result = value;
            }
        }
        public double Sum
        {
            get
            {
                return sum;
            }
        }
        public int Id
        {
            get
            {
                return id;
            }
        }
    }
}
