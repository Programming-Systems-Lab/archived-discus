using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.IO;
using PSL.DISCUS.DAML;

using System.Xml.Serialization;
using System.Reflection;
using DynamicPxy;


namespace DAMLDemo
{
	public enum enuDAMLType
	{
		stringType,
		decimalType,
		thingType
	};
	
	public class DAMLParameter
	{
		private enuDAMLType m_type = enuDAMLType.thingType;
		private object m_data = null;

		public DAMLParameter()
		{}

		public enuDAMLType Type
		{
			get
			{ return m_type; }
			set
			{ m_type = value; }
		}
		
		public object Data
		{
			get
			{ return m_data; }
			set
			{ m_data = value; }
		}
	}
	
	/// <summary>
	/// Summary description for Form1.
	/// </summary>
	public class frmMain : System.Windows.Forms.Form
	{
		private DAMLProcess m_demoProcess = null;
		private DAMLProcessModel m_processModel = null;
		private System.Windows.Forms.ComboBox cmbProcess;
		private System.Windows.Forms.GroupBox grpDetails;
		private System.Windows.Forms.RadioButton rdoAtomic;
		private System.Windows.Forms.RadioButton rdoSimple;
		private System.Windows.Forms.RadioButton rdoComposite;
		private System.Windows.Forms.GroupBox grpProcessType;
		private System.Windows.Forms.Button btnQuery;
		private System.Windows.Forms.TreeView tvwDetails;
		private System.Windows.Forms.TreeView tvwExecDetails;
		private System.Windows.Forms.Button btnExecDemo;
		private System.Windows.Forms.GroupBox grpExecDetails;
		private System.Windows.Forms.RichTextBox rtbResults;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public frmMain()
		{
			m_demoProcess = new DAMLProcess();

			// Can only execute Atomic processes
			m_demoProcess.ProcessType = enuProcessType.AtomicProcess;
			m_demoProcess.Name = "GetWeather";
			
			// Inputs necessary
			RDFProperty input = new RDFProperty();
			input.Name = "zipcode";
			input.Domain = "#WeatherRetriever";
			input.Range = "http://www.w3.org/2000/10/XMLSchema#string";
			input.SubPropertyOf = "http://www.daml.org/services/daml-s/2001/10/Process.daml#input";
			input.SameValueAs = "";

			m_demoProcess.AddInput( input );

			// Conditional output
			// WS returns complex type (class instance) this maps to DAML "Thing"
			RDFProperty condOutput = new RDFProperty();
			condOutput.Name = "CurrentWeather";
			condOutput.Domain = "#WeatherRetriever";
			condOutput.Range = "http://www.daml.org/2001/03/daml+oil#Thing";
			condOutput.SubPropertyOf = "http://www.daml.org/services/daml-s/2001/10/Process.daml#conditionalOutput";

			m_demoProcess.AddConditionalOutput( condOutput );

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
			this.grpDetails = new System.Windows.Forms.GroupBox();
			this.tvwExecDetails = new System.Windows.Forms.TreeView();
			this.tvwDetails = new System.Windows.Forms.TreeView();
			this.cmbProcess = new System.Windows.Forms.ComboBox();
			this.rdoAtomic = new System.Windows.Forms.RadioButton();
			this.rdoSimple = new System.Windows.Forms.RadioButton();
			this.rdoComposite = new System.Windows.Forms.RadioButton();
			this.grpProcessType = new System.Windows.Forms.GroupBox();
			this.btnQuery = new System.Windows.Forms.Button();
			this.btnExecDemo = new System.Windows.Forms.Button();
			this.grpExecDetails = new System.Windows.Forms.GroupBox();
			this.rtbResults = new System.Windows.Forms.RichTextBox();
			this.grpDetails.SuspendLayout();
			this.grpProcessType.SuspendLayout();
			this.grpExecDetails.SuspendLayout();
			this.SuspendLayout();
			// 
			// grpDetails
			// 
			this.grpDetails.Controls.AddRange(new System.Windows.Forms.Control[] {
																					 this.tvwDetails});
			this.grpDetails.Location = new System.Drawing.Point(8, 112);
			this.grpDetails.Name = "grpDetails";
			this.grpDetails.Size = new System.Drawing.Size(192, 224);
			this.grpDetails.TabIndex = 1;
			this.grpDetails.TabStop = false;
			this.grpDetails.Text = "Query Details";
			// 
			// tvwExecDetails
			// 
			this.tvwExecDetails.ImageIndex = -1;
			this.tvwExecDetails.Location = new System.Drawing.Point(8, 16);
			this.tvwExecDetails.Name = "tvwExecDetails";
			this.tvwExecDetails.SelectedImageIndex = -1;
			this.tvwExecDetails.Size = new System.Drawing.Size(176, 112);
			this.tvwExecDetails.TabIndex = 8;
			this.tvwExecDetails.Visible = false;
			// 
			// tvwDetails
			// 
			this.tvwDetails.ImageIndex = -1;
			this.tvwDetails.Location = new System.Drawing.Point(8, 16);
			this.tvwDetails.Name = "tvwDetails";
			this.tvwDetails.SelectedImageIndex = -1;
			this.tvwDetails.Size = new System.Drawing.Size(176, 200);
			this.tvwDetails.TabIndex = 7;
			// 
			// cmbProcess
			// 
			this.cmbProcess.Location = new System.Drawing.Point(8, 8);
			this.cmbProcess.Name = "cmbProcess";
			this.cmbProcess.Size = new System.Drawing.Size(192, 21);
			this.cmbProcess.TabIndex = 2;
			this.cmbProcess.Text = "Processes";
			this.cmbProcess.TextChanged += new System.EventHandler(this.cmbProcess_TextChanged);
			// 
			// rdoAtomic
			// 
			this.rdoAtomic.Location = new System.Drawing.Point(16, 24);
			this.rdoAtomic.Name = "rdoAtomic";
			this.rdoAtomic.TabIndex = 3;
			this.rdoAtomic.Text = "Atomic";
			this.rdoAtomic.CheckedChanged += new System.EventHandler(this.rdoAtomic_CheckedChanged);
			// 
			// rdoSimple
			// 
			this.rdoSimple.Location = new System.Drawing.Point(16, 48);
			this.rdoSimple.Name = "rdoSimple";
			this.rdoSimple.TabIndex = 0;
			this.rdoSimple.Text = "Simple";
			this.rdoSimple.CheckedChanged += new System.EventHandler(this.rdoSimple_CheckedChanged);
			// 
			// rdoComposite
			// 
			this.rdoComposite.Location = new System.Drawing.Point(16, 72);
			this.rdoComposite.Name = "rdoComposite";
			this.rdoComposite.TabIndex = 4;
			this.rdoComposite.Text = "Composite";
			this.rdoComposite.CheckedChanged += new System.EventHandler(this.rdoComposite_CheckedChanged);
			// 
			// grpProcessType
			// 
			this.grpProcessType.Controls.AddRange(new System.Windows.Forms.Control[] {
																						 this.rdoAtomic,
																						 this.rdoSimple,
																						 this.rdoComposite});
			this.grpProcessType.Location = new System.Drawing.Point(272, 0);
			this.grpProcessType.Name = "grpProcessType";
			this.grpProcessType.Size = new System.Drawing.Size(128, 104);
			this.grpProcessType.TabIndex = 5;
			this.grpProcessType.TabStop = false;
			this.grpProcessType.Text = "Process Type";
			// 
			// btnQuery
			// 
			this.btnQuery.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btnQuery.Location = new System.Drawing.Point(128, 56);
			this.btnQuery.Name = "btnQuery";
			this.btnQuery.TabIndex = 6;
			this.btnQuery.Text = "Query";
			this.btnQuery.Click += new System.EventHandler(this.btnQuery_Click);
			// 
			// btnExecDemo
			// 
			this.btnExecDemo.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btnExecDemo.Location = new System.Drawing.Point(8, 56);
			this.btnExecDemo.Name = "btnExecDemo";
			this.btnExecDemo.TabIndex = 7;
			this.btnExecDemo.Text = "ExecDemo";
			this.btnExecDemo.Visible = false;
			this.btnExecDemo.Click += new System.EventHandler(this.btnExecDemo_Click);
			// 
			// grpExecDetails
			// 
			this.grpExecDetails.Controls.AddRange(new System.Windows.Forms.Control[] {
																						 this.rtbResults,
																						 this.tvwExecDetails});
			this.grpExecDetails.Location = new System.Drawing.Point(208, 112);
			this.grpExecDetails.Name = "grpExecDetails";
			this.grpExecDetails.Size = new System.Drawing.Size(192, 224);
			this.grpExecDetails.TabIndex = 9;
			this.grpExecDetails.TabStop = false;
			this.grpExecDetails.Text = "Execute Process Details";
			// 
			// rtbResults
			// 
			this.rtbResults.Location = new System.Drawing.Point(8, 136);
			this.rtbResults.Name = "rtbResults";
			this.rtbResults.Size = new System.Drawing.Size(176, 80);
			this.rtbResults.TabIndex = 9;
			this.rtbResults.Text = "";
			// 
			// frmMain
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(408, 342);
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.grpExecDetails,
																		  this.btnExecDemo,
																		  this.btnQuery,
																		  this.grpProcessType,
																		  this.cmbProcess,
																		  this.grpDetails});
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.MaximizeBox = false;
			this.Name = "frmMain";
			this.Text = "DAMLDemo";
			this.Load += new System.EventHandler(this.frmMain_Load);
			this.grpDetails.ResumeLayout(false);
			this.grpProcessType.ResumeLayout(false);
			this.grpExecDetails.ResumeLayout(false);
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

		private void frmMain_Load(object sender, System.EventArgs e)
		{
			FileStream fs = new FileStream( "CongoProcess.txt", FileMode.Open );
			StreamReader s = new StreamReader( fs );
			string strDAML = s.ReadToEnd();

			m_processModel = new DAMLProcessModel();
			m_processModel.LoadXml( strDAML );
			
			rdoAtomic.Checked = true;

			if( rdoAtomic.Checked )
				LoadProcessData( m_demoProcess, ref tvwExecDetails );
		}

		private void btnQuery_Click(object sender, System.EventArgs e)
		{
		  string[] arrProcessNames = null;
		  // based on which rdoXXXX checked
		  // query for those types of processes
		  if( rdoAtomic.Checked )
			  arrProcessNames = m_processModel.AtomicProcesses;
		  else if( rdoSimple.Checked )
			  arrProcessNames = m_processModel.SimpleProcesses;
		  else arrProcessNames = m_processModel.CompositeProcesses;

		  // Load up combo box
		  cmbProcess.Text = "";
		  cmbProcess.Items.AddRange( arrProcessNames );

		  if( arrProcessNames.Length > 0 )
			  cmbProcess.Text = arrProcessNames[0];
		}

		// Hook into combo box selection change event
		// to retrieve details on process
		private void cmbProcess_TextChanged(object sender, System.EventArgs e)
		{
			string strEvent = e.ToString();

			// If no text selected then exit
			if( this.cmbProcess.Text == "" )
				return;
			
			// otherwise go get data and display
			DAMLProcess process = new DAMLProcess();
			enuProcessType processType = enuProcessType.AtomicProcess;

			if( rdoAtomic.Checked )
				processType = enuProcessType.AtomicProcess;
			else if( rdoSimple.Checked )
				processType = enuProcessType.SimpleProcess;
			else processType = enuProcessType.CompositeProcess;

			// Do query
			process = m_processModel.GetProcessData( cmbProcess.Text, processType );

			LoadProcessData( process, ref tvwDetails );
		}

		private void LoadProcessData( DAMLProcess process, ref TreeView tview )
		{
			// Clear treeview
			tview.Nodes.Clear();
			
			if( !process.HasData )
			{
				tview.Nodes.Add( "No Details" );	
				return;
			}
			
			// Add nodes...

			// Inputs...
			if( process.HasInputs )
			{
				TreeNode Node = new TreeNode( "Inputs" );
				RDFProperty[] arrData = process.Inputs;

				for( int i = 0; i < arrData.Length; i++ )
				{
					Node.Nodes.Add( arrData[i].Name + " -> " + arrData[i].Range );
				}

				tview.Nodes.Add( Node );
			}

			if( process.HasParameters )
			{
				TreeNode Node = new TreeNode( "Parameters" );
				RDFProperty[] arrData = process.Parameters;

				for( int i = 0; i < arrData.Length; i++ )
				{
					Node.Nodes.Add( arrData[i].Name + " -> " + arrData[i].Range );
				}

				tview.Nodes.Add( Node );
			}
			
			if( process.HasOutputs )
			{
				TreeNode Node = new TreeNode( "Outputs" );
				RDFProperty[] arrData = process.Outputs;

				for( int i = 0; i < arrData.Length; i++ )
				{
					Node.Nodes.Add( arrData[i].Name + " -> " + arrData[i].Range );
				}

				tview.Nodes.Add( Node );
			}

			if( process.HasConditionalOutputs )
			{
				TreeNode Node = new TreeNode( "ConditionalOutputs" );
				RDFProperty[] arrData = process.ConditionalOutputs;

				for( int i = 0; i < arrData.Length; i++ )
				{
					Node.Nodes.Add( arrData[i].Name + " -> " + arrData[i].Range );
				}

				tview.Nodes.Add( Node );
			}
			
			if( process.HasCoOutputs )
			{
				TreeNode Node = new TreeNode( "CoOutputs" );
				RDFProperty[] arrData = process.CoOutputs;

				for( int i = 0; i < arrData.Length; i++ )
				{
					Node.Nodes.Add( arrData[i].Name + " -> " + arrData[i].Range );
				}

				tview.Nodes.Add( Node );
			}
			
			if( process.HasEffects )
			{
				TreeNode Node = new TreeNode( "Effects" );
				RDFProperty[] arrData = process.Effects;

				for( int i = 0; i < arrData.Length; i++ )
				{
					Node.Nodes.Add( arrData[i].Name + " -> " + arrData[i].Range );
				}

				tview.Nodes.Add( Node );
			}

			if( process.HasCoConditions )
			{
				TreeNode Node = new TreeNode( "CoConditions" );
				RDFProperty[] arrData = process.CoConditions;

				for( int i = 0; i < arrData.Length; i++ )
				{
					Node.Nodes.Add( arrData[i].Name + " -> " + arrData[i].Range );
				}

				tview.Nodes.Add( Node );
			}
			
			if( process.HasPreconditions )
			{
				TreeNode Node = new TreeNode( "Preconditions" );
				RDFProperty[] arrData = process.Preconditions;

				for( int i = 0; i < arrData.Length; i++ )
				{
					Node.Nodes.Add( arrData[i].Name + " -> " + arrData[i].Range );
				}

				tview.Nodes.Add( Node );
			}
			
			if( process.HasSubProcesses )
			{
				TreeNode Node = new TreeNode( "SubProcesses" );
				DAMLProcess[] arrData = process.SubProcesses;

				for( int i = 0; i < arrData.Length; i++ )
				{
					Node.Nodes.Add( arrData[i].Name + " -> " + arrData[i].ProcessType.ToString() );
				}

				tview.Nodes.Add( Node );
			}

			// Expand inputs
			tview.Nodes[0].Expand();
		}

		private void rdoAtomic_CheckedChanged(object sender, System.EventArgs e)
		{
			string strEvent = e.ToString();
			resetUI();
		}
		
		private void rdoSimple_CheckedChanged(object sender, System.EventArgs e)
		{
			string strEvent = e.ToString();
			resetUI();
		}

		private void rdoComposite_CheckedChanged(object sender, System.EventArgs e)
		{
			string strEvent = e.ToString();
			resetUI();
		}

		private void resetUI()
		{
			this.cmbProcess.Text = "";
			this.cmbProcess.Items.Clear();
			this.tvwDetails.Nodes.Clear();

			// Show or hide hidden UI elements
			if( this.rdoAtomic.Checked )
			{
				this.grpExecDetails.Visible = true;
				this.grpExecDetails.Text = "Execute Process: " + m_demoProcess.Name;
				if( !this.btnExecDemo.Visible )
					this.btnExecDemo.Visible = true;

				if( !this.tvwExecDetails.Visible )
				{
					this.tvwExecDetails.Visible = true;
					// populate with details of process we are
					// executing
				}

				if( !this.rtbResults.Visible )
					this.rtbResults.Visible = true;
			}
			else
			{
				this.btnExecDemo.Visible = false;
				this.tvwExecDetails.Nodes.Clear();
				this.tvwExecDetails.Visible = false;
				this.rtbResults.Clear();
				this.rtbResults.Visible = false;
				this.grpExecDetails.Visible = false;
			}
		}

		// Simple scenario to show how an agent could execute a WS described using
		// DAML
		private object DemoExecuteService( DAMLProcess process, DAMLParameter zipcode )
		{
			object objRes = null;

			// Ensure zipcode.Type matches to System.String in .NET world
			if( zipcode.Type == enuDAMLType.stringType && process.Inputs[0].Range.IndexOf( "#string" ) != -1 )
			{
				DynamicPxy.WeatherRetriever wr = new DynamicPxy.WeatherRetriever();
				Type PxyType = wr.GetType();
							
				// Even though we pass zipcode.data which is an object it is
				// dynamically cast to string
				MethodInfo mInfo = PxyType.GetMethod( process.Name );
				if( mInfo != null )
				{
					objRes = mInfo.Invoke( wr, new object[]{ zipcode.Data } );
				}
			}
			else MessageBox.Show( "Input type mismatch on process execute" );

			return objRes;
		}

		private void btnExecDemo_Click(object sender, System.EventArgs e)
		{
			if( !this.tvwExecDetails.Visible )
				return;
			
			this.tvwExecDetails.Nodes.Clear();
			LoadProcessData( m_demoProcess, ref tvwExecDetails );

			DAMLParameter zipcode = new DAMLParameter();
			zipcode.Type = enuDAMLType.stringType;
			zipcode.Data = "10025";
			
			TreeNode resultsNode = new TreeNode( "Results" );
			object objRes = null;

			// Do execute
			objRes = DemoExecuteService( m_demoProcess, zipcode );
			string strRetVal = "No results available";
			
			// Add results
			if( objRes != null )
			{
				// Serialize to string
				
				// Get returned type
				Type returnType = objRes.GetType();
				// Create XmlSerializer
				XmlSerializer ser = new XmlSerializer( returnType );
				// Create a memory stream
				MemoryStream ms = new MemoryStream();
				// Serialize to stream ms
				ser.Serialize( ms, objRes );
				// Goto start of stream
				ms.Seek( 0, System.IO.SeekOrigin.Begin );
				// Create a stream reader
				TextReader reader = new StreamReader( ms );
				// Read entire stream, this is our return value
				strRetVal = reader.ReadToEnd();
				resultsNode.Nodes.Add( strRetVal );
			}
			else resultsNode.Nodes.Add( strRetVal );	
			
			this.rtbResults.Text = strRetVal;
			this.tvwExecDetails.Nodes.Add( resultsNode );
		}
	}
}
