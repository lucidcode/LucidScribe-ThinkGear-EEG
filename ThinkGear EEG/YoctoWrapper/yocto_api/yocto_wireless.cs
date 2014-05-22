/*********************************************************************
 *
 * $Id: yocto_wireless.cs 15131 2014-02-28 10:23:25Z seb $
 *
 * Implements yFindWireless(), the high-level API for Wireless functions
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
 *  THE SOFTWARE AND DOCUMENTATION ARE PROVIDED "AS IS" WITHOUT
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
  //--- (generated code: YWlanRecord class start)
  public class YWlanRecord
  {
    //--- (end of generated code: YWlanRecord class start)
    //--- (generated code: YWlanRecord definitions)

    protected string _ssid;
    protected int _channel = 0;
    protected string _sec;
    protected int _rssi = 0;
    //--- (end of generated code: YWlanRecord definitions)

    public YWlanRecord(string data)
    {
      YAPI.TJsonParser p;
      Nullable<YAPI.TJSONRECORD> node;
      //--- (generated code: YWlanRecord attributes initialization)
      //--- (end of generated code: YWlanRecord attributes initialization)
      p = new YAPI.TJsonParser(data, false);
      node = p.GetChildNode(null, "ssid");
      this._ssid = node.Value.svalue;
      node = p.GetChildNode(null, "sec");
      this._sec = node.Value.svalue;
      node = p.GetChildNode(null, "rssi");
      this._rssi = (int)node.Value.ivalue;
      node = p.GetChildNode(null, "channel");
      this._channel = (int)node.Value.ivalue;
    }

    //--- (generated code: YWlanRecord implementation)


    public virtual string get_ssid()
    {
      return this._ssid;
    }

    public virtual int get_channel()
    {
      return this._channel;
    }

    public virtual string get_security()
    {
      return this._sec;
    }

    public virtual int get_linkQuality()
    {
      return this._rssi;
    }

    //--- (end of generated code: YWlanRecord implementation)

  }


  //--- (generated code: YWireless class start)
  /**
   * <summary>
   *   YWireless functions provides control over wireless network parameters
   *   and status for devices that are wireless-enabled.
   * <para>
   * </para>
   * <para>
   * </para>
   * </summary>
   */
  public class YWireless : YFunction
  {
    //--- (end of generated code: YWireless class start)
    //--- (generated code: YWireless definitions)
    public new delegate void ValueCallback(YWireless func, string value);
    public new delegate void TimedReportCallback(YWireless func, YMeasure measure);

    public const int LINKQUALITY_INVALID = YAPI.INVALID_UINT;
    public const string SSID_INVALID = YAPI.INVALID_STRING;
    public const int CHANNEL_INVALID = YAPI.INVALID_UINT;
    public const int SECURITY_UNKNOWN = 0;
    public const int SECURITY_OPEN = 1;
    public const int SECURITY_WEP = 2;
    public const int SECURITY_WPA = 3;
    public const int SECURITY_WPA2 = 4;
    public const int SECURITY_INVALID = -1;

    public const string MESSAGE_INVALID = YAPI.INVALID_STRING;
    public const string WLANCONFIG_INVALID = YAPI.INVALID_STRING;
    protected int _linkQuality = LINKQUALITY_INVALID;
    protected string _ssid = SSID_INVALID;
    protected int _channel = CHANNEL_INVALID;
    protected int _security = SECURITY_INVALID;
    protected string _message = MESSAGE_INVALID;
    protected string _wlanConfig = WLANCONFIG_INVALID;
    protected ValueCallback _valueCallbackWireless = null;
    //--- (end of generated code: YWireless definitions)

    public YWireless(string func)
      : base(func)
    {
      _className = "Wireless";
      //--- (generated code: YWireless attributes initialization)
      //--- (end of generated code: YWireless attributes initialization)
    }

    //--- (generated code: YWireless implementation)

    protected override void _parseAttr(YAPI.TJSONRECORD member)
    {
      if (member.name == "linkQuality")
      {
        _linkQuality = (int)member.ivalue;
        return;
      }
      if (member.name == "ssid")
      {
        _ssid = member.svalue;
        return;
      }
      if (member.name == "channel")
      {
        _channel = (int)member.ivalue;
        return;
      }
      if (member.name == "security")
      {
        _security = (int)member.ivalue;
        return;
      }
      if (member.name == "message")
      {
        _message = member.svalue;
        return;
      }
      if (member.name == "wlanConfig")
      {
        _wlanConfig = member.svalue;
        return;
      }
      base._parseAttr(member);
    }

    /**
     * <summary>
     *   Returns the link quality, expressed in percent.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   an integer corresponding to the link quality, expressed in percent
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YWireless.LINKQUALITY_INVALID</c>.
     * </para>
     */
    public int get_linkQuality()
    {
      if (this._cacheExpiration <= YAPI.GetTickCount())
      {
        if (this.load(YAPI.DefaultCacheValidity) != YAPI.SUCCESS)
        {
          return LINKQUALITY_INVALID;
        }
      }
      return this._linkQuality;
    }

    /**
     * <summary>
     *   Returns the wireless network name (SSID).
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   a string corresponding to the wireless network name (SSID)
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YWireless.SSID_INVALID</c>.
     * </para>
     */
    public string get_ssid()
    {
      if (this._cacheExpiration <= YAPI.GetTickCount())
      {
        if (this.load(YAPI.DefaultCacheValidity) != YAPI.SUCCESS)
        {
          return SSID_INVALID;
        }
      }
      return this._ssid;
    }

    /**
     * <summary>
     *   Returns the 802.11 channel currently used, or 0 when the selected network has not been found.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   an integer corresponding to the 802.11 channel currently used, or 0 when the selected network has not been found
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YWireless.CHANNEL_INVALID</c>.
     * </para>
     */
    public int get_channel()
    {
      if (this._cacheExpiration <= YAPI.GetTickCount())
      {
        if (this.load(YAPI.DefaultCacheValidity) != YAPI.SUCCESS)
        {
          return CHANNEL_INVALID;
        }
      }
      return this._channel;
    }

    /**
     * <summary>
     *   Returns the security algorithm used by the selected wireless network.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   a value among <c>YWireless.SECURITY_UNKNOWN</c>, <c>YWireless.SECURITY_OPEN</c>,
     *   <c>YWireless.SECURITY_WEP</c>, <c>YWireless.SECURITY_WPA</c> and <c>YWireless.SECURITY_WPA2</c>
     *   corresponding to the security algorithm used by the selected wireless network
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YWireless.SECURITY_INVALID</c>.
     * </para>
     */
    public int get_security()
    {
      if (this._cacheExpiration <= YAPI.GetTickCount())
      {
        if (this.load(YAPI.DefaultCacheValidity) != YAPI.SUCCESS)
        {
          return SECURITY_INVALID;
        }
      }
      return this._security;
    }

    /**
     * <summary>
     *   Returns the latest status message from the wireless interface.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   a string corresponding to the latest status message from the wireless interface
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YWireless.MESSAGE_INVALID</c>.
     * </para>
     */
    public string get_message()
    {
      if (this._cacheExpiration <= YAPI.GetTickCount())
      {
        if (this.load(YAPI.DefaultCacheValidity) != YAPI.SUCCESS)
        {
          return MESSAGE_INVALID;
        }
      }
      return this._message;
    }

    public string get_wlanConfig()
    {
      if (this._cacheExpiration <= YAPI.GetTickCount())
      {
        if (this.load(YAPI.DefaultCacheValidity) != YAPI.SUCCESS)
        {
          return WLANCONFIG_INVALID;
        }
      }
      return this._wlanConfig;
    }

    public int set_wlanConfig(string newval)
    {
      string rest_val;
      rest_val = newval;
      return _setAttr("wlanConfig", rest_val);
    }

    /**
     * <summary>
     *   Changes the configuration of the wireless lan interface to connect to an existing
     *   access point (infrastructure mode).
     * <para>
     *   Remember to call the <c>saveToFlash()</c> method and then to reboot the module to apply this setting.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="ssid">
     *   the name of the network to connect to
     * </param>
     * <param name="securityKey">
     *   the network key, as a character string
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
    public int joinNetwork(string ssid, string securityKey)
    {
      string rest_val;
      rest_val = "INFRA:" + ssid + "\\" + securityKey;
      return _setAttr("wlanConfig", rest_val);
    }

    /**
     * <summary>
     *   Changes the configuration of the wireless lan interface to create an ad-hoc
     *   wireless network, without using an access point.
     * <para>
     *   If a security key is specified,
     *   the network is protected by WEP128, since WPA is not standardized for
     *   ad-hoc networks.
     *   Remember to call the <c>saveToFlash()</c> method and then to reboot the module to apply this setting.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="ssid">
     *   the name of the network to connect to
     * </param>
     * <param name="securityKey">
     *   the network key, as a character string
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
    public int adhocNetwork(string ssid, string securityKey)
    {
      string rest_val;
      rest_val = "ADHOC:" + ssid + "\\" + securityKey;
      return _setAttr("wlanConfig", rest_val);
    }

    /**
     * <summary>
     *   Retrieves a wireless lan interface for a given identifier.
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
     *   This function does not require that the wireless lan interface is online at the time
     *   it is invoked. The returned object is nevertheless valid.
     *   Use the method <c>YWireless.isOnline()</c> to test if the wireless lan interface is
     *   indeed online at a given time. In case of ambiguity when looking for
     *   a wireless lan interface by logical name, no error is notified: the first instance
     *   found is returned. The search is performed first by hardware name,
     *   then by logical name.
     * </para>
     * </summary>
     * <param name="func">
     *   a string that uniquely characterizes the wireless lan interface
     * </param>
     * <returns>
     *   a <c>YWireless</c> object allowing you to drive the wireless lan interface.
     * </returns>
     */
    public static YWireless FindWireless(string func)
    {
      YWireless obj;
      obj = (YWireless)YFunction._FindFromCache("Wireless", func);
      if (obj == null)
      {
        obj = new YWireless(func);
        YFunction._AddToCache("Wireless", func, obj);
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
      this._valueCallbackWireless = callback;
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
      if (this._valueCallbackWireless != null)
      {
        this._valueCallbackWireless(this, value);
      }
      else
      {
        base._invokeValueCallback(value);
      }
      return 0;
    }

    /**
     * <summary>
     *   Returns a list of YWlanRecord objects that describe detected Wireless networks.
     * <para>
     *   This list is not updated when the module is already connected to an acces point (infrastructure mode).
     *   To force an update of this list, <c>adhocNetwork()</c> must be called to disconnect
     *   the module from the current network. The returned list must be unallocated by the caller.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   a list of <c>YWlanRecord</c> objects, containing the SSID, channel,
     *   link quality and the type of security of the wireless network.
     * </returns>
     * <para>
     *   On failure, throws an exception or returns an empty list.
     * </para>
     */
    public virtual List<YWlanRecord> get_detectedWlans()
    {
      byte[] json;
      List<string> wlanlist = new List<string>();
      List<YWlanRecord> res = new List<YWlanRecord>();
      // may throw an exception
      json = this._download("wlan.json?by=name");
      wlanlist = this._json_get_array(json);
      res.Clear();
      for (int ii = 0; ii < wlanlist.Count; ii++)
      {
        res.Add(new YWlanRecord(wlanlist[ii]));
      }
      return res;
    }

    /**
     * <summary>
     *   Continues the enumeration of wireless lan interfaces started using <c>yFirstWireless()</c>.
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   a pointer to a <c>YWireless</c> object, corresponding to
     *   a wireless lan interface currently online, or a <c>null</c> pointer
     *   if there are no more wireless lan interfaces to enumerate.
     * </returns>
     */
    public YWireless nextWireless()
    {
      string hwid = "";
      if (YAPI.YISERR(_nextFunction(ref hwid)))
        return null;
      if (hwid == "")
        return null;
      return FindWireless(hwid);
    }

    //--- (end of generated code: YWireless implementation)

    //--- (generated code: Wireless functions)

    /**
     * <summary>
     *   Starts the enumeration of wireless lan interfaces currently accessible.
     * <para>
     *   Use the method <c>YWireless.nextWireless()</c> to iterate on
     *   next wireless lan interfaces.
     * </para>
     * </summary>
     * <returns>
     *   a pointer to a <c>YWireless</c> object, corresponding to
     *   the first wireless lan interface currently online, or a <c>null</c> pointer
     *   if there are none.
     * </returns>
     */
    public static YWireless FirstWireless()
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
      err = YAPI.apiGetFunctionsByClass("Wireless", 0, p, size, ref neededsize, ref errmsg);
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
      return FindWireless(serial + "." + funcId);
    }



    //--- (end of generated code: Wireless functions)
  }
}