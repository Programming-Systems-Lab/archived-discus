using System;
using System.Xml;
using System.Xml.XPath;
using System.Collections;
using System.Diagnostics;
using System.Xml.Serialization;

namespace PSL.DISCUS.DAML
{
	// Value types representing IOPEs
	public struct IOType
	{
		private string m_strParamDesc;
		private string m_strParamName;
		private string m_strRestrictedTo;
		private string m_strRefersTo;

		public bool isValid
		{
			get
			{ return ( m_strParamDesc.Length > 0 && m_strParamName.Length > 0 ) && ( m_strRestrictedTo.Length > 0 && m_strRefersTo.Length > 0 ); }
		}
		
		public string ParameterDesc
		{
			get
			{ return m_strParamDesc; }
			set
			{ m_strParamDesc = value; }
		}

		public string ParameterName
		{
			get
			{ return m_strParamName; }
			set
			{ m_strParamName = value; }
		}

		public string RestrictedTo
		{
			get
			{ return m_strRestrictedTo; }
			set
			{ m_strRestrictedTo = value; }
		}

		public string RefersTo
		{
			get
			{ return m_strRefersTo; }
			set
			{ m_strRefersTo = value; }
		}

	}
		
	public struct EPType
	{
		private string m_strConditionDesc;
		private string m_strConditionName;
		private string m_strStatement;
		private string m_strRefersTo;

		public bool isValid
		{
			get
			{ return ( m_strConditionDesc.Length > 0 && m_strConditionName.Length > 0 ) && ( m_strStatement.Length > 0 && m_strRefersTo.Length > 0 ); }
		}

		public string ConditionDesc
		{
			get
			{ return m_strConditionDesc; }
			set
			{ m_strConditionDesc = value; }
		}

		public string ConditionName
		{
			get
			{ return m_strConditionName; }
			set
			{ m_strConditionName = value; }
		}

		public string Statement
		{
			get
			{ return m_strStatement; }
			set
			{ m_strStatement = value; }
		}

		public string RefersTo
		{
			get
			{ return m_strRefersTo; }
			set
			{ m_strRefersTo = value; }
		}
	}

	public enum enuIOPESearchBy
	{
		PARAM_DESC,
		PARAM_NAME,
		COND_DESC,
		COND_NAME,
		REFERS_TO
	};

	/// <summary>
	/// Summary description for DAMLServiceProfile.
	/// </summary>
	public class DAMLServiceProfile:DAMLContainer
	{
		/* Constructor */
		public DAMLServiceProfile()
		{
			// Init inherited members
			m_EvtLog = new EventLog( "Application" );
			m_EvtLog.Source = "DAMLServiceProfile";
			m_doc = new XmlDocument();
			m_mgr = null;
		}
		
		// Properties
		public string[] OntologyImports
		{
			get
			{ return GetOntologyImports(); }
		}
		
		public string ServiceName
		{
			get
			{ return GetServiceName(); }
		}
		
		public string ServiceProfile
		{
			get
			{ return GetServiceProfile(); }
		}

		public string PresentedBy
		{
			get
			{ return GetPresentedBy(); }
		}

		public string TextDescription
		{
			get
			{ return GetTextDescription(); }
		}
		
		public string IntendedPurpose
		{
			get
			{ return GetIntendedPurpose(); }
		}
			
		public string RequestedBy
		{
			get
			{ return GetRequestedBy(); }
		}
		
		public string ServiceProvider
		{
			get
			{ return GetServiceProvider(); }
		}

		public string ProfileName
		{
			get
			{ return GetProfileName(); }
		}

		public string ProfilePhone
		{
			get
			{ return GetProfilePhone(); }
		}
		
		public string ProfileFax
		{
			get
			{ return GetProfileFax(); }
		}
		
		public string ProfileEmail
		{
			get
			{ return GetProfileEmail(); }
		}

		public string PhysicalAddress
		{
			get
			{ return GetPhysicalAddress(); }
		}

		public string WebUrl
		{
			get
			{ return GetWebUrl(); }
		}

		public string GeographicRadius
		{
			get
			{ return GetGeographicRadius(); }
		}
		
		public string QualityRating
		{
			get
			{ return GetQualityRating(); }
		}

		public string ProcessModel
		{
			get
			{ return GetProcessModel(); }
		}
		
		public IOType[] InputParameters
		{
			get
			{ return GetInputParameters(); }
		}

		public IOType[] OutputParameters
		{
			get
			{ return GetOutputParameters(); }
		}

		public EPType[] Preconditions
		{
			get
			{ return GetPreconditions(); }
		}

		public EPType[] Effects
		{
			get
			{ return GetEffects(); }
		}
		
		
		// Methods

		/* Function returns the data of a named input
		 * 
		 * Inputs: strName - name of input to retrieve data about
		 * 
		 * Return value: the IOType data structure of the named input
		 */
		public IOType GetInputByName( string strName )
		{
			// Create Expression Builder instance
			IIOPEXPathExprBuilder IBuilder = IOPEXPathExprBuilderFactory.CreateInstance( enuIOPEType.Input );
			// Build XPath Expression
			string strXPathExpr = DAMLConstants.SERVICE_PROFILE + IBuilder.BuildExpression( enuIOPESearchBy.PARAM_NAME, strName );
			XmlNode root = m_doc.DocumentElement;
			XmlNode node = root.SelectSingleNode( strXPathExpr, m_mgr );
			node = node.ParentNode;

			IOType ioData = GetIONodeData( node );

			return ioData;
		}
		
		/* Function returns the data of a input based on its description
		 * 
		 * Inputs: strDesc - description of input to retrieve data about
		 * 
		 * Return value: the IOType data structure of the input
		 */
		public IOType GetInputByDescription( string strDesc )
		{
			// Create Expression Builder instance
			IIOPEXPathExprBuilder IBuilder = IOPEXPathExprBuilderFactory.CreateInstance( enuIOPEType.Input );
			// Build XPath Expression
			string strXPathExpr = DAMLConstants.SERVICE_PROFILE + IBuilder.BuildExpression( enuIOPESearchBy.PARAM_DESC, strDesc );
			XmlNode root = m_doc.DocumentElement;
			XmlNode node = root.SelectSingleNode( strXPathExpr, m_mgr );
			
			IOType ioData = GetIONodeData( node );

			return ioData;
		}
		
		/* Function returns the data of a input based on a refersTo statement
		 * 
		 * Inputs: strRef - reference of input to retrieve data about
		 * 
		 * Return value: the IOType data structure of the input
		 */
		public IOType GetInputByReference( string strRef )
		{
			// Create Expression Builder instance
			IIOPEXPathExprBuilder IBuilder = IOPEXPathExprBuilderFactory.CreateInstance( enuIOPEType.Input );
			// Build XPath Expression
			string strXPathExpr = DAMLConstants.SERVICE_PROFILE + IBuilder.BuildExpression( enuIOPESearchBy.REFERS_TO, strRef );
			XmlNode root = m_doc.DocumentElement;
			XmlNode node = root.SelectSingleNode( strXPathExpr, m_mgr );
			node = node.ParentNode;

			IOType ioData = GetIONodeData( node );

			return ioData;
		}
		
		/* Function returns the data of a named output
		 * 
		 * Inputs: strName - name of output to retrieve data about
		 * 
		 * Return value: the IOType data structure of the named output
		 */
		public IOType GetOutputByName( string strName )
		{
			// Create Expression Builder instance
			IIOPEXPathExprBuilder IBuilder = IOPEXPathExprBuilderFactory.CreateInstance( enuIOPEType.Output );
			// Build XPath Expression
			string strXPathExpr = DAMLConstants.SERVICE_PROFILE + IBuilder.BuildExpression( enuIOPESearchBy.PARAM_NAME, strName );
			XmlNode root = m_doc.DocumentElement;
			XmlNode node = root.SelectSingleNode( strXPathExpr, m_mgr );
			node = node.ParentNode;

			IOType ioData = GetIONodeData( node );

			return ioData;
		}

		/* Function returns the data of a output based on its description
		 * 
		 * Inputs: strDesc - description of output to retrieve data about
		 * 
		 * Return value: the IOType data structure of the output
		 */
		public IOType GetOutputByDescription( string strDesc )
		{
			// Create Expression Builder instance
			IIOPEXPathExprBuilder IBuilder = IOPEXPathExprBuilderFactory.CreateInstance( enuIOPEType.Output );
			// Build XPath Expression
			string strXPathExpr = DAMLConstants.SERVICE_PROFILE + IBuilder.BuildExpression( enuIOPESearchBy.PARAM_DESC, strDesc );
			XmlNode root = m_doc.DocumentElement;
			XmlNode node = root.SelectSingleNode( strXPathExpr, m_mgr );
			
			IOType ioData = GetIONodeData( node );

			return ioData;
		}
		
		/* Function returns the data of a output based on a refersTo statement
		 * 
		 * Inputs: strRef - reference of output to retrieve data about
		 * 
		 * Return value: the IOType data structure of the output
		 */
		public IOType GetOutputByReference( string strRef )
		{
			// Create Expression Builder instance
			IIOPEXPathExprBuilder IBuilder = IOPEXPathExprBuilderFactory.CreateInstance( enuIOPEType.Output );
			// Build XPath Expression
			string strXPathExpr = DAMLConstants.SERVICE_PROFILE + IBuilder.BuildExpression( enuIOPESearchBy.REFERS_TO, strRef );
			XmlNode root = m_doc.DocumentElement;
			XmlNode node = root.SelectSingleNode( strXPathExpr, m_mgr );
			node = node.ParentNode;

			IOType ioData = GetIONodeData( node );

			return ioData;
		}

		/* Function returns the data of a named precondition
		 * 
		 * Inputs: strName - name of precondition to retrieve data about
		 * 
		 * Return value: the EPType data structure of the named precondition
		 */
		public EPType GetPreconditionByName( string strName )
		{
			// Create Expression Builder instance
			IIOPEXPathExprBuilder IBuilder = IOPEXPathExprBuilderFactory.CreateInstance( enuIOPEType.Precondition );
			// Build XPath Expression
			string strXPathExpr = DAMLConstants.SERVICE_PROFILE + IBuilder.BuildExpression( enuIOPESearchBy.COND_NAME, strName );
			XmlNode root = m_doc.DocumentElement;
			XmlNode node = root.SelectSingleNode( strXPathExpr, m_mgr );
			node = node.ParentNode;

			EPType epData = GetEPNodeData( node );

			return epData;
		}

		/* Function returns the data of a precondition based on its description
		 * 
		 * Inputs: strDesc - description of precondition to retrieve data about
		 * 
		 * Return value: the EPType data structure of the precondition
		 */
		public EPType GetPreconditionByDescription( string strDesc )
		{
			// Create Expression Builder instance
			IIOPEXPathExprBuilder IBuilder = IOPEXPathExprBuilderFactory.CreateInstance( enuIOPEType.Precondition );
			// Build XPath Expression
			string strXPathExpr = DAMLConstants.SERVICE_PROFILE + IBuilder.BuildExpression( enuIOPESearchBy.COND_DESC, strDesc );
			XmlNode root = m_doc.DocumentElement;
			XmlNode node = root.SelectSingleNode( strXPathExpr, m_mgr );
			
			EPType epData = GetEPNodeData( node );

			return epData;
		}

		/* Function returns the data of a precondition based on a refersTo statement
		 * 
		 * Inputs: strRef - reference of precondition to retrieve data about
		 * 
		 * Return value: the EPType data structure of the precondition
		 */
		public EPType GetPreconditionByReference( string strRef )
		{
			// Create Expression Builder instance
			IIOPEXPathExprBuilder IBuilder = IOPEXPathExprBuilderFactory.CreateInstance( enuIOPEType.Precondition );
			// Build XPath Expression
			string strXPathExpr = DAMLConstants.SERVICE_PROFILE + IBuilder.BuildExpression( enuIOPESearchBy.REFERS_TO, strRef );
			XmlNode root = m_doc.DocumentElement;
			XmlNode node = root.SelectSingleNode( strXPathExpr, m_mgr );
			node = node.ParentNode;

			EPType epData = GetEPNodeData( node );

			return epData;
		}
		
		/* Function returns the data of a named effect
		 * 
		 * Inputs: strName - name of effect to retrieve data about
		 * 
		 * Return value: the EPType data structure of the named effect
		 */
		public EPType GetEffectByName( string strName )
		{
			// Create Expression Builder instance
			IIOPEXPathExprBuilder IBuilder = IOPEXPathExprBuilderFactory.CreateInstance( enuIOPEType.Effect );
			// Build XPath Expression
			string strXPathExpr = DAMLConstants.SERVICE_PROFILE + IBuilder.BuildExpression( enuIOPESearchBy.COND_NAME, strName );
			XmlNode root = m_doc.DocumentElement;
			XmlNode node = root.SelectSingleNode( strXPathExpr, m_mgr );
			node = node.ParentNode;
			
			EPType epData = GetEPNodeData( node );

			return epData;
		}

		/* Function returns the data of an effect based on its description
		 * 
		 * Inputs: strDesc - description of effect to retrieve data about
		 * 
		 * Return value: the EPType data structure of the effect
		 */
		public EPType GetEffectByDescription( string strDesc )
		{
			// Create Expression Builder instance
			IIOPEXPathExprBuilder IBuilder = IOPEXPathExprBuilderFactory.CreateInstance( enuIOPEType.Effect );
			// Build XPath Expression
			string strXPathExpr = DAMLConstants.SERVICE_PROFILE + IBuilder.BuildExpression( enuIOPESearchBy.COND_DESC, strDesc );
			XmlNode root = m_doc.DocumentElement;
			XmlNode node = root.SelectSingleNode( strXPathExpr, m_mgr );
			
			EPType epData = GetEPNodeData( node );

			return epData;
		}
		
		/* Function returns the data of an effect based on a refersTo statement
		 * 
		 * Inputs: strRef - reference of effect to retrieve data about
		 * 
		 * Return value: the EPType data structure of the effect
		 */
		public EPType GetEffectByReference( string strRef )
		{
			// Create Expression Builder instance
			IIOPEXPathExprBuilder IBuilder = IOPEXPathExprBuilderFactory.CreateInstance( enuIOPEType.Effect );
			// Build XPath Expression
			string strXPathExpr = DAMLConstants.SERVICE_PROFILE + IBuilder.BuildExpression( enuIOPESearchBy.REFERS_TO, strRef );
			XmlNode root = m_doc.DocumentElement;
			XmlNode node = root.SelectSingleNode( strXPathExpr, m_mgr );
			node = node.ParentNode;

			EPType epData = GetEPNodeData( node );

			return epData;
		}
		
		/* Function returs all the effects in a ServiceProfile
		 * 
		 * Inputs: none
		 * 
		 * Return values: an array of effects
		 */
		private EPType[] GetEffects()
		{
			ArrayList lstEffects = new ArrayList();

			try
			{
				XmlNode root = m_doc.DocumentElement;
				
				// Create Expression Builder instance
				IIOPEXPathExprBuilder IBuilder = IOPEXPathExprBuilderFactory.CreateInstance( enuIOPEType.Effect );
				// Build XPath Expression
				string strXPathExpr = DAMLConstants.SERVICE_PROFILE + IBuilder.BuildExpression( enuIOPESearchBy.COND_DESC );
							
				XmlNodeList nodeList = root.SelectNodes( strXPathExpr, m_mgr );

				foreach( XmlNode descNode in nodeList )
				{
					EPType effect = GetEPNodeData( descNode );
					
					if( effect.isValid )
						lstEffects.Add( effect );
				}
			
			}
			catch( Exception e )
			{
				m_EvtLog.WriteEntry( e.Message, EventLogEntryType.Error );
			}

			return (EPType[]) lstEffects.ToArray( typeof(EPType) );
		}

		/* Function returs all the preconditions in a ServiceProfile
		 * 
		 * Inputs: none
		 * 
		 * Return values: an array of preconditions
		 */
		private EPType[] GetPreconditions()
		{
			ArrayList lstPreconds = new ArrayList();

			try
			{
				XmlNode root = m_doc.DocumentElement;
				
				// Create Expression Builder instance
				IIOPEXPathExprBuilder IBuilder = IOPEXPathExprBuilderFactory.CreateInstance( enuIOPEType.Precondition );
				// Build XPath Expression
				string strXPathExpr = DAMLConstants.SERVICE_PROFILE + IBuilder.BuildExpression( enuIOPESearchBy.COND_DESC );
							
				XmlNodeList nodeList = root.SelectNodes( strXPathExpr, m_mgr );

				foreach( XmlNode descNode in nodeList )
				{
					EPType precond = this.GetEPNodeData( descNode );
					
					if( precond.isValid )
						lstPreconds.Add( precond );
				}
			
			}
			catch( Exception e )
			{
				m_EvtLog.WriteEntry( e.Message, EventLogEntryType.Error );
			}

			return (EPType[]) lstPreconds.ToArray( typeof(EPType) );
		}
		
		/* Function returs all the outputs in a ServiceProfile
		 * 
		 * Inputs: none
		 * 
		 * Return values: an array of outputs
		 */
		private IOType[] GetOutputParameters()
		{
			ArrayList lstOutputs = new ArrayList();

			try
			{
				XmlNode root = m_doc.DocumentElement;
				
				// Create Expression Builder instance
				IIOPEXPathExprBuilder IBuilder = IOPEXPathExprBuilderFactory.CreateInstance( enuIOPEType.Output );
				// Build XPath Expression
				string strXPathExpr = DAMLConstants.SERVICE_PROFILE + IBuilder.BuildExpression( enuIOPESearchBy.PARAM_DESC );
							
				XmlNodeList nodeList = root.SelectNodes( strXPathExpr, m_mgr );

				foreach( XmlNode descNode in nodeList )
				{
					IOType output = GetIONodeData( descNode );
									
					if( output.isValid )
						lstOutputs.Add( output );
				}
			
			}
			catch( Exception e )
			{
				m_EvtLog.WriteEntry( e.Message, EventLogEntryType.Error );
			}

			return (IOType[]) lstOutputs.ToArray( typeof(IOType) );
		}

		/* Function returs all the inputs in a ServiceProfile
		 * 
		 * Inputs: none
		 * 
		 * Return values: an array of inputs
		 */
		private IOType[] GetInputParameters()
		{
			ArrayList lstInputs = new ArrayList();

			try
			{
				XmlNode root = m_doc.DocumentElement;
				
				// Create Expression Builder instance
				IIOPEXPathExprBuilder IBuilder = IOPEXPathExprBuilderFactory.CreateInstance( enuIOPEType.Input );
				// Build XPath Expression
				string strXPathExpr = DAMLConstants.SERVICE_PROFILE + IBuilder.BuildExpression( enuIOPESearchBy.PARAM_DESC );
							
				XmlNodeList nodeList = root.SelectNodes( strXPathExpr, m_mgr );

				foreach( XmlNode descNode in nodeList )
				{
					IOType input = GetIONodeData( descNode );
					
					if( input.isValid )
						lstInputs.Add( input );
				}
			
			}
			catch( Exception e )
			{
				m_EvtLog.WriteEntry( e.Message, EventLogEntryType.Error );
			}

			return (IOType[]) lstInputs.ToArray( typeof(IOType) );
		}
		
		/* Function strips the IONode data from a description node
		 * 
		 * Inputs: descNode - description node
		 * 
		 * Return values: the IOType data retrieved
		 */
		private IOType GetIONodeData( XmlNode descNode )
		{
			IOType ioData = new IOType();
									
			ioData.ParameterDesc = descNode.Attributes[DAMLConstants.RDF_ID].Value;
			
			// Get Param name
			XmlNode nameNode = descNode.SelectSingleNode( DAMLConstants.PROFILE_PARAM_NAME, m_mgr );
			ioData.ParameterName = nameNode.InnerText;
					
			// Get Param RestrictedTo
			XmlNode restrictNode = descNode.SelectSingleNode( DAMLConstants.PROFILE_RESTRICTED_TO, m_mgr );
			ioData.RestrictedTo = restrictNode.Attributes[DAMLConstants.RDF_RESOURCE].Value;
			
			// Get Param RefersTo
			XmlNode referNode = descNode.SelectSingleNode( DAMLConstants.PROFILE_REFERS_TO, m_mgr );
			ioData.RefersTo = referNode.Attributes[DAMLConstants.RDF_RESOURCE].Value;
						
			return ioData;
		}

		/* Function strips the EPNode data from a description node
		 * 
		 * Inputs: descNode - description node
		 * 
		 * Return values: the EPType data retrieved
		 */
		private EPType GetEPNodeData( XmlNode descNode )
		{
			EPType epData = new EPType();
				
			epData.ConditionDesc = descNode.Attributes[DAMLConstants.RDF_ID].Value;
								
			XmlNode nameNode = descNode.SelectSingleNode( DAMLConstants.PROFILE_CONDITION_NAME, m_mgr );
			epData.ConditionName = nameNode.InnerText;
					
			XmlNode stmntNode = descNode.SelectSingleNode( DAMLConstants.PROFILE_STATEMENT, m_mgr );
			epData.Statement = stmntNode.Attributes[DAMLConstants.RDF_RESOURCE].Value;
						
			XmlNode referNode = descNode.SelectSingleNode( DAMLConstants.PROFILE_REFERS_TO, m_mgr );
			epData.RefersTo = referNode.Attributes[DAMLConstants.RDF_RESOURCE].Value;

			return epData;
		}

		/* Function returns the process model uri from the service profile
		 * 
		 * Inputs: none
		 * 
		 * Return value: process model uri
		 */
		private string GetProcessModel()
		{
			string strRetVal = "";

			try
			{
				XmlNode root = m_doc.DocumentElement;
				XmlNode node = root.SelectSingleNode( DAMLConstants.SERVICE_PROFILE + "/" + DAMLConstants.PROFILE_HAS_PROCESS, m_mgr );
				
				if( node == null )
					return "";

				strRetVal = node.Attributes[DAMLConstants.RDF_RESOURCE].Value;
			}
			catch( Exception e )
			{
				m_EvtLog.WriteEntry( e.Message, EventLogEntryType.Error );
			}

			return strRetVal;
		}

		/* Function returns the quality rating data from the service profile
		 * 
		 * Inputs: none
		 * 
		 * Return value: quality rating data
		 */
		private string GetQualityRating()
		{
			string strRetVal = "";

			try
			{
				XmlNode root = m_doc.DocumentElement;
				XmlNode node = root.SelectSingleNode( DAMLConstants.SERVICE_PROFILE + "/" + DAMLConstants.PROFILE_QUALITY_RATING, m_mgr );
				
				if( node == null )
					return "";

				strRetVal = node.Attributes[DAMLConstants.RDF_RESOURCE].Value;
			}
			catch( Exception e )
			{
				m_EvtLog.WriteEntry( e.Message, EventLogEntryType.Error );
			}

			return strRetVal;
		}
		
		/* Function returns the geographical radius data from the service profile
		 * 
		 * Inputs: none
		 * 
		 * Return value: geographical radius data
		 */
		private string GetGeographicRadius()
		{
			string strRetVal = "";

			try
			{
				XmlNode root = m_doc.DocumentElement;
				XmlNode node = root.SelectSingleNode( DAMLConstants.SERVICE_PROFILE + "/" + DAMLConstants.PROFILE_GEOG_RADIUS, m_mgr );
				
				if( node == null )
					return "";

				strRetVal = node.Attributes[DAMLConstants.RDF_RESOURCE].Value;
			}
			catch( Exception e )
			{
				m_EvtLog.WriteEntry( e.Message, EventLogEntryType.Error );
			}

			return strRetVal;
		}

		/* Function returns the web url data from the service profile
		 * 
		 * Inputs: none
		 * 
		 * Return value: web url data
		 */
		private string GetWebUrl()
		{
			string strRetVal = "";
			
			try
			{
				XmlNode root = m_doc.DocumentElement;
				XmlNode node = root.SelectSingleNode( DAMLConstants.SERVICE_PROFILE + "/" + DAMLConstants.PROFILE_PROVIDED_BY + "/" + DAMLConstants.PROFILE_SERVICE_PROVIDER + "/" + DAMLConstants.PROFILE_WEB_URL, m_mgr );
				
				if( node == null )
					return "";
				
				strRetVal = node.InnerText;
			}
			catch( Exception e )
			{
				m_EvtLog.WriteEntry( e.Message, EventLogEntryType.Error );
			}

			return strRetVal;
		}

		/* Function returns the physical address data from the service profile
		 * 
		 * Inputs: none
		 * 
		 * Return value: physical address data
		 */
		private string GetPhysicalAddress()
		{
			string strRetVal = "";
			
			try
			{
				XmlNode root = m_doc.DocumentElement;
				XmlNode node = root.SelectSingleNode( DAMLConstants.SERVICE_PROFILE + "/" + DAMLConstants.PROFILE_PROVIDED_BY + "/" + DAMLConstants.PROFILE_SERVICE_PROVIDER + "/" + DAMLConstants.PROFILE_PHYSICAL_ADDRESS, m_mgr );
				
				if( node == null )
					return "";
				
				strRetVal = node.InnerText;
			}
			catch( Exception e )
			{
				m_EvtLog.WriteEntry( e.Message, EventLogEntryType.Error );
			}

			return strRetVal;
		}

		/* Function returns the profile email data from the service profile
		 * 
		 * Inputs: none
		 * 
		 * Return value: profile email data
		 */
		private string GetProfileEmail()
		{
			string strRetVal = "";
			
			try
			{
				XmlNode root = m_doc.DocumentElement;
				XmlNode node = root.SelectSingleNode( DAMLConstants.SERVICE_PROFILE + "/" + DAMLConstants.PROFILE_PROVIDED_BY + "/" + DAMLConstants.PROFILE_SERVICE_PROVIDER + "/" + DAMLConstants.PROFILE_EMAIL, m_mgr );
				
				if( node == null )
					return "";
				
				strRetVal = node.InnerText;
			}
			catch( Exception e )
			{
				m_EvtLog.WriteEntry( e.Message, EventLogEntryType.Error );
			}

			return strRetVal;
		}

		/* Function returns the profile fax data from the service profile
		 * 
		 * Inputs: none
		 * 
		 * Return value: profile fax data
		 */
		private string GetProfileFax()
		{
			string strRetVal = "";
			
			try
			{
				XmlNode root = m_doc.DocumentElement;
				XmlNode node = root.SelectSingleNode( DAMLConstants.SERVICE_PROFILE + "/" + DAMLConstants.PROFILE_PROVIDED_BY + "/" + DAMLConstants.PROFILE_SERVICE_PROVIDER + "/" + DAMLConstants.PROFILE_FAX, m_mgr );
				
				if( node == null )
					return "";
				
				strRetVal = node.InnerText;
			}
			catch( Exception e )
			{
				m_EvtLog.WriteEntry( e.Message, EventLogEntryType.Error );
			}

			return strRetVal;
		}
		
		/* Function returns the profile phone data from the service profile
		 * 
		 * Inputs: none
		 * 
		 * Return value: profile phone data
		 */
		private string GetProfilePhone()
		{
			string strRetVal = "";
			
			try
			{
				XmlNode root = m_doc.DocumentElement;
				XmlNode node = root.SelectSingleNode( DAMLConstants.SERVICE_PROFILE + "/" + DAMLConstants.PROFILE_PROVIDED_BY + "/" + DAMLConstants.PROFILE_SERVICE_PROVIDER + "/" + DAMLConstants.PROFILE_PHONE, m_mgr );
				
				if( node == null )
					return "";
				
				strRetVal = node.InnerText;
			}
			catch( Exception e )
			{
				m_EvtLog.WriteEntry( e.Message, EventLogEntryType.Error );
			}

			return strRetVal;
		}
	
		/* Function returns the profile name data from the service profile
		 * 
		 * Inputs: none
		 * 
		 * Return value: profile name data
		 */
		private string GetProfileName()
		{
			string strRetVal = "";
			
			try
			{
				XmlNode root = m_doc.DocumentElement;
				XmlNode node = root.SelectSingleNode( DAMLConstants.SERVICE_PROFILE + "/" + DAMLConstants.PROFILE_PROVIDED_BY + "/" + DAMLConstants.PROFILE_SERVICE_PROVIDER + "/" + DAMLConstants.PROFILE_NAME, m_mgr );
				
				if( node == null )
					return "";
				
				strRetVal = node.InnerText;
			}
			catch( Exception e )
			{
				m_EvtLog.WriteEntry( e.Message, EventLogEntryType.Error );
			}

			return strRetVal;
		}

		/* Function returns the profile service provider data from the service profile
		 * 
		 * Inputs: none
		 * 
		 * Return value: profile service provider data
		 */
		private string GetServiceProvider()
		{
			string strRetVal = "";

			try
			{
				XmlNode root = m_doc.DocumentElement;
				XmlNode node = root.SelectSingleNode( DAMLConstants.SERVICE_PROFILE + "/" + DAMLConstants.PROFILE_PROVIDED_BY + "/" + DAMLConstants.PROFILE_SERVICE_PROVIDER, m_mgr );
				
				if( node == null )
					return "";

				strRetVal = node.Attributes[DAMLConstants.RDF_ID].Value;
							
			}
			catch( Exception e )
			{
				m_EvtLog.WriteEntry( e.Message, EventLogEntryType.Error );
			}
			
			return strRetVal;
		}
		
		/* Function returns the profile RequestedBy data from the service profile
		 * 
		 * Inputs: none
		 * 
		 * Return value: profile RequestedBy data
		 */
		private string GetRequestedBy()
		{
			string strRetVal = "";

			try
			{
				XmlNode root = m_doc.DocumentElement;
				XmlNode node = root.SelectSingleNode( DAMLConstants.SERVICE_PROFILE + "/" + DAMLConstants.PROFILE_REQUESTED_BY, m_mgr );
				
				if( node == null )
					return "";

				strRetVal = node.InnerText;
			}
			catch( Exception e )
			{
				m_EvtLog.WriteEntry( e.Message, EventLogEntryType.Error );
			}

			return strRetVal;
		}

		/* Function returns the profile IntendedPurpose data from the service profile
		 * 
		 * Inputs: none
		 * 
		 * Return value: profile IntendedPurpose data
		 */
		private string GetIntendedPurpose()
		{
			string strRetVal = "";

			try
			{
				XmlNode root = m_doc.DocumentElement;
				XmlNode node = root.SelectSingleNode( DAMLConstants.SERVICE_PROFILE + "/" + DAMLConstants.PROFILE_INTENDED_PURPOSE, m_mgr );
				
				if( node == null )
					return "";

				strRetVal = node.InnerText;
			}
			catch( Exception e )
			{
				m_EvtLog.WriteEntry( e.Message, EventLogEntryType.Error );
			}

			return strRetVal;
		}

		/* Function returns the profile TextDescription data from the service profile
		 * 
		 * Inputs: none
		 * 
		 * Return value: profile TextDescription data
		 */
		private string GetTextDescription()
		{
			string strRetVal = "";

			try
			{
				XmlNode root = m_doc.DocumentElement;
				XmlNode node = root.SelectSingleNode( DAMLConstants.SERVICE_PROFILE + "/" + DAMLConstants.PROFILE_TEXT_DESC, m_mgr );
				
				if( node == null )
					return "";

				strRetVal = node.InnerText;
			}
			catch( Exception e )
			{
				m_EvtLog.WriteEntry( e.Message, EventLogEntryType.Error );
			}

			return strRetVal;
		}

		/* Function returns the profile PresentedBy data from the service profile
		 * 
		 * Inputs: none
		 * 
		 * Return value: profile PresentedBy data
		 */
		private string GetPresentedBy()
		{
			string strRetVal = "";

			try
			{
				XmlNode root = m_doc.DocumentElement;
				XmlNode node = root.SelectSingleNode( DAMLConstants.SERVICE_PROFILE + "/" + DAMLConstants.SERVICE_PRESENTED_BY, m_mgr );
				
				if( node == null )
					return "";

				strRetVal = node.Attributes[DAMLConstants.RDF_RESOURCE].Value;
			}
			catch( Exception e )
			{
				m_EvtLog.WriteEntry( e.Message, EventLogEntryType.Error );
			}

			return strRetVal;
		}
		
		/* Function returns the ServiceProfile data from the service profile
		 * 
		 * Inputs: none
		 * 
		 * Return value: ServiceProfile data
		 */
		private string GetServiceProfile()
		{
			string strRetVal = "";
			
			try
			{
				XmlNode root = m_doc.DocumentElement;
				XmlNode node = root.SelectSingleNode( DAMLConstants.SERVICE_PROFILE, m_mgr );

				if( node == null )
					return "";

				strRetVal = node.Attributes[DAMLConstants.RDF_ID].Value;
			}
			catch( Exception e )
			{
				m_EvtLog.WriteEntry( e.Message, EventLogEntryType.Error );
			}

			return strRetVal;
		}
		
		/* Function returns the ServiceName data from the service profile
		 * 
		 * Inputs: none
		 * 
		 * Return value: ServiceName data
		 */
		private string GetServiceName()
		{
			string strRetVal = "";

			try
			{
				XmlNode root = m_doc.DocumentElement;
				XmlNode node = root.SelectSingleNode( DAMLConstants.SERVICE_PROFILE + "/" + DAMLConstants.PROFILE_SERVICE_NAME, m_mgr );
				
				if( node == null )
					return "";

				strRetVal = node.InnerText;
			}
			catch( Exception e )
			{
				m_EvtLog.WriteEntry( e.Message, EventLogEntryType.Error );
			}

			return strRetVal;
		}

	}				
}
