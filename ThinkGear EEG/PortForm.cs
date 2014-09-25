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
        public static string m_strPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\lucidcode\\Lucid Scribe\\";
        public String SelectedPort = "";
        public String Algorithm = "REM Detector";
        public int Threshold = 800;
        private Boolean loaded = false;
        public Boolean TCMP = false;
        public Boolean NZT48 = false;

        public Boolean Arduino = false;
        public String ArduinoPort = "COM1";
        public String ArduinoDelay = "1";
        public String ArduinoOn = "1";
        public String ArduinoOff = "0";

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
          cmbArduinoPort.Items.Clear();
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

            cmbArduinoPort.Items.Add(strPortName);

            ListViewItem lstItem = new ListViewItem(strPortName);
            lstItem.ImageIndex = 0;
            lstPorts.Items.Add(lstItem);
          }
        }

        private void LoadSettings()
        {
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

          XmlDocument xmlSettings = new XmlDocument();
          xmlSettings.Load(m_strPath + "Plugins\\ThinkGear.EEG.User.lsd");

          cmbAlgorithm.Text = xmlSettings.DocumentElement.SelectSingleNode("//Algorithm").InnerText;
          cmbThreshold.Text = xmlSettings.DocumentElement.SelectSingleNode("//Threshold").InnerText;

          if (xmlSettings.DocumentElement.SelectSingleNode("//NZT48") != null && xmlSettings.DocumentElement.SelectSingleNode("//NZT48").InnerText == "1")
          {
            chkNZT48.Checked = true;
            NZT48 = true;
          }

          if (File.Exists(m_strPath + "Plugins\\NZT-48.video.lsd"))
          {
            txtVideo.Text = File.ReadAllText(m_strPath + "Plugins\\NZT-48.video.lsd");
          }

          if (xmlSettings.DocumentElement.SelectSingleNode("//Arduino") != null && xmlSettings.DocumentElement.SelectSingleNode("//Arduino").InnerText == "1")
          {
            chkArduino.Checked = true;
            Arduino = true;
          }

          if (xmlSettings.DocumentElement.SelectSingleNode("//ArduinoPort") != null)
          {
            cmbArduinoPort.Text = xmlSettings.DocumentElement.SelectSingleNode("//ArduinoPort").InnerText;
            ArduinoPort = xmlSettings.DocumentElement.SelectSingleNode("//ArduinoPort").InnerText;
          }

          if (xmlSettings.DocumentElement.SelectSingleNode("//ArduinoOn") != null)
          {
            txtArduinoOn.Text = xmlSettings.DocumentElement.SelectSingleNode("//ArduinoOn").InnerText;
            ArduinoOn = xmlSettings.DocumentElement.SelectSingleNode("//ArduinoOn").InnerText;
          }

          if (xmlSettings.DocumentElement.SelectSingleNode("//ArduinoOff") != null)
          {
            txtArduinoOff.Text = xmlSettings.DocumentElement.SelectSingleNode("//ArduinoOff").InnerText;
            ArduinoOff = xmlSettings.DocumentElement.SelectSingleNode("//ArduinoOff").InnerText;
          }

          if (xmlSettings.DocumentElement.SelectSingleNode("//ArduinoDelay") != null)
          {
            cmbArduinoDelay.Text = xmlSettings.DocumentElement.SelectSingleNode("//ArduinoDelay").InnerText;
            ArduinoDelay = xmlSettings.DocumentElement.SelectSingleNode("//ArduinoDelay").InnerText;
          }

          if (xmlSettings.DocumentElement.SelectSingleNode("//TCMP") != null && xmlSettings.DocumentElement.SelectSingleNode("//TCMP").InnerText == "1")
          {
            chkTCMP.Checked = true;
            TCMP = true;
          }
        }

        private void SaveSettings()
        {
          String settingsXML = "<LucidScribeData>";
          settingsXML += "<Plugin>";
          settingsXML += "<Algorithm>" + cmbAlgorithm.Text + "</Algorithm>";
          settingsXML += "<Threshold>" + cmbThreshold.Text + "</Threshold>";

          if (chkNZT48.Checked)
          {
            settingsXML += "<NZT48>1</NZT48>";
          }
          else
          {
            settingsXML += "<NZT48>0</NZT48>";
          }

          if (chkArduino.Checked)
          {
            settingsXML += "<Arduino>1</Arduino>";
          }
          else
          {
            settingsXML += "<Arduino>0</Arduino>";
          }

          settingsXML += "<ArduinoPort>" + cmbArduinoPort.Text + "</ArduinoPort>";
          settingsXML += "<ArduinoDelay>" + cmbArduinoDelay.Text + "</ArduinoDelay>";
          settingsXML += "<ArduinoOn>" + txtArduinoOn.Text + "</ArduinoOn>";
          settingsXML += "<ArduinoOff>" + txtArduinoOff.Text + "</ArduinoOff>";

          if (chkTCMP.Checked)
          {
            settingsXML += "<TCMP>1</TCMP>";
          }
          else
          {
            settingsXML += "<TCMP>0</TCMP>";
          }

          settingsXML += "</Plugin>";
          settingsXML += "</LucidScribeData>";
          File.WriteAllText(m_strPath + "Plugins\\ThinkGear.EEG.User.lsd", settingsXML);
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

        private void chkNZT48_CheckedChanged(object sender, EventArgs e)
        {
          if (!loaded) { return; }

          NZT48 = chkNZT48.Checked;
          SaveSettings();
        }

        private void txtVideo_TextChanged(object sender, EventArgs e)
        {
          SaveNZT48Settings();
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
          OpenFileDialog openFileDialog = new OpenFileDialog();
          openFileDialog.Title = "Select NZT-48 Video";
          if (openFileDialog.ShowDialog() == DialogResult.OK)
          {
            txtVideo.Text = openFileDialog.FileName;
          }
        }

        private void SaveNZT48Settings()
        {
          File.WriteAllText(m_strPath + "Plugins\\NZT-48.video.lsd", txtVideo.Text);
        }

        private void chkArduino_CheckedChanged(object sender, EventArgs e)
        {
          if (!loaded) { return; }

          Arduino = chkArduino.Checked;
          SaveSettings();
        }

        private void cmbArduinoPort_SelectedIndexChanged(object sender, EventArgs e)
        {
          if (!loaded) { return; }

          ArduinoPort = cmbArduinoPort.Text;
          SaveSettings();
        }

        private void cmbArduinoDelay_SelectedIndexChanged(object sender, EventArgs e)
        {
          if (!loaded) { return; }

          ArduinoDelay = cmbArduinoDelay.Text;
          SaveSettings();
        }

        private void txtArduinoOn_TextChanged(object sender, EventArgs e)
        {
          if (!loaded) { return; }

          ArduinoOn = txtArduinoOn.Text;
          SaveSettings();
        }

        private void txtArduinoOff_TextChanged(object sender, EventArgs e)
        {
          if (!loaded) { return; }

          ArduinoOff = txtArduinoOff.Text;
          SaveSettings();
        }
    }
}
