using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
//using System.Text.RegularExpressions;
using System.Net;
using System.Security.Cryptography;
using System.Collections;
//using System.IO.Compression;
using System.Configuration;
using ICSharpCode.SharpZipLib;
using ICSharpCode.SharpZipLib.Zip;
using ICSharpCode.SharpZipLib.Checksums;
using System.Xml;
using System.Reflection;

using SharpSvn.Security;
using SharpSvn;
//using System.Collections.ObjectModel;


namespace test10

{

    public partial class Form1 : Form
    {

        public string path = "ftp://10.12.12.9:21";//FTP的服务器地址，格式为ftp://192.168.1.234:8021/。ip地址和端口换成自己的，写在配置文件中，方便修改
        private static string FTPUSERNAME = "anonymous";//FTP服务器的用户名
        private static string FTPPASSWORD = "123";//FTP服务器的密码
        public string LPath = @"D:\";
        string[] SFiles = new string[15];
        private string SelectDir = "";//选择的文件夹的名字
        private string NewFileCrePath = "";
        private delegate void updateui(long rowCount, int i, ProgressBar PB);//一个委托类
        public int CountFile = 0;
        public string SkinMode = "MacOS";
        public string SvnUrl;
        //  Form2 ConForm2 = new Form2();这里会死循环，互相new
        //   System.Configuration.Configuration config = null;
        public Form1()
        {
            InitializeComponent();




        }

        //===================================================
        /// <summary>
        /// 从ftp服务器下载文件的功能----带进度条
        /// </summary>
        /// <param name="ftpfilepath">ftp下载的地址</param>
        /// <param name="filePath">保存本地的地址</param>
        /// <param name="ftpfilename">要下载的文件名</param>  
        /// <param name="pb">进度条引用</param>
        /// <returns></returns>
        public bool Download1(string ftpfilepath, string filePath, string ftpfilename, ProgressBar pb)
        {

            FtpWebRequest reqFtp = null;
            FtpWebResponse response = null;
            Stream ftpStream = null;
            FileStream outputStream = null;
            try
            {
                filePath = filePath.Replace("我的电脑\\", "");//无用 
                ftpfilepath = ftpfilepath.Replace("\\", "/");//无用
                string newFileName = filePath + ftpfilename;//给原名字
                if (File.Exists(newFileName))
                {

                    MessageBox.Show("要下载的文件已存在!", "提示");
                    return false;
                    // File.Delete(newFileName);//如果已有则删除已有

                }
                string url = path + "/" + ftpfilepath + "/" + ftpfilename;//("ftp://10.12.12.9/iTap3.x/iTapAutoSettle3.x_1240.zip
                //URL是URI的子集,统一资源标志符URI就是在某一规则下能把一个资源独一无二地标识出来。统一资源定位符URL就是用定位的方式实现的URI。
                //URI可被视为定位符（URL），名称（URN）或两者兼备。统一资源名（URN）如同一个人的名称，而统一资源定位符（URL）代表一个人的住址。
                //换言之，URN定义某事物的身份，而URL提供查找该事物的方法。
                reqFtp = (FtpWebRequest)FtpWebRequest.Create(new Uri(url));//创造连接，创建一个FtpWebRequest对象，指向ftp服务器的uri
                reqFtp.UseBinary = true;//二进制传输，给FtpWebRequest对象设置属性
                reqFtp.Method = WebRequestMethods.Ftp.DownloadFile;// 设置ftp的执行方法（上传，下载等）

                reqFtp.Credentials = new NetworkCredential(FTPUSERNAME, FTPPASSWORD);//账户密码
                response = (FtpWebResponse)reqFtp.GetResponse();//获得文件，执行请求
                ftpStream = response.GetResponseStream();//获得流，接收相应流
                long cl = GetFileSize(url);//获得文件大小cl
                int bufferSize = 2048;//时间延迟
                int readCount;
                byte[] buffer = new byte[bufferSize];
                readCount = ftpStream.Read(buffer, 0, bufferSize);//从流里读取的字节数为0时，则下载结束。
                outputStream = new FileStream(newFileName, FileMode.Create);//输出流

                float percent = 0;
                while (readCount > 0)
                {
                    outputStream.Write(buffer, 0, readCount);//写入
                    readCount = ftpStream.Read(buffer, 0, bufferSize);//写入
                    percent = (float)outputStream.Length / (float)cl * 100;//百分之多少了
                    if (percent <= 100)//进度条显示的百分比
                    {
                        if (pb != null)
                        {
                            pb.Invoke(new updateui(upui), new object[] { cl, (int)percent, pb });//进度条

                            // 首先代码定义了一个委托updateui(long rowCount, int i, ProgressBar PB),委托实例化
                            //委托与其带的参数类型数量相同。首先使用新的委托类型声明一个变量,并且初始化委托变量.注意,声明时的参数只要使用委托传递的函数的函数名,而不加括号
                            //upui：PB.Value = i;
                        }
                    }

                }

                return true;
            }
            catch (Exception ex)//捕捉
            {
                throw ex;
            }
            finally//异常清理，无论是否异常，都会执行
            {
                if (reqFtp != null)
                {
                    reqFtp.Abort();
                }
                if (response != null)
                {
                    response.Close();
                }
                if (ftpStream != null)
                {
                    ftpStream.Close();
                }
                if (outputStream != null)
                {
                    outputStream.Close();
                }
            }
        }//下载文件
        public static void upui(long rowCount, int i, ProgressBar PB)//进度条
        {
            try
            {
                PB.Value = i;
            }
            catch { }
        }
        //===================================================
        #region 获得文件的大小
        /// <summary>
        /// 获得文件大小
        /// </summary>
        /// <param name="url">FTP文件的完全路径</param>
        /// <returns></returns>
        public static long GetFileSize(string url)//获取大小就要重新连接吗？是的，method只能有一个
        {

            long fileSize = 0;
            try
            {
                FtpWebRequest reqFtp = (FtpWebRequest)FtpWebRequest.Create(new Uri(url));//路径创造新的连接
                reqFtp.UseBinary = true;
                reqFtp.Credentials = new NetworkCredential(FTPUSERNAME, FTPPASSWORD);//连接
                reqFtp.Method = WebRequestMethods.Ftp.GetFileSize;//方法大小
                FtpWebResponse response = (FtpWebResponse)reqFtp.GetResponse();
                fileSize = response.ContentLength;//获得大小

                response.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return fileSize;
        }
        #endregion
        //===================================================上传文件
        /// <summary>
        /// 文件上传
        /// </summary>
        /// <param name="filePath">原路径（绝对路径）包括文件名</param>
        /// <param name="objPath">目标文件夹：服务器下的相对路径 不填为根目录</param>
        public void FileUpLoad(string filePath, string objPath = "")
        {
            try
            {
                string url = path;
                if (objPath != "")
                    url += objPath + "/";
                try
                {

                    FtpWebRequest reqFTP = null;
                    //待上传的文件 （全路径）
                    try
                    {
                        FileInfo fileInfo = new FileInfo(filePath);
                        using (FileStream fs = fileInfo.OpenRead())
                        {
                            long length = fs.Length;
                            reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri(url + fileInfo.Name));

                            //设置连接到FTP的帐号密码
                            reqFTP.Credentials = new NetworkCredential(FTPUSERNAME, FTPPASSWORD);
                            //设置请求完成后是否保持连接
                            reqFTP.KeepAlive = false;
                            //指定执行命令
                            reqFTP.Method = WebRequestMethods.Ftp.UploadFile;
                            //指定数据传输类型
                            reqFTP.UseBinary = true;

                            using (Stream stream = reqFTP.GetRequestStream())
                            {
                                //设置缓冲大小
                                int BufferLength = 5120;
                                byte[] b = new byte[BufferLength];
                                int i;
                                while ((i = fs.Read(b, 0, BufferLength)) > 0)
                                {
                                    stream.Write(b, 0, i);
                                }
                                MessageBox.Show("上传文件成功", "提示");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    finally
                    {

                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //==============================================================刷新列表时候用
        /// <summary>
        /// 获取当前目录下文件夹
        /// </summary>
        /// <returns></returns>
        public string[] GetFilesDirList()
        {

            // string[] downloadFiles;
            try
            {
                StringBuilder result = new StringBuilder();//如果要修改字符串而不创建新的对象，则可以使用 System.Text.StringBuilder 类。
                FtpWebRequest ftp;
                ftp = (FtpWebRequest)FtpWebRequest.Create(new Uri(path));//"ftp://10.12.12.9";
                ftp.Credentials = new NetworkCredential(FTPUSERNAME, FTPPASSWORD);
                ftp.Method = WebRequestMethods.Ftp.ListDirectoryDetails;//目录
                WebResponse response = ftp.GetResponse();//response为一个ftp的WebResponse
                StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.Default);//读入responses所创建的数据流
                string line = reader.ReadLine();//输入流中的下一行；如果到达了输入流的末尾，则为空引用
                while (line != null)
                {
                    result.Append(line);//)Append 方法可用来将文本或对象的字符串表示形式添加到由当前 StringBuilder 对象表示的字符串的结尾处。
                    result.Append("\n");
                    line = reader.ReadLine();
                    textBox1.Text = line;
                }
                result.Remove(result.ToString().LastIndexOf("\n"), 1);//移除最后的换行之后
                reader.Close();
                response.Close();
                return result.ToString().Split('\n');
            }
            catch (Exception ex)
            {
                MessageBox.Show("连接失败！", "提示");
                throw ex;
            }
        }
        //===================================================下载时候用
        /// <summary>
        /// 获取选中文件夹下的文件
        /// </summary>
        /// <returns></returns>
        public string[] GetFilesList()//
        {


            string url = path + "/" + SelectDir;//============和之前的文件夹唯一的不同就在于此，加上了文件夹的名字..
            try
            {
                StringBuilder result = new StringBuilder();//如果要修改字符串而不创建新的对象，则可以使用 System.Text.StringBuilder 类。
                FtpWebRequest ftp;
                ftp = (FtpWebRequest)WebRequest.Create(new Uri(url));
                ftp.Credentials = new NetworkCredential(FTPUSERNAME, FTPPASSWORD);
                ftp.Method = WebRequestMethods.Ftp.ListDirectoryDetails;//目录
                WebResponse response = ftp.GetResponse();
                StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.Default);//读入responses所创建的数据流
                string line = reader.ReadLine();//输入流中的下一行；如果到达了输入流的末尾，则为空引用
                while (line != null)
                {
                    result.Append(line);//)Append 方法可用来将文本或对象的字符串表示形式添加到由当前 StringBuilder 对象表示的字符串的结尾处。
                    result.Append("\n");
                    line = reader.ReadLine();
                    textBox1.Text = line;
                }
                result.Remove(result.ToString().LastIndexOf("\n"), 1);//移除最后的换行
                reader.Close();
                response.Close();
                return result.ToString().Split('\n');
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //===================================================//显示列表、刷新

        private void button2_Click(object sender, EventArgs e)
        {
            listBox1.Items.Clear();
            try
            {
                string[] AllDirs = GetFilesDirList();//连接获取文件夹
                for (int i = 0; i < AllDirs.Length; i++)//将所有的数组（文件夹名字、信息）给到列表里
                {
                    listBox1.Items.Add(AllDirs[i]);//全部给到显示中

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }


        }
        //===================================================//载入
        public void form1load(object sender, EventArgs e)
        {
            //    button2_Click(null, null);
            // ConfigurationManager.RefreshSection("appSettings");

            path = GetValueByKey("FtpPath");
            LPath = GetValueByKey("LocalPath");
            FTPUSERNAME = GetValueByKey("FtpUser");
            FTPPASSWORD = GetValueByKey("FtpPassWord");//读取配置文件
            SvnUrl = GetValueByKey("SvnUrl");
            if (!LPath.EndsWith("\\"))
            {
                LPath = LPath + "\\";//非常关键的一步
            }
            SkinMode = GetValueByKey("Skin");
            skinEngine1 = new Sunisoft.IrisSkin.SkinEngine(this);
            skinEngine1.SkinFile = Application.StartupPath + "//Skins//" + SkinMode + ".ssk";
            Sunisoft.IrisSkin.SkinEngine se = null;
            se = new Sunisoft.IrisSkin.SkinEngine();
            se.SkinAllForm = true;

            //path = ConForm2.textBox6.Text;
            //LPath = textBox4.Text;
            // LPath = LPath.Replace("\\", "\\\\");
        }
        //===================================================选中列表后给文件夹名字


        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            String StrTemp = listBox1.SelectedItem.ToString();

            StrTemp = StrTemp.Substring(39);//文件前面有39个奇怪的字符...无论文件与文件夹
            SelectDir = StrTemp;//名字给SelectDir
            textBox1.Text = StrTemp;//吧文件夹名字显示

        }
        //===================================================排序后下载选中
        // Dictionary
        /// <summary>
        /// 排序后下载选中
        /// </summary>
        private void SelectNew_Click(object sender, EventArgs e)
        {
            string[] AllFiles = GetFilesList();//获得全部文件名
            int Len = AllFiles.Length;
            string newstr = "";
            string name = "1";
            string[] name1 = new string[Len];
            string[] sernum1 = new string[Len];
            int i = 0, coo = 0;
            int i1 = 0, i2 = 0, Fir = 0, j = 0, k = 0, Total = 0, max = 0;
            int[] bnum;
            int[] bcomp = new int[Len];
            CountFile = 0;
            //   comp = AllFiles.OrderBy(s => int.Parse(Regex.Match(s, @"\d+").Value)).ToArray();
            // MessageBox.Show(LPath );
            if (SelectDir != "")
            {
                for (i = 0; i < Len; i++)
                {
                    AllFiles[i] = AllFiles[i].Substring(39);//AllFiles具有完整的名称
                    newstr = AllFiles[i];
                    newstr = newstr.Substring(0, newstr.Length - 4); //newstr去除.ZIP
                    coo = 1 + newstr.LastIndexOf("_");//检索最后一个"_"的位置
                    name1[i] = newstr.Substring(0, coo); //MessageBox.Show(a[i]);//名字给A
                    sernum1[i] = newstr.Substring(coo);// MessageBox.Show(b[i]);//数字给B  
                }
                bnum = Array.ConvertAll(sernum1, int.Parse);//iNums = Array.ConvertAll<string, int>(sNums , s => int.Parse(s)); iNums = Array.ConvertAll<string, int>(sNums, int.Parse);   
                for (i1 = 0; i1 < (Len - 1); i1++)//开始选出最高版本文件
                {
                    name = name1[i1];
                    if (name1[i1] == name1[i1 + 1])
                    {
                        Total++;//数目加一
                    }

                    if ((!(name1[i1] == name1[i1 + 1])) || (i1 == (Len - 2)))//当有文件名字变更或者是到达文件末尾的时候，
                    {
                        Array.Clear(bcomp, 0, bcomp.Length);//清空这个bcomp数组
                        for (k = Fir, j = 0; j < (Total + 1); k++, j++)//将这一组数据给这个获取最大数的的数组赋值，Total在这里关键，加一与否决定最后一个是否算上
                        {
                            bcomp[k] = bnum[k];
                        }
                        max = bcomp.Max();//选出最大的那个数字
                        name = name + max + ".zip";
                        Download1(SelectDir, LPath, name, pb1);//直接将名字和数字拼接在一起，下载
                        operation(name, 1);
                        Total = 0;
                        Fir = i1 + 1;
                    }
                    i2 = i1 + 1; //MessageBox.Show(i2.ToString());
                    if ((i2 == (Len - 1)) && (name1[i2] != name1[i2 - 1]))//多判断一次，最后两个不同
                    {
                        name = name1[i2] + bnum[i2] + ".zip";

                        Download1(SelectDir, LPath, name, pb1);
                        operation(name, 1);
                    }
                }               
            }
        }

        //===================================================压缩用的是库函数
        public static void ZipDirectory(string folderToZip, string zipedFileName)//重载
        {
            ZipDirectory(folderToZip, zipedFileName, string.Empty, true, string.Empty, string.Empty, true);
        }


        /// <summary>
        /// 压缩文件夹
        /// </summary>
        /// <param name="folderToZip">需要压缩的文件夹</param>
        /// <param name="zipedFileName">压缩后的Zip完整文件名（如D:\test.zip）</param>
        /// <param name="isRecurse">如果文件夹下有子文件夹，是否递归压缩</param>
        /// <param name="password">解压时需要提供的密码</param>
        /// <param name="fileRegexFilter">文件过滤正则表达式</param>
        /// <param name="directoryRegexFilter">文件夹过滤正则表达式</param>
        /// <param name="isCreateEmptyDirectories">是否压缩文件中的空文件夹</param>

        public static void ZipDirectory(string folderToZip, string zipedFileName, string password, bool isRecurse, string fileRegexFilter, string directoryRegexFilter, bool isCreateEmptyDirectories)
        {
            FastZip fastZip = new FastZip();
            fastZip.CreateEmptyDirectories = isCreateEmptyDirectories;
            fastZip.Password = password;
            fastZip.CreateZip(zipedFileName, folderToZip, isRecurse, fileRegexFilter, directoryRegexFilter);
        }


        //===================================================解压用的是库函数
        /// <summary>  
        /// 功能：解压zip格式的文件。  
        /// </summary>  
        /// <param name="zipFilePath">压缩文件路径</param>  
        /// <param name="unZipDir">解压文件存放路径,为空时默认与压缩文件同一级目录下，跟压缩文件同名的文件夹</param>  
        /// <returns>解压是否成功</returns>  
        public void UnZip(string zipFilePath, string unZipDir)
        {
            if (zipFilePath == string.Empty)
            {
                throw new Exception("压缩文件不能为空！");
            }
            if (!File.Exists(zipFilePath))
            {
                throw new FileNotFoundException("压缩文件不存在！");
            }
            //解压文件夹为空时默认与压缩文件同一级目录下，跟压缩文件同名的文件夹  
            if (unZipDir == string.Empty)
                unZipDir = zipFilePath.Replace(Path.GetFileName(zipFilePath), Path.GetFileNameWithoutExtension(zipFilePath));
            if (!unZipDir.EndsWith("/"))
                unZipDir += "/";
            if (!Directory.Exists(unZipDir))
                Directory.CreateDirectory(unZipDir);

            using (var s = new ZipInputStream(File.OpenRead(zipFilePath)))
            {

                ZipEntry theEntry;
                while ((theEntry = s.GetNextEntry()) != null)
                {
                    string directoryName = Path.GetDirectoryName(theEntry.Name);
                    string fileName = Path.GetFileName(theEntry.Name);
                    if (!string.IsNullOrEmpty(directoryName))
                    {
                        Directory.CreateDirectory(unZipDir + directoryName);
                    }
                    if (directoryName != null && !directoryName.EndsWith("/"))
                    {
                    }
                    if (fileName != String.Empty)
                    {
                        using (FileStream streamWriter = File.Create(unZipDir + theEntry.Name))
                        {

                            int size;
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
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 操作进行解压、删除、（移动、提取、生成）的函数
        /// </summary>
        /// <param name="name1">文件名</param>
        /// <param name="flag">1++，0=0（如D:\test.zip）</param>
        public void operation(string name1, int flag)
        {

            if (flag == 1)
            {
                SFiles[CountFile] = name1;
                UnZip(LPath + SFiles[CountFile], "");
                File.Delete(LPath + SFiles[CountFile]);
                CountFile++;
            }
            if (flag == 0)
            {
                CountFile = 0;
            }


        }
        //===================================================文件创建、移动、删除、生成、压缩
        private void button5_Click(object sender, EventArgs e)
        {
            string VerPath = "";
            string FromVerPath = "";
            string SFPath = "";
            string STemp = "", ToPath = "";
            NewFileCrePath = Path.Combine(LPath, SelectDir);
            int FileMoveCount = CountFile;
            if (SelectDir != "")
            {
                if (!Directory.Exists(NewFileCrePath))
                {
                    Directory.CreateDirectory(NewFileCrePath);//创造一个新文件夹
                                                              //MessageBox.Show(sf.ToString());
                    VerPath = NewFileCrePath + @"\Version.txt";
                    FileStream fcreate = File.Create(VerPath);//创造一个版本记录文件
                    fcreate.Close();
                    for (int sfs = 0; sfs < FileMoveCount; sfs++)//sf是个数，从1开始，sfs是循环计数，从0开始
                    {
                        STemp = SFiles[sfs];//保持SFiles数组的稳定-.zip
                        STemp = STemp.Substring(0, STemp.Length - 4);//去掉文件名称后缀
                        SFPath = LPath + STemp + @"\" + STemp;//将要移动的文件夹   
                        ToPath = NewFileCrePath + @"\" + STemp;//目标文件夹
                        if (Directory.Exists(SFPath))
                        {
                            try
                            {
                                //       MessageBox.Show(SFPath);
                                //    MessageBox.Show(ToPath);

                                Directory.Move(SFPath, ToPath);//ToPath要设置为移动到这里的那个文件夹的名字（或者重命名
                            }
                            catch (Exception ee)
                            { throw ee; }


                            SFPath = LPath + STemp;
                            Directory.Delete(SFPath, true);
                        }
                        else
                        {
                            MessageBox.Show("要移动的文件不存在！", "提示");
                            return;
                        }
                        //=============================版本生成
                        FromVerPath = NewFileCrePath + @"\" + STemp + @"\Version.txt";

                        FileStream fsver = new FileStream(FromVerPath, FileMode.Open);//吧每一个文件的版本提取
                        StreamReader reader = new StreamReader(fsver, UnicodeEncoding.GetEncoding("GB2312"));//中文、读取                                                                            // String[] str = new String[5];
                        string str = string.Empty;
                        while (string.IsNullOrEmpty(str))
                        {
                            str = reader.ReadLine();//如果是空的话，那就再读一行吧！
                        }
                        StreamWriter sw = new StreamWriter(VerPath, true, UnicodeEncoding.GetEncoding("GB2312"));//写入
                        str = STemp + "\t" + "\t" + str;
                        sw.WriteLine(str);//在后面写入
                        sw.Close();
                        fsver.Close();

                    }//for循环结束

                    FileMoveCount = 0;//下次点击重新计数文件数目
                    string newzip = NewFileCrePath + ".zip";
                    ZipDirectory(NewFileCrePath, newzip);//压缩一下
                    Directory.Delete(NewFileCrePath, true);
                    MessageBox.Show("生成成功！", "提示");
                    // FileUpLoad(newzip,"");上传一下
                }
                else
                {
                    MessageBox.Show("要创建的文件夹已经存在！", "提示");
                }
            }
            SelectDir = "";//清零

        }
        //===================================================//上传
        private void button6_Click(object sender, EventArgs e)
        {
            //  FileUpLoad(NewFileCrePath,"");


        }
        //===================================================//浏览路径
        private void button4_Click(object sender, EventArgs e)
        {

            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.Description = "请选择文件路径";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                string foldPath = dialog.SelectedPath;
                textBox4.Text = foldPath;
                button3_Click(null, null);
            }

        }


        //===================================================一键完成
        private void button1_Click(object sender, EventArgs e)
        {
            SelectNew_Click(null, null);
            button5_Click(null, null);
            //button6_Click(null, null);
            SvnButton_Click(null, null);
        }

        private void button7_Click(object sender, EventArgs e)//***
        {



        }
        //===================================================设为默认，修改config***

        private void button3_Click(object sender, EventArgs e)
        {

            ModifyAppSettings("FtpPath", textBox2.Text);
            ModifyAppSettings("LocalPath", textBox4.Text);
            path = textBox2.Text;
            LPath = textBox4.Text;
            LPath = LPath.Replace("\\", "\\\\");

        }
        //===================================================读写配置文件
        public string GetValueByKey(string key)//获得
        {
            ConfigurationManager.RefreshSection("appSettings");//强制刷新
            return ConfigurationManager.AppSettings[key];//获得设置
        }
        public void ModifyAppSettings(string strKey, string value)//修改
        {
            //获得配置文件的全路径    
            var assemblyConfigFile = Assembly.GetEntryAssembly().Location;//获取入口装配
            var appDomainConfigFile = AppDomain.CurrentDomain.SetupInformation.ConfigurationFile;//获得配置文件的目录地址
            ChangeConfiguration(strKey, value, assemblyConfigFile);
            ModifyAppConfig(strKey, value, appDomainConfigFile);
        }

        public void ModifyAppConfig(string strKey, string value, string configPath)//实现修改
        {
            var doc = new XmlDocument();
            doc.Load(configPath);

            //找出名称为“add”的所有元素    
            var nodes = doc.GetElementsByTagName("add");
            for (int i = 0; i < nodes.Count; i++)
            {
                //获得将当前元素的key属性    
                var xmlAttributeCollection = nodes[i].Attributes;
                if (xmlAttributeCollection != null)
                {
                    var att = xmlAttributeCollection["key"];
                    if (att == null) continue;
                    //根据元素的第一个属性来判断当前的元素是不是目标元素    
                    if (att.Value != strKey) continue;
                    //对目标元素中的第二个属性赋值    
                    att = xmlAttributeCollection["value"];//查找旧的修改新的
                    att.Value = value;
                }
                break;
            }

            //保存上面的修改    
            doc.Save(configPath);
            ConfigurationManager.RefreshSection("appSettings");//强制刷新
        }

        public void ChangeConfiguration(string key, string value, string path)//改变key之后添加新的***
        {
            var config = ConfigurationManager.OpenExeConfiguration(path);
            config.AppSettings.Settings.Remove(key);
            config.AppSettings.Settings.Add(key, value);//删去旧的来设置新的
            config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("appSettings");
        }
        //===================================================
        private void button8_Click(object sender, EventArgs e)//前往配置
        {
            Form2 f2 = new Form2();
            Hide();
            f2.m_ff = this;
            f2.Show();
        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {

        }
        //===================================================SVN下载
        private void SvnButton_Click(object sender, EventArgs e)
        {

            //string SvnUrl = GetValueByKey("SvnUrl");
            string SvnUserName = GetValueByKey("SvnUserName");
            string SvnPass = GetValueByKey("SvnPass");
            SvnUrl = GetValueByKey("SvnUrl");
            string Lp = LPath + "SVN";
            SvnClient client = new SvnClient();//创建一个服务器端
            SvnUriTarget rem = new SvnUriTarget(SvnUrl);//连接这个svn目标
            MessageBox.Show(Lp, "提示");
            //  client.Authentication.ClearAuthenticationCache();
            if (Directory.Exists(Lp))
            {

                MessageBox.Show("该目录下已有名为SVN的文件夹", "提示");

                // File.Delete(newFileName);//如果已有则删除已有

            }
            else
            {
                client.Authentication.Clear();//清除原有的账户信息
                                              //重新设定账户信息

                // SharpSvn.UI.SvnUIBindArgs uiBindArgs = new SharpSvn.UI.SvnUIBindArgs();
                //SharpSvn.UI.SvnUI.Bind(client, uiBindArgs);//自动登录,授权的UI界面
                //
                client.Authentication.UserNamePasswordHandlers += new EventHandler<SvnUserNamePasswordEventArgs>(delegate (object s, SvnUserNamePasswordEventArgs ee)
                {
                    ee.UserName = SvnUserName;
                    ee.Password = SvnPass;
                });//账号密码

                //    EventArgs是包含事件数据的类的基类,用于传递事件的细节。 
                //EventHandler是一个委托声明如下（其在.Net类库中如下声明的） 
                //public delegate void EventHandler(object sender, EventArgs e) 所以，所有形如:  
                //void 函娄名(object 参数名, EventArgs 参数名);
                //    的函数都可以作为Control类的Click事件响应方法了。如下面所定义的一个事件响应方法： 
                //private void button1_Click(object sender, System.EventArgs e)
                //参数object sender表示引发事件的对象，（其实这里传递的是对象的引用，如果是button1的click事件则sender就是button1）System.EventArgs e 代表事件的相应信息，如鼠标的x,y值等。 
                //下面我们研究一下Button类看看其中的事件声明，以Click事件为例。 
                //public event EventHandler Click;
                //    这里定义了一个EventHandler类型的事件Click
                //private void button1_Click(object sender, System.EventArgs e)
                //    { 
                //                   ... 
                //             }
                //    这是我们和button1_click事件所对应的方法。注意方法的参数符合委托中的签名（既参数列表）。那我们怎么把这个方法和事件联系起来呢，请看下面的代码。 
                //this.button1.Click += new System.EventHandler(this.button1_Click); （其实button1.Click 为System.EventHandler委派的实例事件。与委派中委派实例委托给某一方法非常相似） 
                //把this.button1_Click方法绑定到this.button1.Click事件。 
                try
                {
                    client.CheckOut(rem, Lp);//全部检出
                    MessageBox.Show("SVN下载完成", "提示");
                }
                catch// (Exception ee)
                {

                    //  MessageBox.Show(ee.ToString ());
                    MessageBox.Show("请检测账号密码是否正确", "提示");
                    //throw ee;
                }
            }

        }
        //===================================================//退出
        private void button9_Click(object sender, EventArgs e)
        {
            Dispose();
            Application.Exit();
        }
        //===================================================打开浏览器
        private void button10_Click(object sender, EventArgs e)
        {
            // RegistryKey key = Registry.ClassesRoot.OpenSubKey(@"http\shell\open\command\");
            // string s = key.GetValue("").ToString();

            //s就是默认浏览器，不过后面带了参数，把它截去，不过需要注意的是：不同的浏览器后面的参数不一样！
            //"D:\Program Files (x86)\Google\Chrome\Application\chrome.exe" -- "%1"
            //   System.Diagnostics.Process.Start(s.Substring(0, s.Length - 8), "http://blog.csdn.net/testcs_dn");
            SvnUrl = GetValueByKey("SvnUrl");
            System.Diagnostics.Process.Start(SvnUrl);
        }
    }
}
