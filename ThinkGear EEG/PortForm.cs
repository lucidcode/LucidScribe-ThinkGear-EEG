using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO.Ports;

namespace lucidcode.LucidScribe.Plugin.NeuroSky.MindSet
{
    public partial class PortForm : Form
    {

        public String SelectedPort = "";

        public PortForm()
        {
            InitializeComponent();
        }

        private void PortForm_Load(object sender, EventArgs e)
        {
            foreach (string strPort in SerialPort.GetPortNames())
            {
                String strPortName = strPort;
                strPortName = strPortName.Replace("a", "");
                strPortName = strPortName.Replace("b", "");
                strPortName = strPortName.Replace("c", "");
                strPortName = strPortName.Replace("d", "");
                strPortName = strPortName.Replace("e", "");
                strPortName = strPortName.Replace("f", "");
                strPortName = strPortName.Replace("g", "");
                strPortName = strPortName.Replace("h", "");
                strPortName = strPortName.Replace("i", "");
                strPortName = strPortName.Replace("j", "");
                strPortName = strPortName.Replace("k", "");
                strPortName = strPortName.Replace("l", "");
                strPortName = strPortName.Replace("m", "");
                strPortName = strPortName.Replace("n", "");
                strPortName = strPortName.Replace("o", "");
                strPortName = strPortName.Replace("p", "");
                strPortName = strPortName.Replace("q", "");
                strPortName = strPortName.Replace("r", "");
                strPortName = strPortName.Replace("s", "");
                strPortName = strPortName.Replace("t", "");
                strPortName = strPortName.Replace("u", "");
                strPortName = strPortName.Replace("v", "");
                strPortName = strPortName.Replace("w", "");
                strPortName = strPortName.Replace("x", "");
                strPortName = strPortName.Replace("y", "");
                strPortName = strPortName.Replace("z", "");

                ListViewItem lstItem = new ListViewItem(strPortName);
                lstItem.ImageIndex = 0;
                lstPorts.Items.Add(lstItem);
            }
        }

        private void lstPlaylists_MouseMove(object sender, MouseEventArgs e)
        {
            if (lstPorts.GetItemAt(e.X, e.Y) != null)
            {
                lstPorts.Cursor = Cursors.Hand;
            }
            else
            {
                lstPorts.Cursor = Cursors.Default;
            }
        }

        private void lstPlaylists_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstPorts.SelectedItems.Count > 0)
            {
                SelectedPort = lstPorts.SelectedItems[0].Text;
                DialogResult = DialogResult.OK;
                Close();
            }
        }
    }
}
