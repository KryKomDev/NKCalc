using NeoKolors.Common;
using NeoKolors.Console;
using NeoKolors.Console.Ansi.Mouse;
using NeoKolors.Tui;
using NeoKolors.Tui.Dom;
using NeoKolors.Tui.Elements;
using NeoKolors.Tui.Events;
using NeoKolors.Tui.Extensions;
using NeoKolors.Tui.Fonts;
using NeoKolors.Tui.Styles;
using NeoKolors.Tui.Styles.Properties;
using NeoKolors.Tui.Styles.Values;

namespace NKCalc;

class Program {
    static void Main(string[] args) {
        
        NKDebug.RedirectFatalToLog    = true;
        NKDebug.Logger.SimpleMessages = true;
        NKDebug.Logger.FileConfig     = LogFileConfig.Replace("/home/kokulan/RiderProjects/NKCalc/NKCalc/calc.log");
        
        var b0   = new Button("0") { Style  = new StyleCollection { GridAlign = new Rectangle(0, 5, 0, 5) } };
        var b1   = new Button("1") { Style  = new StyleCollection { GridAlign = new Rectangle(0, 4, 0, 4) } };
        var b2   = new Button("2") { Style  = new StyleCollection { GridAlign = new Rectangle(1, 4, 1, 4) } };
        var b3   = new Button("3") { Style  = new StyleCollection { GridAlign = new Rectangle(2, 4, 2, 4) } };
        var b4   = new Button("4") { Style  = new StyleCollection { GridAlign = new Rectangle(0, 3, 0, 3) } };
        var b5   = new Button("5") { Style  = new StyleCollection { GridAlign = new Rectangle(1, 3, 1, 3) } };
        var b6   = new Button("6") { Style  = new StyleCollection { GridAlign = new Rectangle(2, 3, 2, 3) } };
        var b7   = new Button("7") { Style  = new StyleCollection { GridAlign = new Rectangle(0, 2, 0, 2) } };
        var b8   = new Button("8") { Style  = new StyleCollection { GridAlign = new Rectangle(1, 2, 1, 2) } };
        var b9   = new Button("9") { Style  = new StyleCollection { GridAlign = new Rectangle(2, 2, 2, 2) } };
        var bDot = new Button(".") { Style  = new StyleCollection { GridAlign = new Rectangle(1, 5, 1, 5) } };
        var bDel = new Button("DEL") { Style = new StyleCollection { GridAlign = new Rectangle(2, 5, 2, 5) } };
        var bAc  = new Button("AC") { Style = new StyleCollection { GridAlign = new Rectangle(0, 1, 0, 1) } };
        var bBr  = new Button("()") { Style = new StyleCollection { GridAlign = new Rectangle(1, 1, 1, 1) } };
        var bPrc = new Button("%") { Style  = new StyleCollection { GridAlign = new Rectangle(2, 1, 2, 1) } };
        var bDiv = new Button("/") { Style  = new StyleCollection { GridAlign = new Rectangle(3, 1, 3, 1) } };
        var bMul = new Button("x") { Style  = new StyleCollection { GridAlign = new Rectangle(3, 2, 3, 2) } };
        var bSub = new Button("-") { Style  = new StyleCollection { GridAlign = new Rectangle(3, 3, 3, 3) } };
        var bAdd = new Button("+") { Style  = new StyleCollection { GridAlign = new Rectangle(3, 4, 3, 4) } };
        var bEq  = new Button("=") { 
            Style  = new StyleCollection {
                GridAlign = new Rectangle(3, 5, 3, 5),
                BackgroundColor = NKConsoleColor.RED
            },
            Info = { Id = "eq" }
        };
        
        string s = "";
        
        var eq = new Text(s) {
            Style = new StyleCollection {
                GridAlign       = new Rectangle(0, 0, 3, 0),
                BackgroundColor = NKConsoleColor.BLACK,
                Font            = FontAtlas.Get("Future"),
                TextAlign       = new Align(HorizontalAlign.RIGHT, VerticalAlign.CENTER),
                Width           = Dimension.Stretch,
                Height          = Dimension.Stretch,
                PaddingRight    = 2.Ch
            }
        };
        
        void UpdateDisplay() {
            eq.Content = s;
        }

        void Append(string token) {
            s += token;
            UpdateDisplay();
        }

        void Clear() {
            s = "";
            UpdateDisplay();
        }

        void Delete() {
            if (s.Length > 0) {
                s = s.Substring(0, s.Length - 1);
                UpdateDisplay();
            }
        }

        void AddBrackets() {
            int openCount = s.Count(c => c == '(');
            int closeCount = s.Count(c => c == ')');
            
            if (s.Length > 0) {
                char last = s[^1];
                if (openCount > closeCount && (char.IsDigit(last) || last == ')')) {
                    s += ")";
                } else {
                    s += "(";
                }
            } else {
                s += "(";
            }
            UpdateDisplay();
        }

        void Evaluate() {
            if (string.IsNullOrEmpty(s)) return;
            try {
                double result = new Evaluator().Parse(s);
                s = result.ToString(System.Globalization.CultureInfo.InvariantCulture);
                UpdateDisplay();
            } catch {
                eq.Content = "Error";
                s = "";
            }
        }

        b0.OnClick   += _ => Append("0");
        b1.OnClick   += _ => Append("1");
        b2.OnClick   += _ => Append("2");
        b3.OnClick   += _ => Append("3");
        b4.OnClick   += _ => Append("4");
        b5.OnClick   += _ => Append("5");
        b6.OnClick   += _ => Append("6");
        b7.OnClick   += _ => Append("7");
        b8.OnClick   += _ => Append("8");
        b9.OnClick   += _ => Append("9");
        bDot.OnClick += _ => Append(".");
        bAdd.OnClick += _ => Append("+");
        bSub.OnClick += _ => Append("-");
        bMul.OnClick += _ => Append("x");
        bDiv.OnClick += _ => Append("/");
        bPrc.OnClick += _ => Append("%");
        bAc.OnClick  += _ => Clear();
        bDel.OnClick += _ => Delete();
        bBr.OnClick  += _ => AddBrackets();
        bEq.OnClick  += _ => Evaluate();

        var grid = new Div(b0, b1, b2, b3, b4, b5, b6, b7, b8, b9, bDot, bDel, bAc, bBr, bPrc, bDiv, bMul, bSub, bAdd, bEq, eq) {
            Style = new StyleCollection {
                Grid = new GridDimensions(
                    [25.Vw, 25.Vw, 25.Vw, Dimension.Auto], 
                    [30.Vh, 14.Vh, 14.Vh, 14.Vh, 14.Vh, Dimension.Auto]
                ),
                Width = Dimension.Stretch,
                EnableGrid = true,
            }
        };
        
        var body = new Body(grid);

        var dom = new NKDom(body);
        dom.GetElementsByType(typeof(Button)).Apply(
            new MarginProperty(1.Ch),
            new FontProperty(FontAtlas.Get("Future")),
            new TextAlignProperty(Align.Center),
            new WidthProperty(Dimension.Stretch),
            new HeightProperty(Dimension.Stretch)
        );

        dom.GetElementsByType(typeof(Button)).Where(e => e.Info.Id != "eq").Apply(
            new BackgroundColorProperty(NKConsoleColor.DARK_GRAY)
        );

        var app = new NKApplication(
            new NKAppConfig(
                RenderingConfig.Limited(60),
                mouseReportLevel: MouseReportLevel.ALL,
                mouseReportProtocol: MouseReportProtocol.SGR
            ),
            body
        );

        AppEventBus.SetSourceApplication(app);

        app.KeyEvent += k => {
            if (k.Key == KeyCode.BACKSPACE) {
                Delete();
            } 
            else if (k.Key == KeyCode.ENTER || k.Char == '=') {
                Evaluate();
            }
            else if (k.Key == KeyCode.ESCAPE) {
                Clear();
            }
            else if (k.Char != '\0') {
                char c = k.Char;
                if (char.IsDigit(c) || "+-/%().".Contains(c)) {
                    Append(c.ToString());
                } 
                else if (c is '*' or 'x' or 'X') {
                    Append("x");
                }
            }
        };
        
        app.Start();
    }

    private class Evaluator {
        private string _expression = "";
        private int _pos = -1;
        private int _ch;

        private void NextChar() {
            _ch = (++_pos < _expression.Length) ? _expression[_pos] : -1;
        }

        private bool Eat(int charToEat) {
            while (_ch == ' ') NextChar();
            if (_ch == charToEat) {
                NextChar();
                return true;
            }
            return false;
        }

        public double Parse(string str) {
            _expression = str.Replace("x", "*");
            _pos = -1;
            NextChar();
            double x = ParseExpression();
            if (_pos < _expression.Length) throw new Exception("Unexpected: " + (char)_ch);
            return x;
        }

        private double ParseExpression() {
            double x = ParseTerm();
            for (;;) {
                if      (Eat('+')) x += ParseTerm();
                else if (Eat('-')) x -= ParseTerm();
                else return x;
            }
        }

        private double ParseTerm() {
            double x = ParseFactor();
            for (;;) {
                if      (Eat('*')) x *= ParseFactor();
                else if (Eat('/')) x /= ParseFactor();
                else if (Eat('%')) x %= ParseFactor();
                else return x;
            }
        }

        private double ParseFactor() {
            if (Eat('+')) return ParseFactor();
            if (Eat('-')) return -ParseFactor();

            double x;
            int startPos = _pos;
            if (Eat('(')) {
                x = ParseExpression();
                if (!Eat(')')) throw new Exception("Missing ')'");
            } else if (_ch is >= '0' and <= '9' or '.') {
                while (_ch is >= '0' and <= '9' or '.') NextChar();
                x = double.Parse(_expression.Substring(startPos, _pos - startPos), System.Globalization.CultureInfo.InvariantCulture);
            } else {
                throw new Exception("Unexpected: " + (char)_ch);
            }

            if (Eat('%')) x /= 100.0;

            return x;
        }
    }
}