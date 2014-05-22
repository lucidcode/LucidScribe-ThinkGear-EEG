/*********************************************************************
 *
 * $Id: yocto_led.cs 15251 2014-03-06 10:14:33Z seb $
 *
 * Implements yFindLed(), the high-level API for Led functions
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
  //--- (YLed return codes)
  //--- (end of YLed return codes)
  //--- (YLed class start)
  /**
   * <summary>
   *   Yoctopuce application programming interface
   *   allows you not only to drive the intensity of the led, but also to
   *   have it blink at various preset frequencies.
   * <para>
   * </para>
   * <para>
   * </para>
   * </summary>
   */
  public class YLed : YFunction
  {
    //--- (end of YLed class start)
    //--- (YLed definitions)
    public new delegate void ValueCallback(YLed func, string value);
    public new delegate void TimedReportCallback(YLed func, YMeasure measure);

    public const int POWER_OFF = 0;
    public const int POWER_ON = 1;
    public const int POWER_INVALID = -1;

    public const int LUMINOSITY_INVALID = YAPI.INVALID_UINT;
    public const int BLINKING_STILL = 0;
    public const int BLINKING_RELAX = 1;
    public const int BLINKING_AWARE = 2;
    public const int BLINKING_RUN = 3;
    public const int BLINKING_CALL = 4;
    public const int BLINKING_PANIC = 5;
    public const int BLINKING_INVALID = -1;

    protected int _power = POWER_INVALID;
    protected int _luminosity = LUMINOSITY_INVALID;
    protected int _blinking = BLINKING_INVALID;
    protected ValueCallback _valueCallbackLed = null;
    //--- (end of YLed definitions)

    public YLed(string func)
      : base(func)
    {
      _className = "Led";
      //--- (YLed attributes initialization)
      //--- (end of YLed attributes initialization)
    }

    //--- (YLed implementation)

    protected override void _parseAttr(YAPI.TJSONRECORD member)
    {
      if (member.name == "power")
      {
        _power = member.ivalue > 0 ? 1 : 0;
        return;
      }
      if (member.name == "luminosity")
      {
        _luminosity = (int)member.ivalue;
        return;
      }
      if (member.name == "blinking")
      {
        _blinking = (int)member.ivalue;
        return;
      }
      base._parseAttr(member);
    }

    /**
     * <summary>
     *   Returns the current led state.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   either <c>YLed.POWER_OFF</c> or <c>YLed.POWER_ON</c>, according to the current led state
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YLed.POWER_INVALID</c>.
     * </para>
     */
    public int get_power()
    {
      if (this._cacheExpiration <= YAPI.GetTickCount())
      {
        if (this.load(YAPI.DefaultCacheValidity) != YAPI.SUCCESS)
        {
          return POWER_INVALID;
        }
      }
      return this._power;
    }

    /**
     * <summary>
     *   Changes the state of the led.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="newval">
     *   either <c>YLed.POWER_OFF</c> or <c>YLed.POWER_ON</c>, according to the state of the led
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
    public int set_power(int newval)
    {
      string rest_val;
      rest_val = (newval > 0 ? "1" : "0");
      return _setAttr("power", rest_val);
    }

    /**
     * <summary>
     *   Returns the current led intensity (in per cent).
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   an integer corresponding to the current led intensity (in per cent)
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YLed.LUMINOSITY_INVALID</c>.
     * </para>
     */
    public int get_luminosity()
    {
      if (this._cacheExpiration <= YAPI.GetTickCount())
      {
        if (this.load(YAPI.DefaultCacheValidity) != YAPI.SUCCESS)
        {
          return LUMINOSITY_INVALID;
        }
      }
      return this._luminosity;
    }

    /**
     * <summary>
     *   Changes the current led intensity (in per cent).
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="newval">
     *   an integer corresponding to the current led intensity (in per cent)
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
    public int set_luminosity(int newval)
    {
      string rest_val;
      rest_val = (newval).ToString();
      return _setAttr("luminosity", rest_val);
    }

    /**
     * <summary>
     *   Returns the current led signaling mode.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   a value among <c>YLed.BLINKING_STILL</c>, <c>YLed.BLINKING_RELAX</c>, <c>YLed.BLINKING_AWARE</c>,
     *   <c>YLed.BLINKING_RUN</c>, <c>YLed.BLINKING_CALL</c> and <c>YLed.BLINKING_PANIC</c> corresponding to
     *   the current led signaling mode
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YLed.BLINKING_INVALID</c>.
     * </para>
     */
    public int get_blinking()
    {
      if (this._cacheExpiration <= YAPI.GetTickCount())
      {
        if (this.load(YAPI.DefaultCacheValidity) != YAPI.SUCCESS)
        {
          return BLINKING_INVALID;
        }
      }
      return this._blinking;
    }

    /**
     * <summary>
     *   Changes the current led signaling mode.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="newval">
     *   a value among <c>YLed.BLINKING_STILL</c>, <c>YLed.BLINKING_RELAX</c>, <c>YLed.BLINKING_AWARE</c>,
     *   <c>YLed.BLINKING_RUN</c>, <c>YLed.BLINKING_CALL</c> and <c>YLed.BLINKING_PANIC</c> corresponding to
     *   the current led signaling mode
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
    public int set_blinking(int newval)
    {
      string rest_val;
      rest_val = (newval).ToString();
      return _setAttr("blinking", rest_val);
    }

    /**
     * <summary>
     *   Retrieves a led for a given identifier.
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
     *   This function does not require that the led is online at the time
     *   it is invoked. The returned object is nevertheless valid.
     *   Use the method <c>YLed.isOnline()</c> to test if the led is
     *   indeed online at a given time. In case of ambiguity when looking for
     *   a led by logical name, no error is notified: the first instance
     *   found is returned. The search is performed first by hardware name,
     *   then by logical name.
     * </para>
     * </summary>
     * <param name="func">
     *   a string that uniquely characterizes the led
     * </param>
     * <returns>
     *   a <c>YLed</c> object allowing you to drive the led.
     * </returns>
     */
    public static YLed FindLed(string func)
    {
      YLed obj;
      obj = (YLed)YFunction._FindFromCache("Led", func);
      if (obj == null)
      {
        obj = new YLed(func);
        YFunction._AddToCache("Led", func, obj);
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
      this._valueCallbackLed = callback;
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
      if (this._valueCallbackLed != null)
      {
        this._valueCallbackLed(this, value);
      }
      else
      {
        base._invokeValueCallback(value);
      }
      return 0;
    }

    /**
     * <summary>
     *   Continues the enumeration of leds started using <c>yFirstLed()</c>.
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   a pointer to a <c>YLed</c> object, corresponding to
     *   a led currently online, or a <c>null</c> pointer
     *   if there are no more leds to enumerate.
     * </returns>
     */
    public YLed nextLed()
    {
      string hwid = "";
      if (YAPI.YISERR(_nextFunction(ref hwid)))
        return null;
      if (hwid == "")
        return null;
      return FindLed(hwid);
    }

    //--- (end of YLed implementation)

    //--- (Led functions)

    /**
     * <summary>
     *   Starts the enumeration of leds currently accessible.
     * <para>
     *   Use the method <c>YLed.nextLed()</c> to iterate on
     *   next leds.
     * </para>
     * </summary>
     * <returns>
     *   a pointer to a <c>YLed</c> object, corresponding to
     *   the first led currently online, or a <c>null</c> pointer
     *   if there are none.
     * </returns>
     */
    public static YLed FirstLed()
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
      err = YAPI.apiGetFunctionsByClass("Led", 0, p, size, ref neededsize, ref errmsg);
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
      return FindLed(serial + "." + funcId);
    }



    //--- (end of Led functions)
  }
}