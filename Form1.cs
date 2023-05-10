using System.Windows.Forms;
using System.IO;
using System;
using System.Runtime.InteropServices;
using Microsoft.Web.WebView2.Core;
using System.Drawing;

namespace WindowsFormsApp6
{
    [ClassInterface(ClassInterfaceType.AutoDual)]
    [ComVisible(true)]
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            //指定html位置。可以用MessageBox.Show(url)打印路径确认文件位置是否正确。
            string path = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName + @"\@tinymce\index.html";  // 这里注意parent搭配获取目录的方法，index.html是放在项目sln文件同目录内@tiny_mce文件夹里面的。
            //webView21.Source = new Uri("D:\\GitHub\\WindowsFormsApp6\\@tinymce\\index.html");
            webView21.Source = new Uri(path);  //加载html
            webView21.Dock = DockStyle.Fill;  //确保横向铺满

            //窗口大小调整宽度时，3个按钮的位置也会浮动。
            this.SizeChanged += (s, e) => {
                button1.Location = new Point(this.ClientSize.Width - button1.Width - 20, button1.Location.Y);
                button2.Location = new Point(this.ClientSize.Width - button2.Width - 20, button2.Location.Y);
                button3.Location = new Point(this.ClientSize.Width - button3.Width - 20, button3.Location.Y);
            };

            //如下3行代码是确保网页js可以调用C#方法
            this.webView21.CoreWebView2InitializationCompleted += new EventHandler<Microsoft.Web.WebView2.Core.CoreWebView2InitializationCompletedEventArgs>(this.WebView2InitializationCompleted);
        }
        private void WebView2InitializationCompleted(object sender, CoreWebView2InitializationCompletedEventArgs e)
        {
            this.webView21.CoreWebView2.AddHostObjectToScript("customHost", this);
        }

        //C#调用js函数1
        private void Button1_Click(object sender, EventArgs e)
        {
            webView21.CoreWebView2.ExecuteScriptAsync("ShowMessage()");
        }
        //C#调用js函数2
        private void Button2_Click(object sender, EventArgs e)
        {
            int formHeight = this.ClientSize.Height;
            webView21.CoreWebView2.ExecuteScriptAsync($"ShowMessageArg('{formHeight}')");
        }
        //C#调用js函数3，并使用winform弹窗
        private async void Button3_Click(object sender, EventArgs e)
        {
            int formHeight = this.ClientSize.Height;
            var jsResult = await webView21.CoreWebView2.ExecuteScriptAsync($"GetData_js('{formHeight}')");
            if (!string.IsNullOrEmpty(jsResult))
            {
                MessageBox.Show(jsResult);
            }
        }


        //【js调用C#】
        public string str_abc()  //有返回值
        {
            string UserName = "我是C#属性";
            string abc = UserName+":zhihu20230511";
            //MessageBox.Show(abc);
            //webView21.CoreWebView2.ExecuteScriptAsync($"alert('{abc} ')");
            return abc;
        }
        public void str_123()  //无返回值
        {
            string UserName = "我是C#属性22";
            //MessageBox.Show("3333");
            //webView21.CoreWebView2.PostWebMessageAsString(UserName);
            webView21.CoreWebView2.ExecuteScriptAsync($"alert('{UserName} ')");
        }
        public void ShowMessage()  //前端未引用此函数，只是展示一个无参写法
        {
            MessageBox.Show("网页直接调用C#弹窗：123a");
        }
        public void ShowMessageArg(string arg)
        {
            MessageBox.Show("网页传值调用C#弹窗：" + arg);
        }
        public void TestCalcAddByCsharpMethod(int num1, int num2, string message)
        {
            MessageBox.Show($"C#方法接收到J传入的参数 num1={num1}，num2={num2}，message={message}", "C#提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            webView21.CoreWebView2.ExecuteScriptAsync($"alert('计算结果为：'+'{num1 + num2} ')");
        }
        public void GetData_cs(string arg_value)
        {
            //MessageBox.Show("【c#收到js字符串是】:" + arg_value);
            string js_value = arg_value + "zhihulkme";
            webView21.CoreWebView2.ExecuteScriptAsync($"ShowMessageArg('收到c#要求执行的js函数，弹窗显示js_value：{js_value}')");
            webView21.CoreWebView2.ExecuteScriptAsync($"document.getElementById('idcardmsg').value = '{js_value}';");
        }
        void Form1_SizeChanged(object sender, EventArgs e)
        {
            int formHeight = this.ClientSize.Height;
            string jsCode =
            $"document.querySelector('#idcardmsg').value = {formHeight};"
            + $"tinymce.activeEditor.editorContainer.style.height = {formHeight - 400} + 'px'"  // 减去400是为了让底部的“提交”按钮在合适位置；公式可以按需调整。
            ;
            webView21.ExecuteScriptAsync(jsCode);
        }
    }
}
