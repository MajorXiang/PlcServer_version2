 using Plc.Rpc;
 using UnityEngine;

 namespace Plc.ModbusTcp
{
    public class EnumData
    {
        public int index;
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
            // Debug.Log(_str);
            return _str;
        }
    }
}