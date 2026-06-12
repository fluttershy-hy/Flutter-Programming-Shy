using System;
using System.Drawing;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace 软构作业6
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            BuildUI();
        }

        // ── 控件 ──────────────────────────────────────────────
        private TextBox txtUrl;
        private Button btnFetch;
        private TextBox txtPhones;
        private TextBox txtEmails;
        private Label lblPhone, lblEmail, lblUrl;

        private void BuildUI()
        {
            this.Text = "网页信息提取器";
            this.Size = new Size(600, 500);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.BackColor = Color.FromArgb(245, 245, 245);

            // URL 标签
            lblUrl = new Label();
            lblUrl.Text = "请输入 URL：";
            lblUrl.Location = new Point(20, 20);
            lblUrl.Size = new Size(100, 25);
            lblUrl.Font = new Font("微软雅黑", 10);
            this.Controls.Add(lblUrl);

            // URL 输入框
            txtUrl = new TextBox();
            txtUrl.Location = new Point(20, 48);
            txtUrl.Size = new Size(460, 30);
            txtUrl.Font = new Font("微软雅黑", 10);
            txtUrl.Text = "https://";
            this.Controls.Add(txtUrl);

            // 获取按钮
            btnFetch = new Button();
            btnFetch.Text = "获取";
            btnFetch.Location = new Point(495, 46);
            btnFetch.Size = new Size(80, 32);
            btnFetch.Font = new Font("微软雅黑", 10, FontStyle.Bold);
            btnFetch.BackColor = Color.FromArgb(0, 120, 215);
            btnFetch.ForeColor = Color.White;
            btnFetch.FlatStyle = FlatStyle.Flat;
            btnFetch.FlatAppearance.BorderSize = 0;
            btnFetch.Click += BtnFetch_Click;
            this.Controls.Add(btnFetch);

            // 手机号标签
            lblPhone = new Label();
            lblPhone.Text = "手机号码：";
            lblPhone.Location = new Point(20, 100);
            lblPhone.Size = new Size(100, 25);
            lblPhone.Font = new Font("微软雅黑", 10);
            this.Controls.Add(lblPhone);

            // 手机号结果框
            txtPhones = new TextBox();
            txtPhones.Location = new Point(20, 128);
            txtPhones.Size = new Size(555, 130);
            txtPhones.Font = new Font("微软雅黑", 10);
            txtPhones.Multiline = true;
            txtPhones.ScrollBars = ScrollBars.Vertical;
            txtPhones.ReadOnly = true;
            txtPhones.BackColor = Color.White;
            this.Controls.Add(txtPhones);

            // 邮箱标签
            lblEmail = new Label();
            lblEmail.Text = "邮箱地址：";
            lblEmail.Location = new Point(20, 275);
            lblEmail.Size = new Size(100, 25);
            lblEmail.Font = new Font("微软雅黑", 10);
            this.Controls.Add(lblEmail);

            // 邮箱结果框
            txtEmails = new TextBox();
            txtEmails.Location = new Point(20, 303);
            txtEmails.Size = new Size(555, 130);
            txtEmails.Font = new Font("微软雅黑", 10);
            txtEmails.Multiline = true;
            txtEmails.ScrollBars = ScrollBars.Vertical;
            txtEmails.ReadOnly = true;
            txtEmails.BackColor = Color.White;
            this.Controls.Add(txtEmails);
        }

        // ── 点击"获取"按钮 ────────────────────────────────────
        private async void BtnFetch_Click(object sender, EventArgs e)
        {
            string url = txtUrl.Text.Trim();
            if (string.IsNullOrEmpty(url) || url == "https://")
            {
                MessageBox.Show("请输入有效的 URL！", "提示");
                return;
            }

            btnFetch.Enabled = false;
            btnFetch.Text = "请稍候...";
            txtPhones.Text = "";
            txtEmails.Text = "";

            try
            {
                // 抓取网页
                using (var client = new HttpClient())
                {
                    client.Timeout = TimeSpan.FromSeconds(15);
                    client.DefaultRequestHeaders.Add("User-Agent",
                        "Mozilla/5.0 (Windows NT 10.0; Win64; x64)");

                    string html = await client.GetStringAsync(url);

                    // ── 正则：手机号 ──────────────────────────
                    // 匹配 1开头的11位手机号（可带 - 或空格分隔）
                    string phonePattern =
                        @"(?<!\d)(1[3-9]\d{9})(?!\d)";

                    var phones = new System.Collections.Generic.HashSet<string>();
                    foreach (Match m in Regex.Matches(html, phonePattern))
                        phones.Add(m.Value);

                    // ── 正则：邮箱 ────────────────────────────
                    string emailPattern =
                        @"[a-zA-Z0-9._%+\-]+@[a-zA-Z0-9.\-]+\.[a-zA-Z]{2,}";

                    var emails = new System.Collections.Generic.HashSet<string>();
                    foreach (Match m in Regex.Matches(html, emailPattern))
                        emails.Add(m.Value);

                    // ── 显示结果 ──────────────────────────────
                    txtPhones.Text = phones.Count > 0
                        ? string.Join(Environment.NewLine, phones)
                        : "未找到手机号码";

                    txtEmails.Text = emails.Count > 0
                        ? string.Join(Environment.NewLine, emails)
                        : "未找到邮箱地址";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("抓取失败：" + ex.Message, "错误");
            }
            finally
            {
                btnFetch.Enabled = true;
                btnFetch.Text = "获取";
            }
        }
    }
}