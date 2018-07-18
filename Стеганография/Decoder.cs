using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Collections;

namespace WindowsFormsApplication1
{
    public partial class Decoder : Form
    {
        public Decoder()
        {
            InitializeComponent();
        }

        private void Decoder_Load(object sender, EventArgs e)
        {

        }

        private BitArray ByteToBit(byte src)
        {
            BitArray bitArray = new BitArray(8);
            bool st = false;
            for (int i = 0; i < 8; i++)
            {
                if ((src >> i & 1) == 1)
                {
                    st = true;
                }
                else st = false;
                bitArray[i] = st;
            }
            return bitArray;
        }

        private byte BitToByte(BitArray scr)
        {
            byte num = 0;
            for (int i = 0; i < scr.Count; i++)
                if (scr[i] == true)
                    num += (byte)Math.Pow(2, i);
            return num;
        }

        /*Проверяет, зашифрован ли файл,  возвраещает true, если символ в первом пикслеле равен / иначе false */
        private bool isEncryption(Bitmap scr)
        {
            byte[] rez = new byte[1];
            Color color = scr.GetPixel(0, 0);
            BitArray colorArray = ByteToBit(color.R); //получаем байт цвета и преобразуем в массив бит
            BitArray messageArray = ByteToBit(color.R); ;//инициализируем результирующий массив бит
            messageArray[0] = colorArray[0];
            messageArray[1] = colorArray[1];

            colorArray = ByteToBit(color.G);//получаем байт цвета и преобразуем в массив бит
            messageArray[2] = colorArray[0];
            messageArray[3] = colorArray[1];
            messageArray[4] = colorArray[2];

            colorArray = ByteToBit(color.B);//получаем байт цвета и преобразуем в массив бит
            messageArray[5] = colorArray[0];
            messageArray[6] = colorArray[1];
            messageArray[7] = colorArray[2];
            rez[0] = BitToByte(messageArray); //получаем байт символа, записанного в 1 пикселе
            string m = Encoding.GetEncoding(1251).GetString(rez);
            if (m == "/")
            {
                return true;
            }
            else return false;
        }

        /*Читает количество символов для дешифрования из первых бит картинки*/
        private int ReadCountText(Bitmap src)
        {
            byte[] rez = new byte[3]; //массив на 3 элемента, т.е. максимум 999 символов шифруется
            for (int i = 0; i < 3; i++)
            {
                Color color = src.GetPixel(0, i + 1); //цвет 1, 2, 3 пикселей 
                BitArray colorArray = ByteToBit(color.R); //биты цвета
                BitArray bitCount = ByteToBit(color.R); ; //инициализация результирующего массива бит
                bitCount[0] = colorArray[0];
                bitCount[1] = colorArray[1];

                colorArray = ByteToBit(color.G);
                bitCount[2] = colorArray[0];
                bitCount[3] = colorArray[1];
                bitCount[4] = colorArray[2];

                colorArray = ByteToBit(color.B);
                bitCount[5] = colorArray[0];
                bitCount[6] = colorArray[1];
                bitCount[7] = colorArray[2];
                rez[i] = BitToByte(bitCount);
            }
            string m = Encoding.GetEncoding(1251).GetString(rez);
            return Convert.ToInt32(m, 10);
        }

        /*Открыть файл для дешифрования */
        private void button1_Click(object sender, EventArgs e) {
            string FilePic;
            OpenFileDialog dPic = new OpenFileDialog();
            dPic.Filter = "Файлы изображений (*.bmp)|*.bmp|Все файлы (*.*)|*.*";
            if (dPic.ShowDialog() == DialogResult.OK)
            {
                FilePic = dPic.FileName;
            }
            else
            {
                FilePic = "";
                return;
            }

            FileStream rFile;
            try
            {
                rFile = new FileStream(FilePic, FileMode.Open); //открываем поток
            }
            catch (IOException)
            {
                MessageBox.Show("Ошибка открытия файла", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            Bitmap bPic = new Bitmap(rFile);
            if (!isEncryption(bPic))
            {
                MessageBox.Show("В файле нет зашифрованной информации", "Информация", MessageBoxButtons.OK);
                return;
            }

            int countSymbol = ReadCountText(bPic); //считали количество зашифрованных символов
            byte[] message = new byte[countSymbol];
            int index = 0;
            bool st = false;
            for (int i = 4; i < bPic.Width; i++)
            {
                for (int j = 0; j < bPic.Height; j++)
                {
                    Color pixelColor = bPic.GetPixel(i, j);
                    if (index == message.Length)
                    {
                        st = true;
                        break;
                    }
                    BitArray colorArray = ByteToBit(pixelColor.R);
                    BitArray messageArray = ByteToBit(pixelColor.R); ;
                    messageArray[0] = colorArray[0];
                    messageArray[1] = colorArray[1];

                    colorArray = ByteToBit(pixelColor.G);
                    messageArray[2] = colorArray[0];
                    messageArray[3] = colorArray[1];
                    messageArray[4] = colorArray[2];

                    colorArray = ByteToBit(pixelColor.B);
                    messageArray[5] = colorArray[0];
                    messageArray[6] = colorArray[1];
                    messageArray[7] = colorArray[2];
                    message[index] = BitToByte(messageArray);
                    index++;
                }
                if (st)
                {
                    break;
                }
            }
            string strMessage = Encoding.GetEncoding(1251).GetString(message);

            string sFileText;
            SaveFileDialog dSaveText = new SaveFileDialog();
            dSaveText.Filter = "Текстовые файлы (*.txt)|*.txt|Все файлы (*.*)|*.*";
            if (dSaveText.ShowDialog() == DialogResult.OK)
            {
                sFileText = dSaveText.FileName;
            }
            else
            {
                sFileText = "";
                return;
            };

            FileStream wFile;
            try
            {
                wFile = new FileStream(sFileText, FileMode.Create); //открываем поток на запись результатов
            }
            catch (IOException)
            {
                MessageBox.Show("Ошибка открытия файла на запись", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            StreamWriter wText = new StreamWriter(wFile, Encoding.Default);
            wText.Write(strMessage);
            MessageBox.Show("Текст записан в файл", "Информация", MessageBoxButtons.OK);
            wText.Close();
            wFile.Close(); //закрываем поток
        }
    }
}
