using System;
using System.Collections;

namespace PSL.DISCUS.DAML
{
	public class DAMLProcess
	{
		// All Processes have...
		protected enuProcessType m_Type = enuProcessType.AtomicProcess;
		protected string m_strName = "";
		
		// Each of these types represented as an RDFProperty
		// A collection of inputs 
		protected ArrayList m_arrInputs = new ArrayList();
		// A collection of outputs
		protected ArrayList m_arrOutputs = new ArrayList();
		// A collection of preconditions
		protected ArrayList m_arrPreconditions = new ArrayList();
		// A collection of effects
		protected ArrayList m_arrEffects = new ArrayList();
		// A collection of ConditionalOutputs
		protected ArrayList m_arrConditionalOutputs = new ArrayList();
		// A collection of CoConditions
		protected ArrayList m_arrCoConditions = new ArrayList();
		// A collection of CoOutputs
		protected ArrayList m_arrCoOutputs = new ArrayList();
		// A collection of Parameters
		protected ArrayList m_arrParameters = new ArrayList();
		// A collection of sub-steps (applicable for composite processes only)
		// protected ArrayList m_arrSubProcesses = new ArrayList();
		
		public DAMLProcess()
		{
		}

		// Properties
		public enuProcessType ProcessType
		{
			get
			{ return m_Type; }
			set
			{ m_Type = value; }
		}

		public string Name
		{
			get
			{ return m_strName; }
			set
			{ m_strName = value; }
		}
		
		public RDFProperty[] Inputs
		{
			get
			{ return (RDFProperty[]) m_arrInputs.ToArray( typeof(RDFProperty) ); }
		}

		public RDFProperty[] Outputs
		{
			get
			{ return (RDFProperty[]) m_arrOutputs.ToArray( typeof(RDFProperty) ); }
		}

		public RDFProperty[] Preconditions
		{
			get
			{ return (RDFProperty[]) m_arrPreconditions.ToArray( typeof(RDFProperty) ); }
		}

		public RDFProperty[] Effects
		{
			get
			{ return (RDFProperty[]) m_arrEffects.ToArray( typeof(RDFProperty) ); }
		}

		public RDFProperty[] ConditionalOutputs
		{
			get
			{ return (RDFProperty[]) m_arrConditionalOutputs.ToArray( typeof(RDFProperty) ); }
		}

		public RDFProperty[] CoConditions
		{
			get
			{ return (RDFProperty[]) m_arrCoConditions.ToArray( typeof(RDFProperty) ); }
		}

		public RDFProperty[] CoOutputs
		{
			get
			{ return (RDFProperty[]) m_arrCoOutputs.ToArray( typeof(RDFProperty) ); }
		}

		public RDFProperty[] Parameters
		{
			get
			{ return (RDFProperty[]) m_arrParameters.ToArray( typeof(RDFProperty) ); }
		}

		
		// AddXXXX
		public void AddInput( RDFProperty data )
		{
			m_arrInputs.Add( data );
		}

		public void AddInput( RDFProperty[] arrData )
		{
			m_arrInputs.AddRange( arrData );
		}

		public void AddOutput( RDFProperty data )
		{
			m_arrOutputs.Add( data );
		}

		public void AddOutput( RDFProperty[] arrData )
		{
			m_arrOutputs.AddRange( arrData );
		}
		public void AddPrecondition( RDFProperty data )
		{
			m_arrPreconditions.Add( data );
		}

		public void AddPrecondition( RDFProperty[] arrData )
		{
			m_arrPreconditions.AddRange( arrData );
		}

		public void AddEffect( RDFProperty data )
		{
			m_arrEffects.Add( data );
		}
		public void AddEffect( RDFProperty[] arrData )
		{
			m_arrEffects.AddRange( arrData );
		}
		
		public void AddConditionalOutput( RDFProperty data )
		{
			m_arrConditionalOutputs.Add( data );
		}
		
		public void AddConditionalOutput( RDFProperty[] arrData )
		{
			m_arrConditionalOutputs.AddRange( arrData );
		}

		public void AddCoCondition( RDFProperty data )
		{
			m_arrCoConditions.Add( data );
		}
		public void AddCoCondition( RDFProperty[] arrData )
		{
			m_arrCoConditions.AddRange( arrData );
		}
		public void AddCoOutput( RDFProperty data )
		{
			m_arrCoOutputs.Add( data );
		}
		public void AddCoOutput( RDFProperty[] arrData )
		{
			m_arrCoOutputs.AddRange( arrData );
		}
		public void AddParameter( RDFProperty data )
		{
			m_arrParameters.Add( data );
		}
		public void AddParameter( RDFProperty[] arrData )
		{
			m_arrParameters.AddRange( arrData );
		}


		// ClearXXXX
		public void ClearInputs()
		{
			m_arrInputs.Clear();
		}

		public void ClearOutputs()
		{
			m_arrOutputs.Clear();
		}

		public void ClearPreconditons()
		{
			m_arrPreconditions.Clear();
		}

		public void ClearEffects()
		{
			m_arrEffects.Clear();
		}

		public void ClearConditionalOutputs()
		{
			m_arrConditionalOutputs.Clear();
		}

		public void ClearCoConditions()
		{
			m_arrCoConditions.Clear();
		}

		public void ClearCoOutputs()
		{
			m_arrCoOutputs.Clear();
		}

		public void ClearParameters()
		{
			m_arrParameters.Clear();
		}

		// HasXXXX
		public bool HasInputs
		{
			get
			{ return m_arrInputs.Count > 0; }
		}

		public bool HasOutputs
		{
			get
			{ return m_arrOutputs.Count > 0; }
		}

		public bool HasPreconditions
		{
			get
			{ return m_arrPreconditions.Count > 0; }
		}
		public bool HasEffects
		{
			get
			{ return m_arrEffects.Count > 0; }
		}

		public bool HasConditionalOutputs
		{
			get
			{ return m_arrConditionalOutputs.Count > 0; }
		}

		public bool HasCoConditions
		{
			get
			{ return m_arrCoConditions.Count > 0; }
		}
		public bool HasCoOutputs
		{
			get
			{ return m_arrCoOutputs.Count > 0; }
		}
		public bool HasParameters
		{
			get
			{ return m_arrParameters.Count > 0; }
		}

	}
}
