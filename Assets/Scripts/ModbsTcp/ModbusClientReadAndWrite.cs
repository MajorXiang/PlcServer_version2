using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using Plc.Rpc;
using System.Threading;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Text;
using Plc.WebServerRequest;

namespace Plc.ModbusTcp
{
    public partial class ModbusTcpClient
    {
        
        public List<EnumData> enumListCache;
        public List<EnumData> writeEnumList = new List<EnumData>();
        [HideInInspector]
        public bool InitWebServerEnumValueState = false;
        [HideInInspector]
        public bool SetEnumList = false;
        public string TeamName = "Test";
        private int enumValueInitCount = 0;
        
        private string stringCache = "";
        private List<ValueItem> valueItems ;
        private List<EnumData> enumList;
        [HideInInspector]
        public bool isFirstToReadData = true;
        private int readDataFrequency = 1;
        private int reSendWriteValueFrequency = 2;
        public ESceneNameType eSceneNameType = ESceneNameType.None;//作为场景类型数据从.dat文件外部读取
        // private int enumValueMaxCount = 72;//plc end mac address  40072
        // private int enumValueMinCount = 0;//plc start mac address  40001
        private string initValue = "1";
        private string macAddress = "1";
        private int wirteValueSleepTime = 15;
        private bool MatchGrmToSceneType = false;//is match the scene type to send rpc msg data 
        private void Start()
        {
            DataInit();
        }

        void DataInit()
        {
            int[] index = new int[9];
            var instance = ModbusTcpClientsManager.Instance.parsePlcEnumConfigJson;
            valueItems = new List<ValueItem>();
            ListAddAll<ValueItem>(ref valueItems, instance.FirePower);
            index[0] = valueItems.Count;
            ListAddAll<ValueItem>(ref valueItems, instance.WindPower);
            index[1] = valueItems.Count;
            ListAddAll<ValueItem>(ref valueItems, instance.IntelligentManufacturing);
            index[2] = valueItems.Count ;
            ListAddAll<ValueItem>(ref valueItems, instance.SolarPower);
            index[3] = valueItems.Count;
            ListAddAll<ValueItem>(ref valueItems, instance.WarehouseLogistics);
            index[4] = valueItems.Count;
            ListAddAll<ValueItem>(ref valueItems, instance.WaterPower);
            index[5] = valueItems.Count;
            ListAddAll<ValueItem>(ref valueItems, instance.AutomobileMaking);
            index[6]= valueItems.Count;
            ListAddAll<ValueItem>(ref valueItems, instance.CoalToMethanol);
            index[7] = valueItems.Count;
            ListAddAll<ValueItem>(ref valueItems, instance.AviationOil);
            index[8] = valueItems.Count;
            enumList = ParseEnumConfigList(valueItems,index);
            EnumListInit();
        }

        void EnumListInit()
        {
            for (int i = 0; i < enumList.Count; i++)
            {
                enumList[i].index = CalculateTools.GetMacAddress(valueItems[i].ModbusAddress);
            }
        }

        void ListAddAll<T>(ref List<T> _sourceList, List<T> _addList)
        {
            for (int i = 0; i < _addList.Count; i++)
            {
                _sourceList.Add(_addList[i]);
            }
        }

        void ClientConnectedEvent()
        {
            PlcServerDataInit(valueItems);
        }
        
        #region  Client data Init
        void PlcServerDataInit(List<ValueItem> _valueItems)
        {
            int firstAddress = 0;
            for (int i = 0; i < _valueItems.Count; i++)
            {
                Thread.Sleep(wirteValueSleepTime);
                firstAddress = CalculateTools.GetMacAddress(_valueItems[i].ModbusAddress) -1;
                // Debug.Log(eSceneNameType + " : " +  firstAddress);
                ModbusWrite(macAddress, firstAddress.ToString(), initValue);
            }
        }

        /// <summary>
        /// 检测初始化写入值是否完全写完
        /// </summary>
        /// <param name="_index"></param>
        /// <param name="_writeValueStr"></param>
        void DetectedWriteValueInitState(string _index,string _writeValueStr)
        {
            enumValueInitCount++;
            // Debug.Log(  "CMD: writeInit : index value : " + _index +"  write value : " + _writeValueStr );
            if (enumValueInitCount >= valueItems.Count)
            {
                SetEnumList = true;
                Debug.Log("success to write enumvalue :" + IP + " count : " +enumValueInitCount );
                while (true)
                {
                    ModbusRead("1", "0", enumList.Count.ToString());
                    Thread.Sleep(1200 * readDataFrequency);
                }
            }
        }

        #endregion

        #region  Auto Refresh

        void AutoReadEnumValueFinshEvent(string _str)
        {
            _str = GetReadStringToEndStr(_str, "03");
            if (SetEnumList & stringCache == "")
            {
                stringCache = _str;
            }
            else
            {
                EqualsReadDataRef(_str);
            }
        }

        public string GetReadStringToEndStr(string sourse, string startStr)
        {
            string result = string.Empty;
            int _index = 0;
            int topLength = 5;
            if ((sourse.Length - topLength) > 0)
            {
                _index = sourse.LastIndexOf(startStr) + topLength;
                result = sourse.Substring(_index,sourse.Length - _index);
            }
            return result;
        }
        
        void EqualsReadDataRef(string _newStr)
        {
            if (_newStr == stringCache)
            {
                Debug.Log("no data to refresh!");
            }
            else
            {
                RefreshReadData(_newStr, stringCache);
                stringCache = _newStr;
            }
        }

        /// <summary>
        /// Detected data refresh position
        /// </summary>
        /// <param name="_newStr"></param>
        /// <param name="_cacheStr"></param>
        void RefreshReadData(string _newStr,string _cacheStr)
        {
            Debug.Log("false has new data to ref : " + _newStr.Length);
            Debug.Log("false has data chache  is : " + _cacheStr.Length);
            string splitString = " ";
            List<string> _strArryNew = _newStr.Split(new string[] { splitString }, StringSplitOptions.None).ToList();
            _strArryNew = _strArryNew.Where(s => !string.IsNullOrEmpty(s)).ToList();

            List<string> _strlistCache = _cacheStr.Split(new string[] { splitString }, StringSplitOptions.None).ToList();
            _strlistCache = _strlistCache.Where(s => !string.IsNullOrEmpty(s)).ToList();

            for (int i = 0; i < _strArryNew.Count; i++)
            {
                if (!_strlistCache[i].Equals(_strArryNew[i]))
                {
                    int index = (i + 1) / 2;
                    Debug.Log("change pos : " + index +  "  value : " + _strArryNew[i]);
                    GetSceneType(index, _strlistCache[i],_strArryNew[i]);
                    //line 0 = OK; line 1 = count;
                    // enumListCache[i - 2].value = _strArryNew[i];
                    // Debug.Log("Thread Type: " + threadType.ToString() + " SceneType : " + enumListCache[i - 2].eSceneNameType.ToString() + "  has data change :" + enumListCache[i - 2].enumName + " From : " + _strlistCache[i] + " to :" + _strArryNew[i]);
                    // MatchGrmToScene(enumListCache[i - 2].eSceneNameType, enumListCache[i - 2].enumName, _strArryNew[i],threadType.ToString(),GrmNumber);
                }
            }
        }

        void GetSceneType(int _index, string _cachevalue,string _newStr)
        {
            for (int i = 0; i < enumList.Count; i++)
            {
                if (_index == enumList[i].index)
                {
                    // MatchGrmToScene(enumList[i].eSceneNameType, enumList[i].enumName, _newStr, threadType.ToString(), "");
                     Debug.Log("Thread Type: " + threadType+ " SceneType : " + enumList[i].eSceneNameType + "  has data change :" + enumList[i].enumName + " From : " + 
                     _cachevalue + " to :" + _newStr);
                }
            }
        }
        
        /// <summary>
        /// 是否根据grm 所属的场景进行数据分类分发
        /// </summary>
        /// <param name="_eSceneNameType"></param>
        /// <param name="_enumName"></param>
        /// <param name="_enumValue"></param>
        /// <param name="_threadType"></param>
        /// <param name="_grmNumber"></param>
        void MatchGrmToScene(ESceneNameType _eSceneNameType, string _enumName, string _enumValue,string _threadType,string _grmNumber)
        {
            if (!MatchGrmToSceneType)
            {
                RpcServer.Instance.serverSendMsg.SendEnumValueChangeMsg(_eSceneNameType, _enumName, _enumValue, TeamName,_grmNumber);
                Debug.LogError(" 不做场景匹配检测的消息转发");
                return;
            }
            else
            {
                // Debug.LogError(" 消息场景配对： 结果为：" + _eSceneNameType.ToString().Equals(eSceneNameType.ToString()));
                if (_eSceneNameType.ToString().Equals(eSceneNameType.ToString()))
                {
                    RpcServer.Instance.serverSendMsg.SendEnumValueChangeMsg(_eSceneNameType, _enumName, _enumValue, TeamName,_grmNumber);
                }
                else
                {
                    Debug.Log("No Match enumValue change Event :  SceneNameType : " + _eSceneNameType + "  enum Name : " + _enumName + "  enumValue : " + _enumValue);
                }
            }
        }
        #endregion
        
        #region Modbus_Read

        /// <summary>
        /// modbus tcp read data form server
        /// </summary>
        /// <param name="_macAddress"></param>
        /// <param name="_firstAddress">startAddress</param>
        /// <param name="_length"></param>
        public void ModbusRead(string _macAddress, string _firstAddress, string _length)
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
            // callback(txtsend);
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
