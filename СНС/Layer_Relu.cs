using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace СНС
{
    class Layer_Relu : ILayers
    {
        List<INeurons> neuronsList = new List<INeurons>(); // Подумать над логикой
        int neuronsCount = 0;
        int type = 1; // 0 - входной слой, 1 - скрытые слои, 2 - выходной

        public Layer_Relu(List<List<double>> InpWeights)
        {
            neuronsCount = InpWeights.Count;
            for (int i = 0; i < neuronsCount; i++)
            {
                Neuron_Relu n = new Neuron_Relu(i);
                n.SetLinks(InpWeights[i]);
                neuronsList.Add(n);
            }
        }

        public Layer_Relu(int n)
        {
            neuronsCount = n + 1;
            neuronsList.Add(new Neuron_Relu(0));
            for (int i = 1; i < neuronsCount; i++)
                neuronsList.Add(new Neuron_Relu(i));
        }

        public void SetWeights(string[] input)
        {
            for (int i = 1; i < neuronsCount; i++)
            {
                List<double> w = new List<double>();
                string[] s = input[i - 1].Split(' ');
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
            foreach (Neuron_Relu nr in neuronsList)
                if (nr.Id != 0)
                    nr.AddIncomingLinks(n);
        }

        public List<double> Result(List<double> input)
        {
            List<double> r = new List<double>();
            r.Add(1);
            for (int i = 1; i < neuronsCount; i++)
                r.Add(neuronsList[i].Res(input));
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
    class Neuron_Relu : INeurons
    {
        public List<Link> incomingLinksList = new List<Link>();
        private int incomingLinksCount = 0;
        public int type = 2;
        public double result, sum = 1;
        public int id;

        public Neuron_Relu(int id)
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

        public double Res(List<double> input)
        {
            result = 0;
            sum = 0;
            for (int i = 0; i < input.Count; i++)
                sum += input[i] * incomingLinksList[i].Weight;
            if (Sum > 0)
                result = Sum;
            else
                result = 0.01 * Sum;
            return result;
        }

        public double Derivate(double x)
        {
            double der = 0;
            if (x == 0.01)
                der = 0.01;
            else
                der = 1;

            return der;
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
