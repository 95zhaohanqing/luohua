using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace test10
{
    public partial class Form3 : Form
    {
        public Form3()
        {
            InitializeComponent();
        }

        private void Form3_Load(object sender, EventArgs e)
        {

        }

        private void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {

        }
    }


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
    //class Class1
    //{
    //    static void Main()
    //    {
    //        Student s1 = new Student();
    //        s1.Name = "Student1";
    //        Student s2 = new Student();
    //        s2.Name = "Student2";
    //        s1.RegisterOK += new Student.DelegateRegisterOkEvent(Student_RegisterOK);
    //        s2.RegisterOK += new Student.DelegateRegisterOkEvent(Student_RegisterOK);

    //        //当Register方法一执行，触发RegisterOK事件 
    //        //RegisterOK事件一触发，然后执行Student_RegisterOK方法 
    //        s1.Register();
    //        s2.Register();
    //        Console.ReadLine();
    //    }
    //    static void Student_RegisterOK(RegisterOkArgs e)
    //    {
    //        Console.WriteLine(e.EventInfo);
    //    }
    //}

    //class Student
    //{
    //    public delegate void DelegateRegisterOkEvent(RegisterOkArgs e);
    //    public event DelegateRegisterOkEvent RegisterOK;
    //    public string Name;
    //    public void Register()
    //    {
    //        Console.WriteLine("Register Method");
    //        RegisterOK(new RegisterOkArgs("Student Name: " + Name));
    //    }
    //}
    //class RegisterOkArgs : EventArgs
    //{
    //    public string EventInfo;
    //    public RegisterOkArgs(string eventInfo) : base()
    //    {
    //        this.EventInfo = eventInfo;
    //    }
    //}





  
             

    /// <summary>
    /// 排序后下载选中
    /// </summary>
  

}
