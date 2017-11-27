using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace СНС
{
    class MnistImage
    {
        public int width, height;
        public List<List<byte>> pixels = new List<List<byte>>();
        public byte label;

        public MnistImage(int width, int height, List<List<byte>> pxl, byte label)
        {
            this.width = width;
            this.height = height;
            this.label = label;

            for (int i = 0; i < height; i++)
            {
                List<byte> cols = new List<byte>();
                pixels.Add(cols);
                pixels[i].AddRange(pxl[i]);
            }
        }
    }
}
