using System.Collections.Generic;
using UnityEngine;
using System;
using Plc.Rpc;

namespace Plc.ModbusTcp
{
    public partial class ModbusTcpClient
    {
        public List<EnumData> enumListCache;
        public List<EnumData> writeEnumList = new List<EnumData>();
        private string enumInitValue = "1";
        private string stringCache = "";
        public bool SetEnumList = false;
        public bool isFirstToReadData = true;
        //request read data from webserver frequency  read time
        private int mreadDataFrequency = 1;
        public ESceneNameType eSceneNameType = ESceneNameType.None;//作为场景类型数据从.dat文件外部读取
        public int targetEnumCount = 0;
        public string TeamName = "";
        public bool InitWebServerEnumValueState = false;
        //init web server init enum value resend request frequency
        private int reSendWriteValueFrequency = 2;

        private bool clientWriteValue = false;

        private bool MatchGrmToSceneType = true;


        #region Modbus_Read
        /// <summary>
        /// modbus tcp read data form server
        /// </summary>
        /// <param name="_macAddress"></param>
        /// <param name="_firstAddress">startAddress</param>
        /// <param name="_length"></param>
        private void ModbusRead(string _macAddress, string _firstAddress, string _length)
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

            Debug.Log("cmd type : " + ID + " StartAddress :" + StartAddress + "发送\r\n" + txtsend);
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

            if (CheckLength(length, ref _Length))
            {
                Debug.Log("目标数据长度错误！;");
                return;
            }
        }
        #endregion


        #region write
        private void ModbusWrite(string _macAddress, string _firstAddress)
        {
            //功能码 3 = write
            ushort ID = 6;
            ushort MacAddress = 0;
            ushort StartAddress = 0;
            byte[] dd = new byte[2];
            //将string 转换成 ushort 并且进行数据校验。
            SetWriteValueToUshort(_macAddress, _firstAddress, ref MacAddress, ref StartAddress, ref dd);

            byte[] aa = new byte[12];
            MBmaster.WriteSingleRegister(ID, StartAddress, dd, MacAddress, out aa);

            //将发送数据显示在TEXTBOX中
            string txtsend = "";
            for (int i = 0; i < aa.Length; i++)
            {
                txtsend += string.Format("{0:X2} ", aa[i]);
            }

            Debug.Log("cmd type : " + ID + " StartAddress :" + StartAddress + "发送\r\n" + txtsend);
        }

        void SetWriteValueToUshort(string _macAddress, string _firstAddress, ref ushort _MacAddress, ref ushort _StartAddress, ref byte[] dd)
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
            string value = Convert.ToString(_StartAddress);
            SetWriteValue(value, ref dd);

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
            return false;
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
            int j = 0;
            for (int i = 0; i < 4; i = i + 2, j++)
            {
                dd[j] = Convert.ToByte(_value.Substring(i, 2), 16);
                Debug.Log(dd[j].ToString());
                Debug.Log("\r\n");
            }
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
