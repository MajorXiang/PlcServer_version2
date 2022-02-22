using LitJson;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Plc.Rpc
{
    public class ParseClientsConfigJson 
    {
        private const string ClientConfigjsonName = "ClientsConfig";
        private const string PlcEnumValueNameConfigjsonName = "PlcEnumValueData"; 

        public  ClientsConfigJson GetClientsConfigJson()
        {
            ClientsConfigJson clientsConfigJson;
            string jsonPath = GetStreamingAssetsJsonPath(ClientConfigjsonName);
            clientsConfigJson = ParseJson<ClientsConfigJson>(jsonPath);
            return clientsConfigJson;
        }
        
        public  ParsePlcEnumConfigJson GetPlcEnumConfigJson()
        {
            ParsePlcEnumConfigJson parsePlcEnumConfigJson;
            string jsonPath = GetStreamingAssetsJsonPath(PlcEnumValueNameConfigjsonName);
            parsePlcEnumConfigJson = ParseJson<ParsePlcEnumConfigJson>(jsonPath);
            return parsePlcEnumConfigJson;
        }

        public string GetStreamingAssetsJsonPath( string _name )
        {
            string _path = Application.dataPath + "/StreamingAssets/" + _name + ".json";
            return _path;
        }

        /// <summary>
        /// parse json data
        /// </summary>
        /// <typeparam name="T">JsonType</typeparam>
        /// <param name="_path">JsonPath</param>
        /// <returns></returns>
        public T ParseJson<T>(string _path)
        {
            StreamReader streamreader = new StreamReader(_path);
            string _jsonStr = streamreader.ReadToEnd();
            T t = JsonMapper.ToObject<T>(_jsonStr);
            return t;
        }

    }
}