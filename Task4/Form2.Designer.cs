namespace Task4{
	partial class Form2{
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing){
			if (disposing && (components != null)) components.Dispose();
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code
		private void InitializeComponent()
		{
			panel1 = new System.Windows.Forms.Panel();
			startButton = new System.Windows.Forms.Button();
			stopButton = new System.Windows.Forms.Button();
			SuspendLayout();

			// panel1
			panel1.Location = new System.Drawing.Point(18, 62);
			panel1.Name = "panel1";
			panel1.Size = new System.Drawing.Size(776, 382);
			panel1.TabIndex = 5;


			// startButton
			startButton.Location = new System.Drawing.Point(6, 6);
			startButton.Name = "startButton";
			startButton.Size = new System.Drawing.Size(100, 50);
			startButton.TabIndex = 4;
			startButton.Text = "start";
			startButton.UseVisualStyleBackColor = true;
			startButton.Click += startButton_Click;


			// stopButton
			stopButton.Location = new System.Drawing.Point(106, 6);
			stopButton.Name = "stopButton";
			stopButton.Size = new System.Drawing.Size(100, 50);
			stopButton.TabIndex = 3;
			stopButton.Text = "stop";
			stopButton.UseVisualStyleBackColor = true;
			stopButton.Click += stopButton_Click;


			// Form2
			AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			ClientSize = new System.Drawing.Size(800, 450);
			Controls.Add(panel1);
			Controls.Add(startButton);
			Controls.Add(stopButton);
			Name = "Form2";
			Text = "Form2";
			ResumeLayout(false);
		}
		#endregion

		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Button startButton;
		private System.Windows.Forms.Button stopButton;
	}
}