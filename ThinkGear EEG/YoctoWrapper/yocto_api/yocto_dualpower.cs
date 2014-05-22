/*********************************************************************
 *
 * $Id: yocto_dualpower.cs 15251 2014-03-06 10:14:33Z seb $
 *
 * Implements yFindDualPower(), the high-level API for DualPower functions
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

  //--- (YDualPower return codes)
  //--- (end of YDualPower return codes)
  //--- (YDualPower class start)
  /**
   * <summary>
   *   Yoctopuce application programming interface allows you to control
   *   the power source to use for module functions that require high current.
   * <para>
   *   The module can also automatically disconnect the external power
   *   when a voltage drop is observed on the external power source
   *   (external battery running out of power).
   * </para>
   * <para>
   * </para>
   * </summary>
   */
  public class YDualPower : YFunction
  {
    //--- (end of YDualPower class start)
    //--- (YDualPower definitions)
    public new delegate void ValueCallback(YDualPower func, string value);
    public new delegate void TimedReportCallback(YDualPower func, YMeasure measure);

    public const int POWERSTATE_OFF = 0;
    public const int POWERSTATE_FROM_USB = 1;
    public const int POWERSTATE_FROM_EXT = 2;
    public const int POWERSTATE_INVALID = -1;

    public const int POWERCONTROL_AUTO = 0;
    public const int POWERCONTROL_FROM_USB = 1;
    public const int POWERCONTROL_FROM_EXT = 2;
    public const int POWERCONTROL_OFF = 3;
    public const int POWERCONTROL_INVALID = -1;

    public const int EXTVOLTAGE_INVALID = YAPI.INVALID_UINT;
    protected int _powerState = POWERSTATE_INVALID;
    protected int _powerControl = POWERCONTROL_INVALID;
    protected int _extVoltage = EXTVOLTAGE_INVALID;
    protected ValueCallback _valueCallbackDualPower = null;
    //--- (end of YDualPower definitions)

    public YDualPower(string func)
      : base(func)
    {
      _className = "DualPower";
      //--- (YDualPower attributes initialization)
      //--- (end of YDualPower attributes initialization)
    }

    //--- (YDualPower implementation)

    protected override void _parseAttr(YAPI.TJSONRECORD member)
    {
      if (member.name == "powerState")
      {
        _powerState = (int)member.ivalue;
        return;
      }
      if (member.name == "powerControl")
      {
        _powerControl = (int)member.ivalue;
        return;
      }
      if (member.name == "extVoltage")
      {
        _extVoltage = (int)member.ivalue;
        return;
      }
      base._parseAttr(member);
    }

    /**
     * <summary>
     *   Returns the current power source for module functions that require lots of current.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   a value among <c>YDualPower.POWERSTATE_OFF</c>, <c>YDualPower.POWERSTATE_FROM_USB</c> and
     *   <c>YDualPower.POWERSTATE_FROM_EXT</c> corresponding to the current power source for module
     *   functions that require lots of current
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YDualPower.POWERSTATE_INVALID</c>.
     * </para>
     */
    public int get_powerState()
    {
      if (this._cacheExpiration <= YAPI.GetTickCount())
      {
        if (this.load(YAPI.DefaultCacheValidity) != YAPI.SUCCESS)
        {
          return POWERSTATE_INVALID;
        }
      }
      return this._powerState;
    }

    /**
     * <summary>
     *   Returns the selected power source for module functions that require lots of current.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   a value among <c>YDualPower.POWERCONTROL_AUTO</c>, <c>YDualPower.POWERCONTROL_FROM_USB</c>,
     *   <c>YDualPower.POWERCONTROL_FROM_EXT</c> and <c>YDualPower.POWERCONTROL_OFF</c> corresponding to the
     *   selected power source for module functions that require lots of current
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YDualPower.POWERCONTROL_INVALID</c>.
     * </para>
     */
    public int get_powerControl()
    {
      if (this._cacheExpiration <= YAPI.GetTickCount())
      {
        if (this.load(YAPI.DefaultCacheValidity) != YAPI.SUCCESS)
        {
          return POWERCONTROL_INVALID;
        }
      }
      return this._powerControl;
    }

    /**
     * <summary>
     *   Changes the selected power source for module functions that require lots of current.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="newval">
     *   a value among <c>YDualPower.POWERCONTROL_AUTO</c>, <c>YDualPower.POWERCONTROL_FROM_USB</c>,
     *   <c>YDualPower.POWERCONTROL_FROM_EXT</c> and <c>YDualPower.POWERCONTROL_OFF</c> corresponding to the
     *   selected power source for module functions that require lots of current
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
    public int set_powerControl(int newval)
    {
      string rest_val;
      rest_val = (newval).ToString();
      return _setAttr("powerControl", rest_val);
    }

    /**
     * <summary>
     *   Returns the measured voltage on the external power source, in millivolts.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   an integer corresponding to the measured voltage on the external power source, in millivolts
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YDualPower.EXTVOLTAGE_INVALID</c>.
     * </para>
     */
    public int get_extVoltage()
    {
      if (this._cacheExpiration <= YAPI.GetTickCount())
      {
        if (this.load(YAPI.DefaultCacheValidity) != YAPI.SUCCESS)
        {
          return EXTVOLTAGE_INVALID;
        }
      }
      return this._extVoltage;
    }

    /**
     * <summary>
     *   Retrieves a dual power control for a given identifier.
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
     *   This function does not require that the power control is online at the time
     *   it is invoked. The returned object is nevertheless valid.
     *   Use the method <c>YDualPower.isOnline()</c> to test if the power control is
     *   indeed online at a given time. In case of ambiguity when looking for
     *   a dual power control by logical name, no error is notified: the first instance
     *   found is returned. The search is performed first by hardware name,
     *   then by logical name.
     * </para>
     * </summary>
     * <param name="func">
     *   a string that uniquely characterizes the power control
     * </param>
     * <returns>
     *   a <c>YDualPower</c> object allowing you to drive the power control.
     * </returns>
     */
    public static YDualPower FindDualPower(string func)
    {
      YDualPower obj;
      obj = (YDualPower)YFunction._FindFromCache("DualPower", func);
      if (obj == null)
      {
        obj = new YDualPower(func);
        YFunction._AddToCache("DualPower", func, obj);
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
      this._valueCallbackDualPower = callback;
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
      if (this._valueCallbackDualPower != null)
      {
        this._valueCallbackDualPower(this, value);
      }
      else
      {
        base._invokeValueCallback(value);
      }
      return 0;
    }

    /**
     * <summary>
     *   Continues the enumeration of dual power controls started using <c>yFirstDualPower()</c>.
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   a pointer to a <c>YDualPower</c> object, corresponding to
     *   a dual power control currently online, or a <c>null</c> pointer
     *   if there are no more dual power controls to enumerate.
     * </returns>
     */
    public YDualPower nextDualPower()
    {
      string hwid = "";
      if (YAPI.YISERR(_nextFunction(ref hwid)))
        return null;
      if (hwid == "")
        return null;
      return FindDualPower(hwid);
    }

    //--- (end of YDualPower implementation)

    //--- (DualPower functions)

    /**
     * <summary>
     *   Starts the enumeration of dual power controls currently accessible.
     * <para>
     *   Use the method <c>YDualPower.nextDualPower()</c> to iterate on
     *   next dual power controls.
     * </para>
     * </summary>
     * <returns>
     *   a pointer to a <c>YDualPower</c> object, corresponding to
     *   the first dual power control currently online, or a <c>null</c> pointer
     *   if there are none.
     * </returns>
     */
    public static YDualPower FirstDualPower()
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
      err = YAPI.apiGetFunctionsByClass("DualPower", 0, p, size, ref neededsize, ref errmsg);
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
      return FindDualPower(serial + "." + funcId);
    }



    //--- (end of DualPower functions)
  }
}