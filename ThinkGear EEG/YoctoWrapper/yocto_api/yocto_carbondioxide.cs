/*********************************************************************
 *
 * $Id: yocto_carbondioxide.cs 15251 2014-03-06 10:14:33Z seb $
 *
 * Implements yFindCarbonDioxide(), the high-level API for CarbonDioxide functions
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
  //--- (YCarbonDioxide return codes)
  //--- (end of YCarbonDioxide return codes)
  //--- (YCarbonDioxide class start)
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
  public class YCarbonDioxide : YSensor
  {
    //--- (end of YCarbonDioxide class start)
    //--- (YCarbonDioxide definitions)
    public new delegate void ValueCallback(YCarbonDioxide func, string value);
    public new delegate void TimedReportCallback(YCarbonDioxide func, YMeasure measure);

    protected ValueCallback _valueCallbackCarbonDioxide = null;
    protected TimedReportCallback _timedReportCallbackCarbonDioxide = null;
    //--- (end of YCarbonDioxide definitions)

    public YCarbonDioxide(string func)
      : base(func)
    {
      _className = "CarbonDioxide";
      //--- (YCarbonDioxide attributes initialization)
      //--- (end of YCarbonDioxide attributes initialization)
    }

    //--- (YCarbonDioxide implementation)

    protected override void _parseAttr(YAPI.TJSONRECORD member)
    {
      base._parseAttr(member);
    }

    /**
     * <summary>
     *   Retrieves a CO2 sensor for a given identifier.
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
     *   This function does not require that the CO2 sensor is online at the time
     *   it is invoked. The returned object is nevertheless valid.
     *   Use the method <c>YCarbonDioxide.isOnline()</c> to test if the CO2 sensor is
     *   indeed online at a given time. In case of ambiguity when looking for
     *   a CO2 sensor by logical name, no error is notified: the first instance
     *   found is returned. The search is performed first by hardware name,
     *   then by logical name.
     * </para>
     * </summary>
     * <param name="func">
     *   a string that uniquely characterizes the CO2 sensor
     * </param>
     * <returns>
     *   a <c>YCarbonDioxide</c> object allowing you to drive the CO2 sensor.
     * </returns>
     */
    public static YCarbonDioxide FindCarbonDioxide(string func)
    {
      YCarbonDioxide obj;
      obj = (YCarbonDioxide)YFunction._FindFromCache("CarbonDioxide", func);
      if (obj == null)
      {
        obj = new YCarbonDioxide(func);
        YFunction._AddToCache("CarbonDioxide", func, obj);
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
      this._valueCallbackCarbonDioxide = callback;
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
      if (this._valueCallbackCarbonDioxide != null)
      {
        this._valueCallbackCarbonDioxide(this, value);
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
      this._timedReportCallbackCarbonDioxide = callback;
      return 0;
    }

    public override int _invokeTimedReportCallback(YMeasure value)
    {
      if (this._timedReportCallbackCarbonDioxide != null)
      {
        this._timedReportCallbackCarbonDioxide(this, value);
      }
      else
      {
        base._invokeTimedReportCallback(value);
      }
      return 0;
    }

    /**
     * <summary>
     *   Continues the enumeration of CO2 sensors started using <c>yFirstCarbonDioxide()</c>.
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   a pointer to a <c>YCarbonDioxide</c> object, corresponding to
     *   a CO2 sensor currently online, or a <c>null</c> pointer
     *   if there are no more CO2 sensors to enumerate.
     * </returns>
     */
    public YCarbonDioxide nextCarbonDioxide()
    {
      string hwid = "";
      if (YAPI.YISERR(_nextFunction(ref hwid)))
        return null;
      if (hwid == "")
        return null;
      return FindCarbonDioxide(hwid);
    }

    //--- (end of YCarbonDioxide implementation)

    //--- (CarbonDioxide functions)

    /**
     * <summary>
     *   Starts the enumeration of CO2 sensors currently accessible.
     * <para>
     *   Use the method <c>YCarbonDioxide.nextCarbonDioxide()</c> to iterate on
     *   next CO2 sensors.
     * </para>
     * </summary>
     * <returns>
     *   a pointer to a <c>YCarbonDioxide</c> object, corresponding to
     *   the first CO2 sensor currently online, or a <c>null</c> pointer
     *   if there are none.
     * </returns>
     */
    public static YCarbonDioxide FirstCarbonDioxide()
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
      err = YAPI.apiGetFunctionsByClass("CarbonDioxide", 0, p, size, ref neededsize, ref errmsg);
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
      return FindCarbonDioxide(serial + "." + funcId);
    }



    //--- (end of CarbonDioxide functions)
  }
}