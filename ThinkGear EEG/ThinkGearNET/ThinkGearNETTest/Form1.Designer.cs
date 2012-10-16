namespace ThinkGearNETTest
{
	partial class Form1
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.cboPort = new System.Windows.Forms.ComboBox();
			this.btnConnect = new System.Windows.Forms.Button();
			this.txtState = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.btnDisconnect = new System.Windows.Forms.Button();
			this.lblAttention = new System.Windows.Forms.Label();
			this.lblMeditation = new System.Windows.Forms.Label();
			this.btnEnableBlink = new System.Windows.Forms.Button();
			this.btnDisableBlink = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// cboPort
			// 
			this.cboPort.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cboPort.FormattingEnabled = true;
			this.cboPort.Location = new System.Drawing.Point(64, 8);
			this.cboPort.Name = "cboPort";
			this.cboPort.Size = new System.Drawing.Size(121, 21);
			this.cboPort.TabIndex = 0;
			// 
			// btnConnect
			// 
			this.btnConnect.Location = new System.Drawing.Point(18, 36);
			this.btnConnect.Name = "btnConnect";
			this.btnConnect.Size = new System.Drawing.Size(84, 23);
			this.btnConnect.TabIndex = 1;
			this.btnConnect.Text = "Connect";
			this.btnConnect.UseVisualStyleBackColor = true;
			this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
			// 
			// txtState
			// 
			this.txtState.Location = new System.Drawing.Point(8, 136);
			this.txtState.Multiline = true;
			this.txtState.Name = "txtState";
			this.txtState.Size = new System.Drawing.Size(204, 244);
			this.txtState.TabIndex = 2;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(8, 12);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(56, 13);
			this.label1.TabIndex = 3;
			this.label1.Text = "COM Port:";
			// 
			// btnDisconnect
			// 
			this.btnDisconnect.Location = new System.Drawing.Point(114, 36);
			this.btnDisconnect.Name = "btnDisconnect";
			this.btnDisconnect.Size = new System.Drawing.Size(84, 23);
			this.btnDisconnect.TabIndex = 4;
			this.btnDisconnect.Text = "Disconnect";
			this.btnDisconnect.UseVisualStyleBackColor = true;
			this.btnDisconnect.Click += new System.EventHandler(this.btnDisconnect_Click);
			// 
			// lblAttention
			// 
			this.lblAttention.AutoSize = true;
			this.lblAttention.Location = new System.Drawing.Point(12, 92);
			this.lblAttention.Name = "lblAttention";
			this.lblAttention.Size = new System.Drawing.Size(49, 13);
			this.lblAttention.TabIndex = 5;
			this.lblAttention.Text = "Attention";
			// 
			// lblMeditation
			// 
			this.lblMeditation.AutoSize = true;
			this.lblMeditation.Location = new System.Drawing.Point(12, 112);
			this.lblMeditation.Name = "lblMeditation";
			this.lblMeditation.Size = new System.Drawing.Size(56, 13);
			this.lblMeditation.TabIndex = 5;
			this.lblMeditation.Text = "Meditation";
			// 
			// btnEnableBlink
			// 
			this.btnEnableBlink.Location = new System.Drawing.Point(18, 60);
			this.btnEnableBlink.Name = "btnEnableBlink";
			this.btnEnableBlink.Size = new System.Drawing.Size(84, 23);
			this.btnEnableBlink.TabIndex = 6;
			this.btnEnableBlink.Text = "Enable Blink";
			this.btnEnableBlink.UseVisualStyleBackColor = true;
			this.btnEnableBlink.Click += new System.EventHandler(this.btnEnableBlink_Click);
			// 
			// btnDisableBlink
			// 
			this.btnDisableBlink.Location = new System.Drawing.Point(114, 60);
			this.btnDisableBlink.Name = "btnDisableBlink";
			this.btnDisableBlink.Size = new System.Drawing.Size(84, 23);
			this.btnDisableBlink.TabIndex = 7;
			this.btnDisableBlink.Text = "Disable Blink";
			this.btnDisableBlink.UseVisualStyleBackColor = true;
			this.btnDisableBlink.Click += new System.EventHandler(this.btnDisableBlink_Click);
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(217, 384);
			this.Controls.Add(this.btnDisableBlink);
			this.Controls.Add(this.btnEnableBlink);
			this.Controls.Add(this.lblMeditation);
			this.Controls.Add(this.lblAttention);
			this.Controls.Add(this.btnDisconnect);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.txtState);
			this.Controls.Add(this.btnConnect);
			this.Controls.Add(this.cboPort);
			this.MaximizeBox = false;
			this.Name = "Form1";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "ThinkGearNET Test";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
			this.Load += new System.EventHandler(this.Form1_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.ComboBox cboPort;
		private System.Windows.Forms.Button btnConnect;
		private System.Windows.Forms.TextBox txtState;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button btnDisconnect;
		private System.Windows.Forms.Label lblAttention;
		private System.Windows.Forms.Label lblMeditation;
		private System.Windows.Forms.Button btnEnableBlink;
		private System.Windows.Forms.Button btnDisableBlink;
	}
}

