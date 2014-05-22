/*********************************************************************
 *
 * $Id: yocto_temperature.cs 15251 2014-03-06 10:14:33Z seb $
 *
 * Implements yFindTemperature(), the high-level API for Temperature functions
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
  //--- (YTemperature return codes)
  //--- (end of YTemperature return codes)
  //--- (YTemperature class start)
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
  public class YTemperature : YSensor
  {
    //--- (end of YTemperature class start)
    //--- (YTemperature definitions)
    public new delegate void ValueCallback(YTemperature func, string value);
    public new delegate void TimedReportCallback(YTemperature func, YMeasure measure);

    public const int SENSORTYPE_DIGITAL = 0;
    public const int SENSORTYPE_TYPE_K = 1;
    public const int SENSORTYPE_TYPE_E = 2;
    public const int SENSORTYPE_TYPE_J = 3;
    public const int SENSORTYPE_TYPE_N = 4;
    public const int SENSORTYPE_TYPE_R = 5;
    public const int SENSORTYPE_TYPE_S = 6;
    public const int SENSORTYPE_TYPE_T = 7;
    public const int SENSORTYPE_PT100_4WIRES = 8;
    public const int SENSORTYPE_PT100_3WIRES = 9;
    public const int SENSORTYPE_PT100_2WIRES = 10;
    public const int SENSORTYPE_INVALID = -1;

    protected int _sensorType = SENSORTYPE_INVALID;
    protected ValueCallback _valueCallbackTemperature = null;
    protected TimedReportCallback _timedReportCallbackTemperature = null;
    //--- (end of YTemperature definitions)

    public YTemperature(string func)
      : base(func)
    {
      _className = "Temperature";
      //--- (YTemperature attributes initialization)
      //--- (end of YTemperature attributes initialization)
    }

    //--- (YTemperature implementation)

    protected override void _parseAttr(YAPI.TJSONRECORD member)
    {
      if (member.name == "sensorType")
      {
        _sensorType = (int)member.ivalue;
        return;
      }
      base._parseAttr(member);
    }

    /**
     * <summary>
     *   Returns the temperature sensor type.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   a value among <c>YTemperature.SENSORTYPE_DIGITAL</c>, <c>YTemperature.SENSORTYPE_TYPE_K</c>,
     *   <c>YTemperature.SENSORTYPE_TYPE_E</c>, <c>YTemperature.SENSORTYPE_TYPE_J</c>,
     *   <c>YTemperature.SENSORTYPE_TYPE_N</c>, <c>YTemperature.SENSORTYPE_TYPE_R</c>,
     *   <c>YTemperature.SENSORTYPE_TYPE_S</c>, <c>YTemperature.SENSORTYPE_TYPE_T</c>,
     *   <c>YTemperature.SENSORTYPE_PT100_4WIRES</c>, <c>YTemperature.SENSORTYPE_PT100_3WIRES</c> and
     *   <c>YTemperature.SENSORTYPE_PT100_2WIRES</c> corresponding to the temperature sensor type
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YTemperature.SENSORTYPE_INVALID</c>.
     * </para>
     */
    public int get_sensorType()
    {
      if (this._cacheExpiration <= YAPI.GetTickCount())
      {
        if (this.load(YAPI.DefaultCacheValidity) != YAPI.SUCCESS)
        {
          return SENSORTYPE_INVALID;
        }
      }
      return this._sensorType;
    }

    /**
     * <summary>
     *   Modify the temperature sensor type.
     * <para>
     *   This function is used to
     *   to define the type of thermocouple (K,E...) used with the device.
     *   This will have no effect if module is using a digital sensor.
     *   Remember to call the <c>saveToFlash()</c> method of the module if the
     *   modification must be kept.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="newval">
     *   a value among <c>YTemperature.SENSORTYPE_DIGITAL</c>, <c>YTemperature.SENSORTYPE_TYPE_K</c>,
     *   <c>YTemperature.SENSORTYPE_TYPE_E</c>, <c>YTemperature.SENSORTYPE_TYPE_J</c>,
     *   <c>YTemperature.SENSORTYPE_TYPE_N</c>, <c>YTemperature.SENSORTYPE_TYPE_R</c>,
     *   <c>YTemperature.SENSORTYPE_TYPE_S</c>, <c>YTemperature.SENSORTYPE_TYPE_T</c>,
     *   <c>YTemperature.SENSORTYPE_PT100_4WIRES</c>, <c>YTemperature.SENSORTYPE_PT100_3WIRES</c> and
     *   <c>YTemperature.SENSORTYPE_PT100_2WIRES</c>
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
    public int set_sensorType(int newval)
    {
      string rest_val;
      rest_val = (newval).ToString();
      return _setAttr("sensorType", rest_val);
    }

    /**
     * <summary>
     *   Retrieves a temperature sensor for a given identifier.
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
     *   This function does not require that the temperature sensor is online at the time
     *   it is invoked. The returned object is nevertheless valid.
     *   Use the method <c>YTemperature.isOnline()</c> to test if the temperature sensor is
     *   indeed online at a given time. In case of ambiguity when looking for
     *   a temperature sensor by logical name, no error is notified: the first instance
     *   found is returned. The search is performed first by hardware name,
     *   then by logical name.
     * </para>
     * </summary>
     * <param name="func">
     *   a string that uniquely characterizes the temperature sensor
     * </param>
     * <returns>
     *   a <c>YTemperature</c> object allowing you to drive the temperature sensor.
     * </returns>
     */
    public static YTemperature FindTemperature(string func)
    {
      YTemperature obj;
      obj = (YTemperature)YFunction._FindFromCache("Temperature", func);
      if (obj == null)
      {
        obj = new YTemperature(func);
        YFunction._AddToCache("Temperature", func, obj);
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
      this._valueCallbackTemperature = callback;
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
      if (this._valueCallbackTemperature != null)
      {
        this._valueCallbackTemperature(this, value);
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
      this._timedReportCallbackTemperature = callback;
      return 0;
    }

    public override int _invokeTimedReportCallback(YMeasure value)
    {
      if (this._timedReportCallbackTemperature != null)
      {
        this._timedReportCallbackTemperature(this, value);
      }
      else
      {
        base._invokeTimedReportCallback(value);
      }
      return 0;
    }

    /**
     * <summary>
     *   Continues the enumeration of temperature sensors started using <c>yFirstTemperature()</c>.
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   a pointer to a <c>YTemperature</c> object, corresponding to
     *   a temperature sensor currently online, or a <c>null</c> pointer
     *   if there are no more temperature sensors to enumerate.
     * </returns>
     */
    public YTemperature nextTemperature()
    {
      string hwid = "";
      if (YAPI.YISERR(_nextFunction(ref hwid)))
        return null;
      if (hwid == "")
        return null;
      return FindTemperature(hwid);
    }

    //--- (end of YTemperature implementation)

    //--- (Temperature functions)

    /**
     * <summary>
     *   Starts the enumeration of temperature sensors currently accessible.
     * <para>
     *   Use the method <c>YTemperature.nextTemperature()</c> to iterate on
     *   next temperature sensors.
     * </para>
     * </summary>
     * <returns>
     *   a pointer to a <c>YTemperature</c> object, corresponding to
     *   the first temperature sensor currently online, or a <c>null</c> pointer
     *   if there are none.
     * </returns>
     */
    public static YTemperature FirstTemperature()
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
      err = YAPI.apiGetFunctionsByClass("Temperature", 0, p, size, ref neededsize, ref errmsg);
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
      return FindTemperature(serial + "." + funcId);
    }



    //--- (end of Temperature functions)
  }
}