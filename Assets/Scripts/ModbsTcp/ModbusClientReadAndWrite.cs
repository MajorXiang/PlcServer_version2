﻿using System.Collections.Generic;
using UnityEngine;
using System;
using Plc.Rpc;
using System.Threading;
namespace Plc.ModbusTcp
{
    public partial class ModbusTcpClient
    {
        public ESceneNameType eSceneNameType = ESceneNameType.None;//作为场景类型数据从.dat文件外部读取
        private int enumValueMaxCount = 72;//plc end mac address  40072
        private int enumValueMinCount = 0;//plc start mac address  40001
        private string initValue = "1";
        private string macAddress = "1";
        private int wirteValueSleepTime = 5;
        void ClientConnectedEvent()
        {
            PlcServerDataInit();
        }
        
        #region  Client data Init

        void PlcServerDataInit()
        {
            for (int i = 0; i < enumValueMaxCount; i++)
            {
                Thread.Sleep(wirteValueSleepTime);
                // ModbusWrite(macAddress, i.ToString(), initValue);
                ModbusWrite(macAddress, i.ToString(), initValue);
            }
            //detected plc server enum value = init Value
            PlcEnumValueMatch();
        }

        void PlcEnumValueMatch()
        {
            var length = enumValueMaxCount - enumValueMinCount + 1;
            ModbusRead(macAddress, enumValueMinCount.ToString(), length.ToString(),WriteEnumDataFinsh);
            
        }

        void WriteEnumDataFinsh(string _emumValue)
        {
            Debug.Log("call back info : " +  _emumValue);
        }

        #endregion

        #region Modbus_Read
        /// <summary>
        /// modbus tcp read data form server
        /// </summary>
        /// <param name="_macAddress"></param>
        /// <param name="_firstAddress">startAddress</param>
        /// <param name="_length"></param>
        public void ModbusRead(string _macAddress, string _firstAddress, string _length ,Action<string> callback)
        {
            //功能码 3 = read
            ushort ID = 3;
            ushort MacAddress = 0;
            ushort StartAddress = 0;
            ushort Length = 0;
            //将string 转换成 ushort 并且进行数据校验。
            SetReadValueToUshort(_macAddress, _firstAddress, _length, ref MacAddress, ref StartAddress, ref Length);

            byte[] aa = new byte[12];
            MBmaster.ReadHoldingRegister(ID, StartAddress, Length, MacAddress, out aa);

            //将发送数据显示在TEXTBOX中
            string txtsend = "";
            for (int i = 0; i < aa.Length; i++)
            {
                txtsend += string.Format("{0:X2} ", aa[i]);
            }
            callback(txtsend);
            Debug.Log("read cmd type : " + ID +" _firstAddress : " + _firstAddress  + "发送\r\n" + txtsend);
        }

        void SetReadValueToUshort(string _macAddress, string _firstAddress, string length, ref ushort _MacAddress, ref ushort _StartAddress, ref ushort _Length)
        {
            if (!GetMacAddress(_macAddress, ref _MacAddress))
            {
                Debug.Log("设备地址格式错误！");
                return;
            }

            if (!GetStartAddress(_firstAddress, ref _StartAddress))
            {
                Debug.Log("请求的开始位置错误！请重新输入;");
                return;
            }

            if (!CheckLength(length, ref _Length))
            {
                Debug.Log("目标数据长度错误！;");
                return;
            }
        }
        #endregion


        #region write
        public void ModbusWrite(string _macAddress, string _firstAddress,string _value)
        {
            //功能码 6 = write
            ushort ID = 6;
            ushort MacAddress = 0;
            ushort StartAddress = 0;
            byte[] dd = new byte[2];
            
            int intfirstAddress = int.Parse(_firstAddress);
            string strA = intfirstAddress.ToString("x2");
            
            //将string 转换成 ushort 并且进行数据校验。
            SetWriteValueToUshort(_macAddress, strA,_value, ref MacAddress, ref StartAddress, ref dd);

            byte[] aa = new byte[12];
            MBmaster.WriteSingleRegister(ID, StartAddress, dd, MacAddress, out aa);

            //将发送数据显示在TEXTBOX中
            string txtsend = "";
            for (int i = 0; i < aa.Length; i++)
            {
                txtsend += string.Format("{0:X2} ", aa[i]);
            }

            // Debug.Log("write cmd type : " + ID + " StartAddress :" + StartAddress + "发送\r\n" + txtsend);
        }

        void SetWriteValueToUshort(string _macAddress, string _firstAddress, string _value,ref ushort _MacAddress, ref ushort _StartAddress, ref byte[] dd)
        {
            if (!GetMacAddress(_macAddress, ref _MacAddress))
            {
                Debug.Log("设备地址格式错误！");
                return;
            }

            if (!GetStartAddress(_firstAddress, ref _StartAddress))
            {
                Debug.Log("请求的开始位置错误！请重新输入;");
                return;
            }
            
            // string value = Convert.ToString(_StartAddress);
            SetWriteValue(_value, ref dd);
        }
        #endregion

        #region check data 

        bool GetMacAddress(string _macAddress, ref ushort _MacAddress)
        {
            try
            {
                _MacAddress = Convert.ToUInt16(_macAddress);
            }
            catch
            {
                Debug.Log("设备地址 ：请输入数字！");
                return false;
            }
            if (_MacAddress > 255 || _MacAddress < 0)
            {
                Debug.Log("设备地址：请输入0-255！");
                return false;
            }
            return true;
        }

        bool GetStartAddress(string _address, ref ushort _StartAddress)
        {
            //起始地址 判断是否是4位16进制数
            string address = "";
            Boolean address_valid;
            formatchecking(_address, 4, out address, out address_valid);

            if (!address_valid)
            {
                Debug.Log("寄存器地址数值不符合规范，最多输入4位十六进制数");
            }
            _StartAddress = Convert.ToUInt16(address, 16);
            return address_valid;
        }

        bool CheckLength(string length, ref ushort _Length)
        {
            Boolean num_valid = true;
            try
            {
                //验证是否是有效的数字
                ushort num_01 = Convert.ToUInt16(length);
                Boolean bo_num = (Convert.ToUInt16(length) > 99);
            }
            catch
            {
                num_valid = false;
            }

            if (!num_valid)
            {
                Debug.Log("输入数量不符合规范，请重新输入十进制数字");
            }

            //数量过多，大于read_num = 99;
            if ((Convert.ToUInt16(length) > 99) | (Convert.ToInt16(length) == 0))
            {
                num_valid = false;
            }

            if (!num_valid)
            {
                Debug.Log("输入数量过大或为0，请重新输入十进制数字");
            }
            _Length = Convert.ToUInt16(length);
            return num_valid;
        }

        void SetWriteValue( string _value, ref byte[] dd)
        {
            int intValue = int.Parse(_value);
            string strA = intValue.ToString("x2");
            string value1 = "";
            Boolean valuevalid1;
            // _value = Convert.ToUInt16(_value).ToString();
            formatchecking(strA, 4, out value1, out valuevalid1);
            if (!valuevalid1)
            {
                Debug.Log("输入数值不符合规范，最多输入4位十六进制数");
                return;
            }
            int j = 0;
            for (int i = 0; i < 4; i = i + 2, j++)
            {
                dd[j] = Convert.ToByte(value1.Substring(i, 2), 16);
                // Debug.Log(dd[j].ToString());
                // Debug.Log("\r\n");
            }
            // Debug.Log("11111111" + _value + " 十六进制 : " + strA);
        }

        /// <summary>
        /// checking cmd data formatch 
        /// </summary>
        /// <param name="strinput"></param>
        /// <param name="length"></param>
        /// <param name="stroutput"></param>
        /// <param name="Valid"></param>
        public static void formatchecking(string strinput, int length, out string stroutput, out Boolean Valid)
        {
            stroutput = "";
            Valid = true;
            byte temp;
            if ((strinput.Length <= length) & (strinput.Length > 0))
            {
                for (int i = 0; i < strinput.Length; i++)
                {
                    try
                    {
                        temp = Convert.ToByte(strinput[i].ToString(), 16);
                    }
                    catch
                    {
                        Valid = false;
                        stroutput = "";
                        break;
                    }
                    stroutput += strinput[i];
                }

                if (Valid & (strinput.Length < length))
                {
                    for (int j = 0; j < length - strinput.Length; j++)
                    {
                        stroutput = "0" + stroutput;
                    }
                }
            }
            else
            {
                Valid = false;
                stroutput = "";
            }
        }
        #endregion
    }
}
