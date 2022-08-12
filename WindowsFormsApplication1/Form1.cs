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
using CoreAudioApi;
using System.Media;
using WMPLib;



namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        //Variables
        string Radiourl; //Хранит в себе "Частоту" радиостанции
        string[] RadioNames; //Массив имён
        string[] RadioStations; //Массив "частот"
        string[] prevArray = new string[2]; //Промежуточный массив имён и "частот" 
        int freqmax; //Количество обнаруженых частот
        string currentstationname; // Имя текущей проигрываемой станции
        int currentstationnumber; //Номер текущей радиостанции
        bool turned_on = false; //Включено ли радио
        bool fm_mp3 = true; //Триггер режимов ФМ или МП3
        int animmode = 2; //Animation mode
        int alphabrightness = 255; // 
        int Rbrightness = 0;
        int Gbrightness = 0;
        int Bbrightness = 0;
        //мп3 плеер
        FolderBrowserDialog Fold = new FolderBrowserDialog(); //Диалог выбора папки с музыкой
        List<string> Plist = new List<string>(); //Плейлист

        int volume = 50; //Переменная громкости (по умолчанию 50)

        WindowsMediaPlayer WMP = new WindowsMediaPlayer(); //Плеер радио
        WindowsMediaPlayer mp3 = new WindowsMediaPlayer(); // Плеер МП3



        //функция переключения между режимами
        private void modeswitch(int mode)
        {
            switch (mode)
            {
                case 0:
                    if (turned_on == false)
                    {
                        //  SoundPlayer sp = new SoundPlayer;
                        //sp.Open(new Uri("указываем адрес до аудиозаписи");
                        //  sp.Play();
                        this.BackColor = Color.Red;
                        label3.Text = "Radio enabled";
                        label4.Text = "Radio station:";
                        label6.Visible = true;
                        label3.Visible = true;

                        string line;
                        int count = 0;
                        using (StreamReader fr = new StreamReader("radios.txt"))
                        {

                            while ((line = fr.ReadLine()) != null) //читаем по одной линии(строке) пока не вычитаем все из потока (пока не достигнем конца файла)
                            {

                                prevArray = line.Split(';');
                                RadioNames[count] = prevArray[0];
                                RadioStations[count] = prevArray[1];
                                count++;

                            }
                        }

                        currentstationname = RadioNames[0];
                        Radiourl = RadioStations[0];
                        WMP.URL = Radiourl;
                        WMP.controls.play();
                        currentstationnumber = 0;
                        turned_on = true;
                    }
                    else
                if (turned_on == true)
                    {
                        this.BackColor = Color.Black;
                        WMP.controls.stop();
                        label6.Visible = false;
                        label3.Visible = false;
                        turned_on = false;

                    }
                    break;
            }
        }


        private void button2_Click(object sender, EventArgs e)//Переключает радиостанцию вперёд
        {
            if (currentstationnumber != freqmax - 1)
            {
                currentstationnumber++;
                Radiourl = RadioStations[currentstationnumber];
                WMP.URL = Radiourl;
                currentstationname = RadioNames[currentstationnumber];


            }
            else
                MessageBox.Show(" последняя радиостанция");

        }

        string duration;
        bool flag = false;
        int lastcharacter;
        string currentsongduration;

        private void button1_Click_1(object sender, EventArgs e)
        {
            Animation.Start();
            if (fm_mp3 == true)
            {
                modeswitch(0);
            }
            else
                if (fm_mp3 == false)
            {
                modeswitch(1);
            }

        }


        private void button3_Click(object sender, EventArgs e)
        {
            if (currentstationnumber == 0)
            {
                MessageBox.Show(" Первая радиостанция");
            }
            else
                if (currentstationnumber != 0)
            {
                currentstationnumber--;
                Radiourl = RadioStations[currentstationnumber];
                currentstationname = RadioNames[currentstationnumber];
                WMP.URL = Radiourl;
            }
        }
        int i = 0;
        private void Form1_Load(object sender, EventArgs e)
        {
            timer1.Start();
            using (StreamReader sr = new StreamReader("radios.txt"))
            {
                String line;



                while ((line = sr.ReadLine()) != null) //читаем по одной линии(строке) пока не вычитаем все из потока (пока не достигнем конца файла)
                {
                    i++;

                }
                RadioNames = new string[i];
                RadioStations = new string[i];
                freqmax = i;
                label7.Text = i + " stations found";

            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {




            MMDeviceEnumerator VU = new MMDeviceEnumerator();
            MMDevice device = VU.GetDefaultAudioEndpoint(EDataFlow.eRender, ERole.eMultimedia);
            float peaks = (float)device.AudioMeterInformation.MasterPeakValue * 100;
            panel1.Height = (int)peaks * 5;
            panel2.Height = (int)peaks * 5;
            panel2.Top = this.Height - (int)peaks * 5;
            panel1.Top = this.Height - (int)peaks * 5;


            if ((int)peaks == 0)
            {
                label6.Text = "...SEEKING...";
            }
            else
                if ((int)peaks != 0)
            {
                label6.Text = currentstationname;
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            fm_mp3 = true;
            modeswitch(0);
        }
        int song;
        private void button5_Click(object sender, EventArgs e)
        {
            Plist.Clear();
            fm_mp3 = false;
            flag = false;
            label6.Visible = true;

            if (Fold.ShowDialog() == DialogResult.OK)
            {
                DirectoryInfo dir = new DirectoryInfo(Fold.SelectedPath);
                foreach (var item in dir.GetFiles("*.mp3"))
                {

                    Plist.Add(Fold.SelectedPath + "\\" + item.Name);
                }
                song = 0;
                mp3.URL = Plist[song];
                mp3.controls.play();

                timer2.Start();




            }


        }


        private void button9_MouseDown(object sender, MouseEventArgs e)
        {
            if (volume != 100)
            {
                volume++;
                label1.Text = "Volume: " + volume.ToString();
                WMP.settings.volume = volume;
            }
        }

        private void button8_MouseDown(object sender, MouseEventArgs e)
        {
            if (volume != 0)
            {
                volume--;
                label1.Text = "Volume: " + volume.ToString();
                WMP.settings.volume = volume;
            }
        }

        private void button9_Click(object sender, EventArgs e)
        {

        }
        int lastcharacter2;
        int lastcharacters;
        private void timer2_Tick(object sender, EventArgs e)
        {
            label5.Text = song.ToString();
            // if (flag == false) // проверяем, внесены ли изменения в информацию о песне
            //  {
            label2.Text = mp3.status;
            label4.Text = mp3.currentMedia.name;
            duration = mp3.currentMedia.durationString;
            label7.Text = duration.ToString();

            //Добавим ещё одну проверку, так как песни бывают разнве, тут нужно два варианта
            if ((int)duration[duration.Length - 1] - 48 == 0)
            {
                lastcharacter = (int)duration[duration.Length - 2] - 48;
                lastcharacter2 = (int)duration[duration.Length - 1] - 48;
                lastcharacters = lastcharacter * 10 + lastcharacter2;
                lastcharacters = lastcharacters - 2;
                duration = duration.Substring(0, duration.Length - 2);
                duration = duration + lastcharacters;
            }
            else
                if ((int)duration[duration.Length - 1] - 48 != 0)
            {
                lastcharacter = (int)duration[duration.Length - 1] - 48;
                lastcharacter = lastcharacter - 2;
                duration = duration.Substring(0, duration.Length - 1);
                duration = duration + lastcharacter;
            }


            label8.Text = duration.ToString();


            //      flag = true;
            //     }


            currentsongduration = mp3.controls.currentPositionString;
            label1.Text = currentsongduration.ToString();


            if (mp3.controls.currentPositionString == duration)
            {
                song++;
                mp3.URL = Plist[song];
                mp3.controls.play();
                flag = false;
            }

        }
        bool brtOn = false;
        bool RbrtOn = false;
        bool GbrtOn = false;
        bool BbrtOn = false;
        private void Animation_Tick(object sender, EventArgs e)
        {
            if (animmode == 1)
            {//Пока напишем простую пульсацию
                if (Rbrightness >= 0 && Rbrightness != 255 && brtOn == false)
                {
                    
                    Rbrightness += 15;
                    if (Rbrightness == 255)
                    {
                        brtOn = true;
                    }

                }
                else
                    if (Rbrightness <= 255 && Rbrightness != 0 && brtOn == true)
                {
                    Rbrightness -= 15;
                    if (Rbrightness == 0)
                    {
                        brtOn = false;
                    }
                }
            }
            else
                if (animmode == 2)
            {
                //RGBshift
                if (Rbrightness >= 0 && Bbrightness <= 255 && RbrtOn == false)
                {
                    if (Bbrightness != 0)
                    {
                        Bbrightness -= 5;
                        
                    }
                    Rbrightness += 5;
                    if (Rbrightness == 255)
                    {
                        RbrtOn = true;
                        GbrtOn = false;

                    }
                    
                }
                else
             if (Rbrightness <= 255 && Gbrightness >= 0 && GbrtOn == false)
                {
                    Rbrightness -= 5;
                    Gbrightness += 5;
                    if (Gbrightness == 255)
                    {
                        GbrtOn = true;
                        BbrtOn = false;

                    }


                }
                else
                    if (Gbrightness <= 255 && Bbrightness >= 0 && BbrtOn == false)
                {
                   
                    Gbrightness -= 5;
                    Bbrightness += 5;
                    if (Bbrightness == 255)
                    {
                        BbrtOn = true;
                        
                    }
                }
                else
                    if(GbrtOn = true && RbrtOn == true && BbrtOn == true)
                {
                    RbrtOn = false;
                }

                    





                this.BackColor = Color.FromArgb(255, Rbrightness, Gbrightness, Bbrightness); //Пульсируем синим
            }
        }
    }
}
