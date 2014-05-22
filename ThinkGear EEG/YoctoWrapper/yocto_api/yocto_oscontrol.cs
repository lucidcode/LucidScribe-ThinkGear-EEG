/*********************************************************************
 *
 * $Id: yocto_oscontrol.cs 15251 2014-03-06 10:14:33Z seb $
 *
 * Implements yFindOsControl(), the high-level API for OsControl functions
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
  //--- (YOsControl return codes)
  //--- (end of YOsControl return codes)
  //--- (YOsControl class start)
  /**
   * <summary>
   *   The OScontrol object allows some control over the operating system running a VirtualHub.
   * <para>
   *   OsControl is available on the VirtualHub software only. This feature must be activated at the VirtualHub
   *   start up with -o option.
   * </para>
   * <para>
   * </para>
   * </summary>
   */
  public class YOsControl : YFunction
  {
    //--- (end of YOsControl class start)
    //--- (YOsControl definitions)
    public new delegate void ValueCallback(YOsControl func, string value);
    public new delegate void TimedReportCallback(YOsControl func, YMeasure measure);

    public const int SHUTDOWNCOUNTDOWN_INVALID = YAPI.INVALID_UINT;
    protected int _shutdownCountdown = SHUTDOWNCOUNTDOWN_INVALID;
    protected ValueCallback _valueCallbackOsControl = null;
    //--- (end of YOsControl definitions)

    public YOsControl(string func)
      : base(func)
    {
      _className = "OsControl";
      //--- (YOsControl attributes initialization)
      //--- (end of YOsControl attributes initialization)
    }

    //--- (YOsControl implementation)

    protected override void _parseAttr(YAPI.TJSONRECORD member)
    {
      if (member.name == "shutdownCountdown")
      {
        _shutdownCountdown = (int)member.ivalue;
        return;
      }
      base._parseAttr(member);
    }

    /**
     * <summary>
     *   Returns the remaining number of seconds before the OS shutdown, or zero when no
     *   shutdown has been scheduled.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   an integer corresponding to the remaining number of seconds before the OS shutdown, or zero when no
     *   shutdown has been scheduled
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YOsControl.SHUTDOWNCOUNTDOWN_INVALID</c>.
     * </para>
     */
    public int get_shutdownCountdown()
    {
      if (this._cacheExpiration <= YAPI.GetTickCount())
      {
        if (this.load(YAPI.DefaultCacheValidity) != YAPI.SUCCESS)
        {
          return SHUTDOWNCOUNTDOWN_INVALID;
        }
      }
      return this._shutdownCountdown;
    }

    public int set_shutdownCountdown(int newval)
    {
      string rest_val;
      rest_val = (newval).ToString();
      return _setAttr("shutdownCountdown", rest_val);
    }

    /**
     * <summary>
     *   Retrieves OS control for a given identifier.
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
     *   This function does not require that the OS control is online at the time
     *   it is invoked. The returned object is nevertheless valid.
     *   Use the method <c>YOsControl.isOnline()</c> to test if the OS control is
     *   indeed online at a given time. In case of ambiguity when looking for
     *   OS control by logical name, no error is notified: the first instance
     *   found is returned. The search is performed first by hardware name,
     *   then by logical name.
     * </para>
     * </summary>
     * <param name="func">
     *   a string that uniquely characterizes the OS control
     * </param>
     * <returns>
     *   a <c>YOsControl</c> object allowing you to drive the OS control.
     * </returns>
     */
    public static YOsControl FindOsControl(string func)
    {
      YOsControl obj;
      obj = (YOsControl)YFunction._FindFromCache("OsControl", func);
      if (obj == null)
      {
        obj = new YOsControl(func);
        YFunction._AddToCache("OsControl", func, obj);
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
      this._valueCallbackOsControl = callback;
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
      if (this._valueCallbackOsControl != null)
      {
        this._valueCallbackOsControl(this, value);
      }
      else
      {
        base._invokeValueCallback(value);
      }
      return 0;
    }

    /**
     * <summary>
     *   Schedules an OS shutdown after a given number of seconds.
     * <para>
     * </para>
     * </summary>
     * <param name="secBeforeShutDown">
     *   number of seconds before shutdown
     * </param>
     * <returns>
     *   <c>YAPI.SUCCESS</c> when the call succeeds.
     * </returns>
     * <para>
     *   On failure, throws an exception or returns a negative error code.
     * </para>
     */
    public virtual int shutdown(int secBeforeShutDown)
    {
      return this.set_shutdownCountdown(secBeforeShutDown);
    }

    /**
     * <summary>
     *   Continues the enumeration of OS control started using <c>yFirstOsControl()</c>.
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   a pointer to a <c>YOsControl</c> object, corresponding to
     *   OS control currently online, or a <c>null</c> pointer
     *   if there are no more OS control to enumerate.
     * </returns>
     */
    public YOsControl nextOsControl()
    {
      string hwid = "";
      if (YAPI.YISERR(_nextFunction(ref hwid)))
        return null;
      if (hwid == "")
        return null;
      return FindOsControl(hwid);
    }

    //--- (end of YOsControl implementation)

    //--- (OsControl functions)

    /**
     * <summary>
     *   Starts the enumeration of OS control currently accessible.
     * <para>
     *   Use the method <c>YOsControl.nextOsControl()</c> to iterate on
     *   next OS control.
     * </para>
     * </summary>
     * <returns>
     *   a pointer to a <c>YOsControl</c> object, corresponding to
     *   the first OS control currently online, or a <c>null</c> pointer
     *   if there are none.
     * </returns>
     */
    public static YOsControl FirstOsControl()
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
      err = YAPI.apiGetFunctionsByClass("OsControl", 0, p, size, ref neededsize, ref errmsg);
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
      return FindOsControl(serial + "." + funcId);
    }



    //--- (end of OsControl functions)
  }
}