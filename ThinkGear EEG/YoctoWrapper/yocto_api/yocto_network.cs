/*********************************************************************
 *
 * $Id: yocto_network.cs 15251 2014-03-06 10:14:33Z seb $
 *
 * Implements yFindNetwork(), the high-level API for Network functions
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
  //--- (YNetwork return codes)
  //--- (end of YNetwork return codes)
  //--- (YNetwork class start)
  /**
   * <summary>
   *   YNetwork objects provide access to TCP/IP parameters of Yoctopuce
   *   modules that include a built-in network interface.
   * <para>
   * </para>
   * <para>
   * </para>
   * </summary>
   */
  public class YNetwork : YFunction
  {
    //--- (end of YNetwork class start)
    //--- (YNetwork definitions)
    public new delegate void ValueCallback(YNetwork func, string value);
    public new delegate void TimedReportCallback(YNetwork func, YMeasure measure);

    public const int READINESS_DOWN = 0;
    public const int READINESS_EXISTS = 1;
    public const int READINESS_LINKED = 2;
    public const int READINESS_LAN_OK = 3;
    public const int READINESS_WWW_OK = 4;
    public const int READINESS_INVALID = -1;

    public const string MACADDRESS_INVALID = YAPI.INVALID_STRING;
    public const string IPADDRESS_INVALID = YAPI.INVALID_STRING;
    public const string SUBNETMASK_INVALID = YAPI.INVALID_STRING;
    public const string ROUTER_INVALID = YAPI.INVALID_STRING;
    public const string IPCONFIG_INVALID = YAPI.INVALID_STRING;
    public const string PRIMARYDNS_INVALID = YAPI.INVALID_STRING;
    public const string SECONDARYDNS_INVALID = YAPI.INVALID_STRING;
    public const string USERPASSWORD_INVALID = YAPI.INVALID_STRING;
    public const string ADMINPASSWORD_INVALID = YAPI.INVALID_STRING;
    public const int DISCOVERABLE_FALSE = 0;
    public const int DISCOVERABLE_TRUE = 1;
    public const int DISCOVERABLE_INVALID = -1;

    public const int WWWWATCHDOGDELAY_INVALID = YAPI.INVALID_UINT;
    public const string CALLBACKURL_INVALID = YAPI.INVALID_STRING;
    public const int CALLBACKMETHOD_POST = 0;
    public const int CALLBACKMETHOD_GET = 1;
    public const int CALLBACKMETHOD_PUT = 2;
    public const int CALLBACKMETHOD_INVALID = -1;

    public const int CALLBACKENCODING_FORM = 0;
    public const int CALLBACKENCODING_JSON = 1;
    public const int CALLBACKENCODING_JSON_ARRAY = 2;
    public const int CALLBACKENCODING_CSV = 3;
    public const int CALLBACKENCODING_YOCTO_API = 4;
    public const int CALLBACKENCODING_INVALID = -1;

    public const string CALLBACKCREDENTIALS_INVALID = YAPI.INVALID_STRING;
    public const int CALLBACKMINDELAY_INVALID = YAPI.INVALID_UINT;
    public const int CALLBACKMAXDELAY_INVALID = YAPI.INVALID_UINT;
    public const int POECURRENT_INVALID = YAPI.INVALID_UINT;
    protected int _readiness = READINESS_INVALID;
    protected string _macAddress = MACADDRESS_INVALID;
    protected string _ipAddress = IPADDRESS_INVALID;
    protected string _subnetMask = SUBNETMASK_INVALID;
    protected string _router = ROUTER_INVALID;
    protected string _ipConfig = IPCONFIG_INVALID;
    protected string _primaryDNS = PRIMARYDNS_INVALID;
    protected string _secondaryDNS = SECONDARYDNS_INVALID;
    protected string _userPassword = USERPASSWORD_INVALID;
    protected string _adminPassword = ADMINPASSWORD_INVALID;
    protected int _discoverable = DISCOVERABLE_INVALID;
    protected int _wwwWatchdogDelay = WWWWATCHDOGDELAY_INVALID;
    protected string _callbackUrl = CALLBACKURL_INVALID;
    protected int _callbackMethod = CALLBACKMETHOD_INVALID;
    protected int _callbackEncoding = CALLBACKENCODING_INVALID;
    protected string _callbackCredentials = CALLBACKCREDENTIALS_INVALID;
    protected int _callbackMinDelay = CALLBACKMINDELAY_INVALID;
    protected int _callbackMaxDelay = CALLBACKMAXDELAY_INVALID;
    protected int _poeCurrent = POECURRENT_INVALID;
    protected ValueCallback _valueCallbackNetwork = null;
    //--- (end of YNetwork definitions)

    public YNetwork(string func)
      : base(func)
    {
      _className = "Network";
      //--- (YNetwork attributes initialization)
      //--- (end of YNetwork attributes initialization)
    }

    //--- (YNetwork implementation)

    protected override void _parseAttr(YAPI.TJSONRECORD member)
    {
      if (member.name == "readiness")
      {
        _readiness = (int)member.ivalue;
        return;
      }
      if (member.name == "macAddress")
      {
        _macAddress = member.svalue;
        return;
      }
      if (member.name == "ipAddress")
      {
        _ipAddress = member.svalue;
        return;
      }
      if (member.name == "subnetMask")
      {
        _subnetMask = member.svalue;
        return;
      }
      if (member.name == "router")
      {
        _router = member.svalue;
        return;
      }
      if (member.name == "ipConfig")
      {
        _ipConfig = member.svalue;
        return;
      }
      if (member.name == "primaryDNS")
      {
        _primaryDNS = member.svalue;
        return;
      }
      if (member.name == "secondaryDNS")
      {
        _secondaryDNS = member.svalue;
        return;
      }
      if (member.name == "userPassword")
      {
        _userPassword = member.svalue;
        return;
      }
      if (member.name == "adminPassword")
      {
        _adminPassword = member.svalue;
        return;
      }
      if (member.name == "discoverable")
      {
        _discoverable = member.ivalue > 0 ? 1 : 0;
        return;
      }
      if (member.name == "wwwWatchdogDelay")
      {
        _wwwWatchdogDelay = (int)member.ivalue;
        return;
      }
      if (member.name == "callbackUrl")
      {
        _callbackUrl = member.svalue;
        return;
      }
      if (member.name == "callbackMethod")
      {
        _callbackMethod = (int)member.ivalue;
        return;
      }
      if (member.name == "callbackEncoding")
      {
        _callbackEncoding = (int)member.ivalue;
        return;
      }
      if (member.name == "callbackCredentials")
      {
        _callbackCredentials = member.svalue;
        return;
      }
      if (member.name == "callbackMinDelay")
      {
        _callbackMinDelay = (int)member.ivalue;
        return;
      }
      if (member.name == "callbackMaxDelay")
      {
        _callbackMaxDelay = (int)member.ivalue;
        return;
      }
      if (member.name == "poeCurrent")
      {
        _poeCurrent = (int)member.ivalue;
        return;
      }
      base._parseAttr(member);
    }

    /**
     * <summary>
     *   Returns the current established working mode of the network interface.
     * <para>
     *   Level zero (DOWN_0) means that no hardware link has been detected. Either there is no signal
     *   on the network cable, or the selected wireless access point cannot be detected.
     *   Level 1 (LIVE_1) is reached when the network is detected, but is not yet connected.
     *   For a wireless network, this shows that the requested SSID is present.
     *   Level 2 (LINK_2) is reached when the hardware connection is established.
     *   For a wired network connection, level 2 means that the cable is attached at both ends.
     *   For a connection to a wireless access point, it shows that the security parameters
     *   are properly configured. For an ad-hoc wireless connection, it means that there is
     *   at least one other device connected on the ad-hoc network.
     *   Level 3 (DHCP_3) is reached when an IP address has been obtained using DHCP.
     *   Level 4 (DNS_4) is reached when the DNS server is reachable on the network.
     *   Level 5 (WWW_5) is reached when global connectivity is demonstrated by properly loading the
     *   current time from an NTP server.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   a value among <c>YNetwork.READINESS_DOWN</c>, <c>YNetwork.READINESS_EXISTS</c>,
     *   <c>YNetwork.READINESS_LINKED</c>, <c>YNetwork.READINESS_LAN_OK</c> and
     *   <c>YNetwork.READINESS_WWW_OK</c> corresponding to the current established working mode of the network interface
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YNetwork.READINESS_INVALID</c>.
     * </para>
     */
    public int get_readiness()
    {
      if (this._cacheExpiration <= YAPI.GetTickCount())
      {
        if (this.load(YAPI.DefaultCacheValidity) != YAPI.SUCCESS)
        {
          return READINESS_INVALID;
        }
      }
      return this._readiness;
    }

    /**
     * <summary>
     *   Returns the MAC address of the network interface.
     * <para>
     *   The MAC address is also available on a sticker
     *   on the module, in both numeric and barcode forms.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   a string corresponding to the MAC address of the network interface
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YNetwork.MACADDRESS_INVALID</c>.
     * </para>
     */
    public string get_macAddress()
    {
      if (this._cacheExpiration == 0)
      {
        if (this.load(YAPI.DefaultCacheValidity) != YAPI.SUCCESS)
        {
          return MACADDRESS_INVALID;
        }
      }
      return this._macAddress;
    }

    /**
     * <summary>
     *   Returns the IP address currently in use by the device.
     * <para>
     *   The address may have been configured
     *   statically, or provided by a DHCP server.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   a string corresponding to the IP address currently in use by the device
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YNetwork.IPADDRESS_INVALID</c>.
     * </para>
     */
    public string get_ipAddress()
    {
      if (this._cacheExpiration <= YAPI.GetTickCount())
      {
        if (this.load(YAPI.DefaultCacheValidity) != YAPI.SUCCESS)
        {
          return IPADDRESS_INVALID;
        }
      }
      return this._ipAddress;
    }

    /**
     * <summary>
     *   Returns the subnet mask currently used by the device.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   a string corresponding to the subnet mask currently used by the device
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YNetwork.SUBNETMASK_INVALID</c>.
     * </para>
     */
    public string get_subnetMask()
    {
      if (this._cacheExpiration <= YAPI.GetTickCount())
      {
        if (this.load(YAPI.DefaultCacheValidity) != YAPI.SUCCESS)
        {
          return SUBNETMASK_INVALID;
        }
      }
      return this._subnetMask;
    }

    /**
     * <summary>
     *   Returns the IP address of the router on the device subnet (default gateway).
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   a string corresponding to the IP address of the router on the device subnet (default gateway)
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YNetwork.ROUTER_INVALID</c>.
     * </para>
     */
    public string get_router()
    {
      if (this._cacheExpiration <= YAPI.GetTickCount())
      {
        if (this.load(YAPI.DefaultCacheValidity) != YAPI.SUCCESS)
        {
          return ROUTER_INVALID;
        }
      }
      return this._router;
    }

    public string get_ipConfig()
    {
      if (this._cacheExpiration <= YAPI.GetTickCount())
      {
        if (this.load(YAPI.DefaultCacheValidity) != YAPI.SUCCESS)
        {
          return IPCONFIG_INVALID;
        }
      }
      return this._ipConfig;
    }

    public int set_ipConfig(string newval)
    {
      string rest_val;
      rest_val = newval;
      return _setAttr("ipConfig", rest_val);
    }

    /**
     * <summary>
     *   Changes the configuration of the network interface to enable the use of an
     *   IP address received from a DHCP server.
     * <para>
     *   Until an address is received from a DHCP
     *   server, the module uses the IP parameters specified to this function.
     *   Remember to call the <c>saveToFlash()</c> method and then to reboot the module to apply this setting.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="fallbackIpAddr">
     *   fallback IP address, to be used when no DHCP reply is received
     * </param>
     * <param name="fallbackSubnetMaskLen">
     *   fallback subnet mask length when no DHCP reply is received, as an
     *   integer (eg. 24 means 255.255.255.0)
     * </param>
     * <param name="fallbackRouter">
     *   fallback router IP address, to be used when no DHCP reply is received
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
    public int useDHCP(string fallbackIpAddr, int fallbackSubnetMaskLen, string fallbackRouter)
    {
      string rest_val;
      rest_val = "DHCP:" + fallbackIpAddr + "/" + fallbackSubnetMaskLen.ToString() + "/" + fallbackRouter;
      return _setAttr("ipConfig", rest_val);
    }

    /**
     * <summary>
     *   Changes the configuration of the network interface to use a static IP address.
     * <para>
     *   Remember to call the <c>saveToFlash()</c> method and then to reboot the module to apply this setting.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="ipAddress">
     *   device IP address
     * </param>
     * <param name="subnetMaskLen">
     *   subnet mask length, as an integer (eg. 24 means 255.255.255.0)
     * </param>
     * <param name="router">
     *   router IP address (default gateway)
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
    public int useStaticIP(string ipAddress, int subnetMaskLen, string router)
    {
      string rest_val;
      rest_val = "STATIC:" + ipAddress + "/" + subnetMaskLen.ToString() + "/" + router;
      return _setAttr("ipConfig", rest_val);
    }

    /**
     * <summary>
     *   Returns the IP address of the primary name server to be used by the module.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   a string corresponding to the IP address of the primary name server to be used by the module
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YNetwork.PRIMARYDNS_INVALID</c>.
     * </para>
     */
    public string get_primaryDNS()
    {
      if (this._cacheExpiration <= YAPI.GetTickCount())
      {
        if (this.load(YAPI.DefaultCacheValidity) != YAPI.SUCCESS)
        {
          return PRIMARYDNS_INVALID;
        }
      }
      return this._primaryDNS;
    }

    /**
     * <summary>
     *   Changes the IP address of the primary name server to be used by the module.
     * <para>
     *   When using DHCP, if a value is specified, it overrides the value received from the DHCP server.
     *   Remember to call the <c>saveToFlash()</c> method and then to reboot the module to apply this setting.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="newval">
     *   a string corresponding to the IP address of the primary name server to be used by the module
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
    public int set_primaryDNS(string newval)
    {
      string rest_val;
      rest_val = newval;
      return _setAttr("primaryDNS", rest_val);
    }

    /**
     * <summary>
     *   Returns the IP address of the secondary name server to be used by the module.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   a string corresponding to the IP address of the secondary name server to be used by the module
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YNetwork.SECONDARYDNS_INVALID</c>.
     * </para>
     */
    public string get_secondaryDNS()
    {
      if (this._cacheExpiration <= YAPI.GetTickCount())
      {
        if (this.load(YAPI.DefaultCacheValidity) != YAPI.SUCCESS)
        {
          return SECONDARYDNS_INVALID;
        }
      }
      return this._secondaryDNS;
    }

    /**
     * <summary>
     *   Changes the IP address of the secondary name server to be used by the module.
     * <para>
     *   When using DHCP, if a value is specified, it overrides the value received from the DHCP server.
     *   Remember to call the <c>saveToFlash()</c> method and then to reboot the module to apply this setting.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="newval">
     *   a string corresponding to the IP address of the secondary name server to be used by the module
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
    public int set_secondaryDNS(string newval)
    {
      string rest_val;
      rest_val = newval;
      return _setAttr("secondaryDNS", rest_val);
    }

    /**
     * <summary>
     *   Returns a hash string if a password has been set for "user" user,
     *   or an empty string otherwise.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   a string corresponding to a hash string if a password has been set for "user" user,
     *   or an empty string otherwise
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YNetwork.USERPASSWORD_INVALID</c>.
     * </para>
     */
    public string get_userPassword()
    {
      if (this._cacheExpiration <= YAPI.GetTickCount())
      {
        if (this.load(YAPI.DefaultCacheValidity) != YAPI.SUCCESS)
        {
          return USERPASSWORD_INVALID;
        }
      }
      return this._userPassword;
    }

    /**
     * <summary>
     *   Changes the password for the "user" user.
     * <para>
     *   This password becomes instantly required
     *   to perform any use of the module. If the specified value is an
     *   empty string, a password is not required anymore.
     *   Remember to call the <c>saveToFlash()</c> method of the module if the
     *   modification must be kept.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="newval">
     *   a string corresponding to the password for the "user" user
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
    public int set_userPassword(string newval)
    {
      string rest_val;
      rest_val = newval;
      return _setAttr("userPassword", rest_val);
    }

    /**
     * <summary>
     *   Returns a hash string if a password has been set for user "admin",
     *   or an empty string otherwise.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   a string corresponding to a hash string if a password has been set for user "admin",
     *   or an empty string otherwise
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YNetwork.ADMINPASSWORD_INVALID</c>.
     * </para>
     */
    public string get_adminPassword()
    {
      if (this._cacheExpiration <= YAPI.GetTickCount())
      {
        if (this.load(YAPI.DefaultCacheValidity) != YAPI.SUCCESS)
        {
          return ADMINPASSWORD_INVALID;
        }
      }
      return this._adminPassword;
    }

    /**
     * <summary>
     *   Changes the password for the "admin" user.
     * <para>
     *   This password becomes instantly required
     *   to perform any change of the module state. If the specified value is an
     *   empty string, a password is not required anymore.
     *   Remember to call the <c>saveToFlash()</c> method of the module if the
     *   modification must be kept.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="newval">
     *   a string corresponding to the password for the "admin" user
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
    public int set_adminPassword(string newval)
    {
      string rest_val;
      rest_val = newval;
      return _setAttr("adminPassword", rest_val);
    }

    /**
     * <summary>
     *   Returns the activation state of the multicast announce protocols to allow easy
     *   discovery of the module in the network neighborhood (uPnP/Bonjour protocol).
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   either <c>YNetwork.DISCOVERABLE_FALSE</c> or <c>YNetwork.DISCOVERABLE_TRUE</c>, according to the
     *   activation state of the multicast announce protocols to allow easy
     *   discovery of the module in the network neighborhood (uPnP/Bonjour protocol)
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YNetwork.DISCOVERABLE_INVALID</c>.
     * </para>
     */
    public int get_discoverable()
    {
      if (this._cacheExpiration <= YAPI.GetTickCount())
      {
        if (this.load(YAPI.DefaultCacheValidity) != YAPI.SUCCESS)
        {
          return DISCOVERABLE_INVALID;
        }
      }
      return this._discoverable;
    }

    /**
     * <summary>
     *   Changes the activation state of the multicast announce protocols to allow easy
     *   discovery of the module in the network neighborhood (uPnP/Bonjour protocol).
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="newval">
     *   either <c>YNetwork.DISCOVERABLE_FALSE</c> or <c>YNetwork.DISCOVERABLE_TRUE</c>, according to the
     *   activation state of the multicast announce protocols to allow easy
     *   discovery of the module in the network neighborhood (uPnP/Bonjour protocol)
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
    public int set_discoverable(int newval)
    {
      string rest_val;
      rest_val = (newval > 0 ? "1" : "0");
      return _setAttr("discoverable", rest_val);
    }

    /**
     * <summary>
     *   Returns the allowed downtime of the WWW link (in seconds) before triggering an automated
     *   reboot to try to recover Internet connectivity.
     * <para>
     *   A zero value disables automated reboot
     *   in case of Internet connectivity loss.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   an integer corresponding to the allowed downtime of the WWW link (in seconds) before triggering an automated
     *   reboot to try to recover Internet connectivity
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YNetwork.WWWWATCHDOGDELAY_INVALID</c>.
     * </para>
     */
    public int get_wwwWatchdogDelay()
    {
      if (this._cacheExpiration <= YAPI.GetTickCount())
      {
        if (this.load(YAPI.DefaultCacheValidity) != YAPI.SUCCESS)
        {
          return WWWWATCHDOGDELAY_INVALID;
        }
      }
      return this._wwwWatchdogDelay;
    }

    /**
     * <summary>
     *   Changes the allowed downtime of the WWW link (in seconds) before triggering an automated
     *   reboot to try to recover Internet connectivity.
     * <para>
     *   A zero value disables automated reboot
     *   in case of Internet connectivity loss. The smallest valid non-zero timeout is
     *   90 seconds.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="newval">
     *   an integer corresponding to the allowed downtime of the WWW link (in seconds) before triggering an automated
     *   reboot to try to recover Internet connectivity
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
    public int set_wwwWatchdogDelay(int newval)
    {
      string rest_val;
      rest_val = (newval).ToString();
      return _setAttr("wwwWatchdogDelay", rest_val);
    }

    /**
     * <summary>
     *   Returns the callback URL to notify of significant state changes.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   a string corresponding to the callback URL to notify of significant state changes
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YNetwork.CALLBACKURL_INVALID</c>.
     * </para>
     */
    public string get_callbackUrl()
    {
      if (this._cacheExpiration <= YAPI.GetTickCount())
      {
        if (this.load(YAPI.DefaultCacheValidity) != YAPI.SUCCESS)
        {
          return CALLBACKURL_INVALID;
        }
      }
      return this._callbackUrl;
    }

    /**
     * <summary>
     *   Changes the callback URL to notify significant state changes.
     * <para>
     *   Remember to call the
     *   <c>saveToFlash()</c> method of the module if the modification must be kept.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="newval">
     *   a string corresponding to the callback URL to notify significant state changes
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
    public int set_callbackUrl(string newval)
    {
      string rest_val;
      rest_val = newval;
      return _setAttr("callbackUrl", rest_val);
    }

    /**
     * <summary>
     *   Returns the HTTP method used to notify callbacks for significant state changes.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   a value among <c>YNetwork.CALLBACKMETHOD_POST</c>, <c>YNetwork.CALLBACKMETHOD_GET</c> and
     *   <c>YNetwork.CALLBACKMETHOD_PUT</c> corresponding to the HTTP method used to notify callbacks for
     *   significant state changes
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YNetwork.CALLBACKMETHOD_INVALID</c>.
     * </para>
     */
    public int get_callbackMethod()
    {
      if (this._cacheExpiration <= YAPI.GetTickCount())
      {
        if (this.load(YAPI.DefaultCacheValidity) != YAPI.SUCCESS)
        {
          return CALLBACKMETHOD_INVALID;
        }
      }
      return this._callbackMethod;
    }

    /**
     * <summary>
     *   Changes the HTTP method used to notify callbacks for significant state changes.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="newval">
     *   a value among <c>YNetwork.CALLBACKMETHOD_POST</c>, <c>YNetwork.CALLBACKMETHOD_GET</c> and
     *   <c>YNetwork.CALLBACKMETHOD_PUT</c> corresponding to the HTTP method used to notify callbacks for
     *   significant state changes
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
    public int set_callbackMethod(int newval)
    {
      string rest_val;
      rest_val = (newval).ToString();
      return _setAttr("callbackMethod", rest_val);
    }

    /**
     * <summary>
     *   Returns the encoding standard to use for representing notification values.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   a value among <c>YNetwork.CALLBACKENCODING_FORM</c>, <c>YNetwork.CALLBACKENCODING_JSON</c>,
     *   <c>YNetwork.CALLBACKENCODING_JSON_ARRAY</c>, <c>YNetwork.CALLBACKENCODING_CSV</c> and
     *   <c>YNetwork.CALLBACKENCODING_YOCTO_API</c> corresponding to the encoding standard to use for
     *   representing notification values
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YNetwork.CALLBACKENCODING_INVALID</c>.
     * </para>
     */
    public int get_callbackEncoding()
    {
      if (this._cacheExpiration <= YAPI.GetTickCount())
      {
        if (this.load(YAPI.DefaultCacheValidity) != YAPI.SUCCESS)
        {
          return CALLBACKENCODING_INVALID;
        }
      }
      return this._callbackEncoding;
    }

    /**
     * <summary>
     *   Changes the encoding standard to use for representing notification values.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="newval">
     *   a value among <c>YNetwork.CALLBACKENCODING_FORM</c>, <c>YNetwork.CALLBACKENCODING_JSON</c>,
     *   <c>YNetwork.CALLBACKENCODING_JSON_ARRAY</c>, <c>YNetwork.CALLBACKENCODING_CSV</c> and
     *   <c>YNetwork.CALLBACKENCODING_YOCTO_API</c> corresponding to the encoding standard to use for
     *   representing notification values
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
    public int set_callbackEncoding(int newval)
    {
      string rest_val;
      rest_val = (newval).ToString();
      return _setAttr("callbackEncoding", rest_val);
    }

    /**
     * <summary>
     *   Returns a hashed version of the notification callback credentials if set,
     *   or an empty string otherwise.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   a string corresponding to a hashed version of the notification callback credentials if set,
     *   or an empty string otherwise
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YNetwork.CALLBACKCREDENTIALS_INVALID</c>.
     * </para>
     */
    public string get_callbackCredentials()
    {
      if (this._cacheExpiration <= YAPI.GetTickCount())
      {
        if (this.load(YAPI.DefaultCacheValidity) != YAPI.SUCCESS)
        {
          return CALLBACKCREDENTIALS_INVALID;
        }
      }
      return this._callbackCredentials;
    }

    /**
     * <summary>
     *   Changes the credentials required to connect to the callback address.
     * <para>
     *   The credentials
     *   must be provided as returned by function <c>get_callbackCredentials</c>,
     *   in the form <c>username:hash</c>. The method used to compute the hash varies according
     *   to the the authentication scheme implemented by the callback, For Basic authentication,
     *   the hash is the MD5 of the string <c>username:password</c>. For Digest authentication,
     *   the hash is the MD5 of the string <c>username:realm:password</c>. For a simpler
     *   way to configure callback credentials, use function <c>callbackLogin</c> instead.
     *   Remember to call the <c>saveToFlash()</c> method of the module if the
     *   modification must be kept.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="newval">
     *   a string corresponding to the credentials required to connect to the callback address
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
    public int set_callbackCredentials(string newval)
    {
      string rest_val;
      rest_val = newval;
      return _setAttr("callbackCredentials", rest_val);
    }

    /**
     * <summary>
     *   Connects to the notification callback and saves the credentials required to
     *   log into it.
     * <para>
     *   The password is not stored into the module, only a hashed
     *   copy of the credentials are saved. Remember to call the
     *   <c>saveToFlash()</c> method of the module if the modification must be kept.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="username">
     *   username required to log to the callback
     * </param>
     * <param name="password">
     *   password required to log to the callback
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
    public int callbackLogin(string username, string password)
    {
      string rest_val;
      rest_val = username + ":" + password;
      return _setAttr("callbackCredentials", rest_val);
    }

    /**
     * <summary>
     *   Returns the minimum waiting time between two callback notifications, in seconds.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   an integer corresponding to the minimum waiting time between two callback notifications, in seconds
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YNetwork.CALLBACKMINDELAY_INVALID</c>.
     * </para>
     */
    public int get_callbackMinDelay()
    {
      if (this._cacheExpiration <= YAPI.GetTickCount())
      {
        if (this.load(YAPI.DefaultCacheValidity) != YAPI.SUCCESS)
        {
          return CALLBACKMINDELAY_INVALID;
        }
      }
      return this._callbackMinDelay;
    }

    /**
     * <summary>
     *   Changes the minimum waiting time between two callback notifications, in seconds.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="newval">
     *   an integer corresponding to the minimum waiting time between two callback notifications, in seconds
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
    public int set_callbackMinDelay(int newval)
    {
      string rest_val;
      rest_val = (newval).ToString();
      return _setAttr("callbackMinDelay", rest_val);
    }

    /**
     * <summary>
     *   Returns the maximum waiting time between two callback notifications, in seconds.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   an integer corresponding to the maximum waiting time between two callback notifications, in seconds
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YNetwork.CALLBACKMAXDELAY_INVALID</c>.
     * </para>
     */
    public int get_callbackMaxDelay()
    {
      if (this._cacheExpiration <= YAPI.GetTickCount())
      {
        if (this.load(YAPI.DefaultCacheValidity) != YAPI.SUCCESS)
        {
          return CALLBACKMAXDELAY_INVALID;
        }
      }
      return this._callbackMaxDelay;
    }

    /**
     * <summary>
     *   Changes the maximum waiting time between two callback notifications, in seconds.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="newval">
     *   an integer corresponding to the maximum waiting time between two callback notifications, in seconds
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
    public int set_callbackMaxDelay(int newval)
    {
      string rest_val;
      rest_val = (newval).ToString();
      return _setAttr("callbackMaxDelay", rest_val);
    }

    /**
     * <summary>
     *   Returns the current consumed by the module from Power-over-Ethernet (PoE), in milli-amps.
     * <para>
     *   The current consumption is measured after converting PoE source to 5 Volt, and should
     *   never exceed 1800 mA.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   an integer corresponding to the current consumed by the module from Power-over-Ethernet (PoE), in milli-amps
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YNetwork.POECURRENT_INVALID</c>.
     * </para>
     */
    public int get_poeCurrent()
    {
      if (this._cacheExpiration <= YAPI.GetTickCount())
      {
        if (this.load(YAPI.DefaultCacheValidity) != YAPI.SUCCESS)
        {
          return POECURRENT_INVALID;
        }
      }
      return this._poeCurrent;
    }

    /**
     * <summary>
     *   Retrieves a network interface for a given identifier.
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
     *   This function does not require that the network interface is online at the time
     *   it is invoked. The returned object is nevertheless valid.
     *   Use the method <c>YNetwork.isOnline()</c> to test if the network interface is
     *   indeed online at a given time. In case of ambiguity when looking for
     *   a network interface by logical name, no error is notified: the first instance
     *   found is returned. The search is performed first by hardware name,
     *   then by logical name.
     * </para>
     * </summary>
     * <param name="func">
     *   a string that uniquely characterizes the network interface
     * </param>
     * <returns>
     *   a <c>YNetwork</c> object allowing you to drive the network interface.
     * </returns>
     */
    public static YNetwork FindNetwork(string func)
    {
      YNetwork obj;
      obj = (YNetwork)YFunction._FindFromCache("Network", func);
      if (obj == null)
      {
        obj = new YNetwork(func);
        YFunction._AddToCache("Network", func, obj);
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
      this._valueCallbackNetwork = callback;
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
      if (this._valueCallbackNetwork != null)
      {
        this._valueCallbackNetwork(this, value);
      }
      else
      {
        base._invokeValueCallback(value);
      }
      return 0;
    }

    /**
     * <summary>
     *   Pings str_host to test the network connectivity.
     * <para>
     *   Sends four ICMP ECHO_REQUEST requests from the
     *   module to the target str_host. This method returns a string with the result of the
     *   4 ICMP ECHO_REQUEST requests.
     * </para>
     * </summary>
     * <param name="host">
     *   the hostname or the IP address of the target
     * </param>
     * <para>
     * </para>
     * <returns>
     *   a string with the result of the ping.
     * </returns>
     */
    public virtual string ping(string host)
    {
      byte[] content;
      // may throw an exception
      content = this._download("ping.txt?host=" + host);
      return YAPI.DefaultEncoding.GetString(content);
    }

    /**
     * <summary>
     *   Continues the enumeration of network interfaces started using <c>yFirstNetwork()</c>.
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   a pointer to a <c>YNetwork</c> object, corresponding to
     *   a network interface currently online, or a <c>null</c> pointer
     *   if there are no more network interfaces to enumerate.
     * </returns>
     */
    public YNetwork nextNetwork()
    {
      string hwid = "";
      if (YAPI.YISERR(_nextFunction(ref hwid)))
        return null;
      if (hwid == "")
        return null;
      return FindNetwork(hwid);
    }

    //--- (end of YNetwork implementation)

    //--- (Network functions)

    /**
     * <summary>
     *   Starts the enumeration of network interfaces currently accessible.
     * <para>
     *   Use the method <c>YNetwork.nextNetwork()</c> to iterate on
     *   next network interfaces.
     * </para>
     * </summary>
     * <returns>
     *   a pointer to a <c>YNetwork</c> object, corresponding to
     *   the first network interface currently online, or a <c>null</c> pointer
     *   if there are none.
     * </returns>
     */
    public static YNetwork FirstNetwork()
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
      err = YAPI.apiGetFunctionsByClass("Network", 0, p, size, ref neededsize, ref errmsg);
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
      return FindNetwork(serial + "." + funcId);
    }



    //--- (end of Network functions)
  }
}