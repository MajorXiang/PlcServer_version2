using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Plc.Http
{
    public class TeamDataItem
    {
        /// <summary>
        /// 
        /// </summary>
        public string teamName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string GrmNumber { get; set; }
    }

    public class TeamDataJson
    {
        /// <summary>
        /// 
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public List<TeamDataItem> teamDatas = new List<TeamDataItem>();

        public void DebugSelf()
        {
            for (int i = 0; i < teamDatas.Count; i++)
            {
                // items[i].SID = "";
                var debugStr = "Count : " + Count;
                debugStr += ("  item : " + i);
                debugStr += ("  teamName : " + teamDatas[i].teamName);
                debugStr += ("  grmIp : " + teamDatas[i].GrmNumber);
                Debug.Log(debugStr);
            }
        }
    }
}