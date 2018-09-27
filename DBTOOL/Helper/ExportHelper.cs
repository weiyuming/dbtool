using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.IO;
using System.Collections;

namespace DBTOOL.Helper
{
    static class ExportHelper
    {
        /// <summary>
        /// 导出csv文件到指定路径
        /// </summary>
        /// <param name="grid"></param>
        public static bool ExportDataGridToCSV(DataTable dt, String path, String fileName, int frequency)
        {

            path = path + "\\" + fileName;//拼接路径和文件名
            
            System.IO.FileStream fs = new FileStream(path, System.IO.FileMode.Append, System.IO.FileAccess.Write);
            using(StreamWriter sw = new StreamWriter(fs, new System.Text.UnicodeEncoding()))
            {
                try
                {

                    if (frequency == 1)//第一次才写头信息，否则不写
                    {
                        //Tabel header
                        for (int i = 0; i < dt.Columns.Count; i++)
                        {
                            sw.Write(dt.Columns[i].ColumnName);
                            sw.Write("\t");
                        }
                        sw.WriteLine("");
                    }
                    

                    String value = "";
                    //Table body
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        for (int j = 0; j < dt.Columns.Count; j++)
                        {
                            value = dt.Rows[i][j].ToString();

                            //if (!StringHelper.isParameterEmpty(value) && StringHelper.IsNumber(value) && value.Length >= 13)
                            //{
                            //    sw.Write("=" + value);
                            //}
                            //else
                            //{
                            //    sw.Write(value);
                            //}

                            sw.Write(value);//此处有问题，如果是数值格式的会有问题，暂时不知道如何解决
                            sw.Write("\t");
                            //sw.Write(dt.Rows[i][j].ToString());
                        }
                        sw.WriteLine("");
                    }

                    return true;
                }
                catch (Exception err)
                {
                    return false;
                    throw err;
                }
                finally
                {
                    //释放资源
                    sw.Flush();
                    sw.Close();
                }
            }
            
            

        }





        /// <summary>  
        /// 由数据集DataSet自动生成Insert的SQL语句集合  
        /// </summary>  
        /// <param name="ds">数据集</param>  
        /// <param name="TableName">表名_不带下划线</param>  
        /// <param name="backTableName">表名_带下划线</param>  
        /// <returns></returns>  
        public static List<String> DataTableToInsertSQLArrayList(DataTable dt, String TableName,String backTableName)
        {

            String sql = " SELECT    lums.NAME,    lums.TYPENAME,    lums.LONGLENGTH,    lums.DEFAULT,    lums.NULLS,    lums.REMARKS,    lums.KEYSEQ,    lums.PARTKEYSEQ,    lums.TBNAME,    lums.TBCREATOR,    lums.COLTYPE,    lums.CODEPAGE,    lums.DBCSCODEPG,    lums.LENGTH,    lums.SCALE,    lums.COLNO,    lums.COLCARD,    lums.HIGH2KEY,    lums.LOW2KEY,    lums.AVGCOLLEN,    lums.TYPESCHEMA,    lums.LOGGED,    lums.COMPACT,    lums.NQUANTILES,    lums.NMOSTFREQ,    lums.COMPOSITE_CODEPAGE,    lums.SOURCE_TABSCHEMA,    lums.SOURCE_TABNAME,    lums.HIDDEN,    lums.GENERATED,    lums.INLINE_LENGTH,    lums.NUMNULLS,    lums.AVGCOLLENCHAR,    lums.SUB_COUNT,    lums.SUB_DELIM_LENGTH,    lums.IDENTITY,    lums.COMPRESS,    lums.AVGDISTINCTPERPAGE,    lums.PAGEVARIANCERATIO,    lums.IMPLICITVALUE,    lums.SECLABELID,    lums.COLLATIONID,    lums.PCTINLINED   FROM    sysibm.SYSCOLUMNS lums WHERE    tbname = '" + TableName + "'; ";
            DataWrite dataWrite = new DataWrite();
            DataTable jigouDt = dataWrite.GetDataTableBySQL(sql);//获取一次表结构

            Dictionary<string, string> jiegouDic = new Dictionary<string, string>();
            for (int i = 0; i < jigouDt.Rows.Count; i++)
            {
                String name = jigouDt.Rows[i][0].ToString();
                String typeName = jigouDt.Rows[i][1].ToString();
                jiegouDic.Add(name,typeName);//key是字段名，value是字段类型
            }



            List<String> allSql = new List<String>();


            List<String> ziduanList = new List<String>();

            //  
            string FieldAll = "";
            //获取列名集合  
            for (int i = 0; i < dt.Columns.Count; i++)// 因为使用了分页的写法，所以最后一列叫做RC，此处去掉
            {

                String value = dt.Columns[i].ColumnName.ToString();

                if (!(i == dt.Columns.Count-1 && StringHelper.isEqual("RC",value)))//最后一个，并且叫做RC 则不处理
                {
                    ziduanList.Add(value);
                    FieldAll = FieldAll + value + ",";
                }
                
            }
            FieldAll = FieldAll.Substring(0, FieldAll.Length - 1);//去掉最后一个“，”  

            DataView dv = dt.DefaultView;
            string ValueAll = "";
            //判断字段类型，其实也可以全部用单引号引起来，只是数据库处理时，内部需要转化类型  
            for (int n = 0; n < dv.Count; n++)//  用于循环写insert  
            {
                for (int m = 0; m < dt.Columns.Count-1; m++)//  因为分页，所以最后一列是需要，实际是无用的，所以要-1 将其去掉
                {




                    switch (dv[n][m].GetType().ToString())
                    {
                        case "System.DateTime":

                            ValueAll += "'" + (Convert.ToDateTime(dv[n][m])).ToString("yyyy-MM-dd HH:mm:ss") + "'";
                            ValueAll += ", ";
                            break;
                        case "System.String":
                            ValueAll += "'" + dv[n][m].ToString() + "'";
                            ValueAll += ", ";
                            break;
                        case "System.Int32":
                            ValueAll += Convert.ToInt32(dv[n][m]);
                            ValueAll += ", ";
                            break;
                        case "System.Single":
                            ValueAll += Convert.ToSingle(dv[n][m]);
                            ValueAll += ", ";
                            break;
                        case "System.Double":
                            ValueAll += Convert.ToDouble(dv[n][m]);
                            ValueAll += ", ";
                            break;
                        case "System.Decimal":
                            ValueAll += Convert.ToDecimal(dv[n][m]);
                            ValueAll += ", ";
                            break;
                        //case "System.TIMESTAMP":
                        //    if (true)
                        //    {

                        //    }
                        //    ValueAll += (dv[n][m]);
                        //    ValueAll += ",";
                        //    break;
                        default:

                            String name = ziduanList[m];//字段名
                            String type = jiegouDic[name];//根据字段名取字段类型

                            if (StringHelper.isEqual("TIMESTAMP", type) || StringHelper.isEqual("INTEGER", type) || StringHelper.isEqual("DECIMAL", type) || StringHelper.isEqual("BIGINT", type) || StringHelper.isEqual("LONG", type))
                            {
                                ValueAll += "null";
                                ValueAll += ", ";
                            }
                            else
                            {
                                ValueAll += "'" + dv[n][m].ToString() + "'";
                                ValueAll += ", ";
                            }
                            break;
                    }
                }
                ValueAll = ValueAll.Substring(0, ValueAll.Length - 1); //去掉最后一个“，”  
                allSql.Add("insert into " + backTableName + " (" + FieldAll + ") values (" + ValueAll + ");");//insert  
                ValueAll = "";//清空  继续循环  
            }
            return allSql; //返回语句  
        }


        /// <summary>
        /// 由数据集DataSet自动生成Insert的SQL语句集合,并导出为sql语句 
        /// </summary>
        /// <param name="ds">数据集</param>  
        /// <param name="TableName">表名_不带下划线</param>  
        /// <param name="backTableName">表名_带下划线</param>  
        /// <param name="path">路径名</param>
        /// <param name="fileName">导出的表名</param>
        /// <returns></returns>
        public static String DataTableExportInsertSQL(DataTable dt, String tableName,String backTableName,String path, String fileName)
        {
            String allSql = "";
            List<String> list = DataTableToInsertSQLArrayList(dt, tableName, backTableName);

            String pathFile = path + "\\" + fileName;


            //用于强制资源释放
            System.IO.FileStream fs = new FileStream(pathFile, System.IO.FileMode.Append, System.IO.FileAccess.Write);
            using (StreamWriter sw = new StreamWriter(fs,  new System.Text.UnicodeEncoding()))
            {

                try
                {
                    foreach (String str in list)
                    {
                        sw.WriteLine(str);//写入行数据
                    }
                }
                catch (Exception err)
                {
                    throw err;

                }
                finally
                {
                    //释放资源
                    sw.Flush();
                    sw.Close();
                }
            }
            return allSql; //返回语句  
        }




        /// <summary>  
        /// 由数据集DataSet自动生成DB2的建表语句
        /// </summary>  
        /// <param name="ds">数据集</param>  
        /// <param name="TableName">表名</param>  
        /// <returns></returns>  
        public static String DataTableToCreateTableSQL(String tableName,String backTableName)
        {
            String sql = " SELECT    lums.NAME,    lums.TYPENAME,    lums.LONGLENGTH,    lums.DEFAULT,    lums.NULLS,    lums.REMARKS,    lums.KEYSEQ,    lums.LENGTH,    lums.SCALE,    lums.PARTKEYSEQ,    lums.TBNAME,    lums.TBCREATOR,    lums.COLTYPE,    lums.CODEPAGE,    lums.DBCSCODEPG,    lums.COLNO,    lums.COLCARD,    lums.HIGH2KEY,    lums.LOW2KEY,    lums.AVGCOLLEN,    lums.TYPESCHEMA,    lums.LOGGED,    lums.COMPACT,    lums.NQUANTILES,    lums.NMOSTFREQ,    lums.COMPOSITE_CODEPAGE,    lums.SOURCE_TABSCHEMA,    lums.SOURCE_TABNAME,    lums.HIDDEN,    lums.GENERATED,    lums.INLINE_LENGTH,    lums.NUMNULLS,    lums.AVGCOLLENCHAR,    lums.SUB_COUNT,    lums.SUB_DELIM_LENGTH,    lums.IDENTITY,    lums.COMPRESS,    lums.AVGDISTINCTPERPAGE,    lums.PAGEVARIANCERATIO,    lums.IMPLICITVALUE,    lums.SECLABELID,    lums.COLLATIONID,    lums.PCTINLINED   FROM    sysibm.SYSCOLUMNS lums WHERE    tbname = '" + tableName + "'; ";
            DataWrite dataWrite = new DataWrite();
            DataTable dt = dataWrite.GetDataTableBySQL(sql);

            String createTableSql = "CREATE  TABLE " + backTableName + " ( ";

            String remarkSql = "";
            String pks = "";

            String value = "";
            for (int i = 0; i < dt.Rows.Count; i++)//获取行
            {


                //    //lums.NAME, 字段名
                //    //lums.TYPENAME,  字段数据类型
                //    //lums.LONGLENGTH, 字段长度
                //    //lums.DEFAULT, 默认值
                //    //lums.NULLS, 是否为空  Y可以为空  N不可以为空
                //    //lums.REMARKS,  备注说明
                //    //lums.KEYSEQ,   主键，  1标示主键或者唯一索引，此处默认他是主键
                //    String columnName = dt.Columns[j].ColumnName.ToString();//获取名，只需要用到前几列，并且sql里面固定顺序了
                String name  = dt.Rows[i][0].ToString();
                String typeName = dt.Rows[i][1].ToString();
                String longLength = dt.Rows[i][2].ToString();
                String defaultStr = dt.Rows[i][3].ToString();
                String isNull = dt.Rows[i][4].ToString();
                String remarks = dt.Rows[i][5].ToString();
                String key = dt.Rows[i][6].ToString();
                String lenght = dt.Rows[i][7].ToString();
                String scale = dt.Rows[i][8].ToString();

                createTableSql += " " + name + " " ;

                //部分数据类型不应该拼接字段长度
                if (StringHelper.isEqual("TIMESTAMP", typeName) || StringHelper.isEqual("INTEGER", typeName) || StringHelper.isEqual("BIGINT", typeName) || StringHelper.isEqual("LONG VARCHAR", typeName))
                {
                    createTableSql += " " + typeName;
                }
                else
                {
                    if (StringHelper.isEqual("DECIMAL", typeName))  //长度是（16,2）的情况
                    {
                        createTableSql += " " + typeName + "(" + longLength + "," + scale + ") ";
                    }
                    else
                    {
                        createTableSql += " " + typeName + "(" + longLength + ") ";
                    }
                    
                }


                //默认值
                if (!StringHelper.isParameterEmpty(defaultStr))
                {
                    createTableSql += " " + "DEFAULT " + defaultStr;
                }

                //是否可以为空
                if (StringHelper.isEqual("N",isNull))
                {
                    createTableSql += " " + "NOT NULL ";
                }

                //主键
                if (!StringHelper.isParameterEmpty(key))
                {
                    pks += "  " + name + ",";
                }


                //备注
                if (!StringHelper.isParameterEmpty(remarks))
                {
                    remarkSql += " COMMENT ON COLUMN " + backTableName + "." + name + " IS  ' " + remarks + " ' ; ";
                }

                createTableSql += " ,"; 

                #region  原来打算循环取数，现在决定直接取第X列
                //for (int j = 0; j < dt.Columns.Count; j++)//获取列
                //{
                //    //lums.NAME, 字段名
                //    //lums.TYPENAME,  字段数据类型
                //    //lums.LONGLENGTH, 字段长度
                //    //lums.DEFAULT, 默认值
                //    //lums.NULLS, 是否为空  Y可以为空  N不可以为空
                //    //lums.REMARKS,  备注说明
                //    //lums.KEYSEQ,   主键，  1标示主键或者唯一索引，此处默认他是主键
                //    String columnName = dt.Columns[j].ColumnName.ToString();//获取名，只需要用到前几列，并且sql里面固定顺序了
                //    value = dt.Rows[i][j].ToString();

                //    //只取前几列
                //    if (StringHelper.isEqual("NAME", columnName))
                //    {
                //        createTableSql += "" + value;
                //    }
                //    else if (StringHelper.isEqual("TYPENAME", columnName))
                //    {
                //        createTableSql += "" + value;
                //    }
                //}
                #endregion
            }


            //循环完成后处理数据
            //截取最后的逗号
            if (!StringHelper.isParameterEmpty(createTableSql))
            {
                createTableSql = createTableSql.Substring(0, createTableSql.Length - 1);
            }
            

            if (!StringHelper.isParameterEmpty(pks))
            {
                pks = pks.Substring(0, pks.Length - 1);
                pks = ", CONSTRAINT P_Key_1 PRIMARY KEY (" + pks + ") ";
            }
            

            //开始拼接数据
            createTableSql += pks;  //拼接上主键
            createTableSql += " ); "; //拼接上后面的括号

            createTableSql += remarkSql;  //拼接上备注说明
 
            return createTableSql; //返回语句  
        }


        /// <summary>
        /// 由数据集DataSet自动生成DB2的建表语句
        /// </summary>
        /// <param name="tableName">表名，不带下划线</param>
        /// <param name="backTableName">表名，带下划线</param>
        /// <param name="path">路径</param>
        /// <param name="fileName">导出的文件名</param>
        /// <returns></returns>
        public static String DataTableExportCreateTableSQL(String tableName, String backTableName, String path, String fileName)
        {


            String allSql = DataTableToCreateTableSQL(tableName, backTableName);

            String pathFile = path + "\\" + fileName;


            //用于强制资源释放
            System.IO.FileStream fs = new FileStream(pathFile, System.IO.FileMode.Append, System.IO.FileAccess.Write);
            using (StreamWriter sw = new StreamWriter(fs, new System.Text.UnicodeEncoding()))
            {

                try
                {

                    sw.WriteLine(allSql);//写入行数据

                }
                catch (Exception err)
                {
                    throw err;

                }
                finally
                {
                    //释放资源
                    sw.Flush();
                    sw.Close();
                }
            }
            return allSql; //返回语句 
        }

    }
}
