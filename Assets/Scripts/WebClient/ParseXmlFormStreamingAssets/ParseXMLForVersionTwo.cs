
using System.Collections.Generic;
using System.Xml;
using UnityEngine;
using Plc.ModbusTcp;
namespace Plc.Data
{
    #region parse xml 
    public class ItemVersionTwo
    {
        /// <summary>
        /// 
        /// </summary>
        public string IpName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Port { get; set; }
        /// <summary>
        /// </summary>
        public string DispName { get; set; }

        /// <summary>
        /// </summary>
        public string WebPass { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string VarCount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string OptLevel { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string MinCycle { get; set; }

        /// <summary>
        /// ForServer Request 
        /// </summary>
        public string SID { get; set; }
    }

    public class XML_OBJ_STORE_VersionTwo
    {
        /// <summary>
        /// 
        /// </summary>
        public int Count { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<ItemVersionTwo> items = new List<ItemVersionTwo>();

        public void DebugSelf()
        {
            for (int i = 0; i < items.Count; i++)
            {
                items[i].SID = "";
                var debugStr = "Count : " + Count;
                debugStr += ("  item : " + i);
                debugStr += ("  IpName : " + items[i].IpName);
                debugStr += ("  Port : " + items[i].Port);
                debugStr += ("  DispName : " + items[i].DispName);
                debugStr += ("  WebPass : " + items[i].WebPass);
                debugStr += ("  VarCount : " + items[i].VarCount);
                debugStr += ("  OptLevel : " + items[i].OptLevel);
                debugStr += ("  MinCycle : " + items[i].MinCycle);
                debugStr += ("  SID : " + items[i].SID);
                Debug.Log(debugStr);
            }
        }
    }

    #endregion
    public class ParseXMLForVersionTwo : MonoBehaviour
    {
        string xmlPath = Application.streamingAssetsPath + "/GrmLanWeb_Version2.Dat";
        public XML_OBJ_STORE_VersionTwo xml_OBJ_STORE_VersionTwo = new XML_OBJ_STORE_VersionTwo();
        void Start()
        {
            LoadXml(xmlPath, ref xml_OBJ_STORE_VersionTwo);
            ParseGrmLanWebDataFinsh();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="_path">XmlPath</param>
        /// <param name="_xml_OBJ_STORE_VersionTwo">xmlStoreData</param>
        public void LoadXml(string _path, ref XML_OBJ_STORE_VersionTwo _xml_OBJ_STORE_VersionTwo)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(_path);
            XmlAttributeCollection xc = doc.SelectSingleNode("XML_OBJ_STORE").Attributes;
            xml_OBJ_STORE_VersionTwo.Count = int.Parse(xc[0].Value);
            for (int i = 0; i < _xml_OBJ_STORE_VersionTwo.Count; i++)
            {
                foreach (XmlElement data in doc.SelectNodes("XML_OBJ_STORE/Item" + i))
                {
                    ItemVersionTwo item = new ItemVersionTwo();
                    item.IpName = data.GetAttribute("IpName");
                    item.Port = data.GetAttribute("Port");
                    item.DispName = data.GetAttribute("DispName");
                    item.WebPass = data.GetAttribute("WebPass");
                    item.VarCount = data.GetAttribute("VarCount");
                    item.OptLevel = data.GetAttribute("OptLevel");
                    item.MinCycle = data.GetAttribute("MinCycle");
                    _xml_OBJ_STORE_VersionTwo.items.Add(item);
                }
            }
        }

        void ParseGrmLanWebDataFinsh()
        {
            ModbusTcpClientsManager.Instance.ParseXMLCallBack(xml_OBJ_STORE_VersionTwo);
        }
    }
}