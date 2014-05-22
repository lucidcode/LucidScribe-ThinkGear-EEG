/*********************************************************************
 *
 * $Id: yocto_relay.cs 15251 2014-03-06 10:14:33Z seb $
 *
 * Implements yFindRelay(), the high-level API for Relay functions
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
  //--- (YRelay return codes)
  //--- (end of YRelay return codes)
  //--- (YRelay class start)
  /**
   * <summary>
   *   The Yoctopuce application programming interface allows you to switch the relay state.
   * <para>
   *   This change is not persistent: the relay will automatically return to its idle position
   *   whenever power is lost or if the module is restarted.
   *   The library can also generate automatically short pulses of determined duration.
   *   On devices with two output for each relay (double throw), the two outputs are named A and B,
   *   with output A corresponding to the idle position (at power off) and the output B corresponding to the
   *   active state. If you prefer the alternate default state, simply switch your cables on the board.
   * </para>
   * <para>
   * </para>
   * </summary>
   */
  public class YRelay : YFunction
  {
    //--- (end of YRelay class start)
    //--- (YRelay definitions)
    public new delegate void ValueCallback(YRelay func, string value);
    public new delegate void TimedReportCallback(YRelay func, YMeasure measure);

    public class YRelayDelayedPulse
    {
      public int target = YAPI.INVALID_INT;
      public int ms = YAPI.INVALID_INT;
      public int moving = YAPI.INVALID_UINT;
    }

    public const int STATE_A = 0;
    public const int STATE_B = 1;
    public const int STATE_INVALID = -1;

    public const int STATEATPOWERON_UNCHANGED = 0;
    public const int STATEATPOWERON_A = 1;
    public const int STATEATPOWERON_B = 2;
    public const int STATEATPOWERON_INVALID = -1;

    public const long MAXTIMEONSTATEA_INVALID = YAPI.INVALID_LONG;
    public const long MAXTIMEONSTATEB_INVALID = YAPI.INVALID_LONG;
    public const int OUTPUT_OFF = 0;
    public const int OUTPUT_ON = 1;
    public const int OUTPUT_INVALID = -1;

    public const long PULSETIMER_INVALID = YAPI.INVALID_LONG;
    public const long COUNTDOWN_INVALID = YAPI.INVALID_LONG;
    public static readonly YRelayDelayedPulse DELAYEDPULSETIMER_INVALID = null;
    protected int _state = STATE_INVALID;
    protected int _stateAtPowerOn = STATEATPOWERON_INVALID;
    protected long _maxTimeOnStateA = MAXTIMEONSTATEA_INVALID;
    protected long _maxTimeOnStateB = MAXTIMEONSTATEB_INVALID;
    protected int _output = OUTPUT_INVALID;
    protected long _pulseTimer = PULSETIMER_INVALID;
    protected YRelayDelayedPulse _delayedPulseTimer = new YRelayDelayedPulse();
    protected long _countdown = COUNTDOWN_INVALID;
    protected ValueCallback _valueCallbackRelay = null;
    //--- (end of YRelay definitions)

    public YRelay(string func)
      : base(func)
    {
      _className = "Relay";
      //--- (YRelay attributes initialization)
      //--- (end of YRelay attributes initialization)
    }

    //--- (YRelay implementation)

    protected override void _parseAttr(YAPI.TJSONRECORD member)
    {
      if (member.name == "state")
      {
        _state = member.ivalue > 0 ? 1 : 0;
        return;
      }
      if (member.name == "stateAtPowerOn")
      {
        _stateAtPowerOn = (int)member.ivalue;
        return;
      }
      if (member.name == "maxTimeOnStateA")
      {
        _maxTimeOnStateA = member.ivalue;
        return;
      }
      if (member.name == "maxTimeOnStateB")
      {
        _maxTimeOnStateB = member.ivalue;
        return;
      }
      if (member.name == "output")
      {
        _output = member.ivalue > 0 ? 1 : 0;
        return;
      }
      if (member.name == "pulseTimer")
      {
        _pulseTimer = member.ivalue;
        return;
      }
      if (member.name == "delayedPulseTimer")
      {
        if (member.recordtype == YAPI.TJSONRECORDTYPE.JSON_STRUCT)
        {
          YAPI.TJSONRECORD submemb;
          for (int l = 0; l < member.membercount; l++)
          {
            submemb = member.members[l];
            if (submemb.name == "moving")
              _delayedPulseTimer.moving = (int)submemb.ivalue;
            else if (submemb.name == "target")
              _delayedPulseTimer.target = (int)submemb.ivalue;
            else if (submemb.name == "ms")
              _delayedPulseTimer.ms = (int)submemb.ivalue;
          }
        }
        return;
      }
      if (member.name == "countdown")
      {
        _countdown = member.ivalue;
        return;
      }
      base._parseAttr(member);
    }

    /**
     * <summary>
     *   Returns the state of the relays (A for the idle position, B for the active position).
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   either <c>YRelay.STATE_A</c> or <c>YRelay.STATE_B</c>, according to the state of the relays (A for
     *   the idle position, B for the active position)
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YRelay.STATE_INVALID</c>.
     * </para>
     */
    public int get_state()
    {
      if (this._cacheExpiration <= YAPI.GetTickCount())
      {
        if (this.load(YAPI.DefaultCacheValidity) != YAPI.SUCCESS)
        {
          return STATE_INVALID;
        }
      }
      return this._state;
    }

    /**
     * <summary>
     *   Changes the state of the relays (A for the idle position, B for the active position).
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="newval">
     *   either <c>YRelay.STATE_A</c> or <c>YRelay.STATE_B</c>, according to the state of the relays (A for
     *   the idle position, B for the active position)
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
    public int set_state(int newval)
    {
      string rest_val;
      rest_val = (newval > 0 ? "1" : "0");
      return _setAttr("state", rest_val);
    }

    /**
     * <summary>
     *   Returns the state of the relays at device startup (A for the idle position, B for the active position, UNCHANGED for no change).
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   a value among <c>YRelay.STATEATPOWERON_UNCHANGED</c>, <c>YRelay.STATEATPOWERON_A</c> and
     *   <c>YRelay.STATEATPOWERON_B</c> corresponding to the state of the relays at device startup (A for
     *   the idle position, B for the active position, UNCHANGED for no change)
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YRelay.STATEATPOWERON_INVALID</c>.
     * </para>
     */
    public int get_stateAtPowerOn()
    {
      if (this._cacheExpiration <= YAPI.GetTickCount())
      {
        if (this.load(YAPI.DefaultCacheValidity) != YAPI.SUCCESS)
        {
          return STATEATPOWERON_INVALID;
        }
      }
      return this._stateAtPowerOn;
    }

    /**
     * <summary>
     *   Preset the state of the relays at device startup (A for the idle position,
     *   B for the active position, UNCHANGED for no modification).
     * <para>
     *   Remember to call the matching module <c>saveToFlash()</c>
     *   method, otherwise this call will have no effect.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="newval">
     *   a value among <c>YRelay.STATEATPOWERON_UNCHANGED</c>, <c>YRelay.STATEATPOWERON_A</c> and
     *   <c>YRelay.STATEATPOWERON_B</c>
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
    public int set_stateAtPowerOn(int newval)
    {
      string rest_val;
      rest_val = (newval).ToString();
      return _setAttr("stateAtPowerOn", rest_val);
    }

    /**
     * <summary>
     *   Retourne the maximum time (ms) allowed for $THEFUNCTIONS$ to stay in state A before automatically switching back in to B state.
     * <para>
     *   Zero means no maximum time.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   an integer
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YRelay.MAXTIMEONSTATEA_INVALID</c>.
     * </para>
     */
    public long get_maxTimeOnStateA()
    {
      if (this._cacheExpiration <= YAPI.GetTickCount())
      {
        if (this.load(YAPI.DefaultCacheValidity) != YAPI.SUCCESS)
        {
          return MAXTIMEONSTATEA_INVALID;
        }
      }
      return this._maxTimeOnStateA;
    }

    /**
     * <summary>
     *   Sets the maximum time (ms) allowed for $THEFUNCTIONS$ to stay in state A before automatically switching back in to B state.
     * <para>
     *   Use zero for no maximum time.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="newval">
     *   an integer
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
    public int set_maxTimeOnStateA(long newval)
    {
      string rest_val;
      rest_val = (newval).ToString();
      return _setAttr("maxTimeOnStateA", rest_val);
    }

    /**
     * <summary>
     *   Retourne the maximum time (ms) allowed for $THEFUNCTIONS$ to stay in state B before automatically switching back in to A state.
     * <para>
     *   Zero means no maximum time.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   an integer
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YRelay.MAXTIMEONSTATEB_INVALID</c>.
     * </para>
     */
    public long get_maxTimeOnStateB()
    {
      if (this._cacheExpiration <= YAPI.GetTickCount())
      {
        if (this.load(YAPI.DefaultCacheValidity) != YAPI.SUCCESS)
        {
          return MAXTIMEONSTATEB_INVALID;
        }
      }
      return this._maxTimeOnStateB;
    }

    /**
     * <summary>
     *   Sets the maximum time (ms) allowed for $THEFUNCTIONS$ to stay in state B before automatically switching back in to A state.
     * <para>
     *   Use zero for no maximum time.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="newval">
     *   an integer
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
    public int set_maxTimeOnStateB(long newval)
    {
      string rest_val;
      rest_val = (newval).ToString();
      return _setAttr("maxTimeOnStateB", rest_val);
    }

    /**
     * <summary>
     *   Returns the output state of the relays, when used as a simple switch (single throw).
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   either <c>YRelay.OUTPUT_OFF</c> or <c>YRelay.OUTPUT_ON</c>, according to the output state of the
     *   relays, when used as a simple switch (single throw)
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YRelay.OUTPUT_INVALID</c>.
     * </para>
     */
    public int get_output()
    {
      if (this._cacheExpiration <= YAPI.GetTickCount())
      {
        if (this.load(YAPI.DefaultCacheValidity) != YAPI.SUCCESS)
        {
          return OUTPUT_INVALID;
        }
      }
      return this._output;
    }

    /**
     * <summary>
     *   Changes the output state of the relays, when used as a simple switch (single throw).
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="newval">
     *   either <c>YRelay.OUTPUT_OFF</c> or <c>YRelay.OUTPUT_ON</c>, according to the output state of the
     *   relays, when used as a simple switch (single throw)
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
    public int set_output(int newval)
    {
      string rest_val;
      rest_val = (newval > 0 ? "1" : "0");
      return _setAttr("output", rest_val);
    }

    /**
     * <summary>
     *   Returns the number of milliseconds remaining before the relays is returned to idle position
     *   (state A), during a measured pulse generation.
     * <para>
     *   When there is no ongoing pulse, returns zero.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   an integer corresponding to the number of milliseconds remaining before the relays is returned to idle position
     *   (state A), during a measured pulse generation
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YRelay.PULSETIMER_INVALID</c>.
     * </para>
     */
    public long get_pulseTimer()
    {
      if (this._cacheExpiration <= YAPI.GetTickCount())
      {
        if (this.load(YAPI.DefaultCacheValidity) != YAPI.SUCCESS)
        {
          return PULSETIMER_INVALID;
        }
      }
      return this._pulseTimer;
    }

    public int set_pulseTimer(long newval)
    {
      string rest_val;
      rest_val = (newval).ToString();
      return _setAttr("pulseTimer", rest_val);
    }

    /**
     * <summary>
     *   Sets the relay to output B (active) for a specified duration, then brings it
     *   automatically back to output A (idle state).
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="ms_duration">
     *   pulse duration, in millisecondes
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
    public int pulse(int ms_duration)
    {
      string rest_val;
      rest_val = (ms_duration).ToString();
      return _setAttr("pulseTimer", rest_val);
    }

    public YRelayDelayedPulse get_delayedPulseTimer()
    {
      if (this._cacheExpiration <= YAPI.GetTickCount())
      {
        if (this.load(YAPI.DefaultCacheValidity) != YAPI.SUCCESS)
        {
          return DELAYEDPULSETIMER_INVALID;
        }
      }
      return this._delayedPulseTimer;
    }

    public int set_delayedPulseTimer(YRelayDelayedPulse newval)
    {
      string rest_val;
      rest_val = (newval.target).ToString() + ":" + (newval.ms).ToString();
      return _setAttr("delayedPulseTimer", rest_val);
    }

    /**
     * <summary>
     *   Schedules a pulse.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="ms_delay">
     *   waiting time before the pulse, in millisecondes
     * </param>
     * <param name="ms_duration">
     *   pulse duration, in millisecondes
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
    public int delayedPulse(int ms_delay, int ms_duration)
    {
      string rest_val;
      rest_val = (ms_delay).ToString() + ":" + (ms_duration).ToString();
      return _setAttr("delayedPulseTimer", rest_val);
    }

    /**
     * <summary>
     *   Returns the number of milliseconds remaining before a pulse (delayedPulse() call)
     *   When there is no scheduled pulse, returns zero.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   an integer corresponding to the number of milliseconds remaining before a pulse (delayedPulse() call)
     *   When there is no scheduled pulse, returns zero
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YRelay.COUNTDOWN_INVALID</c>.
     * </para>
     */
    public long get_countdown()
    {
      if (this._cacheExpiration <= YAPI.GetTickCount())
      {
        if (this.load(YAPI.DefaultCacheValidity) != YAPI.SUCCESS)
        {
          return COUNTDOWN_INVALID;
        }
      }
      return this._countdown;
    }

    /**
     * <summary>
     *   Retrieves a relay for a given identifier.
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
     *   This function does not require that the relay is online at the time
     *   it is invoked. The returned object is nevertheless valid.
     *   Use the method <c>YRelay.isOnline()</c> to test if the relay is
     *   indeed online at a given time. In case of ambiguity when looking for
     *   a relay by logical name, no error is notified: the first instance
     *   found is returned. The search is performed first by hardware name,
     *   then by logical name.
     * </para>
     * </summary>
     * <param name="func">
     *   a string that uniquely characterizes the relay
     * </param>
     * <returns>
     *   a <c>YRelay</c> object allowing you to drive the relay.
     * </returns>
     */
    public static YRelay FindRelay(string func)
    {
      YRelay obj;
      obj = (YRelay)YFunction._FindFromCache("Relay", func);
      if (obj == null)
      {
        obj = new YRelay(func);
        YFunction._AddToCache("Relay", func, obj);
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
      this._valueCallbackRelay = callback;
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
      if (this._valueCallbackRelay != null)
      {
        this._valueCallbackRelay(this, value);
      }
      else
      {
        base._invokeValueCallback(value);
      }
      return 0;
    }

    /**
     * <summary>
     *   Continues the enumeration of relays started using <c>yFirstRelay()</c>.
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   a pointer to a <c>YRelay</c> object, corresponding to
     *   a relay currently online, or a <c>null</c> pointer
     *   if there are no more relays to enumerate.
     * </returns>
     */
    public YRelay nextRelay()
    {
      string hwid = "";
      if (YAPI.YISERR(_nextFunction(ref hwid)))
        return null;
      if (hwid == "")
        return null;
      return FindRelay(hwid);
    }

    //--- (end of YRelay implementation)

    //--- (Relay functions)

    /**
     * <summary>
     *   Starts the enumeration of relays currently accessible.
     * <para>
     *   Use the method <c>YRelay.nextRelay()</c> to iterate on
     *   next relays.
     * </para>
     * </summary>
     * <returns>
     *   a pointer to a <c>YRelay</c> object, corresponding to
     *   the first relay currently online, or a <c>null</c> pointer
     *   if there are none.
     * </returns>
     */
    public static YRelay FirstRelay()
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
      err = YAPI.apiGetFunctionsByClass("Relay", 0, p, size, ref neededsize, ref errmsg);
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
      return FindRelay(serial + "." + funcId);
    }



    //--- (end of Relay functions)
  }
}