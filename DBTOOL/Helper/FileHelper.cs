using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Security.AccessControl;

namespace DBTOOL.Helper
{
    static class FileHelper
    {
        public static void addpathPower(string pathname, string username, string power)
        {

            DirectoryInfo dirinfo = new DirectoryInfo(pathname);

            if ((dirinfo.Attributes & FileAttributes.ReadOnly) != 0)
            {
                dirinfo.Attributes = FileAttributes.Normal;
            }

            //取得访问控制列表   
            DirectorySecurity dirsecurity = dirinfo.GetAccessControl();

            switch (power)
            {
                case "FullControl":
                    dirsecurity.AddAccessRule(new FileSystemAccessRule(username, FileSystemRights.FullControl, InheritanceFlags.ContainerInherit, PropagationFlags.InheritOnly, AccessControlType.Allow));
                    break;
                case "ReadOnly":
                    dirsecurity.AddAccessRule(new FileSystemAccessRule(username, FileSystemRights.Read, AccessControlType.Allow));
                    break;
                case "Write":
                    dirsecurity.AddAccessRule(new FileSystemAccessRule(username, FileSystemRights.Write, AccessControlType.Allow));
                    break;
                case "Modify":
                    dirsecurity.AddAccessRule(new FileSystemAccessRule(username, FileSystemRights.Modify, AccessControlType.Allow));
                    break;
            }
            dirinfo.SetAccessControl(dirsecurity);
        }


        //https://www.cnblogs.com/hanmos/archive/2011/01/28/1946974.html


        /// <summary>
        /// 去除文件夹的只读属性
        /// </summary>
        /// <param name="path"></param>
        public static void RemoveReadOnly(String path)
        {
            System.IO.DirectoryInfo DirInfo = new DirectoryInfo(path);
            DirInfo.Attributes = FileAttributes.Normal & FileAttributes.Directory;
        
        }


        /// <summary>
        /// 去除文件的只读属性
        /// </summary>
        /// <param name="path"></param>
        //public static void setFileAttributes(String path)
        //{
        //    System.IO.File.SetAttributes("filepath", System.IO.FileAttributes.Normal);
        
        //}



        /// <summary>
        /// 根据路径删除文件
        /// </summary>
        /// <param name="path"></param>
        public static void DeleteFile(string path)
        {
            FileAttributes attr = File.GetAttributes(path);
            if (attr == FileAttributes.Directory)
            {
                Directory.Delete(path, true);
            }
            else
            {
                File.Delete(path);
            }
        }




        /// <summary>
        /// 获得指定路径下所有文件名
        /// </summary>
        /// <param name="path"></param>
        public static List<String> getFileName(string path)
        {
            List<String> list = new List<string>();
            DirectoryInfo root = new DirectoryInfo(path);
            foreach (FileInfo f in root.GetFiles())
            {
                list.Add(f.Name);
            }
            return list;
        }

        ////获得指定路径下所有子目录名
        //public static void getDirectory(StreamWriter sw, string path, int indent)
        //{
        //    getFileName(sw, path, indent);
        //    DirectoryInfo root = new DirectoryInfo(path);
        //    foreach (DirectoryInfo d in root.GetDirectories())
        //    {
        //        for (int i = 0; i < indent; i++)
        //        {
        //            sw.Write("  ");
        //        }
        //        sw.WriteLine("文件夹：" + d.Name);
        //        getDirectory(sw, d.FullName, indent + 2);
        //        sw.WriteLine();
        //    }
        //}
        
        
    }
}
