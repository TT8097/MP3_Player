using AxWMPLib;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using WMPLib;

namespace musicplayer
{
    public partial class Form1 : Form
    {
        int time_tick;
        int div_bar;
        int index_selected;
        bool is_played_once = false;

        Dictionary<string,string> absolut_path= new Dictionary<string,string>();

        public Form1()
        {
            InitializeComponent();
            axWindowsMediaPlayer1.uiMode = "none";
            axWindowsMediaPlayer1.settings.volume = 0;
            axWindowsMediaPlayer1.settings.autoStart = false;
            axWindowsMediaPlayer1.PlayStateChange += AxWindowsMediaPlayer1_PlayStateChange;
            listView1.ItemSelectionChanged += ListView1_ItemSelectionChanged;
            Set_Clickable(false);
           
        }

        private void ListView1_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
           
            if (e.IsSelected) {
               
                Set_Clickable(true);
                if (checkBox1.Checked) { button2.Enabled = false; button3.Enabled = false; } else { button2.Enabled = true; button3.Enabled = true; }
                if (e.Item.Text == label1.Text) { is_played_once = true;  } else { is_played_once = false; Stop.Enabled = false; }
            }else { Set_Clickable(false); }
            
        }

        private void Play_Button_Click(object sender, EventArgs e)
        {
            axWindowsMediaPlayer1.Ctlcontrols.stop();
            Stop.Enabled = true;
            buttonAdd.Enabled = false;
            axWindowsMediaPlayer1.URL = absolut_path[ listView1.SelectedItems[0].Text];
           axWindowsMediaPlayer1.Ctlcontrols.currentPosition = (is_played_once==false ?0:(double)trackBar1.Value / 4);
            axWindowsMediaPlayer1.Ctlcontrols.play();
            label1.Text = listView1.SelectedItems[0].Text;
            timer1.Stop();
            timer1.Start();
        }

        private void Stop_Click(object sender, EventArgs e)
        {
            is_played_once = true;
            axWindowsMediaPlayer1.Ctlcontrols.stop();
            timer1.Stop();
            buttonAdd.Enabled = true;


        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (trackBar1.Value == div_bar) { timer1.Stop(); if (trackBar1.Value == div_bar) { buttonAdd.Enabled = true; is_played_once = true; }
                if (listView1.SelectedItems.Count > 0)
                {
                    if (checkBox1.Checked && listView1.SelectedItems[0].Index < listView1.Items.Count - 1)
                    {


                        int index_next = listView1.SelectedItems[0].Index + 1;
                        listView1.SelectedItems[0].Selected = false;
                        listView1.Items[index_next].Selected = true;
                        trackBar1.Value = 0;
                        is_played_once = false;
                        Play_Button_Click(sender, e);

                    }
                }
            }
            else
            {
                if (!is_played_once)
                {
                    Calculate_Divisions(axWindowsMediaPlayer1.currentMedia.duration);
                    trackBar1.Maximum = div_bar; 
                     }
                trackBar1.Value += 1;
            }

        }

        private void Audio_Track_Scroll(object sender, EventArgs e)
        {
            axWindowsMediaPlayer1.Ctlcontrols.currentPosition = (double)trackBar1.Value / 4;
            if (trackBar1.Value == div_bar) { buttonAdd.Enabled = true; is_played_once = true; }

        }
        private void Calculate_Divisions(double duration_time)
        {
            int val = Convert.ToInt32(duration_time);
            div_bar = val * 4;
        }

        private void ButtonAdd_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "filtr |*.mp3;*.wav;";
            dlg.ShowDialog();
           
            if (dlg.FileName!="")
            {
                
                string[] chop_path = dlg.FileName.Split('\\');
                if (!absolut_path.ContainsKey(chop_path[chop_path.Length - 1]))
                {
                    axWindowsMediaPlayer1.URL = dlg.FileName;
                    absolut_path[chop_path[chop_path.Length - 1]] = dlg.FileName;

                    listView1.Items.Add(chop_path[chop_path.Length - 1]);
                }
                else { MessageBox.Show("Taki utwur juz zostal dodany"); }
            }
            
        }
        

        private void AxWindowsMediaPlayer1_PlayStateChange(object sender, _WMPOCXEvents_PlayStateChangeEvent e)
        {


            if (!is_played_once)
            {
                Calculate_Divisions(axWindowsMediaPlayer1.currentMedia.duration);
                trackBar1.Maximum = div_bar;
            }
          
            
        }

        private void Volume_Scroll(object sender, EventArgs e)
        {
            axWindowsMediaPlayer1.settings.volume = trackBar2.Value;
            
        }

        private void Right_Swap_Click(object sender, EventArgs e)
        {

            int x = listView1.SelectedItems[0].Index;
            String listItem = listView1.Items[x].Text;
            if (listView1.Items.Count - 1 == listView1.SelectedItems[0].Index)
            {
                listView1.Items[x].Text = listView1.Items[0].Text;
                listView1.Items[0].Text = listItem;
                listView1.SelectedItems.Clear();
                listView1.Items[0].Selected = true;
            }
            else
            {
                listView1.Items[x].Text = listView1.Items[x + 1].Text;
                listView1.Items[x + 1].Text = listItem;
                listView1.SelectedItems.Clear();
                listView1.Items[x + 1].Selected = true;
            }




        }

        private void Left_Swap(object sender, EventArgs e)
        {
            int x = listView1.SelectedItems[0].Index;
            String listItem = listView1.Items[x].Text;
            if (0 == listView1.SelectedItems[0].Index)
            {
                int size = listView1.Items.Count - 1;
                listView1.Items[x].Text = listView1.Items[size].Text;
                listView1.Items[size].Text = listItem;
                listView1.SelectedItems.Clear();
                listView1.Items[size].Selected = true;
            }
            else
            {
                listView1.Items[x].Text = listView1.Items[x - 1].Text;
                listView1.Items[x - 1].Text = listItem;
                listView1.SelectedItems.Clear();
                listView1.Items[x - 1].Selected = true;
            }

        }

        private void ButtonDel_Click(object sender, EventArgs e)
        {
            axWindowsMediaPlayer1.Ctlcontrols.stop();
            timer1.Stop();
            absolut_path.Remove(listView1.SelectedItems[0].Text);
            listView1.Items.RemoveAt(listView1.SelectedItems[0].Index);

        }

        private void Set_Clickable(bool state){
            play_button.Enabled = state;
            button2.Enabled = state;
            button3.Enabled = state;
            Stop.Enabled = state;
            buttonDel.Enabled = state;
        
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            set_buttons_autoplay();
        }
        private void set_buttons_autoplay() { if (checkBox1.Checked) { button2.Enabled = false; button3.Enabled = false; } else { button2.Enabled = true; button3.Enabled = true; } }
    }
}
