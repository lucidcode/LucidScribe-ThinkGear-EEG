using System;
using System.Windows.Forms;
using ThinkGearNET;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Linq;

namespace lucidcode.LucidScribe.Plugin.NeuroSky.MindSet
{
    public static class Device
    {
        private static ThinkGearWrapper thinkGearWrapper;
        private static bool initialized;
        private static bool initError;

        private static double attention;
        private static double lastAttention;
        private static double blinkStrength;
        private static double meditation;
        private static double lastMeditation;
        private static double alpha;
        private static double lastAlpha;
        private static double beta;
        private static double lastBeta;
        private static double delta;
        private static double lastDelta;
        private static double gamma;
        private static double lastGamma;
        private static double theta;
        private static double lastTheta;

        private static bool clearDisplay;
        private static bool clearHighscore;
        private static double displayValue;
        private static double highscoreValue;

        public static EventHandler<ThinkGearChangedEventArgs> ThinkGearChanged;
        
        static PortForm portForm = new PortForm();

        public static Boolean Initialize()
        {
            try
            {
                if (!initialized && !initError)
                {
                    if (portForm.ShowDialog() == DialogResult.OK)
                    {
                        try
                        {
                            thinkGearWrapper = new ThinkGearWrapper();
                            thinkGearWrapper.ThinkGearChanged += _thinkGearWrapper_ThinkGearChanged;
                            if (thinkGearWrapper.Connect(portForm.SelectedPort, 57600, true))
                            {
                                thinkGearWrapper.EnableBlinkDetection(true);
                                initialized = true;
                            }
                            else
                            {
                                throw new Exception("Could not connect to port " + portForm.SelectedPort + ".");
                            }
                        }
                        catch (Exception ex)
                        {
                            if (!initError)
                            {
                                MessageBox.Show(ex.Message, "LucidScribe.InitializePlugin()", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                            initError = true;
                        }
                    }
                    else
                    {
                        initError = true;
                        return false;
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                initError = true;
                throw (new Exception("The 'NeuroSky' plugin failed to initialize: " + ex.Message));
            }
        }

        public static string Algorithm
        {
            get
            {
                return portForm.Algorithm;
            }
        }

        public static int Threshold
        {
            get
            {
                return portForm.Threshold;
            }
        }

        public static bool TCMP
        {
            get
            {
                return portForm.TCMP;
            }
        }

        public static bool NZT48
        {
            get
            {
                return portForm.NZT48;
            }
        }

        public static bool Normalize
        {
            get
            {
                return portForm.Normalize;
            }
        }

        static void _thinkGearWrapper_ThinkGearChanged(object sender, ThinkGearChangedEventArgs e)
        {
            lastAttention = e.ThinkGearState.Attention * 10;
            blinkStrength = e.ThinkGearState.BlinkStrength * 10;
            lastMeditation = e.ThinkGearState.Meditation * 10;
            lastAlpha = ((e.ThinkGearState.Alpha1 / 100) + (e.ThinkGearState.Alpha2 / 100)) / 2;
            lastBeta = ((e.ThinkGearState.Beta1 / 100) + (e.ThinkGearState.Beta2 / 100)) / 2;
            lastDelta = e.ThinkGearState.Delta / 10000;
            lastGamma = ((e.ThinkGearState.Gamma1 / 100) + (e.ThinkGearState.Gamma2 / 100)) / 2;
            lastTheta = e.ThinkGearState.Theta / 1000;

            if (clearDisplay)
            {
                clearDisplay = false;
                displayValue = 0;
            }

            if (clearHighscore)
            {
                clearHighscore = false;
                highscoreValue = 0;
            }

            if (e.ThinkGearState.Raw >= highscoreValue)
            {
                highscoreValue = e.ThinkGearState.Raw;
            }

            if (e.ThinkGearState.Raw >= displayValue)
            {
                displayValue = e.ThinkGearState.Raw;
            }

            if (ThinkGearChanged != null)
            {
                ThinkGearChanged(sender, e);
            }
        }

        public static void Dispose()
        {
            thinkGearWrapper.ThinkGearChanged -= _thinkGearWrapper_ThinkGearChanged;
            thinkGearWrapper.Disconnect();
        }

        public static Double GetEEG()
        {
            double temp = displayValue;
            clearDisplay = true;
            return temp;
        }

        public static Double GetHighscore()
        {
            double temp = highscoreValue;
            clearHighscore = true;
            return temp;
        }

        public static Double GetREM()
        {
            return 0;
        }

        public static Double GetAttention()
        {
            if (lastAttention > attention)
            {
                attention += (lastAttention / 100);
            }
            else
            {
                attention -= (lastAttention / 100);
            }
            return attention;
        }

        public static Double GetMeditation()
        {
            if (lastMeditation > meditation)
            {
                meditation += (lastMeditation / 100);
            }
            else
            {
                meditation -= (lastMeditation / 100);
            }
            return meditation;
        }

        public static Double GetAlpha()
        {
            if (lastAlpha > alpha)
            {
                alpha += (lastAlpha / 100);
            }
            else
            {
                alpha -= (lastAlpha / 100);
            }
            return alpha;
        }

        public static Double GetBeta()
        {
            if (lastBeta > beta)
            {
                beta += (lastBeta / 100);
            }
            else
            {
                beta -= (lastBeta / 100);
            }
            return beta;
        }

        public static Double GetDelta()
        {
            if (lastDelta > delta)
            {
                delta += (lastDelta / 100);
            }
            else
            {
                delta -= (lastDelta / 100);
            }
            return delta;
        }

        public static Double GetGamma()
        {
            if (lastGamma > gamma)
            {
                gamma += (lastGamma / 100);
            }
            else
            {
                gamma -= (lastGamma / 100);
            }
            return gamma;
        }

        public static Double GetTheta()
        {
            if (lastTheta > theta)
            {
                theta += (lastTheta / 100);
            }
            else
            {
                theta -= (lastTheta / 100);
            }
            return theta;
        }

        public static Double GetBlinkStrength()
        {
            return blinkStrength;
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
                    double eegValue = Device.GetEEG();

                    if (Device.Normalize)
                    {
                        if (eegValue > 999) { eegValue = 999; }
                        if (eegValue < 0) { eegValue = 0; }
                    }

                    return eegValue;
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
                    double eegValue = Device.GetEEG();

                    if (Device.Normalize)
                    {
                        if (eegValue > 999) { eegValue = 999; }
                        if (eegValue < 0) { eegValue = 0; }
                    }

                    if (Device.Algorithm == "REM Detector")
                    {
                        // Update the mem list
                        m_arrHistory.Add(Convert.ToInt32(eegValue));
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
                        if (eegValue >= Device.Threshold)
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
                    return "NS TCMP";
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
            Dictionary<char, String> Code = new Dictionary<char, String>()
          {
              {'A' , ".-"},
              {'B' , "-..."},
              {'C' , "-.-."},
              {'D' , "-.."},
              {'E' , "."},
              {'F' , "..-."},
              {'G' , "--."},
              {'H' , "...."},
              {'I' , ".."},
              {'J' , ".---"},
              {'K' , "-.-"},
              {'L' , ".-.."},
              {'M' , "--"},
              {'N' , "-."},
              {'O' , "---"},
              {'P' , ".--."},
              {'Q' , "--.-"},
              {'R' , ".-."},
              {'S' , "..."},
              {'T' , "-"},
              {'U' , "..-"},
              {'V' , "...-"},
              {'W' , ".--"},
              {'X' , "-..-"},
              {'Y' , "-.--"},
              {'Z' , "--.."},
              {'0' , "-----"},
              {'1' , ".----"},
              {'2' , "..----"},
              {'3' , "...--"},
              {'4' , "....-"},
              {'5' , "....."},
              {'6' , "-...."},
              {'7' , "--..."},
              {'8' , "---.."},
              {'9' , "----."},
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

                    double eegValue = Device.GetEEG();

                    if (Device.Normalize)
                    {
                        if (eegValue > 999) { eegValue = 999; }
                        if (eegValue < 0) { eegValue = 0; }
                    }

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

                    if (!FirstTick && (eegValue > dotHeight))
                    {
                        m_arrHistory.Add(Convert.ToInt32(eegValue));
                    }

                    if (!FirstTick && m_arrHistory.Count > 0)
                    {
                        m_arrHistory.Add(Convert.ToInt32(eegValue));
                    }

                    if (FirstTick && (eegValue > dotHeight))
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
                                SendKeys.Send(myValue.Key.ToString());
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

                    double betaValue = 0;

                    if (Device.Algorithm == "Beta")
                    {
                        betaValue = Device.GetBeta();
                    }

                    if (Device.Normalize)
                    {
                        if (betaValue > 999) { betaValue = 999; }
                        if (betaValue < 0) { betaValue = 0; }
                    }

                    if (betaValue > Device.Threshold)
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
                    double attentionValue = Device.GetAttention();

                    if (Device.Normalize)
                    {
                        if (attentionValue > 999) { attentionValue = 999; }
                        if (attentionValue < 0) { attentionValue = 0; }
                    }

                    return attentionValue;
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
                    double meditationValue = Device.GetMeditation();

                    if (Device.Normalize)
                    {
                        if (meditationValue > 999) { meditationValue = 999; }
                        if (meditationValue < 0) { meditationValue = 0; }
                    }

                    return meditationValue;
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
                    double alphaValue = Device.GetAlpha();

                    if (Device.Normalize)
                    {
                        if (alphaValue > 999) { alphaValue = 999; }
                        if (alphaValue < 0) { alphaValue = 0; }
                    }

                    return alphaValue;
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
                    double betaValue = Device.GetBeta();

                    if (Device.Normalize)
                    {
                        if (betaValue > 999) { betaValue = 999; }
                        if (betaValue < 0) { betaValue = 0; }
                    }

                    return betaValue;
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
                    double deltaValue = Device.GetDelta();

                    if (Device.Normalize)
                    {
                        if (deltaValue > 999) { deltaValue = 999; }
                        if (deltaValue < 0) { deltaValue = 0; }
                    }

                    return deltaValue;
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
                    double gammaValue = Device.GetGamma();

                    if (Device.Normalize)
                    {
                        if (gammaValue > 999) { gammaValue = 999; }
                        if (gammaValue < 0) { gammaValue = 0; }
                    }

                    return gammaValue;
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
                    double thetaValue = Device.GetTheta();

                    if (Device.Normalize)
                    {
                        if (thetaValue > 999) { thetaValue = 999; }
                        if (thetaValue < 0) { thetaValue = 0; }
                    }

                    return thetaValue;
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
                    double blinkStrengthValue = Device.GetBlinkStrength();

                    if (Device.Normalize)
                    {
                        if (blinkStrengthValue > 999) { blinkStrengthValue = 999; }
                        if (blinkStrengthValue < 0) { blinkStrengthValue = 0; }
                    }

                    return blinkStrengthValue;
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
                    double eegValue = Device.GetEEG();

                    if (eegValue > 999) { eegValue = 999; }

                    if (eegValue == 999)
                    {
                        eegValue = 888;
                        uint X = (uint)Cursor.Position.X;
                        uint Y = (uint)Cursor.Position.Y;
                        mouse_event(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_LEFTUP, X, Y, 0, 0);
                    }
                    else
                    {
                        eegValue = 0;
                    }

                    return eegValue;
                }
            }
        }
    }

}
