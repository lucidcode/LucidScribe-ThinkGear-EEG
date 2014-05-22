/*********************************************************************
 *
 * $Id: yocto_genericsensor.cs 15251 2014-03-06 10:14:33Z seb $
 *
 * Implements yFindGenericSensor(), the high-level API for GenericSensor functions
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

  //--- (YGenericSensor return codes)
  //--- (end of YGenericSensor return codes)
  //--- (YGenericSensor class start)
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
  public class YGenericSensor : YSensor
  {
    //--- (end of YGenericSensor class start)
    //--- (YGenericSensor definitions)
    public new delegate void ValueCallback(YGenericSensor func, string value);
    public new delegate void TimedReportCallback(YGenericSensor func, YMeasure measure);

    public const double SIGNALVALUE_INVALID = YAPI.INVALID_DOUBLE;
    public const string SIGNALUNIT_INVALID = YAPI.INVALID_STRING;
    public const string SIGNALRANGE_INVALID = YAPI.INVALID_STRING;
    public const string VALUERANGE_INVALID = YAPI.INVALID_STRING;
    protected double _signalValue = SIGNALVALUE_INVALID;
    protected string _signalUnit = SIGNALUNIT_INVALID;
    protected string _signalRange = SIGNALRANGE_INVALID;
    protected string _valueRange = VALUERANGE_INVALID;
    protected ValueCallback _valueCallbackGenericSensor = null;
    protected TimedReportCallback _timedReportCallbackGenericSensor = null;
    //--- (end of YGenericSensor definitions)

    public YGenericSensor(string func)
      : base(func)
    {
      _className = "GenericSensor";
      //--- (YGenericSensor attributes initialization)
      //--- (end of YGenericSensor attributes initialization)
    }

    //--- (YGenericSensor implementation)

    protected override void _parseAttr(YAPI.TJSONRECORD member)
    {
      if (member.name == "signalValue")
      {
        _signalValue = member.ivalue / 65536.0;
        return;
      }
      if (member.name == "signalUnit")
      {
        _signalUnit = member.svalue;
        return;
      }
      if (member.name == "signalRange")
      {
        _signalRange = member.svalue;
        return;
      }
      if (member.name == "valueRange")
      {
        _valueRange = member.svalue;
        return;
      }
      base._parseAttr(member);
    }

    /**
     * <summary>
     *   Changes the measuring unit for the measured value.
     * <para>
     *   Remember to call the <c>saveToFlash()</c> method of the module if the
     *   modification must be kept.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="newval">
     *   a string corresponding to the measuring unit for the measured value
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
    public int set_unit(string newval)
    {
      string rest_val;
      rest_val = newval;
      return _setAttr("unit", rest_val);
    }

    /**
     * <summary>
     *   Returns the measured value of the electrical signal used by the sensor.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   a floating point number corresponding to the measured value of the electrical signal used by the sensor
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YGenericSensor.SIGNALVALUE_INVALID</c>.
     * </para>
     */
    public double get_signalValue()
    {
      if (this._cacheExpiration <= YAPI.GetTickCount())
      {
        if (this.load(YAPI.DefaultCacheValidity) != YAPI.SUCCESS)
        {
          return SIGNALVALUE_INVALID;
        }
      }
      return Math.Round(this._signalValue * 1000) / 1000;
    }

    /**
     * <summary>
     *   Returns the measuring unit of the electrical signal used by the sensor.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   a string corresponding to the measuring unit of the electrical signal used by the sensor
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YGenericSensor.SIGNALUNIT_INVALID</c>.
     * </para>
     */
    public string get_signalUnit()
    {
      if (this._cacheExpiration == 0)
      {
        if (this.load(YAPI.DefaultCacheValidity) != YAPI.SUCCESS)
        {
          return SIGNALUNIT_INVALID;
        }
      }
      return this._signalUnit;
    }

    /**
     * <summary>
     *   Returns the electric signal range used by the sensor.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   a string corresponding to the electric signal range used by the sensor
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YGenericSensor.SIGNALRANGE_INVALID</c>.
     * </para>
     */
    public string get_signalRange()
    {
      if (this._cacheExpiration <= YAPI.GetTickCount())
      {
        if (this.load(YAPI.DefaultCacheValidity) != YAPI.SUCCESS)
        {
          return SIGNALRANGE_INVALID;
        }
      }
      return this._signalRange;
    }

    /**
     * <summary>
     *   Changes the electric signal range used by the sensor.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="newval">
     *   a string corresponding to the electric signal range used by the sensor
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
    public int set_signalRange(string newval)
    {
      string rest_val;
      rest_val = newval;
      return _setAttr("signalRange", rest_val);
    }

    /**
     * <summary>
     *   Returns the physical value range measured by the sensor.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   a string corresponding to the physical value range measured by the sensor
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YGenericSensor.VALUERANGE_INVALID</c>.
     * </para>
     */
    public string get_valueRange()
    {
      if (this._cacheExpiration <= YAPI.GetTickCount())
      {
        if (this.load(YAPI.DefaultCacheValidity) != YAPI.SUCCESS)
        {
          return VALUERANGE_INVALID;
        }
      }
      return this._valueRange;
    }

    /**
     * <summary>
     *   Changes the physical value range measured by the sensor.
     * <para>
     *   The range change may have a side effect
     *   on the display resolution, as it may be adapted automatically.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="newval">
     *   a string corresponding to the physical value range measured by the sensor
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
    public int set_valueRange(string newval)
    {
      string rest_val;
      rest_val = newval;
      return _setAttr("valueRange", rest_val);
    }

    /**
     * <summary>
     *   Retrieves a generic sensor for a given identifier.
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
     *   This function does not require that the generic sensor is online at the time
     *   it is invoked. The returned object is nevertheless valid.
     *   Use the method <c>YGenericSensor.isOnline()</c> to test if the generic sensor is
     *   indeed online at a given time. In case of ambiguity when looking for
     *   a generic sensor by logical name, no error is notified: the first instance
     *   found is returned. The search is performed first by hardware name,
     *   then by logical name.
     * </para>
     * </summary>
     * <param name="func">
     *   a string that uniquely characterizes the generic sensor
     * </param>
     * <returns>
     *   a <c>YGenericSensor</c> object allowing you to drive the generic sensor.
     * </returns>
     */
    public static YGenericSensor FindGenericSensor(string func)
    {
      YGenericSensor obj;
      obj = (YGenericSensor)YFunction._FindFromCache("GenericSensor", func);
      if (obj == null)
      {
        obj = new YGenericSensor(func);
        YFunction._AddToCache("GenericSensor", func, obj);
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
      this._valueCallbackGenericSensor = callback;
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
      if (this._valueCallbackGenericSensor != null)
      {
        this._valueCallbackGenericSensor(this, value);
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
      this._timedReportCallbackGenericSensor = callback;
      return 0;
    }

    public override int _invokeTimedReportCallback(YMeasure value)
    {
      if (this._timedReportCallbackGenericSensor != null)
      {
        this._timedReportCallbackGenericSensor(this, value);
      }
      else
      {
        base._invokeTimedReportCallback(value);
      }
      return 0;
    }

    /**
     * <summary>
     *   Continues the enumeration of generic sensors started using <c>yFirstGenericSensor()</c>.
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   a pointer to a <c>YGenericSensor</c> object, corresponding to
     *   a generic sensor currently online, or a <c>null</c> pointer
     *   if there are no more generic sensors to enumerate.
     * </returns>
     */
    public YGenericSensor nextGenericSensor()
    {
      string hwid = "";
      if (YAPI.YISERR(_nextFunction(ref hwid)))
        return null;
      if (hwid == "")
        return null;
      return FindGenericSensor(hwid);
    }

    //--- (end of YGenericSensor implementation)

    //--- (GenericSensor functions)

    /**
     * <summary>
     *   Starts the enumeration of generic sensors currently accessible.
     * <para>
     *   Use the method <c>YGenericSensor.nextGenericSensor()</c> to iterate on
     *   next generic sensors.
     * </para>
     * </summary>
     * <returns>
     *   a pointer to a <c>YGenericSensor</c> object, corresponding to
     *   the first generic sensor currently online, or a <c>null</c> pointer
     *   if there are none.
     * </returns>
     */
    public static YGenericSensor FirstGenericSensor()
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
      err = YAPI.apiGetFunctionsByClass("GenericSensor", 0, p, size, ref neededsize, ref errmsg);
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
      return FindGenericSensor(serial + "." + funcId);
    }



    //--- (end of GenericSensor functions)
  }
}