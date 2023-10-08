namespace Task4{
	partial class Form1{
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing){
			if (disposing && (components != null)) components.Dispose();
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		private void InitializeComponent()
		{
			stopButton = new System.Windows.Forms.Button();
			startButton = new System.Windows.Forms.Button();
			panel1 = new System.Windows.Forms.Panel();
			SuspendLayout();
			// stopButton	
			stopButton.Location = new System.Drawing.Point(100, 0);
			stopButton.Name = "stopButton";
			stopButton.Size = new System.Drawing.Size(100, 50);
			stopButton.TabIndex = 0;
			stopButton.Text = "stop";
			stopButton.UseVisualStyleBackColor = true;
			stopButton.Click += stopButton_Click;
			// startButton
			startButton.Location = new System.Drawing.Point(0, 0);
			startButton.Name = "startButton";
			startButton.Size = new System.Drawing.Size(100, 50);
			startButton.TabIndex = 1;
			startButton.Text = "start";
			startButton.UseVisualStyleBackColor = true;
			startButton.Click += startButton_Click;
			// panel1
			panel1.Location = new System.Drawing.Point(12, 56);
			panel1.Name = "panel1";
			panel1.Size = new System.Drawing.Size(776, 382);
			panel1.TabIndex = 2;
			// Form1
			AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			ClientSize = new System.Drawing.Size(800, 450);
			Controls.Add(panel1);
			Controls.Add(startButton);
			Controls.Add(stopButton);
			Name = "Form1";
			Text = "Form1";
			ResumeLayout(false);
		}
		#endregion

		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Button startButton;
		private System.Windows.Forms.Button stopButton;
	}
}
