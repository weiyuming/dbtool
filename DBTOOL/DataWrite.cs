using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DBTOOL.Helper;
using DBTOOL.Models;
using System.IO;
using System.Data;
using System.Drawing;

namespace DBTOOL
{
    class DataWrite
    {
        public delegate void UpdateUI(String msg);//声明一个更新主线程的委托
        public UpdateUI outputLog; 


        

        public delegate void AccomplishTask();//声明一个在完成任务时通知主线程的委托
        public AccomplishTask TaskCallBack;


        



        /// <summary>
        /// 从xml中获取 保存位置的路径，如果不存在则创建
        /// </summary>
        /// <returns></returns>
        //public String GetPath()
        //{


        //    List<PathVo> pathList = XmlHelper.GetPath();
        //    PathVo pathVo = pathList[0];
        //    String path = pathVo.TableSavePath;
        //    outputLog("【从配置中获取路径为】" + path);

        //    if (!Directory.Exists(@path))//若文件夹不存在则新建文件夹   
        //    {
        //        Directory.CreateDirectory(@path); //新建文件夹   
        //        outputLog("【创建文件夹】" + path);
        //    }
        //    return path;

        //}


        /// <summary>
        /// 调用导出方法
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="path"></param>
        /// <param name="fileName"></param>
        public void ExportBackToCsv(DataTable dt, String tableName, String path, String fileName, int frequency)
        {
           

            //outputLog("【导出表(" + tableName + ")开始】");
            //outputLog("【导出表文件为】" + fileName);
            //将表数据转成csv并保存
            ExportHelper.ExportDataGridToCSV(dt, path, fileName, frequency);
            //outputLog("【导出表(" + tableName + ")结束】");
        }

        /// <summary>
        /// 根据表名查询数据，返回一个dt
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public DataTable GetDataTableByTableName(String tableName)
        {
            //outputLog("【当前处理的表为】" + tableName);
            //String sql = "SELECT * FROM " + tableName + " where id = '2869101000000003873' fetch first 100 rows only with ur ";
            String sql = "SELECT * FROM " + tableName + "  with ur ";
            //outputLog("【获取表(" + tableName + ")数据开始】");
            //调用方法，查询表的数据
            DataTable dt = DBHelper.GetDataTable(sql);
            //outputLog("【获取表(" + tableName + ")数据结束】");

            return dt;

        }




        /// <summary>
        /// 根据表名查询数据，返回一个dt
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public DataTable GetDataTableByTableName(String tableName,int startSize,int endSize)
        {
            //outputLog("【当前处理的表为】" + tableName);
            //String sql = "SELECT * FROM " + tableName + " where id = '2869101000000003873' fetch first 100 rows only with ur ";
            String sql = "SELECT    *    FROM    (   SELECT  a.* , rownumber() over() AS RC     FROM  (select * from " + tableName + "  ) AS a    )  WHERE    rc BETWEEN " + startSize + " AND " + endSize + "   with ur ";
            //outputLog("【获取表(" + tableName + ")数据开始】");
            //调用方法，查询表的数据
            DataTable dt = DBHelper.GetDataTable(sql);
            //outputLog("【获取表(" + tableName + ")数据结束】");

            return dt;

        }





        // <summary>
        /// 根据表名查询数据的行数
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public int GetCountByTableName(String tableName)
        {
            int count = 0;
            String sql = "SELECT count(1) FROM " + tableName + "  with ur ";
            //调用方法，查询表的数据
            DataTable dt = DBHelper.GetDataTable(sql);

            count = Convert.ToInt32( dt.Rows[0][0]);
            return count;

        }


        /// <summary>
        /// 根据表名查询数据，返回一个dt
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public DataTable GetDataTableBySQL(String sql)
        {
            //outputLog("【获取表(" + tableName + ")数据开始】");
            //调用方法，查询表的数据
            DataTable dt = DBHelper.GetDataTable(sql);
            //outputLog("【获取表(" + tableName + ")数据结束】");

            return dt;

        }





        /// <summary>
        /// 主导出方法
        /// </summary>
        /// <param name="temp"></param>
        public void BackTable(object temp)
        {


            try
            {
                List<SettingVo> settingList = XmlHelper.GetSetting();
                //只有一个，直接取
                SettingVo vo = settingList[0];
                int maxRow = vo.MaxRow;

                String time = DateHelper.GetTime();


                




                //第一步，获取要保存的路径

                List<SettingVo> pathList = XmlHelper.GetSetting();
                SettingVo pathVo = pathList[0];
                String path = pathVo.TableSavePath;//不带当前日期的路径
                String pathTemp = path + "\\" + time;//拼接上当前时间，生成一个临时文件路径，所有文件放在临时文件路径下，方便后续打包及删除
                outputLog("【从配置中获取路径为】" + path);



                // 因为存在之前失败的情况，故此先删除一次非法文件
                List<String> fileList = FileHelper.getFileNameAll(path);
                foreach (String fileName in fileList)
                {
                    String[] tempStr = fileName.Split('.');

                    //不带后缀名的文件全部删除

                    if (tempStr.Count() < 2)
                    {
                        FileHelper.DeleteFile(path + "\\" + fileName);
                        outputLog("【正在删除未获取到后缀名的文件】" + path + "\\" + fileName);
                        continue;
                    }

                    String tempFileName = tempStr[0];
                    String filetType = tempStr[1];

                    //非zip的文件直接删除


                    if (!"rar".Equals(filetType))
                    {
                        FileHelper.DeleteFile(path + "\\" + fileName);
                        outputLog("【正在删除非RAR文件】" + path + "\\" + fileName);
                        continue;
                    }

                }


                if (!Directory.Exists(pathTemp))//若文件夹不存在则新建文件夹   
                {
                    outputLog("【创建临时文件夹】" + pathTemp);
                    Directory.CreateDirectory(pathTemp); //新建文件夹   

                }




                






                //第二步，获取所有要备份的的表
                List<Table> list = XmlHelper.GetTable(); //获取所有要备份的表

                //第三步，执行查询及导出csv的操作
                int count = list.Count;
                int i = 1;
                foreach (Table table in list)
                {


                    outputLog("");//为了日志美观，插入一个空行

                    outputLog("【总体进度】" + i + "/" + count);
                    String tableName = table.TableName;  //实际的表名
                    String fileName = tableName + "_" + time;
                    String backTableName = tableName + "_" + time;//表名+下划线




                    outputLog("【当前处理的表为】" + tableName);
                    // 获取总的数据行数
                    //outputLog("【获取当前表的总行数开始】" + i + "/" + count);
                    int tableRows = GetCountByTableName(tableName);
                    outputLog("【当前表的总行数为】" + tableRows);
                    // Boolean isContinue = true;//是否继续，默认继续
                    //计算需要多少次才能处理完
                    int size = (tableRows / maxRow);
                    if ((tableRows % maxRow) > 0)
                    {
                        size++;
                    }
                    outputLog("【计算后本表的循环次数为】" + size);

                    if (size > 0)// 小于等0 则表示表内无数据
                    {
                        //2、生成建表语句
                        fileName = backTableName + "【建表语句】.sql";
                        outputLog("【开始生成建表语句】" + pathTemp + "\\" + fileName);
                        String sql = ExportHelper.DataTableExportCreateTableSQL(tableName, backTableName, pathTemp, fileName);


                        for (int j = 1; j <= size; j++)
                        {
                            int countStart = 0;
                            int countEnd = 0;

                            countStart = (j - 1) * maxRow + 1;
                            countEnd = j * maxRow;


                            outputLog("");//为了日志美观，插入一个空行

                            //循环分页查询数据，并导出
                            //获取数据集合

                            outputLog("【开始循环获取表数据】" + j + "/" + size);
                            DataTable dt = GetDataTableByTableName(tableName, countStart, countEnd);


                            //3、导出csv
                            fileName = backTableName + ".csv";
                            outputLog("【开始循环生成CSV文件 " + j + "/" + size + " 】" + pathTemp + "\\" + fileName);
                            //将表数据转成csv并保存
                            ExportBackToCsv(dt, tableName, pathTemp, fileName, j);


                            //4、生成insert语句  是带后缀名的表
                            fileName = backTableName + "【INSERT语句】.sql";
                            outputLog("【开始循环生成INSERT语句 " + j + "/" + size + " 】" + pathTemp + "\\" + fileName);
                            ExportHelper.DataTableExportInsertSQL(dt, tableName, backTableName, pathTemp, fileName);




                        }
                    }
                    else
                    {
                        outputLog("【当前表内无数据，不进行处理】" + tableName);
                    }

                    i++;
                }




                System.GC.Collect();//手动进行一次垃圾回收


                //第四步，将文件夹打包

                //FileHelper.addpathPower(pathTemp, pathVo.UserName, "FullControl");//给子文件夹一个读写权限
                //FileHelper.addpathPower(path, pathVo.UserName, "FullControl");//给父文件夹一个读写权限

                //FileHelper.RemoveReadOnly(pathTemp);//去除文件夹的只读属性
                //FileHelper.RemoveReadOnly(path);//去除文件夹的只读属性

                outputLog("【打包压缩文件开始】需要打包的文件为：" + pathTemp);
                String exportFile = pathTemp + ".rar";//将路径直接转换为要打包的文件名
                Boolean success = false;
                //ZipHelper.Zip(pathTemp, exportFile, ref errorOut, 6);//此打包方式用的是内置的dll，打包大文件时会有问题

                success = ZipHelper.CondenseRarOrZip(pathTemp, exportFile, true, null, true);//调用的本机的rar
                outputLog("【打包压缩文件结束】打包后的文件为：" + exportFile);



                System.GC.Collect();//手动进行一次垃圾回收


                //第五步，删除打包前的文件夹
                outputLog("【删除文件开始】" + pathTemp);
                if (success)//打包成功才删除文件夹
                {
                    FileHelper.DeleteFile(pathTemp);

                }

                outputLog("【删除文件结束】");


                //删除N天前的文件
                int day = 2;

                fileList = FileHelper.getFileName(path);
                foreach (String fileName in fileList)
                {
                    String[] tempStr = fileName.Split('.');

                    //不带后缀名的文件全部删除

                    if (tempStr.Count() < 2)
                    {
                        FileHelper.DeleteFile(path + "\\" + fileName);
                        outputLog("【正在删除未获取到后缀名的文件】" + path + "\\" + fileName);
                        continue;
                    }

                    String tempFileName = tempStr[0];
                    String filetType = tempStr[1];

                    //非zip的文件直接删除


                    if (!"rar".Equals(filetType))
                    {
                        FileHelper.DeleteFile(path + "\\" + fileName);
                        outputLog("【正在删除非RAR文件】" + path + "\\" + fileName);
                        continue;
                    }


                    //简单判断
                    DateTime oldDate = DateTime.ParseExact(tempFileName.Substring(0, 8), "yyyyMMdd", null);
                    DateTime newDate = DateTime.ParseExact(time.Substring(0, 8), "yyyyMMdd", null);
                    TimeSpan ts = newDate - oldDate;
                    int resultDay = ts.Days; ;

                    if (resultDay > day)
                    {
                        FileHelper.DeleteFile(path + "\\" + fileName);
                        outputLog("【正在删除大于" + day + "天的文件】" + path + "\\" + fileName);
                    }

                }

                System.GC.Collect();//手动进行一次垃圾回收
                outputLog("【===== 数据操作结束 =====】");
            }
            catch (Exception ex)
            {
                outputLog("【出现异常】"+ex.Message);
            }



            
        }




        public void test(Object obj)
        {

            //string pathTemp = @"D:\wym\dbback\20180517112532";
            string pathTemp = @obj.ToString();
           
           

            if (StringHelper.isParameterEmpty(pathTemp))
            {
                outputLog("获取打包文件路径失败");
            }
            else
            { 
                outputLog("【打包压缩文件开始】需要打包的文件为：" + pathTemp);
                string exportFile = pathTemp + ".rar";//将路径直接转换为要打包的文件名
                //ZipHelper.Zip(pathTemp, exportFile, ref errorOut, 6);
                Boolean temp = ZipHelper.CondenseRarOrZip(pathTemp, exportFile, true, null, true);
                outputLog(temp + "");
                outputLog("【打包压缩文件结束】打包后的文件为：" + exportFile);
            }

            
           
        }



        
        

    }
}
