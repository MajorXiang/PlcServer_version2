using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Plc.Rpc;
using System;
using System.IO;
using Excel;
using System.Data;
using System.Linq;
using System.Text;
using LitJson;

namespace HotFixDll.ComingFish.ResourcesTool
{
    public class AttackPointItem
    {
        public string sceneName { get; set; }
        public string enumName { get; set; }
        public string enumStr { get; set; }
    }
    public class Items
    {
        public List<AttackPointItem> Attack { get; set; }
    }
    public static class SceneNameGet
    {
        public const string firePower = "FirePower";
        public const string windPower = "WindPower";
        public const string intelligentManufacturing = "IntelligentManufacturing";
        public const string solarPower = "SolarPower";
        public const string warehouseLogistics = "WarehouseLogistics";
        public const string waterPower = "WaterPower";
        public const string automobileMaking = "AutomobileMaking";
        public const string coalToMethanol = "CoalToMethanol";
        public const string aviationOil = "AviationOil";
    }
    public class LogRecord : MonoBehaviour
    {
        List<AttackPointItem> items = new List<AttackPointItem>();

        List<AttackPointItem> firePower = new List<AttackPointItem>();
        List<AttackPointItem> windPower = new List<AttackPointItem>();
        List<AttackPointItem> intelligentManufacturing = new List<AttackPointItem>();
        List<AttackPointItem> solarPower = new List<AttackPointItem>();
        List<AttackPointItem> warehouseLogistics = new List<AttackPointItem>();
        List<AttackPointItem> waterPower = new List<AttackPointItem>();
        List<AttackPointItem> automobileMaking = new List<AttackPointItem>();
        List<AttackPointItem> coalToMethanol = new List<AttackPointItem>();
        List<AttackPointItem> aviationOil = new List<AttackPointItem>();

        string path = Application.streamingAssetsPath + "/AttackPoint.xlsx";
        string savePath = "";
        string _savePath = "";
        string attackEvent = "";
        string attackStr = "";
        string data = "";
        float time;
        int txtIndex = 0;
        // Start is called before the first frame update
        void Start()
        {
            savePath = Environment.CurrentDirectory;
            //+@"\SaveLog\";
            List<string> _strArryNew = savePath.Split(new string[] { @"\" }, StringSplitOptions.None).ToList();
            foreach (var item in _strArryNew)
            {
                _savePath =_savePath+item+"/";
            }
            _savePath += "SaveLog/";
            //LoadInfo(path);
            ReadData();
        }
        // Update is called once per frame
        void Update()
        {
            time += Time.deltaTime;
            if (time > 5f)
            {
                AutoSave(txtIndex);
                txtIndex++;
                time = 0;
            }
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                PLCMsg _plcMsg = new PLCMsg();
                _plcMsg.SetEnumValueChangeMsg(ESceneNameType.FirePower, "S1N主汽开关R", "0", "超能特战队");
                SaveAttackEvent(_plcMsg);
            }
            #region 这是一个将表格数据转json数据的方法
            //if (Input.GetKeyDown(KeyCode.S))
            //{
            //    if (!File.Exists(Application.persistentDataPath + "/savaAttackPoint.json"))
            //    {
            //        File.Create(Application.persistentDataPath + "/savaAttackPoint.json");
            //    }
            //    //string datas = JsonUtility.ToJson(items, true);
            //    Items it = new Items();
            //    it.Attack = items;
            //    string datas = JsonMapper.ToJson(it);
            //    File.WriteAllText(Application.persistentDataPath + "/savaAttackPoint.json", datas);
            //    Debug.Log(datas);
            //}
            #endregion
        }
        void AutoSave(int num)
        {
            //string newSavePath = _savePath + DateTime.Now.Year + "Year" + DateTime.Now.Month + "Month" + DateTime.Now.Day + "Day" + DateTime.Now.Hour + ":" + DateTime.Now.Minute + ".txt";
            
            string newSavePath = _savePath + num + ".txt";
            Debug.Log(newSavePath);
            try 
            {
                //找到当前路径
                FileInfo file = new FileInfo(newSavePath);
                //判断有没有文件，有则打开文件，，没有创建后打开文件
                StreamWriter sw = file.CreateText();
                //ToJson接口将你的列表类传进去，，并自动转换为string类型
                //string json = JsonMapper.ToJson(personList.dictionary);
                //将转换好的字符串存进文件，
                sw.WriteLine(attackStr);
                sw.Close();
                sw.Dispose();
            }
            catch(Exception ex)
            {
                Debug.Log(ex);
            }
        }
        void ReadData()
        {
            data= File.ReadAllText(Application.streamingAssetsPath+ "/savaAttackPoint.json");
            Items it = new Items();
            it = JsonMapper.ToObject<Items>(data);
            foreach (var point in it.Attack)
            {
                //Debug.LogError(point.sceneName + "/" + point.enumName + "/" + point.enumStr);
                switch (point.sceneName)
                {
                    case SceneNameGet.firePower:
                        firePower.Add(point);
                        break;
                    case SceneNameGet.windPower:
                        windPower.Add(point);
                        break;
                    case SceneNameGet.solarPower:
                        solarPower.Add(point);
                        break;
                    case SceneNameGet.intelligentManufacturing:
                        intelligentManufacturing.Add(point);
                        break;
                    case SceneNameGet.warehouseLogistics:
                        warehouseLogistics.Add(point);
                        break;
                    case SceneNameGet.waterPower:
                        windPower.Add(point);
                        break;
                    case SceneNameGet.automobileMaking:
                        automobileMaking.Add(point);
                        break;
                    case SceneNameGet.coalToMethanol:
                        coalToMethanol.Add(point);
                        break;
                    case SceneNameGet.aviationOil:
                        aviationOil.Add(point);
                        break;
                }
            }
        }
        /// <summary>
        /// 读取表格的路径和读取表格
        /// </summary>
        /// <param name="_path"></param>
        /// <returns></returns>
        DataSet ReadExcel(string _path)
        {
            FileStream fs = File.Open(_path, FileMode.Open, FileAccess.Read, FileShare.Read);
            IExcelDataReader iExcelDR = ExcelReaderFactory.CreateOpenXmlReader(fs);
            DataSet ds = iExcelDR.AsDataSet();
            return ds;
        }
        /// <summary>
        /// 读取表格中的单元格数据
        /// </summary>
        /// <param name="_path"></param>
        void LoadInfo(string _path)
        {
            DataSet ds = ReadExcel(_path);
            int columns = ds.Tables[0].Columns.Count;
            int rows = ds.Tables[0].Rows.Count;
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    AttackPointItem item = new AttackPointItem()
                    {
                        sceneName = ds.Tables[0].Rows[i][j].ToString(),
                        enumName = ds.Tables[0].Rows[i][j].ToString(),
                        enumStr = ds.Tables[0].Rows[i][j].ToString()
                    };
                    items.Add(item);
                }            }
            Items it = new Items();
            it.Attack = items;
            data = JsonMapper.ToJson(it);
            ReadData();
        }


        /// <summary>
        /// 攻击记录转成相应的攻击信息
        /// </summary>
        /// <param name="_sceneNameType"></param>
        /// <param name="_enumName"></param>
        /// <param name="_teamName"></param>
        /// <returns></returns>
        public string GetAttackEvent(ESceneNameType _sceneNameType,string _enumName,string _teamName)
        {
            string str = "";
            switch (_sceneNameType.ToString())
            {
                case SceneNameGet.firePower:
                    for (int i = 0; i < firePower.Count; i++)
                    {
                        if (firePower[i].enumName == _enumName)
                        {
                            str = firePower[i].enumStr;
                        }
                    }
                    break;
                case SceneNameGet.windPower:
                    for (int i = 0; i < windPower.Count; i++)
                    {
                        if (windPower[i].enumName == _enumName)
                        {
                            str = windPower[i].enumStr;
                        }
                    }
                    break;
                case SceneNameGet.solarPower:
                    for (int i = 0; i < solarPower.Count; i++)
                    {
                        if (solarPower[i].enumName == _enumName)
                        {
                            str = solarPower[i].enumStr;
                        }
                    }
                    break;
                case SceneNameGet.intelligentManufacturing:
                    for (int i = 0; i < intelligentManufacturing.Count; i++)
                    {
                        if (intelligentManufacturing[i].enumName == _enumName)
                        {
                            str = intelligentManufacturing[i].enumStr;
                        }
                    }
                    break;
                case SceneNameGet.warehouseLogistics:
                    for (int i = 0; i < warehouseLogistics.Count; i++)
                    {
                        if (warehouseLogistics[i].enumName == _enumName)
                        {
                            str = warehouseLogistics[i].enumStr;
                        }
                    }
                    break;
                case SceneNameGet.waterPower:
                    for (int i = 0; i < waterPower.Count; i++)
                    {
                        if (waterPower[i].enumName == _enumName)
                        {
                            str = waterPower[i].enumStr;
                        }
                    }
                    break;
                case SceneNameGet.automobileMaking:
                    for (int i = 0; i < automobileMaking.Count; i++)
                    {
                        if (automobileMaking[i].enumName == _enumName)
                        {
                            str = automobileMaking[i].enumStr;
                        }
                    }
                    break;
                case SceneNameGet.coalToMethanol:
                    for (int i = 0; i < coalToMethanol.Count; i++)
                    {
                        if (coalToMethanol[i].enumName == _enumName)
                        {
                            str = coalToMethanol[i].enumStr;
                        }
                    }
                    break;
                case SceneNameGet.aviationOil:
                    for (int i = 0; i < aviationOil.Count; i++)
                    {
                        if (aviationOil[i].enumName == _enumName)
                        {
                            str = aviationOil[i].enumStr;
                        }
                    }
                    break;
            }
            if(str!="")
            {
                List<string> _strArryNew = str.Split(new string[] { "队伍" }, StringSplitOptions.None).ToList();
                str = _teamName + _strArryNew[1];
            }
            return str;
        }
        /// <summary>
        /// 接收PLC对象，将攻击记录记录下来
        /// </summary>
        /// <param name="_plcMsg"></param>
        public void SaveAttackEvent(PLCMsg _plcMsg)
        {
            attackEvent = GetAttackEvent(_plcMsg.eSceneNameType,_plcMsg.enumName,_plcMsg.threadType);
            Debug.LogError(attackEvent);
            if (attackEvent != null)
            {
                attackStr += DateTime.Now.ToString() + attackEvent + "\n";
                //Debug.Log(attackStr);
            }
            //Debug.Log(_plcMsg.threadType + " eSceneNameType:" + _plcMsg.eSceneNameType.ToString() + " enumName:" + _plcMsg.enumName + " enumValue:" + _plcMsg.enumValue);
        }
    }
}

