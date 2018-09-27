using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.OleDb;

namespace DBTOOL.Helper
{
    static class DBHelper
    {



        /// <summary>
        /// 从web.config中获得数据库连接字符串
        /// </summary>
        //public static string Connstr
        //{
        //    get { return "Data Source=.\\SQLEXPRESS;Initial Catalog=MyBookShop;Integrated Security=True"; }
        //}
        public static string Connstr
        {
            //引用的配置文件的 连接字符串
            get { return @"Provider=IBMDADB2;HostName=10.0.198.148;Database=pjt;uid=erpex;pwd=CaiGjt2017;protocol=TCPIP;port=50000;"; }
        }



        /// <summary>
        /// 执行sql语句返回DataTable
        /// </summary>
        /// <param name="sql">安全的sql语句</param>
        /// <returns>根据sql语句得到所有记录</returns>
        public static DataTable GetDataTable(string sql)
        {
            //using 只在{} 内有效
            using (OleDbConnection conn = new OleDbConnection(Connstr)) //创建数据库连接对象(打开数据库)
            {
                OleDbDataAdapter oad = new OleDbDataAdapter(sql,conn);
                //conn.Open();
                //OleDbCommand cmd = new OleDbCommand(sql, conn);
                //OleDbDataReader rdr = cmd.ExecuteReader();
                DataTable dt = new DataTable();
                oad.Fill(dt);
                //conn.Close();
                return dt;
            }



        }
    }
}
