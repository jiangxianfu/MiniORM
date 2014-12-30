using System;
using System.Collections.Generic;

namespace miniORM.TestCase
{
    /// <summary>
    /// 数据访问层
    /// </summary>
    public class DataAccessLayer
    {
        private static string DBConn = "server=.\\sqlexpress;database=testdb;uid=sa;pwd=sa;";
        private static DBHelper CreateDBHelper()
        {
            return new MSSQLHelper(DBConn);
        }
        public static List<WebInfo> GetData()
        {
            DBHelper db = CreateDBHelper();
            var reader = db.ExecuteReader("select * from[fo] (nolock)");
            return EntityHelper<WebInfo>.SetValues(reader);
        }
        public static WebInfo GetDataOne()
        {
            DBHelper db = CreateDBHelper();
            var reader = db.ExecuteReader("select top 1 BatchID, Cluster, AliasCluster, ClusterIP, ClusterVPort, MachineName, IPAddress, Port, [Status ], CreateTime, Zone from [dbo].[aaab] (nolock)");
            return EntityHelper<WebInfo>.SetValue(reader);
        }
    }
    /// <summary>
    /// 实体层
    /// </summary>
    public class WebInfo : BaseEntity
    {
        public int BatchID { get; set; }
        public string Cluster { get; set; }
        public string AliasCluster { get; set; }
        public string ClusterIP { get; set; }
        public int ClusterVPort { get; set; }
        public string MachineName { get; set; }
        public string IPAddress { get; set; }
        public int Port { get; set; }
        public int Status { get; set; }
        public DateTime CreateTime { get; set; }
        public int? Zone { get; set; }

        internal override void SetEntity(System.Data.IDataReader reader, Dictionary<string, int> lowerFields)
        {
            BatchID = reader.GetInt32(lowerFields["batchid"]);
            Cluster = GetField<string>(reader, lowerFields["cluster"]);
            AliasCluster = reader.GetString(lowerFields["aliascluster"]);
            ClusterIP = reader.GetString(lowerFields["clusterip"]);
            ClusterVPort = reader.GetInt32(lowerFields["clustervport"]);
            MachineName = reader.GetString(lowerFields["machinename"]);
            IPAddress = reader.GetString(lowerFields["ipaddress"]);
            Port = reader.GetInt32(lowerFields["port"]);
            Status = reader.GetInt32(lowerFields["status"]);
            CreateTime = reader.GetDateTime(lowerFields["createtime"]);
            Zone = GetField<int?>(reader, lowerFields["zone"]);
        }
    }
    /// <summary>
    /// 业务逻辑层
    /// </summary>
    public class BusinessLayer
    {
        public List<WebInfo> WebInfos()
        {
            var data = DataAccessLayer.GetData();
            return data;
        }
        public WebInfo WebInfo()
        {
            var data = DataAccessLayer.GetDataOne();
            return data;
        }
    }
}
