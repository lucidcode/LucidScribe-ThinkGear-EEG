using System;
using System.IO.Ports;
using System.Threading;
using System.Windows.Forms;
using ThinkGearNET;

namespace ThinkGearNETTest
{
	public partial class Form1 : Form
	{
		private ThinkGearWrapper _thinkGearWrapper = new ThinkGearWrapper();

		public Form1()
		{
			InitializeComponent();
		}

		private void Form1_Load(object sender, EventArgs e)
    {
      cboPort.Items.Add("COM4");
      cboPort.Items.Add("COM5");
			foreach(string port in SerialPort.GetPortNames())
			{
        String strPortName = port;
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
        cboPort.Items.Add(strPortName);
    }
			cboPort.SelectedIndex = 0;
		}

		private void btnConnect_Click(object sender, EventArgs e)
		{
			_thinkGearWrapper = new ThinkGearWrapper();

			// setup the event
			_thinkGearWrapper.ThinkGearChanged += _thinkGearWrapper_ThinkGearChanged;

			// connect to the device on the specified COM port at 57600 baud
			if(!_thinkGearWrapper.Connect(cboPort.SelectedItem.ToString(), 57600, true))
				MessageBox.Show("Could not connect to headset.");
		}

		void _thinkGearWrapper_ThinkGearChanged(object sender, ThinkGearChangedEventArgs e)
		{
			// update the textbox and sleep for a tiny bit
			BeginInvoke(new MethodInvoker(delegate 
				{
					lblAttention.Text = "Attention: " + e.ThinkGearState.Attention;
					lblMeditation.Text = "Meditation: " + e.ThinkGearState.Meditation;
					txtState.Text = e.ThinkGearState.ToString();
				}));
			Thread.Sleep(10);
		}

		private void Form1_FormClosing(object sender, FormClosingEventArgs e)
		{
			_thinkGearWrapper.Disconnect();
		}

		private void btnDisconnect_Click(object sender, EventArgs e)
		{
			_thinkGearWrapper.Disconnect();
		}

		private void btnEnableBlink_Click(object sender, EventArgs e)
		{
			_thinkGearWrapper.EnableBlinkDetection(true);
		}

		private void btnDisableBlink_Click(object sender, EventArgs e)
		{
			_thinkGearWrapper.EnableBlinkDetection(false);
		}
	}
}
