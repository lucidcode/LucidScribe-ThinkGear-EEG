/*********************************************************************
 *
 * $Id: pic24config.php 15635 2014-03-28 21:04:00Z mvuilleu $
 *
 * Implements yFindAltitude(), the high-level API for Altitude functions
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


namespace YocoWrapper
{
  using System;
  using System.Collections;
  using System.Collections.Generic;
  using System.Runtime.InteropServices;
  using System.Diagnostics;
  using System.Text;
  using YDEV_DESCR = System.Int32;
  using YFUN_DESCR = System.Int32;

  //--- (YAltitude return codes)
  //--- (end of YAltitude return codes)
  //--- (YAltitude class start)
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
  public class YAltitude : YSensor
  {
    //--- (end of YAltitude class start)
    //--- (YAltitude definitions)
    public new delegate void ValueCallback(YAltitude func, string value);
    public new delegate void TimedReportCallback(YAltitude func, YMeasure measure);

    public const double QNH_INVALID = YAPI.INVALID_DOUBLE;
    protected double _qnh = QNH_INVALID;
    protected ValueCallback _valueCallbackAltitude = null;
    protected TimedReportCallback _timedReportCallbackAltitude = null;
    //--- (end of YAltitude definitions)

    public YAltitude(string func)
      : base(func)
    {
      _className = "Altitude";
      //--- (YAltitude attributes initialization)
      //--- (end of YAltitude attributes initialization)
    }

    //--- (YAltitude implementation)

    protected override void _parseAttr(YAPI.TJSONRECORD member)
    {
      if (member.name == "qnh")
      {
        _qnh = member.ivalue / 65536.0;
        return;
      }
      base._parseAttr(member);
    }

    /**
     * <summary>
     *   Changes the current estimated altitude.
     * <para>
     *   This allows to compensate for
     *   ambient pressure variations and to work in relative mode.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="newval">
     *   a floating point number corresponding to the current estimated altitude
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
    public int set_currentValue(double newval)
    {
      string rest_val;
      rest_val = Math.Round(newval * 65536.0).ToString();
      return _setAttr("currentValue", rest_val);
    }

    /**
     * <summary>
     *   Changes the barometric pressure adjusted to sea level used to compute
     *   the altitude (QNH).
     * <para>
     *   This enables you to compensate for atmospheric pressure
     *   changes due to weather conditions.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="newval">
     *   a floating point number corresponding to the barometric pressure adjusted to sea level used to compute
     *   the altitude (QNH)
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
    public int set_qnh(double newval)
    {
      string rest_val;
      rest_val = Math.Round(newval * 65536.0).ToString();
      return _setAttr("qnh", rest_val);
    }

    /**
     * <summary>
     *   Returns the barometric pressure adjusted to sea level used to compute
     *   the altitude (QNH).
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   a floating point number corresponding to the barometric pressure adjusted to sea level used to compute
     *   the altitude (QNH)
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YAltitude.QNH_INVALID</c>.
     * </para>
     */
    public double get_qnh()
    {
      if (this._cacheExpiration <= YAPI.GetTickCount())
      {
        if (this.load(YAPI.DefaultCacheValidity) != YAPI.SUCCESS)
        {
          return QNH_INVALID;
        }
      }
      return this._qnh;
    }

    /**
     * <summary>
     *   Retrieves an altimeter for a given identifier.
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
     *   This function does not require that the altimeter is online at the time
     *   it is invoked. The returned object is nevertheless valid.
     *   Use the method <c>YAltitude.isOnline()</c> to test if the altimeter is
     *   indeed online at a given time. In case of ambiguity when looking for
     *   an altimeter by logical name, no error is notified: the first instance
     *   found is returned. The search is performed first by hardware name,
     *   then by logical name.
     * </para>
     * </summary>
     * <param name="func">
     *   a string that uniquely characterizes the altimeter
     * </param>
     * <returns>
     *   a <c>YAltitude</c> object allowing you to drive the altimeter.
     * </returns>
     */
    public static YAltitude FindAltitude(string func)
    {
      YAltitude obj;
      obj = (YAltitude)YFunction._FindFromCache("Altitude", func);
      if (obj == null)
      {
        obj = new YAltitude(func);
        YFunction._AddToCache("Altitude", func, obj);
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
      this._valueCallbackAltitude = callback;
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
      if (this._valueCallbackAltitude != null)
      {
        this._valueCallbackAltitude(this, value);
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
      this._timedReportCallbackAltitude = callback;
      return 0;
    }

    public override int _invokeTimedReportCallback(YMeasure value)
    {
      if (this._timedReportCallbackAltitude != null)
      {
        this._timedReportCallbackAltitude(this, value);
      }
      else
      {
        base._invokeTimedReportCallback(value);
      }
      return 0;
    }

    /**
     * <summary>
     *   Continues the enumeration of altimeters started using <c>yFirstAltitude()</c>.
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   a pointer to a <c>YAltitude</c> object, corresponding to
     *   an altimeter currently online, or a <c>null</c> pointer
     *   if there are no more altimeters to enumerate.
     * </returns>
     */
    public YAltitude nextAltitude()
    {
      string hwid = "";
      if (YAPI.YISERR(_nextFunction(ref hwid)))
        return null;
      if (hwid == "")
        return null;
      return FindAltitude(hwid);
    }

    //--- (end of YAltitude implementation)

    //--- (Altitude functions)

    /**
     * <summary>
     *   Starts the enumeration of altimeters currently accessible.
     * <para>
     *   Use the method <c>YAltitude.nextAltitude()</c> to iterate on
     *   next altimeters.
     * </para>
     * </summary>
     * <returns>
     *   a pointer to a <c>YAltitude</c> object, corresponding to
     *   the first altimeter currently online, or a <c>null</c> pointer
     *   if there are none.
     * </returns>
     */
    public static YAltitude FirstAltitude()
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
      err = YAPI.apiGetFunctionsByClass("Altitude", 0, p, size, ref neededsize, ref errmsg);
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
      return FindAltitude(serial + "." + funcId);
    }



    //--- (end of Altitude functions)
  }
}