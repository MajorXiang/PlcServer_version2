
using System.Collections.Generic;
using System.Xml;
using UnityEngine;
using Plc.WebServerRequest;
namespace Plc.Data
{
    #region parse xml 
    public class Item
    {
        /// <summary>
        /// 
        /// </summary>
        public string IpName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string GRM { get; set; }
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
        public string SID{ get; set; }
    }
    
    public class XML_OBJ_STORE
    {
        /// <summary>
        /// 
        /// </summary>
        public int Count { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<Item> items = new List<Item>(); 

        public void DebugSelf()
        {
            for (int i = 0; i < items.Count; i++)
            {
                items[i].SID = "";
                var debugStr = "Count : " + Count;
                debugStr +=("  item : " + i);
                debugStr +=("  IpName : " + items[i].IpName);
                debugStr +=("  GRM : " + items[i].GRM);
                debugStr +=("  DispName : " + items[i].DispName);
                debugStr +=("  WebPass : " + items[i].WebPass);
                debugStr +=("  VarCount : " + items[i].VarCount);
                debugStr +=("  OptLevel : " + items[i].OptLevel);
                debugStr +=("  MinCycle : " + items[i].MinCycle);
                debugStr += ("  SID : " + items[i].SID);
                Debug.Log(debugStr);
            }
        }
    }

    #endregion
    public class ParseXML : MonoBehaviour
    {
        string xmlPath = Application.streamingAssetsPath + "/GrmLanWeb.Dat";
        public XML_OBJ_STORE xml_OBJ_STORE = new XML_OBJ_STORE();
        void Start()
        {
            LoadXml(xmlPath, ref xml_OBJ_STORE);
            ParseGrmLanWebDataFinsh();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="_path">XmlPath</param>
        /// <param name="_xml_OBJ_STORE">xmlStoreData</param>
        public void LoadXml(string _path, ref XML_OBJ_STORE _xml_OBJ_STORE)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(_path);
            XmlAttributeCollection xc = doc.SelectSingleNode("XML_OBJ_STORE").Attributes;
            xml_OBJ_STORE.Count = int.Parse(xc[0].Value);
            for (int i = 0; i < _xml_OBJ_STORE.Count; i++)
            {
                foreach (XmlElement data in doc.SelectNodes("XML_OBJ_STORE/Item" + i))
                {
                    Item item = new Item();
                    item.IpName = data.GetAttribute("IpName");
                    item.GRM = data.GetAttribute("GRM");
                    item.DispName = data.GetAttribute("DispName");
                    item.WebPass = data.GetAttribute("WebPass");
                    item.VarCount = data.GetAttribute("VarCount");
                    item.OptLevel = data.GetAttribute("OptLevel");
                    item.MinCycle = data.GetAttribute("MinCycle");
                    _xml_OBJ_STORE.items.Add(item);
                }
            }
        }

        void ParseGrmLanWebDataFinsh()
        {
            WebClientManage.Instance.ParseXMLCallBack(xml_OBJ_STORE);
        }
    }
}