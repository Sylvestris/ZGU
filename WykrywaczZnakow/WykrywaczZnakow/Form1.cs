using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.Util;

namespace WykrywaczZnakow
{
    public partial class Form1 : Form
    {
        Image<Bgr, byte> imgInput;
        Image<Bgr, byte> imgSmooth;
        List<PictureBox> wykryteZnaki = new List<PictureBox>();

        int pozycja_wykrytegoZnaku_x = 1114;
        int licznik_wykrytych_znakow = 0;
        bool isZnak_1 = false;
        bool isZnak_2 = false;

        public Form1()
        {
            InitializeComponent();
            this.Size = new Size(484, 442);
            progressBar2.Maximum = 10;

        }

        public void wyczysc_program()
        {
            zdjecieGlowneBox.Image = null;
            zdjecieCannyBox.Image = null;
            zdjecieWykrytyZnak.Image = null;
            pictureBox2.Image = null;
            this.Size = new Size(484, 442);
            resetujProgressBar();

            //resetowanie tablicy wykrytych znakow
            foreach(PictureBox znak in wykryteZnaki)
            {
                znak.Image = null;
                this.Controls.Remove(znak);
                
            }
            wykryteZnaki.Clear();

            licznik_wykrytych_znakow = 0;
            isZnak_1 = false;
            isZnak_2 = false;
            pozycja_wykrytegoZnaku_x = 1114;
            label1.Text = "";
        }

        private void zwiekszProgressBar(int liczba)
        {
            progressBar2.Increment(liczba);
        }

        public void resetujProgressBar()
        {
            progressBar2.Value = 0;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="rodzaj_zdjecia"></param>
        private void ustawWykrytyZnak(int rodzaj_zdjecia)
        {
            String nazwa = "";
            Image newImage = Image.FromFile(@"C:\Users\Sylwek\Desktop\Informatyka\IV\Zaawansowana Grafika Użytkowa\ZGU - projekt zaliczeniowy\img\d1_2.png");
            const int staly_mnoznik_x = 125;

            if (rodzaj_zdjecia == 1 && !isZnak_1)
            {
                pozycja_wykrytegoZnaku_x = pozycja_wykrytegoZnaku_x - (staly_mnoznik_x * licznik_wykrytych_znakow);
                licznik_wykrytych_znakow++;

                nazwa = "D-1";
                newImage = Image.FromFile(@"C:\Users\Sylwek\Desktop\Informatyka\IV\Zaawansowana Grafika Użytkowa\ZGU - projekt zaliczeniowy\img\d1_2.png");
                isZnak_1 = true;
            }

            if (rodzaj_zdjecia == 2 && !isZnak_2)
            {
                pozycja_wykrytegoZnaku_x = pozycja_wykrytegoZnaku_x - (staly_mnoznik_x * licznik_wykrytych_znakow);
                licznik_wykrytych_znakow++;

                nazwa = "A-7";
                newImage = Image.FromFile(@"C:\Users\Sylwek\Desktop\Informatyka\IV\Zaawansowana Grafika Użytkowa\ZGU - projekt zaliczeniowy\img\Znak_A-7.jpg");
                isZnak_2 = true;
            }

            if (nazwa == "")
                return;

            PictureBox picture = new PictureBox
            {
                Name = nazwa,
                Size = new Size(110, 110),
                Location = new Point(pozycja_wykrytegoZnaku_x, 438),
                Image = newImage,
                SizeMode = PictureBoxSizeMode.StretchImage
            };
            wykryteZnaki.Add(picture);
        }

        private void wykryjZnaki()
        {
            //lista trójkątów
            List<Triangle2DF> triangleList = new List<Triangle2DF>();

            //lista prostokątów i kwadratów
            List<RotatedRect> boxList = new List<RotatedRect>();

            zwiekszProgressBar(1);

            Image<Gray, byte> canny_zdj = new Image<Gray, byte>(imgInput.Width, imgInput.Height, new Gray(0));
            canny_zdj = imgInput.Canny(300, 250);

            zdjecieCannyBox.Image = canny_zdj.Bitmap;
            zdjecieCannyBox.SizeMode = PictureBoxSizeMode.StretchImage;
            zwiekszProgressBar(2);

            LineSegment2D[] lines = CvInvoke.HoughLinesP(
               canny_zdj,
               1,
               Math.PI / 45.0,
               20, 
               30, 
               10); 

            Image<Gray, byte> imgOut = canny_zdj.Convert<Gray, byte>().ThresholdBinary(new Gray(50), new Gray(200));
            VectorOfVectorOfPoint contours = new VectorOfVectorOfPoint();
            Mat hier = new Mat();

            zwiekszProgressBar(1);

            imgSmooth = imgInput.PyrDown().PyrUp();
            imgSmooth._SmoothGaussian(3);
            imgOut = imgSmooth.InRange(new Bgr(0, 140, 150), new Bgr(80, 255, 255));
            imgOut = imgOut.PyrDown().PyrUp();
            imgOut._SmoothGaussian(3);

            zwiekszProgressBar(2);

            Dictionary<int, double> dict = new Dictionary<int, double>();
            CvInvoke.FindContours(imgOut, contours, null, Emgu.CV.CvEnum.RetrType.List, Emgu.CV.CvEnum.ChainApproxMethod.ChainApproxSimple);
            label1.Text = contours.Size.ToString();
            if (contours.Size > 0)
            {
                for (int i = 0; i < contours.Size; i++)
                {
                    using (VectorOfPoint contour = contours[i])
                    using (VectorOfPoint approxContour = new VectorOfPoint())
                    {
                        CvInvoke.ApproxPolyDP(contour, approxContour, CvInvoke.ArcLength(contour, true) * 0.05, true);
                        if (CvInvoke.ContourArea(approxContour, false) > 50) 
                        {
                            if (approxContour.Size == 3)
                            {
                                Point[] pts = approxContour.ToArray();
                                triangleList.Add(new Triangle2DF(
                                   pts[0],
                                   pts[1],
                                   pts[2]
                                   ));

                                if (pts[1].X > pts[0].X && pts[1].Y > pts[0].Y)
                                {
                                    ustawWykrytyZnak(2);
                                    double area = CvInvoke.ContourArea(contours[i]);
                                    dict.Add(i, area);
                                }
                            }
                            else if (approxContour.Size == 4)
                            {
                                bool isRectangle = true;
                                Point[] pts = approxContour.ToArray();
                                LineSegment2D[] edges = PointCollection.PolyLine(pts, true);

                                for (int j = 0; j < edges.Length; j++)
                                {
                                    double angle = Math.Abs(
                                       edges[(j + 1) % edges.Length].GetExteriorAngleDegree(edges[j]));
                                    if (angle < 80 || angle > 100)
                                    {
                                        isRectangle = false;
                                        break;
                                    }
                                }
                                if (isRectangle)
                                {
                                    RotatedRect rrect = CvInvoke.MinAreaRect(contours[i]);
                                    if ((rrect.Angle < -40 && rrect.Angle > -50)|| (rrect.Angle > 40 && rrect.Angle < 50))
                                    {
                                        boxList.Add(CvInvoke.MinAreaRect(approxContour));
                                        double area = CvInvoke.ContourArea(contours[i]);
                                        dict.Add(i, area);
                                        ustawWykrytyZnak(1);
                                    }
                                }
                            }
                        }
                    }
                }

            }
            zwiekszProgressBar(2);

            var item = dict.OrderByDescending(v => v.Value);

            foreach (var it in item)
            {
                int key = int.Parse(it.Key.ToString());

                Rectangle rect = CvInvoke.BoundingRectangle(contours[key]);
                CvInvoke.Rectangle(imgInput, rect, new MCvScalar(0, 0, 255), 1);
            }

            zwiekszProgressBar(2);
            pictureBox2.Image = imgInput.Bitmap;
            pictureBox2.SizeMode = PictureBoxSizeMode.StretchImage;

            Image<Bgr, Byte> lineImage = imgInput.CopyBlank();
            foreach (LineSegment2D line in lines)
            lineImage.Draw(line, new Bgr(Color.Red), 1);
            zdjecieWykrytyZnak.Image = lineImage.Bitmap;
            zdjecieWykrytyZnak.SizeMode = PictureBoxSizeMode.StretchImage;

        }

        private void otwórzToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog okno_pliku = new OpenFileDialog();

            if(okno_pliku.ShowDialog() == DialogResult.OK)
            {
                imgInput = new Image<Bgr, byte>(okno_pliku.FileName);
                zdjecieGlowneBox.Image = imgInput.Bitmap;
            }
        }

        private void plikToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void zdjecieCannyBox_Click(object sender, EventArgs e)
        {

        }

        private void zdjecieGlowneBox_Click(object sender, EventArgs e)
        {

        }

        private void btn_wczytajZdjecie_Click(object sender, EventArgs e)
        {
            wyczysc_program();
            OpenFileDialog okno_pliku = new OpenFileDialog();

            if (okno_pliku.ShowDialog() == DialogResult.OK)
            {
                imgInput = new Image<Bgr, byte>(okno_pliku.FileName);
                zdjecieGlowneBox.Image = imgInput.Bitmap;
            }
            zdjecieGlowneBox.SizeMode = PictureBoxSizeMode.StretchImage;
        }

        public void wyswietlWykryteZnaki()
        {
            label1.Text = "Wykryto znaki: ";
            string spacja;

            if (wykryteZnaki.Count < 1)
                spacja = " ";
            else
                spacja = ", ";

            int licznik = 0;
            foreach (PictureBox znak in wykryteZnaki)
            {
                licznik++;
                this.Controls.Add(znak);
                if (licznik == wykryteZnaki.Count)
                    spacja = "";
                label1.Text += znak.Name + spacja;
            }
        }

        private void btn_wykryjZnaki_Click(object sender, EventArgs e)
        {
            if (imgInput != null)
            {
                this.Size = new Size(1250, 600);
                wykryjZnaki();
                wyswietlWykryteZnaki();
            }
        }

        private void progressBar2_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}
