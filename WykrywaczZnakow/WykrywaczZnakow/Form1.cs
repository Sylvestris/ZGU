using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
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
        

		//zmienna wskazująca pozycje X wykrytego znaku
        int pozycja_wykrytegoZnaku_x = 1114;
	
        int licznik_wykrytych_znakow = 0;
		
		//lista wykrytych znaków
		List<PictureBox> wykryteZnaki = new List<PictureBox>();
		
		//zmienne wskazujące czy znak został wykryty
        bool isZnak_1 = false;
        bool isZnak_2 = false;

        public Form1()
        {
            InitializeComponent();
			
			//ustalenie rozmiaru okna
            this.Size = new Size(484, 442);
			
			//ustalenie wartości max progressbar'a
            progressBar2.Maximum = 10;

        }

		/*
		Funkcja zerująca program. Pozwala na wczytanie kolejnego obrazu do przetworzenia
		*/
        public void wyczysc_program()
        {
			//czyszczenie pictureBox
            zdjecieGlowneBox.Image = null;
            zdjecieCannyBox.Image = null;
            zdjecieWykrytyZnak.Image = null;
            pictureBox2.Image = null;
			
            this.Size = new Size(484, 442);
			
			//funkcja resetująca progressBar
            resetujProgressBar();

            //resetowanie tablicy wykrytych znakow
            foreach(PictureBox znak in wykryteZnaki)
            {
                znak.Image = null;
                this.Controls.Remove(znak);
                
            }
			
			//czyszczenie tablicy wykrytych znakow
            wykryteZnaki.Clear();

            licznik_wykrytych_znakow = 0;
            isZnak_1 = false;
            isZnak_2 = false;
            pozycja_wykrytegoZnaku_x = 1114;
            label1.Text = "";
        }

		/*
		Funkcja zwiekszająca progressBar o podaną ilość
		*/
        private void zwiekszProgressBar(int liczba)
        {
            progressBar2.Increment(liczba);
        }

        public void resetujProgressBar()
        {
            progressBar2.Value = 0;
        }
        /// <summary>
        /// Wybór zdjęcia
        /// </summary>
        /// <param name="rodzaj_zdjecia"> Rodzaj zdjęcia</param>
        /// <returns>Zdjęcie znaku</returns>
		
		/*
		Funkcja ustawiająca zdj wykrytego znaku
		*/
        private void ustawWykrytyZnak(int rodzaj_zdjecia)
        {
            string sciezka = Directory.GetCurrentDirectory();
            string znaki = @"\znaki\";
            string znakD1 = "d1_2.png";
            string znakA7 = "Znak_A-7.jpg";

            String nazwa = "";
            Image newImage = Image.FromFile(sciezka + znaki + znakD1);
			
			//przesuniecie kloejnego wykrytego znaku o stala wartosc X
            const int staly_mnoznik_x = 125;

			//ustawienie znaku D-1
            if (rodzaj_zdjecia == 1 && !isZnak_1)
            {
                pozycja_wykrytegoZnaku_x = pozycja_wykrytegoZnaku_x - (staly_mnoznik_x * licznik_wykrytych_znakow);
                licznik_wykrytych_znakow++;

                nazwa = "D-1";
                newImage = Image.FromFile(sciezka + znaki + znakD1);
                isZnak_1 = true;
            }

			//ustawienie znaku A-7
            if (rodzaj_zdjecia == 2 && !isZnak_2)
            {
                pozycja_wykrytegoZnaku_x = pozycja_wykrytegoZnaku_x - (staly_mnoznik_x * licznik_wykrytych_znakow);
                licznik_wykrytych_znakow++;

                nazwa = "A-7";
                newImage = Image.FromFile(sciezka + znaki + znakA7);
                isZnak_2 = true;
            }

			//zakoncz jesli nie przypisano nazwy znaku = znak zostal juz wykryty i pokazany
            if (nazwa == "")
                return;

			//ustawinie wybranego zdjecia i dodanie do tablicy wykrytych znakow
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
			
			//przetworzenie zdjecia do postaci wskazujacej tylko biale kontury na czarnym tle
            Image<Gray, byte> canny_zdj = new Image<Gray, byte>(imgInput.Width, imgInput.Height, new Gray(0));
            canny_zdj = imgInput.Canny(300, 250);

			//przypisanie canny_zdj do pictureBox i rozciagniecie
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
			
			//wygladzenie obrazu
            imgSmooth = imgInput.PyrDown().PyrUp();
            imgSmooth._SmoothGaussian(3);
			
			//ograniczenie wykrywanych figur do odpowiedniego zakresu ze skali RGB - zoltego
            imgOut = imgSmooth.InRange(new Bgr(0, 140, 150), new Bgr(80, 255, 255));
            imgOut = imgOut.PyrDown().PyrUp();
            imgOut._SmoothGaussian(3);

            zwiekszProgressBar(2);

            Dictionary<int, double> dict = new Dictionary<int, double>();
			
			//wyszukanie konturow spelniajacych wymogi odnosnie mi.in. koloru 
            CvInvoke.FindContours(imgOut, contours, null, Emgu.CV.CvEnum.RetrType.List, Emgu.CV.CvEnum.ChainApproxMethod.ChainApproxSimple);
            label1.Text = contours.Size.ToString();
			
			//jesli odnaleziono chocby jeden kontur
            if (contours.Size > 0)
            {
				//petla przechodzaca po wszystkich wykrytych konturach
                for (int i = 0; i < contours.Size; i++)
                {
                    using (VectorOfPoint contour = contours[i])
                    using (VectorOfPoint approxContour = new VectorOfPoint())
                    {
                        CvInvoke.ApproxPolyDP(contour, approxContour, CvInvoke.ArcLength(contour, true) * 0.05, true);
						
						//filtr wielkosci pola wykrytego konturu
                        if (CvInvoke.ContourArea(approxContour, false) > 50) 
                        {
							//jesli to trójkąt
                            if (approxContour.Size == 3)
                            {
								//tablica punktow i dodanie ich do tablicy trojkatow
                                Point[] pts = approxContour.ToArray();
                                triangleList.Add(new Triangle2DF(
                                   pts[0],
                                   pts[1],
                                   pts[2]
                                   ));

								//sprawdzenie czy wykryty trojkat jest figura obróconą jednym z wierzcholkow do dolu
                                if (pts[1].X > pts[0].X && pts[1].Y > pts[0].Y)
                                {
									//ustawienie znaku A-7
                                    ustawWykrytyZnak(2);
                                    double area = CvInvoke.ContourArea(contours[i]);
									//dodanie do tablicy glownej
                                    dict.Add(i, area);
                                }
                            }
							
							//jesli to czworokat
                            else if (approxContour.Size == 4)
                            {
                                bool isRectangle = true;
								
								///rozbicie figury na pojedyncze krawedzie
                                Point[] pts = approxContour.ToArray();
                                LineSegment2D[] edges = PointCollection.PolyLine(pts, true);

								//petla przechodzaca po wszystkich krawedziach
                                for (int j = 0; j < edges.Length; j++)
                                {
									//sprawdzenie wielkosci kąta miedzy sprawdzanymi krawedziami
                                    double angle = Math.Abs(
                                       edges[(j + 1) % edges.Length].GetExteriorAngleDegree(edges[j]));
									//przerwanie jesli kąty w figurze są mniejsze niż 80 i wieksze niż 100
                                    if (angle < 80 || angle > 100)
                                    {
                                        isRectangle = false;
                                        break;
                                    }
                                }
                                if (isRectangle)
                                {
                                    RotatedRect rrect = CvInvoke.MinAreaRect(contours[i]);
									
									//ostateczne sprawdzenie czy wykryta figura jest obrocona wzgledem srodka o wartosc od 40 do 50
										//stopni - znak D-1 jest obroconym kwadratem o 45 st wzgledem srodka	
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

				//pobranie odpowiednich konturow
                Rectangle rect = CvInvoke.BoundingRectangle(contours[key]);
				
				//narysowanie czerwonego prostokata wokol wykrytego znaku
                CvInvoke.Rectangle(imgInput, rect, new MCvScalar(0, 0, 255), 1);
            }

            zwiekszProgressBar(2);
            pictureBox2.Image = imgInput.Bitmap;
            pictureBox2.SizeMode = PictureBoxSizeMode.StretchImage;

			//utworzenie zdjecia wskazujacego WSZYSTKIE kontury w poczatkowym zdjeciu - czerowne linie
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

		/*
		Funkcja wczytujaca zdjecie z podanej lokalizacji
		*/
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

		/*
		Funkcja wyswietlajaca wszystkie wykryte znaki
		*/
        public void wyswietlWykryteZnaki()
        {
            label1.Text = "Wykryto znaki: ";
            string spacja;

            if (wykryteZnaki.Count < 1)
                spacja = " ";
            else
                spacja = ", ";

            int licznik = 0;
			
			//petla po wszystkich wykrytych znakach i wsyswietlenie ich
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
