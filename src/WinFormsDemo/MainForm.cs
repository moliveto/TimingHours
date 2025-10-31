using Application.Work.Timing;
using Domain.Work.Timing;

namespace WinFormsDemo
{
    public partial class MainForm : Form
    {
        private TimingBarLayout _layout = null!;
        private TimingInfoHours _timing = null!;

        public MainForm()
        {
            InitializeComponent();
            DoubleBuffered = true;

            _timing = TimingInfoHours.CreateAuto(
                timeInit: 1.0, timeInitTR: 0.2,
                timeWork: 5.0, timeWorkWK: 3.5, timeWorkTR: 0.8,
                timeEnd: 0.5, timeEndTR: 0.1);

            RecalculateLayout();
            Resize += (_, __) => { RecalculateLayout(); Invalidate(); };
        }

        private void RecalculateLayout()
        {
            var totalHeight = Math.Max(100, ClientSize.Height - 140);
            _layout = TimingBarCalculator.Calculate(_timing, totalHeightPx: totalHeight, minBandPx: 0);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            var g = e.Graphics;
            g.Clear(Color.White);

            var barX = 100;
            var barW = 80;
            var topY = 80;

            DrawTitle(g);
            DrawInitBlock(g, barX, topY, barW);
            DrawWorkBlock(g, barX, topY, barW);
            DrawEndBlock(g, barX, topY, barW);
            DrawBrackets(g, barX, topY);
            DrawLegend(g, barX + 200, topY + _layout.TotalHeightPx - 80);
        }

        private static void DrawTitle(Graphics g)
        {
            using var f = new Font("Segoe UI", 12, FontStyle.Bold);
            g.DrawString("Timing Bar – WinForms Demo", f, Brushes.Black, 16, 16);
        }

        private void DrawInitBlock(Graphics g, int x, int topY, int w)
        {
            var y = topY + _layout.InitTopY;
            using var pen = new Pen(Color.Black);
            using var red = new SolidBrush(Color.FromArgb(170, 0, 0));
            using var yellow = new SolidBrush(Color.FromArgb(230, 200, 40));

            g.FillRectangle(red, x, y, w, _layout.InitTotalPx);
            g.FillRectangle(yellow, x, y, w, _layout.InitTrPx);
            g.DrawRectangle(pen, x, y, w, _layout.InitTotalPx);
        }

        private void DrawWorkBlock(Graphics g, int x, int topY, int w)
        {
            var y = topY + _layout.WorkTopY;
            using var pen = new Pen(Color.Black);
            using var red = new SolidBrush(Color.FromArgb(170, 0, 0));
            using var yellow = new SolidBrush(Color.FromArgb(230, 200, 40));
            using var green = new SolidBrush(Color.FromArgb(40, 140, 40));
            using var whiteBrush = new SolidBrush(Color.White);
            using var blackBrush = new SolidBrush(Color.Black);
            using var font = new Font("Segoe UI", 9, FontStyle.Bold);

            g.FillRectangle(red, x, y, w, _layout.WorkTotalPx);
            g.FillRectangle(green, x, y, w, _layout.WorkWkPx);
            g.FillRectangle(yellow, x, y + _layout.WorkWkPx, w, _layout.WorkTrPx);
            g.DrawRectangle(pen, x, y, w, _layout.WorkTotalPx);

            if (_layout.WorkWkPx >= 14)
                DrawCenteredText(g, $"{_timing.TimeWorkWK:0.0} h", font, whiteBrush, x, y, w, _layout.WorkWkPx);
            if (_layout.WorkTrPx >= 14)
                DrawCenteredText(g, $"{_timing.TimeWorkTR:0.0} h", font, blackBrush, x, y + _layout.WorkWkPx, w, _layout.WorkTrPx);
            if (_layout.WorkSpPx >= 14)
                DrawCenteredText(g, $"{_timing.TimeWorkSP:0.0} h", font, whiteBrush, x, y + _layout.WorkWkPx + _layout.WorkTrPx, w, _layout.WorkSpPx);
        }

        private void DrawEndBlock(Graphics g, int x, int topY, int w)
        {
            var y = topY + _layout.EndTopY;
            using var pen = new Pen(Color.Black);
            using var red = new SolidBrush(Color.FromArgb(170, 0, 0));
            using var yellow = new SolidBrush(Color.FromArgb(230, 200, 40));

            g.FillRectangle(red, x, y, w, _layout.EndTotalPx);
            g.FillRectangle(yellow, x, y, w, _layout.EndTrPx);
            g.DrawRectangle(pen, x, y, w, _layout.EndTotalPx);
        }

        private static void DrawCenteredText(Graphics g, string text, Font font, Brush brush, int x, int y, int w, int h)
        {
            var size = g.MeasureString(text, font);
            var tx = x + (w - size.Width) / 2f;
            var ty = y + (h - size.Height) / 2f;
            g.DrawString(text, font, brush, tx, ty);
        }

        private void DrawBrackets(Graphics g, int barX, int topY)
        {
            using var gray = new Pen(Color.Gray, 2);
            using var blue = new SolidBrush(Color.FromArgb(10, 80, 200));
            using var font = new Font("Segoe UI", 9, FontStyle.Bold);

            // Register total
            var x = barX - 20;
            var y1 = topY + _layout.RegisterBracketTop;
            var y2 = topY + _layout.RegisterBracketBottom;
            g.DrawLine(gray, x, y1, x, y2);
            g.DrawLine(gray, x - 10, y1, x, y1);
            g.DrawLine(gray, x - 10, y2, x, y2);
            var txt = $"{_timing.TimeTotalReg:0.0} hs";
            var sz = g.MeasureString(txt, font);
            g.DrawString(txt, font, blue, x - 12 - sz.Width, y1 + (y2 - y1 - sz.Height) / 2f);

            // Work total
            x = barX - 8;
            y1 = topY + _layout.WorkBracketTop;
            y2 = topY + _layout.WorkBracketBottom;
            g.DrawLine(gray, x, y1, x, y2);
            g.DrawLine(gray, x - 8, y1, x, y1);
            g.DrawLine(gray, x - 8, y2, x, y2);
            txt = $"{_timing.TimeWork:0.0} hs";
            sz = g.MeasureString(txt, font);
            g.DrawString(txt, font, blue, x - 10 - sz.Width, y1 + (y2 - y1 - sz.Height) / 2f);
        }

        private static void DrawLegend(Graphics g, int x, int y)
        {
            using var pen = new Pen(Color.Black);
            using var green = new SolidBrush(Color.FromArgb(40, 140, 40));
            using var yellow = new SolidBrush(Color.FromArgb(230, 200, 40));
            using var red = new SolidBrush(Color.FromArgb(170, 0, 0));
            using var font = new Font("Segoe UI", 9, FontStyle.Regular);

            var box = 16;
            var gap = 8;

            g.FillRectangle(green, x, y, box, box);
            g.DrawRectangle(pen, x, y, box, box);
            g.DrawString("WK (Trabajo efectivo)", font, Brushes.Black, x + box + 8, y - 1);

            y += box + gap;
            g.FillRectangle(yellow, x, y, box, box);
            g.DrawRectangle(pen, x, y, box, box);
            g.DrawString("TR (Transporte)", font, Brushes.Black, x + box + 8, y - 1);

            y += box + gap;
            g.FillRectangle(red, x, y, box, box);
            g.DrawRectangle(pen, x, y, box, box);
            g.DrawString("SP (Parado)", font, Brushes.Black, x + box + 8, y - 1);
        }
    }
}
