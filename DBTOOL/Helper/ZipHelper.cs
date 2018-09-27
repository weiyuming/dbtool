using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using ICSharpCode.SharpZipLib.Checksums;
using ICSharpCode.SharpZipLib.Zip;
using System.Diagnostics;

namespace DBTOOL.Helper
{
    class ZipHelper
    {





        /// <summary>
        /// 解压RAR和ZIP文件(需存在Winrar.exe(只要自己电脑上可以解压或压缩文件就存在Winrar.exe))
        /// </summary>
        /// <param name="UnPath">解压后文件保存目录</param>
        /// <param name="rarPathName">待解压文件存放绝对路径（包括文件名称）</param>
        /// <param name="IsCover">所解压的文件是否会覆盖已存在的文件(如果不覆盖,所解压出的文件和已存在的相同名称文件不会共同存在,只保留原已存在文件)</param>
        /// <param name="PassWord">解压密码(如果不需要密码则为空)</param>
        /// <returns>true(解压成功);false(解压失败)</returns>
        public static bool UnRarOrZip(string UnPath, string rarPathName, bool IsCover, string PassWord)
        {
            if (!Directory.Exists(UnPath))
                Directory.CreateDirectory(UnPath);
            Process Process1 = new Process();
            Process1.StartInfo.FileName = "Winrar.exe";
            Process1.StartInfo.CreateNoWindow = true;
            string cmd = "";
            if (!string.IsNullOrEmpty(PassWord) && IsCover)
                //解压加密文件且覆盖已存在文件( -p密码 )
                cmd = string.Format(" x -p{0} -o+ {1} {2} -y", PassWord, rarPathName, UnPath);
            else if (!string.IsNullOrEmpty(PassWord) && !IsCover)
                //解压加密文件且不覆盖已存在文件( -p密码 )
                cmd = string.Format(" x -p{0} -o- {1} {2} -y", PassWord, rarPathName, UnPath);
            else if (IsCover)
                //覆盖命令( x -o+ 代表覆盖已存在的文件)
                cmd = string.Format(" x -o+ {0} {1} -y", rarPathName, UnPath);
            else
                //不覆盖命令( x -o- 代表不覆盖已存在的文件)
                cmd = string.Format(" x -o- {0} {1} -y", rarPathName, UnPath);
            //命令
            Process1.StartInfo.Arguments = cmd;
            Process1.Start();
            Process1.WaitForExit();//无限期等待进程 winrar.exe 退出
            //Process1.ExitCode==0指正常执行，Process1.ExitCode==1则指不正常执行
            if (Process1.ExitCode == 0)
            {
                Process1.Close();
                return true;
            }
            else
            {
                Process1.Close();
                return false;
            }

        }

        /// <summary>
        /// 压缩文件成RAR或ZIP文件(需存在Winrar.exe(只要自己电脑上可以解压或压缩文件就存在Winrar.exe))
        /// </summary>
        /// <param name="filesPath">将要压缩的文件夹或文件的绝对路径</param>
        /// <param name="rarPathName">压缩后的压缩文件保存绝对路径（包括文件名称）</param>
        /// <param name="IsCover">所压缩文件是否会覆盖已有的压缩文件(如果不覆盖,所压缩文件和已存在的相同名称的压缩文件不会共同存在,只保留原已存在压缩文件)</param>
        /// <param name="PassWord">压缩密码(如果不需要密码则为空)</param>
        /// <returns>true(压缩成功);false(压缩失败)</returns>
        public static bool CondenseRarOrZip(string filesPath, string rarPathName, bool IsCover, string PassWord, bool isBackstage)
        {
            string rarPath = Path.GetDirectoryName(rarPathName);
            if (!Directory.Exists(rarPath))
                Directory.CreateDirectory(rarPath);

            Process Process1 = new Process();
            Process1.StartInfo.FileName = "Winrar.exe";
            Process1.StartInfo.CreateNoWindow = true;


            string cmd = " a -ep1 ";

            if (isBackstage)
            {
                cmd += " -ibck ";
            }
            

            if (!string.IsNullOrEmpty(PassWord) && IsCover)
                //压缩加密文件且覆盖已存在压缩文件( -p密码 -o+覆盖 )
                cmd += string.Format(" -p{0} -o+ {1} {2} -r", PassWord, rarPathName, filesPath);
            else if (!string.IsNullOrEmpty(PassWord) && !IsCover)
                //压缩加密文件且不覆盖已存在压缩文件( -p密码 -o-不覆盖 )
                cmd += string.Format(" -p{0} -o- {1} {2} -r", PassWord, rarPathName, filesPath);
            else if (string.IsNullOrEmpty(PassWord) && IsCover)
                //压缩且覆盖已存在压缩文件( -o+覆盖 )
                cmd += string.Format(" -o+ {0} {1} -r", rarPathName, filesPath);
            else
                //压缩且不覆盖已存在压缩文件( -o-不覆盖 )
                cmd += string.Format(" -o- {0} {1} -r", rarPathName, filesPath);

            //命令
            Process1.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;  //隐藏 WinRAR 窗口  
            //Process1.StartInfo.CreateNoWindow = true; //設置不顯示窗口
            Process1.StartInfo.Arguments = cmd;
            Process1.Start();
            Process1.WaitForExit();//无限期等待进程 winrar.exe 退出
            //Process1.ExitCode==0指正常执行，Process1.ExitCode==1则指不正常执行
            if (Process1.ExitCode == 0)
            {
                Process1.Close();
                return true;
            }
            else
            {
                Process1.Close();
                return false;
            }

        }








        #region 压缩

        #region 公开压缩方法
        /// <summary>     
        /// 压缩文件或文件夹   ----带密码  
        /// </summary>     
        /// <param name="fileToZip">要压缩的路径-文件夹或者文件</param>     
        /// <param name="zipedFile">压缩后的文件名</param>     
        /// <param name="password">密码</param>  
        /// <param name="errorOut">如果失败返回失败信息</param>  
        /// <param name="compressLevel">压缩级别 0-9   数字越大压缩率越高</param>  
        /// <returns>压缩结果</returns>     
        public static bool Zip(string fileToZip, string zipedFile, string password, ref string errorOut, int compressLevel)
        {
            bool result = false;
            try
            {
                if (Directory.Exists(fileToZip))
                    result = ZipDirectory(fileToZip, zipedFile, password, compressLevel);
                else if (File.Exists(fileToZip))
                    result = ZipFile(fileToZip, zipedFile, password);
            }
            catch (Exception ex)
            {
                errorOut = ex.Message;
            }
            return result;
        }

        /// <summary>     
        /// 压缩文件或文件夹 ----无密码   
        /// </summary>     
        /// <param name="fileToZip">要压缩的路径-文件夹或者文件</param>     
        /// <param name="zipedFile">压缩后的文件名</param>  
        /// <param name="errorOut">如果失败返回失败信息</param>  
        /// <param name="compressLevel">压缩级别 0-9   数字越大压缩率越高</param>  
        /// <returns>压缩结果</returns>     
        public static bool Zip(string fileToZip, string zipedFile, ref string errorOut, int compressLevel)
        {
            bool result = false;
            try
            {
                if (Directory.Exists(fileToZip))
                    result = ZipDirectory(fileToZip, zipedFile, null, compressLevel);
                else if (File.Exists(fileToZip))
                    result = ZipFile(fileToZip, zipedFile, null);
            }
            catch (Exception ex)
            {
                errorOut = ex.Message;
            }
            return result;
        }
        #endregion

        #region 内部处理方法
        /// <summary>     
        /// 压缩文件     
        /// </summary>     
        /// <param name="fileToZip">要压缩的文件全名</param>     
        /// <param name="zipedFile">压缩后的文件名</param>     
        /// <param name="password">密码</param>     
        /// <returns>压缩结果</returns>     
        private static bool ZipFile(string fileToZip, string zipedFile, string password)
        {
            bool result = true;
            ZipOutputStream zipStream = null;
            FileStream fs = null;
            ZipEntry ent = null;

            if (!File.Exists(fileToZip))
                return false;

            try
            {
                fs = File.OpenRead(fileToZip);
                byte[] buffer = new byte[fs.Length];
                fs.Read(buffer, 0, buffer.Length);
                fs.Close();

                fs = File.Create(zipedFile);
                zipStream = new ZipOutputStream(fs);
                if (!string.IsNullOrEmpty(password)) zipStream.Password = password;
                ent = new ZipEntry(Path.GetFileName(fileToZip));
                zipStream.PutNextEntry(ent);
                zipStream.SetLevel(6);

                zipStream.Write(buffer, 0, buffer.Length);

            }
            catch (Exception ex)
            {
                result = false;
                throw ex;
            }
            finally
            {
                if (zipStream != null)
                {
                    zipStream.Finish();
                    zipStream.Close();
                }
                if (ent != null)
                {
                    ent = null;
                }
                if (fs != null)
                {
                    fs.Close();
                    fs.Dispose();
                }
            }
            GC.Collect();
            GC.Collect(1);

            return result;
        }

        /// <summary>  
        /// 压缩文件夹  
        /// </summary>  
        /// <param name="strFile">带压缩的文件夹目录</param>  
        /// <param name="strZip">压缩后的文件名</param>  
        /// <param name="password">压缩密码</param>  
        /// <returns>是否压缩成功</returns>  
        private static bool ZipDirectory(string strFile, string strZip, string password, int compressLevel)
        {
            bool result = false;
            if (!Directory.Exists(strFile)) return false;
            if (strFile[strFile.Length - 1] != Path.DirectorySeparatorChar)
                strFile += Path.DirectorySeparatorChar;
            ZipOutputStream s = new ZipOutputStream(File.Create(strZip));
            s.SetLevel(compressLevel); // 0 - store only to 9 - means best compression  
            if (!string.IsNullOrEmpty(password)) s.Password = password;
            try
            {
                result = zip(strFile, s, strFile);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                s.Finish();
                s.Close();
            }
            return result;
        }

        /// <summary>  
        /// 压缩文件夹内部方法  
        /// </summary>  
        /// <param name="strFile"></param>  
        /// <param name="s"></param>  
        /// <param name="staticFile"></param>  
        /// <returns></returns>  
        private static bool zip(string strFile, ZipOutputStream s, string staticFile)
        {
            bool result = true;
            if (strFile[strFile.Length - 1] != Path.DirectorySeparatorChar) strFile += Path.DirectorySeparatorChar;
            Crc32 crc = new Crc32();
            try
            {
                string[] filenames = Directory.GetFileSystemEntries(strFile);
                foreach (string file in filenames)
                {

                    if (Directory.Exists(file))
                    {
                        zip(file, s, staticFile);
                    }

                    else // 否则直接压缩文件  
                    {
                        //打开压缩文件  
                        FileStream fs = File.OpenRead(file);

                        byte[] buffer = new byte[fs.Length];
                        fs.Read(buffer, 0, buffer.Length);
                        string tempfile = file.Substring(staticFile.LastIndexOf("\\") + 1);
                        ZipEntry entry = new ZipEntry(tempfile);

                        entry.DateTime = DateTime.Now;
                        entry.Size = fs.Length;
                        fs.Close();
                        crc.Reset();
                        crc.Update(buffer);
                        entry.Crc = crc.Value;
                        s.PutNextEntry(entry);

                        s.Write(buffer, 0, buffer.Length);
                    }
                }
            }
            catch (Exception ex)
            {
                result = false;
                throw ex;
            }
            return result;

        }
        #endregion

        #endregion


        #region 解压

        #region 公开解压方法
        /// <summary>     
        /// 解压功能(解压压缩文件到指定目录)---->不需要密码  
        /// </summary>     
        /// <param name="fileToUnZip">待解压的文件</param>     
        /// <param name="zipedFolder">指定解压目标目录</param>    
        /// <param name="errorOut">如果失败返回失败信息</param>   
        /// <returns>解压结果</returns>     
        public static bool UPZipFile(string fileToUnZip, string zipedFolder, ref string errorOut)
        {
            bool result = false;
            try
            {
                result = UPZipFileByPassword(fileToUnZip, zipedFolder, null);
            }
            catch (Exception ex)
            {
                errorOut = ex.Message;
            }
            return result;
        }
        /// <summary>     
        /// 解压功能(解压压缩文件到指定目录)---->需要密码  
        /// </summary>     
        /// <param name="fileToUnZip">待解压的文件</param>     
        /// <param name="zipedFolder">指定解压目标目录</param>  
        /// <param name="password">密码</param>   
        /// <param name="errorOut">如果失败返回失败信息</param>   
        /// <returns>解压结果</returns>  
        public static bool UPZipFile(string fileToUnZip, string zipedFolder, string password, ref string errorOut)
        {
            bool result = false;
            try
            {
                result = UPZipFileByPassword(fileToUnZip, zipedFolder, password);
            }
            catch (Exception ex)
            {
                errorOut = ex.Message;
            }

            return result;
        }
        #endregion

        #region 内部处理方法
        /// <summary>  
        /// 解压功能 内部处理方法  
        /// </summary>  
        /// <param name="TargetFile">待解压的文件</param>  
        /// <param name="fileDir">指定解压目标目录</param>  
        /// <param name="password">密码</param>  
        /// <returns>成功返回true</returns>  
        private static bool UPZipFileByPassword(string TargetFile, string fileDir, string password)
        {
            bool rootFile = true;
            try
            {
                //读取压缩文件(zip文件)，准备解压缩  
                ZipInputStream zipStream = new ZipInputStream(File.OpenRead(TargetFile.Trim()));
                ZipEntry theEntry;
                string path = fileDir;

                string rootDir = " ";
                if (!string.IsNullOrEmpty(password)) zipStream.Password = password;
                while ((theEntry = zipStream.GetNextEntry()) != null)
                {
                    rootDir = Path.GetDirectoryName(theEntry.Name);
                    if (rootDir.IndexOf("\\") >= 0)
                    {
                        rootDir = rootDir.Substring(0, rootDir.IndexOf("\\") + 1);
                    }
                    string dir = Path.GetDirectoryName(theEntry.Name);
                    string fileName = Path.GetFileName(theEntry.Name);
                    if (dir != " ")
                    {
                        if (!Directory.Exists(fileDir + "\\" + dir))
                        {
                            path = fileDir + "\\" + dir;
                            Directory.CreateDirectory(path);
                        }
                    }
                    else if (dir == " " && fileName != "")
                    {
                        path = fileDir;
                    }
                    else if (dir != " " && fileName != "")
                    {
                        if (dir.IndexOf("\\") > 0)
                        {
                            path = fileDir + "\\" + dir;
                        }
                    }

                    if (dir == rootDir)
                    {
                        path = fileDir + "\\" + rootDir;
                    }

                    //以下为解压缩zip文件的基本步骤  
                    //基本思路就是遍历压缩文件里的所有文件，创建一个相同的文件。  
                    if (fileName != String.Empty)
                    {
                        FileStream streamWriter = File.Create(path + "\\" + fileName);

                        int size = 2048;
                        byte[] data = new byte[2048];
                        while (true)
                        {
                            size = zipStream.Read(data, 0, data.Length);
                            if (size > 0)
                            {
                                streamWriter.Write(data, 0, size);
                            }
                            else
                            {
                                break;
                            }
                        }

                        streamWriter.Close();
                    }
                }
                if (theEntry != null)
                {
                    theEntry = null;
                }
                if (zipStream != null)
                {
                    zipStream.Close();
                }
            }
            catch (Exception ex)
            {
                rootFile = false;
                throw ex;
            }
            finally
            {
                GC.Collect();
                GC.Collect(1);
            }
            return rootFile;
        }
        #endregion  

        #endregion

























        #region 以下是初版的压缩方法，大文件时会有问题

        /// <summary>  
        /// 所有文件缓存  
        /// </summary>  
        List<string> files = new List<string>();

        /// <summary>  
        /// 所有空目录缓存  
        /// </summary>  
        List<string> paths = new List<string>();

        /// <summary>  
        /// 压缩单个文件  
        /// </summary>  
        /// <param name="fileToZip">要压缩的文件</param>  
        /// <param name="zipedFile">压缩后的文件全名</param>  
        /// <param name="compressionLevel">压缩程度，范围0-9，数值越大，压缩程序越高</param>  
        /// <param name="blockSize">分块大小</param>  
        public void ZipFile(string fileToZip, string zipedFile, int compressionLevel, int blockSize)
        {
            if (!System.IO.File.Exists(fileToZip))//如果文件没有找到，则报错  
            {
                throw new FileNotFoundException("The specified file " + fileToZip + " could not be found. Zipping aborderd");
            }

            FileStream streamToZip = new FileStream(fileToZip, FileMode.Open, FileAccess.Read);
            FileStream zipFile = File.Create(zipedFile);
            ZipOutputStream zipStream = new ZipOutputStream(zipFile);
            ZipEntry zipEntry = new ZipEntry(fileToZip);
            zipStream.PutNextEntry(zipEntry);
            zipStream.SetLevel(compressionLevel);
            byte[] buffer = new byte[blockSize];
            int size = streamToZip.Read(buffer, 0, buffer.Length);
            zipStream.Write(buffer, 0, size);

            try
            {
                while (size < streamToZip.Length)
                {
                    int sizeRead = streamToZip.Read(buffer, 0, buffer.Length);
                    zipStream.Write(buffer, 0, sizeRead);
                    size += sizeRead;
                }
            }
            catch (Exception ex)
            {
                GC.Collect();
                throw ex;
            }

            zipStream.Finish();
            zipStream.Close();
            streamToZip.Close();
            GC.Collect();
        }

        /// <summary>  
        /// 压缩目录（包括子目录及所有文件）  
        /// </summary>  
        /// <param name="rootPath">要压缩的根目录</param>  
        /// <param name="destinationPath">保存路径</param>  
        /// <param name="compressLevel">压缩程度，范围0-9，数值越大，压缩程序越高</param>  
        public void ZipFileFromDirectory(string rootPath, string destinationPath, int compressLevel)
        {
            GetAllDirectories(rootPath);

            /* while (rootPath.LastIndexOf("\\") + 1 == rootPath.Length)//检查路径是否以"\"结尾 
            { 
 
            rootPath = rootPath.Substring(0, rootPath.Length - 1);//如果是则去掉末尾的"\" 
 
            } 
            */
            //string rootMark = rootPath.Substring(0, rootPath.LastIndexOf("\\") + 1);//得到当前路径的位置，以备压缩时将所压缩内容转变成相对路径。  
            string rootMark = rootPath + "\\";//得到当前路径的位置，以备压缩时将所压缩内容转变成相对路径。  
            Crc32 crc = new Crc32();
            ZipOutputStream outPutStream = new ZipOutputStream(File.Create(destinationPath));
            outPutStream.SetLevel(compressLevel); // 0 - store only to 9 - means best compression  
            foreach (string file in files)
            {
                FileStream fileStream = File.OpenRead(file);//打开压缩文件  
                byte[] buffer = new byte[fileStream.Length];
                fileStream.Read(buffer, 0, buffer.Length);
                ZipEntry entry = new ZipEntry(file.Replace(rootMark, string.Empty));
                entry.DateTime = DateTime.Now;

                entry.Size = fileStream.Length;
                fileStream.Close();
                crc.Reset();
                crc.Update(buffer);
                entry.Crc = crc.Value;
                outPutStream.PutNextEntry(entry);
                outPutStream.Write(buffer, 0, buffer.Length);
            }

            this.files.Clear();

            foreach (string emptyPath in paths)
            {
                ZipEntry entry = new ZipEntry(emptyPath.Replace(rootMark, string.Empty) + "/");
                outPutStream.PutNextEntry(entry);
            }

            this.paths.Clear();
            outPutStream.Finish();
            outPutStream.Close();
            GC.Collect();
        }

        /// <summary>  
        /// 取得目录下所有文件及文件夹，分别存入files及paths  
        /// </summary>  
        /// <param name="rootPath">根目录</param>  
        private void GetAllDirectories(string rootPath)
        {
            string[] subPaths = Directory.GetDirectories(rootPath);//得到所有子目录  
            foreach (string path in subPaths)
            {
                GetAllDirectories(path);//对每一个字目录做与根目录相同的操作：即找到子目录并将当前目录的文件名存入List  
            }
            string[] files = Directory.GetFiles(rootPath);
            foreach (string file in files)
            {
                this.files.Add(file);//将当前目录中的所有文件全名存入文件List  
            }
            if (subPaths.Length == files.Length && files.Length == 0)//如果是空目录  
            {
                this.paths.Add(rootPath);//记录空目录  
            }
        }

        /// <summary>  
        /// 解压缩文件(压缩文件中含有子目录)  
        /// </summary>  
        /// <param name="zipfilepath">待解压缩的文件路径</param>  
        /// <param name="unzippath">解压缩到指定目录</param>  
        /// <returns>解压后的文件列表</returns>  
        public List<string> UnZip(string zipfilepath, string unzippath)
        {
            //解压出来的文件列表  
            List<string> unzipFiles = new List<string>();

            //检查输出目录是否以“\\”结尾  
            if (unzippath.EndsWith("\\") == false || unzippath.EndsWith(":\\") == false)
            {
                unzippath += "\\";
            }

            ZipInputStream s = new ZipInputStream(File.OpenRead(zipfilepath));
            ZipEntry theEntry;
            while ((theEntry = s.GetNextEntry()) != null)
            {
                string directoryName = Path.GetDirectoryName(unzippath);
                string fileName = Path.GetFileName(theEntry.Name);

                //生成解压目录【用户解压到硬盘根目录时，不需要创建】  
                if (!string.IsNullOrEmpty(directoryName))
                {
                    Directory.CreateDirectory(directoryName);
                }

                if (fileName != String.Empty)
                {
                    //如果文件的压缩后大小为0那么说明这个文件是空的,因此不需要进行读出写入  
                    if (theEntry.CompressedSize == 0)
                        continue;
                    //解压文件到指定的目录  
                    directoryName = Path.GetDirectoryName(unzippath + theEntry.Name);
                    //建立下面的目录和子目录  
                    Directory.CreateDirectory(directoryName);

                    //记录导出的文件  
                    unzipFiles.Add(unzippath + theEntry.Name);

                    FileStream streamWriter = File.Create(unzippath + theEntry.Name);

                    int size = 2048;
                    byte[] data = new byte[2048];
                    while (true)
                    {
                        size = s.Read(data, 0, data.Length);
                        if (size > 0)
                        {
                            streamWriter.Write(data, 0, size);
                        }
                        else
                        {
                            break;
                        }
                    }
                    streamWriter.Close();
                }
            }
            s.Close();
            GC.Collect();
            return unzipFiles;
        }

        public string GetZipFileExtention(string fileFullName)
        {
            int index = fileFullName.LastIndexOf(".");
            if (index <= 0)
            {
                throw new Exception("The source package file is not a compress file");
            }

            //extension string
            string ext = fileFullName.Substring(index);

            if (ext == ".rar" || ext == ".zip")
            {
                return ext;
            }
            else
            {
                throw new Exception("The source package file is not a compress file");
            }
        }
        #endregion
    }
}
