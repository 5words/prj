﻿using System;
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
using Emgu.Util;
using Emgu.CV.Structure;
using Emgu.CV.CvEnum;

namespace Numplate
{
    public partial class Form1 : Form
    {
        
        public Form1()
        {
            InitializeComponent();
        }
        //byte[] picturebytes;
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
                Image<Gray, Byte> Gr_img = image.Convert<Gray, Byte>();
                imageBox1.Image = Gr_img;

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
                imageBox1.Image = a.Sobel(1, 0, 3).AddWeighted(a.Sobel(0, 1, 3), 0.5, 0.5, 0);

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
