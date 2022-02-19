using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace Plc.Rpc
{
    public enum ESceneNameType
    {
        None,
        FirePower,//火力发电场景
        WindPower,//风力发电场景
        IntelligentManufacturing,//智慧制造场景
        SolarPower,//太阳能发电场景
        WarehouseLogistics,//仓储物流
        WaterPower,//水力发电厂
        AutomobileMaking,//汽车制造
        CoalToMethanol,//煤制甲醇
        AviationOil//航油
    }

    [System.Obsolete]
    public class AddPortMsg : MessageBase
    {
        public string address;
        public int id;
        public ESceneNameType eplatformType;
        public void SetPortDataType(string address, int id, ESceneNameType eplatformType)
        {
            this.address = address;
            this.id = id;
            this.eplatformType = eplatformType;
        }

        public void DebugSelf(string _type)
        {
            Debug.Log(_type + " client port : " + eplatformType.ToString() + " address : " + address + " ID :" + id);
        }
    }
}