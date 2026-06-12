using System;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.Windows.Forms;

namespace 软构作业8
{
    public partial class Form1 : Form
    {
        // 数据库文件路径（和 exe 同目录）
        private string dbPath = "words.db";
        private string connStr => $"Data Source={dbPath};Version=3;";

        // 当前题目
        private string currentWord = "";
        private string currentMeaning = "";

        // 控件
        private Label lblMeaning;
        private TextBox txtAnswer;
        private Button btnNext;
        private Label lblResult;
        private Label lblProgress;
        private int currentIndex = 0;
        private DataTable wordTable;

        public Form1()
        {
            InitializeComponent();
            BuildUI();
            InitDatabase();
            LoadWords();
            ShowNext();
        }

        // ══════════════════════════════════════════════════════
        //  界面构建
        // ══════════════════════════════════════════════════════
        private void BuildUI()
        {
            this.Text = "背单词";
            this.Size = new Size(480, 360);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.BackColor = Color.FromArgb(248, 248, 248);

            // 进度
            lblProgress = new Label();
            lblProgress.Location = new Point(20, 15);
            lblProgress.Size = new Size(420, 25);
            lblProgress.Font = new Font("微软雅黑", 10);
            lblProgress.ForeColor = Color.Gray;
            lblProgress.Text = "第 1 题";
            this.Controls.Add(lblProgress);

            // 中文词义（题目）
            var lblHint = new Label();
            lblHint.Text = "中文词义：";
            lblHint.Location = new Point(20, 50);
            lblHint.Size = new Size(90, 25);
            lblHint.Font = new Font("微软雅黑", 10);
            this.Controls.Add(lblHint);

            lblMeaning = new Label();
            lblMeaning.Location = new Point(20, 80);
            lblMeaning.Size = new Size(420, 60);
            lblMeaning.Font = new Font("微软雅黑", 20, FontStyle.Bold);
            lblMeaning.ForeColor = Color.FromArgb(0, 100, 200);
            lblMeaning.Text = "";
            this.Controls.Add(lblMeaning);

            // 输入框提示
            var lblInputHint = new Label();
            lblInputHint.Text = "请输入对应英文单词（按回车确认）：";
            lblInputHint.Location = new Point(20, 155);
            lblInputHint.Size = new Size(300, 25);
            lblInputHint.Font = new Font("微软雅黑", 10);
            this.Controls.Add(lblInputHint);

            // 输入框
            txtAnswer = new TextBox();
            txtAnswer.Location = new Point(20, 185);
            txtAnswer.Size = new Size(300, 35);
            txtAnswer.Font = new Font("微软雅黑", 14);
            txtAnswer.KeyDown += TxtAnswer_KeyDown;
            this.Controls.Add(txtAnswer);

            // 确认按钮
            btnNext = new Button();
            btnNext.Text = "确认";
            btnNext.Location = new Point(335, 183);
            btnNext.Size = new Size(105, 37);
            btnNext.Font = new Font("微软雅黑", 11, FontStyle.Bold);
            btnNext.BackColor = Color.FromArgb(0, 120, 215);
            btnNext.ForeColor = Color.White;
            btnNext.FlatStyle = FlatStyle.Flat;
            btnNext.FlatAppearance.BorderSize = 0;
            btnNext.Click += BtnNext_Click;
            this.Controls.Add(btnNext);

            // 结果标签
            lblResult = new Label();
            lblResult.Location = new Point(20, 235);
            lblResult.Size = new Size(420, 50);
            lblResult.Font = new Font("微软雅黑", 16, FontStyle.Bold);
            lblResult.Text = "";
            this.Controls.Add(lblResult);
        }

        // ══════════════════════════════════════════════════════
        //  数据库初始化（建表 + 插入示例单词）
        // ══════════════════════════════════════════════════════
        private void InitDatabase()
        {
            if (!System.IO.File.Exists(dbPath))
                SQLiteConnection.CreateFile(dbPath);

            using (var conn = new SQLiteConnection(connStr))
            {
                conn.Open();

                // 建表
                var cmd = conn.CreateCommand();
                cmd.CommandText = @"
                    CREATE TABLE IF NOT EXISTS Words (
                        Id      INTEGER PRIMARY KEY AUTOINCREMENT,
                        English TEXT NOT NULL,
                        Chinese TEXT NOT NULL
                    )";
                cmd.ExecuteNonQuery();

                // 检查是否已有数据
                cmd.CommandText = "SELECT COUNT(*) FROM Words";
                long count = (long)cmd.ExecuteScalar();
                if (count == 0)
                {
                    // 插入示例单词
                    string[] data = {
                        "apple|苹果",   "book|书",      "cat|猫",
                        "dog|狗",       "egg|鸡蛋",     "fish|鱼",
                        "gold|金子",    "hand|手",      "ice|冰",
                        "joy|快乐",     "key|钥匙",     "light|光",
                        "moon|月亮",    "night|夜晚",   "ocean|海洋",
                        "paper|纸",     "queen|女王",   "river|河流",
                        "star|星星",    "tree|树"
                    };
                    foreach (var item in data)
                    {
                        var parts = item.Split('|');
                        cmd.CommandText =
                            $"INSERT INTO Words(English,Chinese) VALUES('{parts[0]}','{parts[1]}')";
                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }

        // ══════════════════════════════════════════════════════
        //  加载所有单词
        // ══════════════════════════════════════════════════════
        private void LoadWords()
        {
            using (var conn = new SQLiteConnection(connStr))
            {
                conn.Open();
                var adapter = new SQLiteDataAdapter(
                    "SELECT English, Chinese FROM Words ORDER BY RANDOM()", conn);
                wordTable = new DataTable();
                adapter.Fill(wordTable);
            }
        }

        // ══════════════════════════════════════════════════════
        //  显示下一题
        // ══════════════════════════════════════════════════════
        private void ShowNext()
        {
            if (wordTable == null || wordTable.Rows.Count == 0) return;

            if (currentIndex >= wordTable.Rows.Count)
            {
                lblMeaning.Text = "全部完成！";
                txtAnswer.Enabled = false;
                btnNext.Enabled = false;
                lblResult.Text = "";
                lblProgress.Text = "已完成所有单词";
                return;
            }

            DataRow row = wordTable.Rows[currentIndex];
            currentWord = row["English"].ToString();
            currentMeaning = row["Chinese"].ToString();

            lblMeaning.Text = currentMeaning;
            lblProgress.Text = $"第 {currentIndex + 1} 题 / 共 {wordTable.Rows.Count} 题";
            lblResult.Text = "";
            txtAnswer.Clear();
            txtAnswer.Focus();
        }

        // ══════════════════════════════════════════════════════
        //  判断答案
        // ══════════════════════════════════════════════════════
        private void Check()
        {
            string answer = txtAnswer.Text.Trim().ToLower();
            if (answer == "")
            {
                lblResult.Text = "请输入答案！";
                lblResult.ForeColor = Color.Orange;
                return;
            }

            if (answer == currentWord.ToLower())
            {
                lblResult.Text = "✔ 正确！";
                lblResult.ForeColor = Color.FromArgb(0, 160, 0);
            }
            else
            {
                lblResult.Text = $"✘ 错误！正确答案是：{currentWord}";
                lblResult.ForeColor = Color.FromArgb(200, 0, 0);
            }

            currentIndex++;

            // 延迟1.5秒后显示下一题
            var timer = new System.Windows.Forms.Timer();
            timer.Interval = 1500;
            timer.Tick += (s, e) =>
            {
                timer.Stop();
                ShowNext();
            };
            timer.Start();
        }

        private void BtnNext_Click(object sender, EventArgs e) => Check();

        private void TxtAnswer_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return) Check();
        }
    }
}