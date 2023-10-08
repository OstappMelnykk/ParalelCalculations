using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Task4{
	internal static class Program{
		[STAThread]
		static void Main()
		{
			Application.SetHighDpiMode(HighDpiMode.SystemAware);
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);

			Thread[] threads = new Thread[4] {
				new Thread(() => { Application.Run(new Form1()); }),
				new Thread(() => { Application.Run(new Form2()); }),
				new Thread(() => { Application.Run(new Form3()); }),
				new Thread(() => { Application.Run(new Form4()); })
			};

			foreach (var x in threads)
				x.Start();
			foreach (var x in threads)
				x.Join();
		}
	}
}
