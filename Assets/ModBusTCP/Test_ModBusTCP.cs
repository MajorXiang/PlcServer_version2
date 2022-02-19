using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ModbusTCP;
using System;

public class Test_ModBusTCP : MonoBehaviour
{
    private ModbusTCP.Master MBmaster;
    private byte[] data;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.J))
        {
            StartCoroutine(btnOpen_Click());
        }
    }

    private IEnumerator btnOpen_Click()
    {
        string IP = "127.0.0.1";
        string Port = "502";

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
        btnread_Click();
        btnsend_Click();
        yield return 0;
    }

    private void btnClose_Click()
    {
        MBmaster.Dispose();
        MBmaster = null;
    }

    private void btnsend_Click()
    {
        string str_Debug = "";
        //设备号
        string _mac = "1";
        ushort mac_addr;
        try
        {
            mac_addr = Convert.ToUInt16(_mac);
        }
        catch
        {
            Debug.Log("设备地址 ：请输入数字！");
            return;
        }

        if (mac_addr > 255 || mac_addr < 0)
        {
            Debug.Log("设备地址：请输入0-255！");
            return;
        }

        //功能码
        ushort ID = 6;

        //起始地址 判断是否是4位16进制数
        string _address = "0";
        string address = "";
        Boolean address_valid;
        formatchecking(_address, 4, out address, out address_valid);

        if (!address_valid)
        {
            Debug.Log("寄存器地址数值不符合规范，最多输入4位十六进制数");
            return;
        }

        ushort StartAddress = Convert.ToUInt16(address, 16);

        //输入数值

        string _value1 = "4";
        string value1 = "";
        Boolean valuevalid1;

        formatchecking(_value1, 4, out value1, out valuevalid1);

        if (!valuevalid1)
        {
            Debug.Log("输入数值不符合规范，最多输入4位十六进制数");
            return;
        }

        byte[] dd = new byte[2];
        int j = 0;

        for (int i = 0; i < 4; i = i + 2, j++)
        {
            dd[j] = Convert.ToByte(value1.Substring(i, 2), 16);
            Debug.Log(dd[j].ToString());
            Debug.Log("\r\n");
        }

        byte[] aa = new byte[12];
        
        MBmaster.WriteSingleRegister(ID, StartAddress, dd, mac_addr, out aa);

        //将发送数据显示在TEXTBOX中
        string _txtsend = "";
        string txtsend = "";

        for (int i = 0; i < aa.Length; i++)
        {
            txtsend += string.Format("{0:X2} ", aa[i]);
        }

        Debug.Log(txtsend);

        Debug.Log(ID.ToString());
        Debug.Log("\r\n");
        Debug.Log(StartAddress.ToString());
        Debug.Log("发送\r\n");
    }
    private void btnread_Click()
    {
        string str = "";
        //设备号
        string _mac = "1";
        ushort mac_addr;
        try
        {
            mac_addr = Convert.ToUInt16(_mac);
        }
        catch
        {
            Debug.Log("设备地址 ：请输入数字！");
            return;
        }

        if (mac_addr > 255 || mac_addr < 0)
        {
            Debug.Log("设备地址：请输入0-255！");
            return;
        }

        //功能码
        ushort ID = 3;

        //起始地址 判断是否是4位16进制数
        string _address = "0";
        string address = "";
        Boolean address_valid;
        formatchecking(_address, 4, out address, out address_valid);

        if (!address_valid)
        {
            Debug.Log("寄存器地址数值不符合规范，最多输入4位十六进制数");
            return;
        }

        ushort StartAddress = Convert.ToUInt16(address, 16);

        //长度
        string length = "7";
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
            return;
        }

        //数量过多，大于read_num = 99;
        if ((Convert.ToUInt16(length) > 99) | (Convert.ToInt16(length) == 0))
            num_valid = false;

        if (!num_valid)
        {
            Debug.Log("输入数量过大或为0，请重新输入十进制数字");
            return;
        }

        ushort Length = Convert.ToUInt16(length);

        byte[] aa = new byte[12];
        MBmaster.ReadHoldingRegister(ID, StartAddress, Length, mac_addr, out aa);

        //将发送数据显示在TEXTBOX中
        string _txtsend  = "";
        string txtsend = "";

        for (int i = 0; i < aa.Length; i++)
        {
            txtsend += string.Format("{0:X2} ", aa[i]);
        }

        Debug.Log(txtsend);

        Debug.Log(ID.ToString());
        Debug.Log("\r\n");
        Debug.Log(StartAddress.ToString());
        Debug.Log("发送\r\n");
    }


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

    /// <summary>
    /// 
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
}
