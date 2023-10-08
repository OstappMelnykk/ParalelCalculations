using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Task4
{
	public partial class Form4 : Form
	{
		private Timer myTimer = new Timer();
		private float angle = 0.0f;

		public Form4()
		{
			myTimer.Interval = 1;
			myTimer.Tick += new EventHandler(tick);
			myTimer.Start();
			InitializeComponent();
		}

		private void stopButton_Click(object sender, EventArgs e) => myTimer.Stop();
		private void startButton_Click(object sender, EventArgs e) => myTimer.Start();

		private void tick(object sender, EventArgs e)
		{
			angle = (angle + 1.0f) % 360.0f;

			Graphics g = panel1.CreateGraphics();
			g.Clear(Color.White);

			float size = 200, x = panel1.Width / 2, y = panel1.Height / 2;

			Matrix rotationMatrix = new Matrix();
			rotationMatrix.RotateAt(angle, new PointF(x, y));
			g.Transform = rotationMatrix;
			g.DrawRectangle(new Pen(Color.Black, 3), x - size / 2, y - size / 2, size, size);
			g.Dispose();
		}
	}
}
