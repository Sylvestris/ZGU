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

        // zmienna wskazująca pozycje X wykrytego znaku
        int pozycja_wykrytegoZnaku_x = 1114;
        int licznik_wykrytych_znakow = 0;
		
		// lista wykrytych znaków
		List<PictureBox> wykryteZnaki = new List<PictureBox>();
		
		// zmienne wskazujące czy znak został wykryty
        bool isZnak_1 = false;
        bool isZnak_2 = false;

        /// <summary>
        /// Ustawienia wielkości okna aplikacji oraz progressbar'a
        /// </summary>
        public Form1()
        {
            InitializeComponent();
			
			// ustalenie rozmiaru okna
            this.Size = new Size(484, 442);
			
			// ustalenie wartości max progressbar'a
            progressBar2.Maximum = 10;
        }

        /// <summary>
        /// Funkcja zerująca program, która pozwala na wczytanie kolejnego obrazu do przetworzenia
        /// </summary>
        public void Wyczysc_program()
        {
			// czyszczenie pictureBox
            zdjecieGlowneBox.Image = null;
            zdjecieCannyBox.Image = null;
            zdjecieWykrytyZnak.Image = null;
            pictureBox2.Image = null;
			
            this.Size = new Size(484, 442);
			
			// funkcja resetująca progressBar
            ResetujProgressBar();

            // resetowanie tablicy wykrytych znakow
            foreach(PictureBox znak in wykryteZnaki)
            {
                znak.Image = null;
                this.Controls.Remove(znak);
            }
			
			// czyszczenie tablicy (listy) wykrytych znaków
            wykryteZnaki.Clear();

            licznik_wykrytych_znakow = 0;
            isZnak_1 = false;
            isZnak_2 = false;
            pozycja_wykrytegoZnaku_x = 1114;
            label1.Text = "";
        }

        /// <summary>
        /// Funkcja zwiekszająca progressBar o podaną ilość
        /// </summary>
        /// <param name="liczba"> Ilość o jaką jest zwiększany progressBar</param>
        private void ZwiekszProgressBar(int liczba)
        {
            progressBar2.Increment(liczba);
        }

        /// <summary>
        /// Funkcja resetująca progressBar
        /// </summary>
        public void ResetujProgressBar()
        {
            progressBar2.Value = 0;
        }

        /// <summary>
        /// Funkcja ustawiająca zdjęcie wykrytego znaku
        /// </summary>
        /// <param name="rodzaj_zdjecia"> Rodzaj zdjęcia</param>
        /// <returns>Zdjęcie znaku</returns>
        private void UstawWykrytyZnak(int rodzaj_zdjecia)
        {
            // parametry, z któych składana jest ścieżka do pliku ze zdjeciem wykrytych znaków
            string sciezka = Directory.GetCurrentDirectory();
            string znaki = @"\znaki\";
            string znakD1 = "d1_2.png";
            string znakA7 = "Znak_A-7.jpg";

            String nazwa = "";
            Image newImage = Image.FromFile(sciezka + znaki + znakD1);
			
			// przesunięcie kolejnego wykrytego znaku o stałą wartosc X
            const int staly_mnoznik_x = 125;

			// ustawienie znaku D-1
            if (rodzaj_zdjecia == 1 && !isZnak_1)
            {
                pozycja_wykrytegoZnaku_x = pozycja_wykrytegoZnaku_x - (staly_mnoznik_x * licznik_wykrytych_znakow);
                licznik_wykrytych_znakow++;

                nazwa = "D-1";
                newImage = Image.FromFile(sciezka + znaki + znakD1);
                isZnak_1 = true;
            }

			// ustawienie znaku A-7
            if (rodzaj_zdjecia == 2 && !isZnak_2)
            {
                pozycja_wykrytegoZnaku_x = pozycja_wykrytegoZnaku_x - (staly_mnoznik_x * licznik_wykrytych_znakow);
                licznik_wykrytych_znakow++;

                nazwa = "A-7";
                newImage = Image.FromFile(sciezka + znaki + znakA7);
                isZnak_2 = true;
            }

			// zakoncz jeśli nie przypisano nazwy znaku = znak zostal już wykryty i pokazany
            if (nazwa == "")
                return;

			// ustawinie wybranego zdjecia i dodanie do tablicy wykrytych znakow
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

        /// <summary>
        /// Funkcja wykrywająca znaki
        /// </summary>
        private void WykryjZnaki()
        {
            // lista trójkątów
            List<Triangle2DF> triangleList = new List<Triangle2DF>();

            // lista prostokątów i kwadratów
            List<RotatedRect> boxList = new List<RotatedRect>();

            ZwiekszProgressBar(1);
			
			// przetworzenie zdjecia do postaci wskazującej tylko białe kontury na czarnym tle
            Image<Gray, byte> canny_zdj = new Image<Gray, byte>(imgInput.Width, imgInput.Height, new Gray(0));
            canny_zdj = imgInput.Canny(300, 250);

			// przypisanie canny_zdj do pictureBox i rozciagniecie
            zdjecieCannyBox.Image = canny_zdj.Bitmap;
            zdjecieCannyBox.SizeMode = PictureBoxSizeMode.StretchImage;
            ZwiekszProgressBar(2);

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

            ZwiekszProgressBar(1);
			
			// wygładzenie obrazu
            imgSmooth = imgInput.PyrDown().PyrUp();
            imgSmooth._SmoothGaussian(3);
			
			// ograniczenie wykrywanych figur do odpowiedniego zakresu ze skali RGB - zółtego
            imgOut = imgSmooth.InRange(new Bgr(0, 140, 150), new Bgr(80, 255, 255));
            imgOut = imgOut.PyrDown().PyrUp();
            imgOut._SmoothGaussian(3);

            ZwiekszProgressBar(2);

            Dictionary<int, double> dict = new Dictionary<int, double>();
			
			// wyszukanie konturów spełniajacych wymogi odnośnie między innymi koloru 
            CvInvoke.FindContours(imgOut, contours, null, Emgu.CV.CvEnum.RetrType.List, Emgu.CV.CvEnum.ChainApproxMethod.ChainApproxSimple);
            label1.Text = contours.Size.ToString();
			
			// jeśli odnaleziono choćby jeden kontur
            if (contours.Size > 0)
            {
				// petla przechodząca po wszystkich wykrytych konturach
                for (int i = 0; i < contours.Size; i++)
                {
                    using (VectorOfPoint contour = contours[i])
                    using (VectorOfPoint approxContour = new VectorOfPoint())
                    {
                        CvInvoke.ApproxPolyDP(contour, approxContour, CvInvoke.ArcLength(contour, true) * 0.05, true);
						
						// filtr wielkości pola wykrytego konturu
                        if (CvInvoke.ContourArea(approxContour, false) > 50) 
                        {
							// jesli to trójkąt
                            if (approxContour.Size == 3)
                            {
								// tablica punktów i dodanie ich do tablicy trójkątów
                                Point[] pts = approxContour.ToArray();
                                triangleList.Add(new Triangle2DF(
                                   pts[0],
                                   pts[1],
                                   pts[2]
                                   ));

								// sprawdzenie, czy wykryty trojkat jest figurą obróconą jednym z wierzcholkow do dołu
                                if (pts[1].X > pts[0].X && pts[1].Y > pts[0].Y)
                                {
									// ustawienie znaku A-7
                                    UstawWykrytyZnak(2);
                                    double area = CvInvoke.ContourArea(contours[i]);
									// dodanie do tablicy (słownika) głównej
                                    dict.Add(i, area);
                                }
                            }
							
							// jesli to czworokąt
                            else if (approxContour.Size == 4)
                            {
                                bool isRectangle = true;
								
								// rozbicie figury na pojedyncze krawędzie
                                Point[] pts = approxContour.ToArray();
                                LineSegment2D[] edges = PointCollection.PolyLine(pts, true);

								// petla przechodzaca po wszystkich krawedziach
                                for (int j = 0; j < edges.Length; j++)
                                {
									// sprawdzenie wielkosci kąta miedzy sprawdzanymi krawędziami
                                    double angle = Math.Abs(
                                       edges[(j + 1) % edges.Length].GetExteriorAngleDegree(edges[j]));
									// przerwanie jeśli kąty w figurze są mniejsze niż 80 i wieksze niż 100 stopni
                                    if (angle < 80 || angle > 100)
                                    {
                                        isRectangle = false;
                                        break;
                                    }
                                }

                                if (isRectangle)
                                {
                                    RotatedRect rrect = CvInvoke.MinAreaRect(contours[i]);

                                    // ostateczne sprawdzenie, czy wykryta figura jest obrócona względem środka o wartość od 40 do 50 stopni 
                                    // znak D-1 jest obróconym kwadratem o 45 stopni względem środka	
                                    if ((rrect.Angle < -40 && rrect.Angle > -50)|| (rrect.Angle > 40 && rrect.Angle < 50))
                                    {
                                        boxList.Add(CvInvoke.MinAreaRect(approxContour));
                                        double area = CvInvoke.ContourArea(contours[i]);
                                        dict.Add(i, area);
                                        UstawWykrytyZnak(1);
                                    }
                                }
                            }
                        }
                    }
                }

            }

            ZwiekszProgressBar(2);

            var item = dict.OrderByDescending(v => v.Value);

            foreach (var it in item)
            {
                int key = int.Parse(it.Key.ToString());

				// pobranie odpowiednich konturów
                Rectangle rect = CvInvoke.BoundingRectangle(contours[key]);
				
				// narysowanie czerwonego prostokąta wokół wykrytego znaku
                CvInvoke.Rectangle(imgInput, rect, new MCvScalar(0, 0, 255), 1);
            }

            ZwiekszProgressBar(2);

            pictureBox2.Image = imgInput.Bitmap;
            pictureBox2.SizeMode = PictureBoxSizeMode.StretchImage;

			// utworzenie zdjęcia wskazującego WSZYSTKIE kontury w początkowym zdjęciu - czerowne linie
            Image<Bgr, Byte> lineImage = imgInput.CopyBlank();
            foreach (LineSegment2D line in lines)
            lineImage.Draw(line, new Bgr(Color.Red), 1);
            zdjecieWykrytyZnak.Image = lineImage.Bitmap;
            zdjecieWykrytyZnak.SizeMode = PictureBoxSizeMode.StretchImage;
        }

        /// <summary>
        /// Funkcja otwierająca ToolStripMenu
        /// </summary>
        private void OtworzToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog okno_pliku = new OpenFileDialog();

            if(okno_pliku.ShowDialog() == DialogResult.OK)
            {
                imgInput = new Image<Bgr, byte>(okno_pliku.FileName);
                zdjecieGlowneBox.Image = imgInput.Bitmap;
            }
        }

        // ewenty po kliknięciu przycisków
        private void PlikToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void ZdjecieCannyBox_Click(object sender, EventArgs e)
        {

        }

        private void ZdjecieGlowneBox_Click(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// Funkcja wczytujaca zdjecie z podanej lokalizacji
        /// </summary>
        private void Btn_wczytajZdjecie_Click(object sender, EventArgs e)
        {
            Wyczysc_program();
            OpenFileDialog okno_pliku = new OpenFileDialog();

            if (okno_pliku.ShowDialog() == DialogResult.OK)
            {
                imgInput = new Image<Bgr, byte>(okno_pliku.FileName);
                zdjecieGlowneBox.Image = imgInput.Bitmap;
            }
            zdjecieGlowneBox.SizeMode = PictureBoxSizeMode.StretchImage;
        }

        /// <summary>
        /// Funkcja wyswietlajaca wszystkie wykryte znaki
        /// </summary>
        public void WyswietlWykryteZnaki()
        {
            label1.Text = "Wykryto znaki: ";
            string spacja;

            if (wykryteZnaki.Count < 1)
                spacja = " ";
            else
                spacja = ", ";

            int licznik = 0;
			
			// pętla po wszystkich wykrytych znakach i ich wyświetlenie
            foreach (PictureBox znak in wykryteZnaki)
            {
                licznik++;
                this.Controls.Add(znak);
                if (licznik == wykryteZnaki.Count)
                    spacja = "";
                label1.Text += znak.Name + spacja;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Btn_wykryjZnaki_Click(object sender, EventArgs e)
        {
            if (imgInput != null)
            {
                this.Size = new Size(1250, 600);
                WykryjZnaki();
                WyswietlWykryteZnaki();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ProgressBar2_Click(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Label1_Click(object sender, EventArgs e)
        {

        }
    }
}
