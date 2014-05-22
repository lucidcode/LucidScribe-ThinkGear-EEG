/*********************************************************************
 *
 * $Id: yocto_files.cs 15131 2014-02-28 10:23:25Z seb $
 *
 * Implements yFindFiles(), the high-level API for Files functions
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

  //--- (generated code: YFileRecord class start)
  public class YFileRecord
  {
    //--- (end of generated code: YFileRecord class start)
    //--- (generated code: YFileRecord definitions)

    protected string _name;
    protected int _size = 0;
    protected int _crc = 0;
    //--- (end of generated code: YFileRecord definitions)

    public YFileRecord(string data)
    {
      YAPI.TJsonParser p;
      Nullable<YAPI.TJSONRECORD> node;
      //--- (generated code: YFileRecord attributes initialization)
      //--- (end of generated code: YFileRecord attributes initialization)
      p = new YAPI.TJsonParser(data, false);
      node = p.GetChildNode(null, "name");
      this._name = node.Value.svalue;
      node = p.GetChildNode(null, "size");
      this._size = (int)node.Value.ivalue;
      node = p.GetChildNode(null, "crc");
      this._crc = (int)node.Value.ivalue;
    }

    //--- (generated code: YFileRecord implementation)


    public virtual string get_name()
    {
      return this._name;
    }

    public virtual int get_size()
    {
      return this._size;
    }

    public virtual int get_crc()
    {
      return this._crc;
    }

    //--- (end of generated code: YFileRecord implementation)
  }



  //--- (generated code: YFiles class start)
  /**
   * <summary>
   *   The filesystem interface makes it possible to store files
   *   on some devices, for instance to design a custom web UI
   *   (for networked devices) or to add fonts (on display
   *   devices).
   * <para>
   * </para>
   * <para>
   * </para>
   * </summary>
   */
  public class YFiles : YFunction
  {
    //--- (end of generated code: YFiles class start)
    //--- (generated code: YFiles definitions)
    public new delegate void ValueCallback(YFiles func, string value);
    public new delegate void TimedReportCallback(YFiles func, YMeasure measure);

    public const int FILESCOUNT_INVALID = YAPI.INVALID_UINT;
    public const int FREESPACE_INVALID = YAPI.INVALID_UINT;
    protected int _filesCount = FILESCOUNT_INVALID;
    protected int _freeSpace = FREESPACE_INVALID;
    protected ValueCallback _valueCallbackFiles = null;
    //--- (end of generated code: YFiles definitions)

    public YFiles(string func)
      : base(func)
    {
      _className = "Files";
      //--- (generated code: YFiles attributes initialization)
      //--- (end of generated code: YFiles attributes initialization)
    }


    //--- (generated code: YFiles implementation)

    protected override void _parseAttr(YAPI.TJSONRECORD member)
    {
      if (member.name == "filesCount")
      {
        _filesCount = (int)member.ivalue;
        return;
      }
      if (member.name == "freeSpace")
      {
        _freeSpace = (int)member.ivalue;
        return;
      }
      base._parseAttr(member);
    }

    /**
     * <summary>
     *   Returns the number of files currently loaded in the filesystem.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   an integer corresponding to the number of files currently loaded in the filesystem
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YFiles.FILESCOUNT_INVALID</c>.
     * </para>
     */
    public int get_filesCount()
    {
      if (this._cacheExpiration <= YAPI.GetTickCount())
      {
        if (this.load(YAPI.DefaultCacheValidity) != YAPI.SUCCESS)
        {
          return FILESCOUNT_INVALID;
        }
      }
      return this._filesCount;
    }

    /**
     * <summary>
     *   Returns the free space for uploading new files to the filesystem, in bytes.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   an integer corresponding to the free space for uploading new files to the filesystem, in bytes
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YFiles.FREESPACE_INVALID</c>.
     * </para>
     */
    public int get_freeSpace()
    {
      if (this._cacheExpiration <= YAPI.GetTickCount())
      {
        if (this.load(YAPI.DefaultCacheValidity) != YAPI.SUCCESS)
        {
          return FREESPACE_INVALID;
        }
      }
      return this._freeSpace;
    }

    /**
     * <summary>
     *   Retrieves a filesystem for a given identifier.
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
     *   This function does not require that the filesystem is online at the time
     *   it is invoked. The returned object is nevertheless valid.
     *   Use the method <c>YFiles.isOnline()</c> to test if the filesystem is
     *   indeed online at a given time. In case of ambiguity when looking for
     *   a filesystem by logical name, no error is notified: the first instance
     *   found is returned. The search is performed first by hardware name,
     *   then by logical name.
     * </para>
     * </summary>
     * <param name="func">
     *   a string that uniquely characterizes the filesystem
     * </param>
     * <returns>
     *   a <c>YFiles</c> object allowing you to drive the filesystem.
     * </returns>
     */
    public static YFiles FindFiles(string func)
    {
      YFiles obj;
      obj = (YFiles)YFunction._FindFromCache("Files", func);
      if (obj == null)
      {
        obj = new YFiles(func);
        YFunction._AddToCache("Files", func, obj);
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
      this._valueCallbackFiles = callback;
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
      if (this._valueCallbackFiles != null)
      {
        this._valueCallbackFiles(this, value);
      }
      else
      {
        base._invokeValueCallback(value);
      }
      return 0;
    }

    public virtual byte[] sendCommand(string command)
    {
      string url;
      url = "files.json?a=" + command;
      // may throw an exception
      return this._download(url);
    }

    /**
     * <summary>
     *   Reinitializes the filesystem to its clean, unfragmented, empty state.
     * <para>
     *   All files previously uploaded are permanently lost.
     * </para>
     * </summary>
     * <returns>
     *   <c>YAPI.SUCCESS</c> if the call succeeds.
     * </returns>
     * <para>
     *   On failure, throws an exception or returns a negative error code.
     * </para>
     */
    public virtual int format_fs()
    {
      byte[] json;
      string res;
      json = this.sendCommand("format");
      res = this._json_get_key(json, "res");
      if (!(res == "ok")) { this._throw(YAPI.IO_ERROR, "format failed"); return YAPI.IO_ERROR; }
      return YAPI.SUCCESS;
    }

    /**
     * <summary>
     *   Returns a list of YFileRecord objects that describe files currently loaded
     *   in the filesystem.
     * <para>
     * </para>
     * </summary>
     * <param name="pattern">
     *   an optional filter pattern, using star and question marks
     *   as wildcards. When an empty pattern is provided, all file records
     *   are returned.
     * </param>
     * <returns>
     *   a list of <c>YFileRecord</c> objects, containing the file path
     *   and name, byte size and 32-bit CRC of the file content.
     * </returns>
     * <para>
     *   On failure, throws an exception or returns an empty list.
     * </para>
     */
    public virtual List<YFileRecord> get_list(string pattern)
    {
      byte[] json;
      List<string> filelist = new List<string>();
      List<YFileRecord> res = new List<YFileRecord>();
      json = this.sendCommand("dir&f=" + pattern);
      filelist = this._json_get_array(json);
      res.Clear();
      for (int ii = 0; ii < filelist.Count; ii++)
      {
        res.Add(new YFileRecord(filelist[ii]));
      }
      return res;
    }

    /**
     * <summary>
     *   Downloads the requested file and returns a binary buffer with its content.
     * <para>
     * </para>
     * </summary>
     * <param name="pathname">
     *   path and name of the file to download
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
     *   Uploads a file to the filesystem, to the specified full path name.
     * <para>
     *   If a file already exists with the same path name, its content is overwritten.
     * </para>
     * </summary>
     * <param name="pathname">
     *   path and name of the new file to create
     * </param>
     * <param name="content">
     *   binary buffer with the content to set
     * </param>
     * <returns>
     *   <c>YAPI.SUCCESS</c> if the call succeeds.
     * </returns>
     * <para>
     *   On failure, throws an exception or returns a negative error code.
     * </para>
     */
    public virtual int upload(string pathname, byte[] content)
    {
      return this._upload(pathname, content);
    }

    /**
     * <summary>
     *   Deletes a file, given by its full path name, from the filesystem.
     * <para>
     *   Because of filesystem fragmentation, deleting a file may not always
     *   free up the whole space used by the file. However, rewriting a file
     *   with the same path name will always reuse any space not freed previously.
     *   If you need to ensure that no space is taken by previously deleted files,
     *   you can use <c>format_fs</c> to fully reinitialize the filesystem.
     * </para>
     * </summary>
     * <param name="pathname">
     *   path and name of the file to remove.
     * </param>
     * <returns>
     *   <c>YAPI.SUCCESS</c> if the call succeeds.
     * </returns>
     * <para>
     *   On failure, throws an exception or returns a negative error code.
     * </para>
     */
    public virtual int remove(string pathname)
    {
      byte[] json;
      string res;
      json = this.sendCommand("del&f=" + pathname);
      res = this._json_get_key(json, "res");
      if (!(res == "ok")) { this._throw(YAPI.IO_ERROR, "unable to remove file"); return YAPI.IO_ERROR; }
      return YAPI.SUCCESS;
    }

    /**
     * <summary>
     *   Continues the enumeration of filesystems started using <c>yFirstFiles()</c>.
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   a pointer to a <c>YFiles</c> object, corresponding to
     *   a filesystem currently online, or a <c>null</c> pointer
     *   if there are no more filesystems to enumerate.
     * </returns>
     */
    public YFiles nextFiles()
    {
      string hwid = "";
      if (YAPI.YISERR(_nextFunction(ref hwid)))
        return null;
      if (hwid == "")
        return null;
      return FindFiles(hwid);
    }

    //--- (end of generated code: YFiles implementation)

    //--- (generated code: Files functions)

    /**
     * <summary>
     *   Starts the enumeration of filesystems currently accessible.
     * <para>
     *   Use the method <c>YFiles.nextFiles()</c> to iterate on
     *   next filesystems.
     * </para>
     * </summary>
     * <returns>
     *   a pointer to a <c>YFiles</c> object, corresponding to
     *   the first filesystem currently online, or a <c>null</c> pointer
     *   if there are none.
     * </returns>
     */
    public static YFiles FirstFiles()
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
      err = YAPI.apiGetFunctionsByClass("Files", 0, p, size, ref neededsize, ref errmsg);
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
      return FindFiles(serial + "." + funcId);
    }



    //--- (end of generated code: Files functions)

  }
}