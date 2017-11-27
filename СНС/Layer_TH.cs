using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace СНС
{
    class Layer_TH : ILayers
    {
        List<INeurons> neuronsList = new List<INeurons>(); // Подумать над логикой
        int neuronsCount = 0;
        int type = 1; // 0 - входной слой, 1 - скрытые слои, 2 - выходной

        public Layer_TH(List<List<double>> InpWeights, int t)
        {
            type = t;
            neuronsCount = InpWeights.Count;
            for (int i = 0; i < neuronsCount; i++)
            {
                Neuron_TH n = new Neuron_TH(i);
                n.SetLinks(InpWeights[i]);
                neuronsList.Add(n);
            }
        }

        public Layer_TH(int n, int t)
        {
            type = t;
            neuronsCount = n;
            if (type != 2)
            {
                neuronsCount++;
                neuronsList.Add(new Neuron_TH(0));
                for (int i = 1; i < neuronsCount; i++)
                    neuronsList.Add(new Neuron_TH(i));
            }
            else
                for (int i = 0; i < neuronsCount; i++)
                    neuronsList.Add(new Neuron_TH(i));
        }

        public void SetWeights(string[] input)
        {
            if (type != 2)
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
            else
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
        }

        public void SetWeights(List<List<double>> w)
        {
            for (int i = 0; i < neuronsCount; i++)
                neuronsList[i].SetLinks(w[i]);
        }

        public void AddLinks(int n)
        {
            foreach (Neuron_TH nr in neuronsList)
            {
                if (nr.Id == 0)
                {
                    if (type == 2)
                        nr.AddIncomingLinks(n);
                }
                else
                    nr.AddIncomingLinks(n);
            }
        }

        public List<double> Result(List<double> input)
        {
            List<double> r = new List<double>();

            if (type != 2)
            {
                r.Add(1);
                for (int i = 1; i < neuronsCount; i++)
                    r.Add(neuronsList[i].Res(input));
            }
            else
            {
                for (int i = 0; i < neuronsCount; i++)
                    r.Add(neuronsList[i].Res(input));
            }

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

    class Neuron_TH : INeurons
    {
        List<Link> incomingLinksList = new List<Link>();
        int incomingLinksCount = 0;
        int type = 1;
        double result, sum = 1;
        int id;

        public Neuron_TH(int id)
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
            sum = 0;
            for (int i = 0; i < input.Count; i++)
                sum += input[i] * incomingLinksList[i].Weight;
            result = 2.0 / (1 + Math.Exp(-2 * sum)) - 1.0;
            return result;
        }

        public double Derivate(double x)
        { 
            return  1.0 - (x * x);
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
