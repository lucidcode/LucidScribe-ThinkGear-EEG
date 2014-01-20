using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO.Ports;
using System.Xml;
using System.IO;

namespace lucidcode.LucidScribe.Plugin.NeuroSky.MindSet
{
    public partial class PortForm : Form
    {

        private string m_strPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\lucidcode\\Lucid Scribe\\";
        public String SelectedPort = "";
        public String Algorithm = "REM Detector";
        public int Threshold = 800;
        private Boolean loaded = false;
        public Boolean TCMP = false;

        public PortForm()
        {
            InitializeComponent();
        }

        private void PortForm_Load(object sender, EventArgs e)
        {
          LoadPortList();
          LoadSettings();
          loaded = true;
        }

        private void LoadPortList()
        {
          lstPorts.Clear();
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

        private void LoadSettings()
        {
          XmlDocument xmlSettings = new XmlDocument();

          if (!File.Exists(m_strPath + "Plugins\\ThinkGear.EEG.User.lsd"))
          {
            String defaultSettings = "<LucidScribeData>";
            defaultSettings += "<Plugin>";
            defaultSettings += "<Algorithm>REM Detector</Algorithm>";
            defaultSettings += "<Threshold>800</Threshold>";
            defaultSettings += "</Plugin>";
            defaultSettings += "</LucidScribeData>";
            File.WriteAllText(m_strPath + "Plugins\\ThinkGear.EEG.User.lsd", defaultSettings);
          }

          xmlSettings.Load(m_strPath + "Plugins\\ThinkGear.EEG.User.lsd");

          cmbAlgorithm.Text = xmlSettings.DocumentElement.SelectSingleNode("//Algorithm").InnerText;
          cmbThreshold.Text = xmlSettings.DocumentElement.SelectSingleNode("//Threshold").InnerText;

          if (xmlSettings.DocumentElement.SelectSingleNode("//TCMP") != null && xmlSettings.DocumentElement.SelectSingleNode("//TCMP").InnerText == "1")
          {
            chkTCMP.Checked = true;
          }
        }

        private void SaveSettings()
        {
          String defaultSettings = "<LucidScribeData>";
          defaultSettings += "<Plugin>";
          defaultSettings += "<Algorithm>" + cmbAlgorithm.Text + "</Algorithm>";
          defaultSettings += "<Threshold>" + cmbThreshold.Text + "</Threshold>";

          if (chkTCMP.Checked)
          {
            defaultSettings += "<TCMP>1</TCMP>";
          }
          else
          {
            defaultSettings += "<TCMP>0</TCMP>";
          }

          defaultSettings += "</Plugin>";
          defaultSettings += "</LucidScribeData>";
          File.WriteAllText(m_strPath + "Plugins\\ThinkGear.EEG.User.lsd", defaultSettings);
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

        private void mnuRefresh_Click(object sender, EventArgs e)
        {
          LoadPortList();
        }

        private void cmbAlgorithm_SelectedIndexChanged(object sender, EventArgs e)
        {
          Algorithm = cmbAlgorithm.Text;
          if (loaded) { SaveSettings(); }
        }

        private void cmbThreshold_SelectedIndexChanged(object sender, EventArgs e)
        {
          Threshold = Convert.ToInt32(cmbThreshold.Text);
          if (loaded) { SaveSettings(); }
        }

        private void chkTCMP_CheckedChanged(object sender, EventArgs e)
        {
          if (!loaded) { return; }

          TCMP = chkTCMP.Checked;
          SaveSettings();
        }
    }
}
