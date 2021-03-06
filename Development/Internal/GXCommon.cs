//
// --------------------------------------------------------------------------
//  Gurux Ltd
// 
//
//
// Filename:        $HeadURL$
//
// Version:         $Revision$,
//                  $Date$
//                  $Author$
//
// Copyright (c) Gurux Ltd
//
//---------------------------------------------------------------------------
//
//  DESCRIPTION
//
// This file is a part of Gurux Device Framework.
//
// Gurux Device Framework is Open Source software; you can redistribute it
// and/or modify it under the terms of the GNU General Public License 
// as published by the Free Software Foundation; version 2 of the License.
// Gurux Device Framework is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. 
// See the GNU General Public License for more details.
//
// More information of Gurux products: http://www.gurux.org
//
// This code is licensed under the GNU General Public License v2. 
// Full text may be retrieved at http://www.gnu.org/licenses/gpl-2.0.txt
//---------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gurux.DLMS.Internal
{
    /// <summary>
    /// Reserved for internal use.
    /// </summary>
    enum ActionType
    {
        None = 0,
        Index = 1,
        Count = 2,
    }    

    /// <summary>
    /// Reserved for internal use.
    /// </summary>
    class GXCommon
    {
        internal const byte HDLCFrameStartEnd = 0x7E;
        internal const byte InitialRequest = 0x1;
        internal const byte InitialResponce = 0x8;
        internal const byte AARQTag = 0x60;
        internal const byte AARETag = 0x61;
        internal static readonly byte[] LogicalNameObjectID = { 0x60, 0x85, 0x74, 0x05, 0x08, 0x01, 0x01 };
        internal static readonly byte[] ShortNameObjectID = { 0x60, 0x85, 0x74, 0x05, 0x08, 0x01, 0x02 };
        internal static readonly byte[] LLCSendBytes = { 0xE6, 0xE6, 0x00 };
        internal static readonly byte[] LLCReplyBytes = { 0xE6, 0xE7, 0x00 };

        /**
         * Reserved for internal use.
         * @param value
         * @param BitMask
         * @param val 
         */
        internal static void SetBits(byte value, byte BitMask, bool val)
        {
            value &= (byte)~BitMask;
            //Set bit.
            if (val)
            {
                value |= BitMask;
            }
            else //Clear bit.
            {
                value &= (byte)~BitMask;
            }
        }

        /**
         * Reserved for internal use.
         * @param value
         * @param BitMask
         * @return 
         */
        internal static bool GetBits(byte value, int BitMask)
        {
            return (value & BitMask) != 0;
        }


        internal static byte[] RawData(byte[] data, ref int index, int count)
        {
            byte[] buff = new byte[count];
            Array.Copy(data, index, buff, 0, count);
            index += count;
            return buff;
        }

        internal static byte[] Swap(byte[] data, int index, int count)
        {
            byte[] buff = new byte[count];
            Array.Copy(data, index, buff, 0, count);
            Array.Reverse(buff);
            return buff;
        }

        internal static byte[] Swap(List<byte> data, int index, int count)
        {
            return Swap(data.ToArray(), index, count);
        }

        internal static Int16 GetInt16(byte[] data, ref int index)
        {
            Int16 value = (Int16)(BitConverter.ToInt16(Swap(data, index, 2), 0) & 0xFFFF);
            index += 2;
            return value;
        }

        internal static int GetInt32(byte[] data, ref int index)
        {
            int value = BitConverter.ToInt32(Swap(data, index, 4), 0);
            index += 4;
            return value;
        }

        internal static long GetInt64(byte[] data, ref int index)
        {
            long value = BitConverter.ToInt64(Swap(data, index, 8), 0);
            index += 8;
            return value;
        }

        internal static float ToFloat(byte[] data, ref int index)
        {
            float value = BitConverter.ToSingle(Swap(data, index, 4), 0);
            index += 4;
            return value;
        }

        internal static double ToDouble(byte[] data, ref int index)
        {
            double value = BitConverter.ToDouble(Swap(data, index, 8), 0);
            index += 8;
            return value;
        }

        internal static ushort GetUInt16(byte[] data, ref int index)
        {
            ushort value = (ushort)(BitConverter.ToUInt16(Swap(data, index, 2), 0) & 0xFFFF);
            index += 2;
            return value;
        }

        internal static uint GetUInt32(byte[] data, ref int index)
        {
            uint value = BitConverter.ToUInt32(Swap(data, index, 4), 0);
            index += 4;
            return value;
        }

        internal static ulong GetUInt64(byte[] data, ref int index)
        {
            ulong value = BitConverter.ToUInt64(Swap(data, index, 8), 0);
            index += 8;
            return value;
        }

        internal static short GetInt16(List<byte> data, ref int index)
        {
            short value = BitConverter.ToInt16(Swap(data, index, 2), 0);
            index += 2;
            return value;
        }

        internal static int GetInt32(List<byte> data, ref int index)
        {
            int value = BitConverter.ToInt32(Swap(data, index, 4), 0);
            index += 4;
            return value;
        }

        internal static ushort GetUInt16(List<byte> data, ref int index)
        {
            ushort value = BitConverter.ToUInt16(Swap(data, index, 2), 0);
            index += 2;
            return value;
        }

        internal static uint GetUInt32(List<byte> data, ref int index)
        {
            uint value = BitConverter.ToUInt32(Swap(data, index, 4), 0);
            index += 4;
            return value;
        }

        internal static void SetInt16(short value, List<byte> data)
        {
            data.AddRange(Swap(BitConverter.GetBytes(value), 0, 2));
        }

        internal static void SetInt32(int value, List<byte> data)
        {
            data.AddRange(Swap(BitConverter.GetBytes(value), 0, 4));
        }

        internal static void SetUInt16(ushort value, List<byte> data)
        {
            data.AddRange(Swap(BitConverter.GetBytes(value), 0, 2));
        }

        internal static void SetUInt32(uint value, List<byte> data)
        {
            data.AddRange(Swap(BitConverter.GetBytes(value), 0, 4));
        }

        /// <summary>
        /// Get object count.
        /// </summary>
        /// <remarks>
        /// If first byte > 0x80 it will tell bytes count.
        /// </remarks>
        /// <param name="buff"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        internal static int GetObjectCount(byte[] buff, ref int index)
        {
            int cnt = buff[index++] & 0xFF;
            if (cnt > 0x80)
            {
                int tmp = cnt;
                cnt = 0;
                for (int pos = tmp - 0x81; pos > -1; --pos)
                {
                    cnt += (buff[index++] & 0xFF) << (8 * pos);
                }
            }
            return cnt;
        }


        internal static void SetObjectCount(int count, List<byte> buff)
        {
            SetObjectCount(count, buff, -1);
        }

        /// <summary>
        /// Set item count.
        /// </summary>
        /// <param name="count"></param>
        /// <param name="buff"></param>
        internal static void SetObjectCount(int count, List<byte> buff, int index)
        {
            if (count > 0x7F)
            {
                int cnt = 0;
                while (count >> (7 * ++cnt) > 0);
                if (index == -1)
                {
                    buff.Add((byte)(0x80 + cnt));
                }
                else
                {
                    buff.Insert(index, (byte)(0x80 + cnt));
                    ++index;
                }
                List<byte> tmp = new List<byte>(BitConverter.GetBytes(count));                
                tmp = tmp.GetRange(0, cnt);
                tmp.Reverse();
                if (index == -1)
                {
                    buff.AddRange(tmp);
                }
                else
                {
                    buff.InsertRange(index, tmp);
                }
            }
            else
            {
                if (index == -1)
                {
                    buff.Add((byte)count);
                }
                else
                {
                    buff.Insert(index, (byte)count);
                }
            }
        }

        /// <summary>
        /// Get object count. If first byte > 0x80 it will tell bytes count.
        /// </summary>
        /// <param name="buff"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        internal static int GetObjectCount(List<byte> buff, ref int index)
        {
            int cnt = buff[index++] & 0xFF;
            if (cnt > 0x80)
            {
                int tmp = cnt;
                cnt = 0;
                for (int pos = tmp - 0x81; pos > -1; --pos)
                {
                    cnt += (buff[index++] & 0xFF) << (8 * pos);
                }
            }
            return cnt;
        }

        /// <summary>
        /// Compares, whether two given arrays are similar.
        /// </summary>
        /// <param name="arr1">First array to compare.</param>
        /// <param name="index">Starting index of table, for first array.</param>
        /// <param name="arr2">Second array to compare.</param>
        /// <returns>True, if arrays are similar. False, if the arrays differ.</returns>
        public static bool Compare(byte[] arr1, ref int index, byte[] arr2)
        {
            if (arr1.Length - index < arr2.Length)
            {
                return false;
            }
            int pos;
            for (pos = 0; pos != arr2.Length; ++pos)
            {
                if (arr1[pos + index] != arr2[pos])
                {
                    return false;
                }
            }
            index += pos;
            return true;
        }

        static string ToBitString(byte value, int count)
        {
            if (count > 8)
            {
                count = 8;
            }
            char[] data = new char[count];
            for (int pos = 0; pos != count; ++pos)
            {
                if ((value & (1 << pos)) != 0)
                {
                    data[count - pos - 1] = '1';
                }
                else
                {
                    data[count - pos - 1] = '0';
                }
            }
            return new string(data);
        }


        /// <summary>
        /// Reserved for internal use.
        /// </summary>
        internal static object GetData(byte[] buff, ref int pos, ActionType action, out int count, out int index, ref DataType type, ref int cachePosition)
        {
            count = 0;
            index = 0;            
            object value = null;
            if (pos == buff.Length)
            {
                pos = -1;
                return null;
            }
            bool knownType = type != DataType.None;
            if (!knownType)
            {
                type = (DataType)buff[pos++];
            }
            if (type == DataType.None)
            {
                return value;
            }
            if (pos == buff.Length)
            {
                pos = -1;
                return null;
            }
            int size = buff.Length - pos;
            if (type == DataType.Array ||
                    type == DataType.Structure)
            {
                count = GXCommon.GetObjectCount(buff, ref pos);
                if (action == ActionType.Count)
                {
                    return value; //Don't go further. Only object's count is resolved.
                }
                if (cachePosition > pos)
                {
                    pos = cachePosition;
                }
                size = buff.Length - pos;
                if (count != 0 && size < 1)
                {
                    pos = -1;
                    return null;
                }                
                List<object> arr = new List<object>(count);                
                for (index = 0; index != count; ++index)
                {
                    DataType itemType = DataType.None;
                    int colCount, colIndex;                                           
                    int tmpPos = 0;
                    object tmp = GetData(buff, ref pos, ActionType.None, out colCount, out colIndex, ref itemType, ref tmpPos);
                    if (colCount == colIndex && pos != -1)
                    {
                        arr.Add(tmp);
                    }
                    if (pos == -1)
                    {
                        break;
                    }
                    else
                    {
                        cachePosition = pos;
                    }             
                }
                if (index == count && pos != -1)
                {
                    cachePosition = buff.Length;
                }
                value = arr.ToArray();
            }
            else if (type == DataType.Boolean)
            {
                value = buff[pos++] != 0;
            }
            else if (type == DataType.BitString)
            {
                int oldPos = pos;
                int cnt = GetObjectCount(buff, ref pos);
                size -= pos - oldPos;
                double t = cnt;
                t /= 8;
                if (cnt % 8 != 0)
                {
                    ++t;
                }
                int byteCnt = (int)Math.Floor(t);
                if (size < byteCnt) //If there is not enought data available.
                {
                    pos = -1;
                    return null;
                }         
                string str = "";
                while (cnt > 0)
                {
                    str += ToBitString(buff[pos++], cnt);
                    cnt -= 8;
                }                               
                value = str;
            }
            else if (type == DataType.Int32)
            {
                if (size < 4) //If there is not enought data available.
                {
                    pos = -1;
                    return null;
                }
                value = GXCommon.GetInt32(buff, ref pos);
            }
            else if (type == DataType.UInt32)
            {
                if (size < 4) //If there is not enought data available.
                {
                    pos = -1;
                    return null;
                }
                value = GXCommon.GetUInt32(buff, ref pos);
            }
            else if (type == DataType.StringUTF8)
            {
                int len = 0;
                if (knownType)
                {
                    len = buff.Length;
                }
                else
                {
                    len = GXCommon.GetObjectCount(buff, ref pos);
                    if (buff.Length - pos < len) //If there is not enought data available.
                    {
                        pos = -1;
                        return null;
                    }
                }
                if (len > 0)
                {
                    value = ASCIIEncoding.UTF8.GetString(GXCommon.RawData(buff, ref pos, len));
                }
                else
                {
                    value = "";
                }
            }
            else if (type == DataType.String)
            {
                int len = 0;
                if (knownType)
                {
                    len = buff.Length;
                }
                else
                {
                    len = GXCommon.GetObjectCount(buff, ref pos);
                    if (buff.Length - pos < len) //If there is not enought data available.
                    {
                        pos = -1;
                        return null;
                    }
                }
                if (len > 0)
                {
                    bool octetString = false;
                    if (knownType)
                    {
                        //Check that this is not octet string.
                        foreach (byte it in buff)
                        {                            
                            if (it != 0 && it < 0x20)
                            {
                                octetString = true;
                                break;
                            }
                        }
                    }
                    if (octetString)
                    {
                        StringBuilder sb = new StringBuilder(3 * buff.Length);
                        foreach (byte it in buff)
                        {
                            sb.Append(it);
                            sb.Append('.');
                        }
                        sb.Remove(sb.Length - 1, 1);
                        value = sb.ToString();
                    }
                    else
                    {
                        //Remove '\0' from string if used.
                        while (len > 0 && buff[len - 1] == 0)
                        {
                            --len;
                        }
                        value = ASCIIEncoding.ASCII.GetString(GXCommon.RawData(buff, ref pos, len));
                    }
                }
            }
            //Example Logical name is octet string, so do not change to string...
            else if (type == DataType.OctetString)
            {
                int len = 0;
                if (knownType)
                {
                    len = buff.Length;
                }
                else
                {
                    len = GXCommon.GetObjectCount(buff, ref pos);
                    if (buff.Length - pos < len) //If there is not enought data available.
                    {
                        pos = -1;
                        return null;
                    }
                }
                value = GXCommon.RawData(buff, ref pos, len);
            }
            else if (type == DataType.BinaryCodedDesimal)
            {
                int len;
                if (knownType)
                {
                    len = buff.Length;
                }
                else
                {
                    len = GXCommon.GetObjectCount(buff, ref pos);
                }
                StringBuilder bcd = new StringBuilder(len * 2);
                for (int a = 0; a != len; ++a)
                {
                    int idHigh = buff[pos] >> 4;
                    int idLow = buff[pos] & 0x0F;
                    ++pos;
                    bcd.Append(string.Format("{0}{1}", idHigh, idLow));
                }
                return bcd.ToString();
            }
            else if (type == DataType.Int8)
            {
                value = (sbyte)buff[pos++];
            }
            else if (type == DataType.Int16)
            {
                if (size < 2) //If there is not enought data available.
                {
                    pos = -1;
                    return null;
                }
                value = GXCommon.GetInt16(buff, ref pos);
            }
            else if (type == DataType.UInt8)
            {
                value = buff[pos++];
            }
            else if (type == DataType.UInt16)
            {
                if (size < 2) //If there is not enought data available.
                {
                    pos = -1;
                    return null;
                }
                value = GXCommon.GetUInt16(buff, ref pos);
            }
            else if (type == DataType.CompactArray)
            {
                throw new Exception("Invalid data type.");
            }
            else if (type == DataType.Int64)
            {
                if (size < 8) //If there is not enought data available.
                {
                    pos = -1;
                    return null;
                }
                value = GXCommon.GetInt64(buff, ref pos);
            }
            else if (type == DataType.UInt64)
            {
                if (size < 8) //If there is not enought data available.
                {
                    pos = -1;
                    return null;
                }
                value = GXCommon.GetUInt64(buff, ref pos);
            }
            else if (type == DataType.Enum)
            {
                if (size < 1) //If there is not enought data available.
                {
                    pos = -1;
                    return null;
                }
                value = buff[pos++];
            }
            else if (type == DataType.Float32)
            {
                if (size < 4) //If there is not enought data available.
                {
                    pos = -1;
                    return null;
                }
                value = GXCommon.ToFloat(buff, ref pos);
            }
            else if (type == DataType.Float64)
            {
                if (size < 8) //If there is not enought data available.
                {
                    pos = -1;
                    return null;
                }
                value = GXCommon.ToDouble(buff, ref pos);
            }
            else if (type == DataType.DateTime)
            {
                if (size < 12) //If there is not enought data available.
                {
                    pos = -1;
                    return null;
                }
                if (!knownType)
                {
                    byte sz = buff[pos++];
                    if (sz != 12)
                    {
                        throw new Exception("Invalid datetime format.");
                    }
                }
                GXDateTime dt = new GXDateTime();
                //Get year.
                int year = GXCommon.GetUInt16(buff, ref pos);
                if (year == 0xFFFF || year == 0)
                {
                    year = DateTime.MinValue.Year;
                    dt.Skip |= DateTimeSkips.Year;
                }
                //Get month
                int month = buff[pos++];
                if (month == 0 || month == 0xFF || month == 0xFE || month == 0xFD)
                {
                    month = 1;
                    dt.Skip |= DateTimeSkips.Month;
                }
                int day = buff[pos++];
                if (day < 1 || day == 0xFF)
                {
                    day = 1;
                    dt.Skip |= DateTimeSkips.Day;
                }
                else if (day == 0xFD || day == 0xFE)
                {
                    day = DateTime.DaysInMonth(year, month) + (sbyte)day + 2;
                }
                //Skip week day
                ++pos;
                //Get time.
                int hours = buff[pos++];
                if (hours == 0xFF)
                {
                    hours = 0;
                    dt.Skip |= DateTimeSkips.Hour;
                }
                int minutes = buff[pos++];
                if (minutes == 0xFF)
                {
                    minutes = 0;
                    dt.Skip |= DateTimeSkips.Minute;
                }
                int seconds = buff[pos++];
                if (seconds == 0xFF)
                {
                    seconds = 0;
                    dt.Skip |= DateTimeSkips.Second;
                }
                int milliseconds = buff[pos++];
                if (milliseconds != 0xFF && milliseconds != 0)
                {
                    milliseconds *= 10;
                }
                else
                {
                    milliseconds = 0;
                    dt.Skip |= DateTimeSkips.Ms;
                }
                int deviation = GXCommon.GetInt16(buff, ref pos);                
                dt.Status = (ClockStatus)buff[pos++];                
                if ((deviation & 0xFFFF) != 0x8000)
                {
                    dt.Value = DateTime.SpecifyKind(new DateTime(year, month, day, hours, minutes, seconds, milliseconds), DateTimeKind.Utc);
                    dt.Value = dt.Value.AddMinutes(deviation);
                    dt.Value = dt.Value.ToLocalTime();
                }
                else
                {
                    dt.Value = new DateTime(year, month, day, hours, minutes, seconds, milliseconds);
                }                
                value = dt;
            }
            else if (type == DataType.Date)
            {
                if (size < 5) //If there is not enought data available.
                {
                    pos = 0xFF;
                    return null;
                }
                //Get year.
                int year = GXCommon.GetUInt16(buff, ref pos);                
                //Iskra meter returns bytes in wrong order.
                if (year != 0xFFFF && year > 2100)
                {
                    pos -= 2;
                    year = buff[pos++] | buff[pos++] << 8;                    
                }
                //Get month
                int month = buff[pos++];
                int day = buff[pos++];
                //Skip week day
                int DayOfWeek = buff[pos++];
                //If day of week are not used.                
                GXDateTime dt = new GXDateTime(year, month, day, -1, -1, 1, -1);
                return dt;
            }
            else if (type == DataType.Time)
            {
                if (size < 4) //If there is not enought data available.
                {
                    pos = -1;
                    return null;
                }
                //Get time.
                int hours = buff[pos++];                
                int minutes = buff[pos++];
                int seconds = buff[pos++];
                int milliseconds = buff[pos++];
                GXDateTime dt = new GXDateTime(-1, -1, -1, hours, minutes, seconds, milliseconds);
                value = dt;
            }
            else
            {
                throw new Exception("Invalid data type.");
            }
            return value;
        }

        public static DataType GetValueType(object value)
        {
            if (value == null)
            {
                return DataType.None;
            }
            if (value is byte[])
            {
                return DataType.OctetString;
            }
            if (value.GetType().IsEnum)
            {
                return DataType.Enum;
            }
            if (value is byte)
            {
                return DataType.UInt8;
            }
            if (value is sbyte)
            {
                return DataType.Int8;
            }
            if (value is UInt16)
            {
                return DataType.UInt16;
            }
            if (value is Int16)
            {
                return DataType.Int16;
            }
            if (value is UInt32)
            {
                return DataType.UInt32;
            }
            if (value is Int32)
            {
                return DataType.Int32;
            } 
            if (value is UInt64)
            {
                return DataType.UInt64;
            }
            if (value is Int64)
            {
                return DataType.Int64;
            }
            if (value is DateTime || value is GXDateTime)
            {
                return DataType.DateTime;
            }
            if (value.GetType().IsArray)
            {
                return DataType.Array;
            }
            if (value is string)
            {
                return DataType.String;
            }
            if (value is bool)
            {
                return DataType.Boolean;
            }
            if (value is float)
            {
                return DataType.Float32;
            }
            if (value is double)
            {
                return DataType.Float64;
            }
            throw new Exception("Invalid value.");
        }

        /// <summary>
        /// Reserved for internal use.
        /// </summary>
        /// <param name="buff"></param>
        /// <param name="type"></param>
        /// <param name="value"></param>
        public static void SetData(List<byte> buff, DataType type, object value)
        {
            bool reverse = true;
            List<byte> tmp = new List<byte>();
             //If byte array is added do not add type.
            if ((type == DataType.Array || type == DataType.Structure) && value is byte[])
            {
                buff.AddRange((byte[]) value);
                return;
            }
            if (type == DataType.None)
            {
                value = null;
            }
            else if (type == DataType.Boolean)
            {
                value = (byte) (Convert.ToBoolean(value) ? 1 : 0);
            }
            else if (type == DataType.BitString)
            {
                byte val = 0;
                int index = 0;
                if (value is string)
                {
                    string str = value as string;
                    SetObjectCount(str.Length, tmp);
                    foreach (char it in str)
                    {
                        if (it == '1')
                        {
                            val |= (byte)(1 << index++);
                        }
                        else if (it == '0')
                        {
                            index++;
                        }
                        else 
                        {
                            throw new ArgumentException("Not a bit string.");
                        }
                        if (index == 8)
                        {
                            index = 0;
                            tmp.Add(val);
                            val = 0;
                        }
                    }
                    if (index != 0)
                    {
                        tmp.Add(val);
                    }
                    value = tmp.ToArray();
                    reverse = false;
                }
                else if (value is byte[])
                {
                    byte[] arr = value as byte[];
                    SetObjectCount(arr.Length, tmp);
                    tmp.AddRange(arr);
                    value = tmp.ToArray();
                    reverse = false;
                }
                else if (value is bool[])
                {
                    bool[] arr = value as bool[];
                    SetObjectCount(arr.Length, tmp);
                    foreach (bool it in arr)
                    {
                        if (it)
                        {
                            val |= (byte)(1 << index++);
                        }
                        else
                        {
                            ++index;
                        }
                        if (index == 8)
                        {
                            index = 0;
                            tmp.Add(val);
                            val = 0;
                        }
                    }
                    if (index != 0)
                    {
                        tmp.Add(val);
                    }
                    value = tmp.ToArray();
                    reverse = false;
                }
                else if (value == null)
                {
                    tmp.Add(0);
                    value = tmp.ToArray();
                    reverse = false;
                }
                else
                {
                    throw new Exception("BitString must give as string.");
                }
            }
            else if (type == DataType.Int32)
            {
                value = Convert.ToInt32(value);
            }
            else if (type == DataType.UInt32)
            {
                value = Convert.ToUInt32(value);
            }
            else if (type == DataType.String)
            {                
                if (value != null)
                {
                    string str = value.ToString();
                    SetObjectCount(str.Length, tmp);
                    tmp.AddRange(ASCIIEncoding.ASCII.GetBytes(str));
                }
                else
                {
                    SetObjectCount(0, tmp);
                }
                value = tmp.ToArray();
                reverse = false;
            }
            else if (type == DataType.StringUTF8)
            {                
                if (value != null)
                {
                    string str = value.ToString();
                    byte[] tmp1 = ASCIIEncoding.UTF8.GetBytes(str);
                    SetObjectCount(tmp1.Length, tmp);
                    tmp.AddRange(tmp1);
                }
                else
                {
                    SetObjectCount(0, tmp);
                }
                value = tmp.ToArray();
                reverse = false;
            }                
            else if (type == DataType.Array || type == DataType.Structure)
            {
                if (value != null)
                {
                    Array arr = (Array)value;
                    SetObjectCount(arr.Length, tmp);
                    foreach (object it in arr)
                    {
                        SetData(tmp, GetValueType(it), it);
                    }
                }
                else
                {
                    SetObjectCount(0, tmp);
                }
                value = tmp.ToArray();
                reverse = false;
            }
            //Example Logical name is octet string, so do not change to string...
            else if (type == DataType.OctetString)
            {
                reverse = false;
                if (value is string)
                {
                    string str = value as string;
                    string[] items = str.Split('.');
                    if (items.Length == 1 && items[0].Equals(str))
                    {
                        SetObjectCount(str.Length, tmp);
                        tmp.AddRange(ASCIIEncoding.ASCII.GetBytes(str));
                        value = tmp.ToArray(); 
                    }
                    else
                    {
                        SetObjectCount(items.Length, tmp);
                        foreach (string it in items)
                        {
                            tmp.Add(byte.Parse(it));
                        }
                        value = tmp.ToArray();                        
                    }
                }
                else if (value == null)
                {
                    SetObjectCount(0, tmp);
                    value = tmp.ToArray();                    
                }
                else if (value is byte[])
                {
                    SetObjectCount((value as byte[]).Length, tmp);
                    tmp.AddRange(value as byte[]);
                    value = tmp.ToArray();                    
                }
                else if (value is GXDateTime)
                {                    
                    value = GetDateTime(value as GXDateTime);
                }
                else if (value is DateTime)
                {
                    value = GetDateTime(new GXDateTime(Convert.ToDateTime(value)));
                }
                else
                {
                    value = Convert.ToString(value);
                }
            }
            else if (type == DataType.BinaryCodedDesimal)
            {
                if (!(value is string))
                {
                    throw new Exception("BCD value must give as string.");
                }
                string str = value.ToString().Trim();
                int len = str.Length;
                if (len % 2 != 0)
                {
                    str = "0" + str;
                    ++len;
                }
                len /= 2;
                List<byte> val = new List<byte>(len);
                val.Add((byte)(len));
                for (int pos = 0; pos != len; ++pos)
                {
                    byte ch1 = byte.Parse(str.Substring(2 * pos, 1));
                    byte ch2 = byte.Parse(str.Substring(2 * pos + 1, 1));
                    val.Add((byte)(ch1 << 4 | ch2));
                }
                reverse = false;
                value = val.ToArray();
            }
            else if (type == DataType.Int8)
            {
                value = Convert.ToSByte(value);
            }
            else if (type == DataType.Int16)
            {
                value = Convert.ToInt16(value);
            }
            else if (type == DataType.UInt8)
            {
                value = Convert.ToByte(value);
            }
            else if (type == DataType.UInt16)
            {
                value = Convert.ToUInt16(value);
            }
            else if (type == DataType.CompactArray)
            {
                throw new Exception("Invalid data type.");
            }
            else if (type == DataType.Int64)
            {
                value = Convert.ToInt64(value);
            }
            else if (type == DataType.UInt64)
            {
                value = Convert.ToUInt64(value);
            }
            else if (type == DataType.Enum)
            {
                value = Convert.ToByte(value);
            }
            else if (type == DataType.Float32)
            {
                value = Convert.ToSingle(value);
            }
            else if (type == DataType.Float64)
            {
                value = Convert.ToDouble(value);
            }
            else if (type == DataType.DateTime)
            {
                type = DataType.OctetString;
                if (value is GXDateTime)
                {
                    value = GetDateTime(value as GXDateTime);
                }
                else
                {
                    value = GetDateTime(new GXDateTime(Convert.ToDateTime(value)));
                }
                reverse = false;
            }
            else if (type == DataType.Date)
            {
                GXDateTime dt;
                if (value is GXDateTime)
                {
                    dt = value as GXDateTime;
                }
                else if (value is DateTime)
                {
                    dt = new GXDateTime((DateTime)value);
                }
                else
                {
                    throw new Exception("Invalid date format.");
                }
                type = DataType.OctetString;
                //Add size
                tmp.Add(5);
                //Add year.
                if ((dt.Skip & DateTimeSkips.Year) != 0)
                {
                    GXCommon.SetUInt16(0xFFFF, tmp);
                }
                else
                {
                    GXCommon.SetUInt16((ushort)dt.Value.Year, tmp);
                }
                //Add month.
                if (dt.DaylightSavingsBegin)
                {
                    tmp.Add(0xFE);
                }
                else if (dt.DaylightSavingsEnd)
                {
                    tmp.Add(0xFD);
                }
                else if ((dt.Skip & DateTimeSkips.Month) != 0)
                {
                    tmp.Add(0xFF);
                }
                else
                {
                    tmp.Add((byte)dt.Value.Month);
                }
                
                if ((dt.Skip & DateTimeSkips.Day) != 0)
                {
                    tmp.Add(0xFF);
                }
                else
                {
                    tmp.Add((byte)dt.Value.Day);
                }                
                //Week day is not spesified.
                tmp.Add(0xFF);
                value = tmp.ToArray();
                reverse = false;
            }
            else if (type == DataType.Time)
            {
                GXDateTime dt;
                if (value is GXDateTime)
                {
                    dt = value as GXDateTime;
                }
                else if (value is DateTime)
                {
                    dt = new GXDateTime((DateTime)value);
                }
                else
                {
                    throw new Exception("Invalid date format.");
                }
                type = DataType.OctetString;
                //Add size
                tmp.Add(4);
                //Add time.
                if ((dt.Skip & DateTimeSkips.Hour) != 0)
                {
                    tmp.Add(0xFF);
                }
                else
                {
                    tmp.Add((byte)dt.Value.Hour);
                } 
                
                if ((dt.Skip & DateTimeSkips.Minute) != 0)
                {
                    tmp.Add(0xFF);
                }
                else
                {
                    tmp.Add((byte)dt.Value.Minute);
                } 
                
                if ((dt.Skip & DateTimeSkips.Second) != 0)
                {
                    tmp.Add(0xFF);
                }
                else
                {
                    tmp.Add((byte)dt.Value.Second);
                } 
                tmp.Add((byte)0xFF); //Hundredths of second is not used.
                value = tmp.ToArray();
                reverse = false;
            }
            else
            {
                throw new Exception("Invalid data type.");
            }
            buff.Add((byte)type);
            if (value != null)
            {
                byte[] data = Gurux.Shared.GXCommon.GetAsByteArray(value);
                if (reverse)
                {
                    Array.Reverse(data);
                }
                buff.AddRange(data);
            }
        }

        static byte[] GetDateTime(GXDateTime dt)
        {
            if (dt.Value == DateTime.MinValue)
            {
                dt.Value = DateTime.SpecifyKind(new DateTime(2000, 1, 1).Date, DateTimeKind.Utc);
            }
            else if (dt.Value == DateTime.MaxValue)
            {
                dt.Value = DateTime.SpecifyKind(DateTime.Now.AddYears(1).Date, DateTimeKind.Utc);
            }
            DateTime tm;
            //If used normal time.
            if ((dt.Skip & DateTimeSkips.Devitation) == 0)
            {
                tm = dt.Value;
            }
            else //If devitation is skipped.
            {
                tm = dt.Value.ToUniversalTime();
            }            
            List<byte> tmp = new List<byte>();            
            //Add size
            tmp.Add(12);
            if ((dt.Skip & DateTimeSkips.Year) == 0)
            {
                GXCommon.SetUInt16((ushort)tm.Year, tmp);
            }
            else
            {
                GXCommon.SetUInt16((ushort)0xFFFF, tmp);
            }
            if ((dt.Skip & DateTimeSkips.Month) == 0)
            {
                if (dt.DaylightSavingsBegin)
                {
                    tmp.Add(0xFE);
                }
                else if (dt.DaylightSavingsEnd)
                {
                    tmp.Add(0xFD);
                }
                else
                {
                    tmp.Add((byte)tm.Month);
                }
            }
            else
            {
                tmp.Add(0xFF);
            }
            if ((dt.Skip & DateTimeSkips.Day) == 0)
            {
                tmp.Add((byte)tm.Day);
            }
            else
            {
                tmp.Add(0xFF);
            }            
            //Week day is not spesified.
            //Standard defines. tmp.Add(0xFF);
            tmp.Add(0xFF);
            //Add time.
            if ((dt.Skip & DateTimeSkips.Hour) == 0)
            {
                tmp.Add((byte)tm.Hour);
            }
            else
            {
                tmp.Add(0xFF);
            }

            if ((dt.Skip & DateTimeSkips.Minute) == 0)
            {
                tmp.Add((byte)tm.Minute);
            }
            else
            {
                tmp.Add(0xFF);
            }
            if ((dt.Skip & DateTimeSkips.Second) == 0)
            {
                tmp.Add((byte)tm.Second);
            }
            else
            {
                tmp.Add(0xFF);
            }

            if ((dt.Skip & DateTimeSkips.Ms) == 0)
            {
                tmp.Add((byte)(tm.Millisecond / 10));
            }
            else
            {
                tmp.Add((byte)0xFF); //Hundredths of second is not used.                
            }            
            //Add deviation.
            if ((dt.Skip & DateTimeSkips.Devitation) == 0)
            {
                short devitation = (short)TimeZone.CurrentTimeZone.GetUtcOffset(DateTime.Now).TotalMinutes;                   
                GXCommon.SetInt16(devitation, tmp);                
            }
            else //deviation not used.
            {
                tmp.Add((byte)0x80);
                tmp.Add((byte)0x00);
            }
            //Add clock_status
            tmp.Add((byte)dt.Status);
            return tmp.ToArray();
        }
    }

    /// <summary>
    /// Reserved for internal use.
    /// </summary>
    enum FrameType : byte
    {
        //////////////////////////////////////////////////////////
        // This command is used to set the secondary station in connected mode and reset
        // its sequence number variables.
        SNRM = 0x93, // Set Normal Response Mode (SNRM)
        //////////////////////////////////////////////////////////
        // This response is used to confirm that the secondary received and acted on an SNRM or DISC command.
        UA = 0x73, // Unnumbered Acknowledge (UA)
        //////////////////////////////////////////////////////////
        // This command and response is used to transfer a block of data together with its sequence number.
        // The command also includes the sequence number of the next frame the transmitting station expects to see.
        // This way, it works as an RR. Like RR, it enables transmission of I frames from the opposite side.
        Information = 0x10, // Information (I)
        //////////////////////////////////////////////////////////
        // This response is used to indicate an error condition. The two most likely error conditions are:
        // Invalid command or Sequence number problem.
        Rejected = 0x97,  // Frame Reject
        //////////////////////////////////////////////////////////
        // This command is used to terminate the connection.
        Disconnect = 0x53,
        //////////////////////////////////////////////////////////
        // This response is used to inform the primary station that the secondary is disconnected.
        DisconnectMode = 0x1F // Disconnected Mode
    }

    /**
     * Reserved for internal use.
     */
    internal enum HDLCInfo : byte
    {
        MaxInfoTX = 0x5,
        MaxInfoRX = 0x6,
        WindowSizeTX = 0x7,
        WindowSizeRX = 0x8
    }
}
