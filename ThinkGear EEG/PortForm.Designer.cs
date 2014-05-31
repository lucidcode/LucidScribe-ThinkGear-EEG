namespace lucidcode.LucidScribe.Plugin.NeuroSky.MindSet
{
  partial class PortForm
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
      this.components = new System.ComponentModel.Container();
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PortForm));
      this.pnlPlugins = new lucidcode.Controls.Panel3D();
      this.lstPorts = new System.Windows.Forms.ListView();
      this.mnuPortsList = new System.Windows.Forms.ContextMenuStrip(this.components);
      this.mnuRefreshPorts = new System.Windows.Forms.ToolStripMenuItem();
      this.lstImg = new System.Windows.Forms.ImageList(this.components);
      this.Panel3D4 = new lucidcode.Controls.Panel3D();
      this.Label5 = new System.Windows.Forms.Label();
      this.Label6 = new System.Windows.Forms.Label();
      this.panel3D3 = new lucidcode.Controls.Panel3D();
      this.txtArduinoOff = new System.Windows.Forms.TextBox();
      this.cmbArduinoPort = new System.Windows.Forms.ComboBox();
      this.txtArduinoOn = new System.Windows.Forms.TextBox();
      this.chkArduino = new System.Windows.Forms.CheckBox();
      this.cmbStateOff = new System.Windows.Forms.ComboBox();
      this.cmbStateOn = new System.Windows.Forms.ComboBox();
      this.txtTarget = new System.Windows.Forms.TextBox();
      this.chktACS = new System.Windows.Forms.CheckBox();
      this.btnBrowse = new System.Windows.Forms.Button();
      this.txtVideo = new System.Windows.Forms.TextBox();
      this.chkNZT48 = new System.Windows.Forms.CheckBox();
      this.chkTCMP = new System.Windows.Forms.CheckBox();
      this.label8 = new System.Windows.Forms.Label();
      this.cmbAlgorithm = new System.Windows.Forms.ComboBox();
      this.label2 = new System.Windows.Forms.Label();
      this.panel3D5 = new lucidcode.Controls.Panel3D();
      this.label3 = new System.Windows.Forms.Label();
      this.label4 = new System.Windows.Forms.Label();
      this.cmbThreshold = new System.Windows.Forms.ComboBox();
      this.cmbArduinoDelay = new System.Windows.Forms.ComboBox();
      this.pnlPlugins.SuspendLayout();
      this.mnuPortsList.SuspendLayout();
      this.Panel3D4.SuspendLayout();
      this.panel3D3.SuspendLayout();
      this.panel3D5.SuspendLayout();
      this.SuspendLayout();
      // 
      // pnlPlugins
      // 
      this.pnlPlugins.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.pnlPlugins.BackColor = System.Drawing.Color.White;
      this.pnlPlugins.Controls.Add(this.lstPorts);
      this.pnlPlugins.Controls.Add(this.Panel3D4);
      this.pnlPlugins.Location = new System.Drawing.Point(12, 12);
      this.pnlPlugins.Name = "pnlPlugins";
      this.pnlPlugins.Size = new System.Drawing.Size(359, 246);
      this.pnlPlugins.TabIndex = 5;
      // 
      // lstPorts
      // 
      this.lstPorts.Activation = System.Windows.Forms.ItemActivation.OneClick;
      this.lstPorts.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.lstPorts.BorderStyle = System.Windows.Forms.BorderStyle.None;
      this.lstPorts.ContextMenuStrip = this.mnuPortsList;
      this.lstPorts.LargeImageList = this.lstImg;
      this.lstPorts.Location = new System.Drawing.Point(3, 25);
      this.lstPorts.MultiSelect = false;
      this.lstPorts.Name = "lstPorts";
      this.lstPorts.Size = new System.Drawing.Size(353, 218);
      this.lstPorts.TabIndex = 8;
      this.lstPorts.TileSize = new System.Drawing.Size(150, 32);
      this.lstPorts.UseCompatibleStateImageBehavior = false;
      this.lstPorts.View = System.Windows.Forms.View.Tile;
      this.lstPorts.SelectedIndexChanged += new System.EventHandler(this.lstPlaylists_SelectedIndexChanged);
      this.lstPorts.MouseMove += new System.Windows.Forms.MouseEventHandler(this.lstPlaylists_MouseMove);
      // 
      // mnuPortsList
      // 
      this.mnuPortsList.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuRefreshPorts});
      this.mnuPortsList.Name = "contextMenuStrip1";
      this.mnuPortsList.Size = new System.Drawing.Size(114, 26);
      // 
      // mnuRefreshPorts
      // 
      this.mnuRefreshPorts.Name = "mnuRefreshPorts";
      this.mnuRefreshPorts.Size = new System.Drawing.Size(113, 22);
      this.mnuRefreshPorts.Text = "&Refresh";
      this.mnuRefreshPorts.Click += new System.EventHandler(this.mnuRefresh_Click);
      // 
      // lstImg
      // 
      this.lstImg.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("lstImg.ImageStream")));
      this.lstImg.TransparentColor = System.Drawing.Color.Transparent;
      this.lstImg.Images.SetKeyName(0, "Graph.Plugin2.bmp");
      // 
      // Panel3D4
      // 
      this.Panel3D4.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.Panel3D4.BackColor = System.Drawing.Color.SteelBlue;
      this.Panel3D4.Controls.Add(this.Label5);
      this.Panel3D4.Controls.Add(this.Label6);
      this.Panel3D4.Location = new System.Drawing.Point(0, 0);
      this.Panel3D4.Name = "Panel3D4";
      this.Panel3D4.Size = new System.Drawing.Size(359, 24);
      this.Panel3D4.TabIndex = 4;
      // 
      // Label5
      // 
      this.Label5.Font = new System.Drawing.Font("Verdana", 10F, System.Drawing.FontStyle.Bold);
      this.Label5.ForeColor = System.Drawing.Color.White;
      this.Label5.Image = ((System.Drawing.Image)(resources.GetObject("Label5.Image")));
      this.Label5.Location = new System.Drawing.Point(3, 3);
      this.Label5.Name = "Label5";
      this.Label5.Size = new System.Drawing.Size(19, 19);
      this.Label5.TabIndex = 4;
      this.Label5.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      // 
      // Label6
      // 
      this.Label6.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.Label6.Font = new System.Drawing.Font("Verdana", 8F, System.Drawing.FontStyle.Bold);
      this.Label6.ForeColor = System.Drawing.Color.White;
      this.Label6.Location = new System.Drawing.Point(24, 3);
      this.Label6.Name = "Label6";
      this.Label6.Size = new System.Drawing.Size(332, 19);
      this.Label6.TabIndex = 3;
      this.Label6.Text = "Select port to connect";
      this.Label6.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      // 
      // panel3D3
      // 
      this.panel3D3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.panel3D3.BackColor = System.Drawing.Color.LightSteelBlue;
      this.panel3D3.Controls.Add(this.cmbArduinoDelay);
      this.panel3D3.Controls.Add(this.txtArduinoOff);
      this.panel3D3.Controls.Add(this.cmbArduinoPort);
      this.panel3D3.Controls.Add(this.txtArduinoOn);
      this.panel3D3.Controls.Add(this.chkArduino);
      this.panel3D3.Controls.Add(this.cmbStateOff);
      this.panel3D3.Controls.Add(this.cmbStateOn);
      this.panel3D3.Controls.Add(this.txtTarget);
      this.panel3D3.Controls.Add(this.chktACS);
      this.panel3D3.Controls.Add(this.btnBrowse);
      this.panel3D3.Controls.Add(this.txtVideo);
      this.panel3D3.Controls.Add(this.chkNZT48);
      this.panel3D3.Controls.Add(this.chkTCMP);
      this.panel3D3.Controls.Add(this.label8);
      this.panel3D3.Controls.Add(this.cmbAlgorithm);
      this.panel3D3.Controls.Add(this.label2);
      this.panel3D3.Controls.Add(this.panel3D5);
      this.panel3D3.Controls.Add(this.cmbThreshold);
      this.panel3D3.Location = new System.Drawing.Point(12, 264);
      this.panel3D3.Name = "panel3D3";
      this.panel3D3.Size = new System.Drawing.Size(359, 195);
      this.panel3D3.TabIndex = 37;
      // 
      // txtArduinoOff
      // 
      this.txtArduinoOff.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.txtArduinoOff.Location = new System.Drawing.Point(309, 138);
      this.txtArduinoOff.Name = "txtArduinoOff";
      this.txtArduinoOff.Size = new System.Drawing.Size(44, 21);
      this.txtArduinoOff.TabIndex = 294;
      this.txtArduinoOff.Text = "0";
      this.txtArduinoOff.TextChanged += new System.EventHandler(this.txtArduinoOff_TextChanged);
      // 
      // cmbArduinoPort
      // 
      this.cmbArduinoPort.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.cmbArduinoPort.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.cmbArduinoPort.FormattingEnabled = true;
      this.cmbArduinoPort.Items.AddRange(new object[] {
            "A",
            "B"});
      this.cmbArduinoPort.Location = new System.Drawing.Point(99, 138);
      this.cmbArduinoPort.Name = "cmbArduinoPort";
      this.cmbArduinoPort.Size = new System.Drawing.Size(74, 21);
      this.cmbArduinoPort.TabIndex = 293;
      this.cmbArduinoPort.SelectedIndexChanged += new System.EventHandler(this.cmbArduinoPort_SelectedIndexChanged);
      // 
      // txtArduinoOn
      // 
      this.txtArduinoOn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.txtArduinoOn.Location = new System.Drawing.Point(259, 138);
      this.txtArduinoOn.Name = "txtArduinoOn";
      this.txtArduinoOn.Size = new System.Drawing.Size(44, 21);
      this.txtArduinoOn.TabIndex = 292;
      this.txtArduinoOn.Text = "1";
      this.txtArduinoOn.TextChanged += new System.EventHandler(this.txtArduinoOn_TextChanged);
      // 
      // chkArduino
      // 
      this.chkArduino.Location = new System.Drawing.Point(9, 140);
      this.chkArduino.Name = "chkArduino";
      this.chkArduino.Size = new System.Drawing.Size(88, 17);
      this.chkArduino.TabIndex = 291;
      this.chkArduino.Text = "Arduino";
      this.chkArduino.UseVisualStyleBackColor = true;
      this.chkArduino.CheckedChanged += new System.EventHandler(this.chkArduino_CheckedChanged);
      // 
      // cmbStateOff
      // 
      this.cmbStateOff.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.cmbStateOff.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.cmbStateOff.FormattingEnabled = true;
      this.cmbStateOff.Items.AddRange(new object[] {
            "A",
            "B"});
      this.cmbStateOff.Location = new System.Drawing.Point(309, 112);
      this.cmbStateOff.Name = "cmbStateOff";
      this.cmbStateOff.Size = new System.Drawing.Size(44, 21);
      this.cmbStateOff.TabIndex = 290;
      this.cmbStateOff.SelectedIndexChanged += new System.EventHandler(this.cmbStateOff_SelectedIndexChanged);
      // 
      // cmbStateOn
      // 
      this.cmbStateOn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.cmbStateOn.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.cmbStateOn.FormattingEnabled = true;
      this.cmbStateOn.Items.AddRange(new object[] {
            "A",
            "B"});
      this.cmbStateOn.Location = new System.Drawing.Point(259, 111);
      this.cmbStateOn.Name = "cmbStateOn";
      this.cmbStateOn.Size = new System.Drawing.Size(44, 21);
      this.cmbStateOn.TabIndex = 289;
      this.cmbStateOn.SelectedIndexChanged += new System.EventHandler(this.cmbStateOn_SelectedIndexChanged);
      // 
      // txtTarget
      // 
      this.txtTarget.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.txtTarget.Location = new System.Drawing.Point(99, 111);
      this.txtTarget.Name = "txtTarget";
      this.txtTarget.Size = new System.Drawing.Size(154, 21);
      this.txtTarget.TabIndex = 288;
      this.txtTarget.Text = "ANY";
      this.txtTarget.TextChanged += new System.EventHandler(this.txtTarget_TextChanged);
      // 
      // chktACS
      // 
      this.chktACS.Location = new System.Drawing.Point(9, 113);
      this.chktACS.Name = "chktACS";
      this.chktACS.Size = new System.Drawing.Size(88, 17);
      this.chktACS.TabIndex = 287;
      this.chktACS.Text = "tACS";
      this.chktACS.UseVisualStyleBackColor = true;
      this.chktACS.CheckedChanged += new System.EventHandler(this.chktACS_CheckedChanged);
      // 
      // btnBrowse
      // 
      this.btnBrowse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.btnBrowse.Location = new System.Drawing.Point(323, 83);
      this.btnBrowse.Name = "btnBrowse";
      this.btnBrowse.Size = new System.Drawing.Size(30, 23);
      this.btnBrowse.TabIndex = 286;
      this.btnBrowse.Text = "...";
      this.btnBrowse.UseVisualStyleBackColor = true;
      this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
      // 
      // txtVideo
      // 
      this.txtVideo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.txtVideo.Location = new System.Drawing.Point(99, 84);
      this.txtVideo.Name = "txtVideo";
      this.txtVideo.Size = new System.Drawing.Size(218, 21);
      this.txtVideo.TabIndex = 285;
      this.txtVideo.TextChanged += new System.EventHandler(this.txtVideo_TextChanged);
      // 
      // chkNZT48
      // 
      this.chkNZT48.Location = new System.Drawing.Point(9, 86);
      this.chkNZT48.Name = "chkNZT48";
      this.chkNZT48.Size = new System.Drawing.Size(88, 17);
      this.chkNZT48.TabIndex = 284;
      this.chkNZT48.Text = "NZT-48";
      this.chkNZT48.UseVisualStyleBackColor = true;
      this.chkNZT48.CheckedChanged += new System.EventHandler(this.chkNZT48_CheckedChanged);
      // 
      // chkTCMP
      // 
      this.chkTCMP.Location = new System.Drawing.Point(9, 166);
      this.chkTCMP.Name = "chkTCMP";
      this.chkTCMP.Size = new System.Drawing.Size(151, 17);
      this.chkTCMP.TabIndex = 283;
      this.chkTCMP.Text = "TCMP";
      this.chkTCMP.UseVisualStyleBackColor = true;
      this.chkTCMP.CheckedChanged += new System.EventHandler(this.chkTCMP_CheckedChanged);
      // 
      // label8
      // 
      this.label8.ForeColor = System.Drawing.Color.MidnightBlue;
      this.label8.Location = new System.Drawing.Point(6, 57);
      this.label8.Name = "label8";
      this.label8.Size = new System.Drawing.Size(85, 21);
      this.label8.TabIndex = 276;
      this.label8.Text = "Threshold";
      this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      // 
      // cmbAlgorithm
      // 
      this.cmbAlgorithm.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.cmbAlgorithm.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.cmbAlgorithm.FormattingEnabled = true;
      this.cmbAlgorithm.Items.AddRange(new object[] {
            "Motion Detector",
            "REM Detector",
            "Gamma Detector",
            "Beta"});
      this.cmbAlgorithm.Location = new System.Drawing.Point(99, 30);
      this.cmbAlgorithm.Name = "cmbAlgorithm";
      this.cmbAlgorithm.Size = new System.Drawing.Size(254, 21);
      this.cmbAlgorithm.TabIndex = 275;
      this.cmbAlgorithm.SelectedIndexChanged += new System.EventHandler(this.cmbAlgorithm_SelectedIndexChanged);
      // 
      // label2
      // 
      this.label2.ForeColor = System.Drawing.Color.MidnightBlue;
      this.label2.Location = new System.Drawing.Point(6, 29);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(87, 21);
      this.label2.TabIndex = 271;
      this.label2.Text = "Algorithm";
      this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      // 
      // panel3D5
      // 
      this.panel3D5.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.panel3D5.BackColor = System.Drawing.Color.SteelBlue;
      this.panel3D5.Controls.Add(this.label3);
      this.panel3D5.Controls.Add(this.label4);
      this.panel3D5.Location = new System.Drawing.Point(0, 0);
      this.panel3D5.Name = "panel3D5";
      this.panel3D5.Size = new System.Drawing.Size(359, 24);
      this.panel3D5.TabIndex = 4;
      // 
      // label3
      // 
      this.label3.Font = new System.Drawing.Font("Verdana", 10F, System.Drawing.FontStyle.Bold);
      this.label3.ForeColor = System.Drawing.Color.White;
      this.label3.Image = ((System.Drawing.Image)(resources.GetObject("label3.Image")));
      this.label3.Location = new System.Drawing.Point(3, 3);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(19, 19);
      this.label3.TabIndex = 4;
      this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      // 
      // label4
      // 
      this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.label4.Font = new System.Drawing.Font("Verdana", 8F, System.Drawing.FontStyle.Bold);
      this.label4.ForeColor = System.Drawing.Color.White;
      this.label4.Location = new System.Drawing.Point(24, 3);
      this.label4.Name = "label4";
      this.label4.Size = new System.Drawing.Size(332, 19);
      this.label4.TabIndex = 3;
      this.label4.Text = "Settings";
      this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      // 
      // cmbThreshold
      // 
      this.cmbThreshold.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.cmbThreshold.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.cmbThreshold.FormattingEnabled = true;
      this.cmbThreshold.Items.AddRange(new object[] {
            "100",
            "150",
            "200",
            "250",
            "300",
            "350",
            "400",
            "450",
            "500",
            "550",
            "600",
            "650",
            "700",
            "750",
            "800",
            "850",
            "900",
            "950",
            "1000"});
      this.cmbThreshold.Location = new System.Drawing.Point(99, 57);
      this.cmbThreshold.Name = "cmbThreshold";
      this.cmbThreshold.Size = new System.Drawing.Size(254, 21);
      this.cmbThreshold.TabIndex = 33;
      this.cmbThreshold.SelectedIndexChanged += new System.EventHandler(this.cmbThreshold_SelectedIndexChanged);
      // 
      // cmbArduinoDelay
      // 
      this.cmbArduinoDelay.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.cmbArduinoDelay.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.cmbArduinoDelay.FormattingEnabled = true;
      this.cmbArduinoDelay.Items.AddRange(new object[] {
            "1",
            "2",
            "3",
            "4",
            "5",
            "6",
            "7",
            "8",
            "9"});
      this.cmbArduinoDelay.Location = new System.Drawing.Point(179, 138);
      this.cmbArduinoDelay.Name = "cmbArduinoDelay";
      this.cmbArduinoDelay.Size = new System.Drawing.Size(74, 21);
      this.cmbArduinoDelay.TabIndex = 295;
      this.cmbArduinoDelay.SelectedIndexChanged += new System.EventHandler(this.cmbArduinoDelay_SelectedIndexChanged);
      // 
      // PortForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.BackColor = System.Drawing.Color.LightSteelBlue;
      this.ClientSize = new System.Drawing.Size(383, 471);
      this.Controls.Add(this.pnlPlugins);
      this.Controls.Add(this.panel3D3);
      this.Font = new System.Drawing.Font("Verdana", 8.25F);
      this.ForeColor = System.Drawing.Color.MidnightBlue;
      this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
      this.Name = "PortForm";
      this.Text = "Lucid Scribe - NeuroSky ThinkGear EEG";
      this.Load += new System.EventHandler(this.PortForm_Load);
      this.pnlPlugins.ResumeLayout(false);
      this.mnuPortsList.ResumeLayout(false);
      this.Panel3D4.ResumeLayout(false);
      this.panel3D3.ResumeLayout(false);
      this.panel3D3.PerformLayout();
      this.panel3D5.ResumeLayout(false);
      this.ResumeLayout(false);

    }

    #endregion

    internal lucidcode.Controls.Panel3D pnlPlugins;
    internal lucidcode.Controls.Panel3D Panel3D4;
    internal System.Windows.Forms.Label Label5;
    internal System.Windows.Forms.Label Label6;
    internal System.Windows.Forms.ListView lstPorts;
    internal System.Windows.Forms.ImageList lstImg;
    private System.Windows.Forms.ContextMenuStrip mnuPortsList;
    private System.Windows.Forms.ToolStripMenuItem mnuRefreshPorts;
    internal lucidcode.Controls.Panel3D panel3D3;
    internal System.Windows.Forms.Label label8;
    private System.Windows.Forms.ComboBox cmbAlgorithm;
    internal System.Windows.Forms.Label label2;
    internal lucidcode.Controls.Panel3D panel3D5;
    internal System.Windows.Forms.Label label3;
    internal System.Windows.Forms.Label label4;
    private System.Windows.Forms.ComboBox cmbThreshold;
    private System.Windows.Forms.CheckBox chkTCMP;
    private System.Windows.Forms.Button btnBrowse;
    private System.Windows.Forms.TextBox txtVideo;
    private System.Windows.Forms.CheckBox chkNZT48;
    private System.Windows.Forms.CheckBox chktACS;
    private System.Windows.Forms.ComboBox cmbStateOn;
    private System.Windows.Forms.TextBox txtTarget;
    private System.Windows.Forms.ComboBox cmbStateOff;
    private System.Windows.Forms.ComboBox cmbArduinoPort;
    private System.Windows.Forms.TextBox txtArduinoOn;
    private System.Windows.Forms.CheckBox chkArduino;
    private System.Windows.Forms.TextBox txtArduinoOff;
    private System.Windows.Forms.ComboBox cmbArduinoDelay;
  }
}