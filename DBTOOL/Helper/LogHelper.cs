using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace DBTOOL.Helper
{
    class LogHelper
    {
        public static void SetLog(String msg)
        {

            String filePath = "log.txt";

            //string str = System.Environment.CurrentDirectory;
            //String filePath = str+"\\log.txt";
            try
            {
                //FileStream fs = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite);
                //StreamWriter sw = new StreamWriter(fs);

                StreamWriter sw = new StreamWriter(filePath, true);
                sw.WriteLine(msg);


                sw.Close();
                //fs.Close();

                //if (!File.Exists(filePath))
                //{
                //    FileStream fs1 = new FileStream(@filePath, FileMode.Create, FileAccess.Write);//创建写入文件 
                //    StreamWriter sw = new StreamWriter(fs1);
                //    sw.WriteLine(msg);//开始写入值
                //    sw.Close();
                //    fs1.Close();
                //}
                //else
                //{
                //    FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Write);
                //    StreamWriter sr = new StreamWriter(fs);
                //    sr.WriteLine(msg);//开始写入值
                //    sr.Close();
                //    fs.Close();
                //}



            }
            catch (Exception err)
            {
                throw err;
            }
            finally
            {
            }

        }
    }
}
