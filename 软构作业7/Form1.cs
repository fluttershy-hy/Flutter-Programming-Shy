using System;
using System.Drawing;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace 软构作业7
{
    public partial class Form1 : Form
    {
        private TextBox txtKeyword;
        private Button btnSearch;
        private TextBox txtBaidu;
        private TextBox txtBing;
        private Label lblKeyword, lblBaidu, lblBing;

        public Form1()
        {
            InitializeComponent();
            BuildUI();
        }

        private void BuildUI()
        {
            this.Text = "双引擎搜索摘抄";
            this.Size = new Size(700, 580);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.BackColor = Color.FromArgb(245, 245, 245);

            // 关键字标签
            lblKeyword = new Label();
            lblKeyword.Text = "搜索关键字：";
            lblKeyword.Location = new Point(20, 20);
            lblKeyword.Size = new Size(100, 25);
            lblKeyword.Font = new Font("微软雅黑", 10);
            this.Controls.Add(lblKeyword);

            // 关键字输入框
            txtKeyword = new TextBox();
            txtKeyword.Location = new Point(125, 18);
            txtKeyword.Size = new Size(430, 30);
            txtKeyword.Font = new Font("微软雅黑", 10);
            this.Controls.Add(txtKeyword);

            // 搜索按钮
            btnSearch = new Button();
            btnSearch.Text = "搜索";
            btnSearch.Location = new Point(570, 16);
            btnSearch.Size = new Size(90, 32);
            btnSearch.Font = new Font("微软雅黑", 10, FontStyle.Bold);
            btnSearch.BackColor = Color.FromArgb(0, 120, 215);
            btnSearch.ForeColor = Color.White;
            btnSearch.FlatStyle = FlatStyle.Flat;
            btnSearch.FlatAppearance.BorderSize = 0;
            btnSearch.Click += BtnSearch_Click;
            this.Controls.Add(btnSearch);

            // Baidu 标签
            lblBaidu = new Label();
            lblBaidu.Text = "百度搜索结果（前200字）：";
            lblBaidu.Location = new Point(20, 65);
            lblBaidu.Size = new Size(220, 25);
            lblBaidu.Font = new Font("微软雅黑", 10, FontStyle.Bold);
            lblBaidu.ForeColor = Color.FromArgb(200, 0, 0);
            this.Controls.Add(lblBaidu);

            // Baidu 结果框
            txtBaidu = new TextBox();
            txtBaidu.Location = new Point(20, 93);
            txtBaidu.Size = new Size(645, 200);
            txtBaidu.Font = new Font("微软雅黑", 10);
            txtBaidu.Multiline = true;
            txtBaidu.ScrollBars = ScrollBars.Vertical;
            txtBaidu.ReadOnly = true;
            txtBaidu.BackColor = Color.White;
            txtBaidu.Text = "等待搜索...";
            this.Controls.Add(txtBaidu);

            // Bing 标签
            lblBing = new Label();
            lblBing.Text = "Bing 搜索结果（前200字）：";
            lblBing.Location = new Point(20, 308);
            lblBing.Size = new Size(220, 25);
            lblBing.Font = new Font("微软雅黑", 10, FontStyle.Bold);
            lblBing.ForeColor = Color.FromArgb(0, 100, 180);
            this.Controls.Add(lblBing);

            // Bing 结果框
            txtBing = new TextBox();
            txtBing.Location = new Point(20, 336);
            txtBing.Size = new Size(645, 200);
            txtBing.Font = new Font("微软雅黑", 10);
            txtBing.Multiline = true;
            txtBing.ScrollBars = ScrollBars.Vertical;
            txtBing.ReadOnly = true;
            txtBing.BackColor = Color.White;
            txtBing.Text = "等待搜索...";
            this.Controls.Add(txtBing);
        }

        // ── 点击搜索 ──────────────────────────────────────────
        private async void BtnSearch_Click(object sender, EventArgs e)
        {
            string keyword = txtKeyword.Text.Trim();
            if (string.IsNullOrEmpty(keyword))
            {
                MessageBox.Show("请输入关键字！", "提示");
                return;
            }

            btnSearch.Enabled = false;
            btnSearch.Text = "搜索中...";
            txtBaidu.Text = "正在请求百度...";
            txtBing.Text = "正在请求 Bing...";

            // 两个任务并行执行（多线程/异步）
            Task<string> baiduTask = FetchSearchResultAsync(
                $"https://www.baidu.com/s?wd={Uri.EscapeDataString(keyword)}");

            Task<string> bingTask = FetchSearchResultAsync(
                $"https://www.bing.com/search?q={Uri.EscapeDataString(keyword)}");

            // 等待两个任务都完成
            await Task.WhenAll(baiduTask, bingTask);

            txtBaidu.Text = baiduTask.Result;
            txtBing.Text = bingTask.Result;

            btnSearch.Enabled = true;
            btnSearch.Text = "搜索";
        }

        // ── 抓取搜索页面并提取前200个汉字/文字 ──────────────
        private async Task<string> FetchSearchResultAsync(string url)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.Timeout = TimeSpan.FromSeconds(15);
                    client.DefaultRequestHeaders.Add("User-Agent",
                        "Mozilla/5.0 (Windows NT 10.0; Win64; x64) " +
                        "AppleWebKit/537.36 Chrome/120.0 Safari/537.36");

                    var response = await client.GetAsync(url);
                    byte[] bytes = await response.Content.ReadAsByteArrayAsync();

                    // 自动检测编码（百度是 UTF-8，Bing 也是）
                    string html = System.Text.Encoding.UTF8.GetString(bytes);

                    // 去除 HTML 标签
                    string text = Regex.Replace(html, "<[^>]+>", " ");
                    // 去除 script / style 块
                    text = Regex.Replace(text, @"<(script|style)[^>]*>[\s\S]*?<\/\1>", "",
                        RegexOptions.IgnoreCase);
                    // 合并多余空白
                    text = Regex.Replace(text, @"\s+", " ").Trim();
                    // 只保留中文、字母、数字、标点
                    text = Regex.Replace(text, @"[^\u4e00-\u9fa5a-zA-Z0-9，。！？、：；""''（）\s]", "");
                    text = Regex.Replace(text, @"\s+", " ").Trim();

                    // 取前200个字符
                    return text.Length > 200 ? text.Substring(0, 200) + "..." : text;
                }
            }
            catch (Exception ex)
            {
                return "请求失败：" + ex.Message;
            }
        }
    }
}