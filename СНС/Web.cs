using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;

namespace СНС
{
    public class Web
    {
        private List<ILayers> layersList = new List<ILayers>();
        public int layersCount = 0;
        double al = 0.01;
        double eps = 100, eps0 = 0;

        public Web()
        {
        }


        private void ClearDlt()
        {
            foreach (ILayers lr in layersList)
                foreach (INeurons nr in lr.NeuronsList)
                    foreach (Link lk in nr.IncomingLinksList)
                        lk.dlt = 0;
        }

        public void TeachWithConjugateGradients(List<List<string>> teacher, int batchsize)
        {
            Random rd = new Random();
            int epochCount = 0;

            for (int z = 0; z < 1; z++)
            {
                while (eps > 0.0001)
                {
                    epochCount++;

                    System.Diagnostics.Stopwatch timer = new System.Diagnostics.Stopwatch();
                    timer.Start();
                    for (int v = 0; v < teacher.Count / batchsize; v++)
                    {
                        List<List<string>> rd_t = new List<List<string>>();
                        rd_t = teacher;
                        //for (int i = 0; i < batchsize; i++)
                        //{
                        //    int p1 = rd.Next(0, teacher.Count);
                        //    rd_t.Add(teacher[p1]);
                        //}


                        eps = 0;
                        for (int i = 0; i < rd_t.Count; i++)
                        {
                            for (int j = 1; j < rd_t[i].Count; j++)
                            {
                                //Random rd_drop = new Random();
                                //for (int l = 1; l < layersCount - 1; l++)
                                //{
                                //    for (int nrn = 1; nrn < layersList[l].NeuronsCount; nrn++)
                                //    {
                                //        double chanse = rd_drop.NextDouble();
                                //        if (chanse < 0.5)
                                //            layersList[l].NeuronsList[nrn].IsDropouted = true;
                                //    }
                                //}
                                //this.SaveToFile("C://1/SNS/Weight.txt");
                                //Считываем изображение
                                List<double> input = new List<double>();
                                string[] sr = rd_t[i][j].Split(' ');
                                for (int l = 0; l < sr.Length; l++)
                                    input.Add(double.Parse(sr[l]));
                                //Записываем все результаты нейронов
                                List<List<double>> results = new List<List<double>>();
                                results.Add(layersList[0].Result(input));

                                for (int l = 1; l < layersCount; l++)
                                    results.Add(layersList[l].Result(results[l - 1]));

                                //Эталонный результат
                                string[] stndrt = rd_t[i][0].Split(' ');

                                var mistake = CalculateMistake(stndrt, results);


                                List<Matrix<double>> grads0 = new List<Matrix<double>>();
                                List<Matrix<double>> grads1 = new List<Matrix<double>>();
                                List<Matrix<double>> p = new List<Matrix<double>>();
                                List<double> norm = new List<double>();

                                List<double> bt = new List<double>();

                                int kk = 0;
                                for (int l = 1; l < layersCount; l++)
                                {
                                    double nm = 0;
                                    var r = Matrix<double>.Build.Dense(layersList[l].NeuronsCount, 1);
                                    for (int m = 0; m < layersList[l].NeuronsCount; m++)
                                    {
                                        nm += Math.Pow(mistake[l - 1][m], 2);
                                        r[m, 0] = mistake[l - 1][m];
                                    }
                                    nm = Math.Pow(nm, 0.5);
                                    if (nm == 0)
                                        norm.Add(0.000001);
                                    else
                                        norm.Add(nm);
                                    
                                    grads0.Add(r);
                                }

                                for (int l = 1; l < layersCount; l++)
                                    p.Add(-grads0[l - 1] / norm[l - 1]);

                                while (eps0 > Math.Pow(10, -2))
                                {
                                    double alp = 0;
                                    //for (int l = 1; l < layersCount; l++)
                                    //{
                                    double a = 0, b = 0.1, fi = 1.618;

                                    double x1 = b - (b - a) / fi, x2 = a + (b - a) / fi, E1 = 0, E2 = 0;

                                    List<List<List<double>>> oldWeights = new List<List<List<double>>>();

                                    for (int l = 1; l < layersCount; l++)
                                    {
                                        List<List<double>> temp0 = new List<List<double>>();
                                        for (int nn = 0; nn < layersList[l].NeuronsCount; nn++)
                                        {
                                            List<double> temp1 = new List<double>();
                                            for (int lk = 0; lk < layersList[l].NeuronsList[nn].IncomingLinksCount; lk++)
                                            {
                                                temp1.Add(layersList[l].NeuronsList[nn].IncomingLinksList[lk].Weight);
                                                layersList[l].NeuronsList[nn].IncomingLinksList[lk].Weight += x1 * p[l - 1][nn, 0];
                                            }
                                            temp0.Add(temp1);
                                        }
                                        oldWeights.Add(temp0);
                                    }

                                    while (Math.Abs(b - a) > Math.Pow(10, -2))
                                    {
                                        results.Clear();

                                        results.Add(layersList[0].Result(input));
                                        for (int ll = 1; ll < layersCount; ll++)
                                            results.Add(layersList[ll].Result(results[ll - 1]));

                                        CalculateMistake(stndrt, results);
                                        double mst1 = eps0;

                                        for (int l = 1; l < layersCount; l++)
                                        {
                                            for (int nn = 0; nn < layersList[l].NeuronsCount; nn++)
                                            {
                                                for (int lk = 0; lk < layersList[l].NeuronsList[nn].IncomingLinksCount; lk++)
                                                    layersList[l].NeuronsList[nn].IncomingLinksList[lk].Weight = oldWeights[l-1][nn][lk] + x2 * p[l - 1][nn, 0];
                                            }
                                        }

                                        results.Clear();

                                        results.Add(layersList[0].Result(input));
                                        for (int ll = 1; ll < layersCount; ll++)
                                            results.Add(layersList[ll].Result(results[ll - 1]));

                                        CalculateMistake(stndrt, results);
                                        var mst2 = eps0;

                                        if (mst1 >= mst2)
                                        {
                                            a = x1;
                                            x1 = x2;
                                            x2 = a + (b - a) / fi;
                                        }
                                        else
                                        {
                                            b = x2;
                                            x2 = x1;
                                            x1 = b - (b - a) / fi;
                                        }
                                    }
                                    alp = (a + b) / 2;

                                    for (int l = 1; l < layersCount; l++)
                                    {
                                        for (int m = 0; m < layersList[l].NeuronsCount; m++)
                                        {
                                            for (int k = 0; k < layersList[l].NeuronsList[m].IncomingLinksCount; k++)
                                            {
                                                layersList[l].NeuronsList[m].IncomingLinksList[k].dlt = alp * p[l - 1][m, 0];
                                                layersList[l].NeuronsList[m].IncomingLinksList[k].Weight = oldWeights[l-1][m][k] - layersList[l].NeuronsList[m].IncomingLinksList[k].dlt;
                                            }
                                        }
                                    }
                                }

                                results.Clear();

                                results.Add(layersList[0].Result(input));
                                for (int ll = 1; ll < layersCount; ll++)
                                    results.Add(layersList[ll].Result(results[ll - 1]));

                                CalculateMistake(stndrt, results);

                                if (eps0 > Math.Pow(10, -2))
                                {
                                    kk++;
                                    grads1.Clear();
                                    norm.Clear();
                                    for (int l = 1; l < layersCount; l++)
                                    {
                                        var r = Matrix<double>.Build.Dense(layersList[l].NeuronsCount, 1);
                                        for (int m = 0; m < layersList[l].NeuronsCount; m++)
                                            r[m, 0] = mistake[l - 1][m];
                                        grads1.Add(r);
                                    }

                                    for (int l = 1; l < layersCount; l++)
                                    {
                                        var temp0 = (grads0[l - 1].Transpose() * grads0[l - 1]);
                                        var temp1 = (grads1[l - 1].Transpose() * grads1[l - 1]);
                                        bt.Add(temp0[0, 0] / temp1[0, 0]);

                                        p[l - 1] = -(grads1[l - 1] + bt.Last() * p[l - 1]) / (-grads1[l - 1] + bt.Last() * p[l - 1]).FrobeniusNorm();
                                    }

                                    grads0 = grads1;
                                    grads1.Clear();
                                }
                            }
                            //}
                        }
                    }
                    timer.Stop();

                    eps /= teacher.Count;

                    System.IO.StreamWriter sw = new System.IO.StreamWriter(@"C://1/SNS/eps.txt", true);
                    sw.WriteLine(epochCount + " \t " + eps + " \t " + timer.Elapsed);
                    sw.Close();

                    this.SaveToFile("C://1/SNS/Weight.txt");

                    if (double.IsNaN(eps))
                        eps++;
                }

            }
        }

        public void TeachBackPropagation(List<List<string>> teacher, int batchsize)
        {
            Random rd = new Random();
            int epochCount = 0;

            for (int z = 0; z < 1; z++)
            {
                while (eps > 0.0001)
                {
                    epochCount++;

                    System.Diagnostics.Stopwatch timer = new System.Diagnostics.Stopwatch();
                    timer.Start();
                    for (int v = 0; v < teacher.Count / batchsize; v++)
                    {
                        List<List<string>> rd_t = new List<List<string>>();

                        for (int i = 0; i < batchsize; i++)
                        {
                            int p1 = rd.Next(0, teacher.Count);
                            rd_t.Add(teacher[p1]);
                        }

                        eps = 0;
                        for (int i = 0; i < rd_t.Count; i++)
                        {
                            for (int j = 1; j < rd_t[i].Count; j++)
                            {
                                //Random rd_drop = new Random();
                                //for(int l  = 1; l < layersCount -1; l++)
                                //{ 
                                //    for(int nrn = 1; nrn < layersList[l].NeuronsCount; nrn++)
                                //    { 
                                //        double chanse = rd_drop.NextDouble();
                                //        if (chanse < 0.5)
                                //            layersList[l].NeuronsList[nrn].IsDropouted = true;
                                //    }
                                //}

                                //Считываем изображение
                                List<double> input = new List<double>();
                                string[] sr = rd_t[i][j].Split(' ');
                                for (int l = 0; l < sr.Length; l++)
                                    input.Add(double.Parse(sr[l]));
                                //Записываем все результаты нейронов
                                List<List<double>> results = new List<List<double>>();
                                results.Add(layersList[0].Result(input));

                                for (int l = 1; l < layersCount; l++)
                                    results.Add(layersList[l].Result(results[l - 1]));

                                //Эталонный результат
                                string[] stndrt = rd_t[i][0].Split(' ');

                                var mistake = CalculateMistake(stndrt, results);

                                //Изменяем веса всех связей
                                for (int l = 1; l < layersCount; l++)
                                {
                                    for (int m = 1; m < layersList[l].NeuronsCount; m++)
                                    {
                                        for (int k = 0; k < layersList[l].NeuronsList[m].IncomingLinksCount; k++)
                                        {
                                            if (k == 0)
                                                layersList[l].NeuronsList[m].IncomingLinksList[k].dlt = al * mistake[l - 1][m];
                                            else
                                                layersList[l].NeuronsList[m].IncomingLinksList[k].dlt = al * mistake[l - 1][m]
                                                    * layersList[l].NeuronsList[m].IncomingLinksList[k].Out.Result;

                                            layersList[l].NeuronsList[m].IncomingLinksList[k].Weight += layersList[l].NeuronsList[m].IncomingLinksList[k].dlt;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    timer.Stop();

                    eps /= teacher.Count;

                    System.IO.StreamWriter sw = new System.IO.StreamWriter(@"C://1/SNS/eps.txt", true);
                    sw.WriteLine(epochCount + " \t " + eps + " \t " + timer.Elapsed);
                    sw.Close();

                    this.SaveToFile("C://1/SNS/Weight.txt");

                   if (double.IsNaN(eps))
                        eps++;
                }

            }

        }

        public List<List<double>> CalculateMistake(string[] stndrt, List<List<double>> results)
        {
            List<List<double>> mistake = new List<List<double>>();
            eps0 = 0;
            //Ошибка для последнего слоя
            List<double> lastLayerMistake = new List<double>();
            for (int l = 0; l < layersList[layersCount - 1].NeuronsCount; l++)
            {
                double bt = 0;
                switch (layersList[layersCount - 1].NeuronsList[0].Type)
                {
                    case 1://Сигмоид
                        bt = (double.Parse(stndrt[l]) - results[layersCount - 1][l])
                         * layersList[layersCount - 1].NeuronsList[l].Derivate(layersList[layersCount - 1].NeuronsList[l].Result);
                        eps0 += 0.5 * Math.Pow(double.Parse(stndrt[l]) - results[layersCount - 1][l], 2);
                        eps += 0.5 * Math.Pow(double.Parse(stndrt[l]) - results[layersCount - 1][l], 2);
                        break;
                    case 3://Softmax
                        bt = -1.0 * (results[layersCount - 1][l] - double.Parse(stndrt[l]));
                        //bt = -1.0 * Math.Log(results[layersCount - 1][l]) * double.Parse(stndrt[l]);
                        eps0 += -1.0 * Math.Log(results[layersCount - 1][l]) * double.Parse(stndrt[l]);
                        eps += -1.0 * Math.Log(results[layersCount - 1][l]) * double.Parse(stndrt[l]);
                        break;
                    case 4: //TH
                        bt = Math.Abs(double.Parse(stndrt[l]) - results[layersCount - 1][l])
                         * layersList[layersCount - 1].NeuronsList[l].Derivate(layersList[layersCount - 1].NeuronsList[l].Sum);
                        eps0 += 0.5 * Math.Pow(double.Parse(stndrt[l]) - results[layersCount - 1][l], 2);
                        eps += 0.5 * Math.Pow(double.Parse(stndrt[l]) - results[layersCount - 1][l], 2);
                        break;
                }
                lastLayerMistake.Add(bt);
            }
            mistake.Add(lastLayerMistake);

            //Ошибка для остальных слоев, начиная от предпоследнего
            for (int l = layersCount - 2; l > -1; l--)
            {
                List<double> db = new List<double>();
                // Cчитаем сумму входяших ошибок в слой l
                List<double> reverssum = this.ReversSum(l, mistake[mistake.Count - 1]);
                if (l != 0)
                {
                    for (int m = 0; m < layersList[l].NeuronsCount; m++)
                    {
                        //double o = results[l][m] * (1 - results[l][m]) * reverssum[m];
                        double o = reverssum[m] * layersList[l].NeuronsList[m].Derivate(layersList[l].NeuronsList[m].Result);
                        db.Add(o);
                    }
                    mistake.Add(db);
                }
            }
            //переворачиваем массив ошибок
            mistake.Reverse();

            return mistake;
        }

        private List<double> ReversSum(int n, List<double> input)
        {
            List<double> resullt = new List<double>();

            //Проходим по всем нейронам слоя n
            for (int i = 0; i < layersList[n].NeuronsCount; i++)
            {
                double sum = 0;
                //Проходим по всем нейронам слоя n+1
                if (n == layersCount - 2)
                {
                    for (int j = 0; j < layersList[n + 1].NeuronsCount; j++)
                    {
                        //у каждого нейрона слоя n+1 берем i-ую связь и считаем их взвешенную сумму
                        sum += input[j] * layersList[n + 1].NeuronsList[j].IncomingLinksList[i].Weight;
                    }
                }
                else
                {
                    for (int j = 1; j < layersList[n + 1].NeuronsCount; j++)
                    {
                        //у каждого нейрона слоя n+1 берем i-ую связь и считаем их взвешенную сумму
                        sum += input[j] * layersList[n + 1].NeuronsList[j].IncomingLinksList[i].Weight;
                    }
                }
                resullt.Add(sum);
            }

            return resullt;
        }

        public List<double> Res(List<double> input)
        {
            List<double> res = input;

            foreach (ILayers lr in layersList)
                res = lr.Result(res);

            return res;
        }

        public void SetAllLinks()
        {
            for (int i = 1; i < layersCount; i++)
            {
                if (layersList[i].NeuronsList[1].IncomingLinksCount == 0)
                    layersList[i].AddLinks(layersList[i - 1].NeuronsCount);

                if (i == layersCount - 1)
                {
                    for (int j = 0; j < layersList[i].NeuronsCount; j++)
                    {
                        for (int k = 0; k < layersList[i].NeuronsList[j].IncomingLinksCount; k++)
                        {
                            layersList[i].NeuronsList[j].IncomingLinksList[k].Out = layersList[i - 1].NeuronsList[k];
                        }
                    }
                }
                else
                {
                    for (int j = 1; j < layersList[i].NeuronsCount; j++)
                    {
                        for (int k = 0; k < layersList[i].NeuronsList[j].IncomingLinksCount; k++)
                        {
                            layersList[i].NeuronsList[j].IncomingLinksList[k].Out = layersList[i - 1].NeuronsList[k];
                        }
                    }
                }
            }
        }

        public void RandomizeWeights()
        {
            Random rd = new Random();

            for (int i = 1; i < layersCount; i++)
            {
                List<List<double>> t = new List<List<double>>();

                foreach (INeurons nr in layersList[i].NeuronsList)
                {
                    List<double> t1 = new List<double>();

                    foreach (Link lk in nr.IncomingLinksList)
                        t1.Add(RandomDouble(rd, -0.5, 0.5));

                    t.Add(t1);
                }

                layersList[i].SetWeights(t);
            }
        }

        private double RandomDouble(Random rd, double min, double max)
        {
            return rd.NextDouble() * (max - min) + min;
        }

        public void AddInputLayer(int n)
        {
            Layer_Input lr_i = new Layer_Input(n);
            layersList.Add(lr_i);
            layersCount++;
        }

        public void AddSigmoidLayer(int n, int t)
        {
            Layer_Sigmoid lr_s = new Layer_Sigmoid(n, t);
            layersList.Add(lr_s);
            layersCount++;
        }

        public void AddReluLayer(int n)
        {
            Layer_Relu lr_r = new Layer_Relu(n);
            layersList.Add(lr_r);
            layersCount++;
        }

        public void AddSoftmaxLayer(int n)
        {
            Layer_Softmax lr_s = new Layer_Softmax(n);
            layersList.Add(lr_s);
            layersCount++;
        }

        public void AddTHLayer(int n, int t)
        {
            Layer_TH lr_TH = new Layer_TH(n, t);
            layersList.Add(lr_TH);
            layersCount++;
        }

        public void ReadWeightsFromFile(string path)
        {
            layersList.Clear();
            layersCount = 0;
            string[] input = System.IO.File.ReadAllLines(@path);

            int na = input.Count(i => i == ">") + 1;
            List<string> s = input.ToList();
            int p = 1;

            int m = int.Parse(input[0]);
            this.AddInputLayer(m);

            for (int i = 1; i < na; i++)
            {
                int n = s.IndexOf(">", p + 1);
                int t = int.Parse(s[p + 1]);
                var s1 = s.GetRange(p + 2, n - p - 2);

                switch (t)
                {
                    case 1:
                        if (i == na - 1)
                            this.AddSigmoidLayer(n - p - 2, 2);
                        else
                            this.AddSigmoidLayer(n - p - 2, 1);
                        break;
                    case 2:
                        this.AddReluLayer(n - p - 2);
                        break;
                    case 3:
                        this.AddSoftmaxLayer(n - p - 2);
                        break;
                }

                layersList[i].SetWeights(s1.ToArray());

                p = n + 1;
            }
        }
        public void SaveToFile(string path)
        {
            System.IO.FileStream fs = new System.IO.FileStream(@path, System.IO.FileMode.Create, System.IO.FileAccess.Write);
            System.IO.StreamWriter sw = new System.IO.StreamWriter(fs);

            sw.WriteLine(Convert.ToString(layersList[0].NeuronsList.Count - 1));

            foreach (ILayers lr in layersList)
            {
                if (lr.Type != 0)
                {
                    sw.WriteLine("<");
                    sw.WriteLine(lr.NeuronsList[0].Type);
                    foreach (INeurons nrn in lr.NeuronsList)
                    {
                        if (nrn.IncomingLinksCount != 0)
                        {
                            for (int i = 0; i < nrn.IncomingLinksCount; i++)
                            {
                                if (i != nrn.IncomingLinksCount - 1)
                                    sw.Write(nrn.IncomingLinksList[i].Weight + " ");
                                else
                                    sw.Write(nrn.IncomingLinksList[i].Weight);
                            }
                            sw.WriteLine();
                        }
                    }
                    sw.WriteLine(">");
                }

            }


            sw.Close();
        }
    }
}
