 using Plc.Rpc;
namespace Plc.ModbusTcp
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