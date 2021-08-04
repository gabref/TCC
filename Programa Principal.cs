using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.IO.Ports;  // necessário para ter acesso as portas
using System.Drawing.Drawing2D; // necessário para os desenhos
using System.Collections;//para usar ArrayList

namespace SoftwareScara_D
{
    public partial class Principal : Form
    {

        string RxString;
        private Double degtheta1 = 0, degtheta2 = 0;
        private Double newdegtheta1, newdegtheta2;
        private const Double arm1 = 130;
        private const Double arm2 = 130;
        private const Double pi = 3.14159265358979;
        private Double z;
        private Double Xanterior = 260;
        private Double Yanterior = 0;
        private int Zanterior = 0;

        ArrayList listX = new ArrayList();
        ArrayList listY = new ArrayList();
        ArrayList listZ = new ArrayList();

        public Principal()
        {
            InitializeComponent();
            timerCOM.Enabled = true;
        }

        public void PassValue(String calX, String calY, String calZ)
        {
            txt_x.Text = calX;
            txt_y.Text = calY;
            txt_z.Text = calZ;
        }

        private void atualizaListaCOMs()
        {
            int i;
            bool quantDiferente;    //flag para sinalizar que a quantidade de portas mudou

            i = 0;
            quantDiferente = false;

            //se a quantidade de portas mudou
            if (comboBox1.Items.Count == SerialPort.GetPortNames().Length)
            {
                foreach (string s in SerialPort.GetPortNames())
                {
                    if (comboBox1.Items[i++].Equals(s) == false)
                    {
                        quantDiferente = true;
                    }
                }
            }
            else
            {
                quantDiferente = true;
            }

            //Se não foi detectado diferença
            if (quantDiferente == false)
            {
                return;                     //retorna
            }

            //limpa comboBox
            comboBox1.Items.Clear();

            //adiciona todas as COM diponíveis na lista
            foreach (string s in SerialPort.GetPortNames())
            {
                comboBox1.Items.Add(s);
            }
            //seleciona a primeira posição da lista
            comboBox1.SelectedIndex = 0;
        }

        private void timerCOM_Tick(object sender, EventArgs e)
        {
            atualizaListaCOMs();
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (serialPort1.IsOpen == true)  // se porta aberta 
                serialPort1.Close();            //fecha a porta 
        }

        private void btConectar_Click(object sender, EventArgs e)
        {
            if (serialPort1.IsOpen == false)
            {
                try
                {
                    serialPort1.PortName = comboBox1.Items[comboBox1.SelectedIndex].ToString();
                    serialPort1.Open();

                }
                catch
                {
                    return;

                }
                if (serialPort1.IsOpen)
                {
                    btConectar.Text = "Desconectar";
                    comboBox1.Enabled = false;

                }
            }
            else
            {

                try
                {
                    serialPort1.Close();
                    comboBox1.Enabled = true;
                    btConectar.Text = "Conectar";
                }
                catch
                {
                    return;
                }

            }
        }

        private void btEnviar_Click(object sender, EventArgs e)
        {
            if (serialPort1.IsOpen == true)          //porta está aberta
                serialPort1.Write("0" + textBoxEnviar.Text);  //envia o texto presente no textbox Enviar
        }

        private void serialPort1_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            RxString = serialPort1.ReadExisting();              //le o dado disponível na serial
            this.Invoke(new EventHandler(trataDadoRecebido));   //chama outra thread para escrever o dado no text box
        }

        private void trataDadoRecebido(object sender, EventArgs e)
        {
            textBoxReceber.AppendText(RxString);
        }

        private void moveTo(Double x, Double y)
        {
            Double theta1 = 0, theta2 = 0;
            if (x >= 0 && y > 0)
            {
                theta2 = Math.Acos((Math.Pow(x, 2) + Math.Pow(y, 2) - Math.Pow(arm1, 2) - Math.Pow(arm2, 2)) / (2 * arm1 * arm2));

                Double l_cos = (arm1 + arm2 * Math.Cos(theta2));
                Double l_sin = (arm2 * Math.Sin(theta2));
                
                theta1 = Math.Atan(y / x) - Math.Atan(l_sin / l_cos);
                theta2 *= (1);

                if (theta1 < 0)
                {
                    theta1 = Math.Atan(y / x) + Math.Atan(l_sin / l_cos);
                    theta2 *= (-1);
                }               
                
            }
            else if (x >= 0 && y <= 0)
            {
                theta2 = Math.Acos((Math.Pow(x, 2) + Math.Pow(y, 2) - Math.Pow(arm1, 2) - Math.Pow(arm2, 2)) / (2 * arm1 * arm2));

                Double l_cos = (arm1 + arm2 * Math.Cos(theta2));
                Double l_sin = (arm2 * Math.Sin(theta2));
                
                theta1 = Math.Atan(y / x) + Math.Atan(l_sin / l_cos);
                theta2 *= (-1);
                                
            }
            else if (x <= 0 && y > 0)
            {
                theta2 = Math.Acos(((x * x) + (y * y) - (arm1 * arm1) - Math.Pow(arm2, 2)) / (2 * arm1 * arm2));

                Double l_cos = (arm1 + arm2 * Math.Cos(theta2));
                Double l_sin = (arm2 * Math.Sin(theta2));
                
                theta1 = Math.Atan(y / x) - Math.Atan(l_sin / l_cos) + Math.PI;
                theta2 *= (1);
                                
            }
            else 
            {
                theta2 = Math.Acos((Math.Pow(x, 2) + Math.Pow(y, 2) - Math.Pow(arm1, 2) - Math.Pow(arm2, 2)) / (2 * arm1 * arm2));

                Double l_cos = (arm1 + arm2 * Math.Cos(theta2));
                Double l_sin = (arm2 * Math.Sin(theta2));
                
                theta1 = Math.Atan(y / x) - Math.Atan(l_sin / l_cos) + Math.PI;
                theta2 *= (1);
                
            }
            
            
            if (theta1 > (Math.PI) || theta1 < 0 || theta2 > (Math.PI/2) || theta2 < (-Math.PI/2))
            {
                MessageBox.Show("Este robô não alcança as coordenadas dadas por conta da limitação das juntas.\n Theta1: "+
                    + Math.Round((theta1 * 180 / pi), 3, MidpointRounding.AwayFromZero)+ "\n Theta2: " + 
                    + Math.Round((theta2 * 180 / pi), 3, MidpointRounding.AwayFromZero));
                return;
            }
            else if(Double.IsNaN(theta1) || Double.IsNaN(theta2))
            {
                MessageBox.Show("Este robô não alcança as coordenadas dadas por conta da limitação do tamanho dos braços");
            }

            newdegtheta1 = Math.Round((theta1 * 180 / Math.PI), 3, MidpointRounding.AwayFromZero);
            newdegtheta2 = Math.Round((theta2 * 180 / Math.PI), 3, MidpointRounding.AwayFromZero);

        }

        private void MoveVelocities(Double newdt1, Double newdt2)
        {
            Double deltaOne = newdt1 - degtheta1;
            Double deltaTwo = newdt2 - degtheta2;

            if (deltaOne >= 0 && deltaTwo >= 0)
            {
                if(deltaOne > deltaTwo)
                {
                    for(double i = 0; i <= deltaOne; i++)
                    {
                        degtheta1 ++;
                        if(i <= deltaTwo)
                        {
                            degtheta2 ++;
                        }
                        
                        Thread.Sleep(15);
                        picCanvas.Refresh();
                    }
                }
                else if(deltaOne < deltaTwo)
                {
                    for (Double i = 0; i <= deltaTwo; i++)
                    {
                        degtheta2 ++;
                        if (i <= deltaOne)
                        {
                            degtheta1 ++;
                        }
                        
                        Thread.Sleep(15);
                        picCanvas.Refresh();
                    }
                }
                else if(deltaOne == deltaTwo)
                {

                    for(Double i = 0; i <= deltaOne; i++)
                    {
                        degtheta1 ++;
                        degtheta2 ++;
                        
                        picCanvas.Refresh();
                        Thread.Sleep(15);
                    }
                }
            }
            else if (deltaOne >= 0 && deltaTwo < 0)
            {
                if (deltaOne > Math.Abs(deltaTwo))
                {
                    for (double i = 0; i <= deltaOne; i++)
                    {
                        degtheta1++;
                        if (i <= Math.Abs(deltaTwo))
                        {
                            degtheta2--;
                        }
                        
                        Thread.Sleep(15);
                        picCanvas.Refresh();
                    }
                }
                else if (deltaOne < Math.Abs(deltaTwo))
                {
                    for (double i = 0; i <= Math.Abs(deltaTwo); i++)
                    {
                        degtheta2--;
                        if (i <= deltaOne)
                        {
                            degtheta1++;
                        }
                        
                        Thread.Sleep(15);
                        picCanvas.Refresh();
                    }
                }
                else if (deltaOne == Math.Abs(deltaTwo))
                {
                    for (Double i = 0; i <= deltaOne; i++)
                    {
                        degtheta1++;
                        degtheta2--;
                                            
                        picCanvas.Refresh();
                        Thread.Sleep(15);
                    }
                }
            }
            else if (deltaOne <= 0 && deltaTwo >= 0)
            {
                if (Math.Abs(deltaOne) > deltaTwo)
                {
                    for (double i = 0; i <= Math.Abs(deltaOne); i++)
                    {
                        degtheta1--;
                        if (i <= deltaTwo)
                        {
                            degtheta2++;
                        }
                        
                        Thread.Sleep(15);
                        picCanvas.Refresh();
                    }
                }
                else if (Math.Abs(deltaOne) < deltaTwo)
                {
                    for (double i = 0; i <= deltaTwo; i++)
                    {
                        degtheta2++;
                        if (i <= Math.Abs(deltaOne))
                        {
                            degtheta1--;
                        }
                        
                        Thread.Sleep(15);
                        picCanvas.Refresh();
                    }
                }
                else if (Math.Abs(deltaOne) == deltaTwo)
                {
                    for (Double i = 0; i <= deltaTwo; i++)
                    {
                        degtheta1--;
                        degtheta2++;
                                               

                        picCanvas.Refresh();
                        Thread.Sleep(15);
                    }
                }
            }
            else if (deltaOne <= 0 && deltaTwo < 0)
            {
                if (Math.Abs(deltaOne) > Math.Abs(deltaTwo))
                {
                    for (double i = 0; i <= Math.Abs(deltaOne); i++)
                    {
                        degtheta1--;
                        if (i <= Math.Abs(deltaTwo))
                        {
                            degtheta2--;
                        }
                        
                        Thread.Sleep(15);
                        picCanvas.Refresh();
                    }
                }
                else if (Math.Abs(deltaOne) < Math.Abs(deltaTwo))
                {
                    for (double i = 0; i <= Math.Abs(deltaTwo); i++)
                    {
                        degtheta2--;
                        if (i <= Math.Abs(deltaOne))
                        {
                            degtheta1--;
                        }
                        
                        Thread.Sleep(15);
                        picCanvas.Refresh();
                    }
                }
                else if (Math.Abs(deltaOne) == Math.Abs(deltaTwo))
                {
                    for (Double i = 0; i <= Math.Abs(deltaOne); i++)
                    {
                        degtheta1--;
                        degtheta2--;
                                                
                        picCanvas.Refresh();
                        Thread.Sleep(15);
                    }
                }
            }


            String textoArduino = ("0" + Math.Round(degtheta1, 3).ToString() +
                                    "." + Math.Round(degtheta2, 3).ToString() + "." + z.ToString() + "." +
                                    "a");
                                    
            textBoxEnviar.Text = textoArduino;

            if (serialPort1.IsOpen == true)          //porta está aberta
                serialPort1.Write(textoArduino);  //envia o texto presente no textbox Enviar

        }

        private void MoveVelocitiesReta(Double newdt1, Double newdt2)
        {
            Double deltaOne = newdt1 - degtheta1;
            Double deltaTwo = newdt2 - degtheta2;

            if (deltaOne >= 0 && deltaTwo >= 0)
            {
                if (deltaOne > deltaTwo)
                {
                    for (double i = 0; i <= deltaOne; i++)
                    {
                        degtheta1++;
                        if (i <= deltaTwo)
                        {
                            degtheta2++;
                        }

                        Thread.Sleep(15);
                        picCanvas.Refresh();
                    }
                }
                else if (deltaOne < deltaTwo)
                {
                    for (Double i = 0; i <= deltaTwo; i++)
                    {
                        degtheta2++;
                        if (i <= deltaOne)
                        {
                            degtheta1++;
                        }

                        Thread.Sleep(15);
                        picCanvas.Refresh();
                    }
                }
                else if (deltaOne == deltaTwo)
                {

                    for (Double i = 0; i <= deltaOne; i++)
                    {
                        degtheta1++;
                        degtheta2++;

                        picCanvas.Refresh();
                        Thread.Sleep(15);
                    }
                }
            }
            else if (deltaOne >= 0 && deltaTwo < 0)
            {
                if (deltaOne > Math.Abs(deltaTwo))
                {
                    for (double i = 0; i <= deltaOne; i++)
                    {
                        degtheta1++;
                        if (i <= Math.Abs(deltaTwo))
                        {
                            degtheta2--;
                        }

                        Thread.Sleep(15);
                        picCanvas.Refresh();
                    }
                }
                else if (deltaOne < Math.Abs(deltaTwo))
                {
                    for (double i = 0; i <= Math.Abs(deltaTwo); i++)
                    {
                        degtheta2--;
                        if (i <= deltaOne)
                        {
                            degtheta1++;
                        }

                        Thread.Sleep(15);
                        picCanvas.Refresh();
                    }
                }
                else if (deltaOne == Math.Abs(deltaTwo))
                {
                    for (Double i = 0; i <= deltaOne; i++)
                    {
                        degtheta1++;
                        degtheta2--;

                        picCanvas.Refresh();
                        Thread.Sleep(15);
                    }
                }
            }
            else if (deltaOne <= 0 && deltaTwo >= 0)
            {
                if (Math.Abs(deltaOne) > deltaTwo)
                {
                    for (double i = 0; i <= Math.Abs(deltaOne); i++)
                    {
                        degtheta1--;
                        if (i <= deltaTwo)
                        {
                            degtheta2++;
                        }

                        Thread.Sleep(15);
                        picCanvas.Refresh();
                    }
                }
                else if (Math.Abs(deltaOne) < deltaTwo)
                {
                    for (double i = 0; i <= deltaTwo; i++)
                    {
                        degtheta2++;
                        if (i <= Math.Abs(deltaOne))
                        {
                            degtheta1--;
                        }

                        Thread.Sleep(15);
                        picCanvas.Refresh();
                    }
                }
                else if (Math.Abs(deltaOne) == deltaTwo)
                {
                    for (Double i = 0; i <= deltaTwo; i++)
                    {
                        degtheta1--;
                        degtheta2++;


                        picCanvas.Refresh();
                        Thread.Sleep(15);
                    }
                }
            }
            else if (deltaOne <= 0 && deltaTwo < 0)
            {
                if (Math.Abs(deltaOne) > Math.Abs(deltaTwo))
                {
                    for (double i = 0; i <= Math.Abs(deltaOne); i++)
                    {
                        degtheta1--;
                        if (i <= Math.Abs(deltaTwo))
                        {
                            degtheta2--;
                        }

                        Thread.Sleep(15);
                        picCanvas.Refresh();
                    }
                }
                else if (Math.Abs(deltaOne) < Math.Abs(deltaTwo))
                {
                    for (double i = 0; i <= Math.Abs(deltaTwo); i++)
                    {
                        degtheta2--;
                        if (i <= Math.Abs(deltaOne))
                        {
                            degtheta1--;
                        }

                        Thread.Sleep(15);
                        picCanvas.Refresh();
                    }
                }
                else if (Math.Abs(deltaOne) == Math.Abs(deltaTwo))
                {
                    for (Double i = 0; i <= Math.Abs(deltaOne); i++)
                    {
                        degtheta1--;
                        degtheta2--;

                        picCanvas.Refresh();
                        Thread.Sleep(15);
                    }
                }
            }
                      

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            lbl_theta1_.Text = "\u03b8\u2081";
            lbl_theta2_.Text = "\u03b8\u2082";
            lbl_cinematica1.Text = "\u03b8\u2081";
            lbl_cinematica2.Text = "\u03b8\u2082";
            txt_variacao.Text = "5";
            txt_fecha.Text = "25";

            lbl_x.Text = "260";
            lbl_y.Text = "0";
            lbl_z.Text = "0";
            lbl_theta1.Text = "0";
            lbl_theta2.Text = "0";
            txt_vezes.Text = "1";

            lbl_pwm1.Visible = false;
            lbl_pwm1_.Visible = false;
            lbl_pwm2.Visible = false;
            lbl_pwm2_.Visible = false;

            int posicaoBase = 315;
            int posicaoX = 800;
            int altura = posicaoBase;
            picGarra.Location = new Point(posicaoX, altura);
            
            double zoomLevel = 0.96296;
            Image img = picCanvas.Image;
            Bitmap bitMapImg = new Bitmap(img);
            Size newSize = new Size((int)(bitMapImg.Width *
                zoomLevel), (int)(bitMapImg.Height * zoomLevel));
            Bitmap bmp = new Bitmap(bitMapImg, newSize);
            picCanvas.Image = (Image)bmp;

            this.btn_cup.Visible = false;
            this.btn_cdown.Visible = false;
            this.btn_cleft.Visible = false;
            this.btn_cright.Visible = false;
            this.btn_up.Visible = false;
            this.btn_down.Visible = false;
        }                    

        private void movepicGarra(double z, int Zanterior)
        {
            int posicaoBase = 315;
            int deltaZ = Convert.ToInt32(z) - Zanterior;
            int posicaoX = 800;
            z *= (-1);//inverte sinal de z

            if (z <= 50)//confere se z é positivo e menor que 51
            {
                if(deltaZ <= 0)//move garra para baixo{
                {
                    for (double i = 0; i >= deltaZ; i--)
                    {
                        int altura = posicaoBase - Zanterior - Convert.ToInt32(i);
                        picGarra.Location = new Point(posicaoX, altura);
                        Thread.Sleep(30);
                    }
                }
                else//move garra para cima
                {
                    for(double i = 0; i <= deltaZ; i++)
                    {
                        int altura = posicaoBase - Zanterior - Convert.ToInt32(i);
                        picGarra.Location = new Point(posicaoX, altura);
                        Thread.Sleep(30);
                    }
                }
            }
            else if(z == 0)
            {
                for (double i = 0; i <= deltaZ; i++)
                {
                    int altura = posicaoBase - Zanterior - Convert.ToInt32(i);
                    picGarra.Location = new Point(posicaoX, altura);
                    Thread.Sleep(30);
                }
            }
            else if(z < 0)
            {
                MessageBox.Show("A garra não pode ter coordenadas positivas");
            }
            else if (z > 50)
            {
                MessageBox.Show("As coordenadas não podem ser menores que -50");
            }

        }    

        private void btn_ok_Click(object sender, EventArgs e)
        {
            if (rdb_absoluta.Checked)
            {
                String string_x = txt_x.Text.Trim();
                String string_y = txt_y.Text.Trim();
                String string_z = txt_z.Text.Trim();

                if(btn_traj.Text == "Trajetória")
                {
                    try
                    {
                        Double x = Math.Round(Convert.ToDouble(string_x), 3, MidpointRounding.ToEven);
                        Double y = Math.Round(Convert.ToDouble(string_y), 3, MidpointRounding.ToEven);
                        z = Math.Round(Convert.ToDouble(string_z), 3, MidpointRounding.ToEven);

                        moveTo(x, y);
                        movepicGarra(z, Zanterior);

                        lbl_theta1.Text = newdegtheta1.ToString();
                        lbl_theta2.Text = newdegtheta2.ToString();

                        lbl_x.Text = x.ToString();
                        lbl_y.Text = y.ToString();

                        if (z > 50)
                        {
                            lbl_z.Text = "50";
                        }
                        else
                        {
                            lbl_z.Text = z.ToString();
                        }

                        Int32.TryParse(lbl_z.Text, out Zanterior);

                        MoveVelocities(newdegtheta1, newdegtheta2);


                        txt_x.Clear();
                        txt_y.Clear();
                        txt_z.Clear();


                    }
                    catch (FormatException Erro)
                    {
                        MessageBox.Show("As coordenadas não foram escritas: " + Erro.Message);
                    }
                    finally
                    {
                        if (string.IsNullOrEmpty(string_x) || string.IsNullOrEmpty(string_y) || string.IsNullOrEmpty(string_z))
                        {
                            MessageBox.Show("Escreva as coordenadas!");

                            String a_x = ("-");
                            String a_y = ("-");
                            String a_z = ("-");
                            lbl_x.Text = a_x;
                            lbl_y.Text = a_y;
                            lbl_z.Text = a_z;

                        }

                    }
                }
                else
                {
                    try
                    {
                        Double x1 = Math.Round(Convert.ToDouble(string_x), 3, MidpointRounding.ToEven);
                        Double y1 = Math.Round(Convert.ToDouble(string_y), 3, MidpointRounding.ToEven);
                        z = Math.Round(Convert.ToDouble(string_z), 3, MidpointRounding.ToEven);

                        movepicGarra(z, Zanterior);
                        if (z > 50)
                        {
                            lbl_z.Text = "50";
                        }
                        else
                        {
                            lbl_z.Text = z.ToString();
                        }

                        Int32.TryParse(lbl_z.Text, out Zanterior);

                        Double x0 = Convert.ToDouble(lbl_x.Text);
                        Double y0 = Convert.ToDouble(lbl_y.Text);
                        Double m = (-1)*((y0 - y1) / (x1 - x0));
                        Double dx = x1 - x0;
                        Double a = (y0 - y1);
                        Double b = (x1 - x0);
                        Double d = Math.Abs(x0 * y1 - x1 * y0) / Math.Sqrt(Math.Pow(a, 2) + Math.Pow(b, 2));
                        if(d>= 189)
                        {
                            if (dx >= 0)
                            {
                                Double x = x0;
                                for (double i = 0; i <= dx; i += 0.5)
                                {
                                    Double yreta = (m * x) - ((x0 * y1 - x1 * y0) / (x1 - x0));
                                    moveTo(x, yreta);
                                    MoveVelocitiesReta(newdegtheta1, newdegtheta2);

                                    String textoArduino = ("0" + Math.Round(degtheta1, 3).ToString() +
                                    "." + Math.Round(degtheta2, 3).ToString() + "." + z.ToString() + "." +
                                    "p");

                                    textBoxEnviar.Text = textoArduino;
                                    if (serialPort1.IsOpen == true)          //porta está aberta
                                        serialPort1.Write(textoArduino);  //envia o texto presente no textbox Enviar

                                    Thread.Sleep(5);
                                    lbl_theta1.Text = Math.Round(newdegtheta1, 3).ToString();
                                    lbl_theta2.Text = Math.Round(newdegtheta2, 3).ToString();

                                    lbl_x.Text = x.ToString();
                                    lbl_y.Text = yreta.ToString();

                                    x += 0.5;

                                    if (newdegtheta1 > (180) || newdegtheta1 < 0 || newdegtheta2 > (90) || newdegtheta2 < (-90))
                                    {
                                        MessageBox.Show("Este robô não alcança as coordenadas dadas por conta da limitação das juntas.\n Theta1: " +
                                            +Math.Round((newdegtheta1), 3, MidpointRounding.AwayFromZero) + "\n Theta2: " +
                                            +Math.Round((newdegtheta2), 3, MidpointRounding.AwayFromZero));
                                        Casa();
                                    }
                                    else if (Double.IsNaN(newdegtheta1) || Double.IsNaN(newdegtheta2))
                                    {
                                        MessageBox.Show("Este robô não alcança as coordenadas dadas por conta da limitação do tamanho dos braços");
                                        Casa();
                                    }
                                }

                            }
                            else
                            {
                                Double x = x0;
                                for (double i = 0; i <= Math.Abs(dx); i += 0.5)
                                {
                                    Double yreta = (m * x) - ((x0 * y1 - x1 * y0) / (x1 - x0));
                                    moveTo(x, yreta);
                                    MoveVelocitiesReta(newdegtheta1, newdegtheta2);

                                    String textoArduino = ("0" + Math.Round(degtheta1, 3).ToString() +
                                    "." + Math.Round(degtheta2, 3).ToString() + "." + z.ToString() + "." +
                                    "p");

                                    textBoxEnviar.Text = textoArduino;
                                    if (serialPort1.IsOpen == true)          //porta está aberta
                                        serialPort1.Write(textoArduino);  //envia o texto presente no textbox Enviar

                                    Thread.Sleep(5);
                                    lbl_theta1.Text = Math.Round(newdegtheta1, 3).ToString();
                                    lbl_theta2.Text = Math.Round(newdegtheta2, 3).ToString();

                                    lbl_x.Text = x.ToString();
                                    lbl_y.Text = yreta.ToString();

                                    x -= 0.5;

                                    if (newdegtheta1 > (180) || newdegtheta1 < 0 || newdegtheta2 > (90) || newdegtheta2 < (-90))
                                    {
                                        MessageBox.Show("Este robô não alcança as coordenadas dadas por conta da limitação das juntas.\n Theta1: " +
                                            +Math.Round((newdegtheta1), 3, MidpointRounding.AwayFromZero) + "\n Theta2: " +
                                            +Math.Round((newdegtheta2), 3, MidpointRounding.AwayFromZero));
                                        Casa();
                                    }
                                    else if (Double.IsNaN(newdegtheta1) || Double.IsNaN(newdegtheta2))
                                    {
                                        MessageBox.Show("Este robô não alcança as coordenadas dadas por conta da limitação do tamanho dos braços");
                                        Casa();
                                    }
                                }
                            }
                        }
                        else
                        {
                            MessageBox.Show("Essa reta sairá da área de trabalho");
                            return;
                        }                    
                                                          
                        txt_x.Clear();
                        txt_y.Clear();
                        txt_z.Clear();


                    }
                    catch (FormatException Erro)
                    {
                        MessageBox.Show("As coordenadas não foram escritas: " + Erro.Message);
                    }
                    finally
                    {
                        if (string.IsNullOrEmpty(string_x) || string.IsNullOrEmpty(string_y) || string.IsNullOrEmpty(string_z))
                        {
                            MessageBox.Show("Escreva as coordenadas!");

                            String a_x = ("-");
                            String a_y = ("-");
                            String a_z = ("-");
                            lbl_x.Text = a_x;
                            lbl_y.Text = a_y;
                            lbl_z.Text = a_z;

                        }

                    }
                }


            }
            if (rdb_incremental.Checked)
            {
                String string_x = txt_x.Text.Trim();
                String string_y = txt_y.Text.Trim();
                String string_z = txt_z.Text.Trim();

                try
                {
                    Double x = Xanterior + Math.Round(Convert.ToDouble(string_x), 3, MidpointRounding.ToEven);
                    Double y = Yanterior + Math.Round(Convert.ToDouble(string_y), 3, MidpointRounding.ToEven);
                    z = Zanterior + Math.Round(Convert.ToDouble(string_z), 3, MidpointRounding.ToEven);

                    moveTo(x, y);
                    movepicGarra(z, Zanterior);

                    lbl_theta1.Text = newdegtheta1.ToString();
                    lbl_theta2.Text = newdegtheta2.ToString();

                    lbl_x.Text = x.ToString();
                    lbl_y.Text = y.ToString();

                    if (z > 50)
                    {
                        lbl_z.Text = "50";
                    }
                    else
                    {
                        lbl_z.Text = z.ToString();
                    }

                    Int32.TryParse(lbl_z.Text, out Zanterior);
                    x = Xanterior; y = Yanterior;

                    MoveVelocities(newdegtheta1, newdegtheta2);

                    txt_x.Clear();
                    txt_y.Clear();
                    txt_z.Clear();

                }
                catch (FormatException Erro)
                {
                    MessageBox.Show("As coordenadas não foram escritas: " + Erro.Message);
                }
                finally
                {
                    if (string.IsNullOrEmpty(string_x) || string.IsNullOrEmpty(string_y) || string.IsNullOrEmpty(string_z))
                    {
                        MessageBox.Show("Escreva as coordenadas!");

                        String a_x = ("-");
                        String a_y = ("-");
                        String a_z = ("-");
                        lbl_x.Text = a_x;
                        lbl_y.Text = a_y;
                        lbl_z.Text = a_z;

                    }
                }
            }
        }

        private void btn_close_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void txt_x_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != ',') && (e.KeyChar != '-'))
            {
                e.Handled = true;
                MessageBox.Show("este campo aceita somente numero e vírgula");
            }
            if ((e.KeyChar == ',') && ((sender as TextBox).Text.IndexOf(',') > -1))
            {
                e.Handled = true;
                MessageBox.Show("este campo aceita somente uma vírgula");
            }
        }

        private void txt_y_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != ',') && (e.KeyChar != '-'))
            {
                e.Handled = true;
                MessageBox.Show("este campo aceita somente numero e vírgula");
            }
            if ((e.KeyChar == ',') && ((sender as TextBox).Text.IndexOf(',') > -1))
            {
                e.Handled = true;
                MessageBox.Show("este campo aceita somente uma vírgula");
            }
        }

        private void txt_z_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != ',') && (e.KeyChar != '-'))
            {
                e.Handled = true;
                MessageBox.Show("este campo aceita somente numero e vírgula");
            }
            if ((e.KeyChar == ',') && ((sender as TextBox).Text.IndexOf(',') > -1))
            {
                e.Handled = true;
                MessageBox.Show("este campo aceita somente uma vírgula");
            }
        }

        private void Casa()
        {
            moveTo(260, 0);
            MoveVelocities(newdegtheta1, newdegtheta2);
        }

        private void picCanvas_Click(object sender, EventArgs e)
        {
            //take values of x and y and calculate the angles of the robot
            var mouseEventArgs = e as MouseEventArgs;
            if (mouseEventArgs != null)
            {
                //pega os valores das coordenadas do mouse e os guarda em variáveis
                Double mouseX = mouseEventArgs.X;
                Double mouseY = mouseEventArgs.Y;
                //ajuste do erro de posição
                mouseX = (((mouseX) - (188)) * 1.5);
                mouseY = (((mouseY) - (204)) * 1.5) * (-1);

                if (btn_traj.Text == "Reta")
                {
                    try
                    {
                        Double x1 = Math.Round(Convert.ToDouble(mouseX), 3, MidpointRounding.ToEven);
                        Double y1 = Math.Round(Convert.ToDouble(mouseY), 3, MidpointRounding.ToEven);

                        
                        Double x0 = Convert.ToDouble(lbl_x.Text);
                        Double y0 = Convert.ToDouble(lbl_y.Text);
                        Double a = (y0 - y1);
                        Double b = (x1 - x0);
                        Double d = Math.Abs(x0 * y1 - x1 * y0) / Math.Sqrt(Math.Pow(a,2) + Math.Pow(b,2));
                        if (d >= 189)
                        {
                            /*Double dx = Math.Abs(x1 - x0), sx = x0 < x1 ? 1 : -1;
                            Double dy = Math.Abs(y1 - y0), sy = y0 < y1 ? 1 : -1;
                            Double err = (dx > dy ? dx : -dy) / 2, e2;
                            for (;;)
                            {
                                moveTo(x0, y0);
                                MoveVelocities(newdegtheta1, newdegtheta2);

                                if (x0 == x1 && y0 == y1) return;
                                e2 = err;
                                if (e2 > -dx)
                                {
                                    err -= dy;
                                    x0 += sx;
                                }
                                if (e2 < dy)
                                {
                                    err += dx;
                                    y0 += sy;
                                }
                                //    Serial.print( "// X=" ); Serial.print( x0 ); Serial.print( "  -  Y=" ); Serial.println( y0 );
                                Thread.Sleep(150);           //delay for settling
                            }*/

                            Double m = (-1) * ((y0 - y1) / (x1 - x0));
                            Double dx = x1 - x0;
                            if (dx >= 0)
                            {
                                Double x = x0;
                                for (double i = 0; i <= dx; i += 0.5)
                                {
                                    Double yreta = (m * x) - ((x0 * y1 - x1 * y0) / (x1 - x0));
                                    moveTo(x, yreta);
                                    MoveVelocitiesReta(newdegtheta1, newdegtheta2);

                                    String textoArduino = ("0" + Math.Round(degtheta1, 3).ToString() +
                                    "." + Math.Round(degtheta2, 3).ToString() + "." + lbl_z.Text + "." +
                                    "p");

                                    textBoxEnviar.Text = textoArduino;
                                    if (serialPort1.IsOpen == true)          //porta está aberta
                                        serialPort1.Write(textoArduino);  //envia o texto presente no textbox Enviar

                                    Thread.Sleep(5);
                                    lbl_theta1.Text = Math.Round(newdegtheta1, 3).ToString();
                                    lbl_theta2.Text = Math.Round(newdegtheta2, 3).ToString();

                                    lbl_x.Text = x.ToString();
                                    lbl_y.Text = yreta.ToString();

                                    x += 0.5;

                                    if (newdegtheta1 > (180) || newdegtheta1 < 0 || newdegtheta2 > (90) || newdegtheta2 < (-90))
                                    {
                                        MessageBox.Show("Este robô não alcança as coordenadas dadas por conta da limitação das juntas.\n Theta1: " +
                                            +Math.Round((newdegtheta1), 3, MidpointRounding.AwayFromZero) + "\n Theta2: " +
                                            +Math.Round((newdegtheta2), 3, MidpointRounding.AwayFromZero));
                                        moveTo(260, 0); MoveVelocities(newdegtheta1, newdegtheta2);
                                        return;
                                    }
                                    else if (Double.IsNaN(newdegtheta1) || Double.IsNaN(newdegtheta2))
                                    {
                                        MessageBox.Show("Este robô não alcança as coordenadas dadas por conta da limitação do tamanho dos braços");
                                        Casa();
                                    }

                                }

                            }
                            else
                            {
                                Double x = x0;
                                for (double i = 0; i <= Math.Abs(dx); i += 0.5)
                                {
                                    Double yreta = (m * x) - ((x0 * y1 - x1 * y0) / (x1 - x0));
                                    moveTo(x, yreta);
                                    MoveVelocitiesReta(newdegtheta1, newdegtheta2);

                                    String textoArduino = ("0" + Math.Round(degtheta1, 3).ToString() +
                                    "." + Math.Round(degtheta2, 3).ToString() + "." + lbl_z.Text + "." +
                                    "p");

                                    textBoxEnviar.Text = textoArduino;
                                    if (serialPort1.IsOpen == true)          //porta está aberta
                                        serialPort1.Write(textoArduino);  //envia o texto presente no textbox Enviar

                                    Thread.Sleep(5);
                                    lbl_theta1.Text = Math.Round(newdegtheta1, 3).ToString();
                                    lbl_theta2.Text = Math.Round(newdegtheta2, 3).ToString();

                                    lbl_x.Text = x.ToString();
                                    lbl_y.Text = yreta.ToString();

                                    x -= 0.5;

                                    if (newdegtheta1 > (180) || newdegtheta1 < 0 || newdegtheta2 > (90) || newdegtheta2 < (-90))
                                    {
                                        MessageBox.Show("Este robô não alcança as coordenadas dadas por conta da limitação das juntas.\n Theta1: " +
                                            +Math.Round((newdegtheta1), 3, MidpointRounding.AwayFromZero) + "\n Theta2: " +
                                            +Math.Round((newdegtheta2), 3, MidpointRounding.AwayFromZero));
                                        moveTo(260, 0); MoveVelocities(newdegtheta1, newdegtheta2);
                                        return;
                                    }
                                    else if (Double.IsNaN(newdegtheta1) || Double.IsNaN(newdegtheta2))
                                    {
                                        MessageBox.Show("Este robô não alcança as coordenadas dadas por conta da limitação do tamanho dos braços");
                                        Casa();
                                    }
                                }
                            }
                        }
                        else
                        {
                            MessageBox.Show("Essa reta sairá da área de trabalho");
                            return;
                        }                  
                        
                        txt_x.Clear();
                        txt_y.Clear();
                        txt_z.Clear();
                    }
                    catch (FormatException Erro)
                    {
                        MessageBox.Show("As coordenadas não foram escritas: " + Erro.Message);
                    }
                    finally
                    {
                        if (string.IsNullOrEmpty(mouseX.ToString()) || string.IsNullOrEmpty(mouseY.ToString()))
                        {
                            MessageBox.Show("Escreva as coordenadas!");

                            String a_x = ("-");
                            String a_y = ("-");
                            String a_z = ("-");
                            lbl_x.Text = a_x;
                            lbl_y.Text = a_y;
                            lbl_z.Text = a_z;

                        }

                    }
                }
                else
                {
                    moveTo(mouseX, mouseY);
                    MoveVelocities(newdegtheta1, newdegtheta2);

                    lbl_theta1.Text = newdegtheta1.ToString();
                    lbl_theta2.Text = newdegtheta2.ToString();
                    lbl_x.Text = mouseX.ToString();
                    lbl_y.Text = mouseY.ToString();
                    lbl_z.Text = Zanterior.ToString();
                }              

                
            }
        }

        //Draw the arm
        private void picCanvas_Paint(object sender, PaintEventArgs e)
        {
            DrawRobotArm(e.Graphics, degtheta1, degtheta2);
        }

        private void picCanvas_MouseEnter(object sender, EventArgs e)
        {
            this.lbl_coordinates.Visible = true;
        }

        private void picCanvas_MouseLeave(object sender, EventArgs e)
        {
            this.lbl_coordinates.Visible = false;
        }

        private void picCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            lbl_coordinates.Location = new Point(e.X + 390, e.Y + 100);

            Double x = e.X; Double y = e.Y;
            x = Math.Abs(Convert.ToInt32(((x) - (188)) * 1.5)); y = Math.Abs(Convert.ToInt32(((y) - (204)) * 1.5) * (-1));
            lbl_coordinates.Text = String.Format("X: {0}; Y: {1}", x, y);
        }

        private void rdb_incremental_CheckedChanged(object sender, EventArgs e)
        {
            this.lbl_variacao.Visible = true;
            this.txt_variacao.Visible = true;
            this.btn_cup.Visible = true;
            this.btn_cdown.Visible = true;
            this.btn_cleft.Visible = true;
            this.btn_cright.Visible = true;
            this.btn_up.Visible = true;
            this.btn_down.Visible = true;
            this.btn_traj.Enabled = false;
        }

        private void rdb_absoluta_CheckedChanged(object sender, EventArgs e)
        {
            this.lbl_variacao.Visible = false;
            this.txt_variacao.Visible = false;
            this.btn_cup.Visible = false;
            this.btn_cdown.Visible = false;
            this.btn_cleft.Visible = false;
            this.btn_cright.Visible = false;
            this.btn_up.Visible = false;
            this.btn_down.Visible = false;
            this.btn_traj.Enabled = true;

        }

        private void btn_cup_Click(object sender, EventArgs e)
        {
            if (rdb_incremental.Checked)
            {

                String string_x = lbl_x.Text;
                String string_y = lbl_y.Text;
                String string_variacao = txt_variacao.Text.Trim(); ;

                try
                {
                    Double x = Math.Round(Convert.ToDouble(string_x), 3, MidpointRounding.ToEven);
                    Double y = Math.Round(Convert.ToDouble(string_y), 3, MidpointRounding.ToEven) +
                        Math.Round(Convert.ToDouble(string_variacao), 3, MidpointRounding.ToEven);

                    moveTo(x, y);

                    lbl_theta1.Text = newdegtheta1.ToString();
                    lbl_theta2.Text = newdegtheta2.ToString();

                    lbl_x.Text = x.ToString();
                    lbl_y.Text = y.ToString();
                    
                    x = Xanterior; y = Yanterior;

                    MoveVelocities(newdegtheta1, newdegtheta2);

                    txt_x.Clear();
                    txt_y.Clear();
                    txt_z.Clear();

                }
                catch (FormatException Erro)
                {
                    MessageBox.Show(Erro.Message);
                }
                finally
                {
                    if (string.IsNullOrEmpty(string_x) || string.IsNullOrEmpty(string_y) || string.IsNullOrEmpty(string_variacao))
                    {
                        MessageBox.Show("Escreva a variação dos botões!");

                        String a_x = ("-");
                        String a_y = ("-");
                        String a_z = ("-");
                        lbl_x.Text = a_x;
                        lbl_y.Text = a_y;
                        lbl_z.Text = a_z;

                    }
                }
            }
        }

        private void btn_cdown_Click(object sender, EventArgs e)
        {
            if (rdb_incremental.Checked)
            {

                String string_x = lbl_x.Text;
                String string_y = lbl_y.Text;
                String string_variacao = txt_variacao.Text.Trim(); ;

                try
                {
                    Double x = Math.Round(Convert.ToDouble(string_x), 3, MidpointRounding.ToEven);
                    Double y = Math.Round(Convert.ToDouble(string_y), 3, MidpointRounding.ToEven) -
                        Math.Round(Convert.ToDouble(string_variacao), 3, MidpointRounding.ToEven);

                    moveTo(x, y);

                    lbl_theta1.Text = newdegtheta1.ToString();
                    lbl_theta2.Text = newdegtheta2.ToString();

                    lbl_x.Text = x.ToString();
                    lbl_y.Text = y.ToString();

                    x = Xanterior; y = Yanterior;

                    MoveVelocities(newdegtheta1, newdegtheta2);

                    txt_x.Clear();
                    txt_y.Clear();
                    txt_z.Clear();

                }
                catch (FormatException Erro)
                {
                    MessageBox.Show(Erro.Message);
                }
                finally
                {
                    if (string.IsNullOrEmpty(string_x) || string.IsNullOrEmpty(string_y) || string.IsNullOrEmpty(string_variacao))
                    {
                        MessageBox.Show("Escreva a variação dos botões!");

                        String a_x = ("-");
                        String a_y = ("-");
                        String a_z = ("-");
                        lbl_x.Text = a_x;
                        lbl_y.Text = a_y;
                        lbl_z.Text = a_z;

                    }
                }
            }
        }

        private void btn_cright_Click(object sender, EventArgs e)
        {
            if (rdb_incremental.Checked)
            {

                String string_x = lbl_x.Text;
                String string_y = lbl_y.Text;
                String string_variacao = txt_variacao.Text.Trim(); ;

                try
                {
                    Double x = Math.Round(Convert.ToDouble(string_x), 3, MidpointRounding.ToEven) +
                        Math.Round(Convert.ToDouble(string_variacao), 3, MidpointRounding.ToEven);
                    Double y = Math.Round(Convert.ToDouble(string_y), 3, MidpointRounding.ToEven);

                    moveTo(x, y);

                    lbl_theta1.Text = newdegtheta1.ToString();
                    lbl_theta2.Text = newdegtheta2.ToString();

                    lbl_x.Text = x.ToString();
                    lbl_y.Text = y.ToString();

                    x = Xanterior; y = Yanterior;

                    MoveVelocities(newdegtheta1, newdegtheta2);

                    txt_x.Clear();
                    txt_y.Clear();
                    txt_z.Clear();

                }
                catch (FormatException Erro)
                {
                    MessageBox.Show(Erro.Message);
                }
                finally
                {
                    if (string.IsNullOrEmpty(string_x) || string.IsNullOrEmpty(string_y) || string.IsNullOrEmpty(string_variacao))
                    {
                        MessageBox.Show("Escreva a variação dos botões!");

                        String a_x = ("-");
                        String a_y = ("-");
                        String a_z = ("-");
                        lbl_x.Text = a_x;
                        lbl_y.Text = a_y;
                        lbl_z.Text = a_z;

                    }
                }
            }
        }

        private void btn_cleft_Click(object sender, EventArgs e)
        {
            if (rdb_incremental.Checked)
            {

                String string_x = lbl_x.Text;
                String string_y = lbl_y.Text;
                String string_variacao = txt_variacao.Text.Trim(); ;

                try
                {
                    Double x = Math.Round(Convert.ToDouble(string_x), 3, MidpointRounding.ToEven) -
                        Math.Round(Convert.ToDouble(string_variacao), 3, MidpointRounding.ToEven);
                    Double y = Math.Round(Convert.ToDouble(string_y), 3, MidpointRounding.ToEven);

                    moveTo(x, y);

                    lbl_theta1.Text = newdegtheta1.ToString();
                    lbl_theta2.Text = newdegtheta2.ToString();

                    lbl_x.Text = x.ToString();
                    lbl_y.Text = y.ToString();

                    x = Xanterior; y = Yanterior;

                    MoveVelocities(newdegtheta1, newdegtheta2);

                    txt_x.Clear();
                    txt_y.Clear();
                    txt_z.Clear();

                }
                catch (FormatException Erro)
                {
                    MessageBox.Show(Erro.Message);
                }
                finally
                {
                    if (string.IsNullOrEmpty(string_x) || string.IsNullOrEmpty(string_y) || string.IsNullOrEmpty(string_variacao))
                    {
                        MessageBox.Show("Escreva a variação dos botões!");

                        String a_x = ("-");
                        String a_y = ("-");
                        String a_z = ("-");
                        lbl_x.Text = a_x;
                        lbl_y.Text = a_y;
                        lbl_z.Text = a_z;

                    }
                }
            }
        }

        private void btn_down_Click(object sender, EventArgs e)
        {
            if (rdb_incremental.Checked)
            {                
                String string_variacao = txt_variacao.Text.Trim(); ;

                try
                {
                    Double z = Zanterior - Math.Round(Convert.ToDouble(string_variacao), 3, MidpointRounding.ToEven);

                    movepicGarra(z, Zanterior);

                    if (z > 50)
                    {
                        lbl_z.Text = "50";
                    }
                    else
                    {
                        lbl_z.Text = z.ToString();
                    }

                    String textoArduino = ("0" + Math.Round(degtheta1, 3).ToString() +
                                    "." + Zanterior.ToString() + "." + z.ToString() + "." +
                                    "d");

                    textBoxEnviar.Text = textoArduino;

                    if (serialPort1.IsOpen == true)          //porta está aberta
                        serialPort1.Write(textoArduino);  //envia o texto presente no textbox Enviar

                    Int32.TryParse(lbl_z.Text, out Zanterior);

                    txt_x.Clear();
                    txt_y.Clear();
                    txt_z.Clear();

                }
                catch (FormatException Erro)
                {
                    MessageBox.Show(Erro.Message);
                }
                finally
                {
                    if (string.IsNullOrEmpty(string_variacao))
                    {
                        MessageBox.Show("Escreva a variação dos botões!");
                        
                    }
                }
            }
        }

        private void btn_up_Click(object sender, EventArgs e)
        {
            if (rdb_incremental.Checked)
            {
                String string_variacao = txt_variacao.Text.Trim(); ;

                try
                {
                    Double z = Zanterior + Math.Round(Convert.ToDouble(string_variacao), 3, MidpointRounding.ToEven);

                    movepicGarra(z, Zanterior);

                    if (z > 50)
                    {
                        lbl_z.Text = "50";
                    }
                    else
                    {
                        lbl_z.Text = z.ToString();
                    }

                    String textoArduino = ("0" + Math.Round(degtheta1, 3).ToString() +
                                    "." + Zanterior.ToString() + "." + z.ToString() + "." +
                                    "d");

                    textBoxEnviar.Text = textoArduino;

                    if (serialPort1.IsOpen == true)          //porta está aberta
                        serialPort1.Write(textoArduino);  //envia o texto presente no textbox Enviar

                    Int32.TryParse(lbl_z.Text, out Zanterior);

                    txt_x.Clear();
                    txt_y.Clear();
                    txt_z.Clear();

                }
                catch (FormatException Erro)
                {
                    MessageBox.Show(Erro.Message);
                }
                finally
                {
                    if (string.IsNullOrEmpty(string_variacao))
                    {
                        MessageBox.Show("Escreva a variação dos botões!");

                    }
                }
            }
        }

        private void txt_variacao_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != ','))
            {
                e.Handled = true;
                MessageBox.Show("este campo aceita somente numero e vítgula");
            }
            if ((e.KeyChar == ',') && ((sender as TextBox).Text.IndexOf(',') > -1))
            {
                e.Handled = true;
                MessageBox.Show("este campo aceita somente uma vírgula");
            }
        }

        private void btn_cinematica_Click(object sender, EventArgs e)
        {
            if (btn_cinematica.Text == "Inversa")
            {
                txt_cinematica1.Enabled = true;
                txt_cinematica2.Enabled = true;
                rdb_absoluta.Enabled = false;
                rdb_incremental.Enabled = false;
                txt_x.Enabled = false;
                txt_y.Enabled = false;
                txt_z.Enabled = false;
                txt_variacao.Enabled = false;
                btn_enviathetas.Visible = true;
                btn_traj.Visible = false;

                btn_cinematica.Text = "Direta";

            }
            else
            {
                txt_cinematica1.Enabled = false;
                txt_cinematica2.Enabled = false;
                rdb_absoluta.Enabled = true;
                rdb_incremental.Enabled = true;
                txt_x.Enabled = true;
                txt_y.Enabled = true;
                txt_z.Enabled = true;
                txt_variacao.Enabled = true;
                btn_enviathetas.Visible = false;
                btn_traj.Visible = true;

                btn_cinematica.Text = "Inversa";
            }
        }

        private void btn_enviathetas_Click(object sender, EventArgs e)
        {
            String string_theta1 = txt_cinematica1.Text.Trim();
            String string_theta2 = txt_cinematica2.Text.Trim();

            try
            {
                newdegtheta1 = Math.Round(Convert.ToDouble(string_theta1), 3, MidpointRounding.ToEven);
                newdegtheta2 = Math.Round(Convert.ToDouble(string_theta2), 3, MidpointRounding.ToEven);
                
                lbl_theta1.Text = Math.Round(newdegtheta1, 3, MidpointRounding.ToEven).ToString();
                lbl_theta2.Text = Math.Round(newdegtheta2, 3, MidpointRounding.ToEven).ToString();

                lbl_x.Text = Math.Round(arm1 * (Math.Cos(newdegtheta1 * Math.PI / 180)) + arm2 * 
                    (Math.Cos((newdegtheta1 + newdegtheta2)*Math.PI/180)), 3, MidpointRounding.ToEven).ToString();
                lbl_y.Text = Math.Round(arm1 * (Math.Sin(newdegtheta1 * Math.PI / 180)) + arm2 * 
                    (Math.Sin((newdegtheta1 + newdegtheta2)*Math.PI/180)), 3, MidpointRounding.ToEven).ToString();
                

                MoveVelocities(newdegtheta1, newdegtheta2);

                if (newdegtheta1 > (180) || newdegtheta1 < 0 || newdegtheta2 > (90) || newdegtheta2 < (-90))
                {
                    MessageBox.Show("Este robô não alcança as coordenadas dadas por conta da limitação das juntas.\n Theta1: " +
                        +Math.Round((newdegtheta1), 3, MidpointRounding.AwayFromZero) + "\n Theta2: " +
                        +Math.Round((newdegtheta2), 3, MidpointRounding.AwayFromZero));
                    return;
                }
                else if (Double.IsNaN(newdegtheta1) || Double.IsNaN(newdegtheta2))
                {
                    MessageBox.Show("Este robô não alcança as coordenadas dadas por conta da limitação do tamanho dos braços");
                }

                txt_x.Clear();
                txt_y.Clear();
                txt_z.Clear();
                txt_cinematica1.Clear();
                txt_cinematica2.Clear();
                
            }
            catch (FormatException Erro)
            {
                MessageBox.Show("As coordenadas não foram escritas: " + Erro.Message);
            }
            finally
            {
                if (string.IsNullOrEmpty(string_theta1) || string.IsNullOrEmpty(string_theta2))
                {
                    MessageBox.Show("Escreva os ângulos!");

                    String a_x = ("-");
                    String a_y = ("-");
                    String a_z = ("-");
                    lbl_x.Text = a_x;
                    lbl_y.Text = a_y;
                    lbl_z.Text = a_z;

                }

            }
        }

        private void txt_fecha_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != ','))
            {
                e.Handled = true;
                MessageBox.Show("este campo aceita somente numero e vítgula");
            }
            if ((e.KeyChar == ',') && ((sender as TextBox).Text.IndexOf(',') > -1))
            {
                e.Handled = true;
                MessageBox.Show("este campo aceita somente uma vírgula");
            }
        }

        private void btn_reseta_Click(object sender, EventArgs e)
        {
            Casa();
            String textoArduino = ("01.2.0.r");

            textBoxEnviar.Text = textoArduino;

            if (serialPort1.IsOpen == true)          //porta está aberta
                serialPort1.Write(textoArduino);  //envia o texto presente no textbox Enviar
        }

        private void txt_cinematica1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != ',') && (e.KeyChar != '-'))
            {
                e.Handled = true;
                MessageBox.Show("este campo aceita somente numero e vírgula");
            }
            if ((e.KeyChar == ',') && ((sender as TextBox).Text.IndexOf(',') > -1))
            {
                e.Handled = true;
                MessageBox.Show("este campo aceita somente uma vírgula");
            }
        }

        private void txt_cinematica2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != ',') && (e.KeyChar != '-'))
            {
                e.Handled = true;
                MessageBox.Show("este campo aceita somente numero e vírgula");
            }
            if ((e.KeyChar == ',') && ((sender as TextBox).Text.IndexOf(',') > -1))
            {
                e.Handled = true;
                MessageBox.Show("este campo aceita somente uma vírgula");
            }
        }

        private void btn_traj_Click(object sender, EventArgs e)
        {
            if (btn_traj.Text == "Trajetória")
            {
                btn_traj.Text = "Reta";
            }
            else
            {
                btn_traj.Text = "Trajetória";
            }
        }

        private void btn_resetaCanvas_Click(object sender, EventArgs e)
        {
            Calculadora cal = new Calculadora();
            cal.Show();
        }

        private void btn_add_Click(object sender, EventArgs e)
        {
            listX.Add(lbl_x.Text);
            listY.Add(lbl_y.Text);
            listZ.Add(lbl_z.Text);

            lbl_cont.Text = listX.Count.ToString();
        }

        private void toPointRepeat()
        {
            int wait = Convert.ToInt32(txt_wait.Text);

            for (int i = 0; i < listX.Count; i++)
            {
                String valueX = listX[i] as String;
                String valueY = listY[i] as String;
                String valueZ = listZ[i] as String;

                Double doubleX = Convert.ToDouble(valueX);
                Double doubleY = Convert.ToDouble(valueY);
                Double doubleZ = Convert.ToDouble(valueZ);

                moveTo(doubleX, doubleY);
                movepicGarra(doubleZ, Zanterior);

                lbl_theta1.Text = newdegtheta1.ToString();
                lbl_theta2.Text = newdegtheta2.ToString();

                lbl_x.Text = doubleX.ToString();
                lbl_y.Text = doubleY.ToString();

                if (z > 50)
                {
                    lbl_z.Text = "50";
                }
                else
                {
                    lbl_z.Text = z.ToString();
                }

                Int32.TryParse(lbl_z.Text, out Zanterior);

                MoveVelocities(newdegtheta1, newdegtheta2);

                /*String textoArduino = ("0" + Math.Round(degtheta1, 3).ToString() +
                                "." + Math.Round(degtheta2, 3).ToString() + "." + z.ToString() + "." +
                                "a");

                textBoxEnviar.Text = textoArduino;

                if (serialPort1.IsOpen == true)          //porta está aberta
                    serialPort1.Write(textoArduino);  //envia o texto presente no textbox Enviar*/


                Thread.Sleep(wait);


            }
        }

        private void btn_exe_Click(object sender, EventArgs e)
        {

            if (txt_wait.Text == "")
            {
                MessageBox.Show("Escolha um tempo de espera em cada ponto. \nTempos menores de 2700 milissegundos" +
                    "podem prejudicar a comunicação entre o computador e o arduino");
            }
            else
            {
                if(txt_vezes.Text == "" || txt_vezes.Text == " ")
                {
                    MessageBox.Show("Escreva quantas vezes a sequência deve ser repetida");
                }
                else
                {
                    int j = Convert.ToInt32(txt_vezes.Text);
                    for (int i = 0; i < j; i++)
                    {
                        toPointRepeat();
                    }
                }                           
            }
        }

        private void btn_clear_Click(object sender, EventArgs e)
        {
            listX.Clear();
            listY.Clear();
            listZ.Clear();

            lbl_cont.Text = listX.Count.ToString();
        }

        private void txt_wait_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != ','))
            {
                e.Handled = true;
                MessageBox.Show("este campo aceita somente numero e vítgula");
            }
            if ((e.KeyChar == ',') && ((sender as TextBox).Text.IndexOf(',') > -1))
            {
                e.Handled = true;
                MessageBox.Show("este campo aceita somente uma vírgula");
            }
        }

        private void txt_vezes_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
                MessageBox.Show("este campo aceita somente numero");
            }
            if ((e.KeyChar == ',') && ((sender as TextBox).Text.IndexOf(',') > -1))
            {
                e.Handled = true;
                MessageBox.Show("este campo aceita somente uma vírgula");
            }
        }

        private void btn_info_Click(object sender, EventArgs e)
        {
            if(btn_info.Text == "Informações adicionais")
            {
                btn_info.Text = " Informações adicionais ";
                lbl_pwm1.Visible = true;
                lbl_pwm1_.Visible = true;
                lbl_pwm2.Visible = true;
                lbl_pwm2_.Visible = true;
            }
            else
            {
                btn_info.Text = "Informações adicionais";
                lbl_pwm1.Visible = false;
                lbl_pwm1_.Visible = false;
                lbl_pwm2.Visible = false;
                lbl_pwm2_.Visible = false;
            }
        }

        private void lbl_theta1_TextChanged(object sender, EventArgs e)
        {
            float y0 = 600, y1 = 2400, x0 = 0, x1 = 180;
            float x = Convert.ToSingle(lbl_theta1.Text);
            float y = y0 + (y1 - y0) * ((x - x0) / (x1 - x0));
            lbl_pwm1.Text = Math.Round(y, 3).ToString();
        }

        private void lbl_theta2_TextChanged(object sender, EventArgs e)
        {
            float y0 = 600, y1 = 2400, x0 = 0, x1 = 180;
            float x = Convert.ToSingle(lbl_theta2.Text) + 90;
            float y = y0 + (y1 - y0) * ((x - x0) / (x1 - x0));
            lbl_pwm2.Text = Math.Round(y, 3).ToString();
        }

        private void btn_garra_Click(object sender, EventArgs e)
        {

            double max = 45, min = 0;

            if (btn_garra.Text == "Fechar")
            {                                          

                string fecha = txt_fecha.Text.Trim();
                Double angfecha = Math.Round(Convert.ToDouble(fecha), 3, MidpointRounding.ToEven);

                if(angfecha <= max && angfecha > 0)
                {
                    btn_garra.Text = "Abrir";
                    lbl_fecha.Visible = false;
                    txt_fecha.Visible = false;

                    picGarra.Image = Properties.Resources.Garra__Fechada;

                    String textoArduino = ("0" + "1" +
                                    "." + "2" + "." + angfecha.ToString() + "." +
                                    "c");

                    textBoxEnviar.Text = textoArduino;

                    if (serialPort1.IsOpen == true)          //porta está aberta
                        serialPort1.Write(textoArduino);  //envia o texto presente no textbox Enviar
                }
                else
                {
                    MessageBox.Show("Essa garra não alcança o ângulo desejado, ângulo máximo de valor 45");

                }
            }
            else
            {
                lbl_fecha.Visible = true;
                txt_fecha.Visible = true;

                picGarra.Image = Properties.Resources.Garra_Aberta;
                btn_garra.Text = "Fechar";

                String textoArduino = ("0" + "1" +
                                    "." + "2" + "." + min.ToString() + "." +
                                    "b");

                textBoxEnviar.Text = textoArduino;

                if (serialPort1.IsOpen == true)          //porta está aberta
                    serialPort1.Write(textoArduino);  //envia o texto presente no textbox Enviar
            }
        }

        // Draw the robot arm.
        private void DrawRobotArm(Graphics gr, Double mouseDegt1, Double mouseDegt2)
        {

            try
            {
                const double UpperArmLength = (130 / 1.5);
                const double LowerArmLength = (130 / 1.5);
                const double WristLength = (10 / 1.5);

                gr.SmoothingMode = SmoothingMode.AntiAlias;
                //gr.Clear(picCanvas.BackColor);

                // For each stage in the arm, draw and then *prepend* the
                // new transformation to represent the next arm in the sequence.

                // Translate to center of form.
                float cx = picCanvas.ClientSize.Width / 2;
                float cy = picCanvas.ClientSize.Height / 2;
                gr.TranslateTransform(cx, cy);

                // **************
                // Draw the arms.
                GraphicsState initial_state = gr.Save();

                // Make a rectangle to represent an arm.
                // Later we'll set its width for each arm.
                Rectangle rect = new Rectangle(0, -2, 100, 5);

                // Rotate at the shoulder.
                // (Negative to make the angle increase counter-clockwise).
                gr.RotateTransform(-Convert.ToSingle(mouseDegt1), MatrixOrder.Prepend);

                // Draw the first arm.
                rect.Width = Convert.ToInt32(UpperArmLength);
                gr.FillRectangle(Brushes.DarkGray, rect);
                gr.DrawRectangle(Pens.White, rect);

                // Translate to the end of the first arm.
                gr.TranslateTransform(Convert.ToInt32(UpperArmLength), 0, MatrixOrder.Prepend);

                // Rotate at the elbow.
                gr.RotateTransform(-Convert.ToSingle(mouseDegt2), MatrixOrder.Prepend);

                // Draw the second arm.
                rect.Width = Convert.ToInt32(LowerArmLength);
                gr.FillRectangle(Brushes.DarkGray, rect);
                gr.DrawRectangle(Pens.White, rect);

                // Translate to the end of the second arm.
                gr.TranslateTransform(Convert.ToInt32(LowerArmLength), 0, MatrixOrder.Prepend);

                // Rotate at the wrist.
                gr.RotateTransform(0, MatrixOrder.Prepend);

                // Draw the third arm.
                rect.Width = Convert.ToInt32(WristLength);
                gr.FillRectangle(Brushes.DarkGray, rect);
                gr.DrawRectangle(Pens.White, rect);

                // ***********************************
                // Draw the joints on top of the arms.
                gr.Restore(initial_state);

                // Draw the shoulder centered at the origin.
                Rectangle joint_rect = new Rectangle(-4, -4, 9, 9);
                gr.FillEllipse(Brushes.Black, joint_rect);

                // Rotate at the shoulder.
                // (Negative to make the angle increase counter-clockwise).
                gr.RotateTransform(-Convert.ToSingle(mouseDegt1), MatrixOrder.Prepend);

                // Translate to the end of the first arm.
                gr.TranslateTransform(Convert.ToInt32(UpperArmLength), 0, MatrixOrder.Prepend);

                // Draw the elbow.
                gr.FillEllipse(Brushes.BlanchedAlmond, joint_rect);

                // Rotate at the elbow.
                gr.RotateTransform(-Convert.ToSingle(mouseDegt2), MatrixOrder.Prepend);

                // Translate to the end of the second arm.
                gr.TranslateTransform(Convert.ToInt32(LowerArmLength), 0, MatrixOrder.Prepend);

                // Draw the wrist.
                gr.FillEllipse(Brushes.BlanchedAlmond, joint_rect);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
            finally
            {
                if (double.IsNaN(degtheta1) || double.IsNaN(degtheta2))
                {
                    MessageBox.Show("O resultado não é um número");

                    String a_x = ("-");
                    String a_y = ("-");
                    String a_z = ("-");
                    lbl_x.Text = a_x;
                    lbl_y.Text = a_y;
                    lbl_z.Text = a_z;
                    degtheta1 = 0;
                    degtheta2 = 0;

                }

            }

        }



    }
}
