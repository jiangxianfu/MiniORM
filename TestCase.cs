using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace miniORM.TestCase
{
    public class DataAccess
    {
        private static string DBConn = "server=.\\sqlexpress;database=testdb;uid=sa;pwd=sa;";
        private static DBHelper CreateDBHelper()
        {
            return new MSSQLHelper(DBConn);
        }
        public static List<WebInfo> GetData()
        {
            DBHelper db = CreateDBHelper();
            var reader = db.ExecuteReader("select BatchID, Cluster, AliasCluster, ClusterIP, ClusterVPort, MachineName, IPAddress, Port, [Status ], CreateTime, Zone from [dbo].[CF_WebInfo]");
            return EntityHelper<WebInfo>.SetValues(reader);
        }
        public static WebInfo GetDataOn()
        {
            DBHelper db = CreateDBHelper();
            var reader = db.ExecuteReader("select top 1 BatchID, Cluster, AliasCluster, ClusterIP, ClusterVPort, MachineName, IPAddress, Port, [Status ], CreateTime, Zone from [dbo].[CF_WebInfo]");
            return EntityHelper<WebInfo>.SetValue(reader);
        }
    }
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

    public class BusinessLayer
    {
        public void Index()
        {
            var data = DataAccess.GetData();
            //var da = DataAccess.GetDataOn();
            return View(data);
        }
    }
}
