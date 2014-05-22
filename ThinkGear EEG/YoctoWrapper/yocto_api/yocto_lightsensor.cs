/*********************************************************************
 *
 * $Id: yocto_lightsensor.cs 15251 2014-03-06 10:14:33Z seb $
 *
 * Implements yFindLightSensor(), the high-level API for LightSensor functions
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
  //--- (YLightSensor return codes)
  //--- (end of YLightSensor return codes)
  //--- (YLightSensor class start)
  /**
   * <summary>
   *   The Yoctopuce application programming interface allows you to read an instant
   *   measure of the sensor, as well as the minimal and maximal values observed.
   * <para>
   * </para>
   * <para>
   * </para>
   * </summary>
   */
  public class YLightSensor : YSensor
  {
    //--- (end of YLightSensor class start)
    //--- (YLightSensor definitions)
    public new delegate void ValueCallback(YLightSensor func, string value);
    public new delegate void TimedReportCallback(YLightSensor func, YMeasure measure);

    protected ValueCallback _valueCallbackLightSensor = null;
    protected TimedReportCallback _timedReportCallbackLightSensor = null;
    //--- (end of YLightSensor definitions)

    public YLightSensor(string func)
      : base(func)
    {
      _className = "LightSensor";
      //--- (YLightSensor attributes initialization)
      //--- (end of YLightSensor attributes initialization)
    }

    //--- (YLightSensor implementation)

    protected override void _parseAttr(YAPI.TJSONRECORD member)
    {
      base._parseAttr(member);
    }

    public int set_currentValue(double newval)
    {
      string rest_val;
      rest_val = Math.Round(newval * 65536.0).ToString();
      return _setAttr("currentValue", rest_val);
    }

    /**
     * <summary>
     *   Changes the sensor-specific calibration parameter so that the current value
     *   matches a desired target (linear scaling).
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="calibratedVal">
     *   the desired target value.
     * </param>
     * <para>
     *   Remember to call the <c>saveToFlash()</c> method of the module if the
     *   modification must be kept.
     * </para>
     * <para>
     * </para>
     * <returns>
     *   <c>YAPI.SUCCESS</c> if the call succeeds.
     * </returns>
     * <para>
     *   On failure, throws an exception or returns a negative error code.
     * </para>
     */
    public int calibrate(double calibratedVal)
    {
      string rest_val;
      rest_val = Math.Round(calibratedVal * 65536.0).ToString();
      return _setAttr("currentValue", rest_val);
    }

    /**
     * <summary>
     *   Retrieves a light sensor for a given identifier.
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
     *   This function does not require that the light sensor is online at the time
     *   it is invoked. The returned object is nevertheless valid.
     *   Use the method <c>YLightSensor.isOnline()</c> to test if the light sensor is
     *   indeed online at a given time. In case of ambiguity when looking for
     *   a light sensor by logical name, no error is notified: the first instance
     *   found is returned. The search is performed first by hardware name,
     *   then by logical name.
     * </para>
     * </summary>
     * <param name="func">
     *   a string that uniquely characterizes the light sensor
     * </param>
     * <returns>
     *   a <c>YLightSensor</c> object allowing you to drive the light sensor.
     * </returns>
     */
    public static YLightSensor FindLightSensor(string func)
    {
      YLightSensor obj;
      obj = (YLightSensor)YFunction._FindFromCache("LightSensor", func);
      if (obj == null)
      {
        obj = new YLightSensor(func);
        YFunction._AddToCache("LightSensor", func, obj);
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
      this._valueCallbackLightSensor = callback;
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
      if (this._valueCallbackLightSensor != null)
      {
        this._valueCallbackLightSensor(this, value);
      }
      else
      {
        base._invokeValueCallback(value);
      }
      return 0;
    }

    /**
     * <summary>
     *   Registers the callback function that is invoked on every periodic timed notification.
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
     *   arguments: the function object of which the value has changed, and an YMeasure object describing
     *   the new advertised value.
     * @noreturn
     * </param>
     */
    public int registerTimedReportCallback(TimedReportCallback callback)
    {
      if (callback != null)
      {
        YFunction._UpdateTimedReportCallbackList(this, true);
      }
      else
      {
        YFunction._UpdateTimedReportCallbackList(this, false);
      }
      this._timedReportCallbackLightSensor = callback;
      return 0;
    }

    public override int _invokeTimedReportCallback(YMeasure value)
    {
      if (this._timedReportCallbackLightSensor != null)
      {
        this._timedReportCallbackLightSensor(this, value);
      }
      else
      {
        base._invokeTimedReportCallback(value);
      }
      return 0;
    }

    /**
     * <summary>
     *   Continues the enumeration of light sensors started using <c>yFirstLightSensor()</c>.
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   a pointer to a <c>YLightSensor</c> object, corresponding to
     *   a light sensor currently online, or a <c>null</c> pointer
     *   if there are no more light sensors to enumerate.
     * </returns>
     */
    public YLightSensor nextLightSensor()
    {
      string hwid = "";
      if (YAPI.YISERR(_nextFunction(ref hwid)))
        return null;
      if (hwid == "")
        return null;
      return FindLightSensor(hwid);
    }

    //--- (end of YLightSensor implementation)

    //--- (LightSensor functions)

    /**
     * <summary>
     *   Starts the enumeration of light sensors currently accessible.
     * <para>
     *   Use the method <c>YLightSensor.nextLightSensor()</c> to iterate on
     *   next light sensors.
     * </para>
     * </summary>
     * <returns>
     *   a pointer to a <c>YLightSensor</c> object, corresponding to
     *   the first light sensor currently online, or a <c>null</c> pointer
     *   if there are none.
     * </returns>
     */
    public static YLightSensor FirstLightSensor()
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
      err = YAPI.apiGetFunctionsByClass("LightSensor", 0, p, size, ref neededsize, ref errmsg);
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
      return FindLightSensor(serial + "." + funcId);
    }



    //--- (end of LightSensor functions)
  }
}