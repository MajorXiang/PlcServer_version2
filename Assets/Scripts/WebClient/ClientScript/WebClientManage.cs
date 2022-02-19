using Plc.Data;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Plc.Rpc;
using UnityEngine;
namespace Plc.WebServerRequest
{
    public class WebClientManage : MonoBehaviour, IEnumerable
    {
        [HideInInspector]
        public static WebClientManage Instance;
        public List<WebClient> webClientList = new List<WebClient>();
        Thread[] threadLogins;
        //Thread thread01;
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
        }

        /// <summary>
        /// parse webserver GrmLanWeb.dat info
        /// </summary>
        /// <param name="_xml_OBJ_STORE"></param>
        public void ParseXMLCallBack(XML_OBJ_STORE _xml_OBJ_STORE)
        {
            _xml_OBJ_STORE.DebugSelf();
            WebClientsInit(_xml_OBJ_STORE);
        }

        public void WebClientsInit(XML_OBJ_STORE _xml_OBJ_STORE)
        {
            if (_xml_OBJ_STORE.items.Count < 30)
            {
                threadLogins = new Thread[_xml_OBJ_STORE.items.Count];
            }
            else
            {
                Debug.LogError("Thread error : exceed max thread !");
            }
            for (int i = 0; i < _xml_OBJ_STORE.items.Count; i++)
            {
                GameObject clientObj = new GameObject();
                clientObj.transform.parent = this.transform;
                clientObj.name = "WebClient_" + i + " : " + _xml_OBJ_STORE.items[i].DispName + " IP : " + _xml_OBJ_STORE.items[i].IpName;
                var webClient = clientObj.AddComponent<WebClient>();
                webClient.Item = _xml_OBJ_STORE.items[i];

                webClient.SetThreadType(i);
                webClient.GrmNumber = _xml_OBJ_STORE.items[i].GRM;
                //webClient.ClientLogin(_xml_OBJ_STORE.items[i]);
                // webClient.DispName = _xml_OBJ_STORE.items[i].DispName;
                webClient.eSceneNameType =MatchSceneTypeByName(_xml_OBJ_STORE.items[i].DispName);
                // Debug.Log(webClient.threadType + " : " + webClient.eSceneNameType);
                webClientList.Add(webClient);
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
            threadLogins[count] = new Thread(new ParameterizedThreadStart(webClientList[count].ClientLogin));

            threadLogins[count].Start(count);

        }

        public void WriteWebServerEnumValue(string _enumStr)
        {
            foreach (var item in webClientList)
            {
                item.ClientWriteDataToWebServer(_enumStr);
            }
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

        private void OnDestroy()
        {
            StopThreads();
        }

        public IEnumerator GetEnumerator()
        {
            throw new System.NotImplementedException();
        }
    }
}