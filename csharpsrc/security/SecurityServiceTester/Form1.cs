using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.IO;

namespace SecurityServiceTester
{
	/// <summary>
	/// Summary description for Form1.
	/// </summary>
	public class Form1 : System.Windows.Forms.Form
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Button button3;
		private System.Windows.Forms.Button button2;
		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.Button buttonSubmit;
		private System.Windows.Forms.Panel panel2;
		private System.Windows.Forms.Splitter splitter1;
		private System.Windows.Forms.TextBox inputText;
		private System.Windows.Forms.Button buttonChangeUrl;
		private System.Windows.Forms.Button btnRequestCheck;
		private System.Windows.Forms.Button btnUnsignedRequest;
		private System.Windows.Forms.Panel panel3;
		private System.Windows.Forms.Panel panel4;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox outputText;
		private System.Windows.Forms.TextBox textStatus;
		private System.Windows.Forms.TextBox textAlias;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Button btnCopyUp;
		private System.Windows.Forms.Button btnClear;
		private System.Windows.Forms.Button btnRevokeTreaty;
		private System.Windows.Forms.ComboBox comboURL;

		private SecurityManagerService svc;

		public Form1()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
			svc = new SecurityManagerService();
			comboURL.Text = svc.Url;
			buttonChangeUrl.Enabled = false;
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
			this.panel1 = new System.Windows.Forms.Panel();
			this.buttonChangeUrl = new System.Windows.Forms.Button();
			this.button3 = new System.Windows.Forms.Button();
			this.button2 = new System.Windows.Forms.Button();
			this.button1 = new System.Windows.Forms.Button();
			this.buttonSubmit = new System.Windows.Forms.Button();
			this.btnRequestCheck = new System.Windows.Forms.Button();
			this.btnUnsignedRequest = new System.Windows.Forms.Button();
			this.btnRevokeTreaty = new System.Windows.Forms.Button();
			this.panel2 = new System.Windows.Forms.Panel();
			this.panel3 = new System.Windows.Forms.Panel();
			this.outputText = new System.Windows.Forms.TextBox();
			this.panel4 = new System.Windows.Forms.Panel();
			this.btnCopyUp = new System.Windows.Forms.Button();
			this.btnClear = new System.Windows.Forms.Button();
			this.textStatus = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.textAlias = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.splitter1 = new System.Windows.Forms.Splitter();
			this.inputText = new System.Windows.Forms.TextBox();
			this.comboURL = new System.Windows.Forms.ComboBox();
			this.panel1.SuspendLayout();
			this.panel2.SuspendLayout();
			this.panel3.SuspendLayout();
			this.panel4.SuspendLayout();
			this.SuspendLayout();
			// 
			// panel1
			// 
			this.panel1.Controls.AddRange(new System.Windows.Forms.Control[] {
																				 this.comboURL,
																				 this.buttonChangeUrl,
																				 this.button3,
																				 this.button2,
																				 this.button1,
																				 this.buttonSubmit,
																				 this.btnRequestCheck,
																				 this.btnUnsignedRequest,
																				 this.btnRevokeTreaty});
			this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.panel1.Location = new System.Drawing.Point(0, 341);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(672, 120);
			this.panel1.TabIndex = 6;
			// 
			// buttonChangeUrl
			// 
			this.buttonChangeUrl.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left);
			this.buttonChangeUrl.Enabled = false;
			this.buttonChangeUrl.ForeColor = System.Drawing.Color.Blue;
			this.buttonChangeUrl.Location = new System.Drawing.Point(448, 88);
			this.buttonChangeUrl.Name = "buttonChangeUrl";
			this.buttonChangeUrl.Size = new System.Drawing.Size(88, 24);
			this.buttonChangeUrl.TabIndex = 10;
			this.buttonChangeUrl.Text = "Change URL";
			this.buttonChangeUrl.Click += new System.EventHandler(this.buttonChangeUrl_Click);
			// 
			// button3
			// 
			this.button3.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left);
			this.button3.Location = new System.Drawing.Point(344, 8);
			this.button3.Name = "button3";
			this.button3.Size = new System.Drawing.Size(96, 32);
			this.button3.TabIndex = 8;
			this.button3.Text = "Sign doc";
			this.button3.Click += new System.EventHandler(this.button3_Click);
			// 
			// button2
			// 
			this.button2.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left);
			this.button2.Location = new System.Drawing.Point(344, 48);
			this.button2.Name = "button2";
			this.button2.Size = new System.Drawing.Size(96, 32);
			this.button2.TabIndex = 7;
			this.button2.Text = "Verify doc";
			this.button2.Click += new System.EventHandler(this.verifyDoc);
			// 
			// button1
			// 
			this.button1.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left);
			this.button1.Location = new System.Drawing.Point(8, 48);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(96, 32);
			this.button1.TabIndex = 6;
			this.button1.Text = "Verify signed treaty";
			this.button1.Click += new System.EventHandler(this.button1_Click);
			// 
			// buttonSubmit
			// 
			this.buttonSubmit.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left);
			this.buttonSubmit.Location = new System.Drawing.Point(8, 8);
			this.buttonSubmit.Name = "buttonSubmit";
			this.buttonSubmit.Size = new System.Drawing.Size(96, 32);
			this.buttonSubmit.TabIndex = 5;
			this.buttonSubmit.Text = "Verify unsigned treaty";
			this.buttonSubmit.Click += new System.EventHandler(this.buttonSubmit_Click);
			// 
			// btnRequestCheck
			// 
			this.btnRequestCheck.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left);
			this.btnRequestCheck.Location = new System.Drawing.Point(120, 8);
			this.btnRequestCheck.Name = "btnRequestCheck";
			this.btnRequestCheck.Size = new System.Drawing.Size(96, 32);
			this.btnRequestCheck.TabIndex = 7;
			this.btnRequestCheck.Text = "Signed Request Check";
			this.btnRequestCheck.Click += new System.EventHandler(this.btnRequestCheck_Click);
			// 
			// btnUnsignedRequest
			// 
			this.btnUnsignedRequest.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left);
			this.btnUnsignedRequest.Location = new System.Drawing.Point(120, 48);
			this.btnUnsignedRequest.Name = "btnUnsignedRequest";
			this.btnUnsignedRequest.Size = new System.Drawing.Size(96, 32);
			this.btnUnsignedRequest.TabIndex = 7;
			this.btnUnsignedRequest.Text = "Unsigned Request Check";
			this.btnUnsignedRequest.Click += new System.EventHandler(this.btnUnsignedRequest_Click);
			// 
			// btnRevokeTreaty
			// 
			this.btnRevokeTreaty.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left);
			this.btnRevokeTreaty.Location = new System.Drawing.Point(232, 8);
			this.btnRevokeTreaty.Name = "btnRevokeTreaty";
			this.btnRevokeTreaty.Size = new System.Drawing.Size(96, 32);
			this.btnRevokeTreaty.TabIndex = 8;
			this.btnRevokeTreaty.Text = "Revoke Treaty";
			// 
			// panel2
			// 
			this.panel2.Controls.AddRange(new System.Windows.Forms.Control[] {
																				 this.panel3,
																				 this.splitter1,
																				 this.inputText});
			this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel2.Name = "panel2";
			this.panel2.Size = new System.Drawing.Size(672, 341);
			this.panel2.TabIndex = 7;
			// 
			// panel3
			// 
			this.panel3.Controls.AddRange(new System.Windows.Forms.Control[] {
																				 this.outputText,
																				 this.panel4});
			this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel3.Location = new System.Drawing.Point(0, 162);
			this.panel3.Name = "panel3";
			this.panel3.Size = new System.Drawing.Size(672, 179);
			this.panel3.TabIndex = 9;
			// 
			// outputText
			// 
			this.outputText.Dock = System.Windows.Forms.DockStyle.Fill;
			this.outputText.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.outputText.Location = new System.Drawing.Point(0, 32);
			this.outputText.Multiline = true;
			this.outputText.Name = "outputText";
			this.outputText.ReadOnly = true;
			this.outputText.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.outputText.Size = new System.Drawing.Size(672, 147);
			this.outputText.TabIndex = 12;
			this.outputText.Text = "";
			// 
			// panel4
			// 
			this.panel4.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.panel4.Controls.AddRange(new System.Windows.Forms.Control[] {
																				 this.btnCopyUp,
																				 this.btnClear,
																				 this.textStatus,
																				 this.label1,
																				 this.textAlias,
																				 this.label2});
			this.panel4.Dock = System.Windows.Forms.DockStyle.Top;
			this.panel4.Name = "panel4";
			this.panel4.Size = new System.Drawing.Size(672, 32);
			this.panel4.TabIndex = 11;
			// 
			// btnCopyUp
			// 
			this.btnCopyUp.Location = new System.Drawing.Point(456, 3);
			this.btnCopyUp.Name = "btnCopyUp";
			this.btnCopyUp.Size = new System.Drawing.Size(96, 23);
			this.btnCopyUp.TabIndex = 13;
			this.btnCopyUp.Text = "Copy Up";
			this.btnCopyUp.Click += new System.EventHandler(this.btnCopyUp_Click);
			// 
			// btnClear
			// 
			this.btnClear.Location = new System.Drawing.Point(568, 3);
			this.btnClear.Name = "btnClear";
			this.btnClear.Size = new System.Drawing.Size(96, 23);
			this.btnClear.TabIndex = 12;
			this.btnClear.Text = "Clear All";
			this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
			// 
			// textStatus
			// 
			this.textStatus.Location = new System.Drawing.Point(48, 4);
			this.textStatus.Name = "textStatus";
			this.textStatus.ReadOnly = true;
			this.textStatus.Size = new System.Drawing.Size(40, 20);
			this.textStatus.TabIndex = 1;
			this.textStatus.Text = "";
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(8, 8);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(40, 16);
			this.label1.TabIndex = 0;
			this.label1.Text = "Status:";
			// 
			// textAlias
			// 
			this.textAlias.Location = new System.Drawing.Point(160, 4);
			this.textAlias.Name = "textAlias";
			this.textAlias.ReadOnly = true;
			this.textAlias.Size = new System.Drawing.Size(40, 20);
			this.textAlias.TabIndex = 1;
			this.textAlias.Text = "";
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(120, 8);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(40, 16);
			this.label2.TabIndex = 0;
			this.label2.Text = "SS Id:";
			// 
			// splitter1
			// 
			this.splitter1.Cursor = System.Windows.Forms.Cursors.HSplit;
			this.splitter1.Dock = System.Windows.Forms.DockStyle.Top;
			this.splitter1.Location = new System.Drawing.Point(0, 152);
			this.splitter1.Name = "splitter1";
			this.splitter1.Size = new System.Drawing.Size(672, 10);
			this.splitter1.TabIndex = 8;
			this.splitter1.TabStop = false;
			// 
			// inputText
			// 
			this.inputText.Dock = System.Windows.Forms.DockStyle.Top;
			this.inputText.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.inputText.Multiline = true;
			this.inputText.Name = "inputText";
			this.inputText.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.inputText.Size = new System.Drawing.Size(672, 152);
			this.inputText.TabIndex = 7;
			this.inputText.Text = "";
			// 
			// comboURL
			// 
			this.comboURL.Items.AddRange(new object[] {
														  "http://church.psl.cs.columbia.edu:8080/security/jaxrpc/SecurityManagerService",
														  "http://cairo.clic.cs.columbia.edu:8080/security/jaxrpc/SecurityManagerService"});
			this.comboURL.Location = new System.Drawing.Point(8, 88);
			this.comboURL.Name = "comboURL";
			this.comboURL.Size = new System.Drawing.Size(432, 21);
			this.comboURL.TabIndex = 11;
			this.comboURL.TextChanged += new System.EventHandler(this.comboURL_TextChanged);
			// 
			// Form1
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(672, 461);
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.panel2,
																		  this.panel1});
			this.MaximizeBox = false;
			this.Name = "Form1";
			this.Text = "SecurityService Tester";
			this.Load += new System.EventHandler(this.Form1_Load);
			this.panel1.ResumeLayout(false);
			this.panel2.ResumeLayout(false);
			this.panel3.ResumeLayout(false);
			this.panel4.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main() 
		{
			Application.Run(new Form1());
		}

		private void buttonSubmit_Click(object sender, System.EventArgs evt)
		{
			this.Cursor = Cursors.WaitCursor;
			try 
			{
				String[] response = svc.verifyTreaty(inputText.Text, false);
				showResponse(response);
			} 
			catch (System.Exception e)
			{
				outputText.Text = e.ToString();
			}
			this.Cursor = Cursors.Default;
		}

		private void button1_Click(object sender, System.EventArgs evt)
		{
			this.Cursor = Cursors.WaitCursor;
			try 
			{
				String[] response =  svc.verifyTreaty(inputText.Text, true);
				showResponse(response);
			} 
			catch (System.Exception e)
			{
				outputText.Text = e.ToString();
			}
			this.Cursor = Cursors.Default;
		}

		private void button3_Click(object sender, System.EventArgs evt)
		{
			this.Cursor = Cursors.WaitCursor;
			try 
			{
				String[] response = svc.signDocument(inputText.Text);
				showResponse(response);
			} 
			catch (System.Exception e)
			{
				outputText.Text = e.ToString();
			}
			this.Cursor = Cursors.Default;
		}

		private void verifyDoc(object sender, System.EventArgs evt)
		{
			this.Cursor = Cursors.WaitCursor;
			try 
			{
				String text = inputText.Text;
				
				String[] response = svc.verifyDocument(text);
				showResponse(response);
			} 
			catch (System.Exception e)
			{
				outputText.Text = e.ToString();
			}
			this.Cursor = Cursors.Default;
		}

		private void buttonChangeUrl_Click(object sender, System.EventArgs e)
		{
			try 
			{
				svc.Url = comboURL.Text;
			} 
			catch (Exception) 
			{
				MessageBox.Show(this,"Invalid URL! Please try again.","Error",MessageBoxButtons.OK,MessageBoxIcon.Exclamation);
				return;
			}

			buttonChangeUrl.Enabled = false;
		}

		private void btnRequestCheck_Click(object sender, System.EventArgs evt)
		{
			this.Cursor = Cursors.WaitCursor;
			try 
			{
				String[] response = svc.doRequestCheck(inputText.Text, true);
				showResponse(response);
			} 
			catch (System.Exception e)
			{
				outputText.Text = e.ToString();
			}
			this.Cursor = Cursors.Default;
		}

		private void btnUnsignedRequest_Click(object sender, System.EventArgs evt)
		{
			this.Cursor = Cursors.WaitCursor;
			try 
			{
				String[] response = svc.doRequestCheck(inputText.Text, false);
				showResponse(response);
			} 
			catch (System.Exception e)
			{
				outputText.Text = e.ToString();
			}
			this.Cursor = Cursors.Default;
		}

		private void btnCopyUp_Click(object sender, System.EventArgs e)
		{
			inputText.Text = outputText.Text;
			outputText.Text = "";
		}

		private void btnClear_Click(object sender, System.EventArgs e)
		{
			inputText.Text = "";
			outputText.Text = "";
		}

		private void showResponse(String[] response) 
		{
			textStatus.Text = textAlias.Text = outputText.Text = "";
			
			if (response == null) 
			{
				textStatus.Text = "Null!";
				return;
			} 
			
			if (response.Length >= 1)
				textStatus.Text = response[0];

			if (response.Length >= 2)
				outputText.Text = response[1];

			if (response.Length >= 3)
				textAlias.Text = response[2];

		}

		private void comboURL_TextChanged(object sender, System.EventArgs e)
		{
			buttonChangeUrl.Enabled = true;
			
		}

		private void Form1_Load(object sender, System.EventArgs e)
		{
		
		}

		
		
		
	}
}
