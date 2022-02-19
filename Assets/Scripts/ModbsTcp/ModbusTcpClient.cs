using System.Collections;
using UnityEngine;
using ModbusTCP;
using System;
using Plc.Data;
namespace Plc.ModbusTcp
{
    public partial class ModbusTcpClient : MonoBehaviour
    {
        [HideInInspector]
        public static ModbusTcpClient Instance;
        [HideInInspector]
        public Master MBmaster;
        private byte[] data;
        public string IP = "";
        public string Port = "";
        [SerializeField]
        public EThreadType threadType = EThreadType.None;
        private ItemVersionTwo item;
        public ItemVersionTwo Item { set => item = value; }

        public void SetThreadType(int _number)
        {
            threadType = SceneAndThreadsData.GetType(_number);
        }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                //ModbusInit();
            }
        }

        public void ModbusConnect(object obj)
        {
            try
            {
                ushort intPort = Convert.ToUInt16(Port);
                // Create new modbus master and add event functions
                MBmaster = new Master(IP, intPort);
                MBmaster.OnResponseData += new ModbusTCP.Master.ResponseData(MBmaster_OnResponseData);
                MBmaster.OnException += new ModbusTCP.Master.ExceptionData(MBmaster_OnException);
            }
            catch (SystemException error)
            {
                Debug.Log(error.ToString());
            }
        }

        private void OnDestroy()
        {
            if (MBmaster != null)
            {
                MBmaster.Dispose();
                MBmaster = null;
            }
        }

        #region debug system info

        // ------------------------------------------------------------------------
        // Event for response data
        // ------------------------------------------------------------------------
        private void MBmaster_OnResponseData(ushort ID, byte function, byte[] values)
        {
            data = values;
            string str = "";

            for (int i = 0; i < data.Length; i++)
            {
                str += string.Format("{0:X2} ", data[i]);
            }
            // ------------------------------------------------------------------------
            // Identify requested data

            switch (ID)
            {
                case 1:
                    Debug.Log(str);
                    Debug.Log("获取开关通讯正常");
                    break;
                case 3:
                    Debug.Log(str);
                    Debug.Log("读取寄存器通讯正常");
                    break;
                case 5:
                    Debug.Log(str);
                    Debug.Log("写入开关通讯正常");
                    break;
                case 6:
                    Debug.Log(str);
                    Debug.Log("写单路寄存器通讯正常");
                    break;
                case 16:
                    Debug.Log(str);
                    Debug.Log("多路写入通讯正常");
                    break;
            }
        }

        // ------------------------------------------------------------------------
        // Modbus TCP slave exception
        // ------------------------------------------------------------------------
        private void MBmaster_OnException(ushort id, byte function, byte exception)
        {
            switch (id)
            {
                case 1:
                    Debug.Log("获取开关通讯异常");
                    break;
                case 3:
                    Debug.Log("读取寄存器通讯异常");
                    break;
                case 5:
                    Debug.Log("写入开关通讯异常");
                    break;
                case 6:
                    Debug.Log("写单路寄存器通讯异常");
                    break;
                case 16:
                    Debug.Log("多路写入通讯异常");
                    break;
            }

            string exc = "Modbus says error: ";
            switch (exception)
            {
                case Master.excIllegalFunction: exc += "Illegal function!"; break;
                case Master.excIllegalDataAdr: exc += "Illegal data adress!"; break;
                case Master.excIllegalDataVal: exc += "Illegal data value!"; break;
                case Master.excSlaveDeviceFailure: exc += "Slave device failure!"; break;
                case Master.excAck: exc += "Acknoledge!"; break;
                case Master.excSlaveIsBusy: exc += "Slave is busy!"; break;
                case Master.excGatePathUnavailable: exc += "Gateway path unavailbale!"; break;
                case Master.excExceptionTimeout: exc += "Slave timed out!"; break;
                case Master.excExceptionConnectionLost: exc += "Connection is lost!"; break;
                case Master.excExceptionNotConnected: exc += "Not connected!"; break;
            }
            Debug.Log(exc + "Modbus slave exception");
        }

        #endregion
    }

}