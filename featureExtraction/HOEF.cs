using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;


namespace featureExtraction
{
    public class HOEF
    {
        public float[]  Apply(Bitmap segmentImage)
        {

            var copiedImage = (Bitmap)segmentImage.Clone();

            //number of blocks in x and y directions
            int N = 6;
            int M = 6;

            //find height and width of each block
            int width = copiedImage.Width / N;
            int height = copiedImage.Height / M;

            //count of pixels in each block
            int count;

            //to store the faeture of each block
            List<float> featureVector = new List<float>();

            //used to find the sum of each block pixel to normalize
            int wholeImageCount = 0;


            //run over all 36 blocks of the image
            for (int i = 0; i < N; i++)
            {
                for (int j = 0; j < M; j++)
                {
                    count = 0;
                    //run over all the pixels in a block
                    for (int k = i * width; k < (i + 1) * width; k++)
                    {
                        for (int l = j * height; l < (j + 1) * height; l++)
                        {
                            Color c = copiedImage.GetPixel(k, l);
                            if (c.R == 255 && c.G == 255 && c.B == 255)
                            {
                                count++;
                            }
                        }
                    }
                    wholeImageCount += count;
                    featureVector.Add(count);
                }
            }

            for (int i = 0; i < featureVector.Count; i++)
            {
                featureVector[i] /= wholeImageCount;
            }
            return featureVector.ToArray();
        }
    }
}
