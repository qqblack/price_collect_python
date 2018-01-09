using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;


namespace price_collect_python
{
    public partial class Form1 : Form
    {
        public Encoding StandardErrorEncoding { get; set; }
        public string info_csv = "./查询列表.csv";
        public string[] title = { "商品", "单位", "链接", "价格xpath" };
        // 要将该程序放在bin/debug和bin/release里
        //string exe_path = "价格采集-测试参数传递.exe";  // 测试参数传递时用的
        public string exe_path = "价格采集.exe";
        public delegate void StartProcessDelegate(string runFilePath, params string[] args);

        public color_log log = new color_log(); // 富文本的日志输出

        public Form1()
        {
            InitializeComponent();
        }

        // FormClosingEventArgs 是关闭窗口
        void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (MessageBox.Show(
                    "是否要退出?",
                    "提示",
                    MessageBoxButtons.OKCancel,
                    MessageBoxIcon.Question) == DialogResult.OK)
            {
                //save_info(sender,e);
            }
        }

        // 保存采集信息
        private void save_info(object sender, EventArgs e)
        {

            string[] content = { textBox1.Text, textBox2.Text, textBox3.Text, textBox4.Text };
            string str_content = String.Format("\"{0}\",\"{1}\",\"{2}\",\"{3}\"\n", content);
            string str_title = String.Format("\"{0}\",\"{1}\",\"{2}\",\"{3}\"\n", title);
            if (File.Exists(info_csv))
            {
                File.AppendAllText(@info_csv, str_content, ASCIIEncoding.UTF8);  // 添加模式,utf-8
            }
            else {
                File.WriteAllText(@info_csv, str_title, ASCIIEncoding.UTF8); // 写
                File.AppendAllText(@info_csv, str_content, ASCIIEncoding.UTF8); // 添加
            };
            string Message_str = String.Format("已保存至\"{0}\"", info_csv);
            log.LogMessage(display_box,Message_str);

        }

        // 开始爬取数据
        public void start_craw(object sender, EventArgs e)
        {
            string goods_name, goods_unit, goods_url, price_xpath;
            goods_name = textBox1.Text;
            goods_unit = textBox2.Text;
            goods_url = textBox3.Text;
            price_xpath = textBox4.Text;

            string[] the_args = { goods_name, goods_unit, goods_url, price_xpath };
            StartProcessDelegate la = new StartProcessDelegate(StartProcess); // 创建委托
            //StartProcess(exe_path, the_args);
            Invoke(la, exe_path, the_args);
        }

        // 用于调用外部exe
        public void StartProcess(string runFilePath, params string[] args)
        {
            try
            {
                string s = "";
                foreach (string arg in args)
                {
                    s = s + arg + " ";
                }
                s = s.Trim();
                Process process = new Process();//创建进程对象    
                ProcessStartInfo startInfo = new ProcessStartInfo(runFilePath, s); // 括号里是(程序名,参数)
                process.StartInfo = startInfo;
                process.StartInfo.UseShellExecute = false;       //是否使用操作系统的shell启动,设为false后才能捕捉错误
                startInfo.RedirectStandardOutput = true;         //由调用程序获取输出信息
                startInfo.CreateNoWindow = true;                 //不显示调用程序的窗口
                process.StartInfo.RedirectStandardError = true;  //重定向错误流
                //startInfo.StandardErrorEncoding = ASCIIEncoding.UTF8; // 设置编码
                //startInfo.StandardOutputEncoding = ASCIIEncoding.GetEncoding(936);// gbk只能用代码页来设置
                process.Start();
                StreamReader out_reader = process.StandardOutput;     
                log.LogMessage(display_box, out_reader.ReadToEnd()); // 将输出内容写入日志
                /// 调试
                StreamReader Error_Reader = process.StandardError;// 读取错误流
                string info_error = Error_Reader.ReadToEnd();   // 正确的姿势
                string feedback = Error_Reader.ReadLine();        // ReadLine()只读取第一行
                if (info_error != "")
                {
                    info_error = "参数:\n" + s +"\n捕捉到外部程序的错误:\n" + info_error;
                    log.LogError(display_box,info_error);
                }
                process.WaitForExit();
            }
            catch (System.SystemException e)
            {
                string info_C = "捕捉到C#的错误:\n" + e.ToString();
                log.LogMessage(display_box, info_C);
            }

        }


        // 打开"查询列表.csv",批量采集
        public void batch_crawl(object sender, EventArgs e)
        {

            string[] lines =File.ReadAllLines(@info_csv,Encoding.UTF8);// 读取
            int m =lines.Count();
            for (int i = 1; i <= m - 1; i = i + 1) // 第1行是表头,跳过
            {
                log.LogWarning(display_box, "找到第" + i.ToString() + "个!,内容为:");
                log.LogNormal(display_box, lines[i].ToString());

                string[] data0 = lines[i].Split('\"');
                /// 去除数组中的逗号,空格,空值
                List<string> data_1 = new List<string>(data0);
                string[] data = data_1.Where( p => (p != ",")&(p != " ") & (p != "")).ToArray();

                /// 判断参数是否切割正确
                int args_num = data.Length;
                if (args_num != 4)
                {
                    string Message_info = String.Format("参数个数应该为4,现有{0}个!\n", args_num.ToString());
                    for (int j = 0; j < args_num; j = j + 1)
                    {
                        Message_info += String.Format("{0})", j + 1) + String.Format("{0}\n", data[j]);
                    }
                    log.LogMessage(display_box, Message_info);
                }

                StartProcess(exe_path, data);
            }
        }

        //private void button1_Click(object sender, EventArgs e)
        //{
        //    LogMessage("绿色");
        //    LogError("红色");
        //    LogWarning("粉色");
        //}

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }

    public class color_log
    {
        #region 日志记录、支持其他线程访问  

        public delegate void LogAppendDelegate(RichTextBox RichTextBox0, Color color, string text);

        public void LogAppendMethod(RichTextBox RichTextBox0,Color color, string text)
        {
            if (!RichTextBox0.ReadOnly)
            { RichTextBox0.ReadOnly = true; }

            RichTextBox0.AppendText("\n");
            RichTextBox0.SelectionColor = color;
            RichTextBox0.AppendText(text);
            RichTextBox0.ScrollToCaret(); // 保持滚动条在底部
            RichTextBox0.Refresh();       // 刷新文本框显示
        }

        public void LogError(RichTextBox RichTextBox0,string text)
        {
            LogAppendDelegate la = new LogAppendDelegate(LogAppendMethod);
            RichTextBox0.Invoke(la, RichTextBox0, Color.Red, DateTime.Now.ToString("[yyyy-MM-dd HH:mm:ss] ") + text);
        }

        public void LogWarning(RichTextBox RichTextBox0, string text)
        {
            LogAppendDelegate la = new LogAppendDelegate(LogAppendMethod);
            RichTextBox0.Invoke(la, RichTextBox0, Color.Blue, DateTime.Now.ToString("[yyyy-MM-dd HH:mm:ss] ") + text);
        }

        public void LogMessage(RichTextBox RichTextBox0, string text)
        {
            LogAppendDelegate la = new LogAppendDelegate(LogAppendMethod);
            RichTextBox0.Invoke(la, RichTextBox0, Color.Green, DateTime.Now.ToString("[yyyy-MM-dd HH:mm:ss] ") + text);
        }

        public void LogNormal(RichTextBox RichTextBox0, string text)
        {
            LogAppendDelegate la = new LogAppendDelegate(LogAppendMethod);
            RichTextBox0.Invoke(la, RichTextBox0, Color.Black, DateTime.Now.ToString("[yyyy-MM-dd HH:mm:ss] ") + text);
        }
        #endregion
    }
}
