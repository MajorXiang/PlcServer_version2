using System;
using UnityEngine;

namespace Plc.WebServerRequest
{
    public class CalculateTools : MonoBehaviour
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sourse">字符串数据源</param>
        /// <param name="startstr">开始字符</param>
        /// <param name="endstr">结束字符</param>
        /// <returns></returns>
        public static string MidStrEx(string sourse, string startstr, string endstr)
        {
            string result = string.Empty;
            int startindex, endindex;
            try
            {
                startindex = sourse.IndexOf(startstr);
                if (startindex == -1)
                    return result;
                string tmpstr = sourse.Substring(startindex + startstr.Length);
                endindex = tmpstr.IndexOf(endstr);
                if (endindex == -1)
                    return result;
                result = tmpstr.Remove(endindex);
            }
            catch (Exception ex)
            {
                Debug.Log("MidStrEx Err:" + ex.Message);
            }
            return result;
        }

        /// <summary>
        /// 截取字符串从指定字符到结束的整体内容
        /// </summary>
        /// <param name="sourse">字符串源数据</param>
        /// <param name="startstr">开始字符串</param>
        /// <returns></returns>
        public static string GetStringFromToEnd(string sourse, string startstr)
        {
            string result = string.Empty;
            int _index = sourse.LastIndexOf(startstr);
            result = sourse.Substring(_index + 1);
            return result;
        }
        /// <summary>
        /// 截取字符串从开始到指定字符的整体内容
        /// </summary>
        /// <param name="sourse">字符串源数据</param>
        /// <param name="endstr">节数字符标识</param>
        /// <returns></returns>
        public static string GetStringToEndStr(string sourse, string endstr)
        {
            //最后一个字母R标识Read ，W 标识Write
            string result = string.Empty;
            int _index = sourse.IndexOf(endstr);
            result = sourse.Substring(0,_index);
            return result;
        }
    }
}