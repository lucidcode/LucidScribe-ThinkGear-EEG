/*********************************************************************
 *
 * $Id: yocto_current.cs 15251 2014-03-06 10:14:33Z seb $
 *
 * Implements yFindCurrent(), the high-level API for Current functions
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
  //--- (YCurrent return codes)
  //--- (end of YCurrent return codes)
  //--- (YCurrent class start)
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
  public class YCurrent : YSensor
  {
    //--- (end of YCurrent class start)
    //--- (YCurrent definitions)
    public new delegate void ValueCallback(YCurrent func, string value);
    public new delegate void TimedReportCallback(YCurrent func, YMeasure measure);

    protected ValueCallback _valueCallbackCurrent = null;
    protected TimedReportCallback _timedReportCallbackCurrent = null;
    //--- (end of YCurrent definitions)

    public YCurrent(string func)
      : base(func)
    {
      _className = "Current";
      //--- (YCurrent attributes initialization)
      //--- (end of YCurrent attributes initialization)
    }

    //--- (YCurrent implementation)

    protected override void _parseAttr(YAPI.TJSONRECORD member)
    {
      base._parseAttr(member);
    }

    /**
     * <summary>
     *   Retrieves a current sensor for a given identifier.
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
     *   This function does not require that the current sensor is online at the time
     *   it is invoked. The returned object is nevertheless valid.
     *   Use the method <c>YCurrent.isOnline()</c> to test if the current sensor is
     *   indeed online at a given time. In case of ambiguity when looking for
     *   a current sensor by logical name, no error is notified: the first instance
     *   found is returned. The search is performed first by hardware name,
     *   then by logical name.
     * </para>
     * </summary>
     * <param name="func">
     *   a string that uniquely characterizes the current sensor
     * </param>
     * <returns>
     *   a <c>YCurrent</c> object allowing you to drive the current sensor.
     * </returns>
     */
    public static YCurrent FindCurrent(string func)
    {
      YCurrent obj;
      obj = (YCurrent)YFunction._FindFromCache("Current", func);
      if (obj == null)
      {
        obj = new YCurrent(func);
        YFunction._AddToCache("Current", func, obj);
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
      this._valueCallbackCurrent = callback;
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
      if (this._valueCallbackCurrent != null)
      {
        this._valueCallbackCurrent(this, value);
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
      this._timedReportCallbackCurrent = callback;
      return 0;
    }

    public override int _invokeTimedReportCallback(YMeasure value)
    {
      if (this._timedReportCallbackCurrent != null)
      {
        this._timedReportCallbackCurrent(this, value);
      }
      else
      {
        base._invokeTimedReportCallback(value);
      }
      return 0;
    }

    /**
     * <summary>
     *   Continues the enumeration of current sensors started using <c>yFirstCurrent()</c>.
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   a pointer to a <c>YCurrent</c> object, corresponding to
     *   a current sensor currently online, or a <c>null</c> pointer
     *   if there are no more current sensors to enumerate.
     * </returns>
     */
    public YCurrent nextCurrent()
    {
      string hwid = "";
      if (YAPI.YISERR(_nextFunction(ref hwid)))
        return null;
      if (hwid == "")
        return null;
      return FindCurrent(hwid);
    }

    //--- (end of YCurrent implementation)

    //--- (Current functions)

    /**
     * <summary>
     *   Starts the enumeration of current sensors currently accessible.
     * <para>
     *   Use the method <c>YCurrent.nextCurrent()</c> to iterate on
     *   next current sensors.
     * </para>
     * </summary>
     * <returns>
     *   a pointer to a <c>YCurrent</c> object, corresponding to
     *   the first current sensor currently online, or a <c>null</c> pointer
     *   if there are none.
     * </returns>
     */
    public static YCurrent FirstCurrent()
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
      err = YAPI.apiGetFunctionsByClass("Current", 0, p, size, ref neededsize, ref errmsg);
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
      return FindCurrent(serial + "." + funcId);
    }



    //--- (end of Current functions)
  }
}