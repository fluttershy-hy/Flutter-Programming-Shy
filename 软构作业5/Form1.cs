using System;
using System.Drawing;
using System.Windows.Forms;

namespace 软构作业5
{
    public partial class Form1 : Form
    {
        // ── 状态变量 ──────────────────────────────────────────
        private string currentInput = "";       // 当前正在输入的数字
        private string expression = "";         // 左侧已输入的表达式
        private double firstNumber = 0;         // 第一个操作数
        private string currentOperator = "";    // 当前运算符
        private bool operatorPressed = false;   // 是否刚按了运算符
        private bool resultShown = false;       // 是否刚显示了结果

        // ── 控件变量 ──────────────────────────────────────────
        private TextBox txtDisplay;
        private Button[] btnNumbers = new Button[10];
        private Button btnAdd, btnSubtract, btnMultiply, btnDivide;
        private Button btnEquals, btnClear;

        public Form1()
        {
            InitializeComponent();
            BuildUI();
        }

        // ══════════════════════════════════════════════════════
        //  构建界面
        // ══════════════════════════════════════════════════════
        private void BuildUI()
        {
            // ── 窗体 ──────────────────────────────────────────
            this.Text = "计算器";
            this.Size = new Size(360, 520);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(30, 30, 30);

            // ── 调色板 ────────────────────────────────────────
            Color displayBg = Color.FromArgb(20, 20, 20);
            Color numBg = Color.FromArgb(60, 60, 60);
            Color numHover = Color.FromArgb(80, 80, 80);
            Color opBg = Color.FromArgb(255, 149, 0);   // 橙色
            Color eqBg = Color.FromArgb(52, 199, 89);   // 绿色
            Color clrBg = Color.FromArgb(255, 59, 48);   // 红色
            Color textWhite = Color.White;

            // ── 显示框 ────────────────────────────────────────
            txtDisplay = new TextBox();
            txtDisplay.Location = new Point(20, 20);
            txtDisplay.Size = new Size(300, 65);
            txtDisplay.Font = new Font("Segoe UI", 22, FontStyle.Bold);
            txtDisplay.TextAlign = HorizontalAlignment.Right;
            txtDisplay.ReadOnly = true;
            txtDisplay.BackColor = displayBg;
            txtDisplay.ForeColor = textWhite;
            txtDisplay.BorderStyle = BorderStyle.None;
            txtDisplay.Text = "0";
            this.Controls.Add(txtDisplay);

            // ── 数字按钮 7 8 9 / 4 5 6 / 1 2 3 ──────────────
            int[] layout = { 7, 8, 9, 4, 5, 6, 1, 2, 3 };
            for (int i = 0; i < 9; i++)
            {
                int n = layout[i];
                int col = i % 3;
                int row = i / 3;
                btnNumbers[n] = MakeButton(
                    n.ToString(),
                    new Point(20 + col * 100, 110 + row * 80),
                    new Size(80, 65),
                    numBg, textWhite
                );
                int captured = n;
                btnNumbers[n].Click += (s, e) => NumClick(captured.ToString());
                this.Controls.Add(btnNumbers[n]);
            }

            // ── 数字 0（宽按钮）─────────────────────────────
            btnNumbers[0] = MakeButton("0",
                new Point(20, 350), new Size(180, 65), numBg, textWhite);
            btnNumbers[0].Click += (s, e) => NumClick("0");
            this.Controls.Add(btnNumbers[0]);

            // ── 等号 ─────────────────────────────────────────
            btnEquals = MakeButton("=",
                new Point(220, 350), new Size(100, 65), eqBg, textWhite);
            btnEquals.Click += BtnEquals_Click;
            this.Controls.Add(btnEquals);

            // ── 清空 C ────────────────────────────────────────
            btnClear = MakeButton("C",
                new Point(20, 430), new Size(300, 55), clrBg, textWhite);
            btnClear.Click += BtnClear_Click;
            this.Controls.Add(btnClear);

            // ── 运算符 + - × ÷（右列）────────────────────────
            string[] ops = { "+", "-", "×", "÷" };
            for (int i = 0; i < 4; i++)
            {
                Button btn = MakeButton(ops[i],
                    new Point(320, 110 + i * 80),
                    new Size(20, 65),   // 先占位，下面重设宽度
                    opBg, textWhite);

                // 右列按钮宽度固定
                btn.Location = new Point(320, 110 + i * 80);
                btn.Size = new Size(20, 65);

                // 重新设宽（FixedSingle 的话右边会被裁，改用下方代码）
                btn = MakeButton(ops[i],
                    new Point(315, 110 + i * 80),
                    new Size(25, 65),
                    opBg, textWhite);

                string capturedOp = ops[i];
                btn.Click += (s, e) => OpClick(capturedOp);
                this.Controls.Add(btn);

                switch (i)
                {
                    case 0: btnAdd = btn; break;
                    case 1: btnSubtract = btn; break;
                    case 2: btnMultiply = btn; break;
                    case 3: btnDivide = btn; break;
                }
            }

            // ── 重新布局（整洁版）────────────────────────────
            // 重新设置运算符按钮的位置和尺寸
            // 数字区：x=20~299，运算符区：x=310~339
            // 让运算符列与数字等高，宽度统一
            RebuildLayout(displayBg, numBg, opBg, eqBg, clrBg, textWhite);
        }

        // 最终整洁布局（覆盖上面的草稿逻辑）
        private void RebuildLayout(Color displayBg, Color numBg, Color opBg,
                                   Color eqBg, Color clrBg, Color textWhite)
        {
            // 清除旧控件
            this.Controls.Clear();

            // 窗体尺寸
            this.Size = new Size(380, 540);

            // ── 显示框 ────────────────────────────────────────
            txtDisplay = new TextBox();
            txtDisplay.Location = new Point(15, 15);
            txtDisplay.Size = new Size(340, 70);
            txtDisplay.Font = new Font("Segoe UI", 22, FontStyle.Bold);
            txtDisplay.TextAlign = HorizontalAlignment.Right;
            txtDisplay.ReadOnly = true;
            txtDisplay.BackColor = displayBg;
            txtDisplay.ForeColor = Color.White;
            txtDisplay.BorderStyle = BorderStyle.None;
            txtDisplay.Text = "0";
            this.Controls.Add(txtDisplay);

            // 按钮网格参数
            int btnW = 75;   // 数字/运算符按钮宽
            int btnH = 60;   // 按钮高
            int startX = 15;
            int startY = 100;
            int gap = 5;

            // ── 行1：7 8 9 ÷ ─────────────────────────────────
            // ── 行2：4 5 6 × ─────────────────────────────────
            // ── 行3：1 2 3 - ─────────────────────────────────
            // ── 行4：0 0 C + ─────────────────────────────────（0占两格，C占一格）
            // ── 行5：   =（横跨全部）─────────────────────────

            int[][] numRows = {
                new int[]{ 7, 8, 9 },
                new int[]{ 4, 5, 6 },
                new int[]{ 1, 2, 3 }
            };
            string[] opRow = { "÷", "×", "-", "+" };  // 对应行 0,1,2,3

            // 数字行（前3行）
            for (int row = 0; row < 3; row++)
            {
                for (int col = 0; col < 3; col++)
                {
                    int n = numRows[row][col];
                    int x = startX + col * (btnW + gap);
                    int y = startY + row * (btnH + gap);
                    btnNumbers[n] = MakeButton(n.ToString(),
                        new Point(x, y), new Size(btnW, btnH), numBg, Color.White);
                    int cap = n;
                    btnNumbers[n].Click += (s, e) => NumClick(cap.ToString());
                    this.Controls.Add(btnNumbers[n]);
                }

                // 运算符（右列，行0-2 对应 ÷ × -）
                Button opBtn = MakeButton(opRow[row],
                    new Point(startX + 3 * (btnW + gap), startY + row * (btnH + gap)),
                    new Size(btnW, btnH), opBg, Color.White);
                string capOp = opRow[row];
                opBtn.Click += (s, e) => OpClick(capOp);
                this.Controls.Add(opBtn);
                switch (row)
                {
                    case 0: btnDivide = opBtn; break;
                    case 1: btnMultiply = opBtn; break;
                    case 2: btnSubtract = opBtn; break;
                }
            }

            // 第4行：0（宽） C +
            int row4Y = startY + 3 * (btnH + gap);
            // 0 占两格
            btnNumbers[0] = MakeButton("0",
                new Point(startX, row4Y),
                new Size(btnW * 2 + gap, btnH), numBg, Color.White);
            btnNumbers[0].Click += (s, e) => NumClick("0");
            this.Controls.Add(btnNumbers[0]);

            // C
            btnClear = MakeButton("C",
                new Point(startX + 2 * (btnW + gap), row4Y),
                new Size(btnW, btnH), clrBg, Color.White);
            btnClear.Click += BtnClear_Click;
            this.Controls.Add(btnClear);

            // +
            btnAdd = MakeButton("+",
                new Point(startX + 3 * (btnW + gap), row4Y),
                new Size(btnW, btnH), opBg, Color.White);
            btnAdd.Click += (s, e) => OpClick("+");
            this.Controls.Add(btnAdd);

            // 第5行：= 横跨全部
            int row5Y = startY + 4 * (btnH + gap);
            btnEquals = MakeButton("=",
                new Point(startX, row5Y),
                new Size(btnW * 4 + gap * 3, btnH), eqBg, Color.White);
            btnEquals.Font = new Font("Segoe UI", 16, FontStyle.Bold);
            btnEquals.Click += BtnEquals_Click;
            this.Controls.Add(btnEquals);
        }

        // ── 创建按钮辅助方法 ──────────────────────────────────
        private Button MakeButton(string text, Point loc, Size size,
                                  Color bg, Color fg)
        {
            var btn = new Button();
            btn.Text = text;
            btn.Location = loc;
            btn.Size = size;
            btn.Font = new Font("Segoe UI", 14, FontStyle.Bold);
            btn.BackColor = bg;
            btn.ForeColor = fg;
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            btn.FlatAppearance.MouseOverBackColor =
                ControlPaint.Light(bg, 0.3f);
            btn.Cursor = Cursors.Hand;
            return btn;
        }

        // ══════════════════════════════════════════════════════
        //  事件处理
        // ══════════════════════════════════════════════════════

        // 数字按钮
        private void NumClick(string num)
        {
            if (resultShown)
            {
                currentInput = "";
                expression = "";
                resultShown = false;
            }
            if (operatorPressed)
            {
                currentInput = "";
                operatorPressed = false;
            }
            // 防止多个前导零
            if (currentInput == "0" && num == "0") return;
            currentInput += num;
            ShowDisplay(currentInput);
        }

        // 运算符按钮
        private void OpClick(string op)
        {
            if (currentInput == "" && expression == "") return;
            resultShown = false;

            if (!operatorPressed && currentInput != "")
            {
                firstNumber = double.Parse(currentInput);
                expression = firstNumber + " " + op + " ";
            }
            else
            {
                // 替换运算符
                int last = expression.LastIndexOf(' ', expression.Length - 2);
                if (last >= 0)
                    expression = expression.Substring(0, last + 1) + op + " ";
            }

            currentOperator = op;
            operatorPressed = true;
            ShowDisplay(expression.TrimEnd());
        }

        // 等号
        private void BtnEquals_Click(object sender, EventArgs e)
        {
            if (currentOperator == "" || currentInput == "" || operatorPressed)
                return;

            double second = double.Parse(currentInput);
            double result = 0;

            switch (currentOperator)
            {
                case "+": result = firstNumber + second; break;
                case "-": result = firstNumber - second; break;
                case "×": result = firstNumber * second; break;
                case "÷":
                    if (second == 0)
                    {
                        txtDisplay.Text = "除数不能为 0";
                        return;
                    }
                    result = firstNumber / second;
                    break;
            }

            // 去掉多余小数点：整数显示无小数
            string resultStr = (result == Math.Floor(result) && !double.IsInfinity(result))
                ? ((long)result).ToString()
                : result.ToString("G10");

            // 显示格式：18 + 5 = 23
            txtDisplay.Text = expression + second + " = " + resultStr;

            currentInput = resultStr;
            expression = "";
            currentOperator = "";
            operatorPressed = false;
            resultShown = true;
        }

        // 清空
        private void BtnClear_Click(object sender, EventArgs e)
        {
            currentInput = "";
            expression = "";
            firstNumber = 0;
            currentOperator = "";
            operatorPressed = false;
            resultShown = false;
            txtDisplay.Text = "0";
        }

        // 更新显示框
        private void ShowDisplay(string text)
        {
            txtDisplay.Text = (text == "" || text == null) ? "0" : text;
        }
    }
}