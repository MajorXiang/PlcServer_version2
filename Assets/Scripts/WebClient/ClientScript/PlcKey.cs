using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Plc.WebServerRequest
{

    public class PlcKey
    {
        private const string URL_LOGIN = "/exlog";
        private const string URL_EXDATA = "/exdata?SID=";
        private const string URL_WRITE = "&OP=W";
        private const string URL_READ = "&OP=R";
        private const string URL_GETENUM = "&OP=E";
        private const string URL_GETSTATUS = "&OP=I";
        public const string sKey = "123456";

        public static string GetLoginMethodType()
        {
            return URL_LOGIN;
        }

        public static string GetEnumMethodType(string _sid)
        {
            var _getEnumType = URL_EXDATA + _sid + URL_GETENUM;
            return _getEnumType;
        }

        public static string GetWriteMethodType(string _sid)
        {
            var _getWriteType = URL_EXDATA + _sid + URL_WRITE;
            return _getWriteType;
        }

        public static string GetReadMethodType(string _sid)
        {
            var _getReadType = URL_EXDATA + _sid + URL_READ;
            return _getReadType;
        }

        public static string GetStatusMethodType(string _sid)
        {
            var _getStatusType = URL_EXDATA + _sid + URL_GETSTATUS;
            return _getStatusType;
        }

    }

}