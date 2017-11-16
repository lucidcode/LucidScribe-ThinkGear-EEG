﻿using System;
using System.Diagnostics;
using System.IO.Ports;
using System.Threading;
using System.Windows.Forms;
using ThinkGearNET;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Linq;
using System.IO;
using System.Xml;

namespace lucidcode.LucidScribe.Plugin.NeuroSky.MindSet
{

  public static class Device
  {
    private static ThinkGearWrapper _thinkGearWrapper;
    private static bool m_boolInitialized;
    private static bool m_boolInitError;
    public static String Algorithm;
    public static int Threshold;

    private static double m_dblAttention;
    private static double m_dblLastAttention;
    private static double m_dblBlinkStrength;
    private static double m_dblMeditation;
    private static double m_dblLastMeditation;
    private static double m_dblAlpha;
    private static double m_dblLastAlpha;
    private static double m_dblBeta;
    private static double m_dblLastBeta;
    private static double m_dblDelta;
    private static double m_dblLastDelta;
    private static double m_dblGamma;
    private static double m_dblLastGamma;
    private static double m_dblTheta;
    private static double m_dblLastTheta;
    private static double m_dblRaw;

    private static bool ClearDisplay;
    private static bool ClearHighscore;
    private static double DisplayValue;
    private static double HighscoreValue;

    public static Boolean TCMP = false;
    public static Boolean NZT48 = false;

    public static EventHandler<ThinkGearChangedEventArgs> ThinkGearChanged;

    public static Boolean Initialize()
    {
      try
      {
        if (!m_boolInitialized && !m_boolInitError)
        {
          PortForm formPort = new PortForm();
          if (formPort.ShowDialog() == DialogResult.OK)
          {
            try
            {
              _thinkGearWrapper = new ThinkGearWrapper();
              _thinkGearWrapper.ThinkGearChanged += _thinkGearWrapper_ThinkGearChanged;
              if (_thinkGearWrapper.Connect(formPort.SelectedPort, 57600, true))
              {
                _thinkGearWrapper.EnableBlinkDetection(true);
                Algorithm = formPort.Algorithm;
                Threshold = formPort.Threshold;

                TCMP = formPort.TCMP;
                NZT48 = formPort.NZT48;

                m_boolInitialized = true;
              }
              else
              {
                throw new Exception("Could not connect to port " + formPort.SelectedPort + ".");
              }
            }
            catch (Exception ex)
            {
              if (!m_boolInitError)
              {
                MessageBox.Show(ex.Message, "LucidScribe.InitializePlugin()", MessageBoxButtons.OK, MessageBoxIcon.Error);
              }
              m_boolInitError = true;
            }
          }
          else
          {
            m_boolInitError = true;
            return false;
          }
        }
        return true;
      }
      catch (Exception ex)
      {
        m_boolInitError = true;
        throw (new Exception("The 'NeuroSky' plugin failed to initialize: " + ex.Message));
      }
    }

    static void _thinkGearWrapper_ThinkGearChanged(object sender, ThinkGearChangedEventArgs e)
    {
      m_dblLastAttention = e.ThinkGearState.Attention * 10;
      m_dblBlinkStrength = e.ThinkGearState.BlinkStrength * 10;
      m_dblLastMeditation = e.ThinkGearState.Meditation * 10;
      m_dblLastAlpha = ((e.ThinkGearState.Alpha1 / 100) + (e.ThinkGearState.Alpha2 / 100)) / 2;
      m_dblLastBeta = ((e.ThinkGearState.Beta1 / 100) + (e.ThinkGearState.Beta2 / 100)) / 2;
      m_dblLastDelta = e.ThinkGearState.Delta / 10000;
      m_dblLastGamma = ((e.ThinkGearState.Gamma1 / 100) + (e.ThinkGearState.Gamma2 / 100)) / 2;
      m_dblLastTheta = e.ThinkGearState.Theta / 1000;

      if (ClearDisplay)
      {
        ClearDisplay = false;
        DisplayValue = 0;
      }

      if (ClearHighscore)
      {
        ClearHighscore = false;
        DisplayValue = 0;
      }

      m_dblRaw += e.ThinkGearState.Raw;

      if (e.ThinkGearState.Raw >= HighscoreValue)
      {
        HighscoreValue = e.ThinkGearState.Raw;
      }

      if (e.ThinkGearState.Raw >= DisplayValue)
      {
        DisplayValue = e.ThinkGearState.Raw;
      }

      if (ThinkGearChanged != null)
      {
        ThinkGearChanged(sender, e);
      }
    }



    public static void Dispose()
    {
      _thinkGearWrapper.ThinkGearChanged -= _thinkGearWrapper_ThinkGearChanged;
      _thinkGearWrapper.Disconnect();
    }

    public static Double GetEEG()
    {
      double temp = DisplayValue;
      ClearDisplay = true;
      return DisplayValue;
    }

    public static Double GetHighscore()
    {
      double temp = HighscoreValue;
      ClearHighscore = true;
      return HighscoreValue;
    }

    public static Double GetREM()
    {
      return 0;
    }

    public static Double GetAttention()
    {
      if (m_dblLastAttention > m_dblAttention)
      {
        m_dblAttention += (m_dblLastAttention / 100);
      }
      else
      {
        m_dblAttention -= (m_dblLastAttention / 100);
      }
      return m_dblAttention;
    }

    public static Double GetMeditation()
    {
      if (m_dblLastMeditation > m_dblMeditation)
      {
        m_dblMeditation += (m_dblLastMeditation / 100);
      }
      else
      {
        m_dblMeditation -= (m_dblLastMeditation / 100);
      }
      return m_dblMeditation;
    }

    public static Double GetAlpha()
    {
      if (m_dblLastAlpha > m_dblAlpha)
      {
        m_dblAlpha += (m_dblLastAlpha / 100);
      }
      else
      {
        m_dblAlpha -= (m_dblLastAlpha / 100);
      }
      return m_dblAlpha;
    }

    public static Double GetBeta()
    {
      if (m_dblLastBeta > m_dblBeta)
      {
        m_dblBeta += (m_dblLastBeta / 100);
      }
      else
      {
        m_dblBeta -= (m_dblLastBeta / 100);
      }
      return m_dblBeta;
    }

    public static Double GetDelta()
    {
      if (m_dblLastDelta > m_dblDelta)
      {
        m_dblDelta += (m_dblLastDelta / 100);
      }
      else
      {
        m_dblDelta -= (m_dblLastDelta / 100);
      }
      return m_dblDelta;
    }

    public static Double GetGamma()
    {
      if (m_dblLastGamma > m_dblGamma)
      {
        m_dblGamma += (m_dblLastGamma / 100);
      }
      else
      {
        m_dblGamma -= (m_dblLastGamma / 100);
      }
      return m_dblGamma;
    }

    public static Double GetTheta()
    {
      if (m_dblLastTheta > m_dblTheta)
      {
        m_dblTheta += (m_dblLastTheta / 100);
      }
      else
      {
        m_dblTheta -= (m_dblLastTheta / 100);
      }
      return m_dblTheta;
    }

    public static Double GetBlinkStrength()
    {
      return m_dblBlinkStrength;
    }
  }

  namespace EEG
  {
    public class PluginHandler : lucidcode.LucidScribe.Interface.LucidPluginBase
    {
      public override string Name
      {
        get { return "NeuroSky EEG"; }
      }
      public override bool Initialize()
      {
        return Device.Initialize();
      }
      public override double Value
      {
        get
        {
          double dblValue = Device.GetEEG();
          if (dblValue > 999) { dblValue = 999; }
          if (dblValue < 0) { dblValue = 0; }
          return dblValue;
        }
      }
    }
  }

  namespace RAW
  {
    public class PluginHandler : lucidcode.LucidScribe.Interface.ILluminatedPlugin
    {
      public string Name
      {
        get { return "NeuroSky RAW"; }
      }
      public bool Initialize()
      {
        bool initialized = Device.Initialize();
        Device.ThinkGearChanged += _thinkGearWrapper_ThinkGearChanged;
        return initialized;
      }

      public event Interface.SenseHandler Sensed;
      public void _thinkGearWrapper_ThinkGearChanged(object sender, ThinkGearChangedEventArgs e)
      {
        if (ClearTicks)
        {
          ClearTicks = false;
          TickCount = "";
        }
        TickCount += e.ThinkGearState.Raw + ",";

        if (ClearBuffer)
        {
          ClearBuffer = false;
          BufferData = "";
        }
        BufferData += e.ThinkGearState.Raw + ",";
      }

      public void Dispose()
      {
        Device.ThinkGearChanged -= _thinkGearWrapper_ThinkGearChanged;
        Device.Dispose();
      }

      public Boolean isEnabled = false;
      public Boolean Enabled
      {
        get
        {
          return isEnabled;
        }
        set
        {
          isEnabled = value;
        }
      }

      public Color PluginColor = Color.White;
      public Color Color
      {
        get
        {
          return Color;
        }
        set
        {
          Color = value;
        }
      }

      private Boolean ClearTicks = false;
      public String TickCount = "";
      public String Ticks
      {
        get
        {
          ClearTicks = true;
          return TickCount;
        }
        set
        {
          TickCount = value;
        }
      }

      private Boolean ClearBuffer = false;
      public String BufferData = "";
      public String Buffer
      {
        get
        {
          ClearBuffer = true;
          return BufferData;
        }
        set
        {
          BufferData = value;
        }
      }

      int lastHour;
      public int LastHour
      {
        get
        {
          return lastHour;
        }
        set
        {
          lastHour = value;
        }
      }
    }
  }

  namespace RapidEyeMovement
  {
    public class PluginHandler : lucidcode.LucidScribe.Interface.LucidPluginBase
    {
      List<int> m_arrHistory = new List<int>();
      public override string Name
      {
        get { return "NeuroSky REM"; }
      }
      public override bool Initialize()
      {
        return Device.Initialize();
      }
      public override double Value
      {
        get
        {
          double dblEEG = Device.GetEEG();
          if (dblEEG > 999) { dblEEG = 999; }
          if (dblEEG < 0) { dblEEG = 0; }

          if (Device.Algorithm == "REM Detector")
          {
            // Update the mem list
            m_arrHistory.Add(Convert.ToInt32(dblEEG));
            if (m_arrHistory.Count > 512) { m_arrHistory.RemoveAt(0); }

            // Check for blinks
            int intBlinks = 0;
            bool boolBlinking = false;

            int intBelow = 0;
            int intAbove = 0;

            bool boolDreaming = false;
            foreach (Double dblValue in m_arrHistory)
            {
              if (dblValue > Device.Threshold)
              {
                intAbove += 1;
                intBelow = 0;
              }
              else
              {
                intBelow += 1;
                intAbove = 0;
              }

              if (!boolBlinking)
              {
                if (intAbove >= 1)
                {
                  boolBlinking = true;
                  intBlinks += 1;
                  intAbove = 0;
                  intBelow = 0;
                }
              }
              else
              {
                if (intBelow >= 28)
                {
                  boolBlinking = false;
                  intBelow = 0;
                  intAbove = 0;
                }
                else
                {
                  if (intAbove >= 12)
                  {
                    // reset
                    boolBlinking = false;
                    intBlinks = 0;
                    intBelow = 0;
                    intAbove = 0;
                  }
                }
              }

              if (intBlinks > 6)
              {
                boolDreaming = true;
                break;
              }

              if (intAbove > 12)
              { // reset
                boolBlinking = false;
                intBlinks = 0;
                intBelow = 0;
                intAbove = 0; ;
              }
              if (intBelow > 80)
              { // reset
                boolBlinking = false;
                intBlinks = 0;
                intBelow = 0;
                intAbove = 0; ;
              }
            }

            if (boolDreaming)
            {
              return 888;
            }

            if (intBlinks > 10) { intBlinks = 10; }
            return intBlinks * 100;
          }
          else if (Device.Algorithm == "Gamma Detector")
          {
            if (Device.GetGamma() >= Device.Threshold)
            {
              return 888;
            }
          }
          else if (Device.Algorithm == "Motion Detector")
          {
            if (dblEEG >= Device.Threshold)
            {
              return 888;
            }
          }

          return 0;
        }
      }

    }
  }

  namespace TCMP
  {
    public class PluginHandler : lucidcode.LucidScribe.Interface.LucidPluginBase, lucidcode.LucidScribe.TCMP.ITransConsciousnessPlugin
    {

      public override string Name
      {
        get
        {
          return "DRONE TCMP";
        }
      }

      public override bool Initialize()
      {
        try
        {
          return Device.Initialize();
        }
        catch (Exception ex)
        {
          throw (new Exception("The '" + Name + "' plugin failed to initialize: " + ex.Message));
        }
      }

      private static String Morse = "";
      Dictionary<String, String> Code = new Dictionary<String, String>()
          {
              {"UP" , "."},
              {"LEFT" , ".."},
              {"DOWN", "-"},
              {"RIGHT", "--"},
          };

      List<int> m_arrHistory = new List<int>();
      Boolean FirstTick = false;
      Boolean SpaceSent = true;
      int TicksSinceSpace = 0;
      Boolean Started = false;
      int PreliminaryTicks = 0;

      public override double Value
      {
        get
        {
          if (!Device.TCMP) { return 0; }

          double tempValue = Device.GetEEG();
          if (tempValue > 999) { tempValue = 999; }
          if (tempValue < 0) { tempValue = 0; }

          if (!Started)
          {
            PreliminaryTicks++;
            if (PreliminaryTicks > 10)
            {
              Started = true;
            }

            return 0;
          }

          int signalLength = 0;
          int dotHeight = 500;
          int dashHeight = 900;

          // Update the mem list
          String signal = "";

          if (!FirstTick && (tempValue > dotHeight))
          {
            m_arrHistory.Add(Convert.ToInt32(tempValue));
          }

          if (!FirstTick && m_arrHistory.Count > 0)
          {
            m_arrHistory.Add(Convert.ToInt32(tempValue));
          }

          if (FirstTick && (tempValue > dotHeight))
          {
            FirstTick = false;
          }

          if (!SpaceSent & m_arrHistory.Count == 0)
          {
            TicksSinceSpace++;
            if (TicksSinceSpace > 32)
            {
              // Send the space key
              Morse = " ";
              SendKeys.Send(" ");
              SpaceSent = true;
              TicksSinceSpace = 0;
            }
          }

          if (!FirstTick && m_arrHistory.Count > 32)
          {
            int nextOffset = 0;
            do
            {
              int fivePointValue = 0;
              for (int i = nextOffset; i < m_arrHistory.Count; i++)
              {
                for (int x = i; x < m_arrHistory.Count; x++)
                {
                  if (m_arrHistory[x] > fivePointValue)
                  {
                    fivePointValue = m_arrHistory[x];
                  }

                  if (m_arrHistory[x] < 300)
                  {
                    nextOffset = x + 1;
                    break;
                  }

                  if (x == m_arrHistory.Count - 1)
                  {
                    nextOffset = -1;
                  }
                }

                if (fivePointValue >= dashHeight)
                {
                  signal += "-";
                  signalLength++;
                  break;
                }
                else if (fivePointValue >= dotHeight)
                {
                  signal += ".";
                  signalLength++;
                  break;
                }

                if (i == m_arrHistory.Count - 1)
                {
                  nextOffset = -1;
                }

              }

              if (nextOffset < 0 | nextOffset == m_arrHistory.Count)
              {
                break;
              }

            } while (true);

            m_arrHistory.RemoveAt(0);

            // Check if the signal is morse
            try
            {
              // Make sure that we have a signal
              if (signal != "")
              {
                var myValue = Code.First(x => x.Value == signal);
                Morse = myValue.Key.ToString();
                SendKeys.Send("{" + myValue.Key.ToString() +"}");
                signal = "";
                m_arrHistory.Clear();
                SpaceSent = false;
                TicksSinceSpace = 0;
              }
            }
            catch (Exception ex)
            {
              String err = ex.Message;
            }
          }

          if (m_arrHistory.Count > 0)
          { return 888; }

          return 0;
        }
      }

      string lucidcode.LucidScribe.TCMP.ITransConsciousnessPlugin.MorseCode
      {
        get
        {
          String temp = Morse;
          Morse = "";
          return temp;
        }
      }

      public override void Dispose()
      {
        Device.Dispose();
      }

    }
  }

  namespace NZT48
  {
    public class PluginHandler : lucidcode.LucidScribe.Interface.LucidPluginBase
    {

      public override string Name
      {
        get
        {
          return "NZT-48";
        }
      }

      public override bool Initialize()
      {
        try
        {
          return Device.Initialize();
        }
        catch (Exception ex)
        {
          throw (new Exception("The '" + Name + "' plugin failed to initialize: " + ex.Message));
        }
      }

      public override double Value
      {
        get
        {
          if (!Device.NZT48) { return 0; }

          double tempValue = 0;

          if (Device.Algorithm == "Beta")
          {
            tempValue = Device.GetBeta();
          }

          if (tempValue > 999) { tempValue = 999; }
          if (tempValue < 0) { tempValue = 0; }

          if (tempValue > Device.Threshold)
          {
            return 888;
          }

          return 0;
        }
      }

      public override void Dispose()
      {
        Device.Dispose();
      }

    }
  }

  namespace Attention
  {
    public class PluginHandler : lucidcode.LucidScribe.Interface.LucidPluginBase
    {
      public override string Name
      {
        get { return "Attention"; }
      }
      public override bool Initialize()
      {
        return Device.Initialize();
      }
      public override double Value
      {
        get
        {
          double dblValue = Device.GetAttention();
          if (dblValue > 999) { dblValue = 999; }
          return dblValue;
        }
      }
    }
  }

  namespace Meditation
  {
    public class PluginHandler : lucidcode.LucidScribe.Interface.LucidPluginBase
    {
      public override string Name
      {
        get { return "Meditation"; }
      }
      public override bool Initialize()
      {
        return Device.Initialize();
      }
      public override double Value
      {
        get
        {
          double dblValue = Device.GetMeditation();
          if (dblValue > 999) { dblValue = 999; }
          return dblValue;
        }
      }
    }
  }

  namespace Alpha
  {
    public class PluginHandler : lucidcode.LucidScribe.Interface.LucidPluginBase
    {
      public override string Name
      {
        get { return "Alpha"; }
      }
      public override bool Initialize()
      {
        return Device.Initialize();
      }
      public override double Value
      {
        get
        {
          double dblValue = Device.GetAlpha();
          if (dblValue > 999) { dblValue = 999; }
          return dblValue;
        }
      }
    }
  }

  namespace Beta
  {
    public class PluginHandler : lucidcode.LucidScribe.Interface.LucidPluginBase
    {
      public override string Name
      {
        get { return "Beta"; }
      }
      public override bool Initialize()
      {
        return Device.Initialize();
      }
      public override double Value
      {
        get
        {
          double dblValue = Device.GetBeta();
          if (dblValue > 999) { dblValue = 999; }
          return dblValue;
        }
      }
    }
  }

  namespace Delta
  {
    public class PluginHandler : lucidcode.LucidScribe.Interface.LucidPluginBase
    {
      public override string Name
      {
        get { return "Delta"; }
      }
      public override bool Initialize()
      {
        return Device.Initialize();
      }
      public override double Value
      {
        get
        {
          double dblValue = Device.GetDelta();
          if (dblValue > 999) { dblValue = 999; }
          return dblValue;
        }
      }
    }
  }

  namespace Gamma
  {
    public class PluginHandler : lucidcode.LucidScribe.Interface.LucidPluginBase
    {
      public override string Name
      {
        get { return "Gamma"; }
      }
      public override bool Initialize()
      {
        return Device.Initialize();
      }
      public override double Value
      {
        get
        {
          double dblValue = Device.GetGamma();
          if (dblValue > 999) { dblValue = 999; }
          return dblValue;
        }
      }
    }
  }

  namespace Theta
  {
    public class PluginHandler : lucidcode.LucidScribe.Interface.LucidPluginBase
    {
      public override string Name
      {
        get { return "Theta"; }
      }
      public override bool Initialize()
      {
        return Device.Initialize();
      }
      public override double Value
      {
        get
        {
          double dblValue = Device.GetTheta();
          if (dblValue > 999) { dblValue = 999; }
          return dblValue;
        }
      }
    }
  }

  namespace BlinkStrength
  {
    public class PluginHandler : lucidcode.LucidScribe.Interface.LucidPluginBase
    {
      public override string Name
      {
        get { return "Blink Strength"; }
      }
      public override bool Initialize()
      {
        return Device.Initialize();
      }
      public override double Value
      {
        get
        {
          double dblValue = Device.GetBlinkStrength();
          if (dblValue > 999) { dblValue = 999; }
          return dblValue;
        }
      }
    }
  }

  namespace BlinkClick
  {
    public class PluginHandler : lucidcode.LucidScribe.Interface.LucidPluginBase
    {
      [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
      public static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint cButtons, uint dwExtraInfo);

      private const int MOUSEEVENTF_LEFTDOWN = 0x02;
      private const int MOUSEEVENTF_LEFTUP = 0x04;

      public override string Name
      {
        get { return "Blink Click"; }
      }
      public override bool Initialize()
      {
        return Device.Initialize();
      }
      public override double Value
      {
        get
        {
          double dblValue = Device.GetEEG();
          if (dblValue > 999) { dblValue = 999; }
          if (dblValue == 999)
          {
            dblValue = 888;
            uint X = (uint)Cursor.Position.X;
            uint Y = (uint)Cursor.Position.Y;
            mouse_event(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_LEFTUP, X, Y, 0, 0);
          }
          else
          {
            dblValue = 0;
          }
          return dblValue;
        }
      }
    }
  }

}
