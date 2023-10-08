using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Task4
{
	public partial class Form2 : Form
	{
		private bool Forward = true;
		static Timer myTimer = new Timer();
		private int k = 0;

		public Form2()
		{
			myTimer.Enabled = true;
			myTimer.Interval = 1;
			myTimer.Tick += new EventHandler(tick);
			myTimer.Start();
			InitializeComponent();
		}

		private void stopButton_Click(object sender, EventArgs e) => myTimer.Stop();
		private void startButton_Click(object sender, EventArgs e) => myTimer.Start();

		private void tick(object sender, EventArgs e)
		{
			Graphics g = panel1.CreateGraphics();
			Pen pen = new Pen(Color.Black, 3);
			int tem = k;
			g.Clear(Color.White);
			g.DrawRectangle(pen, 1, 1, k, k);

			if (k >= panel1.Height - 1)
				Forward = false;
			else if (k == 0)
				Forward = true;
			if (Forward)
				k++;
			else
				k--;

			g.Dispose();
			pen.Dispose();
		}
	}
}
