using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Plc.Rpc;
namespace Plc.Data
{
    /// <summary>
    /// GRM 模块
    /// </summary>
    public enum EThreadType
    {
        None,
        Player0,
        Player1,
        Player2,
        Player3,
        Player4,
        Player5,
        Player6,
        Player7
    }

    public class SceneAndThreadsData
    {
        public static EThreadType GetType(int _number)
        {
            switch (_number)
            {
                case 0:
                    return EThreadType.Player0;
                case 1:
                    return EThreadType.Player1;
                case 2:
                    return EThreadType.Player2;
                case 3:
                    return EThreadType.Player3;
                case 4:
                    return EThreadType.Player4;
                case 5:
                    return EThreadType.Player5;
                case 6:
                    return EThreadType.Player6;
                case 7:
                    return EThreadType.Player7;
                default:
                    return EThreadType.None;
            }
        }
        /// <summary>
        /// Set S1 = FirePower
        /// </summary>
        /// <param name="_number"></param>
        /// <returns></returns>
        public static ESceneNameType GetSceneType(int _number)
        {
            switch (_number)
            {
                case 1:
                    return ESceneNameType.FirePower;
                case 2:
                    return ESceneNameType.WindPower;
                case 3:
                    return ESceneNameType.IntelligentManufacturing;
                case 4:
                    return ESceneNameType.SolarPower;
                case 5:
                    return ESceneNameType.WarehouseLogistics;
                case 6:
                    return ESceneNameType.WaterPower;
                case 7:
                    return ESceneNameType.AutomobileMaking;
                case 8:
                    return ESceneNameType.CoalToMethanol;
                case 9:
                    return ESceneNameType.AviationOil;
                default:
                    Debug.LogError("Exceed SceneType Max Length ");
                    return ESceneNameType.None;
            }
        }

    }
}