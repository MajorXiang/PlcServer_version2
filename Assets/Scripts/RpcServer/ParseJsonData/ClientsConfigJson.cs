using System.Collections.Generic;
using System;

namespace Plc.Rpc
{
    public class DiplayItem
    {
        /// <summary>
        /// client exe play target display id
        /// </summary>
        public int targetDisplayID { get; set; }
        /// <summary>
        /// client exe path 
        /// </summary>
        public string path { get; set; }
        /// <summary>
        /// client exe name
        /// </summary>
        public string exeName { get; set; }

        public string eSceneNameType { get; set; }
        public string eSceneNameString { get; set; }
    }

    public class ClientsConfigJson
    {
        /// <summary>
        /// client max display count
        /// </summary>
        public int maxDisplayCount { get; set; }
        /// <summary>
        /// client play resoultion Width
        /// </summary>
        public int rectW { get; set; }
        /// <summary>
        /// client play resoultion Hight
        /// </summary>
        public int rectH { get; set; }

        /// <summary>
        ///client and display type 横向排列还是纵向排列
        /// </summary>
        public bool horizontal { get; set; }
        /// <summary>
        /// client display
        /// </summary>
        public List<DiplayItem> diplay = new List<DiplayItem>();

    }
}