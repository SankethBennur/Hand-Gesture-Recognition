using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using AForge.Imaging.Filters;
using AForge;

namespace Segmentation
{
    public class HandSegmentation
    {
        private bool isSkin(Color c)
        {
            return (c.R > 95 && c.G > 40 && c.B > 20) &&
                (Math.Max(c.R, Math.Max(c.G, c.R)) -
                Math.Min(c.R, Math.Min(c.G, c.B))) > 15 &&
                            (Math.Abs(c.R - c.G) > 15 && c.R > c.G && c.R > c.G);
        }

        public Bitmap Apply(Bitmap originalImage)
        {

            //reduce image size so that less, bicubic resizes with less breakage 
            ResizeBicubic resizeObject = new ResizeBicubic(200, 200);
            Bitmap smallOriginalImage = resizeObject.Apply(originalImage);
            Bitmap copiedImage = (Bitmap)smallOriginalImage.Clone();

            // to get the colour of the pixel passed as parameter
            for (int x = 0; x < smallOriginalImage.Width; x++)
            {
                for (int y = 0; y < smallOriginalImage.Height; y++)
                {
                    if (!isSkin(copiedImage.GetPixel(x, y)))
                    {
                        copiedImage.SetPixel(x, y, Color.Black);
                    }
                }
            }
            copiedImage = Grayscale.CommonAlgorithms.BT709.Apply(copiedImage);
            Threshold bwObj = new Threshold(50);
            copiedImage = bwObj.Apply(copiedImage);


            //applying closing to remove small black spots(closing holes in the image) i.e dilusion followed by erosion 
            AForge.Imaging.Filters.Closing filter = new Closing();
            copiedImage = filter.Apply(copiedImage);
            //pictureBox2.Image = copiedImage;

            //extracting the biggest blob or a blob to get only the palms, here  we get the bounding box
            //bounding box is the smallest box having the image, hence we see only the palms
            ExtractBiggestBlob biggestblobObject = new ExtractBiggestBlob();
            copiedImage = biggestblobObject.Apply(copiedImage);



            //we need to get the coordinates of the bounding box
            IntPoint point = biggestblobObject.BlobPosition;

            //create a rectangle to pass to the crop class, it takes x,y,height,width
            Rectangle rect = new Rectangle(point.X, point.Y, copiedImage.Width, copiedImage.Height);

            Crop cropObject = new Crop(rect);

            //we pass the original image because that cohtains noise, we remove the background and have only palms
            Bitmap croppedImage = cropObject.Apply(smallOriginalImage);


            //we still have a lot of background which need to be removed as the background between the fingers have background
            //hence we do a logical and between original image and the cropped image with pixels having white pixel
            //this operation is called as masking
            for (int x = 0; x < copiedImage.Width; x++)
            {
                for (int y = 0; y < copiedImage.Height; y++)
                {
                    Color c = copiedImage.GetPixel(x, y);
                    if (c.R == 0 && c.G == 0 && c.B == 0)
                    {
                        croppedImage.SetPixel(x, y, Color.Black);

                    }
                }
            }


            //it takes time because each pixel is checked and the image is huge, 
            //so we need to resize, hence we do smallOriginalImage


            //we need to resize all objects to  a standard size

            croppedImage = resizeObject.Apply(croppedImage);
            //pictureBox2.Image = croppedImage;

            //convert the image to edge detected for the next step 
            croppedImage = Grayscale.CommonAlgorithms.BT709.Apply(croppedImage);
            CannyEdgeDetector cannyObj = new CannyEdgeDetector(0, 0, 1.4);
            croppedImage = cannyObj.Apply(croppedImage);
            Threshold threshObj = new Threshold(20);
            croppedImage = threshObj.Apply(croppedImage);
            return (croppedImage);
        }
    }
}
