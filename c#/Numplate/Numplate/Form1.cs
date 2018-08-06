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
        byte[] picturebytes;
        private void button1_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "*.jpg|*.JPG|*.gif|*.GIF|*.bmp|*.BMP";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string fullpath = openFileDialog1.FileName;
                FileStream fs = new FileStream(fullpath, FileMode.Open);
                picturebytes = new byte[fs.Length];
                BinaryReader br = new BinaryReader(fs);
                picturebytes = br.ReadBytes(Convert.ToInt32(fs.Length));
                MemoryStream ms = new MemoryStream(picturebytes);
                Bitmap bmpt = new Bitmap(ms);
                pictureBox1.Image = bmpt;

            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Emgu.CV.Image<Bgr, byte> image = new Emgu.CV.Image<Bgr, byte>(320, 240, new Bgr(0, 255, 255));
            imageBox1.Image = image;
        }

        
    }
}
