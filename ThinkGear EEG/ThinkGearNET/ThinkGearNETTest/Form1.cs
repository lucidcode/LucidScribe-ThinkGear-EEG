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
			foreach(string port in SerialPort.GetPortNames())
				cboPort.Items.Add(port);
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
