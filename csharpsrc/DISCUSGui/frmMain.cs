using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.Diagnostics;
using DISCUSGui.localhost;

namespace DISCUSGui
{
	/// <summary>
	/// Summary description for Form1.
	/// </summary>
	public class frmMain : System.Windows.Forms.Form
	{
		private System.Windows.Forms.TextBox txtOutput;
		private System.Windows.Forms.TextBox txtInput;
		private System.Windows.Forms.Button btnLoadAlpha;
		private System.Windows.Forms.Button btnStart;
		private System.Windows.Forms.Button btnClearAll;
		private System.Windows.Forms.Button btnGK1Data;
		private System.Windows.Forms.Button btnGK2Data;
		private System.Windows.Forms.Button btnExit;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public frmMain()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.txtOutput = new System.Windows.Forms.TextBox();
			this.txtInput = new System.Windows.Forms.TextBox();
			this.btnLoadAlpha = new System.Windows.Forms.Button();
			this.btnStart = new System.Windows.Forms.Button();
			this.btnClearAll = new System.Windows.Forms.Button();
			this.btnGK1Data = new System.Windows.Forms.Button();
			this.btnGK2Data = new System.Windows.Forms.Button();
			this.btnExit = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// txtOutput
			// 
			this.txtOutput.Location = new System.Drawing.Point(8, 200);
			this.txtOutput.Multiline = true;
			this.txtOutput.Name = "txtOutput";
			this.txtOutput.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			this.txtOutput.Size = new System.Drawing.Size(632, 168);
			this.txtOutput.TabIndex = 0;
			this.txtOutput.Text = "";
			// 
			// txtInput
			// 
			this.txtInput.Location = new System.Drawing.Point(8, 8);
			this.txtInput.Multiline = true;
			this.txtInput.Name = "txtInput";
			this.txtInput.Size = new System.Drawing.Size(424, 176);
			this.txtInput.TabIndex = 1;
			this.txtInput.Text = "";
			// 
			// btnLoadAlpha
			// 
			this.btnLoadAlpha.Location = new System.Drawing.Point(456, 40);
			this.btnLoadAlpha.Name = "btnLoadAlpha";
			this.btnLoadAlpha.TabIndex = 2;
			this.btnLoadAlpha.Text = "Load Alpha";
			this.btnLoadAlpha.Click += new System.EventHandler(this.btnLoadAlpha_Click);
			// 
			// btnStart
			// 
			this.btnStart.Location = new System.Drawing.Point(456, 88);
			this.btnStart.Name = "btnStart";
			this.btnStart.TabIndex = 3;
			this.btnStart.Text = "Start";
			this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
			// 
			// btnClearAll
			// 
			this.btnClearAll.Location = new System.Drawing.Point(456, 136);
			this.btnClearAll.Name = "btnClearAll";
			this.btnClearAll.TabIndex = 4;
			this.btnClearAll.Text = "Clear All";
			this.btnClearAll.Click += new System.EventHandler(this.btnClearAll_Click);
			// 
			// btnGK1Data
			// 
			this.btnGK1Data.Location = new System.Drawing.Point(552, 40);
			this.btnGK1Data.Name = "btnGK1Data";
			this.btnGK1Data.TabIndex = 5;
			this.btnGK1Data.Text = "GK1 Data";
			this.btnGK1Data.Click += new System.EventHandler(this.btnGK1Data_Click);
			// 
			// btnGK2Data
			// 
			this.btnGK2Data.Location = new System.Drawing.Point(552, 88);
			this.btnGK2Data.Name = "btnGK2Data";
			this.btnGK2Data.TabIndex = 6;
			this.btnGK2Data.Text = "GK2 Data";
			this.btnGK2Data.Click += new System.EventHandler(this.btnGK2Data_Click);
			// 
			// btnExit
			// 
			this.btnExit.Location = new System.Drawing.Point(552, 136);
			this.btnExit.Name = "btnExit";
			this.btnExit.TabIndex = 7;
			this.btnExit.Text = "Exit";
			this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
			// 
			// frmMain
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(648, 381);
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.btnExit,
																		  this.btnGK2Data,
																		  this.btnGK1Data,
																		  this.btnClearAll,
																		  this.btnStart,
																		  this.btnLoadAlpha,
																		  this.txtInput,
																		  this.txtOutput});
			this.Name = "frmMain";
			this.Text = "Main";
			this.Load += new System.EventHandler(this.frmMain_Load);
			this.ResumeLayout(false);

		}
		#endregion

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main() 
		{
			Application.Run(new frmMain());
		}

		private void btnLoadAlpha_Click(object sender, System.EventArgs e)
		{
			txtInput.Text = "<?xml version=\"1.0\" encoding=\"utf-8\"?><definitions name=\"DemoAlpha\" targetNamespace=\"http://psl.cs.columbia.edu\" xmlns:xlang=\"http://schemas.microsoft.com/bixtalk/xlang\" xmlns:xs=\"http://www.w3.org/2001/XMLSchema\"><!-- Simple demo using simple xlang constructs --><xlang:behavior><xlang:body><xlang:sequence><xlang:action activation=\"true\" gatekeeper=\"PSLGatekeeper1\" servicename=\"BNQuoteService\" operation=\"getPrice\"><parameter name=\"isbn\"><![CDATA[<?xml version=\"1.0\"?><string>1861005458</string>]]></parameter></xlang:action><xlang:action gatekeeper=\"PSLGatekeeper1\" servicename=\"GeoCash\" operation=\"GetATMLocations\"><parameter name=\"Zipcode\"><![CDATA[<?xml version=\"1.0\"?><string>10025</string>]]></parameter></xlang:action></xlang:sequence></xlang:body></xlang:behavior></definitions>";
		}

		private void frmMain_Load(object sender, System.EventArgs e)
		{
		
		}

		private void btnExit_Click(object sender, System.EventArgs e)
		{
			Application.Exit();
		}

		private void btnClearAll_Click(object sender, System.EventArgs e)
		{
			txtInput.Clear();
			txtOutput.Clear();
		}

		private void btnGK1Data_Click(object sender, System.EventArgs e)
		{
			EventLog evtLog = new EventLog( "Application" );
			txtOutput.Clear();

			foreach( EventLogEntry Entry in evtLog.Entries )
			{
				if( Entry.Source.CompareTo( "PSLGatekeeper1" ) == 0 )
					txtOutput.AppendText( Entry.Message + "\n" );
			}
		}

		private void btnGK2Data_Click(object sender, System.EventArgs e)
		{
			EventLog evtLog = new EventLog( "Application" );
			txtOutput.Clear();

			foreach( EventLogEntry Entry in evtLog.Entries )
			{
				if( Entry.Source.CompareTo( "PSLGatekeeper2" ) == 0 )
					txtOutput.AppendText( Entry.Message + "\n" );
			}
		}

		private void btnStart_Click(object sender, System.EventArgs e)
		{
			PSLGatekeeper2 g = new PSLGatekeeper2();
			string [] arrRes = null;
			
			if( txtInput.Text.Length > 0 )
			{
				txtOutput.Clear();
				this.Cursor = Cursors.WaitCursor;
				arrRes = g.ExecuteAlphaProtocol( txtInput.Text );
				this.Cursor = Cursors.Default;
				if( arrRes != null )
				{
					for( int i = 0; i < arrRes.Length; i++ )
						txtOutput.AppendText( arrRes[i] );
				}
			}
		}
	}
}
