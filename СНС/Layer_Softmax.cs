using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace СНС
{
    //Всегда выходной
    class Layer_Softmax : ILayers
    {
        List<INeurons> neuronsList = new List<INeurons>(); // Подумать над логикой
        int neuronsCount = 0;
        int type = 2; // 0 - входной слой, 1 - скрытые слои, 2 - выходной

        public Layer_Softmax(List<List<double>> InpWeights)
        {
            neuronsCount = InpWeights.Count;
            for (int i = 0; i < neuronsCount; i++)
            {
                Neuron_Softmax n = new Neuron_Softmax(i);
                n.SetLinks(InpWeights[i]);
                neuronsList.Add(n);
            }
        }

        public Layer_Softmax(int n)
        {
            neuronsCount = n;
            for (int i = 0; i < neuronsCount; i++)
                neuronsList.Add(new Neuron_Softmax(i));
        }

        public void SetWeights(string[] input)
        {
            for (int i = 0; i < neuronsCount; i++)
            {
                List<double> w = new List<double>();
                string[] s = input[i].Split(' ');
                neuronsList[i].AddIncomingLinks(s.Length);
                for (int j = 0; j < s.Count(); j++)
                    w.Add(double.Parse(s[j]));
                neuronsList[i].SetLinks(w);
            }
        }

        public void SetWeights(List<List<double>> w)
        {
            for (int i = 0; i < neuronsCount; i++)
                neuronsList[i].SetLinks(w[i]);
        }

        public void AddLinks(int n)
        {
            foreach (Neuron_Softmax nr in neuronsList)
                nr.AddIncomingLinks(n);
        }

        public List<double> Result(List<double> input)
        {
            List<double> r = new List<double>();

            foreach (Neuron_Softmax nr in neuronsList)
                r.Add(nr.Res1(input));

            List<double> b = new List<double>();
            foreach (Neuron_Softmax nr in neuronsList)
                b.Add(nr.Sum);

            r.Clear();

            foreach (Neuron_Softmax nr in neuronsList)
                r.Add(nr.Res(b));

            return r;
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

    class Neuron_Softmax : INeurons
    {
        List<Link> incomingLinksList = new List<Link>();
        private int incomingLinksCount = 0;
        private int type = 3;
        private double result, sum = 1;
        private int id;

        public Neuron_Softmax(int id)
        {
            this.id = id;
        }

        public void SetLinks(List<double> w)
        {
            if (incomingLinksCount != w.Count)
            {
                incomingLinksCount = w.Count;
                incomingLinksList.Clear();
                for (int i = 0; i < w.Count; i++)
                {
                    incomingLinksList.Add(new Link(w[i], this));
                }
            }
            else
            {
                for (int i = 0; i < w.Count; i++)
                {
                    incomingLinksList[i].Weight = w[i];
                }
            }
        }

        public void AddIncomingLinks(int n)
        {
            incomingLinksCount += n;
            for (int i = 0; i < n; i++)
                incomingLinksList.Add(new Link(0, this));
        }

        public double Res1(List<double> input)
        {
            result = 0;
            sum = 0;
            for (int i = 0; i < input.Count; i++)
                sum += input[i] * incomingLinksList[i].Weight;
            return sum;
            //result = 1.0 / (1 + Math.Exp(-0.5 * sum));
            //return result;
        }

        public double Res(List<double> nr_sums)
        {
            double s = 0;
            for (int i = 0; i < nr_sums.Count; i++)
                s += Math.Exp(nr_sums[i]);
            result = Math.Exp(sum) / s;
            return Result;
        }

        public double Derivate(double x)
        {
            return x * (1 - x);
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
