/*********************************************************************
 *
 * $Id: yocto_pwmoutput.cs 15529 2014-03-20 17:54:15Z seb $
 *
 * Implements yFindPwmOutput(), the high-level API for PwmOutput functions
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
  //--- (YPwmOutput return codes)
  //--- (end of YPwmOutput return codes)
  //--- (YPwmOutput class start)
  /**
   * <summary>
   *   The Yoctopuce application programming interface allows you to configure, start, and stop the PWM.
   * <para>
   * </para>
   * <para>
   * </para>
   * </summary>
   */
  public class YPwmOutput : YFunction
  {
    //--- (end of YPwmOutput class start)
    //--- (YPwmOutput definitions)
    public new delegate void ValueCallback(YPwmOutput func, string value);
    public new delegate void TimedReportCallback(YPwmOutput func, YMeasure measure);

    public const int ENABLED_FALSE = 0;
    public const int ENABLED_TRUE = 1;
    public const int ENABLED_INVALID = -1;

    public const double DUTYCYCLE_INVALID = YAPI.INVALID_DOUBLE;
    public const double PULSEDURATION_INVALID = YAPI.INVALID_DOUBLE;
    public const int FREQUENCY_INVALID = YAPI.INVALID_UINT;
    public const double PERIOD_INVALID = YAPI.INVALID_DOUBLE;
    public const string PWMTRANSITION_INVALID = YAPI.INVALID_STRING;
    public const int ENABLEDATPOWERON_FALSE = 0;
    public const int ENABLEDATPOWERON_TRUE = 1;
    public const int ENABLEDATPOWERON_INVALID = -1;

    public const double DUTYCYCLEATPOWERON_INVALID = YAPI.INVALID_DOUBLE;
    protected int _enabled = ENABLED_INVALID;
    protected double _dutyCycle = DUTYCYCLE_INVALID;
    protected double _pulseDuration = PULSEDURATION_INVALID;
    protected int _frequency = FREQUENCY_INVALID;
    protected double _period = PERIOD_INVALID;
    protected string _pwmTransition = PWMTRANSITION_INVALID;
    protected int _enabledAtPowerOn = ENABLEDATPOWERON_INVALID;
    protected double _dutyCycleAtPowerOn = DUTYCYCLEATPOWERON_INVALID;
    protected ValueCallback _valueCallbackPwmOutput = null;
    //--- (end of YPwmOutput definitions)

    public YPwmOutput(string func)
      : base(func)
    {
      _className = "PwmOutput";
      //--- (YPwmOutput attributes initialization)
      //--- (end of YPwmOutput attributes initialization)
    }

    //--- (YPwmOutput implementation)

    protected override void _parseAttr(YAPI.TJSONRECORD member)
    {
      if (member.name == "enabled")
      {
        _enabled = member.ivalue > 0 ? 1 : 0;
        return;
      }
      if (member.name == "dutyCycle")
      {
        _dutyCycle = member.ivalue / 65536.0;
        return;
      }
      if (member.name == "pulseDuration")
      {
        _pulseDuration = member.ivalue / 65536.0;
        return;
      }
      if (member.name == "frequency")
      {
        _frequency = (int)member.ivalue;
        return;
      }
      if (member.name == "period")
      {
        _period = member.ivalue / 65536.0;
        return;
      }
      if (member.name == "pwmTransition")
      {
        _pwmTransition = member.svalue;
        return;
      }
      if (member.name == "enabledAtPowerOn")
      {
        _enabledAtPowerOn = member.ivalue > 0 ? 1 : 0;
        return;
      }
      if (member.name == "dutyCycleAtPowerOn")
      {
        _dutyCycleAtPowerOn = member.ivalue / 65536.0;
        return;
      }
      base._parseAttr(member);
    }

    /**
     * <summary>
     *   Returns the state of the PWMs.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   either <c>YPwmOutput.ENABLED_FALSE</c> or <c>YPwmOutput.ENABLED_TRUE</c>, according to the state of the PWMs
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YPwmOutput.ENABLED_INVALID</c>.
     * </para>
     */
    public int get_enabled()
    {
      if (this._cacheExpiration <= YAPI.GetTickCount())
      {
        if (this.load(YAPI.DefaultCacheValidity) != YAPI.SUCCESS)
        {
          return ENABLED_INVALID;
        }
      }
      return this._enabled;
    }

    /**
     * <summary>
     *   Stops or starts the PWM.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="newval">
     *   either <c>YPwmOutput.ENABLED_FALSE</c> or <c>YPwmOutput.ENABLED_TRUE</c>
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
    public int set_enabled(int newval)
    {
      string rest_val;
      rest_val = (newval > 0 ? "1" : "0");
      return _setAttr("enabled", rest_val);
    }

    /**
     * <summary>
     *   Changes the PWM duty cycle, in per cents.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="newval">
     *   a floating point number corresponding to the PWM duty cycle, in per cents
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
    public int set_dutyCycle(double newval)
    {
      string rest_val;
      rest_val = Math.Round(newval * 65536.0).ToString();
      return _setAttr("dutyCycle", rest_val);
    }

    /**
     * <summary>
     *   Returns the PWM duty cycle, in per cents.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   a floating point number corresponding to the PWM duty cycle, in per cents
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YPwmOutput.DUTYCYCLE_INVALID</c>.
     * </para>
     */
    public double get_dutyCycle()
    {
      if (this._cacheExpiration <= YAPI.GetTickCount())
      {
        if (this.load(YAPI.DefaultCacheValidity) != YAPI.SUCCESS)
        {
          return DUTYCYCLE_INVALID;
        }
      }
      return this._dutyCycle;
    }

    /**
     * <summary>
     *   Changes the PWM pulse length, in milliseconds.
     * <para>
     *   A pulse length cannot be longer than period, otherwise it is truncated.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="newval">
     *   a floating point number corresponding to the PWM pulse length, in milliseconds
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
    public int set_pulseDuration(double newval)
    {
      string rest_val;
      rest_val = Math.Round(newval * 65536.0).ToString();
      return _setAttr("pulseDuration", rest_val);
    }

    /**
     * <summary>
     *   Returns the PWM pulse length in milliseconds.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   a floating point number corresponding to the PWM pulse length in milliseconds
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YPwmOutput.PULSEDURATION_INVALID</c>.
     * </para>
     */
    public double get_pulseDuration()
    {
      if (this._cacheExpiration <= YAPI.GetTickCount())
      {
        if (this.load(YAPI.DefaultCacheValidity) != YAPI.SUCCESS)
        {
          return PULSEDURATION_INVALID;
        }
      }
      return this._pulseDuration;
    }

    /**
     * <summary>
     *   Returns the PWM frequency in Hz.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   an integer corresponding to the PWM frequency in Hz
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YPwmOutput.FREQUENCY_INVALID</c>.
     * </para>
     */
    public int get_frequency()
    {
      if (this._cacheExpiration <= YAPI.GetTickCount())
      {
        if (this.load(YAPI.DefaultCacheValidity) != YAPI.SUCCESS)
        {
          return FREQUENCY_INVALID;
        }
      }
      return this._frequency;
    }

    /**
     * <summary>
     *   Changes the PWM frequency.
     * <para>
     *   The duty cycle is kept unchanged thanks to an
     *   automatic pulse width change.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="newval">
     *   an integer corresponding to the PWM frequency
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
    public int set_frequency(int newval)
    {
      string rest_val;
      rest_val = (newval).ToString();
      return _setAttr("frequency", rest_val);
    }

    /**
     * <summary>
     *   Changes the PWM period.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="newval">
     *   a floating point number corresponding to the PWM period
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
    public int set_period(double newval)
    {
      string rest_val;
      rest_val = Math.Round(newval * 65536.0).ToString();
      return _setAttr("period", rest_val);
    }

    /**
     * <summary>
     *   Returns the PWM period in milliseconds.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   a floating point number corresponding to the PWM period in milliseconds
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YPwmOutput.PERIOD_INVALID</c>.
     * </para>
     */
    public double get_period()
    {
      if (this._cacheExpiration <= YAPI.GetTickCount())
      {
        if (this.load(YAPI.DefaultCacheValidity) != YAPI.SUCCESS)
        {
          return PERIOD_INVALID;
        }
      }
      return this._period;
    }

    public string get_pwmTransition()
    {
      if (this._cacheExpiration <= YAPI.GetTickCount())
      {
        if (this.load(YAPI.DefaultCacheValidity) != YAPI.SUCCESS)
        {
          return PWMTRANSITION_INVALID;
        }
      }
      return this._pwmTransition;
    }

    public int set_pwmTransition(string newval)
    {
      string rest_val;
      rest_val = newval;
      return _setAttr("pwmTransition", rest_val);
    }

    /**
     * <summary>
     *   Returns the state of the PWM at device power on.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   either <c>YPwmOutput.ENABLEDATPOWERON_FALSE</c> or <c>YPwmOutput.ENABLEDATPOWERON_TRUE</c>,
     *   according to the state of the PWM at device power on
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YPwmOutput.ENABLEDATPOWERON_INVALID</c>.
     * </para>
     */
    public int get_enabledAtPowerOn()
    {
      if (this._cacheExpiration <= YAPI.GetTickCount())
      {
        if (this.load(YAPI.DefaultCacheValidity) != YAPI.SUCCESS)
        {
          return ENABLEDATPOWERON_INVALID;
        }
      }
      return this._enabledAtPowerOn;
    }

    /**
     * <summary>
     *   Changes the state of the PWM at device power on.
     * <para>
     *   Remember to call the matching module <c>saveToFlash()</c>
     *   method, otherwise this call will have no effect.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="newval">
     *   either <c>YPwmOutput.ENABLEDATPOWERON_FALSE</c> or <c>YPwmOutput.ENABLEDATPOWERON_TRUE</c>,
     *   according to the state of the PWM at device power on
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
    public int set_enabledAtPowerOn(int newval)
    {
      string rest_val;
      rest_val = (newval > 0 ? "1" : "0");
      return _setAttr("enabledAtPowerOn", rest_val);
    }

    /**
     * <summary>
     *   Changes the PWM duty cycle at device power on.
     * <para>
     *   Remember to call the matching
     *   module <c>saveToFlash()</c> method, otherwise this call will have no effect.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="newval">
     *   a floating point number corresponding to the PWM duty cycle at device power on
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
    public int set_dutyCycleAtPowerOn(double newval)
    {
      string rest_val;
      rest_val = Math.Round(newval * 65536.0).ToString();
      return _setAttr("dutyCycleAtPowerOn", rest_val);
    }

    /**
     * <summary>
     *   Returns the PWMs duty cycle at device power on as a floating point number between 0 and 100
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   a floating point number corresponding to the PWMs duty cycle at device power on as a floating point
     *   number between 0 and 100
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YPwmOutput.DUTYCYCLEATPOWERON_INVALID</c>.
     * </para>
     */
    public double get_dutyCycleAtPowerOn()
    {
      if (this._cacheExpiration <= YAPI.GetTickCount())
      {
        if (this.load(YAPI.DefaultCacheValidity) != YAPI.SUCCESS)
        {
          return DUTYCYCLEATPOWERON_INVALID;
        }
      }
      return this._dutyCycleAtPowerOn;
    }

    /**
     * <summary>
     *   Retrieves a PWM for a given identifier.
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
     *   This function does not require that the PWM is online at the time
     *   it is invoked. The returned object is nevertheless valid.
     *   Use the method <c>YPwmOutput.isOnline()</c> to test if the PWM is
     *   indeed online at a given time. In case of ambiguity when looking for
     *   a PWM by logical name, no error is notified: the first instance
     *   found is returned. The search is performed first by hardware name,
     *   then by logical name.
     * </para>
     * </summary>
     * <param name="func">
     *   a string that uniquely characterizes the PWM
     * </param>
     * <returns>
     *   a <c>YPwmOutput</c> object allowing you to drive the PWM.
     * </returns>
     */
    public static YPwmOutput FindPwmOutput(string func)
    {
      YPwmOutput obj;
      obj = (YPwmOutput)YFunction._FindFromCache("PwmOutput", func);
      if (obj == null)
      {
        obj = new YPwmOutput(func);
        YFunction._AddToCache("PwmOutput", func, obj);
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
      this._valueCallbackPwmOutput = callback;
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
      if (this._valueCallbackPwmOutput != null)
      {
        this._valueCallbackPwmOutput(this, value);
      }
      else
      {
        base._invokeValueCallback(value);
      }
      return 0;
    }

    /**
     * <summary>
     *   Performs a smooth transistion of the pulse duration toward a given value.
     * <para>
     *   Any period,
     *   frequency, duty cycle or pulse width change will cancel any ongoing transition process.
     * </para>
     * </summary>
     * <param name="ms_target">
     *   new pulse duration at the end of the transition
     *   (floating-point number, representing the pulse duration in milliseconds)
     * </param>
     * <param name="ms_duration">
     *   total duration of the transition, in milliseconds
     * </param>
     * <returns>
     *   <c>YAPI.SUCCESS</c> when the call succeeds.
     * </returns>
     * <para>
     *   On failure, throws an exception or returns a negative error code.
     * </para>
     */
    public virtual int pulseDurationMove(double ms_target, int ms_duration)
    {
      string newval;
      if (ms_target < 0.0)
      {
        ms_target = 0.0;
      }
      newval = "" + Convert.ToString((int)Math.Round(ms_target * 65536)) + "ms:" + Convert.ToString(ms_duration);
      return this.set_pwmTransition(newval);
    }

    /**
     * <summary>
     *   Performs a smooth change of the pulse duration toward a given value.
     * <para>
     * </para>
     * </summary>
     * <param name="target">
     *   new duty cycle at the end of the transition
     *   (floating-point number, between 0 and 1)
     * </param>
     * <param name="ms_duration">
     *   total duration of the transition, in milliseconds
     * </param>
     * <returns>
     *   <c>YAPI.SUCCESS</c> when the call succeeds.
     * </returns>
     * <para>
     *   On failure, throws an exception or returns a negative error code.
     * </para>
     */
    public virtual int dutyCycleMove(double target, int ms_duration)
    {
      string newval;
      if (target < 0.0)
      {
        target = 0.0;
      }
      if (target > 100.0)
      {
        target = 100.0;
      }
      newval = "" + Convert.ToString((int)Math.Round(target * 65536)) + ":" + Convert.ToString(ms_duration);
      return this.set_pwmTransition(newval);
    }

    /**
     * <summary>
     *   Continues the enumeration of PWMs started using <c>yFirstPwmOutput()</c>.
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   a pointer to a <c>YPwmOutput</c> object, corresponding to
     *   a PWM currently online, or a <c>null</c> pointer
     *   if there are no more PWMs to enumerate.
     * </returns>
     */
    public YPwmOutput nextPwmOutput()
    {
      string hwid = "";
      if (YAPI.YISERR(_nextFunction(ref hwid)))
        return null;
      if (hwid == "")
        return null;
      return FindPwmOutput(hwid);
    }

    //--- (end of YPwmOutput implementation)

    //--- (PwmOutput functions)

    /**
     * <summary>
     *   Starts the enumeration of PWMs currently accessible.
     * <para>
     *   Use the method <c>YPwmOutput.nextPwmOutput()</c> to iterate on
     *   next PWMs.
     * </para>
     * </summary>
     * <returns>
     *   a pointer to a <c>YPwmOutput</c> object, corresponding to
     *   the first PWM currently online, or a <c>null</c> pointer
     *   if there are none.
     * </returns>
     */
    public static YPwmOutput FirstPwmOutput()
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
      err = YAPI.apiGetFunctionsByClass("PwmOutput", 0, p, size, ref neededsize, ref errmsg);
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
      return FindPwmOutput(serial + "." + funcId);
    }



    //--- (end of PwmOutput functions)
  }
}