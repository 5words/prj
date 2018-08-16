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
        static float m_error = 0.9f;
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
            float r = (float)mr.Size.Width / (float)mr.Size.Height;
            if (r < 1) r = (float)mr.Size.Height / (float)mr.Size.Width;
            if ((area < min || area > max) || (r < rmin || r > rmax))
                return false;
            else
                return true;
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
                imageBox1.Image = a.Sobel(1, 0, 3).AddWeighted(a.Sobel(0, 1, 3), 0.5, 0.5, 0);
                //imageBox1.Image=image.Sobel(2,0,3);

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
                if (!verifySize(rrec))
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
