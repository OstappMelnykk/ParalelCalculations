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
	public partial class Form3 : Form
	{
		private double passed = 0;
		static Timer myTimer = new Timer();
		private double k = 0;

		public Form3()
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

			g.FillRectangle(Brushes.Black, (int)k, (int)(Math.Sin(passed / 15) * 20) + 150, 3, 3);

			if (k >= panel1.Width)
			{
				g.Clear(SystemColors.Control);
				k = 0;
			}
			passed++;
			k++;

			g.Dispose();
		}
	}
}
