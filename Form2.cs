using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using SharpSvn.Security;
using SharpSvn;
using System.Collections.ObjectModel;


namespace test10
{
    public partial class Form2 : Form//Partial是局部类型的意思。允许我们将一个类、结构或接口分成几个部分，分别实现在几个不同的.cs文件中。
        //C#编译器在编译的时候仍会将各个部分的局部类型合并成一个完整的类
    {

        public Form1 m_ff=new Form1 ();

        public  string SvnLocalPath = "D:\\Test1";
        private static  string SvnUrl = "http://10.12.12.9/svn/iTap/trunk/iTap7.2";//"https://desktop-5uj29i7/svn/ATest/321123";// //// ////"http://10.12.12.9/!/#iTap/view/head/trunk/iTap7.1/%E8%BD%AF%E4%BB%B6%E8%AE%BE%E8%AE%A1/EsProtocol";
        public  string SvnUserName = "zhaohanqing";
        public  string SvnPass = "Es123456";

       
        public Form2()
        {
            InitializeComponent();
            skinEngine1 = new Sunisoft.IrisSkin.SkinEngine(this);
            skinEngine1.SkinFile = Application.StartupPath + "//Skins//MacOs.ssk";

        }

        private void Form2_Load(object sender, EventArgs e)
        {


            textBox1 .Text  = m_ff.GetValueByKey("SvnUrl");
            textBox2.Text = m_ff.GetValueByKey("LocalPath");
            textBox3.Text = m_ff.GetValueByKey("SvnUserName");
            textBox4.Text = m_ff.GetValueByKey("SvnPass");
            textBox5.Text = m_ff.GetValueByKey("FtpUpPath");
            textBox6.Text = m_ff.GetValueByKey("FtpPath");
         


        }

        private void Back3_Click(object sender, EventArgs e)
        {

            this.Dispose();
            this.Close();
            m_ff.Show();

        }

        private void Form2_FormClosing(object sender, FormClosingEventArgs e)
        {
            // System.Environment.Exit(0);
            //  Environment.Exit(Environment.ExitCode);
            m_ff.Show();
        }

        private void button4_Click(object sender, EventArgs e)
        {


            Form3 form3 = new Form3();
           // form3.mo
            //  webBrowser1.Navigate(textBox1.Text.Trim());
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SvnUserName = m_ff.GetValueByKey("SvnUserName");
            SvnPass = m_ff.GetValueByKey("SvnPass");
            SvnClient client = new SvnClient();//创建一个服务器端
            SvnUriTarget rem = new SvnUriTarget(SvnUrl);//连接这个svn目标

            client.Authentication.ClearAuthenticationCache();
            client.Authentication.Clear();//清除原有的账户信息
                                          //重新设定账户信息

            // SharpSvn.UI.SvnUIBindArgs uiBindArgs = new SharpSvn.UI.SvnUIBindArgs();
            //SharpSvn.UI.SvnUI.Bind(client, uiBindArgs);//自动登录,授权的UI界面

            client.Authentication.UserNamePasswordHandlers += new EventHandler<SvnUserNamePasswordEventArgs>(delegate (object s, SvnUserNamePasswordEventArgs ee)
            {
                ee.UserName = SvnUserName;
                ee.Password =  SvnPass ;
            });//账号密码

         
           client.CheckOut(rem, SvnLocalPath);//全部检出
        }

        private void button2_Click(object sender, EventArgs e)//配置确认，全部写入到配置文件里面
        {

           // string skinmod = "MacOS";
            m_ff.ModifyAppSettings("SvnUrl", textBox1.Text);
            m_ff.ModifyAppSettings("LocalPath", textBox2.Text);
            m_ff.ModifyAppSettings("SvnUserName", textBox3.Text);
            m_ff.ModifyAppSettings("SvnPass", textBox4.Text);
            m_ff.ModifyAppSettings("FtpPath", textBox6.Text);

            m_ff.path = m_ff.GetValueByKey("FtpPath");
            m_ff.LPath = m_ff.GetValueByKey("LocalPath");
            //   m_ff.LPath = m_ff.LPath.Replace("\\", "\\\\");
            if (!m_ff.LPath.EndsWith("\\"))
            {
                m_ff.LPath = m_ff.LPath + "\\";//来判断是D:\ 还是 D:\test，就是是判断根目录还是文件夹
            }
            // skinEngine1 = new Sunisoft.IrisSkin.SkinEngine(this);
            // skinEngine1.SkinFile = Application.StartupPath + "//Skins//MacOs.ssk";
           // skinmod = comboBox1.Text;
            m_ff.ModifyAppSettings("Skin", comboBox1.Text);
            //  m_ff.skinEngine1 = new Sunisoft.IrisSkin.SkinEngine(this);
            //m_ff.skinEngine1.SkinFile = Application.StartupPath + "//Skins//"+ skinmod+".ssk";
            m_ff.SkinMode = m_ff.GetValueByKey("Skin");
          //  m_ff.skinEngine1 = new Sunisoft.IrisSkin.SkinEngine(this);
            m_ff.skinEngine1.SkinFile = Application.StartupPath + "//Skins//" + m_ff.SkinMode + ".ssk";

           // m_ff.form1load(null,null);
            this.Dispose();
            this.Close();
            m_ff.Show();
        }

      
        private void Flush1_Click(object sender, EventArgs e)//刷新
        {
            //SvnUserName = m_ff.GetValueByKey("SvnUserName");
            //SvnPass = m_ff.GetValueByKey("SvnPass");
            SvnClient client = new SvnClient();//创建一个服务器端
            SvnUriTarget rem = new SvnUriTarget(SvnUrl);//连接这个svn目标

         //   client.Authentication.ClearAuthenticationCache();
            client.Authentication.Clear();//清除原有的账户信息
                                          //重新设定账户信息

           // SharpSvn.UI.SvnUIBindArgs uiBindArgs = new SharpSvn.UI.SvnUIBindArgs();
            //SharpSvn.UI.SvnUI.Bind(client, uiBindArgs);//自动登录,授权的UI界面

            client.Authentication.UserNamePasswordHandlers += new EventHandler<SvnUserNamePasswordEventArgs>(delegate (object s, SvnUserNamePasswordEventArgs ee)
            {
                ee.UserName = SvnUserName; //"zhaohanqing";
                ee.Password = SvnPass;// "Es123456";
            });//账号密码

           
            bool gotList;//判断是否导出LIST目录
            List<string> FilesList = new List<string>();//创建一个list 

            Collection<SvnListEventArgs> svnlist;// svn 的事件变量 svnlist 这是一个SHarpsvn的collection
            try
            {
                gotList = client.GetList(rem, out svnlist);//获取client的文件夹列表 地址是rem 给予到 svnlist 成功gotlist 为ture

            }
            catch (Exception er)
            {
                throw er;
            }
            if (gotList)//如果
            {
                foreach (SvnListEventArgs item in svnlist)//创建一个SCN的事件变量 item 这是一个 svnlist  每次输出一个
                {
                    if (!String.IsNullOrEmpty(item.Path.ToString()))//如果不为空就显示出这个名字
                    {
                        // FilesList.Add(item.Path);
                        listBox1.Items.Add(item.Path);//文件、文件夹都显示出来到列表之中
                                                      // MessageBox.Show(item.Path); //
                    }
                }
            }

          
        }

        private void button5_Click(object sender, EventArgs e)//浏览本地目录
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.Description = "请选择文件路径";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                string foldPath = dialog.SelectedPath;
                textBox2.Text = foldPath;
             
            }


        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button5_Click_1(object sender, EventArgs e)
        {

            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.Description = "请选择文件路径";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                string foldPath = dialog.SelectedPath;
                textBox2.Text = foldPath;
              //  button3_Click(null, null);
            }
        }



        //private void Form2_FormClosed(object sender, FormClosedEventArgs e)
        //{
        //    this.Close();
        //    m_ff.Close();
        //}
    }
}
