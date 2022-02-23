using System;
using Plc.Data;
using Plc.Rpc;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace Plc.ModbusTcp
{

    public class ModbusTcpClientsManager : MonoBehaviour
    {
        [HideInInspector]
        public static ModbusTcpClientsManager Instance;
        public List<ModbusTcpClient> modbusTcpClientlist = new List<ModbusTcpClient>();
        Thread[] threadLogins;
        int threadMaxCount = 30;
        [HideInInspector]
        public ParsePlcEnumConfigJson parsePlcEnumConfigJson;
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            parsePlcEnumConfigJson = new ParseClientsConfigJson().GetPlcEnumConfigJson();
        }
        
        private void Update()
        {

            if (Input.GetKeyDown(KeyCode.D))
            {
                // SendWriteMsg();
                // SendReadMsg();
            }
        }
        
        void SendReadMsg()
        {
            // modbusTcpClientlist[0].ModbusRead("1", "2", "1");
        }

        // private int count = 0;
        /// <summary>
        /// to do
        /// </summary>
        void SendWriteMsg()
        {
            // count++;
            // modbusTcpClientlist[0].ModbusWrite("1", "2", count.ToString());
        }
        /// <summary>
        /// parse webserver GrmLanWeb.dat info
        /// </summary>
        /// <param name="_xml_OBJ_STORE"></param>
        public void ParseXMLCallBack(XML_OBJ_STORE_VersionTwo _xml_OBJ_STORE)
        {
            _xml_OBJ_STORE.DebugSelf();
            // parsePlcEnumConfigJson.DebugSelf();
            WebClientsInit(_xml_OBJ_STORE);//开启线程执行代码
        }

        public void WebClientsInit(XML_OBJ_STORE_VersionTwo _xML_OBJ_STORE_VersionTwo)
        {
            if (_xML_OBJ_STORE_VersionTwo.items.Count < threadMaxCount)
            {
                threadLogins = new Thread[_xML_OBJ_STORE_VersionTwo.items.Count];
            }
            else
            {
                Debug.LogError("Thread error : exceed max thread !");
            }
            for (int i = 0; i < _xML_OBJ_STORE_VersionTwo.items.Count; i++)
            {
                GameObject clientObj = new GameObject();
                clientObj.transform.parent = this.transform;
                clientObj.name = "WebClient_" + i + " : " + _xML_OBJ_STORE_VersionTwo.items[i].DispName + " IP : " + _xML_OBJ_STORE_VersionTwo.items[i].IpName;
                var modbusTcpClient = clientObj.AddComponent<ModbusTcpClient>();
                modbusTcpClient.Item = _xML_OBJ_STORE_VersionTwo.items[i];

                modbusTcpClient.SetThreadType(i);
                modbusTcpClient.Port = _xML_OBJ_STORE_VersionTwo.items[i].Port;
                modbusTcpClient.IP = _xML_OBJ_STORE_VersionTwo.items[i].IpName;

                modbusTcpClient.eSceneNameType = MatchSceneTypeByName(_xML_OBJ_STORE_VersionTwo.items[i].DispName);
                // Debug.Log(webClient.threadType + " : " + webClient.eSceneNameType);
                modbusTcpClientlist.Add(modbusTcpClient);
                ThreadInit(i);
            }
        }

        ESceneNameType MatchSceneTypeByName(string _sceneName)
        {
            switch (_sceneName)
            {
                case "FirePower":
                    return ESceneNameType.FirePower;
                    break;
                case "WindPower":
                    return ESceneNameType.WindPower;
                    break;
                case "IntelligentManufacturing":
                    return ESceneNameType.IntelligentManufacturing;
                    break;
                case "SolarPower":
                    return ESceneNameType.SolarPower;
                    break;
                case "WarehouseLogistics":
                    return ESceneNameType.WarehouseLogistics;
                    break;
                case "WaterPower":
                    return ESceneNameType.WaterPower;
                    break;
                case "AutomobileMaking":
                    return ESceneNameType.AutomobileMaking;
                    break;
                case "CoalToMethanol":
                    return ESceneNameType.CoalToMethanol;
                    break;
                case "AviationOil":
                    return ESceneNameType.AviationOil;
                    break;
                default:
                    return ESceneNameType.None;
            }
        }

        void ThreadInit(int count)
        {
            //threadLogins[count] = new Thread(webClientList[count].ClientLogin);
            threadLogins[count] = new Thread(new ParameterizedThreadStart(modbusTcpClientlist[count].ModbusConnect));

            threadLogins[count].Start(count);

        }

        /// <summary>
        /// Get Rpc client cmd data ,set enumValue to server
        /// </summary>
        /// <param name="_enumStr"></param>
        public void WriteWebServerEnumValue(string _enumStr)
        {
            //foreach (var item in modbusTcpClientlist)
            //{
            //    item.ClientWriteDataToWebServer(_enumStr);
            //}
        }

        private void OnDestroy()
        {
            StopThreads();
        }

        void StopThreads()
        {
            if (threadLogins != null)
            {
                foreach (var item in threadLogins)
                {
                    item.Abort();
                }
            }
        }
    }

}