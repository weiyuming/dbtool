using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DBTOOL.Helper;
using DBTOOL.Models;
using System.IO;
using System.Threading;

namespace DBTOOL
{
    public partial class form1 : Form
    {
        public form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            outputLog("【手动执行开始】");
            start();

        }


        String hour = "";
        String min = "";


        private void Form1_Load(object sender, EventArgs e)
        {
            richTextBoxLog.ReadOnly = true;//设置日志框为只读
            richTextBoxLog.BackColor = Color.White;//设置背景色

            List<SettingVo> list = XmlHelper.GetSetting();
            //只有一个，直接取
            SettingVo vo = list[0];
            hour = vo.Hour;
            min = vo.Min;


            outputLog("【系统启动中】配置的定时触发时间为" + hour + "点" + min+"分");

        }




        //日志记录
        public void outputLogThread(String msg)
        {
            // 异步，无参的写法
            //Thread thread1 = new Thread(new ThreadStart(outputLog));

            //异步，有参数的写法
            Thread t = new Thread(new ParameterizedThreadStart(outputLogThread));
            t.Start(msg);

            //outputLog(msg, Color.Black, false);
        }


        //日志记录
        public void outputLogThread(Object obj)
        {
            String msg = obj.ToString();
            outputLog(msg, Color.Black, false);
        }



        //日志记录
        public void outputLog(String msg)
        {
            outputLog(msg, Color.Black, false);
        }


        //日志记录
        public void outputLog(String msg, Color color, Boolean isBold)
        {
                //让文本框获取焦点，不过注释这行也能达到效果
                this.richTextBoxLog.Focus();
                //设置光标的位置到文本尾   
                this.richTextBoxLog.Select(this.richTextBoxLog.TextLength, 0);
                //滚动到控件光标处   
                this.richTextBoxLog.ScrollToCaret();
                //设置字体颜色
                this.richTextBoxLog.SelectionColor = color;
                if (isBold)
                {
                    this.richTextBoxLog.SelectionFont = new Font(Font, FontStyle.Bold);
                }



                System.DateTime currentTime = new System.DateTime();
                currentTime = System.DateTime.Now; //获取当前时间
                msg = currentTime.ToString("yyyy-MM-dd HH:mm:ss") + "  " + msg;
                this.richTextBoxLog.AppendText(msg);//输出到界面
                this.richTextBoxLog.AppendText(Environment.NewLine);

                LogHelper.SetLog(msg);//写入日志文件
            
        }













        delegate void AsynUpdateUI(String msg);

        //更新UI
        private void UpdataUIStatus(String msg)
        {
            if (InvokeRequired)//这句话是判断操作控件是否需要用调用Invoke方法
            {
                //需要用Invoke调用，使用Invoke方法需要一个委托，故而有了 建立一个委托来实现非创建控件的线程更新控件。就是在这里使用的
                this.Invoke(new AsynUpdateUI(delegate(String msgs)
                {
                    outputLog(msgs);
                }), msg);
            }
            else//否则直接更新控件
            {
                outputLog(msg);
            }
        }


        /// <summary>
        /// 总的调用方法
        /// </summary>
        private void start()
        {

            //只要启动就清空一次日志框
            this.richTextBoxLog.Clear();
            try
            {
                DataWrite dataWrite = new DataWrite();//实例化一个写入数据的类
                dataWrite.outputLog += UpdataUIStatus;//绑定更新任务状态的委托

                //dataWrite.TaskCallBack += Accomplish;//绑定完成任务要调用的委托

                Thread thread = new Thread(new ParameterizedThreadStart(dataWrite.BackTable));
                thread.IsBackground = true;
                thread.Start("");
            }
            catch (Exception ex)
            {
                outputLog(ex.Message);
            }

           

        }


        /// <summary>
        /// 自动备份的定时器
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timerStart_Tick(object sender, EventArgs e)
        {
            //获取当前时间
            String NowHour = DateTime.Now.Hour+"";
            String NowMin = DateTime.Now.Minute+"";

            //outputLog("【定时任务进入当前时间】" + NowHour+ "  "  + NowMin);
            //outputLog("【定时任务进入配置时间】" + hour + "  " + min);
            if (StringHelper.isEqual(NowHour, hour) && StringHelper.isEqual(NowMin, min))
            {
                outputLog("【自动执行开始】" );
                start();
            }
        }





        #region
        //创建NotifyIcon对象 
        NotifyIcon notifyicon = new NotifyIcon();
        //创建托盘图标对象 
        Icon ico = new Icon("datas_database_128px_1082312_easyicon.net.ico");
        //创建托盘菜单对象 
        ContextMenu notifyContextMenu = new ContextMenu();
        #endregion


        #region 托盘提示
        //private void Form1_Load(object sender, EventArgs e)
        //{

        //}
        #endregion

        #region 隐藏任务栏图标、显示托盘图标
        private void Form1_SizeChanged(object sender, EventArgs e)
        {
            //判断是否选择的是最小化按钮 
            if (WindowState == FormWindowState.Minimized)
            {
                //托盘显示图标等于托盘图标对象 
                //注意notifyIcon1是控件的名字而不是对象的名字 
                notifyIcon1.Icon = ico;
                //隐藏任务栏区图标 
                this.ShowInTaskbar = false;
                //图标显示在托盘区 
                notifyicon.Visible = true;
            }
        }
        #endregion

        #region 还原窗体
        private void notifyIcon1_DoubleClick(object sender, EventArgs e)
        {
            huanyuan();
        }


        private void huanyuan()
        {
            //判断是否已经最小化于托盘 
            if (WindowState == FormWindowState.Minimized)
            {
                //还原窗体显示 
                WindowState = FormWindowState.Normal;
                //激活窗体并给予它焦点 
                this.Activate();
                //任务栏区显示图标 
                this.ShowInTaskbar = true;
                //托盘区图标隐藏 
                notifyicon.Visible = false;


                //让文本框获取焦点，不过注释这行也能达到效果
                this.richTextBoxLog.Focus();
                //设置光标的位置到文本尾   
                this.richTextBoxLog.Select(this.richTextBoxLog.TextLength, 0);
                //滚动到控件光标处   
                this.richTextBoxLog.ScrollToCaret();
            }
        }
        #endregion


        //关闭窗体前执行
        private void form_FormClosing(object sender, FormClosingEventArgs e)
        {
            //exitpassword form2 = new exitpassword();
            //form.FormBorderStyle = FormBorderStyle.None; 
            //隐藏子窗体边框（去除最小花，最大化，关闭等按钮）
            // form.TopLevel =false; 
            //指示子窗体非顶级窗体
            //this.panel1.Controls.Add(form);//将子窗体载入panel

            //form2.MdiParent = this;
            //form2.StartPosition = FormStartPosition.CenterScreen;

            e.Cancel = true;
            //form2.ShowDialog();

            this.WindowState = FormWindowState.Minimized;

        }

        private void 显示ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            huanyuan();
        }

        private void 退出ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("是否确认退出程序？退出后自动备份功能将失效", "退出", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
            {
                // 关闭所有的线程
                this.Dispose();
                this.Close();
            }
        }

        private void buttonTest_Click(object sender, EventArgs e)
        {
            Object obj = textBox1.Text;
            DataWrite dataWrite = new DataWrite();//实例化一个写入数据的类
            dataWrite.outputLog += UpdataUIStatus;//绑定更新任务状态的委托
            //dataWrite.TaskCallBack += Accomplish;//绑定完成任务要调用的委托

            Thread thread = new Thread(new ParameterizedThreadStart(dataWrite.test));
            thread.IsBackground = true;
            thread.Start(obj);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            
            int date1 = int.Parse("20180601");
            int date2 = int.Parse("20180530");
            int resultDay = date1 - date2;

            DateTime oldDate = DateTime.ParseExact("20180601", "yyyyMMdd", null);
            DateTime newDate = DateTime.ParseExact("20180530", "yyyyMMdd", null);
            TimeSpan ts = newDate - oldDate;
            int differenceInDays = ts.Days;


            MessageBox.Show(differenceInDays.ToString());
        }
    }

}
