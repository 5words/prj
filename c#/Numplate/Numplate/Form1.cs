using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.CvEnum;
using Emgu.CV.Util;



namespace Numplate
{
    public partial class Form1 : Form
    {
        static float m_error = 0.6f;
        static float m_aspect = 3.75f;
        static int m_verifyMin = 1;
        static int m_verifyMax = 24;
        
        public Form1()
        {
            InitializeComponent();
        }
        
        Image<Bgr,Byte>img;
        Image<Gray, Byte> img_gry;
        
        bool verifySize(RotatedRect mr)
        {
            float error = m_error;
            float aspect = m_aspect;
            int min = 34 * 8 * m_verifyMin;  // minimum area
            int max = 34 * 8 * m_verifyMax;  // maximum area
            float rmin = aspect - aspect * error;
            float rmax = aspect + aspect * error;
            float area = mr.Size.Height * mr.Size.Width;
            float r = mr.Size.Width / mr.Size.Height;
            if (r < 1)
            {
                r = mr.Size.Height / mr.Size.Width;
            }

            if ((area < min || area > max) || (r < rmin || r > rmax))
                return false;
            else
                return true;
        }

        Mat colorMatch(Mat img,Mat match ,Color r,bool adaptive_minsv)
        {
            float max_sv = 255;
            float minref_sv = 64;
            float minabs_sv = 95;
            int min_blue = 100;
            int max_blue = 140;
            int min_yellow = 15;
            int max_yellow = 40;
            int min_white = 0;
            int max_white = 30;
            Mat img_hsv = img;
            CvInvoke.CvtColor(img, img_hsv, ColorConversion.Bgr2Hsv,0);
            VectorOfMat hsvSplit= null;
            CvInvoke.Split(img_hsv,hsvSplit);
            CvInvoke.EqualizeHist(hsvSplit[2], hsvSplit[2]);
            CvInvoke.Merge(hsvSplit,img_hsv);
            

            int min_h = 0;
            int max_h = 0;
            if(r == Color.Blue)
            {
                min_h = min_blue;
                max_h = max_blue;
            }
            else if(r == Color.Yellow)
            {
                min_h = min_yellow;
                max_h = max_yellow;
            }
            else if (r == Color.White)
            {
                min_h = min_white;
                max_h = max_white;
            }

            float diff_h = (float)(max_h - min_h) / 2;
            float avg_h = min_h + diff_h;

            int channels = img_hsv.NumberOfChannels;
            int nRows = img_hsv.Rows;

            int nCols = img_hsv.Cols * channels;
            if (img_hsv.IsContinuous)
            {
                nCols *= nRows;
                nRows = 1;
            }
            unsafe
            {
                byte* p;

                int i, j;

                float s_all = 0;
                float v_all = 0;
                float count = 0;
                for (i = 0; i < nRows; i++)
                {
                    p = (byte*)img_hsv.Ptr.ToPointer();
                    for (j = 0; j < nCols; j += 3)
                    {
                        int H = p[j];
                        int S = p[j + 1];
                        int V = p[j + 2];

                        s_all += S;
                        v_all += V;
                        count++;

                        bool colorMatched = false;
                        if (H > min_h && H < max_h)
                        {
                            float Hdiff = 0;
                            if (H > avg_h)
                                Hdiff = H - avg_h;
                            else
                                Hdiff = avg_h - H;

                            float Hdiff_p = Hdiff / diff_h;

                            float min_sv = 0;
                            if (true == adaptive_minsv)
                                min_sv = minref_sv - minref_sv / 2 * (1 - Hdiff_p);
                            else
                                min_sv = minabs_sv;

                            if ((S > min_sv && min_sv < max_sv) && (V > min_sv && V < max_sv))
                                colorMatched = true;

                        }
                        if(colorMatched == true)
                        {
                            p[j] = 0;
                            p[j + 1] = 0;
                            p[j + 2] = 255;

                        }
                        else
                        {
                            p[j] = 0;
                            p[j + 1] = 0;
                            p[j + 2] = 0;
                        }
                    }

                }

                Mat img_gry;
                VectorOfMat hsvSplit_done=null;
                CvInvoke.Split(img_hsv,hsvSplit_done);
                img_gry = hsvSplit_done[2];
                
                match = img_gry;

                return img_gry;
                
            }



        }

        


        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofdOpenLocalImage = new OpenFileDialog();
            ofdOpenLocalImage.InitialDirectory = @"C:\Users\Public\Pictures"; //默认路径
            ofdOpenLocalImage.Title = "选择图片";
            ofdOpenLocalImage.Multiselect = true;
            ofdOpenLocalImage.Filter = "jpg图片|*.jpg|jpeg图片|*.jpeg|bmp图片|*.bmp|png图片|*.png|pdf文件|*.pdf";
            ofdOpenLocalImage.CheckFileExists = true;
            ofdOpenLocalImage.Multiselect = false;
            if (ofdOpenLocalImage.ShowDialog(this) == DialogResult.OK)
            {
                Image<Bgr, byte> image = new Image<Bgr, byte>(ofdOpenLocalImage.FileName);
                imageBox1.Image = image;
                img = image;
            }
            else
            {
                MessageBox.Show("未选择图片");
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //Image<Bgr, Byte> image = new Image<Bgr, byte>(@"D:\src.jpg");//从文件加载图片
            //imageBox1.Image = image;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (imageBox1.Image != null)
            {
                var image = (Image<Bgr, Byte>)imageBox1.Image;
                imageBox1.Image = image.SmoothGaussian(5);

            }
            else
            {
                MessageBox.Show("请先导入图片");
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (imageBox1.Image != null)
            {
                var image = (Image<Bgr, Byte>)imageBox1.Image;
                Image<Gray, Byte> Gr_img = image.Convert<Gray, byte>();
                imageBox1.Image = Gr_img;
                img_gry = Gr_img;
            }
            else
            {
                MessageBox.Show("请先导入图片");
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (imageBox1.Image != null)
            {
                var a = (Image<Gray, Byte>)imageBox1.Image;
                //Mat grad_x, grad_y;
                //Mat abd_grad_x, abs_grad_y;
                imageBox1.Image = a.Sobel(1, 0, 3).AddWeighted(a.Sobel(0, 1, 3), 1, 0, 0);

            }
            else
            {
                MessageBox.Show("请先导入图片");
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (imageBox1.Image != null)
            {
                var image = (Image<Gray, Single>)imageBox1.Image;
                var threshImage = image.CopyBlank();
                CvInvoke.Threshold(image, threshImage, 150, 255, ThresholdType.Binary);
                imageBox1.Image = threshImage;

                    
            }
            else
            {
                MessageBox.Show("请先导入图片");
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (imageBox1.Image != null)
            {
                var image = (Image<Gray, Single>)imageBox1.Image;
                Mat struct_element = CvInvoke.GetStructuringElement(ElementShape.Rectangle, new Size(17, 3), new Point(-1, -1));
                CvInvoke.MorphologyEx(image, image, MorphOp.Close, struct_element, new Point(-1, -1), 3, BorderType.Default, new MCvScalar(0, 0, 0));
                imageBox1.Image = image;
                //CvInvoke.Imshow("Erode Image", image);
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            var image = (Image<Gray, Single>)imageBox1.Image;
            Image<Gray, Byte> image2 = image.Convert<Gray, Byte>();
            VectorOfVectorOfPoint con = new VectorOfVectorOfPoint();
            Image<Gray, byte> c = new Image<Gray, byte>(image.Width, image.Height);
            CvInvoke.FindContours(image2,con, c, RetrType.External, ChainApproxMethod.ChainApproxNone);
            Point[][] con1 = con.ToArrayOfArray();
            PointF[][] con2 = Array.ConvertAll<Point[],PointF[]>(con1, new Converter<Point[], PointF[]>(PointToPointF));
            for (int i = 0; i < con.Size; i++)
            {
                CvInvoke.DrawContours(img, con, i, new MCvScalar(255, 0, 255, 255), 2);
                RotatedRect rrec = CvInvoke.MinAreaRect(con2[i]);
                PointF[] pointfs = rrec.GetVertices();
                if (verifySize(rrec))
                {
                    for(int j = 0; j < pointfs.Length; j++)
                    {
                        CvInvoke.Line(img, new Point((int)pointfs[j].X, (int)pointfs[j].Y), new Point((int)pointfs[(j + 1) % 4].X, (int)pointfs[(j + 1) % 4].Y), new MCvScalar(0, 255, 0, 255), 4);
                    }      
                }
                
                
            }
            


            PointF[] PointToPointF(Point[] pf)
            {
                PointF[] aaa = new PointF[pf.Length];
                int num = 0;
                foreach (var point in pf)
                {
                    aaa[num].X = (int)point.X;
                    aaa[num++].Y = (int)point.Y;
                }
                return aaa;
            }


            imageBox1.Image = img;

            //Image<Bgr, byte> a = img;
            //Image<Gray, byte> b = new Image<Gray, byte>(a.Width, a.Height);
            //Image<Gray, byte> c = new Image<Gray, byte>(a.Width, a.Height);
            //Image<Bgr, byte> d = new Image<Bgr, byte>(a.Width, a.Height);
            //CvInvoke.Canny(a, b, 100, 60);
            //VectorOfVectorOfPoint con = new VectorOfVectorOfPoint();
            //CvInvoke.FindContours(b, con, c, RetrType.External, ChainApproxMethod.ChainApproxNone);
            //for (int i = 0; i < con.Size; i++)
            //    CvInvoke.DrawContours(d, con, i, new MCvScalar(255, 0, 255, 255), 2);
            //imageBox1.Image = d;
        }

        private void button8_Click(object sender, EventArgs e)
        {
           
           
        }

        private void btn_clickAll_Click(object sender, EventArgs e)
        {
            button2.PerformClick();
            button3.PerformClick();
            button4.PerformClick();
            button5.PerformClick();
            button6.PerformClick();
            button7.PerformClick();
            button8.PerformClick();
        }
    }
    
}

namespace EasyPR
{
    class CPlateLocate
    {
        public CPlateLocate()
        { 


        }

        //! 高斯模糊所用变量  
        public static int m_GaussianBlurSize = 5;

        //! 连接操作所用变量  
        protected int m_MorphSizeWidth;
        protected int m_MorphSizeHeight;

        //! verifySize所用变量  
        protected float m_error;
        protected float m_aspect;
        protected int m_verifyMin;
        protected int m_verifyMax;

        //! 角度判断所用变量  
        protected int m_angle;

    }
}
