using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SoftwareScara_D
{
    public partial class Calculadora : Form
    {
        

        public Calculadora()
        {
            InitializeComponent();
        }

        Double axy = 0;
        Double phi = 0;
        Double dxyz = 0;

        private void DistanciaXY()
        {
            Double x = 0, y = 0, dxy = 0;

            if (txt_x.Text != "" && txt_y.Text != "" && txt_y.Text != "-")
            {
                x = Convert.ToDouble(txt_x.Text);
                y = Convert.ToDouble(txt_y.Text);
                dxy = Math.Sqrt(Math.Pow(x,2) + Math.Pow(y,2));
            }

            txt_xy.Text = Math.Round(dxy,3).ToString();
        }

        private void DistanciaXYZ()
        {
            Double x = 0, y = 0, z = 0;

            if(txt_x.Text != "" && txt_y.Text != "" && txt_z.Text != "" && txt_x.Text != "-" && txt_y.Text != "-" && txt_z.Text != "-")
            {
                x = Convert.ToDouble(txt_x.Text);
                y = Convert.ToDouble(txt_y.Text);
                z = Convert.ToDouble(txt_z.Text);
                dxyz = Math.Sqrt(Math.Pow(x, 2) + Math.Pow(y, 2) + Math.Pow(z, 2));
            }
            txt_xyz.Text = Math.Round(dxyz, 3).ToString();
        }

        private void AnguloXY()
        {
            Double x = 0, y = 0;

            if (txt_x.Text != "" && txt_y.Text != "" && txt_y.Text != "-")
            {
                x = Convert.ToDouble(txt_x.Text);
                y = Convert.ToDouble(txt_y.Text);

                axy = Math.Atan(y/x);
            }
            
            if (btn_a.Text == "Ângulos em Graus")
            {
                txt_axy.Text = Math.Round(axy * 180 / Math.PI, 3).ToString();
            }
            else if(btn_a.Text == "Ângulos em Radianos")
            {
                txt_axy.Text = Math.Round(axy, 3).ToString();
            }
            else if(btn_a.Text == "Sexagesimal")
            {
                try
                {
                    Double axy_graus = axy * 180 / Math.PI;
                    String myString = axy_graus.ToString();

                    if (myString == "0")
                    {
                        txt_axy.Text = "0° 0' 0''";
                    }
                    else
                    {
                        Char charRange = ',';
                        int commaIndex = myString.IndexOf(charRange);

                        if(commaIndex > 0)
                        {
                            String string_graus = myString.Substring(0, commaIndex);
                            String almost_min = "0," + myString.Substring(commaIndex + 1);
                            String string_min = (Convert.ToDouble(almost_min) * 60).ToString();
                            Double graus = Math.Round(Convert.ToDouble(string_graus), 0);
                            Double min = Math.Round((Convert.ToDouble(string_min)), 0);

                            if (string_min.IndexOf(charRange) > 0)
                            {
                                String almost_seg = "0" + string_min.Substring(string_min.IndexOf(charRange));
                                String string_seg = (Convert.ToDouble(almost_seg) * 60).ToString();
                                                                
                                Double seg = Math.Round(Convert.ToDouble(string_seg), 2);

                                txt_axy.Text = graus.ToString() + "° " + min.ToString() + "' " + seg.ToString() + "''";
                            }
                            else
                            {
                                txt_axy.Text = graus.ToString() + "°" + min.ToString() + "' 0''";
                            }
                            
                        }
                        else
                        {
                            txt_axy.Text = myString+"° 0' 0''";
                        }
                        
                    }

                }
                catch (FormatException e)
                {
                    MessageBox.Show(e.Message);
                }
            }
        }

        private void AnguloPhi()
        {
            Double x = 0, y = 0, z = 0;

            if (txt_x.Text != "" && txt_y.Text != "" && txt_z.Text != "" && txt_y.Text != "-" && txt_z.Text != "-")
            {
                x = Convert.ToDouble(txt_x.Text);
                y = Convert.ToDouble(txt_y.Text);
                z = Convert.ToDouble(txt_z.Text);

                phi = Math.Acos(z / dxyz);
            }

            if (btn_a.Text == "Ângulos em Graus")
            {
                txt_phi.Text = Math.Round(phi * 180 / Math.PI, 3).ToString();
            }
            else if (btn_a.Text == "Ângulos em Radianos")
            {
                txt_phi.Text = Math.Round(phi, 3).ToString();
            }
            else if (btn_a.Text == "Sexagesimal")
            {
                try
                {
                    Double phi_graus = phi * 180 / Math.PI;
                    String myString = phi_graus.ToString();

                    if (myString == "0")
                    {
                        txt_axy.Text = "0° 0' 0''";
                    }
                    else
                    {
                        Char charRange = ',';
                        int commaIndex = myString.IndexOf(charRange);

                        if (commaIndex > 0)
                        {
                            String string_graus = myString.Substring(0, commaIndex);
                            String almost_min = "0," + myString.Substring(commaIndex + 1);
                            String string_min = (Convert.ToDouble(almost_min) * 60).ToString();
                            Double graus = Math.Round(Convert.ToDouble(string_graus), 0);
                            Double min = Math.Round((Convert.ToDouble(string_min)), 0);

                            if (string_min.IndexOf(charRange) > 0)
                            {
                                String almost_seg = "0" + string_min.Substring(string_min.IndexOf(charRange));
                                String string_seg = (Convert.ToDouble(almost_seg) * 60).ToString();

                                Double seg = Math.Round(Convert.ToDouble(string_seg), 2);

                                txt_phi.Text = graus.ToString() + "° " + min.ToString() + "' " + seg.ToString() + "''";
                            }
                            else
                            {
                                txt_phi.Text = graus.ToString() + "°" + min.ToString() + "' 0''";
                            }

                        }
                        else
                        {
                            txt_phi.Text = myString + "° 0' 0''";
                        }

                    }

                }
                catch (FormatException e)
                {
                    MessageBox.Show(e.Message);
                }
            }
        }

        private void NovoXYpolar()
        {
            if (txt_xy.Text != "" && txt_axy.Text != "" && txt_xy.Text != "-" && txt_axy.Text != "-")
            {
                Double theta = axy, x, y;
                Double polarXY = Convert.ToDouble(txt_xy.Text);
                x = polarXY * Math.Cos(theta);
                y = polarXY * Math.Sin(theta);

                txt_x.Text = Math.Round(x, 3).ToString();
                txt_y.Text = Math.Round(y, 3).ToString();
            }
                
        }

        private void NovoXYZpolar()
        {
            if (txt_xyz.Text != "" && txt_phi.Text != "" && txt_xyz.Text != "-" && txt_phi.Text != "-")
            {
                Double theta = axy, x, y, z;
                Double polarXYZ = Convert.ToDouble(txt_xyz.Text);
                x = polarXYZ * Math.Sin(phi) * Math.Cos(theta);
                y = polarXYZ * Math.Sin(phi) * Math.Sin(theta);
                z = polarXYZ * Math.Cos(phi);

                txt_x.Text = Math.Round(x, 3).ToString();
                txt_y.Text = Math.Round(y, 3).ToString();
                txt_z.Text = Math.Round(z, 3).ToString();
            }
                
        }

        private void txt_x_TextChanged(object sender, EventArgs e)
        {
           /* DistanciaXYZ();
            DistanciaXY();
            AnguloXY();
            AnguloPhi();*/
        }

        private void txt_y_TextChanged(object sender, EventArgs e)
        {
            /*DistanciaXYZ();
            DistanciaXY();
            AnguloXY();
            AnguloPhi();*/
        }

        private void btn_a_Click(object sender, EventArgs e)
        {
            if (btn_a.Text == "Ângulos em Graus")
            {
                btn_a.Text = "Ângulos em Radianos";
            }
            else if (btn_a.Text == "Ângulos em Radianos")
            {
                btn_a.Text = "Sexagesimal";
            }
            else if (btn_a.Text == "Sexagesimal")
            {
                btn_a.Text = "Ângulos em Graus";
            }
            AnguloXY();
            AnguloPhi();
        }

        private void txt_z_TextChanged(object sender, EventArgs e)
        {
            /*DistanciaXYZ();
            AnguloPhi();*/
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

            DistanciaXYZ();
            DistanciaXY();
            AnguloXY();
            AnguloPhi();
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

            DistanciaXYZ();
            DistanciaXY();
            AnguloXY();
            AnguloPhi();
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

            
            DistanciaXYZ();
            AnguloPhi();
            AnguloXY();
        }

        private void txt_xy_KeyPress(object sender, KeyPressEventArgs e)
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

            NovoXYpolar();
            DistanciaXYZ();
            AnguloPhi();
        }

        private void txt_xyz_KeyPress(object sender, KeyPressEventArgs e)
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

            NovoXYZpolar();
            DistanciaXY();
            AnguloXY();

        }

        private void txt_axy_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != ',') && (e.KeyChar != '-') && (e.KeyChar != '°'))
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

        private void txt_phi_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != ',') && (e.KeyChar != '-') && (e.KeyChar != '°'))
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

        private void txt_xy_TextChanged(object sender, EventArgs e)
        {
            //NovoXYpolar();
        }

        private void txt_xyz_TextChanged(object sender, EventArgs e)
        {
            //NovoXYZpolar();
        }

        private void btn_usa_Click(object sender, EventArgs e)
        {
            String x = txt_x.Text;
            String y = txt_y.Text;
            String z = txt_z.Text;

            Principal frm = new Principal();
            frm.PassValue(x,y,z);
        }

        private void Calculadora_Load(object sender, EventArgs e)
        {

        }
    }
}
