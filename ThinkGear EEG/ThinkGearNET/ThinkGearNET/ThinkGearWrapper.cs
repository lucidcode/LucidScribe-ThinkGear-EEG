using System;
using System.Diagnostics;
using System.Text;
using System.Threading;

namespace ThinkGearNET
{
	public class ThinkGearWrapper
	{
		private int _connectionId;
		private bool _connected;
		private bool _polling;
		private int _errors;
		private AutoResetEvent _event;
		private ThinkGearState _tgState = new ThinkGearState();

		public event EventHandler<ThinkGearChangedEventArgs> ThinkGearChanged;

		public ThinkGearState ThinkGearState
		{
			get { return _tgState; }
		}

		public bool Connect(string comPort, int baud, bool async, bool trace)
		{
			// get the driver version
			_tgState.Version = ThinkGear.TG_GetDriverVersion();
			LogText("ThinkGear DLL Version: " + _tgState.Version);

			// get a new connection ID
			_connectionId = ThinkGear.TG_GetNewConnectionId();
			LogText("Connection ID: " + _connectionId);

			// if < 0, we have an error
			if(_connectionId < 0)
				return false;

			// turn on tracing
			if(trace)
			{
				ThinkGear.TG_SetStreamLog(_connectionId, "streamLog.txt");
				ThinkGear.TG_SetDataLog(_connectionId, "dataLog.txt");
			}

			// connect to the device
      int connect = ThinkGear.TG_Connect(_connectionId, @"\\.\" + comPort, baud, ThinkGear.STREAM_PACKETS);
			LogText("Connect: " + connect);

			// if < 0, we have an error
			if(connect < 0)
				return false;

			_connected = true;

			// if async, start a new polling thread
			if(async)
			{
				_event = new AutoResetEvent(false);

				_polling = true;
				Thread t = new Thread(PollThinkGear);
				t.IsBackground = true;
				t.Start();
			}

			return true;
		}

		public bool Connect(string comPort, int baud, bool async)
		{
			return Connect(comPort, baud, async, false);
		}

		public void Disconnect()
		{
			if(!_connected)
				return;

			_polling = false;

			// wait for the poller to end
			if(_event != null)
				_event.WaitOne(5000, false);

			// close it up
			ThinkGear.TG_FreeConnection(_connectionId);
			_connected = false;
		}

		public bool EnableBlinkDetection(bool enable)
		{
			if(!enable)
				_tgState.BlinkStrength = 0;

			return ThinkGear.TG_EnableBlinkDetection(_connectionId, enable ? 1 : 0) == 0;
		}

		private void PollThinkGear()
		{
			while(_polling)
			{
				// update the current state
        UpdateState();

			}
			_event.Set();
		}

		public void UpdateState()
		{
			if(!_connected)
				return;

			// read all packets
			_tgState.PacketsRead = ThinkGear.TG_ReadPackets(_connectionId, 1);

			// if < 0, we have an error
			if(_tgState.PacketsRead < 0)
				_errors++;
			else
				_errors = 0;

			// a few errors may be normal.  if we get a LOT, we have a problem
			_tgState.Error = _errors > 25000;

			// if we read at least one packet, update the state values
			if(_tgState.PacketsRead > 0)
			{
				_tgState.Battery	        = GetValue(ThinkGear.DATA_BATTERY) ?? _tgState.Battery;
				_tgState.PoorSignal	        = GetValue(ThinkGear.DATA_POOR_SIGNAL) ?? _tgState.PoorSignal;
				_tgState.Attention	        = GetValue(ThinkGear.DATA_ATTENTION) ?? _tgState.Attention;
				_tgState.Meditation         = GetValue(ThinkGear.DATA_MEDITATION) ?? _tgState.Meditation;
				_tgState.Raw		        = GetValue(ThinkGear.DATA_RAW) ?? _tgState.Raw;
				_tgState.Delta		        = GetValue(ThinkGear.DATA_DELTA) ?? _tgState.Delta;
				_tgState.Theta		        = GetValue(ThinkGear.DATA_THETA) ?? _tgState.Theta;
				_tgState.Alpha1		        = GetValue(ThinkGear.DATA_ALPHA1) ?? _tgState.Alpha1;
				_tgState.Alpha2		        = GetValue(ThinkGear.DATA_ALPHA2) ?? _tgState.Alpha2;
				_tgState.Beta1		        = GetValue(ThinkGear.DATA_BETA1) ?? _tgState.Beta1;
				_tgState.Beta2		        = GetValue(ThinkGear.DATA_BETA2) ?? _tgState.Beta2;
				_tgState.Gamma1		        = GetValue(ThinkGear.DATA_GAMMA1) ?? _tgState.Gamma1;
				_tgState.Gamma2		        = GetValue(ThinkGear.DATA_GAMMA2) ?? _tgState.Gamma2;
        _tgState.BlinkStrength = GetValue(ThinkGear.DATA_BLINK_STRENGTH) ?? _tgState.BlinkStrength;

        // let those listening know
        if (ThinkGearChanged != null)
          ThinkGearChanged(this, new ThinkGearChangedEventArgs(_tgState));
				
      }
		}

		private float? GetValue(int type)
		{
			// if the value has been updated, get the value and return it
		    if(ThinkGear.TG_GetValueStatus(_connectionId, type) != 0)
		        return ThinkGear.TG_GetValue(_connectionId, type);

			return null;
		}

		private void LogText(string text)
		{
			Debug.WriteLine(text);
		}
	}

	public class ThinkGearState
	{
		public int Version;
		public int PacketsRead;
		public float Battery;
		public float PoorSignal;
		public float Attention;
		public float Meditation;
		public float Raw;
		public float Delta;
		public float Theta;
		public float Alpha1;
		public float Alpha2;
		public float Beta1;
		public float Beta2;
		public float Gamma1;
		public float Gamma2;
		public float BlinkStrength;
		public bool Error;

		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			sb.AppendFormat("Version: {0}\r\n", Version);
			sb.AppendFormat("Error: {0}\r\n", Error);
			sb.AppendFormat("Packets read: {0}\r\n", PacketsRead);
			sb.AppendFormat("Battery: {0}\r\n", Battery);
			sb.AppendFormat("PoorSignal: {0}\r\n", PoorSignal);
			sb.AppendFormat("Attention: {0}\r\n", Attention);
			sb.AppendFormat("Meditation: {0}\r\n", Meditation);
			sb.AppendFormat("Raw: {0}\r\n", Raw);
			sb.AppendFormat("Delta: {0}\r\n", Delta);
			sb.AppendFormat("Theta: {0}\r\n", Theta);
			sb.AppendFormat("Alpha1: {0}\r\n", Alpha1);
			sb.AppendFormat("Alpha2: {0}\r\n", Alpha2);
			sb.AppendFormat("Beta1: {0}\r\n", Beta1);
			sb.AppendFormat("Beta2: {0}\r\n", Beta2);
			sb.AppendFormat("Gamma1: {0}\r\n", Gamma1);
			sb.AppendFormat("Gamma2: {0}\r\n", Gamma2);
			sb.AppendFormat("BlinkStrength: {0}\r\n", BlinkStrength);
			return sb.ToString();
		}
	}

	public class ThinkGearChangedEventArgs : EventArgs
	{
		public ThinkGearState ThinkGearState { get; set; }

		public ThinkGearChangedEventArgs(ThinkGearState state)
		{
			ThinkGearState = state;
		}
	}
}
