using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Web;
using System.Collections.Specialized;
using System.Drawing;
using System.Diagnostics;
using System.Threading;
using System.Drawing.Drawing2D;
using ClassLibrary1.Model;

namespace WebRole1
{
    /// <summary>
    /// Summary description for Handler1
    /// </summary>
    public class RotatedTextHandler : ImageHandler
    {
        System.Drawing.Imaging.ImageFormat theImageFormat = System.Drawing.Imaging.ImageFormat.Jpeg; // .Png format doesn't work properly (images end up blurry), and .Gif just ends up being a black rectangle
        public RotatedTextHandler()
        {
            
            EnableClientCache = true;
            EnableServerCache = false;
            this.ContentType = theImageFormat;
        }

        static readonly object padlock = new object();

        public override ImageInfo GenerateImage(NameValueCollection parameters)
        {
            string cacheKey = "ColHeadImage" + parameters.GetHashCode();
            ImageInfo theInfo = PMCacheManagement.GetItemFromCache(cacheKey) as ImageInfo;
            //if (theInfo != null)
            //    return theInfo;

            lock (padlock)
            {

                // Trace.TraceInformation("Generating image " + parameters.ToString());
                // Add image generation logic here and return an instance of ImageInfo
                double fontSize = Convert.ToDouble(parameters["TheFontSize"]);
                string fontFamilyName = parameters["TheFontName"];
                FontFamily theFontFamily = null;
                var theFamilies = FontFamily.Families;
                foreach (var f in theFamilies)
                {
                    if (f.Name.Contains(fontFamilyName))
                    {
                        theFontFamily = f;
                        break;
                    }
                }
                using (Font myFont = new Font(theFontFamily, (float)fontSize, FontStyle.Bold, GraphicsUnit.Pixel))
                {
                    using (Graphics gra = Graphics.FromImage(new Bitmap(1, 1)))
                    {
                        string theString = parameters["TheText"];
                        double angle = Convert.ToDouble(parameters["TheAngle"]);
                        double angleRadians = angle * Math.PI / (double)180;
                        SizeF theSize = gra.MeasureString(theString, myFont);
                        if (angle == (double)90)
                        {
                            Bitmap bit = new Bitmap((int)theSize.Height + 1, (int)theSize.Width + 1);
                            using (Graphics gra2 = Graphics.FromImage(bit))
                            {
                                gra2.InterpolationMode = InterpolationMode.HighQualityBicubic;
                                gra2.SmoothingMode = SmoothingMode.HighQuality;
                                gra2.PixelOffsetMode = PixelOffsetMode.HighQuality;
                                gra2.CompositingQuality = CompositingQuality.HighQuality;

                                Color theBackground;
                                if (parameters["TheHilite"] == "1")
                                    theBackground = Color.FromArgb(129, 247, 129); // #81F781
                                else
                                    theBackground = Color.FromArgb(243, 229, 195); // #F3E5C3
                                if (theImageFormat == System.Drawing.Imaging.ImageFormat.Png)
                                    gra2.Clear(Color.Transparent); 
                                else
                                    gra2.Clear(theBackground); 
                                gra2.TranslateTransform(theSize.Height, theSize.Width);
                                gra2.RotateTransform((float)-90);
                                gra2.DrawString(theString, myFont, Brushes.Black, 0, 0 - (int)(theSize.Height * Math.Sin(angleRadians)));
                               // gra2.DrawLine(new Pen(Color.Red), 1, 1, 45, 60);
                               // gra2.DrawLine(new Pen(Color.Red), 100, 100, 50, 50);
                                ImageInfo theImageInfo = new ImageInfo(bit);
                                PMCacheManagement.AddItemToCache(cacheKey, new string[] { }, theImageInfo, new TimeSpan(0, 10, 0));
                                return theImageInfo;
                            }
                        }
                        else
                        { // This works imperfectly right now.  
                            double newHeight = (double)(theSize.Width + fontSize / 3) * Math.Sin(angleRadians);
                            double widthAdjustment = (double)fontSize / 3;
                            double newWidth = newHeight / Math.Tan(angleRadians);
                            //Bitmap bit = new Bitmap(1000, 1000);
                            Bitmap bit = new Bitmap((int)(newWidth + widthAdjustment), (int)newHeight + 1);
                            using (Graphics gra2 = Graphics.FromImage(bit))
                            {
                                gra2.Clear(Color.Transparent);
                                gra2.TranslateTransform((int)widthAdjustment, (float)newHeight);
                                gra2.RotateTransform(0 - (float)angle);
                                gra2.DrawString(theString, myFont, Brushes.Black, 0, 0 - (int)(theSize.Height * Math.Sin(angleRadians)));
                                return new ImageInfo(bit);
                            }
                        }
                    }
                }
                //gra2.DrawString(parameters["Hello"], myFont, Brushes.Black, 100, 100);
            }
        }
    }
}