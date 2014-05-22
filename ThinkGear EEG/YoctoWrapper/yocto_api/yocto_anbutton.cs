/*********************************************************************
 *
 * $Id: yocto_anbutton.cs 15251 2014-03-06 10:14:33Z seb $
 *
 * Implements yFindAnButton(), the high-level API for AnButton functions
 *
 * - - - - - - - - - License information: - - - - - - - - - 
 *
 *  Copyright (C) 2011 and beyond by Yoctopuce Sarl, Switzerland.
 *
 *  Yoctopuce Sarl (hereafter Licensor) grants to you a perpetual
 *  non-exclusive license to use, modify, copy and integrate this
 *  file into your software for the sole purpose of interfacing
 *  with Yoctopuce products.
 *
 *  You may reproduce and distribute copies of this file in
 *  source or object form, as long as the sole purpose of this
 *  code is to interface with Yoctopuce products. You must retain
 *  this notice in the distributed source file.
 *
 *  You should refer to Yoctopuce General Terms and Conditions
 *  for additional information regarding your rights and
 *  obligations.
 *
 *  THE SOFTWARE AND DOCUMENTATION ARE PROVIDED 'AS IS' WITHOUT
 *  WARRANTY OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING 
 *  WITHOUT LIMITATION, ANY WARRANTY OF MERCHANTABILITY, FITNESS
 *  FOR A PARTICULAR PURPOSE, TITLE AND NON-INFRINGEMENT. IN NO
 *  EVENT SHALL LICENSOR BE LIABLE FOR ANY INCIDENTAL, SPECIAL,
 *  INDIRECT OR CONSEQUENTIAL DAMAGES, LOST PROFITS OR LOST DATA,
 *  COST OF PROCUREMENT OF SUBSTITUTE GOODS, TECHNOLOGY OR 
 *  SERVICES, ANY CLAIMS BY THIRD PARTIES (INCLUDING BUT NOT 
 *  LIMITED TO ANY DEFENSE THEREOF), ANY CLAIMS FOR INDEMNITY OR
 *  CONTRIBUTION, OR OTHER SIMILAR COSTS, WHETHER ASSERTED ON THE
 *  BASIS OF CONTRACT, TORT (INCLUDING NEGLIGENCE), BREACH OF
 *  WARRANTY, OR OTHERWISE.
 *
 *********************************************************************/


using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Text;
using YDEV_DESCR = System.Int32;
using YFUN_DESCR = System.Int32;

namespace YocoWrapper
{
  //--- (YAnButton return codes)
  //--- (end of YAnButton return codes)
  //--- (YAnButton class start)
  /**
   * <summary>
   *   Yoctopuce application programming interface allows you to measure the state
   *   of a simple button as well as to read an analog potentiometer (variable resistance).
   * <para>
   *   This can be use for instance with a continuous rotating knob, a throttle grip
   *   or a joystick. The module is capable to calibrate itself on min and max values,
   *   in order to compute a calibrated value that varies proportionally with the
   *   potentiometer position, regardless of its total resistance.
   * </para>
   * <para>
   * </para>
   * </summary>
   */
  public class YAnButton : YFunction
  {
    //--- (end of YAnButton class start)
    //--- (YAnButton definitions)
    public new delegate void ValueCallback(YAnButton func, string value);
    public new delegate void TimedReportCallback(YAnButton func, YMeasure measure);

    public const int CALIBRATEDVALUE_INVALID = YAPI.INVALID_UINT;
    public const int RAWVALUE_INVALID = YAPI.INVALID_UINT;
    public const int ANALOGCALIBRATION_OFF = 0;
    public const int ANALOGCALIBRATION_ON = 1;
    public const int ANALOGCALIBRATION_INVALID = -1;

    public const int CALIBRATIONMAX_INVALID = YAPI.INVALID_UINT;
    public const int CALIBRATIONMIN_INVALID = YAPI.INVALID_UINT;
    public const int SENSITIVITY_INVALID = YAPI.INVALID_UINT;
    public const int ISPRESSED_FALSE = 0;
    public const int ISPRESSED_TRUE = 1;
    public const int ISPRESSED_INVALID = -1;

    public const long LASTTIMEPRESSED_INVALID = YAPI.INVALID_LONG;
    public const long LASTTIMERELEASED_INVALID = YAPI.INVALID_LONG;
    public const long PULSECOUNTER_INVALID = YAPI.INVALID_LONG;
    public const long PULSETIMER_INVALID = YAPI.INVALID_LONG;
    protected int _calibratedValue = CALIBRATEDVALUE_INVALID;
    protected int _rawValue = RAWVALUE_INVALID;
    protected int _analogCalibration = ANALOGCALIBRATION_INVALID;
    protected int _calibrationMax = CALIBRATIONMAX_INVALID;
    protected int _calibrationMin = CALIBRATIONMIN_INVALID;
    protected int _sensitivity = SENSITIVITY_INVALID;
    protected int _isPressed = ISPRESSED_INVALID;
    protected long _lastTimePressed = LASTTIMEPRESSED_INVALID;
    protected long _lastTimeReleased = LASTTIMERELEASED_INVALID;
    protected long _pulseCounter = PULSECOUNTER_INVALID;
    protected long _pulseTimer = PULSETIMER_INVALID;
    protected ValueCallback _valueCallbackAnButton = null;
    //--- (end of YAnButton definitions)

    public YAnButton(string func)
      : base(func)
    {
      _className = "AnButton";
      //--- (YAnButton attributes initialization)
      //--- (end of YAnButton attributes initialization)
    }

    //--- (YAnButton implementation)

    protected override void _parseAttr(YAPI.TJSONRECORD member)
    {
      if (member.name == "calibratedValue")
      {
        _calibratedValue = (int)member.ivalue;
        return;
      }
      if (member.name == "rawValue")
      {
        _rawValue = (int)member.ivalue;
        return;
      }
      if (member.name == "analogCalibration")
      {
        _analogCalibration = member.ivalue > 0 ? 1 : 0;
        return;
      }
      if (member.name == "calibrationMax")
      {
        _calibrationMax = (int)member.ivalue;
        return;
      }
      if (member.name == "calibrationMin")
      {
        _calibrationMin = (int)member.ivalue;
        return;
      }
      if (member.name == "sensitivity")
      {
        _sensitivity = (int)member.ivalue;
        return;
      }
      if (member.name == "isPressed")
      {
        _isPressed = member.ivalue > 0 ? 1 : 0;
        return;
      }
      if (member.name == "lastTimePressed")
      {
        _lastTimePressed = member.ivalue;
        return;
      }
      if (member.name == "lastTimeReleased")
      {
        _lastTimeReleased = member.ivalue;
        return;
      }
      if (member.name == "pulseCounter")
      {
        _pulseCounter = member.ivalue;
        return;
      }
      if (member.name == "pulseTimer")
      {
        _pulseTimer = member.ivalue;
        return;
      }
      base._parseAttr(member);
    }

    /**
     * <summary>
     *   Returns the current calibrated input value (between 0 and 1000, included).
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   an integer corresponding to the current calibrated input value (between 0 and 1000, included)
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YAnButton.CALIBRATEDVALUE_INVALID</c>.
     * </para>
     */
    public int get_calibratedValue()
    {
      if (this._cacheExpiration <= YAPI.GetTickCount())
      {
        if (this.load(YAPI.DefaultCacheValidity) != YAPI.SUCCESS)
        {
          return CALIBRATEDVALUE_INVALID;
        }
      }
      return this._calibratedValue;
    }

    /**
     * <summary>
     *   Returns the current measured input value as-is (between 0 and 4095, included).
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   an integer corresponding to the current measured input value as-is (between 0 and 4095, included)
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YAnButton.RAWVALUE_INVALID</c>.
     * </para>
     */
    public int get_rawValue()
    {
      if (this._cacheExpiration <= YAPI.GetTickCount())
      {
        if (this.load(YAPI.DefaultCacheValidity) != YAPI.SUCCESS)
        {
          return RAWVALUE_INVALID;
        }
      }
      return this._rawValue;
    }

    /**
     * <summary>
     *   Tells if a calibration process is currently ongoing.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   either <c>YAnButton.ANALOGCALIBRATION_OFF</c> or <c>YAnButton.ANALOGCALIBRATION_ON</c>
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YAnButton.ANALOGCALIBRATION_INVALID</c>.
     * </para>
     */
    public int get_analogCalibration()
    {
      if (this._cacheExpiration <= YAPI.GetTickCount())
      {
        if (this.load(YAPI.DefaultCacheValidity) != YAPI.SUCCESS)
        {
          return ANALOGCALIBRATION_INVALID;
        }
      }
      return this._analogCalibration;
    }

    /**
     * <summary>
     *   Starts or stops the calibration process.
     * <para>
     *   Remember to call the <c>saveToFlash()</c>
     *   method of the module at the end of the calibration if the modification must be kept.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="newval">
     *   either <c>YAnButton.ANALOGCALIBRATION_OFF</c> or <c>YAnButton.ANALOGCALIBRATION_ON</c>
     * </param>
     * <para>
     * </para>
     * <returns>
     *   <c>YAPI.SUCCESS</c> if the call succeeds.
     * </returns>
     * <para>
     *   On failure, throws an exception or returns a negative error code.
     * </para>
     */
    public int set_analogCalibration(int newval)
    {
      string rest_val;
      rest_val = (newval > 0 ? "1" : "0");
      return _setAttr("analogCalibration", rest_val);
    }

    /**
     * <summary>
     *   Returns the maximal value measured during the calibration (between 0 and 4095, included).
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   an integer corresponding to the maximal value measured during the calibration (between 0 and 4095, included)
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YAnButton.CALIBRATIONMAX_INVALID</c>.
     * </para>
     */
    public int get_calibrationMax()
    {
      if (this._cacheExpiration <= YAPI.GetTickCount())
      {
        if (this.load(YAPI.DefaultCacheValidity) != YAPI.SUCCESS)
        {
          return CALIBRATIONMAX_INVALID;
        }
      }
      return this._calibrationMax;
    }

    /**
     * <summary>
     *   Changes the maximal calibration value for the input (between 0 and 4095, included), without actually
     *   starting the automated calibration.
     * <para>
     *   Remember to call the <c>saveToFlash()</c>
     *   method of the module if the modification must be kept.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="newval">
     *   an integer corresponding to the maximal calibration value for the input (between 0 and 4095,
     *   included), without actually
     *   starting the automated calibration
     * </param>
     * <para>
     * </para>
     * <returns>
     *   <c>YAPI.SUCCESS</c> if the call succeeds.
     * </returns>
     * <para>
     *   On failure, throws an exception or returns a negative error code.
     * </para>
     */
    public int set_calibrationMax(int newval)
    {
      string rest_val;
      rest_val = (newval).ToString();
      return _setAttr("calibrationMax", rest_val);
    }

    /**
     * <summary>
     *   Returns the minimal value measured during the calibration (between 0 and 4095, included).
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   an integer corresponding to the minimal value measured during the calibration (between 0 and 4095, included)
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YAnButton.CALIBRATIONMIN_INVALID</c>.
     * </para>
     */
    public int get_calibrationMin()
    {
      if (this._cacheExpiration <= YAPI.GetTickCount())
      {
        if (this.load(YAPI.DefaultCacheValidity) != YAPI.SUCCESS)
        {
          return CALIBRATIONMIN_INVALID;
        }
      }
      return this._calibrationMin;
    }

    /**
     * <summary>
     *   Changes the minimal calibration value for the input (between 0 and 4095, included), without actually
     *   starting the automated calibration.
     * <para>
     *   Remember to call the <c>saveToFlash()</c>
     *   method of the module if the modification must be kept.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="newval">
     *   an integer corresponding to the minimal calibration value for the input (between 0 and 4095,
     *   included), without actually
     *   starting the automated calibration
     * </param>
     * <para>
     * </para>
     * <returns>
     *   <c>YAPI.SUCCESS</c> if the call succeeds.
     * </returns>
     * <para>
     *   On failure, throws an exception or returns a negative error code.
     * </para>
     */
    public int set_calibrationMin(int newval)
    {
      string rest_val;
      rest_val = (newval).ToString();
      return _setAttr("calibrationMin", rest_val);
    }

    /**
     * <summary>
     *   Returns the sensibility for the input (between 1 and 1000) for triggering user callbacks.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   an integer corresponding to the sensibility for the input (between 1 and 1000) for triggering user callbacks
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YAnButton.SENSITIVITY_INVALID</c>.
     * </para>
     */
    public int get_sensitivity()
    {
      if (this._cacheExpiration <= YAPI.GetTickCount())
      {
        if (this.load(YAPI.DefaultCacheValidity) != YAPI.SUCCESS)
        {
          return SENSITIVITY_INVALID;
        }
      }
      return this._sensitivity;
    }

    /**
     * <summary>
     *   Changes the sensibility for the input (between 1 and 1000) for triggering user callbacks.
     * <para>
     *   The sensibility is used to filter variations around a fixed value, but does not preclude the
     *   transmission of events when the input value evolves constantly in the same direction.
     *   Special case: when the value 1000 is used, the callback will only be thrown when the logical state
     *   of the input switches from pressed to released and back.
     *   Remember to call the <c>saveToFlash()</c> method of the module if the modification must be kept.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="newval">
     *   an integer corresponding to the sensibility for the input (between 1 and 1000) for triggering user callbacks
     * </param>
     * <para>
     * </para>
     * <returns>
     *   <c>YAPI.SUCCESS</c> if the call succeeds.
     * </returns>
     * <para>
     *   On failure, throws an exception or returns a negative error code.
     * </para>
     */
    public int set_sensitivity(int newval)
    {
      string rest_val;
      rest_val = (newval).ToString();
      return _setAttr("sensitivity", rest_val);
    }

    /**
     * <summary>
     *   Returns true if the input (considered as binary) is active (closed contact), and false otherwise.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   either <c>YAnButton.ISPRESSED_FALSE</c> or <c>YAnButton.ISPRESSED_TRUE</c>, according to true if
     *   the input (considered as binary) is active (closed contact), and false otherwise
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YAnButton.ISPRESSED_INVALID</c>.
     * </para>
     */
    public int get_isPressed()
    {
      if (this._cacheExpiration <= YAPI.GetTickCount())
      {
        if (this.load(YAPI.DefaultCacheValidity) != YAPI.SUCCESS)
        {
          return ISPRESSED_INVALID;
        }
      }
      return this._isPressed;
    }

    /**
     * <summary>
     *   Returns the number of elapsed milliseconds between the module power on and the last time
     *   the input button was pressed (the input contact transitionned from open to closed).
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   an integer corresponding to the number of elapsed milliseconds between the module power on and the last time
     *   the input button was pressed (the input contact transitionned from open to closed)
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YAnButton.LASTTIMEPRESSED_INVALID</c>.
     * </para>
     */
    public long get_lastTimePressed()
    {
      if (this._cacheExpiration <= YAPI.GetTickCount())
      {
        if (this.load(YAPI.DefaultCacheValidity) != YAPI.SUCCESS)
        {
          return LASTTIMEPRESSED_INVALID;
        }
      }
      return this._lastTimePressed;
    }

    /**
     * <summary>
     *   Returns the number of elapsed milliseconds between the module power on and the last time
     *   the input button was released (the input contact transitionned from closed to open).
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   an integer corresponding to the number of elapsed milliseconds between the module power on and the last time
     *   the input button was released (the input contact transitionned from closed to open)
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YAnButton.LASTTIMERELEASED_INVALID</c>.
     * </para>
     */
    public long get_lastTimeReleased()
    {
      if (this._cacheExpiration <= YAPI.GetTickCount())
      {
        if (this.load(YAPI.DefaultCacheValidity) != YAPI.SUCCESS)
        {
          return LASTTIMERELEASED_INVALID;
        }
      }
      return this._lastTimeReleased;
    }

    /**
     * <summary>
     *   Returns the pulse counter value
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   an integer corresponding to the pulse counter value
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YAnButton.PULSECOUNTER_INVALID</c>.
     * </para>
     */
    public long get_pulseCounter()
    {
      if (this._cacheExpiration <= YAPI.GetTickCount())
      {
        if (this.load(YAPI.DefaultCacheValidity) != YAPI.SUCCESS)
        {
          return PULSECOUNTER_INVALID;
        }
      }
      return this._pulseCounter;
    }

    public int set_pulseCounter(long newval)
    {
      string rest_val;
      rest_val = (newval).ToString();
      return _setAttr("pulseCounter", rest_val);
    }

    /**
     * <summary>
     *   Returns the timer of the pulses counter (ms)
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   an integer corresponding to the timer of the pulses counter (ms)
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YAnButton.PULSETIMER_INVALID</c>.
     * </para>
     */
    public long get_pulseTimer()
    {
      if (this._cacheExpiration <= YAPI.GetTickCount())
      {
        if (this.load(YAPI.DefaultCacheValidity) != YAPI.SUCCESS)
        {
          return PULSETIMER_INVALID;
        }
      }
      return this._pulseTimer;
    }

    /**
     * <summary>
     *   Retrieves an analog input for a given identifier.
     * <para>
     *   The identifier can be specified using several formats:
     * </para>
     * <para>
     * </para>
     * <para>
     *   - FunctionLogicalName
     * </para>
     * <para>
     *   - ModuleSerialNumber.FunctionIdentifier
     * </para>
     * <para>
     *   - ModuleSerialNumber.FunctionLogicalName
     * </para>
     * <para>
     *   - ModuleLogicalName.FunctionIdentifier
     * </para>
     * <para>
     *   - ModuleLogicalName.FunctionLogicalName
     * </para>
     * <para>
     * </para>
     * <para>
     *   This function does not require that the analog input is online at the time
     *   it is invoked. The returned object is nevertheless valid.
     *   Use the method <c>YAnButton.isOnline()</c> to test if the analog input is
     *   indeed online at a given time. In case of ambiguity when looking for
     *   an analog input by logical name, no error is notified: the first instance
     *   found is returned. The search is performed first by hardware name,
     *   then by logical name.
     * </para>
     * </summary>
     * <param name="func">
     *   a string that uniquely characterizes the analog input
     * </param>
     * <returns>
     *   a <c>YAnButton</c> object allowing you to drive the analog input.
     * </returns>
     */
    public static YAnButton FindAnButton(string func)
    {
      YAnButton obj;
      obj = (YAnButton)YFunction._FindFromCache("AnButton", func);
      if (obj == null)
      {
        obj = new YAnButton(func);
        YFunction._AddToCache("AnButton", func, obj);
      }
      return obj;
    }

    /**
     * <summary>
     *   Registers the callback function that is invoked on every change of advertised value.
     * <para>
     *   The callback is invoked only during the execution of <c>ySleep</c> or <c>yHandleEvents</c>.
     *   This provides control over the time when the callback is triggered. For good responsiveness, remember to call
     *   one of these two functions periodically. To unregister a callback, pass a null pointer as argument.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="callback">
     *   the callback function to call, or a null pointer. The callback function should take two
     *   arguments: the function object of which the value has changed, and the character string describing
     *   the new advertised value.
     * @noreturn
     * </param>
     */
    public int registerValueCallback(ValueCallback callback)
    {
      string val;
      if (callback != null)
      {
        YFunction._UpdateValueCallbackList(this, true);
      }
      else
      {
        YFunction._UpdateValueCallbackList(this, false);
      }
      this._valueCallbackAnButton = callback;
      // Immediately invoke value callback with current value
      if (callback != null && this.isOnline())
      {
        val = this._advertisedValue;
        if (!(val == ""))
        {
          this._invokeValueCallback(val);
        }
      }
      return 0;
    }

    public override int _invokeValueCallback(string value)
    {
      if (this._valueCallbackAnButton != null)
      {
        this._valueCallbackAnButton(this, value);
      }
      else
      {
        base._invokeValueCallback(value);
      }
      return 0;
    }

    /**
     * <summary>
     *   Returns the pulse counter value as well as his timer
     * </summary>
     * <returns>
     *   <c>YAPI.SUCCESS</c> if the call succeeds.
     * </returns>
     * <para>
     *   On failure, throws an exception or returns a negative error code.
     * </para>
     */
    public virtual int resetCounter()
    {
      return this.set_pulseCounter(0);
    }

    /**
     * <summary>
     *   Continues the enumeration of analog inputs started using <c>yFirstAnButton()</c>.
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   a pointer to a <c>YAnButton</c> object, corresponding to
     *   an analog input currently online, or a <c>null</c> pointer
     *   if there are no more analog inputs to enumerate.
     * </returns>
     */
    public YAnButton nextAnButton()
    {
      string hwid = "";
      if (YAPI.YISERR(_nextFunction(ref hwid)))
        return null;
      if (hwid == "")
        return null;
      return FindAnButton(hwid);
    }

    //--- (end of YAnButton implementation)

    //--- (AnButton functions)

    /**
     * <summary>
     *   Starts the enumeration of analog inputs currently accessible.
     * <para>
     *   Use the method <c>YAnButton.nextAnButton()</c> to iterate on
     *   next analog inputs.
     * </para>
     * </summary>
     * <returns>
     *   a pointer to a <c>YAnButton</c> object, corresponding to
     *   the first analog input currently online, or a <c>null</c> pointer
     *   if there are none.
     * </returns>
     */
    public static YAnButton FirstAnButton()
    {
      YFUN_DESCR[] v_fundescr = new YFUN_DESCR[1];
      YDEV_DESCR dev = default(YDEV_DESCR);
      int neededsize = 0;
      int err = 0;
      string serial = null;
      string funcId = null;
      string funcName = null;
      string funcVal = null;
      string errmsg = "";
      int size = Marshal.SizeOf(v_fundescr[0]);
      IntPtr p = Marshal.AllocHGlobal(Marshal.SizeOf(v_fundescr[0]));
      err = YAPI.apiGetFunctionsByClass("AnButton", 0, p, size, ref neededsize, ref errmsg);
      Marshal.Copy(p, v_fundescr, 0, 1);
      Marshal.FreeHGlobal(p);
      if ((YAPI.YISERR(err) | (neededsize == 0)))
        return null;
      serial = "";
      funcId = "";
      funcName = "";
      funcVal = "";
      errmsg = "";
      if ((YAPI.YISERR(YAPI.yapiGetFunctionInfo(v_fundescr[0], ref dev, ref serial, ref funcId, ref funcName, ref funcVal, ref errmsg))))
        return null;
      return FindAnButton(serial + "." + funcId);
    }



    //--- (end of AnButton functions)
  }
}