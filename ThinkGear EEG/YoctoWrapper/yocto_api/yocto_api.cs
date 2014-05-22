/*********************************************************************
 *
 * $Id: yocto_api.cs 16091 2014-05-08 12:10:31Z seb $
 *
 * High-level programming interface, common to all modules
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

using System.Diagnostics;

using YHANDLE = System.Int32;
using YRETCODE = System.Int32;
using s8 = System.SByte;
using s16 = System.Int16;
using s32 = System.Int32;
using s64 = System.Int64;
using u8 = System.Byte;
using u16 = System.UInt16;
using u32 = System.UInt32;
using u64 = System.UInt64;


// yStrRef of serial number
using YDEV_DESCR = System.Int32;
// yStrRef of serial + (ystrRef of funcId << 16)
using YFUN_DESCR = System.Int32;
// measured in milliseconds
using yTime = System.UInt32;
using yHash = System.Int16;
// (yHash << 1) + [0,1]
using yBlkHdl = System.Char;
using yStrRef = System.Int16;
using yUrlRef = System.Int16;

using System.Runtime.InteropServices;
using System.Text;



namespace YocoWrapper
{
  public class YAPI
  {

    public enum TJSONRECORDTYPE
    {
      JSON_STRING,
      JSON_INTEGER,
      JSON_BOOLEAN,
      JSON_STRUCT,
      JSON_ARRAY
    }

    public struct TJSONRECORD
    {
      public string name;
      public TJSONRECORDTYPE recordtype;
      public string svalue;
      public long ivalue;
      public bool bvalue;
      public int membercount;
      public int memberAllocated;
      public TJSONRECORD[] members;
      public int itemcount;
      public int itemAllocated;
      public TJSONRECORD[] items;
    }

    public class TJsonParser
    {
      private enum Tjstate
      {
        JSTART,
        JWAITFORNAME,
        JWAITFORENDOFNAME,
        JWAITFORCOLON,
        JWAITFORDATA,
        JWAITFORNEXTSTRUCTMEMBER,
        JWAITFORNEXTARRAYITEM,
        JSCOMPLETED,
        JWAITFORSTRINGVALUE,
        JWAITFORINTVALUE,
        JWAITFORBOOLVALUE
      }

      private const int JSONGRANULARITY = 10;
      public int httpcode;

      private TJSONRECORD data;
      public TJsonParser(string jsonData)
        : this(jsonData, true)
      { }


      public TJsonParser(string jsonData, bool withHTTPHeader)
      {
        const string httpheader = "HTTP/1.1 ";
        const string okHeader = "OK\r\n";
        string errmsg = null;
        int p1 = 0;
        int p2 = 0;
        const string CR = "\r\n";
        int start_struct, start_array;

        if (withHTTPHeader)
        {
          if (jsonData.Substring(0, okHeader.Length) == okHeader)
          {
            httpcode = 200;

          }
          else
          {
            if (jsonData.Substring(0, httpheader.Length) != httpheader)
            {
              errmsg = "data should start with " + httpheader;
              throw new System.Exception(errmsg);
            }

            p1 = jsonData.IndexOf(" ", httpheader.Length - 1);
            p2 = jsonData.IndexOf(" ", p1 + 1);

            httpcode = Convert.ToInt32(jsonData.Substring(p1, p2 - p1 + 1));

            if (httpcode != 200)
              return;
          }
          p1 = jsonData.IndexOf(CR + CR + "{"); //json data is a structure
          if (p1 < 0) p1 = jsonData.IndexOf(CR + CR + "["); // json data is an array

          if (p1 < 0)
          {
            errmsg = "data  does not contain JSON data";
            throw new System.Exception(errmsg);
          }

          jsonData = jsonData.Substring(p1 + 4, jsonData.Length - p1 - 4);
        }
        else
        {
          start_struct = jsonData.IndexOf("{"); //json data is a structure
          start_array = jsonData.IndexOf("["); // json data is an array
          if ((start_struct < 0) && (start_array < 0))
          {
            errmsg = "data  does not contain JSON data";
            throw new System.Exception(errmsg);
          }
        }
        data = (TJSONRECORD)Parse(jsonData);
      }

      public string convertToString(Nullable<TJSONRECORD> p, bool showNamePrefix)
      {
        string buffer;

        if (p == null)
          p = data;

        if (p.Value.name != "" && showNamePrefix)
          buffer = '"' + p.Value.name + "\":";
        else
          buffer = "";
        switch (p.Value.recordtype)
        {
          case TJSONRECORDTYPE.JSON_STRING:
            buffer = buffer + '"' + p.Value.svalue + '"';
            break;
          case TJSONRECORDTYPE.JSON_INTEGER:
            buffer = buffer + p.Value.ivalue;
            break;
          case TJSONRECORDTYPE.JSON_BOOLEAN:
            buffer = buffer + (p.Value.bvalue ? "TRUE" : "FALSE");
            break;
          case TJSONRECORDTYPE.JSON_STRUCT:
            buffer = buffer + '{';
            for (int i = 0; i < p.Value.membercount; i++)
            {
              if (i > 0)
                buffer = buffer + ',';
              buffer = buffer + this.convertToString(p.Value.members[i], true);
            }
            buffer = buffer + '}';
            break;
          case TJSONRECORDTYPE.JSON_ARRAY:
            buffer = buffer + '[';
            for (int i = 0; i < p.Value.itemcount; i++)
            {
              if (i > 0)
                buffer = buffer + ',';
              buffer = buffer + this.convertToString(p.Value.items[i], false);
            }
            buffer = buffer + ']';
            break;
        }
        return buffer;
      }


      public void Dispose()
      {
        freestructure(ref data);
      }

      public TJSONRECORD GetRootNode()
      {
        return data;
      }

      private Nullable<TJSONRECORD> Parse(string st)
      {
        int i = 0;
        st = "\"root\" : " + st + " ";
        return ParseEx(Tjstate.JWAITFORNAME, "", ref st, ref i);
      }

      private void ParseError(ref string st, int i, string errmsg)
      {
        int ststart = 0;
        int stend = 0;
        ststart = i - 10;
        stend = i + 10;
        if (ststart < 0) ststart = 0;
        if (stend > st.Length) stend = st.Length - 1;
        errmsg = errmsg + " near " + st.Substring(ststart, i - ststart) + "*" + st.Substring(i, stend - i - 1);
        throw new System.Exception(errmsg);
      }

      private TJSONRECORD createStructRecord(string name)
      {
        TJSONRECORD res = default(TJSONRECORD);
        res.recordtype = TJSONRECORDTYPE.JSON_STRUCT;
        res.name = name;
        res.svalue = "";
        res.ivalue = 0;
        res.bvalue = false;
        res.membercount = 0;
        res.memberAllocated = JSONGRANULARITY;
        Array.Resize(ref res.members, res.memberAllocated);
        res.itemcount = 0;
        res.itemAllocated = 0;
        res.items = null;
        return res;
      }

      private TJSONRECORD createArrayRecord(string name)
      {
        TJSONRECORD res = default(TJSONRECORD);
        res.recordtype = TJSONRECORDTYPE.JSON_ARRAY;
        res.name = name;
        res.svalue = "";
        res.ivalue = 0;
        res.bvalue = false;
        res.itemcount = 0;
        res.itemAllocated = JSONGRANULARITY;
        Array.Resize(ref res.items, res.itemAllocated);
        res.membercount = 0;
        res.memberAllocated = 0;
        res.members = null;
        return res;
      }

      private TJSONRECORD createStrRecord(string name, string value)
      {
        TJSONRECORD res = default(TJSONRECORD);
        res.recordtype = TJSONRECORDTYPE.JSON_STRING;
        res.name = name;
        res.svalue = value;
        res.ivalue = 0;
        res.bvalue = false;
        res.itemcount = 0;
        res.itemAllocated = 0;
        res.items = null;
        res.membercount = 0;
        res.memberAllocated = 0;
        res.members = null;
        return res;
      }

      private TJSONRECORD createIntRecord(string name, long value)
      {
        TJSONRECORD res = default(TJSONRECORD);
        res.recordtype = TJSONRECORDTYPE.JSON_INTEGER;
        res.name = name;
        res.svalue = "";
        res.ivalue = value;
        res.bvalue = false;
        res.itemcount = 0;
        res.itemAllocated = 0;
        res.items = null;
        res.membercount = 0;
        res.memberAllocated = 0;
        res.members = null;
        return res;
      }

      private TJSONRECORD createBoolRecord(string name, bool value)
      {
        TJSONRECORD res = default(TJSONRECORD);
        res.recordtype = TJSONRECORDTYPE.JSON_BOOLEAN;
        res.name = name;
        res.svalue = "";
        res.ivalue = 0;
        res.bvalue = value;
        res.itemcount = 0;
        res.itemAllocated = 0;
        res.items = null;
        res.membercount = 0;
        res.memberAllocated = 0;
        res.members = null;
        return res;
      }

      private void add2StructRecord(ref TJSONRECORD container, ref TJSONRECORD element)
      {
        if (container.recordtype != TJSONRECORDTYPE.JSON_STRUCT)
          throw new System.Exception("container is not a struct type");
        if ((container.membercount >= container.memberAllocated))
        {
          Array.Resize(ref container.members, container.memberAllocated + JSONGRANULARITY);
          container.memberAllocated = container.memberAllocated + JSONGRANULARITY;
        }
        container.members[container.membercount] = element;
        container.membercount = container.membercount + 1;
      }

      private void add2ArrayRecord(ref TJSONRECORD container, ref TJSONRECORD element)
      {
        if (container.recordtype != TJSONRECORDTYPE.JSON_ARRAY)
          throw new System.Exception("container is not an array type");
        if ((container.itemcount >= container.itemAllocated))
        {
          Array.Resize(ref container.items, container.itemAllocated + JSONGRANULARITY);
          container.itemAllocated = container.itemAllocated + JSONGRANULARITY;
        }
        container.items[container.itemcount] = element;
        container.itemcount = container.itemcount + 1;
      }

      private char Skipgarbage(ref string st, ref int i)
      {
        char sti = st[i];
        while ((i < st.Length & (sti == '\n' | sti == '\r' | sti == ' ')))
        {
          i = i + 1;
          if (i < st.Length) sti = st[i];
        }
        return sti;
      }


      private Nullable<TJSONRECORD> ParseEx(Tjstate initialstate, string defaultname, ref string st, ref int i)
      {
        Nullable<TJSONRECORD> functionReturnValue = default(Nullable<TJSONRECORD>);
        TJSONRECORD res = default(TJSONRECORD);
        TJSONRECORD value = default(TJSONRECORD);
        Tjstate state = default(Tjstate);
        string svalue = "";
        long ivalue = 0;
        long isign = 0;
        char sti = '\0';

        string name = null;

        name = defaultname;
        state = initialstate;
        isign = 1;

        ivalue = 0;

        while (i < st.Length)
        {
          sti = st[i];
          switch (state)
          {
            case Tjstate.JWAITFORNAME:
              if (sti == '"')
              {
                state = Tjstate.JWAITFORENDOFNAME;
              }
              else
              {
                if (sti != ' ' & sti != '\n' & sti != ' ')
                  ParseError(ref st, i, "invalid char: was expecting \"");
              }

              break;
            case Tjstate.JWAITFORENDOFNAME:
              if (sti == '"')
              {
                state = Tjstate.JWAITFORCOLON;
              }
              else
              {
                if (sti >= 32)
                  name = name + sti;
                else
                  ParseError(ref st, i, "invalid char: was expecting an identifier compliant char");
              }

              break;
            case Tjstate.JWAITFORCOLON:
              if (sti == ':')
              {
                state = Tjstate.JWAITFORDATA;
              }
              else
              {
                if (sti != ' ' & sti != '\n' & sti != ' ')
                  ParseError(ref st, i, "invalid char: was expecting \"");
              }
              break;
            case Tjstate.JWAITFORDATA:
              if (sti == '{')
              {
                res = createStructRecord(name);
                state = Tjstate.JWAITFORNEXTSTRUCTMEMBER;
              }
              else if (sti == '[')
              {
                res = createArrayRecord(name);
                state = Tjstate.JWAITFORNEXTARRAYITEM;
              }
              else if (sti == '"')
              {
                svalue = "";
                state = Tjstate.JWAITFORSTRINGVALUE;
              }
              else if (sti >= '0' & sti <= '9')
              {
                state = Tjstate.JWAITFORINTVALUE;
                ivalue = sti - 48;
                isign = 1;
              }
              else if (sti == '-')
              {
                state = Tjstate.JWAITFORINTVALUE;
                ivalue = 0;
                isign = -1;
              }
              else if (sti == 't' || sti == 'f' || sti == 'T' || sti == 'F')
              {
                svalue = sti.ToString().ToUpper();
                state = Tjstate.JWAITFORBOOLVALUE;
              }
              else if (sti != ' ' & sti != '\n' & sti != ' ')
              {
                ParseError(ref st, i, "invalid char: was expecting  \",0..9,t or f");
              }
              break;
            case Tjstate.JWAITFORSTRINGVALUE:
              if (sti == '"')
              {
                state = Tjstate.JSCOMPLETED;
                res = createStrRecord(name, svalue);
              }
              else if (sti < 32)
              {
                ParseError(ref st, i, "invalid char: was expecting string value");
              }
              else
              {
                svalue = svalue + sti;
              }
              break;
            case Tjstate.JWAITFORINTVALUE:
              if (sti >= '0' & sti <= '9')
              {
                ivalue = (ivalue * 10) + sti - 48;
              }
              else
              {
                res = createIntRecord(name, isign * ivalue);
                state = Tjstate.JSCOMPLETED;
                i = i - 1;
              }
              break;
            case Tjstate.JWAITFORBOOLVALUE:
              if (sti < 'A' | sti > 'Z')
              {
                if (svalue != "TRUE" & svalue != "FALSE")
                  ParseError(ref st, i, "unexpected value, was expecting \"true\" or \"false\"");
                if (svalue == "TRUE")
                  res = createBoolRecord(name, true);
                else
                  res = createBoolRecord(name, false);
                state = Tjstate.JSCOMPLETED;
                i = i - 1;
              }
              else
              {
                svalue = svalue + sti.ToString().ToUpper();
              }
              break;
            case Tjstate.JWAITFORNEXTSTRUCTMEMBER:
              sti = Skipgarbage(ref st, ref i);
              if (i < st.Length)
              {
                if (sti == '}')
                {
                  functionReturnValue = res;
                  i = i + 1;
                  return functionReturnValue;
                }
                else
                {
                  value = (TJSONRECORD)ParseEx(Tjstate.JWAITFORNAME, "", ref st, ref i);
                  add2StructRecord(ref res, ref value);
                  sti = Skipgarbage(ref st, ref i);
                  if (i < st.Length)
                  {
                    if (sti == '}' & i < st.Length)
                    {
                      i = i - 1;
                    }
                    else if (sti != ' ' & sti != '\n' & sti != ' ' & sti != ',')
                    {
                      ParseError(ref st, i, "invalid char: vas expecting , or }");
                    }
                  }
                }

              }
              break;
            case Tjstate.JWAITFORNEXTARRAYITEM:
              sti = Skipgarbage(ref st, ref i);
              if (i < st.Length)
              {
                if (sti == ']')
                {
                  functionReturnValue = res;
                  i = i + 1;
                  return functionReturnValue;
                }
                else
                {
                  value = (TJSONRECORD)ParseEx(Tjstate.JWAITFORDATA, res.itemcount.ToString(), ref st, ref i);
                  add2ArrayRecord(ref res, ref value);
                  sti = Skipgarbage(ref st, ref i);
                  if (i < st.Length)
                  {
                    if (sti == ']' & i < st.Length)
                    {
                      i = i - 1;
                    }
                    else if (sti != ' ' & sti != '\n' & sti != ' ' & sti != ',')
                    {
                      ParseError(ref st, i, "invalid char: vas expecting , or ]");
                    }
                  }
                }
              }
              break;
            case Tjstate.JSCOMPLETED:
              functionReturnValue = res;
              return functionReturnValue;
          }
          i++;
        }
        ParseError(ref st, i, "unexpected end of data");
        functionReturnValue = null;
        return functionReturnValue;
      }

      private void DumpStructureRec(ref TJSONRECORD p, ref int deep)
      {
        string line = null;
        string indent = null;
        int i = 0;
        line = "";
        indent = "";
        for (i = 0; i <= deep * 2; i++)
        {
          indent = indent + " ";
        }
        line = indent + p.name + ":";
        switch (p.recordtype)
        {
          case TJSONRECORDTYPE.JSON_STRING:
            line = line + " str=" + p.svalue;
            Console.WriteLine(line);
            break;
          case TJSONRECORDTYPE.JSON_INTEGER:
            line = line + " int =" + p.ivalue.ToString();
            Console.WriteLine(line);
            break;
          case TJSONRECORDTYPE.JSON_BOOLEAN:
            if (p.bvalue)
              line = line + " bool = TRUE";
            else
              line = line + " bool = FALSE";
            Console.WriteLine(line);
            break;
          case TJSONRECORDTYPE.JSON_STRUCT:
            Console.WriteLine(line + " struct");
            for (i = 0; i <= p.membercount - 1; i++)
            {
              DumpStructureRec(ref p.members[i], ref deep);
            }

            break;
          case TJSONRECORDTYPE.JSON_ARRAY:
            Console.WriteLine(line + " array");
            for (i = 0; i <= p.itemcount - 1; i++)
            {
              DumpStructureRec(ref p.items[i], ref deep);
            }

            break;
        }
      }


      private void freestructure(ref TJSONRECORD p)
      {
        switch (p.recordtype)
        {
          case TJSONRECORDTYPE.JSON_STRUCT:
            for (int i = p.membercount - 1; i >= 0; i += -1)
            {
              freestructure(ref p.members[i]);
            }

            p.members = new TJSONRECORD[1];

            break;
          case TJSONRECORDTYPE.JSON_ARRAY:
            for (int i = p.itemcount - 1; i >= 0; i += -1)
            {
              freestructure(ref p.items[i]);
            }

            p.items = new TJSONRECORD[1];
            break;
        }
      }


      public void DumpStructure()
      {
        int i = 0;
        DumpStructureRec(ref data, ref i);
      }




      public Nullable<TJSONRECORD> GetChildNode(Nullable<TJSONRECORD> parent, string nodename)
      {
        Nullable<TJSONRECORD> functionReturnValue = default(Nullable<TJSONRECORD>);
        int i = 0;
        int index = 0;
        Nullable<TJSONRECORD> p = parent;

        if (p == null)
          p = data;

        if (p.Value.recordtype == TJSONRECORDTYPE.JSON_STRUCT)
        {
          for (i = 0; i <= p.Value.membercount - 1; i++)
          {
            if (p.Value.members[i].name == nodename)
            {
              functionReturnValue = p.Value.members[i];
              return functionReturnValue;
            }

          }
        }
        else if (p.Value.recordtype == TJSONRECORDTYPE.JSON_ARRAY)
        {
          index = Convert.ToInt32(nodename);
          if ((index >= p.Value.itemcount))
            throw new System.Exception("index out of bounds " + nodename + ">=" + p.Value.itemcount.ToString());
          functionReturnValue = p.Value.items[index];
          return functionReturnValue;
        }

        functionReturnValue = null;
        return functionReturnValue;
      }

      public List<string> GetAllChilds(Nullable<TJSONRECORD> parent)
      {
        List<string> res = new List<string>();
        Nullable<TJSONRECORD> p = parent;

        if (p == null) p = data;

        if (p.Value.recordtype == TJSONRECORDTYPE.JSON_STRUCT)
        {
          for (int i = 0; i < p.Value.membercount; i++)
            res.Add(this.convertToString(p.Value.members[i], false));
        }
        else if (p.Value.recordtype == TJSONRECORDTYPE.JSON_ARRAY)
        {
          for (int i = 0; i < p.Value.itemcount; i++)
            res.Add(this.convertToString(p.Value.items[i], false));
        }
        return res;
      }
    }

    public static Encoding DefaultEncoding = System.Text.Encoding.GetEncoding(1252);

    // Switch to turn off exceptions and use return codes instead, for source-code compatibility
    // with languages without exception support like C
    public static bool ExceptionsDisabled = false;

    static bool _apiInitialized = false;
    // Default cache validity (in [ms]) before reloading data from device. This saves a lots of trafic.
    // Note that a value undger 2 ms makes little sense since a USB bus itself has a 2ms roundtrip period

    public const int DefaultCacheValidity = 5;
    public const string INVALID_STRING = "!INVALID!";
    public const double INVALID_DOUBLE = -1.79769313486231E+308;
    public const int INVALID_INT = -2147483648;
    public const int INVALID_UINT = -1;
    public const long INVALID_LONG = -9223372036854775807L;
    public const string HARDWAREID_INVALID = INVALID_STRING;
    public const string FUNCTIONID_INVALID = INVALID_STRING;
    public const string FRIENDLYNAME_INVALID = INVALID_STRING;

    public const int INVALID_UNSIGNED = -1;
    // yInitAPI argument
    public const int Y_DETECT_NONE = 0;
    public const int Y_DETECT_USB = 1;
    public const int Y_DETECT_NET = 2;

    public const int Y_DETECT_ALL = Y_DETECT_USB | Y_DETECT_NET;

    public const string YOCTO_API_VERSION_STR = "1.10";
    public const int YOCTO_API_VERSION_BCD = 0x0110;

    public const string YOCTO_API_BUILD_NO = "16182";
    public const int YOCTO_DEFAULT_PORT = 4444;
    public const int YOCTO_VENDORID = 0x24e0;
    public const int YOCTO_DEVID_FACTORYBOOT = 1;

    public const int YOCTO_DEVID_BOOTLOADER = 2;
    public const int YOCTO_ERRMSG_LEN = 256;
    public const int YOCTO_MANUFACTURER_LEN = 20;
    public const int YOCTO_SERIAL_LEN = 20;
    public const int YOCTO_BASE_SERIAL_LEN = 8;
    public const int YOCTO_PRODUCTNAME_LEN = 28;
    public const int YOCTO_FIRMWARE_LEN = 22;
    public const int YOCTO_LOGICAL_LEN = 20;
    public const int YOCTO_FUNCTION_LEN = 20;
    // Size of the data (can be non null terminated)
    public const int YOCTO_PUBVAL_SIZE = 6;
    // Temporary storage, > YOCTO_PUBVAL_SIZE
    public const int YOCTO_PUBVAL_LEN = 16;
    public const int YOCTO_PASS_LEN = 20;
    public const int YOCTO_REALM_LEN = 20;
    public const int INVALID_YHANDLE = 0;

    public const int yUnknowSize = 1024;
    [StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Ansi)]
    public struct yDeviceSt
    {
      public u16 vendorid;
      public u16 deviceid;
      public u16 devrelease;
      public u16 nbinbterfaces;
      [MarshalAs(UnmanagedType.ByValTStr, SizeConst = YAPI.YOCTO_MANUFACTURER_LEN)]
      public string manufacturer;
      [MarshalAs(UnmanagedType.ByValTStr, SizeConst = YAPI.YOCTO_PRODUCTNAME_LEN)]
      public string productname;
      [MarshalAs(UnmanagedType.ByValTStr, SizeConst = YAPI.YOCTO_SERIAL_LEN)]
      public string serial;
      [MarshalAs(UnmanagedType.ByValTStr, SizeConst = YAPI.YOCTO_LOGICAL_LEN)]
      public string logicalname;
      [MarshalAs(UnmanagedType.ByValTStr, SizeConst = YAPI.YOCTO_FIRMWARE_LEN)]
      public string firmware;
      public u8 beacon;
    }

    public const int YIOHDL_SIZE = 8;
    [StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Ansi)]
    public struct YIOHDL
    {
      [MarshalAs(UnmanagedType.U1, SizeConst = YAPI.YIOHDL_SIZE)]
      public u8 raw0;
      public u8 raw1;
      public u8 raw2;
      public u8 raw3;
      public u8 raw4;
      public u8 raw5;
      public u8 raw6;
      public u8 raw7;
    }

    public enum yDEVICE_PROP
    {
      PROP_VENDORID,
      PROP_DEVICEID,
      PROP_DEVRELEASE,
      PROP_FIRMWARELEVEL,
      PROP_MANUFACTURER,
      PROP_PRODUCTNAME,
      PROP_SERIAL,
      PROP_LOGICALNAME,
      PROP_URL
    }



    public enum yFACE_STATUS
    {
      YFACE_EMPTY,
      YFACE_RUNNING,
      YFACE_ERROR
    }

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int yFlashCallback(u32 stepnumber, u32 totalStep, IntPtr context);

    [StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Ansi)]
    public struct yFlashArg
    {
      // device windows name on os (used to acces device)
      public StringBuilder OSDeviceName;
      // serial number of the device
      public StringBuilder serial2assign;
      // pointer to the content of the Hex file
      public IntPtr firmwarePtr;
      // len of the Hexfile
      public u32 firmwareLen;
      public yFlashCallback progress;
      public IntPtr context;
    }




    // --- (generated code: YFunction return codes)
    // Yoctopuce error codes, used by default as function return value
    public const int SUCCESS = 0;                   // everything worked allright
    public const int NOT_INITIALIZED = -1;          // call yInitAPI() first !
    public const int INVALID_ARGUMENT = -2;         // one of the arguments passed to the function is invalid
    public const int NOT_SUPPORTED = -3;            // the operation attempted is (currently) not supported
    public const int DEVICE_NOT_FOUND = -4;         // the requested device is not reachable
    public const int VERSION_MISMATCH = -5;         // the device firmware is incompatible with this API version
    public const int DEVICE_BUSY = -6;              // the device is busy with another task and cannot answer
    public const int TIMEOUT = -7;                  // the device took too long to provide an answer
    public const int IO_ERROR = -8;                 // there was an I/O problem while talking to the device
    public const int NO_MORE_DATA = -9;             // there is no more data to read from
    public const int EXHAUSTED = -10;               // you have run out of a limited ressource, check the documentation
    public const int DOUBLE_ACCES = -11;            // you have two process that try to acces to the same device
    public const int UNAUTHORIZED = -12;            // unauthorized access to password-protected device
    public const int RTC_NOT_READY = -13;           // real-time clock has not been initialized (or time was lost)
    //--- (end of generated code: YFunction return codes)  

    public class YAPI_Exception : ApplicationException
    {
      public YRETCODE errorType;
      public YAPI_Exception(YRETCODE errType, string errMsg)
      {
      }
      // New
    }


    static List<YDevice> YDevice_devCache;


    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void HTTPRequestCallback(YDevice device, ref blockingCallbackCtx context, YRETCODE returnval, string result, string errmsg);

    // - Types used for public yocto_api callbacks
    public delegate void yLogFunc(string log);
    public delegate void yDeviceUpdateFunc(YModule modul);
    public delegate double yCalibrationHandler(double rawValue, int calibType, List<int> parameters, List<double> rawValues, List<double> refValues);
    public delegate void YHubDiscoveryCallback(String serial, String url);

    // - Types used for internal yapi callbacks
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void _yapiLogFunc(IntPtr log, u32 loglen);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void _yapiDeviceUpdateFunc(YDEV_DESCR dev);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void _yapiFunctionUpdateFunc(YFUN_DESCR dev, IntPtr value);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void _yapiTimedReportFunc(YFUN_DESCR dev, double timestamp, IntPtr data, u32 len);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void _yapiHubDiscoveryCallback(IntPtr serial, IntPtr url);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void _yapiDeviceLogCallback(YFUN_DESCR dev, IntPtr data);

    // - Variables used to store public yocto_api callbacks
    private static yLogFunc ylog = null;
    private static yDeviceUpdateFunc yArrival = null;
    private static yDeviceUpdateFunc yRemoval = null;
    private static yDeviceUpdateFunc yChange = null;
    private static YHubDiscoveryCallback _HubDiscoveryCallback = null;

    public static bool YISERR(YRETCODE retcode)
    {
      if (retcode < 0)
        return true;
      return false;
    }

    public class blockingCallbackCtx
    {
      public YRETCODE res;
      public string response;
      public string errmsg;
    }

    public static void YblockingCallback(YDevice device, ref blockingCallbackCtx context, YRETCODE returnval, string result, string errmsg)
    {
      context.res = returnval;
      context.response = result;
      context.errmsg = errmsg;
    }

    public class YDevice
    {
      private YDEV_DESCR _devdescr;
      private ulong _cacheStamp;
      private TJsonParser _cacheJson;
      private List<u32> _functions = new List<u32>();

      private string _rootdevice;
      private string _subpath;

      private bool _subpathinit;
      public YDevice(YDEV_DESCR devdesc)
      {
        _devdescr = devdesc;
        _cacheStamp = 0;
        _cacheJson = null;
      }


      public void dispose()
      {
        if (_cacheJson != null)
          _cacheJson.Dispose();
        _cacheJson = null;

      }

      public static YDevice getDevice(YDEV_DESCR devdescr)
      {
        int idx = 0;
        YDevice dev = null;
        for (idx = 0; idx <= YAPI.YDevice_devCache.Count - 1; idx++)
        {
          if (YAPI.YDevice_devCache[idx]._devdescr == devdescr)
          {
            return YAPI.YDevice_devCache[idx];
          }
        }
        dev = new YDevice(devdescr);
        YAPI.YDevice_devCache.Add(dev);
        return dev;
      }

      public YRETCODE HTTPRequestSync(string device, string request, ref string reply, ref string errmsg)
      {
        byte[] binreply = new byte[0];
        YRETCODE res;

        res = this.HTTPRequestSync(device, YAPI.DefaultEncoding.GetBytes(request), ref binreply, ref errmsg);
        reply = YAPI.DefaultEncoding.GetString(binreply);
        return res;
      }

      public YRETCODE HTTPRequestSync(string device, byte[] request, ref byte[] reply, ref string errmsg)
      {
        YIOHDL iohdl;
        IntPtr requestbuf = IntPtr.Zero;
        StringBuilder buffer = new StringBuilder(YAPI.YOCTO_ERRMSG_LEN);
        IntPtr preply = default(IntPtr);
        int replysize = 0;
        YRETCODE res;

        iohdl.raw0 = 0; // dummy, useless init to avoid compiler warning
        iohdl.raw1 = 0;
        iohdl.raw2 = 0;
        iohdl.raw3 = 0;
        iohdl.raw4 = 0;
        iohdl.raw5 = 0;
        iohdl.raw6 = 0;
        iohdl.raw7 = 0;

        requestbuf = Marshal.AllocHGlobal(request.Length);
        Marshal.Copy(request, 0, requestbuf, request.Length);
        res = _yapiHTTPRequestSyncStartEx(ref iohdl, new StringBuilder(device), requestbuf, request.Length, ref preply, ref replysize, buffer);
        Marshal.FreeHGlobal(requestbuf);
        if (res < 0)
        {
          errmsg = buffer.ToString();
          return res;
        }
        reply = new byte[replysize];
        Marshal.Copy(preply, reply, 0, replysize);
        res = _yapiHTTPRequestSyncDone(ref iohdl, buffer);
        errmsg = buffer.ToString();
        return res;
      }

      public YRETCODE HTTPRequestAsync(string request, ref string errmsg)
      {
        return this.HTTPRequestAsync(YAPI.DefaultEncoding.GetBytes(request), ref errmsg);
      }

      public YRETCODE HTTPRequestAsync(byte[] request, ref string errmsg)
      {
        byte[] fullrequest = null;
        IntPtr requestbuf = IntPtr.Zero;
        StringBuilder buffer = new StringBuilder(YAPI.YOCTO_ERRMSG_LEN);
        YRETCODE res = HTTPRequestPrepare(request, ref fullrequest, ref errmsg);

        requestbuf = Marshal.AllocHGlobal(fullrequest.Length);
        Marshal.Copy(fullrequest, 0, requestbuf, fullrequest.Length);
        res = _yapiHTTPRequestAsyncEx(new StringBuilder(_rootdevice), requestbuf, fullrequest.Length, default(IntPtr), default(IntPtr), buffer);
        Marshal.FreeHGlobal(requestbuf);
        errmsg = buffer.ToString();
        return res;
      }

      public YRETCODE HTTPRequestPrepare(byte[] request, ref byte[] fullrequest, ref string errmsg)
      {
        YRETCODE res = default(YRETCODE);
        StringBuilder errbuf = new StringBuilder(YAPI.YOCTO_ERRMSG_LEN);
        StringBuilder b = null;
        int neededsize = 0;
        int p = 0;
        StringBuilder root = new StringBuilder(YAPI.YOCTO_SERIAL_LEN);
        int tmp = 0;

        _cacheStamp = YAPI.GetTickCount();
        // invalidate cache

        if (!(_subpathinit))
        {
          res = YAPI._yapiGetDevicePath(_devdescr, root, null, 0, ref neededsize, errbuf);

          if (YAPI.YISERR(res))
          {
            errmsg = errbuf.ToString();
            return res;
          }

          b = new StringBuilder(neededsize);
          res = YAPI._yapiGetDevicePath(_devdescr, root, b, neededsize, ref tmp, errbuf);
          if (YAPI.YISERR(res))
          {
            errmsg = errbuf.ToString();
            return res;
          }

          _rootdevice = root.ToString();
          _subpath = b.ToString();
          _subpathinit = true;
        }
        // search for the first '/'
        p = 0;
        while (p < request.Length && request[p] != 47) p++;
        fullrequest = new byte[request.Length - 1 + _subpath.Length];
        Buffer.BlockCopy(request, 0, fullrequest, 0, p);
        Buffer.BlockCopy(System.Text.Encoding.ASCII.GetBytes(_subpath), 0, fullrequest, p, _subpath.Length);
        Buffer.BlockCopy(request, p + 1, fullrequest, p + _subpath.Length, request.Length - p - 1);

        return YAPI.SUCCESS;
      }


      public YRETCODE HTTPRequest(string request, ref string buffer, ref string errmsg)
      {
        byte[] binreply = new byte[0];
        YRETCODE res;

        res = this.HTTPRequest(YAPI.DefaultEncoding.GetBytes(request), ref binreply, ref errmsg);
        buffer = YAPI.DefaultEncoding.GetString(binreply);

        return res;
      }

      public YRETCODE HTTPRequest(string request, ref byte[] buffer, ref string errmsg)
      {
        return this.HTTPRequest(YAPI.DefaultEncoding.GetBytes(request), ref buffer, ref errmsg);
      }

      public YRETCODE HTTPRequest(byte[] request, ref byte[] buffer, ref string errmsg)
      {
        byte[] fullrequest = null;

        int res = HTTPRequestPrepare(request, ref fullrequest, ref errmsg);
        if (YAPI.YISERR(res)) return res;

        return HTTPRequestSync(_rootdevice, fullrequest, ref buffer, ref errmsg);
      }

      public YRETCODE requestAPI(out TJsonParser apires, ref string errmsg)
      {
        string buffer = "";
        int res = 0;

        apires = null;
        // Check if we have a valid cache value
        if (_cacheStamp > YAPI.GetTickCount())
        {
          apires = _cacheJson;
          return YAPI.SUCCESS;
        }
        res = HTTPRequest("GET /api.json \r\n\r\n", ref buffer, ref errmsg);
        if (YAPI.YISERR(res))
        {
          // make sure a device scan does not solve the issue
          res = YAPI.yapiUpdateDeviceList(1, ref errmsg);
          if (YAPI.YISERR(res))
          {
            return res;
          }
          res = HTTPRequest("GET /api.json \r\n\r\n", ref buffer, ref errmsg);
          if (YAPI.YISERR(res))
          {
            return res;
          }
        }

        try
        {
          apires = new TJsonParser(buffer);
        }
        catch (Exception E)
        {
          errmsg = "unexpected JSON structure: " + E.Message;
          return YAPI.IO_ERROR;
        }

        // store result in cache
        _cacheJson = apires;
        _cacheStamp = YAPI.GetTickCount() + YAPI.DefaultCacheValidity;

        return YAPI.SUCCESS;
      }

      public YRETCODE getFunctions(ref List<u32> functions, ref string errmsg)
      {
        int res = 0;
        int neededsize = 0;
        int i = 0;
        int count = 0;
        IntPtr p = default(IntPtr);
        s32[] ids = null;
        if (_functions.Count == 0)
        {
          res = YAPI.apiGetFunctionsByDevice(_devdescr, 0, IntPtr.Zero, 64, ref neededsize, ref errmsg);
          if (YAPI.YISERR(res))
          {
            return res;
          }

          p = Marshal.AllocHGlobal(neededsize);

          res = YAPI.apiGetFunctionsByDevice(_devdescr, 0, p, 64, ref neededsize, ref errmsg);
          if (YAPI.YISERR(res))
          {
            Marshal.FreeHGlobal(p);
            return res;
          }

          count = Convert.ToInt32(neededsize / Marshal.SizeOf(i));
          //  i is an 32 bits integer 
          Array.Resize(ref ids, count + 1);
          Marshal.Copy(p, ids, 0, count);
          for (i = 0; i <= count - 1; i++)
          {
            _functions.Add(Convert.ToUInt32(ids[i]));
          }

          Marshal.FreeHGlobal(p);
        }
        functions = _functions;
        return YAPI.SUCCESS;
      }

    }



    /**
     * <summary>
     *   Disables the use of exceptions to report runtime errors.
     * <para>
     *   When exceptions are disabled, every function returns a specific
     *   error value which depends on its type and which is documented in
     *   this reference manual.
     * </para>
     * </summary>
     */
    public static void DisableExceptions()
    {
      ExceptionsDisabled = true;
    }
    /**
     * <summary>
     *   Re-enables the use of exceptions for runtime error handling.
     * <para>
     *   Be aware than when exceptions are enabled, every function that fails
     *   triggers an exception. If the exception is not caught by the user code,
     *   it  either fires the debugger or aborts (i.e. crash) the program.
     *   On failure, throws an exception or returns a negative error code.
     * </para>
     * </summary>
     */
    public static void EnableExceptions()
    {
      ExceptionsDisabled = false;
    }

    // - Internal callback registered into YAPI using a protected delegate
    private static void native_yLogFunction(IntPtr log, u32 loglen)
    {
      if (ylog != null)
        ylog(Marshal.PtrToStringAnsi(log));
    }


    private static void native_yDeviceLogCallback(YFUN_DESCR devdescr, IntPtr data)
    {
      yDeviceSt infos = emptyDeviceSt();
      YModule modul;
      String errmsg = "";
      YModule.LogCallback callback;

      if (yapiGetDeviceInfo(devdescr, ref infos, ref errmsg) != YAPI.SUCCESS)
      {
        return;
      }
      modul = YModule.FindModule(infos.serial + ".module");
      callback = modul.get_logCallback();
      if (callback != null)
      {
        callback(modul, Marshal.PtrToStringAnsi(data));
      }
    }


    /**
     * <summary>
     *   Registers a log callback function.
     * <para>
     *   This callback will be called each time
     *   the API have something to say. Quite useful to debug the API.
     * </para>
     * </summary>
     * <param name="logfun">
     *   a procedure taking a string parameter, or <c>null</c>
     *   to unregister a previously registered  callback.
     * </param>
     */
    public static void RegisterLogFunction(yLogFunc logfun)
    {
      ylog = logfun;

    }



    private class DataEvent
    {

      private YFunction _fun;
      private YSensor _sensor;
      private String _value;
      private List<int> _report;
      private double _timestamp;

      public DataEvent(YFunction fun, String value)
      {
        _fun = fun;
        _sensor = null;
        _value = value;
        _report = null;
        _timestamp = 0;
      }

      public DataEvent(YSensor sensor, double timestamp, List<int> report)
      {
        _fun = null;
        _sensor = sensor;
        _value = null;
        _timestamp = timestamp;
        _report = report;
      }

      public void invoke()
      {
        if (_fun == null)
        {
          YMeasure mesure = _sensor._decodeTimedReport(_timestamp, _report);
          _sensor._invokeTimedReportCallback(mesure);
        }
        else
        {
          // new value
          _fun._invokeValueCallback(_value);
        }
      }

    }

    private class PlugEvent
    {

      public enum EVTYPE
      {
        ARRIVAL,
        REMOVAL,
        CHANGE,
        HUB_DISCOVERY
      }

      private EVTYPE _eventtype;
      private YModule _module;
      private String _serial;
      private String _url;

      public PlugEvent(EVTYPE type, YModule mod)
      {
        _eventtype = type;
        _module = mod;
      }

      public PlugEvent(String serial, String url)
      {
        _eventtype = EVTYPE.HUB_DISCOVERY;
        _serial = serial;
        _url = url;
      }

      public void invoke()
      {
        switch (_eventtype)
        {
          case EVTYPE.ARRIVAL:
            if (yArrival != null)
              yArrival(_module);
            break;
          case EVTYPE.REMOVAL:
            if (yRemoval != null)
              yRemoval(_module);

            break;
          case EVTYPE.CHANGE:
            if (yChange != null)
              yChange(_module);
            break;
          case EVTYPE.HUB_DISCOVERY:
            if (_HubDiscoveryCallback != null)
              _HubDiscoveryCallback(_serial, _url);
            break;
        }
      }
    }

    private static yDeviceSt emptyDeviceSt()
    {
      yDeviceSt infos = default(yDeviceSt);
      infos.vendorid = 0;
      infos.deviceid = 0;
      infos.devrelease = 0;
      infos.nbinbterfaces = 0;
      infos.manufacturer = "";
      infos.productname = "";
      infos.serial = "";
      infos.logicalname = "";
      infos.firmware = "";
      infos.beacon = 0;
      return infos;
    }

    static List<PlugEvent> _PlugEvents;
    static List<DataEvent> _DataEvents;

    private static void native_HubDiscoveryCallback(IntPtr serial_ptr, IntPtr url_ptr)
    {
      String serial = Marshal.PtrToStringAnsi(serial_ptr);
      String url = Marshal.PtrToStringAnsi(url_ptr);
      PlugEvent ev = new PlugEvent(serial, url);
      _PlugEvents.Add(ev);
    }

    /**
     * <summary>
     *   Register a callback function, to be called each time an Network Hub send
     *   an SSDP message.
     * <para>
     *   The callback has two string parameter, the first one
     *   contain the serial number of the hub and the second contain the URL of the
     *   network hub (this URL can be passed to RegisterHub). This callback will be invoked
     *   while yUpdateDeviceList is running. You will have to call this function on a regular basis.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="hubDiscoveryCallback">
     *   a procedure taking two string parameter, or null
     *   to unregister a previously registered  callback.
     * </param>
     */
    public static void RegisterHubDiscoveryCallback(YHubDiscoveryCallback hubDiscoveryCallback)
    {
      String errmsg = "";
      _HubDiscoveryCallback = hubDiscoveryCallback;
      TriggerHubDiscovery(ref errmsg);
    }

    private static void native_yDeviceArrivalCallback(YDEV_DESCR d)
    {
      yDeviceSt infos = emptyDeviceSt();
      PlugEvent ev;
      string errmsg = "";

      if (yapiGetDeviceInfo(d, ref infos, ref errmsg) != SUCCESS)
      {
        return;
      }
      YModule modul = YModule.FindModule(infos.serial + ".module");
      modul.setImmutableAttributes(ref infos);
      ev = new PlugEvent(PlugEvent.EVTYPE.ARRIVAL, modul);
      if (yArrival != null)
        _PlugEvents.Add(ev);
    }

    /**
     * <summary>
     *   Register a callback function, to be called each time
     *   a device is plugged.
     * <para>
     *   This callback will be invoked while <c>yUpdateDeviceList</c>
     *   is running. You will have to call this function on a regular basis.
     * </para>
     * </summary>
     * <param name="arrivalCallback">
     *   a procedure taking a <c>YModule</c> parameter, or <c>null</c>
     *   to unregister a previously registered  callback.
     * </param>
     */
    public static void RegisterDeviceArrivalCallback(yDeviceUpdateFunc arrivalCallback)
    {
      yArrival = arrivalCallback;
      if (arrivalCallback != null)
      {
        string error = "";
        YModule mod = YModule.FirstModule();
        while (mod != null)
        {
          if (mod.isOnline())
          {
            yapiLockDeviceCallBack(ref error);
            native_yDeviceArrivalCallback(mod.functionDescriptor());
            yapiUnlockDeviceCallBack(ref error);
          }
          mod = mod.nextModule();
        }
      }
    }

    private static void native_yDeviceRemovalCallback(YDEV_DESCR d)
    {
      PlugEvent ev;
      yDeviceSt infos = emptyDeviceSt();
      string errmsg = "";
      if (yRemoval == null)
        return;
      infos.deviceid = 0;
      if (yapiGetDeviceInfo(d, ref infos, ref errmsg) != SUCCESS)
        return;
      YModule modul = YModule.FindModule(infos.serial + ".module");
      ev = new PlugEvent(PlugEvent.EVTYPE.REMOVAL, modul);
      _PlugEvents.Add(ev);
    }

    /**
     * <summary>
     *   Register a callback function, to be called each time
     *   a device is unplugged.
     * <para>
     *   This callback will be invoked while <c>yUpdateDeviceList</c>
     *   is running. You will have to call this function on a regular basis.
     * </para>
     * </summary>
     * <param name="removalCallback">
     *   a procedure taking a <c>YModule</c> parameter, or <c>null</c>
     *   to unregister a previously registered  callback.
     * </param>
     */
    public static void RegisterDeviceRemovalCallback(yDeviceUpdateFunc removalCallback)
    {
      yRemoval = removalCallback;
    }

    public static void native_yDeviceChangeCallback(YDEV_DESCR d)
    {
      PlugEvent ev;
      yDeviceSt infos = emptyDeviceSt();
      string errmsg = "";

      if (yChange == null)
        return;
      if (yapiGetDeviceInfo(d, ref infos, ref errmsg) != SUCCESS)
        return;
      YModule modul = YModule.FindModule(infos.serial + ".module");
      ev = new PlugEvent(PlugEvent.EVTYPE.CHANGE, modul);
      _PlugEvents.Add(ev);
    }

    public static void RegisterDeviceChangeCallback(yDeviceUpdateFunc callback)
    {
      yChange = callback;
    }

    private static void queuesCleanUp()
    {
      _PlugEvents.Clear();
      _PlugEvents = null;
      _DataEvents.Clear();
      _DataEvents = null;
    }

    private static void native_yFunctionUpdateCallback(YFUN_DESCR fundesc, IntPtr data)
    {

      if (!IntPtr.Zero.Equals(data))
      {
        for (int i = 0; i < YFunction._ValueCallbackList.Count; i++)
        {
          if (YFunction._ValueCallbackList[i].get_functionDescriptor() == fundesc)
          {
            DataEvent ev = new DataEvent(YFunction._ValueCallbackList[i], Marshal.PtrToStringAnsi(data));
            _DataEvents.Add(ev);
          }
        }
      }
    }

    private static void native_yTimedReportCallback(YFUN_DESCR fundesc, double timestamp, IntPtr rawdata, u32 len)
    {
      for (int i = 0; i < YFunction._TimedReportCallbackList.Count; i++)
      {
        if (YFunction._TimedReportCallbackList[i].get_functionDescriptor() == fundesc)
        {
          byte[] data = new byte[len];
          Marshal.Copy(rawdata, data, 0, (int)len);
          List<int> report = new List<int>((int)len);
          int p = 0;
          while (p < len)
          {
            report.Add(data[p++] & 0xff);
          }
          DataEvent ev = new DataEvent(YFunction._TimedReportCallbackList[i], timestamp, report);
          _DataEvents.Add(ev);
        }
      }

    }


    public static void RegisterCalibrationHandler(int calibType, YAPI.yCalibrationHandler callback)
    {
      string key;
      key = calibType.ToString();
      YFunction._CalibHandlers.Add(key, callback);
    }

    private static double yLinearCalibrationHandler(double rawValue, int calibType, List<int> parameters, List<double> rawValues, List<double> refValues)
    {
      int npt;
      double x, adj;
      double x2, adj2;
      int i;

      npt = calibType % 10;
      x = rawValues[0];
      adj = refValues[0] - x;
      i = 0;

      if (npt > rawValues.Count) npt = rawValues.Count;
      if (npt > refValues.Count) npt = refValues.Count + 1;
      while ((rawValue > rawValues[i]) && (i + 1 < npt))
      {
        i++;
        x2 = x;
        adj2 = adj;
        x = rawValues[i];
        adj = refValues[i] - x;
        if ((rawValue < x) && (x > x2))
        {
          adj = adj2 + (adj - adj2) * (rawValue - x2) / (x - x2);
        }
      }
      return rawValue + adj;
    }



    private static int yapiLockDeviceCallBack(ref string errmsg)
    {
      int functionReturnValue = 0;
      StringBuilder buffer = new StringBuilder(YOCTO_ERRMSG_LEN);
      buffer.Length = 0;
      functionReturnValue = _yapiLockDeviceCallBack(buffer);
      errmsg = buffer.ToString();
      return functionReturnValue;
    }

    private static int yapiUnlockDeviceCallBack(ref string errmsg)
    {
      int functionReturnValue = 0;
      StringBuilder buffer = new StringBuilder(YOCTO_ERRMSG_LEN);
      buffer.Length = 0;
      functionReturnValue = _yapiUnlockDeviceCallBack(buffer);
      errmsg = buffer.ToString();
      return functionReturnValue;
    }

    private static int yapiLockFunctionCallBack(ref string errmsg)
    {
      int functionReturnValue = 0;
      StringBuilder buffer = new StringBuilder(YOCTO_ERRMSG_LEN);
      buffer.Length = 0;
      functionReturnValue = _yapiLockFunctionCallBack(buffer);
      errmsg = buffer.ToString();
      return functionReturnValue;
    }

    private static int yapiUnlockFunctionCallBack(ref string errmsg)
    {
      int functionReturnValue = 0;
      StringBuilder buffer = new StringBuilder(YOCTO_ERRMSG_LEN);
      buffer.Length = 0;
      functionReturnValue = _yapiUnlockFunctionCallBack(buffer);
      errmsg = buffer.ToString();
      return functionReturnValue;
    }

    public static yCalibrationHandler _getCalibrationHandler(int calType)
    {
      string key;

      key = calType.ToString();
      if (YFunction._CalibHandlers.ContainsKey(key))
        return YFunction._CalibHandlers[key];
      return null;
    }

    private static double[] decExp = new double[] {
    1.0e-6, 1.0e-5, 1.0e-4, 1.0e-3, 1.0e-2, 1.0e-1, 1.0,
    1.0e1, 1.0e2, 1.0e3, 1.0e4, 1.0e5, 1.0e6, 1.0e7, 1.0e8, 1.0e9 };

    // Convert Yoctopuce 16-bit decimal floats to standard double-precision floats
    //
    public static double _decimalToDouble(int val)
    {
      bool negate = false;
      double res;

      if (val == 0)
        return 0.0;
      if (val > 32767)
      {
        negate = true;
        val = 65536 - val;
      }
      else if (val < 0)
      {
        negate = true;
        val = -val;
      }
      int exp = val >> 11;
      res = (double)(val & 2047) * decExp[exp];
      return (negate ? -res : res);
    }

    // Convert standard double-precision floats to Yoctopuce 16-bit decimal floats
    //
    public static long _doubleToDecimal(double val)
    {
      int negate = 0;
      double comp, mant;
      int decpow;
      long res;

      if (val == 0.0)
      {
        return 0;
      }
      if (val < 0)
      {
        negate = 1;
        val = -val;
      }
      comp = val / 1999.0;
      decpow = 0;
      while (comp > decExp[decpow] && decpow < 15)
      {
        decpow++;
      }
      mant = val / decExp[decpow];
      if (decpow == 15 && mant > 2047.0)
      {
        res = (15 << 11) + 2047; // overflow
      }
      else
      {
        res = (decpow << 11) + Convert.ToInt32(mant);
      }
      return (negate != 0 ? -res : res);
    }



    public static List<int> _decodeWords(string sdat)
    {
      List<int> udat = new List<int>();

      for (int p = 0; p < sdat.Length; )
      {
        uint val;
        uint c = sdat[p++];
        if (c == '*')
        {
          val = 0;
        }
        else if (c == 'X')
        {
          val = 0xffff;
        }
        else if (c == 'Y')
        {
          val = 0x7fff;
        }
        else if (c >= 'a')
        {
          int srcpos = (int)(udat.Count - 1 - (c - 'a'));
          if (srcpos < 0)
          {
            val = 0;
          }
          else
          {
            val = (uint)udat[srcpos];
          }
        }
        else
        {
          if (p + 2 > sdat.Length)
          {
            return udat;
          }
          val = (c - '0');
          c = sdat[p++];
          val += (c - '0') << 5;
          c = sdat[p++];
          if (c == 'z') c = '\\';
          val += (c - '0') << 10;
        }
        udat.Add((int)val);
      }
      return udat;
    }

    // - Delegate object for our internal callback, protected from GC
    public static _yapiLogFunc native_yLogFunctionDelegate = native_yLogFunction;
    static GCHandle native_yLogFunctionAnchor = GCHandle.Alloc(native_yLogFunctionDelegate);

    public static _yapiFunctionUpdateFunc native_yFunctionUpdateDelegate = native_yFunctionUpdateCallback;
    static GCHandle native_yFunctionUpdateAnchor = GCHandle.Alloc(native_yFunctionUpdateDelegate);

    public static _yapiTimedReportFunc native_yTimedReportDelegate = native_yTimedReportCallback;
    static GCHandle native_yTimedReportAnchor = GCHandle.Alloc(native_yTimedReportDelegate);

    public static _yapiHubDiscoveryCallback native_yapiHubDiscoveryDelegate = native_HubDiscoveryCallback;
    static GCHandle native_yapiHubDiscoveryAnchor = GCHandle.Alloc(native_yapiHubDiscoveryDelegate);

    public static _yapiDeviceUpdateFunc native_yDeviceArrivalDelegate = native_yDeviceArrivalCallback;
    static GCHandle native_yDeviceArrivalAnchor = GCHandle.Alloc(native_yDeviceArrivalDelegate);

    public static _yapiDeviceUpdateFunc native_yDeviceRemovalDelegate = native_yDeviceRemovalCallback;
    static GCHandle native_yDeviceRemovalAnchor = GCHandle.Alloc(native_yDeviceRemovalDelegate);

    public static _yapiDeviceUpdateFunc native_yDeviceChangeDelegate = native_yDeviceChangeCallback;
    static GCHandle native_yDeviceChangeAnchor = GCHandle.Alloc(native_yDeviceChangeDelegate);

    public static _yapiDeviceLogCallback native_yDeviceLogDelegate = native_yDeviceLogCallback;
    static GCHandle native_yDeviceLogAnchor = GCHandle.Alloc(native_yDeviceLogDelegate);


    /**
     * <summary>
     *   Returns the version identifier for the Yoctopuce library in use.
     * <para>
     *   The version is a string in the form <c>"Major.Minor.Build"</c>,
     *   for instance <c>"1.01.5535"</c>. For languages using an external
     *   DLL (for instance C#, VisualBasic or Delphi), the character string
     *   includes as well the DLL version, for instance
     *   <c>"1.01.5535 (1.01.5439)"</c>.
     * </para>
     * <para>
     *   If you want to verify in your code that the library version is
     *   compatible with the version that you have used during development,
     *   verify that the major number is strictly equal and that the minor
     *   number is greater or equal. The build number is not relevant
     *   with respect to the library compatibility.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   a character string describing the library version.
     * </returns>
     */
    public static String GetAPIVersion()
    {
      string version = default(string);
      string date = default(string);
      apiGetAPIVersion(ref  version, ref date);
      return YOCTO_API_VERSION_STR + "." + YOCTO_API_BUILD_NO + " (" + version + ")";
    }

    /**
     * <summary>
     *   Initializes the Yoctopuce programming library explicitly.
     * <para>
     *   It is not strictly needed to call <c>yInitAPI()</c>, as the library is
     *   automatically  initialized when calling <c>yRegisterHub()</c> for the
     *   first time.
     * </para>
     * <para>
     *   When <c>YAPI.DETECT_NONE</c> is used as detection <c>mode</c>,
     *   you must explicitly use <c>yRegisterHub()</c> to point the API to the
     *   VirtualHub on which your devices are connected before trying to access them.
     * </para>
     * </summary>
     * <param name="mode">
     *   an integer corresponding to the type of automatic
     *   device detection to use. Possible values are
     *   <c>YAPI.DETECT_NONE</c>, <c>YAPI.DETECT_USB</c>, <c>YAPI.DETECT_NET</c>,
     *   and <c>YAPI.DETECT_ALL</c>.
     * </param>
     * <param name="errmsg">
     *   a string passed by reference to receive any error message.
     * </param>
     * <returns>
     *   <c>YAPI.SUCCESS</c> when the call succeeds.
     * </returns>
     * <para>
     *   On failure, throws an exception or returns a negative error code.
     * </para>
     */
    public static int InitAPI(int mode, ref string errmsg)
    {
      int i;
      int functionReturnValue = 0;
      StringBuilder buffer = new StringBuilder(YOCTO_ERRMSG_LEN);
      YRETCODE res = default(YRETCODE);

      if (_apiInitialized)
      {
        functionReturnValue = SUCCESS;
        return functionReturnValue;
      }
      string version = default(string);
      string date = default(string);
      if (apiGetAPIVersion(ref  version, ref date) != YOCTO_API_VERSION_BCD)
      {
        errmsg = "yapi.dll does does not match the version of the Libary (Libary=" + YOCTO_API_VERSION_STR + "." + YOCTO_API_BUILD_NO;
        errmsg += " yapi.dll=" + version + ")";
        return VERSION_MISMATCH;
      }


      csmodule_initialization();

      buffer.Length = 0;
      res = _yapiInitAPI(mode, buffer);
      errmsg = buffer.ToString();
      if ((YISERR(res)))
      {
        functionReturnValue = res;
        return functionReturnValue;
      }

      _yapiRegisterDeviceArrivalCallback(Marshal.GetFunctionPointerForDelegate(native_yDeviceArrivalDelegate));
      _yapiRegisterDeviceRemovalCallback(Marshal.GetFunctionPointerForDelegate(native_yDeviceRemovalDelegate));
      _yapiRegisterDeviceChangeCallback(Marshal.GetFunctionPointerForDelegate(native_yDeviceChangeDelegate));
      _yapiRegisterFunctionUpdateCallback(Marshal.GetFunctionPointerForDelegate(native_yFunctionUpdateDelegate));
      _yapiRegisterTimedReportCallback(Marshal.GetFunctionPointerForDelegate(native_yTimedReportDelegate));
      _yapiRegisterHubDiscoveryCallback(Marshal.GetFunctionPointerForDelegate(native_yapiHubDiscoveryDelegate));
      _yapiRegisterDeviceLogCallback(Marshal.GetFunctionPointerForDelegate(native_yDeviceLogDelegate));
      _yapiRegisterLogFunction(Marshal.GetFunctionPointerForDelegate(native_yLogFunctionDelegate));
      for (i = 1; i <= 20; i++)
        RegisterCalibrationHandler(i, yLinearCalibrationHandler);

      _apiInitialized = true;
      functionReturnValue = res;
      return functionReturnValue;
    }
    /**
     * <summary>
     *   Frees dynamically allocated memory blocks used by the Yoctopuce library.
     * <para>
     *   It is generally not required to call this function, unless you
     *   want to free all dynamically allocated memory blocks in order to
     *   track a memory leak for instance.
     *   You should not call any other library function after calling
     *   <c>yFreeAPI()</c>, or your program will crash.
     * </para>
     * </summary>
     */
    public static void FreeAPI()
    {
      if (_apiInitialized)
      {
        _yapiFreeAPI();
        csmodule_cleanup();
        _apiInitialized = false;
      }
    }
    /**
     * <summary>
     *   Setup the Yoctopuce library to use modules connected on a given machine.
     * <para>
     *   The
     *   parameter will determine how the API will work. Use the following values:
     * </para>
     * <para>
     *   <b>usb</b>: When the <c>usb</c> keyword is used, the API will work with
     *   devices connected directly to the USB bus. Some programming languages such a Javascript,
     *   PHP, and Java don't provide direct access to USB hardware, so <c>usb</c> will
     *   not work with these. In this case, use a VirtualHub or a networked YoctoHub (see below).
     * </para>
     * <para>
     *   <b><i>x.x.x.x</i></b> or <b><i>hostname</i></b>: The API will use the devices connected to the
     *   host with the given IP address or hostname. That host can be a regular computer
     *   running a VirtualHub, or a networked YoctoHub such as YoctoHub-Ethernet or
     *   YoctoHub-Wireless. If you want to use the VirtualHub running on you local
     *   computer, use the IP address 127.0.0.1.
     * </para>
     * <para>
     *   <b>callback</b>: that keyword make the API run in "<i>HTTP Callback</i>" mode.
     *   This a special mode allowing to take control of Yoctopuce devices
     *   through a NAT filter when using a VirtualHub or a networked YoctoHub. You only
     *   need to configure your hub to call your server script on a regular basis.
     *   This mode is currently available for PHP and Node.JS only.
     * </para>
     * <para>
     *   Be aware that only one application can use direct USB access at a
     *   given time on a machine. Multiple access would cause conflicts
     *   while trying to access the USB modules. In particular, this means
     *   that you must stop the VirtualHub software before starting
     *   an application that uses direct USB access. The workaround
     *   for this limitation is to setup the library to use the VirtualHub
     *   rather than direct USB access.
     * </para>
     * <para>
     *   If access control has been activated on the hub, virtual or not, you want to
     *   reach, the URL parameter should look like:
     * </para>
     * <para>
     *   <c>http://username:password@adresse:port</c>
     * </para>
     * <para>
     *   You can call <i>RegisterHub</i> several times to connect to several machines.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="url">
     *   a string containing either <c>"usb"</c>,<c>"callback"</c> or the
     *   root URL of the hub to monitor
     * </param>
     * <param name="errmsg">
     *   a string passed by reference to receive any error message.
     * </param>
     * <returns>
     *   <c>YAPI.SUCCESS</c> when the call succeeds.
     * </returns>
     * <para>
     *   On failure, throws an exception or returns a negative error code.
     * </para>
     */
    public static int RegisterHub(string url, ref string errmsg)
    {
      StringBuilder buffer = new StringBuilder(YOCTO_ERRMSG_LEN);
      YRETCODE res;

      if (!_apiInitialized)
      {
        res = InitAPI(0, ref errmsg);
        if (YISERR(res))
          return res;
      }

      buffer.Length = 0;
      res = _yapiRegisterHub(new StringBuilder(url), buffer);
      if (YISERR(res))
      {
        errmsg = buffer.ToString();
      }
      return res;
    }

    /**
     * <summary>
     *   Fault-tolerant alternative to RegisterHub().
     * <para>
     *   This function has the same
     *   purpose and same arguments as <c>RegisterHub()</c>, but does not trigger
     *   an error when the selected hub is not available at the time of the function call.
     *   This makes it possible to register a network hub independently of the current
     *   connectivity, and to try to contact it only when a device is actively needed.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="url">
     *   a string containing either <c>"usb"</c>,<c>"callback"</c> or the
     *   root URL of the hub to monitor
     * </param>
     * <param name="errmsg">
     *   a string passed by reference to receive any error message.
     * </param>
     * <returns>
     *   <c>YAPI.SUCCESS</c> when the call succeeds.
     * </returns>
     * <para>
     *   On failure, throws an exception or returns a negative error code.
     * </para>
     */
    public static int PreregisterHub(string url, ref string errmsg)
    {
      StringBuilder buffer = new StringBuilder(YOCTO_ERRMSG_LEN);
      YRETCODE res;

      if (!_apiInitialized)
      {
        res = InitAPI(0, ref errmsg);
        if (YISERR(res))
          return res;
      }

      buffer.Length = 0;
      res = _yapiPreregisterHub(new StringBuilder(url), buffer);
      if (YISERR(res))
      {
        errmsg = buffer.ToString();
      }
      return res;
    }

    /**
     * <summary>
     *   Setup the Yoctopuce library to no more use modules connected on a previously
     *   registered machine with RegisterHub.
     * <para>
     * </para>
     * </summary>
     * <param name="url">
     *   a string containing either <c>"usb"</c> or the
     *   root URL of the hub to monitor
     * </param>
     */
    public static void UnregisterHub(string url)
    {
      if (!_apiInitialized)
      {
        return;
      }

      _yapiUnregisterHub(new StringBuilder(url));
    }

    /**
     * <summary>
     *   Triggers a (re)detection of connected Yoctopuce modules.
     * <para>
     *   The library searches the machines or USB ports previously registered using
     *   <c>yRegisterHub()</c>, and invokes any user-defined callback function
     *   in case a change in the list of connected devices is detected.
     * </para>
     * <para>
     *   This function can be called as frequently as desired to refresh the device list
     *   and to make the application aware of hot-plug events.
     * </para>
     * </summary>
     * <param name="errmsg">
     *   a string passed by reference to receive any error message.
     * </param>
     * <returns>
     *   <c>YAPI.SUCCESS</c> when the call succeeds.
     * </returns>
     * <para>
     *   On failure, throws an exception or returns a negative error code.
     * </para>
     */
    public static YRETCODE UpdateDeviceList(ref string errmsg)
    {
      StringBuilder errbuffer = new StringBuilder(YOCTO_ERRMSG_LEN);
      YRETCODE res = default(YRETCODE);
      PlugEvent p;

      if (!_apiInitialized)
      {
        res = InitAPI(0, ref errmsg);
        if (YISERR(res))
          return res;
      }
      res = yapiUpdateDeviceList(0, ref errmsg);
      if (YISERR(res)) { return res; }

      errbuffer.Length = 0;
      res = _yapiHandleEvents(errbuffer);
      if (YISERR(res))
      {
        errmsg = errbuffer.ToString();
        return res;
      }

      while (_PlugEvents.Count > 0)
      {
        yapiLockDeviceCallBack(ref errmsg);
        p = _PlugEvents[0];
        _PlugEvents.RemoveAt(0);
        yapiUnlockDeviceCallBack(ref errmsg);
        p.invoke();

      }
      return SUCCESS;
    }


    /**
     * <summary>
     *   Maintains the device-to-library communication channel.
     * <para>
     *   If your program includes significant loops, you may want to include
     *   a call to this function to make sure that the library takes care of
     *   the information pushed by the modules on the communication channels.
     *   This is not strictly necessary, but it may improve the reactivity
     *   of the library for the following commands.
     * </para>
     * <para>
     *   This function may signal an error in case there is a communication problem
     *   while contacting a module.
     * </para>
     * </summary>
     * <param name="errmsg">
     *   a string passed by reference to receive any error message.
     * </param>
     * <returns>
     *   <c>YAPI.SUCCESS</c> when the call succeeds.
     * </returns>
     * <para>
     *   On failure, throws an exception or returns a negative error code.
     * </para>
     */
    public static YRETCODE HandleEvents(ref string errmsg)
    {
      YRETCODE functionReturnValue = default(YRETCODE);

      StringBuilder errBuffer = new StringBuilder(YOCTO_ERRMSG_LEN);
      YRETCODE res = default(YRETCODE);


      errBuffer.Length = 0;
      res = _yapiHandleEvents(errBuffer);

      if ((YISERR(res)))
      {
        errmsg = errBuffer.ToString();
        functionReturnValue = res;
        return functionReturnValue;
      }

      while ((_DataEvents.Count > 0))
      {
        yapiLockFunctionCallBack(ref errmsg);
        if (_DataEvents.Count == 0)
        {
          yapiUnlockFunctionCallBack(ref errmsg);
          break;
        }
        DataEvent ev = _DataEvents[0];
        _DataEvents.RemoveAt(0);
        yapiUnlockFunctionCallBack(ref errmsg);
        ev.invoke();
      }
      functionReturnValue = SUCCESS;
      return functionReturnValue;
    }
    /**
     * <summary>
     *   Pauses the execution flow for a specified duration.
     * <para>
     *   This function implements a passive waiting loop, meaning that it does not
     *   consume CPU cycles significantly. The processor is left available for
     *   other threads and processes. During the pause, the library nevertheless
     *   reads from time to time information from the Yoctopuce modules by
     *   calling <c>yHandleEvents()</c>, in order to stay up-to-date.
     * </para>
     * <para>
     *   This function may signal an error in case there is a communication problem
     *   while contacting a module.
     * </para>
     * </summary>
     * <param name="ms_duration">
     *   an integer corresponding to the duration of the pause,
     *   in milliseconds.
     * </param>
     * <param name="errmsg">
     *   a string passed by reference to receive any error message.
     * </param>
     * <returns>
     *   <c>YAPI.SUCCESS</c> when the call succeeds.
     * </returns>
     * <para>
     *   On failure, throws an exception or returns a negative error code.
     * </para>
     */
    public static int Sleep(int ms_duration, ref string errmsg)
    {
      int functionReturnValue = 0;

      StringBuilder errBuffer = new StringBuilder(YOCTO_ERRMSG_LEN);
      ulong timeout = 0;
      int res = 0;


      timeout = GetTickCount() + (ulong)ms_duration;
      res = SUCCESS;
      errBuffer.Length = 0;

      do
      {
        res = HandleEvents(ref errmsg);
        if ((YISERR(res)))
        {
          functionReturnValue = res;
          return functionReturnValue;
        }
        if ((GetTickCount() < timeout))
        {
          res = _yapiSleep(1, errBuffer);
          if ((YISERR(res)))
          {
            functionReturnValue = res;
            errmsg = errBuffer.ToString();
            return functionReturnValue;
          }
        }

      } while (!(GetTickCount() >= timeout));
      errmsg = errBuffer.ToString();
      functionReturnValue = res;
      return functionReturnValue;
    }


    /**
     * <summary>
     *   Force a hub discovery, if a callback as been registered with <c>yRegisterDeviceRemovalCallback</c> it
     *   will be called for each net work hub that will respond to the discovery.
     * <para>
     * </para>
     * </summary>
     * <param name="errmsg">
     *   a string passed by reference to receive any error message.
     * </param>
     * <returns>
     *   <c>YAPI.SUCCESS</c> when the call succeeds.
     *   On failure, throws an exception or returns a negative error code.
     * </returns>
     */
    public static int TriggerHubDiscovery(ref string errmsg)
    {
      StringBuilder buffer = new StringBuilder(YOCTO_ERRMSG_LEN);
      YRETCODE res;

      if (!_apiInitialized)
      {
        res = InitAPI(0, ref errmsg);
        if (YISERR(res))
          return res;
      }

      buffer.Length = 0;
      res = _yapiTriggerHubDiscovery(buffer);
      if (YISERR(res))
      {
        errmsg = buffer.ToString();
      }
      return res;
    }

    /**
     * <summary>
     *   Returns the current value of a monotone millisecond-based time counter.
     * <para>
     *   This counter can be used to compute delays in relation with
     *   Yoctopuce devices, which also uses the millisecond as timebase.
     * </para>
     * </summary>
     * <returns>
     *   a long integer corresponding to the millisecond counter.
     * </returns>
     */
    public static ulong GetTickCount()
    {
      return Convert.ToUInt64((ulong)_yapiGetTickCount());
    }

    /**
     * <summary>
     *   Checks if a given string is valid as logical name for a module or a function.
     * <para>
     *   A valid logical name has a maximum of 19 characters, all among
     *   <c>A..Z</c>, <c>a..z</c>, <c>0..9</c>, <c>_</c>, and <c>-</c>.
     *   If you try to configure a logical name with an incorrect string,
     *   the invalid characters are ignored.
     * </para>
     * </summary>
     * <param name="name">
     *   a string containing the name to check.
     * </param>
     * <returns>
     *   <c>true</c> if the name is valid, <c>false</c> otherwise.
     * </returns>
     */
    public static bool CheckLogicalName(string name)
    {
      bool functionReturnValue = false;
      if ((_yapiCheckLogicalName(new StringBuilder(name)) == 0))
      {
        functionReturnValue = false;
      }
      else
      {
        functionReturnValue = true;
      }
      return functionReturnValue;
    }

    public static int yapiGetFunctionInfo(YFUN_DESCR fundesc, ref YDEV_DESCR devdesc, ref string serial, ref string funcId, ref string funcName, ref string funcVal, ref string errmsg)
    {
      int functionReturnValue = 0;

      StringBuilder serialBuffer = new StringBuilder(YOCTO_SERIAL_LEN);
      StringBuilder funcIdBuffer = new StringBuilder(YOCTO_FUNCTION_LEN);
      StringBuilder funcNameBuffer = new StringBuilder(YOCTO_LOGICAL_LEN);
      StringBuilder funcValBuffer = new StringBuilder(YOCTO_PUBVAL_LEN);
      StringBuilder errBuffer = new StringBuilder(YOCTO_ERRMSG_LEN);

      serialBuffer.Length = 0;
      funcIdBuffer.Length = 0;
      funcNameBuffer.Length = 0;
      funcValBuffer.Length = 0;
      errBuffer.Length = 0;

      functionReturnValue = _yapiGetFunctionInfo(fundesc, ref devdesc, serialBuffer, funcIdBuffer, funcNameBuffer, funcValBuffer, errBuffer);
      serial = serialBuffer.ToString();
      funcId = funcIdBuffer.ToString();
      funcName = funcNameBuffer.ToString();
      funcVal = funcValBuffer.ToString();
      errmsg = funcValBuffer.ToString();
      return functionReturnValue;
    }

    internal static int yapiGetDeviceByFunction(YFUN_DESCR fundesc, ref string errmsg)
    {
      int functionReturnValue = 0;
      StringBuilder errBuffer = new StringBuilder(YOCTO_ERRMSG_LEN);
      YDEV_DESCR devdesc = default(YDEV_DESCR);
      int res = 0;
      errBuffer.Length = 0;
      res = _yapiGetFunctionInfo(fundesc, ref devdesc, null, null, null, null, errBuffer);
      errmsg = errBuffer.ToString();
      if ((res < 0))
      {
        functionReturnValue = res;
      }
      else
      {
        functionReturnValue = devdesc;
      }
      return functionReturnValue;
    }

    public static u16 apiGetAPIVersion(ref string version, ref string date)
    {
      IntPtr pversion = default(IntPtr);
      IntPtr pdate = default(IntPtr);
      u16 res = default(u16);
      res = _yapiGetAPIVersion(ref pversion, ref pdate);
      version = Marshal.PtrToStringAnsi(pversion);
      date = Marshal.PtrToStringAnsi(pdate);
      return res;
    }


    internal static YRETCODE yapiUpdateDeviceList(uint force, ref string errmsg)
    {
      StringBuilder buffer = new StringBuilder(YOCTO_ERRMSG_LEN);
      buffer.Length = 0;
      YRETCODE res = _yapiUpdateDeviceList(force, buffer);
      if (YAPI.YISERR(res))
      {
        errmsg = buffer.ToString();
      }
      return res;
    }

    protected static YDEV_DESCR yapiGetDevice(ref string device_str, string errmsg)
    {
      YDEV_DESCR functionReturnValue = default(YDEV_DESCR);
      StringBuilder buffer = new StringBuilder(YOCTO_ERRMSG_LEN);
      buffer.Length = 0;
      functionReturnValue = _yapiGetDevice(new StringBuilder(device_str), buffer);
      errmsg = buffer.ToString();
      return functionReturnValue;
    }
    /* not used
    protected static int yapiGetAllDevices(IntPtr dbuffer, int maxsize, ref int neededsize, ref string errmsg)
    {
      int functionReturnValue = 0;
      StringBuilder buffer = new StringBuilder(YOCTO_ERRMSG_LEN);
      buffer.Length = 0;
      functionReturnValue = _yapiGetAllDevices(dbuffer, maxsize, ref neededsize, buffer);
      errmsg = buffer.ToString();
      return functionReturnValue;
    }
    */
    protected static int yapiGetDeviceInfo(YDEV_DESCR d, ref yDeviceSt infos, ref string errmsg)
    {
      int functionReturnValue = 0;
      StringBuilder buffer = new StringBuilder(YOCTO_ERRMSG_LEN);
      buffer.Length = 0;
      functionReturnValue = _yapiGetDeviceInfo(d, ref infos, buffer);
      errmsg = buffer.ToString();
      return functionReturnValue;
    }

    internal static YFUN_DESCR yapiGetFunction(string class_str, string function_str, ref string errmsg)
    {
      YFUN_DESCR functionReturnValue = default(YFUN_DESCR);
      StringBuilder buffer = new StringBuilder(YOCTO_ERRMSG_LEN);
      buffer.Length = 0;
      functionReturnValue = _yapiGetFunction(new StringBuilder(class_str), new StringBuilder(function_str), buffer);
      errmsg = buffer.ToString();
      return functionReturnValue;
    }

    public static int apiGetFunctionsByClass(string class_str, YFUN_DESCR precFuncDesc, IntPtr dbuffer, int maxsize, ref int neededsize, ref string errmsg)
    {
      int functionReturnValue = 0;
      StringBuilder buffer = new StringBuilder(YOCTO_ERRMSG_LEN);
      buffer.Length = 0;
      functionReturnValue = _yapiGetFunctionsByClass(new StringBuilder(class_str), precFuncDesc, dbuffer, maxsize, ref neededsize, buffer);
      errmsg = buffer.ToString();
      return functionReturnValue;
    }

    protected static int apiGetFunctionsByDevice(YDEV_DESCR devdesc, YFUN_DESCR precFuncDesc, IntPtr dbuffer, int maxsize, ref int neededsize, ref string errmsg)
    {
      int functionReturnValue = 0;
      StringBuilder buffer = new StringBuilder(YOCTO_ERRMSG_LEN);
      buffer.Length = 0;
      functionReturnValue = _yapiGetFunctionsByDevice(devdesc, precFuncDesc, dbuffer, maxsize, ref neededsize, buffer);
      errmsg = buffer.ToString();
      return functionReturnValue;
    }

    [DllImport("myDll.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    private extern static void DllCallTest(ref yDeviceSt data);

    [DllImport("yapi.dll", EntryPoint = "yapiInitAPI", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    private extern static int _yapiInitAPI(int mode, StringBuilder errmsg);

    [DllImport("yapi.dll", EntryPoint = "yapiFreeAPI", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    private extern static void _yapiFreeAPI();

    [DllImport("yapi.dll", EntryPoint = "yapiRegisterLogFunction", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    private extern static void _yapiRegisterLogFunction(IntPtr fct);

    [DllImport("yapi.dll", EntryPoint = "yapiRegisterDeviceArrivalCallback", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    private extern static void _yapiRegisterDeviceArrivalCallback(IntPtr fct);

    [DllImport("yapi.dll", EntryPoint = "yapiRegisterDeviceRemovalCallback", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    private extern static void _yapiRegisterDeviceRemovalCallback(IntPtr fct);

    [DllImport("yapi.dll", EntryPoint = "yapiRegisterDeviceChangeCallback", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    private extern static void _yapiRegisterDeviceChangeCallback(IntPtr fct);

    [DllImport("yapi.dll", EntryPoint = "yapiRegisterFunctionUpdateCallback", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    private extern static void _yapiRegisterFunctionUpdateCallback(IntPtr fct);

    [DllImport("yapi.dll", EntryPoint = "yapiRegisterTimedReportCallback", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    private extern static void _yapiRegisterTimedReportCallback(IntPtr fct);

    [DllImport("yapi.dll", EntryPoint = "yapiLockDeviceCallBack", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    private extern static int _yapiLockDeviceCallBack(StringBuilder errmsg);

    [DllImport("yapi.dll", EntryPoint = "yapiUnlockDeviceCallBack", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    private extern static int _yapiUnlockDeviceCallBack(StringBuilder errmsg);

    [DllImport("yapi.dll", EntryPoint = "yapiLockFunctionCallBack", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    private extern static int _yapiLockFunctionCallBack(StringBuilder errmsg);

    [DllImport("yapi.dll", EntryPoint = "yapiUnlockFunctionCallBack", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    private extern static int _yapiUnlockFunctionCallBack(StringBuilder errmsg);

    [DllImport("yapi.dll", EntryPoint = "yapiRegisterHub", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    private extern static int _yapiRegisterHub(StringBuilder rootUrl, StringBuilder errmsg);

    [DllImport("yapi.dll", EntryPoint = "yapiPreregisterHub", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    private extern static int _yapiPreregisterHub(StringBuilder rootUrl, StringBuilder errmsg);

    [DllImport("yapi.dll", EntryPoint = "yapiUnregisterHub", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    private extern static void _yapiUnregisterHub(StringBuilder rootUrl);

    [DllImport("yapi.dll", EntryPoint = "yapiUpdateDeviceList", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    private extern static int _yapiUpdateDeviceList(uint force, StringBuilder errmsg);

    [DllImport("yapi.dll", EntryPoint = "yapiHandleEvents", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    private extern static int _yapiHandleEvents(StringBuilder errmsg);

    [DllImport("yapi.dll", EntryPoint = "yapiGetTickCount", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    private extern static u64 _yapiGetTickCount();

    [DllImport("yapi.dll", EntryPoint = "yapiCheckLogicalName", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    private extern static int _yapiCheckLogicalName(StringBuilder name);

    [DllImport("yapi.dll", EntryPoint = "yapiGetAPIVersion", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    private extern static u16 _yapiGetAPIVersion(ref IntPtr version, ref IntPtr date);

    [DllImport("yapi.dll", EntryPoint = "yapiGetDevice", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    private extern static YDEV_DESCR _yapiGetDevice(StringBuilder device_str, StringBuilder errmsg);

    [DllImport("yapi.dll", EntryPoint = "yapiGetAllDevices", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    private extern static int _yapiGetAllDevices(IntPtr buffer, int maxsize, ref int neededsize, StringBuilder errmsg);

    [DllImport("yapi.dll", EntryPoint = "yapiGetDeviceInfo", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    private extern static int _yapiGetDeviceInfo(YDEV_DESCR d, ref yDeviceSt infos, StringBuilder errmsg);

    [DllImport("yapi.dll", EntryPoint = "yapiGetFunction", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    private extern static YFUN_DESCR _yapiGetFunction(StringBuilder class_str, StringBuilder function_str, StringBuilder errmsg);

    [DllImport("yapi.dll", EntryPoint = "yapiGetFunctionsByClass", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    private extern static int _yapiGetFunctionsByClass(StringBuilder class_str, YFUN_DESCR precFuncDesc, IntPtr buffer, int maxsize, ref int neededsize, StringBuilder errmsg);

    [DllImport("yapi.dll", EntryPoint = "yapiGetFunctionsByDevice", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    private extern static int _yapiGetFunctionsByDevice(YDEV_DESCR device, YFUN_DESCR precFuncDesc, IntPtr buffer, int maxsize, ref int neededsize, StringBuilder errmsg);

    [DllImport("yapi.dll", EntryPoint = "yapiGetFunctionInfo", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    internal extern static int _yapiGetFunctionInfo(YFUN_DESCR fundesc, ref YDEV_DESCR devdesc, StringBuilder serial, StringBuilder funcId, StringBuilder funcName, StringBuilder funcVal, StringBuilder errmsg);

    [DllImport("yapi.dll", EntryPoint = "yapiGetErrorString", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    private extern static int _yapiGetErrorString(int errorcode, StringBuilder buffer, int maxsize, StringBuilder errmsg);

    [DllImport("yapi.dll", EntryPoint = "yapiHTTPRequestSyncStart", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    private extern static int _yapiHTTPRequestSyncStart(ref YIOHDL iohdl, StringBuilder device, IntPtr request, ref IntPtr reply, ref int replysize, StringBuilder errmsg);

    [DllImport("yapi.dll", EntryPoint = "yapiHTTPRequestSyncStartEx", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    private extern static int _yapiHTTPRequestSyncStartEx(ref YIOHDL iohdl, StringBuilder device, IntPtr request, int requestlen, ref IntPtr reply, ref int replysize, StringBuilder errmsg);

    [DllImport("yapi.dll", EntryPoint = "yapiHTTPRequestSyncDone", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    private extern static int _yapiHTTPRequestSyncDone(ref YIOHDL iohdl, StringBuilder errmsg);

    [DllImport("yapi.dll", EntryPoint = "yapiHTTPRequestAsync", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    private extern static int _yapiHTTPRequestAsync(StringBuilder device, IntPtr request, IntPtr callback, IntPtr context, StringBuilder errmsg);

    [DllImport("yapi.dll", EntryPoint = "yapiHTTPRequestAsyncEx", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    private extern static int _yapiHTTPRequestAsyncEx(StringBuilder device, IntPtr request, int requestlen, IntPtr callback, IntPtr context, StringBuilder errmsg);

    [DllImport("yapi.dll", EntryPoint = "yapiHTTPRequest", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    private extern static int _yapiHTTPRequest(StringBuilder device, StringBuilder url, StringBuilder buffer, int buffsize, ref int fullsize, StringBuilder errmsg);

    [DllImport("yapi.dll", EntryPoint = "yapiGetBootloadersDevs", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    private extern static int _yapiGetBootloadersDevs(StringBuilder serials, u32 maxNbSerial, ref u32 totalBootladers, StringBuilder errmsg);

    [DllImport("yapi.dll", EntryPoint = "yapiFlashDevice", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    private extern static int _yapiFlashDevice(ref yFlashArg args, StringBuilder errmsg);

    [DllImport("yapi.dll", EntryPoint = "yapiVerifyDevice", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    private extern static int _yapiVerifyDevice(ref yFlashArg args, StringBuilder errmsg);

    [DllImport("yapi.dll", EntryPoint = "yapiGetDevicePath", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    private extern static int _yapiGetDevicePath(int devdesc, StringBuilder rootdevice, StringBuilder path, int pathsize, ref int neededsize, StringBuilder errmsg);

    [DllImport("yapi.dll", EntryPoint = "yapiSleep", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    private extern static int _yapiSleep(int duration_ms, StringBuilder errmsg);

    [DllImport("yapi.dll", EntryPoint = "yapiRegisterHubDiscoveryCallback", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    private extern static void _yapiRegisterHubDiscoveryCallback(IntPtr fct);

    [DllImport("yapi.dll", EntryPoint = "yapiTriggerHubDiscovery", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    private extern static int _yapiTriggerHubDiscovery(StringBuilder errmsg);

    [DllImport("yapi.dll", EntryPoint = "yapiRegisterDeviceLogCallback", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    internal extern static void _yapiRegisterDeviceLogCallback(IntPtr fct);

    [DllImport("yapi.dll", EntryPoint = "yapiStartStopDeviceLogCallback", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    internal extern static int _yapiStartStopDeviceLogCallback(StringBuilder errmsg, int start_stop);

    private static void csmodule_initialization()
    {
      YDevice_devCache = new List<YDevice>();
      _PlugEvents = new List<PlugEvent>(5);
      _DataEvents = new List<DataEvent>(10);


    }

    private static void csmodule_cleanup()
    {
      YDevice_devCache.Clear();
      YDevice_devCache = null;
      _PlugEvents.Clear();
      _PlugEvents = null;
      _DataEvents.Clear();
      _DataEvents = null;
      YFunction._CalibHandlers.Clear();

    }

  }

  // Backward-compatibility with previous versions of the library
  public class yAPI : YAPI
  {
  }


  //--- (generated code: YDataStream class start)
  /**
   * <summary>
   *   YDataStream objects represent bare recorded measure sequences,
   *   exactly as found within the data logger present on Yoctopuce
   *   sensors.
   * <para>
   * </para>
   * <para>
   *   In most cases, it is not necessary to use YDataStream objects
   *   directly, as the YDataSet objects (returned by the
   *   <c>get_recordedData()</c> method from sensors and the
   *   <c>get_dataSets()</c> method from the data logger) provide
   *   a more convenient interface.
   * </para>
   * <para>
   * </para>
   * </summary>
   */
  public class YDataStream
  {
    //--- (end of generated code: YDataStream class start)

    public const double DATA_INVALID = YAPI.INVALID_DOUBLE;
    public const int DURATION_INVALID = -1;

    //--- (generated code: YDataStream definitions)

    protected YFunction _parent;
    protected int _runNo = 0;
    protected long _utcStamp = 0;
    protected int _nCols = 0;
    protected int _nRows = 0;
    protected int _duration = 0;
    protected List<string> _columnNames = new List<string>();
    protected string _functionId;
    protected bool _isClosed;
    protected bool _isAvg;
    protected bool _isScal;
    protected int _decimals = 0;
    protected double _offset = 0;
    protected double _scale = 0;
    protected int _samplesPerHour = 0;
    protected double _minVal = 0;
    protected double _avgVal = 0;
    protected double _maxVal = 0;
    protected double _decexp = 0;
    protected int _caltyp = 0;
    protected List<int> _calpar = new List<int>();
    protected List<double> _calraw = new List<double>();
    protected List<double> _calref = new List<double>();
    protected List<List<double>> _values = new List<List<double>>();
    //--- (end of generated code: YDataStream definitions)

    protected YAPI.yCalibrationHandler _calhdl;

    public YDataStream(YFunction parent)
    {
      this._parent = parent;
      //--- (generated code: YDataStream attributes initialization)
      //--- (end of generated code: YDataStream attributes initialization)
    }

    public YDataStream(YFunction parent, YDataSet dataset, List<int> encoded)
    {
      this._parent = parent;
      //--- (generated code: YDataStream attributes initialization)
      //--- (end of generated code: YDataStream attributes initialization)
      this._initFromDataSet(dataset, encoded);

    }


    //--- (generated code: YDataStream implementation)


    public virtual int _initFromDataSet(YDataSet dataset, List<int> encoded)
    {
      int val = 0;
      int i = 0;
      int iRaw = 0;
      int iRef = 0;
      double fRaw = 0;
      double fRef = 0;
      double duration_float = 0;
      List<int> iCalib = new List<int>();

      // decode sequence header to extract data
      this._runNo = encoded[0] + (((encoded[1]) << (16)));
      this._utcStamp = encoded[2] + (((encoded[3]) << (16)));
      val = encoded[4];
      this._isAvg = (((val) & (0x100)) == 0);
      this._samplesPerHour = ((val) & (0xff));
      if (((val) & (0x100)) != 0)
      {
        this._samplesPerHour = this._samplesPerHour * 3600;
      }
      else
      {
        if (((val) & (0x200)) != 0)
        {
          this._samplesPerHour = this._samplesPerHour * 60;
        }
      }

      val = encoded[5];
      if (val > 32767)
      {
        val = val - 65536;
      }
      this._decimals = val;
      this._offset = val;
      this._scale = encoded[6];
      this._isScal = (this._scale != 0);

      val = encoded[7];
      this._isClosed = (val != 0xffff);
      if (val == 0xffff)
      {
        val = 0;
      }
      this._nRows = val;
      duration_float = this._nRows * 3600 / this._samplesPerHour;
      this._duration = (int)Math.Round(duration_float);
      // precompute decoding parameters
      this._decexp = 1.0;
      if (this._scale == 0)
      {
        i = 0;
        while (i < this._decimals)
        {
          this._decexp = this._decexp * 10.0;
          i = i + 1;
        }
      }
      iCalib = dataset.get_calibration();
      this._caltyp = iCalib[0];
      if (this._caltyp != 0)
      {
        this._calhdl = YAPI._getCalibrationHandler(this._caltyp);
        this._calpar.Clear();
        this._calraw.Clear();
        this._calref.Clear();
        i = 1;
        while (i + 1 < iCalib.Count)
        {
          iRaw = iCalib[i];
          iRef = iCalib[i + 1];
          this._calpar.Add(iRaw);
          this._calpar.Add(iRef);
          if (this._isScal)
          {
            fRaw = iRaw;
            fRaw = (fRaw - this._offset) / this._scale;
            fRef = iRef;
            fRef = (fRef - this._offset) / this._scale;
            this._calraw.Add(fRaw);
            this._calref.Add(fRef);
          }
          else
          {
            this._calraw.Add(YAPI._decimalToDouble(iRaw));
            this._calref.Add(YAPI._decimalToDouble(iRef));
          }
          i = i + 2;
        }
      }
      // preload column names for backward-compatibility
      this._functionId = dataset.get_functionId();
      if (this._isAvg)
      {
        this._columnNames.Clear();
        this._columnNames.Add("" + this._functionId + "_min");
        this._columnNames.Add("" + this._functionId + "_avg");
        this._columnNames.Add("" + this._functionId + "_max");
        this._nCols = 3;
      }
      else
      {
        this._columnNames.Clear();
        this._columnNames.Add(this._functionId);
        this._nCols = 1;
      }
      // decode min/avg/max values for the sequence
      if (this._nRows > 0)
      {
        this._minVal = this._decodeVal(encoded[8]);
        this._maxVal = this._decodeVal(encoded[9]);
        this._avgVal = this._decodeAvg(encoded[10] + (((encoded[11]) << (16))), this._nRows);
      }
      return 0;
    }

    public virtual int parse(byte[] sdata)
    {
      int idx = 0;
      List<int> udat = new List<int>();
      List<double> dat = new List<double>();
      // may throw an exception
      udat = YAPI._decodeWords(this._parent._json_get_string(sdata));
      this._values.Clear();
      idx = 0;
      if (this._isAvg)
      {
        while (idx + 3 < udat.Count)
        {
          dat.Clear();
          dat.Add(this._decodeVal(udat[idx]));
          dat.Add(this._decodeAvg(udat[idx + 2] + (((udat[idx + 3]) << (16))), 1));
          dat.Add(this._decodeVal(udat[idx + 1]));
          this._values.Add(new List<double>(dat));
          idx = idx + 4;
        }
      }
      else
      {
        if (this._isScal)
        {
          while (idx < udat.Count)
          {
            dat.Clear();
            dat.Add(this._decodeVal(udat[idx]));
            this._values.Add(new List<double>(dat));
            idx = idx + 1;
          }
        }
        else
        {
          while (idx + 1 < udat.Count)
          {
            dat.Clear();
            dat.Add(this._decodeAvg(udat[idx] + (((udat[idx + 1]) << (16))), 1));
            this._values.Add(new List<double>(dat));
            idx = idx + 2;
          }
        }
      }

      this._nRows = this._values.Count;
      return YAPI.SUCCESS;
    }

    public virtual string get_url()
    {
      string url;
      url = "logger.json?id=" +
      this._functionId + "&run=" + Convert.ToString(this._runNo) + "&utc=" + Convert.ToString(this._utcStamp);
      return url;
    }

    public virtual int loadStream()
    {
      return this.parse(this._parent._download(this.get_url()));
    }

    public virtual double _decodeVal(int w)
    {
      double val = 0;
      val = w;
      if (this._isScal)
      {
        val = (val - this._offset) / this._scale;
      }
      else
      {
        val = YAPI._decimalToDouble(w);
      }
      if (this._caltyp != 0)
      {
        val = this._calhdl(val, this._caltyp, this._calpar, this._calraw, this._calref);
      }
      return val;
    }

    public virtual double _decodeAvg(int dw, int count)
    {
      double val = 0;
      val = dw;
      if (this._isScal)
      {
        val = (val / (100 * count) - this._offset) / this._scale;
      }
      else
      {
        val = val / (count * this._decexp);
      }
      if (this._caltyp != 0)
      {
        val = this._calhdl(val, this._caltyp, this._calpar, this._calraw, this._calref);
      }
      return val;
    }

    public virtual bool isClosed()
    {
      return this._isClosed;
    }

    /**
     * <summary>
     *   Returns the run index of the data stream.
     * <para>
     *   A run can be made of
     *   multiple datastreams, for different time intervals.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   an unsigned number corresponding to the run index.
     * </returns>
     */
    public virtual int get_runIndex()
    {
      return this._runNo;
    }

    /**
     * <summary>
     *   Returns the relative start time of the data stream, measured in seconds.
     * <para>
     *   For recent firmwares, the value is relative to the present time,
     *   which means the value is always negative.
     *   If the device uses a firmware older than version 13000, value is
     *   relative to the start of the time the device was powered on, and
     *   is always positive.
     *   If you need an absolute UTC timestamp, use <c>get_startTimeUTC()</c>.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   an unsigned number corresponding to the number of seconds
     *   between the start of the run and the beginning of this data
     *   stream.
     * </returns>
     */
    public virtual int get_startTime()
    {
      return (int)(this._utcStamp - Convert.ToUInt32((DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds));
    }

    /**
     * <summary>
     *   Returns the start time of the data stream, relative to the Jan 1, 1970.
     * <para>
     *   If the UTC time was not set in the datalogger at the time of the recording
     *   of this data stream, this method returns 0.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   an unsigned number corresponding to the number of seconds
     *   between the Jan 1, 1970 and the beginning of this data
     *   stream (i.e. Unix time representation of the absolute time).
     * </returns>
     */
    public virtual long get_startTimeUTC()
    {
      return this._utcStamp;
    }

    /**
     * <summary>
     *   Returns the number of milliseconds between two consecutive
     *   rows of this data stream.
     * <para>
     *   By default, the data logger records one row
     *   per second, but the recording frequency can be changed for
     *   each device function
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   an unsigned number corresponding to a number of milliseconds.
     * </returns>
     */
    public virtual int get_dataSamplesIntervalMs()
    {
      return ((3600000) / (this._samplesPerHour));
    }

    public virtual double get_dataSamplesInterval()
    {
      return 3600.0 / this._samplesPerHour;
    }

    /**
     * <summary>
     *   Returns the number of data rows present in this stream.
     * <para>
     * </para>
     * <para>
     *   If the device uses a firmware older than version 13000,
     *   this method fetches the whole data stream from the device
     *   if not yet done, which can cause a little delay.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   an unsigned number corresponding to the number of rows.
     * </returns>
     * <para>
     *   On failure, throws an exception or returns zero.
     * </para>
     */
    public virtual int get_rowCount()
    {
      if ((this._nRows != 0) && this._isClosed)
      {
        return this._nRows;
      }
      this.loadStream();
      return this._nRows;
    }

    /**
     * <summary>
     *   Returns the number of data columns present in this stream.
     * <para>
     *   The meaning of the values present in each column can be obtained
     *   using the method <c>get_columnNames()</c>.
     * </para>
     * <para>
     *   If the device uses a firmware older than version 13000,
     *   this method fetches the whole data stream from the device
     *   if not yet done, which can cause a little delay.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   an unsigned number corresponding to the number of columns.
     * </returns>
     * <para>
     *   On failure, throws an exception or returns zero.
     * </para>
     */
    public virtual int get_columnCount()
    {
      if (this._nCols != 0)
      {
        return this._nCols;
      }
      this.loadStream();
      return this._nCols;
    }

    /**
     * <summary>
     *   Returns the title (or meaning) of each data column present in this stream.
     * <para>
     *   In most case, the title of the data column is the hardware identifier
     *   of the sensor that produced the data. For streams recorded at a lower
     *   recording rate, the dataLogger stores the min, average and max value
     *   during each measure interval into three columns with suffixes _min,
     *   _avg and _max respectively.
     * </para>
     * <para>
     *   If the device uses a firmware older than version 13000,
     *   this method fetches the whole data stream from the device
     *   if not yet done, which can cause a little delay.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   a list containing as many strings as there are columns in the
     *   data stream.
     * </returns>
     * <para>
     *   On failure, throws an exception or returns an empty array.
     * </para>
     */
    public virtual List<string> get_columnNames()
    {
      if (this._columnNames.Count != 0)
      {
        return this._columnNames;
      }
      this.loadStream();
      return this._columnNames;
    }

    /**
     * <summary>
     *   Returns the smallest measure observed within this stream.
     * <para>
     *   If the device uses a firmware older than version 13000,
     *   this method will always return YDataStream.DATA_INVALID.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   a floating-point number corresponding to the smallest value,
     *   or YDataStream.DATA_INVALID if the stream is not yet complete (still recording).
     * </returns>
     * <para>
     *   On failure, throws an exception or returns YDataStream.DATA_INVALID.
     * </para>
     */
    public virtual double get_minValue()
    {
      return this._minVal;
    }

    /**
     * <summary>
     *   Returns the average of all measures observed within this stream.
     * <para>
     *   If the device uses a firmware older than version 13000,
     *   this method will always return YDataStream.DATA_INVALID.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   a floating-point number corresponding to the average value,
     *   or YDataStream.DATA_INVALID if the stream is not yet complete (still recording).
     * </returns>
     * <para>
     *   On failure, throws an exception or returns YDataStream.DATA_INVALID.
     * </para>
     */
    public virtual double get_averageValue()
    {
      return this._avgVal;
    }

    /**
     * <summary>
     *   Returns the largest measure observed within this stream.
     * <para>
     *   If the device uses a firmware older than version 13000,
     *   this method will always return YDataStream.DATA_INVALID.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   a floating-point number corresponding to the largest value,
     *   or YDataStream.DATA_INVALID if the stream is not yet complete (still recording).
     * </returns>
     * <para>
     *   On failure, throws an exception or returns YDataStream.DATA_INVALID.
     * </para>
     */
    public virtual double get_maxValue()
    {
      return this._maxVal;
    }

    /**
     * <summary>
     *   Returns the approximate duration of this stream, in seconds.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   the number of seconds covered by this stream.
     * </returns>
     * <para>
     *   On failure, throws an exception or returns YDataStream.DURATION_INVALID.
     * </para>
     */
    public virtual int get_duration()
    {
      if (this._isClosed)
      {
        return this._duration;
      }
      return (int)(Convert.ToUInt32((DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds) - this._utcStamp);
    }

    /**
     * <summary>
     *   Returns the whole data set contained in the stream, as a bidimensional
     *   table of numbers.
     * <para>
     *   The meaning of the values present in each column can be obtained
     *   using the method <c>get_columnNames()</c>.
     * </para>
     * <para>
     *   This method fetches the whole data stream from the device,
     *   if not yet done.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   a list containing as many elements as there are rows in the
     *   data stream. Each row itself is a list of floating-point
     *   numbers.
     * </returns>
     * <para>
     *   On failure, throws an exception or returns an empty array.
     * </para>
     */
    public virtual List<List<double>> get_dataRows()
    {
      if ((this._values.Count == 0) || !(this._isClosed))
      {
        this.loadStream();
      }
      return this._values;
    }

    /**
     * <summary>
     *   Returns a single measure from the data stream, specified by its
     *   row and column index.
     * <para>
     *   The meaning of the values present in each column can be obtained
     *   using the method get_columnNames().
     * </para>
     * <para>
     *   This method fetches the whole data stream from the device,
     *   if not yet done.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="row">
     *   row index
     * </param>
     * <param name="col">
     *   column index
     * </param>
     * <returns>
     *   a floating-point number
     * </returns>
     * <para>
     *   On failure, throws an exception or returns YDataStream.DATA_INVALID.
     * </para>
     */
    public virtual double get_data(int row, int col)
    {
      if ((this._values.Count == 0) || !(this._isClosed))
      {
        this.loadStream();
      }
      if (row >= this._values.Count)
      {
        return DATA_INVALID;
      }
      if (col >= this._values[row].Count)
      {
        return DATA_INVALID;
      }
      return this._values[row][col];
    }

    //--- (end of generated code: YDataStream implementation)
  }


  //--- (generated code: YMeasure class start)
  /**
   * <summary>
   *   YMeasure objects are used within the API to represent
   *   a value measured at a specified time.
   * <para>
   *   These objects are
   *   used in particular in conjunction with the YDataSet class.
   * </para>
   * <para>
   * </para>
   * </summary>
   */
  public class YMeasure
  {
    //--- (end of generated code: YMeasure class start)
    //--- (generated code: YMeasure definitions)

    protected double _start = 0;
    protected double _end = 0;
    protected double _minVal = 0;
    protected double _avgVal = 0;
    protected double _maxVal = 0;
    //--- (end of generated code: YMeasure definitions)
    protected DateTime _start_datetime;
    protected DateTime _end_datetime;
    private static DateTime _epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    public YMeasure(double start, double end, double minVal, double avgVal, double maxVal)
    {
      //--- (generated code: YMeasure attributes initialization)
      //--- (end of generated code: YMeasure attributes initialization)
      this._start = start;
      this._end = end;
      this._minVal = minVal;
      this._avgVal = avgVal;
      this._maxVal = maxVal;
      this._start_datetime = _epoch.AddSeconds(this._start);
      this._end_datetime = _epoch.AddSeconds(this._end);
    }

    public YMeasure()
    {
    }

    public DateTime get_startTimeUTC_asDateTime()
    {
      return this._start_datetime;
    }

    public DateTime get_endTimeUTC_asDateTime()
    {
      return this._end_datetime;
    }


    //--- (generated code: YMeasure implementation)


    /**
     * <summary>
     *   Returns the start time of the measure, relative to the Jan 1, 1970 UTC
     *   (Unix timestamp).
     * <para>
     *   When the recording rate is higher then 1 sample
     *   per second, the timestamp may have a fractional part.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   an floating point number corresponding to the number of seconds
     *   between the Jan 1, 1970 UTC and the beginning of this measure.
     * </returns>
     */
    public virtual double get_startTimeUTC()
    {
      return this._start;
    }

    /**
     * <summary>
     *   Returns the end time of the measure, relative to the Jan 1, 1970 UTC
     *   (Unix timestamp).
     * <para>
     *   When the recording rate is higher than 1 sample
     *   per second, the timestamp may have a fractional part.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   an floating point number corresponding to the number of seconds
     *   between the Jan 1, 1970 UTC and the end of this measure.
     * </returns>
     */
    public virtual double get_endTimeUTC()
    {
      return this._end;
    }

    /**
     * <summary>
     *   Returns the smallest value observed during the time interval
     *   covered by this measure.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   a floating-point number corresponding to the smallest value observed.
     * </returns>
     */
    public virtual double get_minValue()
    {
      return this._minVal;
    }

    /**
     * <summary>
     *   Returns the average value observed during the time interval
     *   covered by this measure.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   a floating-point number corresponding to the average value observed.
     * </returns>
     */
    public virtual double get_averageValue()
    {
      return this._avgVal;
    }

    /**
     * <summary>
     *   Returns the largest value observed during the time interval
     *   covered by this measure.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   a floating-point number corresponding to the largest value observed.
     * </returns>
     */
    public virtual double get_maxValue()
    {
      return this._maxVal;
    }

    //--- (end of generated code: YMeasure implementation)


  }

  //--- (generated code: YDataSet class start)
  /**
   * <summary>
   *   YDataSet objects make it possible to retrieve a set of recorded measures
   *   for a given sensor and a specified time interval.
   * <para>
   *   They can be used
   *   to load data points with a progress report. When the YDataSet object is
   *   instanciated by the <c>get_recordedData()</c>  function, no data is
   *   yet loaded from the module. It is only when the <c>loadMore()</c>
   *   method is called over and over than data will be effectively loaded
   *   from the dataLogger.
   * </para>
   * <para>
   *   A preview of available measures is available using the function
   *   <c>get_preview()</c> as soon as <c>loadMore()</c> has been called
   *   once. Measures themselves are available using function <c>get_measures()</c>
   *   when loaded by subsequent calls to <c>loadMore()</c>.
   * </para>
   * <para>
   *   This class can only be used on devices that use a recent firmware,
   *   as YDataSet objects are not supported by firmwares older than version 13000.
   * </para>
   * <para>
   * </para>
   * </summary>
   */
  public class YDataSet
  {
    //--- (end of generated code: YDataSet class start)
    //--- (generated code: YDataSet definitions)

    protected YFunction _parent;
    protected string _hardwareId;
    protected string _functionId;
    protected string _unit;
    protected long _startTime = 0;
    protected long _endTime = 0;
    protected int _progress = 0;
    protected List<int> _calib = new List<int>();
    protected List<YDataStream> _streams = new List<YDataStream>();
    protected YMeasure _summary;
    protected List<YMeasure> _preview = new List<YMeasure>();
    protected List<YMeasure> _measures = new List<YMeasure>();
    //--- (end of generated code: YDataSet definitions)

    // YDataSet constructor, when instantiated directly by a function
    public YDataSet(YFunction parent, string functionId, string unit, long startTime, long endTime)
    {
      //--- (generated code: YDataSet attributes initialization)
      //--- (end of generated code: YDataSet attributes initialization)
      this._parent = parent;
      this._functionId = functionId;
      this._unit = unit;
      this._startTime = startTime;
      this._endTime = endTime;
      this._progress = -1;
      this._summary = new YMeasure();
    }

    // YDataSet constructor for the new datalogger
    public YDataSet(YFunction parent, string data)
    {
      //--- (generated code: YDataSet attributes initialization)
      //--- (end of generated code: YDataSet attributes initialization)
      this._parent = parent;
      this._startTime = 0;
      this._endTime = 0;
      this._summary = new YMeasure();
      this._parse(data);
    }

    protected int _parse(string data)
    {
      YAPI.TJsonParser p;

      if (!YAPI.ExceptionsDisabled) p = new YAPI.TJsonParser(data, false);
      else try
        {
          p = new YAPI.TJsonParser(data, false);
        }
        catch
        {

          return YAPI.NOT_SUPPORTED;
        }

      Nullable<YAPI.TJSONRECORD> node, arr;
      YDataStream stream;
      double summaryMinVal = Double.MaxValue;
      double summaryMaxVal = -Double.MaxValue;
      double summaryTotalTime = 0;
      double summaryTotalAvg = 0;

      node = p.GetChildNode(null, "id");
      this._functionId = node.Value.svalue;
      node = p.GetChildNode(null, "unit");
      this._unit = node.Value.svalue;
      node = p.GetChildNode(null, "cal");
      this._calib = YAPI._decodeWords(node.Value.svalue);
      arr = p.GetChildNode(null, "streams");
      this._streams = new List<YDataStream>();
      this._preview = new List<YMeasure>();
      this._measures = new List<YMeasure>();
      for (int i = 0; i < arr.Value.itemcount; i++)
      {
        stream = _parent._findDataStream(this, arr.Value.items[i].svalue);
        if (_startTime > 0 && stream.get_startTimeUTC() + stream.get_duration() <= _startTime)
        {
          // this stream is too early, drop it
        }
        else if (_endTime > 0 && stream.get_startTimeUTC() > this._endTime)
        {
          // this stream is too late, drop it
        }
        else
        {
          _streams.Add(stream);
          if (stream.isClosed() && stream.get_startTimeUTC() >= this._startTime &&
             (this._endTime == 0 || stream.get_startTimeUTC() + stream.get_duration() <= this._endTime))
          {
            if (summaryMinVal > stream.get_minValue())
            {
              summaryMinVal = stream.get_minValue();
            }
            if (summaryMaxVal < stream.get_maxValue())
            {
              summaryMaxVal = stream.get_maxValue();
            }
            summaryTotalAvg += stream.get_averageValue() * stream.get_duration();
            summaryTotalTime += stream.get_duration();

            YMeasure rec = new YMeasure(
                                stream.get_startTimeUTC(),
                                stream.get_startTimeUTC() + stream.get_duration(),
                                stream.get_minValue(),
                                stream.get_averageValue(),
                                stream.get_maxValue());
            this._preview.Add(rec);
          }
        }
      }
      if ((this._streams.Count > 0) && (summaryTotalTime > 0))
      {
        // update time boundaries with actual data
        stream = this._streams[this._streams.Count - 1];
        UInt32 endtime = (UInt32)(stream.get_startTimeUTC() + stream.get_duration());
        UInt32 startTime = (UInt32)(this._streams[0].get_startTimeUTC() - stream.get_dataSamplesIntervalMs() / 1000);
        if (this._startTime < startTime)
        {
          this._startTime = startTime;
        }
        if (this._endTime == 0 || this._endTime > endtime)
        {
          this._endTime = endtime;
        }
        this._summary = new YMeasure(
                                    _startTime,
                                    _endTime,
                                    summaryMinVal,
                                    summaryTotalAvg / summaryTotalTime,
                                    summaryMaxVal);
      }
      this._progress = 0;
      return this.get_progress();
    }

    //--- (generated code: YDataSet implementation)


    public virtual List<int> get_calibration()
    {
      return this._calib;
    }

    public virtual int processMore(int progress, byte[] data)
    {
      YDataStream stream;
      List<List<double>> dataRows = new List<List<double>>();
      string strdata;
      double tim = 0;
      double itv = 0;
      int nCols = 0;
      int minCol = 0;
      int avgCol = 0;
      int maxCol = 0;
      // may throw an exception
      if (progress != this._progress)
      {
        return this._progress;
      }
      if (this._progress < 0)
      {
        strdata = YAPI.DefaultEncoding.GetString(data);
        if (strdata == "{}")
        {
          this._parent._throw(YAPI.VERSION_MISMATCH, "device firmware is too old");
          return YAPI.VERSION_MISMATCH;
        }
        return this._parse(strdata);
      }
      stream = this._streams[this._progress];
      stream.parse(data);
      dataRows = stream.get_dataRows();
      this._progress = this._progress + 1;
      if (dataRows.Count == 0)
      {
        return this.get_progress();
      }
      tim = (double)stream.get_startTimeUTC();
      itv = stream.get_dataSamplesInterval();
      nCols = dataRows[0].Count;
      minCol = 0;
      if (nCols > 2)
      {
        avgCol = 1;
      }
      else
      {
        avgCol = 0;
      }
      if (nCols > 2)
      {
        maxCol = 2;
      }
      else
      {
        maxCol = 0;
      }

      for (int ii = 0; ii < dataRows.Count; ii++)
      {
        if ((tim >= this._startTime) && ((this._endTime == 0) || (tim <= this._endTime)))
        {
          this._measures.Add(new YMeasure(tim - itv, tim, dataRows[ii][minCol], dataRows[ii][avgCol], dataRows[ii][maxCol]));
          tim = tim + itv;
        };
      }

      return this.get_progress();
    }

    public virtual List<YDataStream> get_privateDataStreams()
    {
      return this._streams;
    }

    /**
     * <summary>
     *   Returns the unique hardware identifier of the function who performed the measures,
     *   in the form <c>SERIAL.FUNCTIONID</c>.
     * <para>
     *   The unique hardware identifier is composed of the
     *   device serial number and of the hardware identifier of the function
     *   (for example <c>THRMCPL1-123456.temperature1</c>)
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   a string that uniquely identifies the function (ex: <c>THRMCPL1-123456.temperature1</c>)
     * </returns>
     * <para>
     *   On failure, throws an exception or returns  <c>YDataSet.HARDWAREID_INVALID</c>.
     * </para>
     */
    public virtual string get_hardwareId()
    {
      YModule mo;
      if (!(this._hardwareId == ""))
      {
        return this._hardwareId;
      }
      mo = this._parent.get_module();
      this._hardwareId = "" + mo.get_serialNumber() + "." + this.get_functionId();
      return this._hardwareId;
    }

    /**
     * <summary>
     *   Returns the hardware identifier of the function that performed the measure,
     *   without reference to the module.
     * <para>
     *   For example <c>temperature1</c>.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   a string that identifies the function (ex: <c>temperature1</c>)
     * </returns>
     */
    public virtual string get_functionId()
    {
      return this._functionId;
    }

    /**
     * <summary>
     *   Returns the measuring unit for the measured value.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   a string that represents a physical unit.
     * </returns>
     * <para>
     *   On failure, throws an exception or returns  <c>YDataSet.UNIT_INVALID</c>.
     * </para>
     */
    public virtual string get_unit()
    {
      return this._unit;
    }

    /**
     * <summary>
     *   Returns the start time of the dataset, relative to the Jan 1, 1970.
     * <para>
     *   When the YDataSet is created, the start time is the value passed
     *   in parameter to the <c>get_dataSet()</c> function. After the
     *   very first call to <c>loadMore()</c>, the start time is updated
     *   to reflect the timestamp of the first measure actually found in the
     *   dataLogger within the specified range.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   an unsigned number corresponding to the number of seconds
     *   between the Jan 1, 1970 and the beginning of this data
     *   set (i.e. Unix time representation of the absolute time).
     * </returns>
     */
    public virtual long get_startTimeUTC()
    {
      return this._startTime;
    }

    /**
     * <summary>
     *   Returns the end time of the dataset, relative to the Jan 1, 1970.
     * <para>
     *   When the YDataSet is created, the end time is the value passed
     *   in parameter to the <c>get_dataSet()</c> function. After the
     *   very first call to <c>loadMore()</c>, the end time is updated
     *   to reflect the timestamp of the last measure actually found in the
     *   dataLogger within the specified range.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   an unsigned number corresponding to the number of seconds
     *   between the Jan 1, 1970 and the end of this data
     *   set (i.e. Unix time representation of the absolute time).
     * </returns>
     */
    public virtual long get_endTimeUTC()
    {
      return this._endTime;
    }

    /**
     * <summary>
     *   Returns the progress of the downloads of the measures from the data logger,
     *   on a scale from 0 to 100.
     * <para>
     *   When the object is instanciated by <c>get_dataSet</c>,
     *   the progress is zero. Each time <c>loadMore()</c> is invoked, the progress
     *   is updated, to reach the value 100 only once all measures have been loaded.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   an integer in the range 0 to 100 (percentage of completion).
     * </returns>
     */
    public virtual int get_progress()
    {
      if (this._progress < 0)
      {
        return 0;
      }
      // index not yet loaded
      if (this._progress >= this._streams.Count)
      {
        return 100;
      }
      return ((1 + (1 + this._progress) * 98) / ((1 + this._streams.Count)));
    }

    /**
     * <summary>
     *   Loads the the next block of measures from the dataLogger, and updates
     *   the progress indicator.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   an integer in the range 0 to 100 (percentage of completion),
     *   or a negative error code in case of failure.
     * </returns>
     * <para>
     *   On failure, throws an exception or returns a negative error code.
     * </para>
     */
    public virtual int loadMore()
    {
      string url;
      YDataStream stream;
      if (this._progress < 0)
      {
        url = "logger.json?id=" + this._functionId;
      }
      else
      {
        if (this._progress >= this._streams.Count)
        {
          return 100;
        }
        else
        {
          stream = this._streams[this._progress];
          url = stream.get_url();
        }
      }
      return this.processMore(this._progress, this._parent._download(url));
    }

    /**
     * <summary>
     *   Returns an YMeasure object which summarizes the whole
     *   DataSet.
     * <para>
     *   In includes the following information:
     *   - the start of a time interval
     *   - the end of a time interval
     *   - the minimal value observed during the time interval
     *   - the average value observed during the time interval
     *   - the maximal value observed during the time interval
     * </para>
     * <para>
     *   This summary is available as soon as <c>loadMore()</c> has
     *   been called for the first time.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   an YMeasure object
     * </returns>
     */
    public virtual YMeasure get_summary()
    {
      return this._summary;
    }

    /**
     * <summary>
     *   Returns a condensed version of the measures that can
     *   retrieved in this YDataSet, as a list of YMeasure
     *   objects.
     * <para>
     *   Each item includes:
     *   - the start of a time interval
     *   - the end of a time interval
     *   - the minimal value observed during the time interval
     *   - the average value observed during the time interval
     *   - the maximal value observed during the time interval
     * </para>
     * <para>
     *   This preview is available as soon as <c>loadMore()</c> has
     *   been called for the first time.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   a table of records, where each record depicts the
     *   measured values during a time interval
     * </returns>
     * <para>
     *   On failure, throws an exception or returns an empty array.
     * </para>
     */
    public virtual List<YMeasure> get_preview()
    {
      return this._preview;
    }

    /**
     * <summary>
     *   Returns all measured values currently available for this DataSet,
     *   as a list of YMeasure objects.
     * <para>
     *   Each item includes:
     *   - the start of the measure time interval
     *   - the end of the measure time interval
     *   - the minimal value observed during the time interval
     *   - the average value observed during the time interval
     *   - the maximal value observed during the time interval
     * </para>
     * <para>
     *   Before calling this method, you should call <c>loadMore()</c>
     *   to load data from the device. You may have to call loadMore()
     *   several time until all rows are loaded, but you can start
     *   looking at available data rows before the load is complete.
     * </para>
     * <para>
     *   The oldest measures are always loaded first, and the most
     *   recent measures will be loaded last. As a result, timestamps
     *   are normally sorted in ascending order within the measure table,
     *   unless there was an unexpected adjustment of the datalogger UTC
     *   clock.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   a table of records, where each record depicts the
     *   measured value for a given time interval
     * </returns>
     * <para>
     *   On failure, throws an exception or returns an empty array.
     * </para>
     */
    public virtual List<YMeasure> get_measures()
    {
      return this._measures;
    }

    //--- (end of generated code: YDataSet implementation)
  }

  //
  // TYFunction Class (virtual class, used internally)
  //
  // This is the parent class for all public objects representing device functions documented in
  // the high-level programming API. This abstract class does all the real job, but without
  // knowledge of the specific function attributes.
  //
  // Instantiating a child class of YFunction does not cause any communication.
  // The instance simply keeps track of its function identifier, and will dynamically bind
  // to a matching device at the time it is really beeing used to read or set an attribute.
  // In order to allow true hot-plug replacement of one device by another, the binding stay
  // dynamic through the life of the object.
  //
  // The YFunction class implements a generic high-level cache for the attribute values of
  // the specified function, pre-parsed from the REST API string.
  //


  //--- (generated code: YFunction class start)
  /**
   * <summary>
   *   This is the parent class for all public objects representing device functions documented in
   *   the high-level programming API.
   * <para>
   *   This abstract class does all the real job, but without
   *   knowledge of the specific function attributes.
   * </para>
   * <para>
   *   Instantiating a child class of YFunction does not cause any communication.
   *   The instance simply keeps track of its function identifier, and will dynamically bind
   *   to a matching device at the time it is really being used to read or set an attribute.
   *   In order to allow true hot-plug replacement of one device by another, the binding stay
   *   dynamic through the life of the object.
   * </para>
   * <para>
   *   The YFunction class implements a generic high-level cache for the attribute values of
   *   the specified function, pre-parsed from the REST API string.
   * </para>
   * <para>
   * </para>
   * </summary>
   */
  public class YFunction
  {
    //--- (end of generated code: YFunction class start)
    //--- (generated code: YFunction definitions)
    public delegate void ValueCallback(YFunction func, string value);
    public delegate void TimedReportCallback(YFunction func, YMeasure measure);

    public const string LOGICALNAME_INVALID = YAPI.INVALID_STRING;
    public const string ADVERTISEDVALUE_INVALID = YAPI.INVALID_STRING;
    protected string _logicalName = LOGICALNAME_INVALID;
    protected string _advertisedValue = ADVERTISEDVALUE_INVALID;
    protected ValueCallback _valueCallbackFunction = null;
    protected ulong _cacheExpiration = 0;
    protected string _serial;
    protected string _funId;
    protected string _hwId;
    //--- (end of generated code: YFunction definitions)
    public static Dictionary<string, YFunction> _cache = new Dictionary<string, YFunction>();
    public static List<YFunction> _ValueCallbackList = new List<YFunction>();
    public static List<YSensor> _TimedReportCallbackList = new List<YSensor>();
    public static Dictionary<string, YAPI.yCalibrationHandler> _CalibHandlers = new Dictionary<string, YAPI.yCalibrationHandler>();

    public delegate void GenericUpdateCallback(YFunction func, string value);

    public const YFUN_DESCR FUNCTIONDESCRIPTOR_INVALID = -1;
    protected string _className;
    protected string _func;
    protected YRETCODE _lastErrorType;
    protected string _lastErrorMsg;
    protected YFUN_DESCR _fundescr;
    protected object _userData;
    private static Hashtable _dataStreams = new Hashtable();

    public YFunction(string func)
    {
      _className = "Function";
      //--- (generated code: YFunction attributes initialization)
      //--- (end of generated code: YFunction attributes initialization)        
      _func = func;
      _lastErrorType = YAPI.SUCCESS;
      _lastErrorMsg = "";
      _cacheExpiration = 0;
      _fundescr = FUNCTIONDESCRIPTOR_INVALID;
      _userData = null;
    }

    public void _throw(YRETCODE errType, string errMsg)
    {
      _lastErrorType = errType;
      _lastErrorMsg = errMsg;
      if (!(YAPI.ExceptionsDisabled))
      {
        throw new YAPI.YAPI_Exception(errType, "YoctoApi error : " + errMsg);
      }
    }


    // function cache methods
    static protected YFunction _FindFromCache(string classname, string func)
    {
      if (_cache.ContainsKey(classname + "_" + func))
        return _cache[classname + "_" + func];
      return null;
    }

    static protected void _AddToCache(string classname, string func, YFunction obj)
    {
      _cache[classname + "_" + func] = obj;
    }

    static void _ClearCache()
    {
      _cache.Clear();
    }


    static protected void _UpdateValueCallbackList(YFunction func, Boolean add)
    {
      if (add)
      {
        func.isOnline();
        if (!_ValueCallbackList.Contains(func))
        {
          _ValueCallbackList.Add(func);
        }
      }
      else
      {
        _ValueCallbackList.Remove(func);
      }
    }

    static protected void _UpdateTimedReportCallbackList(YSensor func, Boolean add)
    {
      if (add)
      {
        func.isOnline();
        if (!_TimedReportCallbackList.Contains(func))
        {
          _TimedReportCallbackList.Add(func);
        }
      }
      else
      {
        _TimedReportCallbackList.Remove(func);
      }
    }


    //  Method used to resolve our name to our unique function descriptor (may trigger a hub scan)
    protected YRETCODE _getDescriptor(ref YFUN_DESCR fundescr, ref string errMsg)
    {
      int res = 0;
      YFUN_DESCR tmp_fundescr;

      tmp_fundescr = YAPI.yapiGetFunction(_className, _func, ref errMsg);
      if (YAPI.YISERR(tmp_fundescr))
      {
        res = YAPI.yapiUpdateDeviceList(1, ref errMsg);
        if (YAPI.YISERR(res))
        {
          return res;
        }

        tmp_fundescr = YAPI.yapiGetFunction(_className, _func, ref errMsg);
        if (YAPI.YISERR(tmp_fundescr))
        {
          return tmp_fundescr;
        }
      }
      _fundescr = fundescr = tmp_fundescr;
      return YAPI.SUCCESS;
    }



    // Return a pointer to our device caching object (may trigger a hub scan)
    protected YRETCODE _getDevice(ref YAPI.YDevice dev, ref string errMsg)
    {
      YRETCODE functionReturnValue = default(YRETCODE);
      YFUN_DESCR fundescr = default(YFUN_DESCR);
      YDEV_DESCR devdescr = default(YDEV_DESCR);
      YRETCODE res = default(YRETCODE);

      // Resolve function name
      res = _getDescriptor(ref fundescr, ref errMsg);
      if ((YAPI.YISERR(res)))
      {
        functionReturnValue = res;
        return functionReturnValue;
      }

      // Get device descriptor
      devdescr = YAPI.yapiGetDeviceByFunction(fundescr, ref errMsg);
      if ((YAPI.YISERR(devdescr)))
      {
        return devdescr;
      }

      // Get device object
      dev = YAPI.YDevice.getDevice(devdescr);

      functionReturnValue = YAPI.SUCCESS;
      return functionReturnValue;
    }

    // Return the next known function of current class listed in the yellow pages
    protected YRETCODE _nextFunction(ref string hwid)
    {
      YRETCODE functionReturnValue = default(YRETCODE);

      YFUN_DESCR fundescr = default(YFUN_DESCR);
      YDEV_DESCR devdescr = default(YDEV_DESCR);
      string serial = "";
      string funcId = "";
      string funcName = "";
      string funcVal = "";
      string errmsg = "";
      int res = 0;
      int count = 0;
      int neededsize = 0;
      int maxsize = 0;
      IntPtr p = default(IntPtr);

      const int n_element = 1;
      int[] pdata = new int[1];

      res = _getDescriptor(ref fundescr, ref errmsg);
      if ((YAPI.YISERR(res)))
      {
        _throw(res, errmsg);
        functionReturnValue = res;
        return functionReturnValue;
      }

      maxsize = n_element * Marshal.SizeOf(pdata[0]);
      p = Marshal.AllocHGlobal(maxsize);
      res = YAPI.apiGetFunctionsByClass(_className, fundescr, p, maxsize, ref neededsize, ref errmsg);
      Marshal.Copy(p, pdata, 0, n_element);
      Marshal.FreeHGlobal(p);
      if ((YAPI.YISERR(res)))
      {
        _throw(res, errmsg);
        functionReturnValue = res;
        return functionReturnValue;
      }

      count = Convert.ToInt32(neededsize / Marshal.SizeOf(pdata[0]));
      if (count == 0)
      {
        hwid = "";
        functionReturnValue = YAPI.SUCCESS;
        return functionReturnValue;
      }

      res = YAPI.yapiGetFunctionInfo(pdata[0], ref devdescr, ref serial, ref funcId, ref funcName, ref funcVal, ref errmsg);

      if ((YAPI.YISERR(res)))
      {
        _throw(res, errmsg);
        functionReturnValue = YAPI.SUCCESS;
        return functionReturnValue;
      }

      hwid = serial + "." + funcId;
      functionReturnValue = YAPI.SUCCESS;
      return functionReturnValue;
    }

    private YRETCODE _buildSetRequest(string changeattr, string changeval, ref string request, ref string errmsg)
    {
      YRETCODE functionReturnValue = default(YRETCODE);

      int res = 0;
      int i = 0;
      YFUN_DESCR fundesc = default(YFUN_DESCR);
      StringBuilder funcid = new StringBuilder(YAPI.YOCTO_FUNCTION_LEN);
      StringBuilder errbuff = new StringBuilder(YAPI.YOCTO_ERRMSG_LEN);

      string uchangeval = null;
      string h = null;
      char c = '\0';
      YDEV_DESCR devdesc = default(YDEV_DESCR);

      funcid.Length = 0;
      errbuff.Length = 0;


      // Resolve the function name
      res = _getDescriptor(ref fundesc, ref errmsg);

      if ((YAPI.YISERR(res)))
      {
        functionReturnValue = res;
        return functionReturnValue;
      }

      res = YAPI._yapiGetFunctionInfo(fundesc, ref devdesc, null, funcid, null, null, errbuff);
      if (YAPI.YISERR(res))
      {
        errmsg = errbuff.ToString();
        _throw(res, errmsg);
        functionReturnValue = res;
        return functionReturnValue;
      }


      request = "GET /api/" + funcid.ToString() + "/";
      uchangeval = "";

      if (changeattr != "")
      {
        request = request + changeattr + "?" + changeattr + "=";
        for (i = 0; i <= changeval.Length - 1; i++)
        {
          c = changeval[i];
          if (c <= ' ' || (c > 'z' && c != '~') || c == '"' || c == '%' || c == '&' ||
                     c == '+' || c == '<' || c == '=' || c == '>' || c == '\\' || c == '^' || c == '`')
          {
            int hh = c;
            h = hh.ToString("X");
            if ((h.Length < 2))
              h = "0" + h;
            uchangeval = uchangeval + "%" + h;
          }
          else
          {
            uchangeval = uchangeval + c;
          }
        }
      }

      request = request + uchangeval + "&. \r\n\r\n";
      functionReturnValue = YAPI.SUCCESS;
      return functionReturnValue;
    }






    // Set an attribute in the function, and parse the resulting new function state
    protected YRETCODE _setAttr(string attrname, string newvalue)
    {
      string errmsg = "";
      string request = "";
      int res = 0;
      YAPI.YDevice dev = null;

      //  Execute http request
      res = _buildSetRequest(attrname, newvalue, ref request, ref errmsg);
      if (YAPI.YISERR(res))
      {
        _throw(res, errmsg);
        return res;
      }

      // Get device Object
      res = _getDevice(ref dev, ref errmsg);
      if (YAPI.YISERR(res))
      {
        _throw(res, errmsg);
        return res;
      }

      res = dev.HTTPRequestAsync(request, ref errmsg);
      if (YAPI.YISERR(res))
      {
        // make sure a device scan does not solve the issue
        res = YAPI.yapiUpdateDeviceList(1, ref errmsg);
        if (YAPI.YISERR(res))
        {
          _throw(res, errmsg);
          return res;
        }
        res = dev.HTTPRequestAsync(request, ref errmsg);
        if (YAPI.YISERR(res))
        {
          _throw(res, errmsg);
          return res;
        }
      }
      if (_cacheExpiration != 0)
      {
        _cacheExpiration = YAPI.GetTickCount();
      }
      return YAPI.SUCCESS;
    }

    // Method used to send http request to the device (not the function)
    protected byte[] _request(string request)
    {
      return this._request(YAPI.DefaultEncoding.GetBytes(request));
    }

    // Method used to send http request to the device (not the function)
    protected byte[] _request(byte[] request)
    {
      YAPI.YDevice dev = null;
      string errmsg = "";
      byte[] buffer = null;
      byte[] check;
      int res;

      // Resolve our reference to our device, load REST API
      res = _getDevice(ref dev, ref errmsg);
      if (YAPI.YISERR(res))
      {
        _throw(res, errmsg);
        return new byte[0];
      }
      res = dev.HTTPRequest(request, ref buffer, ref errmsg);
      if (YAPI.YISERR(res))
      { // Check if an update of the device list does notb solve the issue
        res = YAPI.yapiUpdateDeviceList(1, ref errmsg);
        if (YAPI.YISERR(res))
        {
          _throw(res, errmsg);
          return new byte[0];
        }
        res = dev.HTTPRequest(request, ref buffer, ref errmsg);
        if (YAPI.YISERR(res))
        {
          _throw(res, errmsg);
          return new byte[0];
        }
      }
      if (buffer.Length >= 4)
      {
        check = new byte[4];
        Buffer.BlockCopy(buffer, 0, check, 0, 4);
        if (YAPI.DefaultEncoding.GetString(check) == "OK\r\n")
        {
          return buffer;
        }
        if (buffer.Length >= 17)
        {
          check = new byte[17];
          Buffer.BlockCopy(buffer, 0, check, 0, 17);
          if (YAPI.DefaultEncoding.GetString(check) == "HTTP/1.1 200 OK\r\n")
          {
            return buffer;
          }
        }
      }
      _throw(YAPI.IO_ERROR, "http request failed");
      return new byte[0];
    }

    // Method used to send http request to the device (not the function)
    public byte[] _download(string path)
    {
      string request;
      byte[] buffer, res;
      int found, body;
      request = "GET /" + path + " HTTP/1.1\r\n\r\n";
      buffer = _request(request);
      if (buffer.Length == 0) return buffer;
      for (found = 0; found < buffer.Length - 4; found++)
      {
        if (buffer[found] == 13 && buffer[found + 1] == 10 && buffer[found + 2] == 13 && buffer[found + 3] == 10)
          break;
      }
      if (found >= buffer.Length - 4)
      {
        _throw(YAPI.IO_ERROR, "http request failed");
        return new byte[0];
      }
      body = found + 4;
      res = new byte[buffer.Length - body];
      Buffer.BlockCopy(buffer, body, res, 0, buffer.Length - body);
      return res;
    }

    // Method used to upload a file to the device
    public int _upload(string path, string content)
    {
      return this._upload(path, YAPI.DefaultEncoding.GetBytes(content));
    }

    // Method used to upload a file to the device
    public int _upload(string path, List<byte> content)
    {
      return this._upload(path, content.ToArray());
    }

    // Method used to upload a file to the device
    public int _upload(string path, byte[] content)
    {
      string bodystr, boundary;
      byte[] body, bb, header, footer, fullrequest, buffer;

      bodystr = "Content-Disposition: form-data; name=\"" + path + "\"; filename=\"api\"\r\n" +
                "Content-Type: application/octet-stream\r\n" +
                "Content-Transfer-Encoding: binary\r\n\r\n";
      body = new byte[bodystr.Length + content.Length];
      Buffer.BlockCopy(YAPI.DefaultEncoding.GetBytes(bodystr), 0, body, 0, bodystr.Length);
      Buffer.BlockCopy(content, 0, body, bodystr.Length, content.Length);

      Random random = new Random();
      int pos, i;
      do
      {
        boundary = "Zz" + ((int)random.Next(100000, 999999)).ToString() + "zZ";
        bb = YAPI.DefaultEncoding.GetBytes(boundary);
        pos = 0;
        while (pos <= body.Length - bb.Length)
        {
          if (body[pos] == 90) // 'Z'
          {
            i = 1;
            while (i < bb.Length && body[pos + i] == bb[i]) i++;
            if (i >= bb.Length) break;
            pos += i;
          }
          else pos++;
        }
      }
      while (pos <= body.Length - bb.Length);

      header = YAPI.DefaultEncoding.GetBytes("POST /upload.html HTTP/1.1\r\n" +
                                              "Content-Type: multipart/form-data, boundary=" + boundary + "\r\n" +
                                              "\r\n--" + boundary + "\r\n");
      footer = YAPI.DefaultEncoding.GetBytes("\r\n--" + boundary + "--\r\n");
      fullrequest = new byte[header.Length + body.Length + footer.Length];
      Buffer.BlockCopy(header, 0, fullrequest, 0, header.Length);
      Buffer.BlockCopy(body, 0, fullrequest, header.Length, body.Length);
      Buffer.BlockCopy(footer, 0, fullrequest, header.Length + body.Length, footer.Length);

      buffer = _request(fullrequest);
      if (buffer.Length == 0)
      {
        _throw(YAPI.IO_ERROR, "http request failed");
        return YAPI.IO_ERROR;
      }
      return YAPI.SUCCESS;
    }

    // Method used to cache DataStream objects (new DataLogger)
    public YDataStream _findDataStream(YDataSet dataset, string def)
    {
      string key = dataset.get_functionId() + ":" + def;
      if (_dataStreams.ContainsKey(key))
        return (YDataStream)_dataStreams[key];

      YDataStream newDataStream = new YDataStream(this, dataset, YAPI._decodeWords(def));
      _dataStreams.Add(key, newDataStream);
      return newDataStream;
    }


    protected int _parse(YAPI.TJSONRECORD j)
    {
      int i = 0;
      if ((j.recordtype != YAPI.TJSONRECORDTYPE.JSON_STRUCT)) return -1;
      for (i = 0; i < j.membercount; i++)
      {
        _parseAttr(j.members[i]);
      }
      _parserHelper();
      return 0;
    }


    //--- (generated code: YFunction implementation)

    protected virtual void _parseAttr(YAPI.TJSONRECORD member)
    {
      if (member.name == "logicalName")
      {
        _logicalName = member.svalue;
        return;
      }
      if (member.name == "advertisedValue")
      {
        _advertisedValue = member.svalue;
        return;
      }
    }

    /**
     * <summary>
     *   Returns the logical name of the function.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   a string corresponding to the logical name of the function
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YFunction.LOGICALNAME_INVALID</c>.
     * </para>
     */
    public string get_logicalName()
    {
      if (this._cacheExpiration <= YAPI.GetTickCount())
      {
        if (this.load(YAPI.DefaultCacheValidity) != YAPI.SUCCESS)
        {
          return LOGICALNAME_INVALID;
        }
      }
      return this._logicalName;
    }

    /**
     * <summary>
     *   Changes the logical name of the function.
     * <para>
     *   You can use <c>yCheckLogicalName()</c>
     *   prior to this call to make sure that your parameter is valid.
     *   Remember to call the <c>saveToFlash()</c> method of the module if the
     *   modification must be kept.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="newval">
     *   a string corresponding to the logical name of the function
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
    public int set_logicalName(string newval)
    {
      string rest_val;
      rest_val = newval;
      return _setAttr("logicalName", rest_val);
    }

    /**
     * <summary>
     *   Returns the current value of the function (no more than 6 characters).
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   a string corresponding to the current value of the function (no more than 6 characters)
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YFunction.ADVERTISEDVALUE_INVALID</c>.
     * </para>
     */
    public string get_advertisedValue()
    {
      if (this._cacheExpiration <= YAPI.GetTickCount())
      {
        if (this.load(YAPI.DefaultCacheValidity) != YAPI.SUCCESS)
        {
          return ADVERTISEDVALUE_INVALID;
        }
      }
      return this._advertisedValue;
    }

    /**
     * <summary>
     *   Retrieves a function for a given identifier.
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
     *   This function does not require that the function is online at the time
     *   it is invoked. The returned object is nevertheless valid.
     *   Use the method <c>YFunction.isOnline()</c> to test if the function is
     *   indeed online at a given time. In case of ambiguity when looking for
     *   a function by logical name, no error is notified: the first instance
     *   found is returned. The search is performed first by hardware name,
     *   then by logical name.
     * </para>
     * </summary>
     * <param name="func">
     *   a string that uniquely characterizes the function
     * </param>
     * <returns>
     *   a <c>YFunction</c> object allowing you to drive the function.
     * </returns>
     */
    public static YFunction FindFunction(string func)
    {
      YFunction obj;
      obj = (YFunction)YFunction._FindFromCache("Function", func);
      if (obj == null)
      {
        obj = new YFunction(func);
        YFunction._AddToCache("Function", func, obj);
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
    public virtual int registerValueCallback(ValueCallback callback)
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
      this._valueCallbackFunction = callback;
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

    public virtual int _invokeValueCallback(string value)
    {
      if (this._valueCallbackFunction != null)
      {
        this._valueCallbackFunction(this, value);
      }
      else
      {
      }
      return 0;
    }

    public virtual int _parserHelper()
    {
      return 0;
    }

    /**
     * <summary>
     *   c
     * <para>
     *   omment from .yc definition
     * </para>
     * </summary>
     */
    public YFunction nextFunction()
    {
      string hwid = "";
      if (YAPI.YISERR(_nextFunction(ref hwid)))
        return null;
      if (hwid == "")
        return null;
      return FindFunction(hwid);
    }

    //--- (end of generated code: YFunction implementation)

    //--- (generated code: Function functions)

    /**
     * <summary>
     *   c
     * <para>
     *   omment from .yc definition
     * </para>
     * </summary>
     */
    public static YFunction FirstFunction()
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
      err = YAPI.apiGetFunctionsByClass("Function", 0, p, size, ref neededsize, ref errmsg);
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
      return FindFunction(serial + "." + funcId);
    }



    //--- (end of generated code: Function functions)



    /**
     * <summary>
     *   Returns a short text that describes unambiguously the instance of the function in the form <c>TYPE(NAME)=SERIAL&#46;FUNCTIONID</c>.
     * <para>
     *   More precisely,
     *   <c>TYPE</c>       is the type of the function,
     *   <c>NAME</c>       it the name used for the first access to the function,
     *   <c>SERIAL</c>     is the serial number of the module if the module is connected or <c>"unresolved"</c>, and
     *   <c>FUNCTIONID</c> is  the hardware identifier of the function if the module is connected.
     *   For example, this method returns <c>Relay(MyCustomName.relay1)=RELAYLO1-123456.relay1</c> if the
     *   module is already connected or <c>Relay(BadCustomeName.relay1)=unresolved</c> if the module has
     *   not yet been connected. This method does not trigger any USB or TCP transaction and can therefore be used in
     *   a debugger.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   a string that describes the function
     *   (ex: <c>Relay(MyCustomName.relay1)=RELAYLO1-123456.relay1</c>)
     * </returns>
     */
    public string describe()
    {
      YFUN_DESCR fundescr = default(YFUN_DESCR);
      YDEV_DESCR devdescr = default(YDEV_DESCR);
      string errmsg = "";
      string serial = "";
      string funcId = "";
      string funcName = "";
      string funcValue = "";
      fundescr = YAPI.yapiGetFunction(_className, _func, ref errmsg);
      if ((!(YAPI.YISERR(fundescr))))
      {
        if ((!(YAPI.YISERR(YAPI.yapiGetFunctionInfo(fundescr, ref devdescr, ref serial, ref funcId, ref funcName, ref funcValue, ref errmsg)))))
        {
          return _className + "(" + _func + ")=" + serial + "." + funcId;
        }
      }
      return _className + "(" + _func + ")=unresolved";
    }


    /**
     * <summary>
     *   Returns the unique hardware identifier of the function in the form <c>SERIAL.FUNCTIONID</c>.
     * <para>
     *   The unique hardware identifier is composed of the device serial
     *   number and of the hardware identifier of the function (for example <c>RELAYLO1-123456.relay1</c>).
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   a string that uniquely identifies the function (ex: <c>RELAYLO1-123456.relay1</c>)
     * </returns>
     * <para>
     *   On failure, throws an exception or returns  <c>YFunction.HARDWAREID_INVALID</c>.
     * </para>
     */

    public string get_hardwareId()
    {

      YRETCODE retcode;
      YFUN_DESCR fundesc = 0;
      YDEV_DESCR devdesc = 0;
      string funcName = "";
      string funcVal = "";
      string errmsg = "";
      string snum = "";
      string funcid = "";



      // Resolve the function name
      retcode = _getDescriptor(ref fundesc, ref errmsg);
      if (YAPI.YISERR(retcode))
      {
        _throw(retcode, errmsg);
        return YAPI.HARDWAREID_INVALID;
      }

      retcode = YAPI.yapiGetFunctionInfo(fundesc, ref devdesc, ref snum, ref funcid, ref funcName, ref funcVal, ref errmsg);
      if (YAPI.YISERR(retcode))
      {
        _throw(retcode, errmsg);
        return YAPI.HARDWAREID_INVALID;
      }

      return snum + '.' + funcid;
    }


    /**
     * <summary>
     *   Returns the hardware identifier of the function, without reference to the module.
     * <para>
     *   For example
     *   <c>relay1</c>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   a string that identifies the function (ex: <c>relay1</c>)
     * </returns>
     * <para>
     *   On failure, throws an exception or returns  <c>YFunction.FUNCTIONID_INVALID</c>.
     * </para>
     */

    public string get_functionId()
    {

      YRETCODE retcode;
      YFUN_DESCR fundesc = 0;
      YDEV_DESCR devdesc = 0;
      string funcName = "";
      string funcVal = "";
      string errmsg = "";
      string snum = "";
      string funcid = "";



      // Resolve the function name
      retcode = _getDescriptor(ref fundesc, ref errmsg);
      if (YAPI.YISERR(retcode))
      {
        _throw(retcode, errmsg);
        return YAPI.FUNCTIONID_INVALID;
      }

      retcode = YAPI.yapiGetFunctionInfo(fundesc, ref devdesc, ref snum, ref funcid, ref funcName, ref funcVal, ref errmsg);
      if (YAPI.YISERR(retcode))
      {
        _throw(retcode, errmsg);
        return YAPI.FUNCTIONID_INVALID;
      }

      return funcid;
    }


    public string FriendlyName
    {
      get { return this.get_friendlyName(); }
    }

    /**
     * <summary>
     *   Returns a global identifier of the function in the format <c>MODULE_NAME&#46;FUNCTION_NAME</c>.
     * <para>
     *   The returned string uses the logical names of the module and of the function if they are defined,
     *   otherwise the serial number of the module and the hardware identifier of the function
     *   (for exemple: <c>MyCustomName.relay1</c>)
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   a string that uniquely identifies the function using logical names
     *   (ex: <c>MyCustomName.relay1</c>)
     * </returns>
     * <para>
     *   On failure, throws an exception or returns  <c>YFunction.FRIENDLYNAME_INVALID</c>.
     * </para>
     */

    public virtual string get_friendlyName()
    {

      YRETCODE retcode;
      YFUN_DESCR fundesc = 0;
      YDEV_DESCR devdesc = 0;
      YDEV_DESCR moddescr = 0;
      string funcName = "";
      string dummy = "";
      string errmsg = "";
      string snum = "";
      string funcid = "";
      string friendly = "";
      string mod_name = "";

      // Resolve the function name
      retcode = _getDescriptor(ref fundesc, ref errmsg);
      if (YAPI.YISERR(retcode))
      {
        _throw(retcode, errmsg);
        return YAPI.FRIENDLYNAME_INVALID;
      }

      retcode = YAPI.yapiGetFunctionInfo(fundesc, ref devdesc, ref snum, ref funcid, ref funcName, ref dummy, ref errmsg);
      if (YAPI.YISERR(retcode))
      {
        _throw(retcode, errmsg);
        return YAPI.FRIENDLYNAME_INVALID;
      }

      moddescr = YAPI.yapiGetFunction("Module", snum, ref errmsg);
      if (YAPI.YISERR(moddescr))
      {
        _throw(retcode, errmsg);
        return YAPI.FRIENDLYNAME_INVALID;
      }

      retcode = YAPI.yapiGetFunctionInfo(moddescr, ref devdesc, ref snum, ref dummy, ref mod_name, ref dummy, ref errmsg);
      if (YAPI.YISERR(retcode))
      {
        _throw(retcode, errmsg);
        return YAPI.FRIENDLYNAME_INVALID;
      }

      if (mod_name != "")
      {
        friendly = mod_name + '.';
      }
      else
      {
        friendly = snum + '.';
      }
      if (funcName != "")
      {
        friendly += funcName;
      }
      else
      {
        friendly += funcid;
      }
      return friendly;

    }


    public override string ToString()
    {

      return describe();
    }

    /**
     * <summary>
     *   Returns the numerical error code of the latest error with the function.
     * <para>
     *   This method is mostly useful when using the Yoctopuce library with
     *   exceptions disabled.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   a number corresponding to the code of the latest error that occured while
     *   using the function object
     * </returns>
     */
    public YRETCODE get_errorType()
    {
      return _lastErrorType;
    }
    public YRETCODE errorType()
    {
      return _lastErrorType;
    }
    public YRETCODE errType()
    {
      return _lastErrorType;
    }

    /**
     * <summary>
     *   Returns the error message of the latest error with the function.
     * <para>
     *   This method is mostly useful when using the Yoctopuce library with
     *   exceptions disabled.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   a string corresponding to the latest error message that occured while
     *   using the function object
     * </returns>
     */
    public string get_errorMessage()
    {
      return _lastErrorMsg;
    }
    public string errorMessage()
    {
      return _lastErrorMsg;
    }
    public string errMessage()
    {
      return _lastErrorMsg;
    }

    /**
     * <summary>
     *   Checks if the function is currently reachable, without raising any error.
     * <para>
     *   If there is a cached value for the function in cache, that has not yet
     *   expired, the device is considered reachable.
     *   No exception is raised if there is an error while trying to contact the
     *   device hosting the function.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   <c>true</c> if the function can be reached, and <c>false</c> otherwise
     * </returns>
     */
    public bool isOnline()
    {
      YAPI.YDevice dev = null;
      string errmsg = "";
      YAPI.TJsonParser apires;

      //  A valid value in cache means that the device is online
      if (_cacheExpiration > YAPI.GetTickCount())
      {
        return true;
      }

      // Check that the function is available, without throwing exceptions
      if (YAPI.YISERR(_getDevice(ref dev, ref errmsg)))
      {
        return false;
      }

      // Try to execute a function request to be positively sure that the device is ready
      if (YAPI.YISERR(dev.requestAPI(out apires, ref errmsg)))
      {
        return false;
      }

      // Preload the function data, since we have it in device cache
      load(YAPI.DefaultCacheValidity);
      return true;
    }

    protected string _json_get_key(byte[] data, string key)
    {
      Nullable<YAPI.TJSONRECORD> node;

      string st = YAPI.DefaultEncoding.GetString(data);
      YAPI.TJsonParser p;

      if (!YAPI.ExceptionsDisabled) p = new YAPI.TJsonParser(st, false);
      else try
        {
          p = new YAPI.TJsonParser(st, false);
        }
        catch
        {
          return "";
        }




      node = p.GetChildNode(null, key);

      return node.Value.svalue;
    }

    protected List<string> _json_get_array(byte[] data)
    {
      string st = YAPI.DefaultEncoding.GetString(data);
      YAPI.TJsonParser p;

      if (!YAPI.ExceptionsDisabled) p = new YAPI.TJsonParser(st, false);
      else try
        {
          p = new YAPI.TJsonParser(st, false);
        }
        catch
        {

          return null;
        }


      return p.GetAllChilds(null);
    }

    public string _json_get_string(byte[] data)
    {
      Nullable<YAPI.TJSONRECORD> node;
      string json_str = YAPI.DefaultEncoding.GetString(data);
      YAPI.TJsonParser p = new YAPI.TJsonParser('[' + json_str + ']', false);
      node = p.GetRootNode();
      return node.Value.items[0].svalue;
    }

    /**
     * <summary>
     *   Preloads the function cache with a specified validity duration.
     * <para>
     *   By default, whenever accessing a device, all function attributes
     *   are kept in cache for the standard duration (5 ms). This method can be
     *   used to temporarily mark the cache as valid for a longer period, in order
     *   to reduce network trafic for instance.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="msValidity">
     *   an integer corresponding to the validity attributed to the
     *   loaded function parameters, in milliseconds
     * </param>
     * <returns>
     *   <c>YAPI.SUCCESS</c> when the call succeeds.
     * </returns>
     * <para>
     *   On failure, throws an exception or returns a negative error code.
     * </para>
     */
    public YRETCODE load(int msValidity)
    {
      YRETCODE functionReturnValue = default(YRETCODE);

      YAPI.YDevice dev = null;
      string errmsg = "";
      YAPI.TJsonParser apires = null;
      YFUN_DESCR fundescr = default(YFUN_DESCR);
      int res = 0;
      string errbuf = "";
      string funcId = "";
      YDEV_DESCR devdesc = default(YDEV_DESCR);
      string serial = "";
      string funcName = "";
      string funcVal = "";
      Nullable<YAPI.TJSONRECORD> node = default(Nullable<YAPI.TJSONRECORD>);

      // Resolve our reference to our device, load REST API
      res = _getDevice(ref dev, ref errmsg);
      if ((YAPI.YISERR(res)))
      {
        _throw(res, errmsg);
        functionReturnValue = res;
        return functionReturnValue;
      }

      res = dev.requestAPI(out apires, ref errmsg);
      if (YAPI.YISERR(res))
      {
        _throw(res, errmsg);
        functionReturnValue = res;
        return functionReturnValue;
      }

      // Get our function Id
      fundescr = YAPI.yapiGetFunction(_className, _func, ref errmsg);
      if (YAPI.YISERR(fundescr))
      {
        _throw(res, errmsg);
        functionReturnValue = fundescr;
        return functionReturnValue;
      }

      devdesc = 0;
      res = YAPI.yapiGetFunctionInfo(fundescr, ref devdesc, ref serial, ref funcId, ref funcName, ref funcVal, ref errbuf);
      if (YAPI.YISERR(res))
      {
        _throw(res, errmsg);
        functionReturnValue = res;
        return functionReturnValue;
      }
      _cacheExpiration = YAPI.GetTickCount() + (ulong)msValidity;
      _serial = serial;
      _funId = funcId;
      _hwId = _serial + '.' + _funId;

      node = apires.GetChildNode(null, funcId);
      if (!node.HasValue)
      {
        _throw(YAPI.IO_ERROR, "unexpected JSON structure: missing function " + funcId);
        functionReturnValue = YAPI.IO_ERROR;
        return functionReturnValue;
      }

      _parse(node.GetValueOrDefault());
      functionReturnValue = YAPI.SUCCESS;
      return functionReturnValue;
    }

    /**
     * <summary>
     *   Gets the <c>YModule</c> object for the device on which the function is located.
     * <para>
     *   If the function cannot be located on any module, the returned instance of
     *   <c>YModule</c> is not shown as on-line.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   an instance of <c>YModule</c>
     * </returns>
     */
    public YModule get_module()
    {
      YFUN_DESCR fundescr = default(YFUN_DESCR);
      YDEV_DESCR devdescr = default(YDEV_DESCR);
      string errmsg = "";
      string serial = "";
      string funcId = "";
      string funcName = "";
      string funcValue = "";

      fundescr = YAPI.yapiGetFunction(_className, _func, ref errmsg);
      if ((!(YAPI.YISERR(fundescr))))
      {
        if ((!(YAPI.YISERR(YAPI.yapiGetFunctionInfo(fundescr, ref devdescr, ref serial, ref funcId, ref funcName, ref funcValue, ref errmsg)))))
        {
          return YModule.FindModule(serial + ".module");

        }
      }

      // return a true YModule object even if it is not a module valid for communicating
      return YModule.FindModule("module_of_" + _className + "_" + _func);
    }

    public YModule module()
    {
      return get_module();
    }
    /**
     * <summary>
     *   Returns a unique identifier of type <c>YFUN_DESCR</c> corresponding to the function.
     * <para>
     *   This identifier can be used to test if two instances of <c>YFunction</c> reference the same
     *   physical function on the same physical device.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   an identifier of type <c>YFUN_DESCR</c>.
     * </returns>
     * <para>
     *   If the function has never been contacted, the returned value is <c>YFunction.FUNCTIONDESCRIPTOR_INVALID</c>.
     * </para>
     */
    public YFUN_DESCR get_functionDescriptor()
    {
      return _fundescr;
    }

    public YFUN_DESCR functionDescriptor()
    { return get_functionDescriptor(); }


    /**
     * <summary>
     *   Returns the value of the userData attribute, as previously stored using method
     *   <c>set_userData</c>.
     * <para>
     *   This attribute is never touched directly by the API, and is at disposal of the caller to
     *   store a context.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   the object stored previously by the caller.
     * </returns>
     */
    public object get_userData()
    {
      return _userData;
    }
    public object userData()
    { return get_userData(); }

    /**
     * <summary>
     *   Stores a user context provided as argument in the userData attribute of the function.
     * <para>
     *   This attribute is never touched by the API, and is at disposal of the caller to store a context.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="data">
     *   any kind of object to be stored
     * @noreturn
     * </param>
     */
    public void set_userData(object data)
    {
      _userData = data;
    }
    public void setUserData(object data)
    { set_userData(data); }




  }


  //--- (generated code: YModule class start)
  /**
   * <summary>
   *   This interface is identical for all Yoctopuce USB modules.
   * <para>
   *   It can be used to control the module global parameters, and
   *   to enumerate the functions provided by each module.
   * </para>
   * <para>
   * </para>
   * </summary>
   */
  public class YModule : YFunction
  {
    //--- (end of generated code: YModule class start)
    //--- (generated code: YModule definitions)
    public delegate void LogCallback(YModule module, string logline);
    public new delegate void ValueCallback(YModule func, string value);
    public new delegate void TimedReportCallback(YModule func, YMeasure measure);

    public const string PRODUCTNAME_INVALID = YAPI.INVALID_STRING;
    public const string SERIALNUMBER_INVALID = YAPI.INVALID_STRING;
    public const int PRODUCTID_INVALID = YAPI.INVALID_UINT;
    public const int PRODUCTRELEASE_INVALID = YAPI.INVALID_UINT;
    public const string FIRMWARERELEASE_INVALID = YAPI.INVALID_STRING;
    public const int PERSISTENTSETTINGS_LOADED = 0;
    public const int PERSISTENTSETTINGS_SAVED = 1;
    public const int PERSISTENTSETTINGS_MODIFIED = 2;
    public const int PERSISTENTSETTINGS_INVALID = -1;

    public const int LUMINOSITY_INVALID = YAPI.INVALID_UINT;
    public const int BEACON_OFF = 0;
    public const int BEACON_ON = 1;
    public const int BEACON_INVALID = -1;

    public const long UPTIME_INVALID = YAPI.INVALID_LONG;
    public const int USBCURRENT_INVALID = YAPI.INVALID_UINT;
    public const int REBOOTCOUNTDOWN_INVALID = YAPI.INVALID_INT;
    public const int USBBANDWIDTH_SIMPLE = 0;
    public const int USBBANDWIDTH_DOUBLE = 1;
    public const int USBBANDWIDTH_INVALID = -1;

    protected string _productName = PRODUCTNAME_INVALID;
    protected string _serialNumber = SERIALNUMBER_INVALID;
    protected int _productId = PRODUCTID_INVALID;
    protected int _productRelease = PRODUCTRELEASE_INVALID;
    protected string _firmwareRelease = FIRMWARERELEASE_INVALID;
    protected int _persistentSettings = PERSISTENTSETTINGS_INVALID;
    protected int _luminosity = LUMINOSITY_INVALID;
    protected int _beacon = BEACON_INVALID;
    protected long _upTime = UPTIME_INVALID;
    protected int _usbCurrent = USBCURRENT_INVALID;
    protected int _rebootCountdown = REBOOTCOUNTDOWN_INVALID;
    protected int _usbBandwidth = USBBANDWIDTH_INVALID;
    protected ValueCallback _valueCallbackModule = null;
    protected LogCallback _logCallback = null;
    //--- (end of generated code: YModule definitions)

    public YModule(string func)
      : base(func)
    {
      _className = "Module";
      //--- (generated code: YModule attributes initialization)
      //--- (end of generated code: YModule attributes initialization)
    }

    /**
     * <summary>
     *   Registers a device log callback function.
     * <para>
     *   This callback will be called each time
     *   that a module sends a new log message. Mostly useful to debug a Yoctopuce module.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="callback">
     *   the callback function to call, or a null pointer. The callback function should take two
     *   arguments: the module object that emitted the log message, and the character string containing the log.
     * @noreturn
     * </param>
     */
    public int registerLogCallback(LogCallback callback)
    {
      _logCallback = callback;
      if (_logCallback == null)
      {
        YAPI._yapiStartStopDeviceLogCallback(new StringBuilder(_serial), 0);
      }
      else
      {
        YAPI._yapiStartStopDeviceLogCallback(new StringBuilder(_serial), 1);
      }
      return YAPI.SUCCESS;
    }

    public LogCallback get_logCallback()
    {
      return _logCallback;
    }

    //--- (generated code: YModule implementation)

    protected override void _parseAttr(YAPI.TJSONRECORD member)
    {
      if (member.name == "productName")
      {
        _productName = member.svalue;
        return;
      }
      if (member.name == "serialNumber")
      {
        _serialNumber = member.svalue;
        return;
      }
      if (member.name == "productId")
      {
        _productId = (int)member.ivalue;
        return;
      }
      if (member.name == "productRelease")
      {
        _productRelease = (int)member.ivalue;
        return;
      }
      if (member.name == "firmwareRelease")
      {
        _firmwareRelease = member.svalue;
        return;
      }
      if (member.name == "persistentSettings")
      {
        _persistentSettings = (int)member.ivalue;
        return;
      }
      if (member.name == "luminosity")
      {
        _luminosity = (int)member.ivalue;
        return;
      }
      if (member.name == "beacon")
      {
        _beacon = member.ivalue > 0 ? 1 : 0;
        return;
      }
      if (member.name == "upTime")
      {
        _upTime = member.ivalue;
        return;
      }
      if (member.name == "usbCurrent")
      {
        _usbCurrent = (int)member.ivalue;
        return;
      }
      if (member.name == "rebootCountdown")
      {
        _rebootCountdown = (int)member.ivalue;
        return;
      }
      if (member.name == "usbBandwidth")
      {
        _usbBandwidth = (int)member.ivalue;
        return;
      }
      base._parseAttr(member);
    }

    /**
     * <summary>
     *   Returns the commercial name of the module, as set by the factory.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   a string corresponding to the commercial name of the module, as set by the factory
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YModule.PRODUCTNAME_INVALID</c>.
     * </para>
     */
    public string get_productName()
    {
      if (this._cacheExpiration == 0)
      {
        if (this.load(YAPI.DefaultCacheValidity) != YAPI.SUCCESS)
        {
          return PRODUCTNAME_INVALID;
        }
      }
      return this._productName;
    }

    /**
     * <summary>
     *   Returns the serial number of the module, as set by the factory.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   a string corresponding to the serial number of the module, as set by the factory
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YModule.SERIALNUMBER_INVALID</c>.
     * </para>
     */
    public string get_serialNumber()
    {
      if (this._cacheExpiration == 0)
      {
        if (this.load(YAPI.DefaultCacheValidity) != YAPI.SUCCESS)
        {
          return SERIALNUMBER_INVALID;
        }
      }
      return this._serialNumber;
    }

    /**
     * <summary>
     *   Returns the USB device identifier of the module.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   an integer corresponding to the USB device identifier of the module
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YModule.PRODUCTID_INVALID</c>.
     * </para>
     */
    public int get_productId()
    {
      if (this._cacheExpiration == 0)
      {
        if (this.load(YAPI.DefaultCacheValidity) != YAPI.SUCCESS)
        {
          return PRODUCTID_INVALID;
        }
      }
      return this._productId;
    }

    /**
     * <summary>
     *   Returns the hardware release version of the module.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   an integer corresponding to the hardware release version of the module
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YModule.PRODUCTRELEASE_INVALID</c>.
     * </para>
     */
    public int get_productRelease()
    {
      if (this._cacheExpiration == 0)
      {
        if (this.load(YAPI.DefaultCacheValidity) != YAPI.SUCCESS)
        {
          return PRODUCTRELEASE_INVALID;
        }
      }
      return this._productRelease;
    }

    /**
     * <summary>
     *   Returns the version of the firmware embedded in the module.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   a string corresponding to the version of the firmware embedded in the module
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YModule.FIRMWARERELEASE_INVALID</c>.
     * </para>
     */
    public string get_firmwareRelease()
    {
      if (this._cacheExpiration <= YAPI.GetTickCount())
      {
        if (this.load(YAPI.DefaultCacheValidity) != YAPI.SUCCESS)
        {
          return FIRMWARERELEASE_INVALID;
        }
      }
      return this._firmwareRelease;
    }

    /**
     * <summary>
     *   Returns the current state of persistent module settings.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   a value among <c>YModule.PERSISTENTSETTINGS_LOADED</c>, <c>YModule.PERSISTENTSETTINGS_SAVED</c> and
     *   <c>YModule.PERSISTENTSETTINGS_MODIFIED</c> corresponding to the current state of persistent module settings
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YModule.PERSISTENTSETTINGS_INVALID</c>.
     * </para>
     */
    public int get_persistentSettings()
    {
      if (this._cacheExpiration <= YAPI.GetTickCount())
      {
        if (this.load(YAPI.DefaultCacheValidity) != YAPI.SUCCESS)
        {
          return PERSISTENTSETTINGS_INVALID;
        }
      }
      return this._persistentSettings;
    }

    public int set_persistentSettings(int newval)
    {
      string rest_val;
      rest_val = (newval).ToString();
      return _setAttr("persistentSettings", rest_val);
    }

    /**
     * <summary>
     *   Returns the luminosity of the  module informative leds (from 0 to 100).
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   an integer corresponding to the luminosity of the  module informative leds (from 0 to 100)
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YModule.LUMINOSITY_INVALID</c>.
     * </para>
     */
    public int get_luminosity()
    {
      if (this._cacheExpiration <= YAPI.GetTickCount())
      {
        if (this.load(YAPI.DefaultCacheValidity) != YAPI.SUCCESS)
        {
          return LUMINOSITY_INVALID;
        }
      }
      return this._luminosity;
    }

    /**
     * <summary>
     *   Changes the luminosity of the module informative leds.
     * <para>
     *   The parameter is a
     *   value between 0 and 100.
     *   Remember to call the <c>saveToFlash()</c> method of the module if the
     *   modification must be kept.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="newval">
     *   an integer corresponding to the luminosity of the module informative leds
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
    public int set_luminosity(int newval)
    {
      string rest_val;
      rest_val = (newval).ToString();
      return _setAttr("luminosity", rest_val);
    }

    /**
     * <summary>
     *   Returns the state of the localization beacon.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   either <c>YModule.BEACON_OFF</c> or <c>YModule.BEACON_ON</c>, according to the state of the localization beacon
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YModule.BEACON_INVALID</c>.
     * </para>
     */
    public int get_beacon()
    {
      if (this._cacheExpiration <= YAPI.GetTickCount())
      {
        if (this.load(YAPI.DefaultCacheValidity) != YAPI.SUCCESS)
        {
          return BEACON_INVALID;
        }
      }
      return this._beacon;
    }

    /**
     * <summary>
     *   Turns on or off the module localization beacon.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="newval">
     *   either <c>YModule.BEACON_OFF</c> or <c>YModule.BEACON_ON</c>
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
    public int set_beacon(int newval)
    {
      string rest_val;
      rest_val = (newval > 0 ? "1" : "0");
      return _setAttr("beacon", rest_val);
    }

    /**
     * <summary>
     *   Returns the number of milliseconds spent since the module was powered on.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   an integer corresponding to the number of milliseconds spent since the module was powered on
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YModule.UPTIME_INVALID</c>.
     * </para>
     */
    public long get_upTime()
    {
      if (this._cacheExpiration <= YAPI.GetTickCount())
      {
        if (this.load(YAPI.DefaultCacheValidity) != YAPI.SUCCESS)
        {
          return UPTIME_INVALID;
        }
      }
      return this._upTime;
    }

    /**
     * <summary>
     *   Returns the current consumed by the module on the USB bus, in milli-amps.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   an integer corresponding to the current consumed by the module on the USB bus, in milli-amps
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YModule.USBCURRENT_INVALID</c>.
     * </para>
     */
    public int get_usbCurrent()
    {
      if (this._cacheExpiration <= YAPI.GetTickCount())
      {
        if (this.load(YAPI.DefaultCacheValidity) != YAPI.SUCCESS)
        {
          return USBCURRENT_INVALID;
        }
      }
      return this._usbCurrent;
    }

    /**
     * <summary>
     *   Returns the remaining number of seconds before the module restarts, or zero when no
     *   reboot has been scheduled.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   an integer corresponding to the remaining number of seconds before the module restarts, or zero when no
     *   reboot has been scheduled
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YModule.REBOOTCOUNTDOWN_INVALID</c>.
     * </para>
     */
    public int get_rebootCountdown()
    {
      if (this._cacheExpiration <= YAPI.GetTickCount())
      {
        if (this.load(YAPI.DefaultCacheValidity) != YAPI.SUCCESS)
        {
          return REBOOTCOUNTDOWN_INVALID;
        }
      }
      return this._rebootCountdown;
    }

    public int set_rebootCountdown(int newval)
    {
      string rest_val;
      rest_val = (newval).ToString();
      return _setAttr("rebootCountdown", rest_val);
    }

    /**
     * <summary>
     *   Returns the number of USB interfaces used by the module.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   either <c>YModule.USBBANDWIDTH_SIMPLE</c> or <c>YModule.USBBANDWIDTH_DOUBLE</c>, according to the
     *   number of USB interfaces used by the module
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YModule.USBBANDWIDTH_INVALID</c>.
     * </para>
     */
    public int get_usbBandwidth()
    {
      if (this._cacheExpiration <= YAPI.GetTickCount())
      {
        if (this.load(YAPI.DefaultCacheValidity) != YAPI.SUCCESS)
        {
          return USBBANDWIDTH_INVALID;
        }
      }
      return this._usbBandwidth;
    }

    /**
     * <summary>
     *   Allows you to find a module from its serial number or from its logical name.
     * <para>
     * </para>
     * <para>
     *   This function does not require that the module is online at the time
     *   it is invoked. The returned object is nevertheless valid.
     *   Use the method <c>YModule.isOnline()</c> to test if the module is
     *   indeed online at a given time. In case of ambiguity when looking for
     *   a module by logical name, no error is notified: the first instance
     *   found is returned. The search is performed first by hardware name,
     *   then by logical name.
     * </para>
     * </summary>
     * <param name="func">
     *   a string containing either the serial number or
     *   the logical name of the desired module
     * </param>
     * <returns>
     *   a <c>YModule</c> object allowing you to drive the module
     *   or get additional information on the module.
     * </returns>
     */
    public static YModule FindModule(string func)
    {
      YModule obj;
      obj = (YModule)YFunction._FindFromCache("Module", func);
      if (obj == null)
      {
        obj = new YModule(func);
        YFunction._AddToCache("Module", func, obj);
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
      this._valueCallbackModule = callback;
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
      if (this._valueCallbackModule != null)
      {
        this._valueCallbackModule(this, value);
      }
      else
      {
        base._invokeValueCallback(value);
      }
      return 0;
    }

    /**
     * <summary>
     *   Saves current settings in the nonvolatile memory of the module.
     * <para>
     *   Warning: the number of allowed save operations during a module life is
     *   limited (about 100000 cycles). Do not call this function within a loop.
     * </para>
     * </summary>
     * <returns>
     *   <c>YAPI.SUCCESS</c> when the call succeeds.
     * </returns>
     * <para>
     *   On failure, throws an exception or returns a negative error code.
     * </para>
     */
    public virtual int saveToFlash()
    {
      return this.set_persistentSettings(PERSISTENTSETTINGS_SAVED);
    }

    /**
     * <summary>
     *   Reloads the settings stored in the nonvolatile memory, as
     *   when the module is powered on.
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   <c>YAPI.SUCCESS</c> when the call succeeds.
     * </returns>
     * <para>
     *   On failure, throws an exception or returns a negative error code.
     * </para>
     */
    public virtual int revertFromFlash()
    {
      return this.set_persistentSettings(PERSISTENTSETTINGS_LOADED);
    }

    /**
     * <summary>
     *   Schedules a simple module reboot after the given number of seconds.
     * <para>
     * </para>
     * </summary>
     * <param name="secBeforeReboot">
     *   number of seconds before rebooting
     * </param>
     * <returns>
     *   <c>YAPI.SUCCESS</c> when the call succeeds.
     * </returns>
     * <para>
     *   On failure, throws an exception or returns a negative error code.
     * </para>
     */
    public virtual int reboot(int secBeforeReboot)
    {
      return this.set_rebootCountdown(secBeforeReboot);
    }

    /**
     * <summary>
     *   Schedules a module reboot into special firmware update mode.
     * <para>
     * </para>
     * </summary>
     * <param name="secBeforeReboot">
     *   number of seconds before rebooting
     * </param>
     * <returns>
     *   <c>YAPI.SUCCESS</c> when the call succeeds.
     * </returns>
     * <para>
     *   On failure, throws an exception or returns a negative error code.
     * </para>
     */
    public virtual int triggerFirmwareUpdate(int secBeforeReboot)
    {
      return this.set_rebootCountdown(-secBeforeReboot);
    }

    /**
     * <summary>
     *   Downloads the specified built-in file and returns a binary buffer with its content.
     * <para>
     * </para>
     * </summary>
     * <param name="pathname">
     *   name of the new file to load
     * </param>
     * <returns>
     *   a binary buffer with the file content
     * </returns>
     * <para>
     *   On failure, throws an exception or returns an empty content.
     * </para>
     */
    public virtual byte[] download(string pathname)
    {
      return this._download(pathname);
    }

    /**
     * <summary>
     *   Returns the icon of the module.
     * <para>
     *   The icon is a PNG image and does not
     *   exceeds 1536 bytes.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   a binary buffer with module icon, in png format.
     * </returns>
     */
    public virtual byte[] get_icon2d()
    {
      return this._download("icon2d.png");
    }

    /**
     * <summary>
     *   Returns a string with last logs of the module.
     * <para>
     *   This method return only
     *   logs that are still in the module.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   a string with last logs of the module.
     * </returns>
     */
    public virtual string get_lastLogs()
    {
      byte[] content;
      // may throw an exception
      content = this._download("logs.txt");
      return YAPI.DefaultEncoding.GetString(content);
    }

    /**
     * <summary>
     *   Continues the module enumeration started using <c>yFirstModule()</c>.
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   a pointer to a <c>YModule</c> object, corresponding to
     *   the next module found, or a <c>null</c> pointer
     *   if there are no more modules to enumerate.
     * </returns>
     */
    public YModule nextModule()
    {
      string hwid = "";
      if (YAPI.YISERR(_nextFunction(ref hwid)))
        return null;
      if (hwid == "")
        return null;
      return FindModule(hwid);
    }

    //--- (end of generated code: YModule implementation)
    /**
     * <summary>
     *   Returns a global identifier of the function in the format <c>MODULE_NAME&#46;FUNCTION_NAME</c>.
     * <para>
     *   The returned string uses the logical names of the module and of the function if they are defined,
     *   otherwise the serial number of the module and the hardware identifier of the function
     *   (for exemple: <c>MyCustomName.relay1</c>)
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   a string that uniquely identifies the function using logical names
     *   (ex: <c>MyCustomName.relay1</c>)
     * </returns>
     * <para>
     *   On failure, throws an exception or returns  <c>YModule.FRIENDLYNAME_INVALID</c>.
     * </para>
     */

    public override string get_friendlyName()
    {

      YRETCODE retcode;
      YFUN_DESCR fundesc = 0;
      YDEV_DESCR devdesc = 0;
      string funcName = "";
      string dummy = "";
      string errmsg = "";
      string snum = "";
      string funcid = "";

      // Resolve the function name
      retcode = _getDescriptor(ref fundesc, ref errmsg);
      if (YAPI.YISERR(retcode))
      {
        _throw(retcode, errmsg);
        return YAPI.FRIENDLYNAME_INVALID;
      }

      retcode = YAPI.yapiGetFunctionInfo(fundesc, ref devdesc, ref snum, ref funcid, ref funcName, ref dummy, ref errmsg);
      if (YAPI.YISERR(retcode))
      {
        _throw(retcode, errmsg);
        return YAPI.FRIENDLYNAME_INVALID;
      }

      if (funcName != "")
      {
        return funcName;
      }
      return snum;
    }



    public void setImmutableAttributes(ref YAPI.yDeviceSt infos)
    {
      _serialNumber = infos.serial;
      _productName = infos.productname;
      _productId = infos.deviceid;
    }

    // Return the properties of the nth function of our device
    private YRETCODE _getFunction(int idx, ref string serial, ref string funcId, ref string funcName, ref string funcVal, ref string errmsg)
    {
      YRETCODE functionReturnValue = default(YRETCODE);

      List<u32> functions = null;
      YAPI.YDevice dev = null;
      int res = 0;
      YFUN_DESCR fundescr = default(YFUN_DESCR);
      YDEV_DESCR devdescr = default(YDEV_DESCR);

      // retrieve device object 
      res = _getDevice(ref dev, ref errmsg);
      if ((YAPI.YISERR(res)))
      {
        _throw(res, errmsg);
        functionReturnValue = res;
        return functionReturnValue;
      }



      // get reference to all functions from the device
      res = dev.getFunctions(ref functions, ref errmsg);
      if ((YAPI.YISERR(res)))
      {
        functionReturnValue = res;
        return functionReturnValue;
      }

      // get latest function info from yellow pages
      fundescr = Convert.ToInt32(functions[idx]);

      res = YAPI.yapiGetFunctionInfo(fundescr, ref devdescr, ref serial, ref funcId, ref funcName, ref funcVal, ref errmsg);
      if ((YAPI.YISERR(res)))
      {
        functionReturnValue = res;
        return functionReturnValue;
      }

      functionReturnValue = YAPI.SUCCESS;
      return functionReturnValue;
    }

    /**
     * <summary>
     *   Returns the number of functions (beside the "module" interface) available on the module.
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   the number of functions on the module
     * </returns>
     * <para>
     *   On failure, throws an exception or returns a negative error code.
     * </para>
     */
    public int functionCount()
    {
      int functionReturnValue = 0;
      List<u32> functions = null;
      YAPI.YDevice dev = null;
      string errmsg = "";
      int res = 0;

      res = _getDevice(ref dev, ref errmsg);
      if ((YAPI.YISERR(res)))
      {
        _throw(res, errmsg);
        functionReturnValue = res;
        return functionReturnValue;
      }

      res = dev.getFunctions(ref functions, ref errmsg);
      if ((YAPI.YISERR(res)))
      {
        functions = null;
        _throw(res, errmsg);
        functionReturnValue = res;
        return functionReturnValue;
      }

      functionReturnValue = functions.Count;
      return functionReturnValue;

    }

    /**
     * <summary>
     *   Retrieves the hardware identifier of the <i>n</i>th function on the module.
     * <para>
     * </para>
     * </summary>
     * <param name="functionIndex">
     *   the index of the function for which the information is desired, starting at 0 for the first function.
     * </param>
     * <returns>
     *   a string corresponding to the unambiguous hardware identifier of the requested module function
     * </returns>
     * <para>
     *   On failure, throws an exception or returns an empty string.
     * </para>
     */
    public string functionId(int functionIndex)
    {
      string functionReturnValue = null;
      string serial = "";
      string funcId = "";
      string funcName = "";
      string funcVal = "";
      string errmsg = "";
      int res = 0;
      res = _getFunction(functionIndex, ref serial, ref funcId, ref funcName, ref funcVal, ref errmsg);
      if ((YAPI.YISERR(res)))
      {
        _throw(res, errmsg);
        functionReturnValue = YAPI.INVALID_STRING;
        return functionReturnValue;
      }
      functionReturnValue = funcId;
      return functionReturnValue;
    }

    /**
     * <summary>
     *   Retrieves the logical name of the <i>n</i>th function on the module.
     * <para>
     * </para>
     * </summary>
     * <param name="functionIndex">
     *   the index of the function for which the information is desired, starting at 0 for the first function.
     * </param>
     * <returns>
     *   a string corresponding to the logical name of the requested module function
     * </returns>
     * <para>
     *   On failure, throws an exception or returns an empty string.
     * </para>
     */
    public string functionName(int functionIndex)
    {
      string functionReturnValue = null;
      string serial = "";
      string funcId = "";
      string funcName = "";
      string funcVal = "";
      string errmsg = "";
      int res = 0;

      res = _getFunction(functionIndex, ref serial, ref funcId, ref funcName, ref funcVal, ref errmsg);
      if ((YAPI.YISERR(res)))
      {
        _throw(res, errmsg);
        functionReturnValue = YAPI.INVALID_STRING;
        return functionReturnValue;
      }

      functionReturnValue = funcName;
      return functionReturnValue;
    }

    /**
     * <summary>
     *   Retrieves the advertised value of the <i>n</i>th function on the module.
     * <para>
     * </para>
     * </summary>
     * <param name="functionIndex">
     *   the index of the function for which the information is desired, starting at 0 for the first function.
     * </param>
     * <returns>
     *   a short string (up to 6 characters) corresponding to the advertised value of the requested module function
     * </returns>
     * <para>
     *   On failure, throws an exception or returns an empty string.
     * </para>
     */
    public string functionValue(int functionIndex)
    {
      string functionReturnValue = null;
      string serial = "";
      string funcId = "";
      string funcName = "";
      string funcVal = "";
      string errmsg = "";
      int res = 0;

      res = _getFunction(functionIndex, ref serial, ref funcId, ref funcName, ref funcVal, ref errmsg);
      if ((YAPI.YISERR(res)))
      {
        _throw(res, errmsg);
        functionReturnValue = YAPI.INVALID_STRING;
        return functionReturnValue;
      }
      functionReturnValue = funcVal;
      return functionReturnValue;
    }

    //--- (generated code: Module functions)

    /**
     * <summary>
     *   Starts the enumeration of modules currently accessible.
     * <para>
     *   Use the method <c>YModule.nextModule()</c> to iterate on the
     *   next modules.
     * </para>
     * </summary>
     * <returns>
     *   a pointer to a <c>YModule</c> object, corresponding to
     *   the first module currently online, or a <c>null</c> pointer
     *   if there are none.
     * </returns>
     */
    public static YModule FirstModule()
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
      err = YAPI.apiGetFunctionsByClass("Module", 0, p, size, ref neededsize, ref errmsg);
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
      return FindModule(serial + "." + funcId);
    }



    //--- (end of generated code: Module functions)
  }



  //--- (generated code: YSensor class start)
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
  public class YSensor : YFunction
  {
    //--- (end of generated code: YSensor class start)
    //--- (generated code: YSensor definitions)
    public new delegate void ValueCallback(YSensor func, string value);
    public new delegate void TimedReportCallback(YSensor func, YMeasure measure);

    public const string UNIT_INVALID = YAPI.INVALID_STRING;
    public const double CURRENTVALUE_INVALID = YAPI.INVALID_DOUBLE;
    public const double LOWESTVALUE_INVALID = YAPI.INVALID_DOUBLE;
    public const double HIGHESTVALUE_INVALID = YAPI.INVALID_DOUBLE;
    public const double CURRENTRAWVALUE_INVALID = YAPI.INVALID_DOUBLE;
    public const string LOGFREQUENCY_INVALID = YAPI.INVALID_STRING;
    public const string REPORTFREQUENCY_INVALID = YAPI.INVALID_STRING;
    public const string CALIBRATIONPARAM_INVALID = YAPI.INVALID_STRING;
    public const double RESOLUTION_INVALID = YAPI.INVALID_DOUBLE;
    protected string _unit = UNIT_INVALID;
    protected double _currentValue = CURRENTVALUE_INVALID;
    protected double _lowestValue = LOWESTVALUE_INVALID;
    protected double _highestValue = HIGHESTVALUE_INVALID;
    protected double _currentRawValue = CURRENTRAWVALUE_INVALID;
    protected string _logFrequency = LOGFREQUENCY_INVALID;
    protected string _reportFrequency = REPORTFREQUENCY_INVALID;
    protected string _calibrationParam = CALIBRATIONPARAM_INVALID;
    protected double _resolution = RESOLUTION_INVALID;
    protected ValueCallback _valueCallbackSensor = null;
    protected TimedReportCallback _timedReportCallbackSensor = null;
    protected double _prevTimedReport = 0;
    protected double _iresol = 0;
    protected double _offset = 0;
    protected double _scale = 0;
    protected double _decexp = 0;
    protected bool _isScal;
    protected int _caltyp = 0;
    protected List<int> _calpar = new List<int>();
    protected List<double> _calraw = new List<double>();
    protected List<double> _calref = new List<double>();
    protected YAPI.yCalibrationHandler _calhdl = null;
    //--- (end of generated code: YSensor definitions)


    protected string _encodeCalibrationPoints(List<double> rawValues, List<double> refValues, string actualCparams)
    {
      int npt = (rawValues.Count < refValues.Count ? rawValues.Count : refValues.Count);
      int rawVal, refVal;
      int calibType;
      int minRaw = 0;
      String res;

      if (npt == 0)
      {
        return "";
      }
      int pos = actualCparams.IndexOf(',');
      if (actualCparams == "" || actualCparams == "0" || pos >= 0)
      {
        _throw(YAPI.NOT_SUPPORTED, "Device does not support new calibration parameters. Please upgrade your firmware");
        return "0";
      }
      List<int> iCalib = YAPI._decodeWords(actualCparams);
      int calibrationOffset = iCalib[0];
      int divisor = iCalib[1];
      if (divisor > 0)
      {
        calibType = npt;
      }
      else
      {
        calibType = 10 + npt;
      }
      res = calibType.ToString();
      if (calibType <= 10)
      {
        for (int i = 0; i < npt; i++)
        {
          rawVal = (int)(rawValues[i] * divisor - calibrationOffset + .5);
          if (rawVal >= minRaw && rawVal < 65536)
          {
            refVal = (int)(refValues[i] * divisor - calibrationOffset + .5);
            if (refVal >= 0 && refVal < 65536)
            {
              res += "," + rawVal.ToString() + "," + refVal.ToString();
              minRaw = rawVal + 1;
            }
          }
        }
      }
      else
      {
        // 16-bit floating-point decimal encoding
        for (int i = 0; i < npt; i++)
        {
          rawVal = (int)YAPI._doubleToDecimal(rawValues[i]);
          refVal = (int)YAPI._doubleToDecimal(refValues[i]);
          res += "," + rawVal.ToString() + "," + refVal.ToString();
        }
      }
      return res;
    }

    protected int _decodeCalibrationPoints(string calibParams, List<int> intPt, List<double> rawPt, List<double> calPt)
    {

      intPt = new List<int>();
      rawPt = new List<double>();

      calPt = new List<double>();
      if (calibParams == "" || calibParams == "0")
      {
        // old format: no calibration
        return 0;
      }
      if (calibParams.IndexOf(',') != -1)
      {
        // old format -> device must do the calibration
        return -1;
      }
      // new format
      List<int> iCalib = YAPI._decodeWords(calibParams);
      if (iCalib.Count < 2)
      {
        // bad format
        return -1;
      }
      if (iCalib.Count == 2)
      {
        // no calibration
        return 0;
      }
      int pos = 0;
      double calibrationOffset = iCalib[pos++];
      double divisor = iCalib[pos++];
      int calibType = iCalib[pos++];
      if (calibType == 0)
      {
        return 0;
      }
      // parse calibration parameters
      while (pos < iCalib.Count)
      {
        int ival = iCalib[pos++];
        double fval;
        if (calibType <= 10)
        {
          fval = (ival + calibrationOffset) / divisor;
        }
        else
        {
          fval = YAPI._decimalToDouble(ival);
        }
        intPt.Add(ival);
        if ((intPt.Count & 1) == 1)
        {
          rawPt.Add(fval);
        }
        else
        {
          calPt.Add(fval);
        }
      }
      if (intPt.Count < 10)
      {
        return -1;
      }
      return calibType;
    }

    public YSensor(string func)
      : base(func)
    {
      _className = "Sensor";
      //--- (generated code: YSensor attributes initialization)
      //--- (end of generated code: YSensor attributes initialization)
    }


    //--- (generated code: YSensor implementation)

    protected override void _parseAttr(YAPI.TJSONRECORD member)
    {
      if (member.name == "unit")
      {
        _unit = member.svalue;
        return;
      }
      if (member.name == "currentValue")
      {
        _currentValue = member.ivalue / 65536.0;
        return;
      }
      if (member.name == "lowestValue")
      {
        _lowestValue = member.ivalue / 65536.0;
        return;
      }
      if (member.name == "highestValue")
      {
        _highestValue = member.ivalue / 65536.0;
        return;
      }
      if (member.name == "currentRawValue")
      {
        _currentRawValue = member.ivalue / 65536.0;
        return;
      }
      if (member.name == "logFrequency")
      {
        _logFrequency = member.svalue;
        return;
      }
      if (member.name == "reportFrequency")
      {
        _reportFrequency = member.svalue;
        return;
      }
      if (member.name == "calibrationParam")
      {
        _calibrationParam = member.svalue;
        return;
      }
      if (member.name == "resolution")
      {
        _resolution = (member.ivalue > 100 ? 1.0 / Math.Round(65536.0 / member.ivalue) : 0.001 / Math.Round(67.0 / member.ivalue));
        return;
      }
      base._parseAttr(member);
    }

    /**
     * <summary>
     *   Returns the measuring unit for the measure.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   a string corresponding to the measuring unit for the measure
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YSensor.UNIT_INVALID</c>.
     * </para>
     */
    public string get_unit()
    {
      if (this._cacheExpiration <= YAPI.GetTickCount())
      {
        if (this.load(YAPI.DefaultCacheValidity) != YAPI.SUCCESS)
        {
          return UNIT_INVALID;
        }
      }
      return this._unit;
    }

    /**
     * <summary>
     *   Returns the current value of the measure.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   a floating point number corresponding to the current value of the measure
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YSensor.CURRENTVALUE_INVALID</c>.
     * </para>
     */
    public double get_currentValue()
    {
      double res = 0;
      if (this._cacheExpiration <= YAPI.GetTickCount())
      {
        if (this.load(YAPI.DefaultCacheValidity) != YAPI.SUCCESS)
        {
          return CURRENTVALUE_INVALID;
        }
      }
      res = this._applyCalibration(this._currentRawValue);
      if (res == CURRENTVALUE_INVALID)
      {
        res = this._currentValue;
      }
      res = res * this._iresol;
      return Math.Round(res) / this._iresol;
    }

    /**
     * <summary>
     *   Changes the recorded minimal value observed.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="newval">
     *   a floating point number corresponding to the recorded minimal value observed
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
    public int set_lowestValue(double newval)
    {
      string rest_val;
      rest_val = Math.Round(newval * 65536.0).ToString();
      return _setAttr("lowestValue", rest_val);
    }

    /**
     * <summary>
     *   Returns the minimal value observed for the measure since the device was started.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   a floating point number corresponding to the minimal value observed for the measure since the device was started
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YSensor.LOWESTVALUE_INVALID</c>.
     * </para>
     */
    public double get_lowestValue()
    {
      double res = 0;
      if (this._cacheExpiration <= YAPI.GetTickCount())
      {
        if (this.load(YAPI.DefaultCacheValidity) != YAPI.SUCCESS)
        {
          return LOWESTVALUE_INVALID;
        }
      }
      res = this._lowestValue * this._iresol;
      return Math.Round(res) / this._iresol;
    }

    /**
     * <summary>
     *   Changes the recorded maximal value observed.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="newval">
     *   a floating point number corresponding to the recorded maximal value observed
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
    public int set_highestValue(double newval)
    {
      string rest_val;
      rest_val = Math.Round(newval * 65536.0).ToString();
      return _setAttr("highestValue", rest_val);
    }

    /**
     * <summary>
     *   Returns the maximal value observed for the measure since the device was started.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   a floating point number corresponding to the maximal value observed for the measure since the device was started
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YSensor.HIGHESTVALUE_INVALID</c>.
     * </para>
     */
    public double get_highestValue()
    {
      double res = 0;
      if (this._cacheExpiration <= YAPI.GetTickCount())
      {
        if (this.load(YAPI.DefaultCacheValidity) != YAPI.SUCCESS)
        {
          return HIGHESTVALUE_INVALID;
        }
      }
      res = this._highestValue * this._iresol;
      return Math.Round(res) / this._iresol;
    }

    /**
     * <summary>
     *   Returns the uncalibrated, unrounded raw value returned by the sensor.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   a floating point number corresponding to the uncalibrated, unrounded raw value returned by the sensor
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YSensor.CURRENTRAWVALUE_INVALID</c>.
     * </para>
     */
    public double get_currentRawValue()
    {
      if (this._cacheExpiration <= YAPI.GetTickCount())
      {
        if (this.load(YAPI.DefaultCacheValidity) != YAPI.SUCCESS)
        {
          return CURRENTRAWVALUE_INVALID;
        }
      }
      return this._currentRawValue;
    }

    /**
     * <summary>
     *   Returns the datalogger recording frequency for this function, or "OFF"
     *   when measures are not stored in the data logger flash memory.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   a string corresponding to the datalogger recording frequency for this function, or "OFF"
     *   when measures are not stored in the data logger flash memory
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YSensor.LOGFREQUENCY_INVALID</c>.
     * </para>
     */
    public string get_logFrequency()
    {
      if (this._cacheExpiration <= YAPI.GetTickCount())
      {
        if (this.load(YAPI.DefaultCacheValidity) != YAPI.SUCCESS)
        {
          return LOGFREQUENCY_INVALID;
        }
      }
      return this._logFrequency;
    }

    /**
     * <summary>
     *   Changes the datalogger recording frequency for this function.
     * <para>
     *   The frequency can be specified as samples per second,
     *   as sample per minute (for instance "15/m") or in samples per
     *   hour (eg. "4/h"). To disable recording for this function, use
     *   the value "OFF".
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="newval">
     *   a string corresponding to the datalogger recording frequency for this function
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
    public int set_logFrequency(string newval)
    {
      string rest_val;
      rest_val = newval;
      return _setAttr("logFrequency", rest_val);
    }

    /**
     * <summary>
     *   Returns the timed value notification frequency, or "OFF" if timed
     *   value notifications are disabled for this function.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   a string corresponding to the timed value notification frequency, or "OFF" if timed
     *   value notifications are disabled for this function
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YSensor.REPORTFREQUENCY_INVALID</c>.
     * </para>
     */
    public string get_reportFrequency()
    {
      if (this._cacheExpiration <= YAPI.GetTickCount())
      {
        if (this.load(YAPI.DefaultCacheValidity) != YAPI.SUCCESS)
        {
          return REPORTFREQUENCY_INVALID;
        }
      }
      return this._reportFrequency;
    }

    /**
     * <summary>
     *   Changes the timed value notification frequency for this function.
     * <para>
     *   The frequency can be specified as samples per second,
     *   as sample per minute (for instance "15/m") or in samples per
     *   hour (eg. "4/h"). To disable timed value notifications for this
     *   function, use the value "OFF".
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="newval">
     *   a string corresponding to the timed value notification frequency for this function
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
    public int set_reportFrequency(string newval)
    {
      string rest_val;
      rest_val = newval;
      return _setAttr("reportFrequency", rest_val);
    }

    public string get_calibrationParam()
    {
      if (this._cacheExpiration <= YAPI.GetTickCount())
      {
        if (this.load(YAPI.DefaultCacheValidity) != YAPI.SUCCESS)
        {
          return CALIBRATIONPARAM_INVALID;
        }
      }
      return this._calibrationParam;
    }

    public int set_calibrationParam(string newval)
    {
      string rest_val;
      rest_val = newval;
      return _setAttr("calibrationParam", rest_val);
    }

    /**
     * <summary>
     *   Changes the resolution of the measured physical values.
     * <para>
     *   The resolution corresponds to the numerical precision
     *   when displaying value. It does not change the precision of the measure itself.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="newval">
     *   a floating point number corresponding to the resolution of the measured physical values
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
    public int set_resolution(double newval)
    {
      string rest_val;
      rest_val = Math.Round(newval * 65536.0).ToString();
      return _setAttr("resolution", rest_val);
    }

    /**
     * <summary>
     *   Returns the resolution of the measured values.
     * <para>
     *   The resolution corresponds to the numerical precision
     *   of the measures, which is not always the same as the actual precision of the sensor.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   a floating point number corresponding to the resolution of the measured values
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YSensor.RESOLUTION_INVALID</c>.
     * </para>
     */
    public double get_resolution()
    {
      if (this._cacheExpiration <= YAPI.GetTickCount())
      {
        if (this.load(YAPI.DefaultCacheValidity) != YAPI.SUCCESS)
        {
          return RESOLUTION_INVALID;
        }
      }
      return this._resolution;
    }

    /**
     * <summary>
     *   Retrieves a sensor for a given identifier.
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
     *   This function does not require that the sensor is online at the time
     *   it is invoked. The returned object is nevertheless valid.
     *   Use the method <c>YSensor.isOnline()</c> to test if the sensor is
     *   indeed online at a given time. In case of ambiguity when looking for
     *   a sensor by logical name, no error is notified: the first instance
     *   found is returned. The search is performed first by hardware name,
     *   then by logical name.
     * </para>
     * </summary>
     * <param name="func">
     *   a string that uniquely characterizes the sensor
     * </param>
     * <returns>
     *   a <c>YSensor</c> object allowing you to drive the sensor.
     * </returns>
     */
    public static YSensor FindSensor(string func)
    {
      YSensor obj;
      obj = (YSensor)YFunction._FindFromCache("Sensor", func);
      if (obj == null)
      {
        obj = new YSensor(func);
        YFunction._AddToCache("Sensor", func, obj);
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
      this._valueCallbackSensor = callback;
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
      if (this._valueCallbackSensor != null)
      {
        this._valueCallbackSensor(this, value);
      }
      else
      {
        base._invokeValueCallback(value);
      }
      return 0;
    }

    public override int _parserHelper()
    {
      int position = 0;
      int maxpos = 0;
      List<int> iCalib = new List<int>();
      int iRaw = 0;
      int iRef = 0;
      double fRaw = 0;
      double fRef = 0;
      // Store inverted resolution, to provide better rounding
      if (this._resolution > 0)
      {
        this._iresol = Math.Round(1.0 / this._resolution);
      }
      else
      {
        return 0;
      }

      this._scale = -1;
      this._calpar.Clear();
      this._calraw.Clear();
      this._calref.Clear();

      // Old format: supported when there is no calibration
      if (this._calibrationParam == "" || this._calibrationParam == "0")
      {
        this._caltyp = 0;
        return 0;
      }
      // Old format: calibrated value will be provided by the device
      if ((this._calibrationParam).IndexOf(",") >= 0)
      {
        this._caltyp = -1;
        return 0;
      }
      // New format, decode parameters
      iCalib = YAPI._decodeWords(this._calibrationParam);
      // In case of unknown format, calibrated value will be provided by the device
      if (iCalib.Count < 2)
      {
        this._caltyp = -1;
        return 0;
      }

      // Save variable format (scale for scalar, or decimal exponent)
      this._isScal = (iCalib[1] > 0);
      if (this._isScal)
      {
        this._offset = iCalib[0];
        if (this._offset > 32767)
        {
          this._offset = this._offset - 65536;
        }
        this._scale = iCalib[1];
        this._decexp = 0;
      }
      else
      {
        this._offset = 0;
        this._scale = 1;
        this._decexp = 1.0;
        position = iCalib[0];
        while (position > 0)
        {
          this._decexp = this._decexp * 10;
          position = position - 1;
        }
      }

      // Shortcut when there is no calibration parameter
      if (iCalib.Count == 2)
      {
        this._caltyp = 0;
        return 0;
      }

      this._caltyp = iCalib[2];
      this._calhdl = YAPI._getCalibrationHandler(this._caltyp);
      // parse calibration points
      position = 3;
      if (this._caltyp <= 10)
      {
        maxpos = this._caltyp;
      }
      else
      {
        if (this._caltyp <= 20)
        {
          maxpos = this._caltyp - 10;
        }
        else
        {
          maxpos = 5;
        }
      }
      maxpos = 3 + 2 * maxpos;
      if (maxpos > iCalib.Count)
      {
        maxpos = iCalib.Count;
      }
      this._calpar.Clear();
      this._calraw.Clear();
      this._calref.Clear();
      while (position + 1 < maxpos)
      {
        iRaw = iCalib[position];
        iRef = iCalib[position + 1];
        this._calpar.Add(iRaw);
        this._calpar.Add(iRef);
        if (this._isScal)
        {
          fRaw = iRaw;
          fRaw = (fRaw - this._offset) / this._scale;
          fRef = iRef;
          fRef = (fRef - this._offset) / this._scale;
          this._calraw.Add(fRaw);
          this._calref.Add(fRef);
        }
        else
        {
          this._calraw.Add(YAPI._decimalToDouble(iRaw));
          this._calref.Add(YAPI._decimalToDouble(iRef));
        }
        position = position + 2;
      }



      return 0;
    }

    /**
     * <summary>
     *   Retrieves a DataSet object holding historical data for this
     *   sensor, for a specified time interval.
     * <para>
     *   The measures will be
     *   retrieved from the data logger, which must have been turned
     *   on at the desired time. See the documentation of the DataSet
     *   class for information on how to get an overview of the
     *   recorded data, and how to load progressively a large set
     *   of measures from the data logger.
     * </para>
     * <para>
     *   This function only works if the device uses a recent firmware,
     *   as DataSet objects are not supported by firmwares older than
     *   version 13000.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="startTime">
     *   the start of the desired measure time interval,
     *   as a Unix timestamp, i.e. the number of seconds since
     *   January 1, 1970 UTC. The special value 0 can be used
     *   to include any meaasure, without initial limit.
     * </param>
     * <param name="endTime">
     *   the end of the desired measure time interval,
     *   as a Unix timestamp, i.e. the number of seconds since
     *   January 1, 1970 UTC. The special value 0 can be used
     *   to include any meaasure, without ending limit.
     * </param>
     * <returns>
     *   an instance of YDataSet, providing access to historical
     *   data. Past measures can be loaded progressively
     *   using methods from the YDataSet object.
     * </returns>
     */
    public virtual YDataSet get_recordedData(long startTime, long endTime)
    {
      string funcid;
      string funit;
      // may throw an exception
      funcid = this.get_functionId();
      funit = this.get_unit();
      return new YDataSet(this, funcid, funit, startTime, endTime);
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
    public virtual int registerTimedReportCallback(TimedReportCallback callback)
    {
      if (callback != null)
      {
        YFunction._UpdateTimedReportCallbackList(this, true);
      }
      else
      {
        YFunction._UpdateTimedReportCallbackList(this, false);
      }
      this._timedReportCallbackSensor = callback;
      return 0;
    }

    public virtual int _invokeTimedReportCallback(YMeasure value)
    {
      if (this._timedReportCallbackSensor != null)
      {
        this._timedReportCallbackSensor(this, value);
      }
      else
      {
      }
      return 0;
    }

    /**
     * <summary>
     *   Configures error correction data points, in particular to compensate for
     *   a possible perturbation of the measure caused by an enclosure.
     * <para>
     *   It is possible
     *   to configure up to five correction points. Correction points must be provided
     *   in ascending order, and be in the range of the sensor. The device will automatically
     *   perform a linear interpolation of the error correction between specified
     *   points. Remember to call the <c>saveToFlash()</c> method of the module if the
     *   modification must be kept.
     * </para>
     * <para>
     *   For more information on advanced capabilities to refine the calibration of
     *   sensors, please contact support@yoctopuce.com.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="rawValues">
     *   array of floating point numbers, corresponding to the raw
     *   values returned by the sensor for the correction points.
     * </param>
     * <param name="refValues">
     *   array of floating point numbers, corresponding to the corrected
     *   values for the correction points.
     * </param>
     * <returns>
     *   <c>YAPI.SUCCESS</c> if the call succeeds.
     * </returns>
     * <para>
     *   On failure, throws an exception or returns a negative error code.
     * </para>
     */
    public virtual int calibrateFromPoints(List<double> rawValues, List<double> refValues)
    {
      string rest_val;
      // may throw an exception
      rest_val = this._encodeCalibrationPoints(rawValues, refValues);
      return this._setAttr("calibrationParam", rest_val);
    }

    /**
     * <summary>
     *   Retrieves error correction data points previously entered using the method
     *   <c>calibrateFromPoints</c>.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="rawValues">
     *   array of floating point numbers, that will be filled by the
     *   function with the raw sensor values for the correction points.
     * </param>
     * <param name="refValues">
     *   array of floating point numbers, that will be filled by the
     *   function with the desired values for the correction points.
     * </param>
     * <returns>
     *   <c>YAPI.SUCCESS</c> if the call succeeds.
     * </returns>
     * <para>
     *   On failure, throws an exception or returns a negative error code.
     * </para>
     */
    public virtual int loadCalibrationPoints(List<double> rawValues, List<double> refValues)
    {
      rawValues.Clear();
      refValues.Clear();

      // Load function parameters if not yet loaded
      if (this._scale == 0)
      {
        if (this.load(YAPI.DefaultCacheValidity) != YAPI.SUCCESS)
        {
          return YAPI.DEVICE_NOT_FOUND;
        }
      }

      if (this._caltyp < 0)
      {
        this._throw(YAPI.NOT_SUPPORTED, "Device does not support new calibration parameters. Please upgrade your firmware");
        return YAPI.NOT_SUPPORTED;
      }
      rawValues.Clear();
      refValues.Clear();
      for (int ii = 0; ii < this._calraw.Count; ii++)
      {
        rawValues.Add(this._calraw[ii]);
      }
      for (int ii = 0; ii < this._calref.Count; ii++)
      {
        refValues.Add(this._calref[ii]);
      }
      return YAPI.SUCCESS;
    }

    public virtual string _encodeCalibrationPoints(List<double> rawValues, List<double> refValues)
    {
      string res;
      int npt = 0;
      int idx = 0;
      int iRaw = 0;
      int iRef = 0;

      npt = rawValues.Count;
      if (npt != refValues.Count)
      {
        this._throw(YAPI.INVALID_ARGUMENT, "Invalid calibration parameters (size mismatch)");
        return YAPI.INVALID_STRING;
      }

      // Shortcut when building empty calibration parameters
      if (npt == 0)
      {
        return "0";
      }

      // Load function parameters if not yet loaded
      if (this._scale == 0)
      {
        if (this.load(YAPI.DefaultCacheValidity) != YAPI.SUCCESS)
        {
          return YAPI.INVALID_STRING;
        }
      }

      // Detect old firmware
      if ((this._caltyp < 0) || (this._scale < 0))
      {
        this._throw(YAPI.NOT_SUPPORTED, "Device does not support new calibration parameters. Please upgrade your firmware");
        return "0";
      }
      if (this._isScal)
      {
        res = "" + Convert.ToString(npt);
        idx = 0;
        while (idx < npt)
        {
          iRaw = (int)Math.Round(rawValues[idx] * this._scale + this._offset);
          iRef = (int)Math.Round(refValues[idx] * this._scale + this._offset);
          res = "" + res + "," + Convert.ToString(iRaw) + "," + Convert.ToString(iRef);
          idx = idx + 1;
        }
      }
      else
      {
        res = "" + Convert.ToString(10 + npt);
        idx = 0;
        while (idx < npt)
        {
          iRaw = (int)YAPI._doubleToDecimal(rawValues[idx]);
          iRef = (int)YAPI._doubleToDecimal(refValues[idx]);
          res = "" + res + "," + Convert.ToString(iRaw) + "," + Convert.ToString(iRef);
          idx = idx + 1;
        }
      }
      return res;
    }

    public virtual double _applyCalibration(double rawValue)
    {
      if (rawValue == CURRENTVALUE_INVALID)
      {
        return CURRENTVALUE_INVALID;
      }
      if (this._caltyp == 0)
      {
        return rawValue;
      }
      if (this._caltyp < 0)
      {
        return CURRENTVALUE_INVALID;
      }
      if (!(this._calhdl != null))
      {
        return CURRENTVALUE_INVALID;
      }
      return this._calhdl(rawValue, this._caltyp, this._calpar, this._calraw, this._calref);
    }

    public virtual YMeasure _decodeTimedReport(double timestamp, List<int> report)
    {
      int i = 0;
      int byteVal = 0;
      int poww = 0;
      int minRaw = 0;
      int avgRaw = 0;
      int maxRaw = 0;
      double startTime = 0;
      double endTime = 0;
      double minVal = 0;
      double avgVal = 0;
      double maxVal = 0;

      startTime = this._prevTimedReport;
      endTime = timestamp;
      this._prevTimedReport = endTime;
      if (startTime == 0)
      {
        startTime = endTime;
      }
      if (report[0] > 0)
      {
        minRaw = report[1] + 0x100 * report[2];
        maxRaw = report[3] + 0x100 * report[4];
        avgRaw = report[5] + 0x100 * report[6] + 0x10000 * report[7];
        byteVal = report[8];
        if (((byteVal) & (0x80)) == 0)
        {
          avgRaw = avgRaw + 0x1000000 * byteVal;
        }
        else
        {
          avgRaw = avgRaw - 0x1000000 * (0x100 - byteVal);
        }
        minVal = this._decodeVal(minRaw);
        avgVal = this._decodeAvg(avgRaw);
        maxVal = this._decodeVal(maxRaw);
      }
      else
      {
        poww = 1;
        avgRaw = 0;
        byteVal = 0;
        i = 1;
        while (i < report.Count)
        {
          byteVal = report[i];
          avgRaw = avgRaw + poww * byteVal;
          poww = poww * 0x100;
          i = i + 1;
        }
        if (this._isScal)
        {
          avgVal = this._decodeVal(avgRaw);
        }
        else
        {
          if (((byteVal) & (0x80)) != 0)
          {
            avgRaw = avgRaw - poww;
          }
          avgVal = this._decodeAvg(avgRaw);
        }
        minVal = avgVal;
        maxVal = avgVal;
      }

      return new YMeasure(startTime, endTime, minVal, avgVal, maxVal);
    }

    public virtual double _decodeVal(int w)
    {
      double val = 0;
      val = w;
      if (this._isScal)
      {
        val = (val - this._offset) / this._scale;
      }
      else
      {
        val = YAPI._decimalToDouble(w);
      }
      if (this._caltyp != 0)
      {
        val = this._calhdl(val, this._caltyp, this._calpar, this._calraw, this._calref);
      }
      return val;
    }

    public virtual double _decodeAvg(int dw)
    {
      double val = 0;
      val = dw;
      if (this._isScal)
      {
        val = (val / 100 - this._offset) / this._scale;
      }
      else
      {
        val = val / this._decexp;
      }
      if (this._caltyp != 0)
      {
        val = this._calhdl(val, this._caltyp, this._calpar, this._calraw, this._calref);
      }
      return val;
    }

    /**
     * <summary>
     *   Continues the enumeration of sensors started using <c>yFirstSensor()</c>.
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   a pointer to a <c>YSensor</c> object, corresponding to
     *   a sensor currently online, or a <c>null</c> pointer
     *   if there are no more sensors to enumerate.
     * </returns>
     */
    public YSensor nextSensor()
    {
      string hwid = "";
      if (YAPI.YISERR(_nextFunction(ref hwid)))
        return null;
      if (hwid == "")
        return null;
      return FindSensor(hwid);
    }

    //--- (end of generated code: YSensor implementation)

    //--- (generated code: Sensor functions)

    /**
     * <summary>
     *   Starts the enumeration of sensors currently accessible.
     * <para>
     *   Use the method <c>YSensor.nextSensor()</c> to iterate on
     *   next sensors.
     * </para>
     * </summary>
     * <returns>
     *   a pointer to a <c>YSensor</c> object, corresponding to
     *   the first sensor currently online, or a <c>null</c> pointer
     *   if there are none.
     * </returns>
     */
    public static YSensor FirstSensor()
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
      err = YAPI.apiGetFunctionsByClass("Sensor", 0, p, size, ref neededsize, ref errmsg);
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
      return FindSensor(serial + "." + funcId);
    }



    //--- (end of generated code: Sensor functions)
  }
}