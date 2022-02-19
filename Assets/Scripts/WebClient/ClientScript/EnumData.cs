using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Plc.Data;
using Plc.Rpc;
namespace Plc.WebServerRequest
{
    public class EnumData
    {
        public int sceneNumber;
        public string enumName;
        public string permissions;
        public string value;
        public ESceneNameType eSceneNameType;

        public string DebugSelf()
        {
            var _str = "   sceneNumber: " + sceneNumber;
            _str += "   enumName: " + enumName;
            _str += "   permissions: " + permissions; // "ReadOnly" :  "ReadAndWrite"
            _str += "   value: " + value;
            return _str;
        }
    }
}
