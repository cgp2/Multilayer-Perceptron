using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Drawing;

namespace СНС
{
    public partial class MainWindow : Window
    {
        public string[] files;
        public int number = 0, hide_layers_count = 2;
        public List<double> input = new List<double>();
        public List<List<string>> teacher_list = new List<List<string>>();
        public List<double> answer = new List<double>();
        List<MnistImage> test_images = new List<MnistImage>();
        private Web wb;

        public MainWindow()
        {
            InitializeComponent();

            List<MnistImage> images = ReadMnistBase("C://1/test_images.idx3-ubyte", "C://1/test_labels.idx1-ubyte");

            for (int i = 0; i < images.Count; i++)
            {
                string st = "";

                for (int l = 0; l < images[i].height; l++)
                {
                    for (int m = 0; m < images[i].width; m++)
                    {
                        double d = 0;
                        d = (images[i].pixels[m][l] - 33.7912) / 255;
                        if ((l != 0) || (m != 0))
                            st += " " + d;
                        else
                            st += d;
                    }
                }

                string ss = "";
                for (int j = 0; j < 10; j++)
                {
                    if (j == Convert.ToInt32(images[i].label))
                    {
                        if (j != 9)
                            ss += "1 ";
                        else
                            ss += "1";

                    }
                    else
                    {
                        if (j != 9)
                            ss += "0 ";
                        else
                            ss += "0";
                    }
                }
                List<string> t = new List<string>();
                t.Add(ss);
                t.Add(st);
                teacher_list.Add(t);

            }

            //teacher_list = MakeTeacherListNum("C://1/test", "bmp");

            wb = new Web();
            //wb.ReadWeightsFromFile(@"C://1/SNS/Weight.txt");
            wb.AddInputLayer(28 * 28);
            wb.AddSigmoidLayer(120, 1);
            wb.AddSigmoidLayer(120, 1);
            wb.AddSoftmaxLayer(10);
            wb.SetAllLinks();
            wb.RandomizeWeights();
            wb.TeachBackPropagation(teacher_list, 10000);
            wb.SaveToFile(@"C://1/SNS/Weight.txt");

            test_images = ReadMnistBase("C://1/train-images.idx3-ubyte", "C://1/train-labels.idx1-ubyte");

            double errors_count = 0;
            for (int i = 0; i < test_images.Count; i++)
            {
                List<double> r = Recognize(test_images[i]);
                int res = r.IndexOf(r.Max());
                answer.Add(res);

                if (res != test_images[i].label)
                    errors_count++;
            }

            Org_image.Source = BMtoBMI(MakeBitmapFromMnist(test_images[0], 3));
            Lbl_answer.Content = test_images[0].label;
            Detect_image.Source = new BitmapImage(new Uri(@"C://1/test/" + answer[0] + ".bmp", UriKind.Absolute));

        }

        private void Btn_Next_Click(object sender, RoutedEventArgs e)
        {
            if (number == answer.Count() - 1)
                number = 0;
            else
                number++;

            Org_image.Source = BMtoBMI(MakeBitmapFromMnist(test_images[number], 3));
            Lbl_answer.Content = test_images[number].label;
            Detect_image.Source = new BitmapImage(new Uri(@"C://1/test/" + answer[number] + ".bmp", UriKind.Absolute));
        }

        private List<double> Recognize(MnistImage img)
        {
            List<double> kr = new List<double>();
            List<double> input = new List<double>();

            for (int l = 0; l < img.height; l++)
            {
                for (int m = 0; m < img.width; m++)
                {
                    if (img.pixels[m][l] > 0)
                        input.Add(1);
                    else
                        input.Add(0);
                }
            }

            kr = wb.Res(input);

            return kr;

        }

        public List<List<string>> MakeTeacherListNum(string path, string format)
        {
            files = System.IO.Directory.GetFiles(@path, "*." + format);
            List<List<string>> res = new List<List<string>>();

            for (int i = 0; i < files.Count(); i++)
            {
                string st = "";
                List<string> t = new List<string>();
                Bitmap img = System.Drawing.Image.FromFile(files[i]) as Bitmap;

                for (int l = 0; l < img.Height; l++)
                {
                    for (int m = 0; m < img.Width; m++)
                    {
                        int c = (img.GetPixel(m, l).R);
                        if (c >= 250)
                            c = 0;
                        else
                            c = 1;

                        if ((l != 0) || (m != 0))
                            st += " " + c.ToString();
                        else
                            st += c.ToString();
                    }
                }
                string[] sa = files[i].Split(new[] { path + "\\" }, StringSplitOptions.None);
                string ss = "";
                for (int j = 0; j < 10; j++)
                {
                    if (j == int.Parse(sa[1][0].ToString()))
                    {
                        if (j != 9)
                            ss += "1 ";
                        else
                            ss += "1";

                    }
                    else
                    {
                        if (j != 9)
                            ss += "0 ";
                        else
                            ss += "0";
                    }
                }
                t.Add(ss);
                t.Add(st);
                res.Add(t);
            }

            return res;
        }


        private static List<MnistImage> ReadMnistBase(string imageFilePath, string labelFilePath)
        {
            List<MnistImage> images = new List<MnistImage>();

            System.IO.FileStream fsImage = new System.IO.FileStream(imageFilePath, System.IO.FileMode.Open);
            System.IO.FileStream fsLabel = new System.IO.FileStream(labelFilePath, System.IO.FileMode.Open);
            System.IO.BinaryReader brImage = new System.IO.BinaryReader(fsImage);
            System.IO.BinaryReader brLabel = new System.IO.BinaryReader(fsLabel);

            int magic1 = ReverseByte(brImage.ReadInt32());
            int imageCount = ReverseByte(brImage.ReadInt32());
            int rowsCount = ReverseByte(brImage.ReadInt32());
            int colsCount = ReverseByte(brImage.ReadInt32());

            int magic2 = ReverseByte(brLabel.ReadInt32());
            int labesCount = ReverseByte(brLabel.ReadInt32());

            for (int n = 0; n < imageCount; n++)
            {
                List<List<byte>> bytes = new List<List<byte>>();
                for (int i = 0; i < rowsCount; i++)
                {
                    List<byte> col = new List<byte>();
                    for (int j = 0; j < colsCount; j++)
                        col.Add(brImage.ReadByte());
                    bytes.Add(col);
                }
                byte label = brLabel.ReadByte();
                MnistImage img = new MnistImage(rowsCount, colsCount, bytes, label);
                images.Add(img);
            }

            fsImage.Close();
            fsLabel.Close();
            brImage.Close();
            brLabel.Close();

            return images;
        }

        private static Bitmap MakeBitmapFromMnist(MnistImage mnsIm, int coof)
        {
            Bitmap bmI = new Bitmap(mnsIm.width * coof, mnsIm.height * coof);
            Graphics gr = Graphics.FromImage(bmI);
            for (int i = 0; i < mnsIm.width; i++)
            {
                for (int j = 0; j < mnsIm.height; j++)
                {
                    int pixelColor = 255 - mnsIm.pixels[i][j];
                    System.Drawing.Color c = System.Drawing.Color.FromArgb(pixelColor, pixelColor, pixelColor);
                    SolidBrush sb = new SolidBrush(c);
                    gr.FillRectangle(sb, j * coof, i * coof, coof, coof);
                }
            }

            return bmI;
        }

        private static BitmapImage BMtoBMI(Bitmap bitmap)
        {
            using (System.IO.MemoryStream memory = new System.IO.MemoryStream())
            {
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Png);
                memory.Position = 0;
                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memory;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
                return bitmapImage;
            }
        }

        public static int ReverseByte(int l)
        {
            byte[] bt = BitConverter.GetBytes(l);
            Array.Reverse(bt);
            return BitConverter.ToInt32(bt, 0);
        }
    }



}
