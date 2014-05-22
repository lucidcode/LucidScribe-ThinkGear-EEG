/*********************************************************************
 *
 * $Id: yocto_compass.cs 15251 2014-03-06 10:14:33Z seb $
 *
 * Implements yFindCompass(), the high-level API for Compass functions
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
  //--- (YCompass return codes)
  //--- (end of YCompass return codes)
  //--- (YCompass class start)
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
  public class YCompass : YSensor
  {
    //--- (end of YCompass class start)
    //--- (YCompass definitions)
    public new delegate void ValueCallback(YCompass func, string value);
    public new delegate void TimedReportCallback(YCompass func, YMeasure measure);

    public const int AXIS_X = 0;
    public const int AXIS_Y = 1;
    public const int AXIS_Z = 2;
    public const int AXIS_INVALID = -1;

    public const double MAGNETICHEADING_INVALID = YAPI.INVALID_DOUBLE;
    protected int _axis = AXIS_INVALID;
    protected double _magneticHeading = MAGNETICHEADING_INVALID;
    protected ValueCallback _valueCallbackCompass = null;
    protected TimedReportCallback _timedReportCallbackCompass = null;
    //--- (end of YCompass definitions)

    public YCompass(string func)
      : base(func)
    {
      _className = "Compass";
      //--- (YCompass attributes initialization)
      //--- (end of YCompass attributes initialization)
    }

    //--- (YCompass implementation)

    protected override void _parseAttr(YAPI.TJSONRECORD member)
    {
      if (member.name == "axis")
      {
        _axis = (int)member.ivalue;
        return;
      }
      if (member.name == "magneticHeading")
      {
        _magneticHeading = member.ivalue / 65536.0;
        return;
      }
      base._parseAttr(member);
    }

    public int get_axis()
    {
      if (this._cacheExpiration <= YAPI.GetTickCount())
      {
        if (this.load(YAPI.DefaultCacheValidity) != YAPI.SUCCESS)
        {
          return AXIS_INVALID;
        }
      }
      return this._axis;
    }

    /**
     * <summary>
     *   Returns the magnetic heading, regardless of the configured bearing.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   a floating point number corresponding to the magnetic heading, regardless of the configured bearing
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YCompass.MAGNETICHEADING_INVALID</c>.
     * </para>
     */
    public double get_magneticHeading()
    {
      if (this._cacheExpiration <= YAPI.GetTickCount())
      {
        if (this.load(YAPI.DefaultCacheValidity) != YAPI.SUCCESS)
        {
          return MAGNETICHEADING_INVALID;
        }
      }
      return this._magneticHeading;
    }

    /**
     * <summary>
     *   Retrieves a compass for a given identifier.
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
     *   This function does not require that the compass is online at the time
     *   it is invoked. The returned object is nevertheless valid.
     *   Use the method <c>YCompass.isOnline()</c> to test if the compass is
     *   indeed online at a given time. In case of ambiguity when looking for
     *   a compass by logical name, no error is notified: the first instance
     *   found is returned. The search is performed first by hardware name,
     *   then by logical name.
     * </para>
     * </summary>
     * <param name="func">
     *   a string that uniquely characterizes the compass
     * </param>
     * <returns>
     *   a <c>YCompass</c> object allowing you to drive the compass.
     * </returns>
     */
    public static YCompass FindCompass(string func)
    {
      YCompass obj;
      obj = (YCompass)YFunction._FindFromCache("Compass", func);
      if (obj == null)
      {
        obj = new YCompass(func);
        YFunction._AddToCache("Compass", func, obj);
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
      this._valueCallbackCompass = callback;
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
      if (this._valueCallbackCompass != null)
      {
        this._valueCallbackCompass(this, value);
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
      this._timedReportCallbackCompass = callback;
      return 0;
    }

    public override int _invokeTimedReportCallback(YMeasure value)
    {
      if (this._timedReportCallbackCompass != null)
      {
        this._timedReportCallbackCompass(this, value);
      }
      else
      {
        base._invokeTimedReportCallback(value);
      }
      return 0;
    }

    /**
     * <summary>
     *   Continues the enumeration of compasses started using <c>yFirstCompass()</c>.
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   a pointer to a <c>YCompass</c> object, corresponding to
     *   a compass currently online, or a <c>null</c> pointer
     *   if there are no more compasses to enumerate.
     * </returns>
     */
    public YCompass nextCompass()
    {
      string hwid = "";
      if (YAPI.YISERR(_nextFunction(ref hwid)))
        return null;
      if (hwid == "")
        return null;
      return FindCompass(hwid);
    }

    //--- (end of YCompass implementation)

    //--- (Compass functions)

    /**
     * <summary>
     *   Starts the enumeration of compasses currently accessible.
     * <para>
     *   Use the method <c>YCompass.nextCompass()</c> to iterate on
     *   next compasses.
     * </para>
     * </summary>
     * <returns>
     *   a pointer to a <c>YCompass</c> object, corresponding to
     *   the first compass currently online, or a <c>null</c> pointer
     *   if there are none.
     * </returns>
     */
    public static YCompass FirstCompass()
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
      err = YAPI.apiGetFunctionsByClass("Compass", 0, p, size, ref neededsize, ref errmsg);
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
      return FindCompass(serial + "." + funcId);
    }



    //--- (end of Compass functions)
  }
}