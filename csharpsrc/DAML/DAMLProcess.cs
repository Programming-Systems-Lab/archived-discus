using System;
using System.Collections;

namespace PSL.DAML
{
	// 
	public enum enuProcessSubTaskType
	{
		Sequence, // process:Sequence attribute
		Choice	// process:Choice attribute
	};

	public class DAMLProcess
	{
		// All Processes have...
		protected enuProcessType m_Type = enuProcessType.AtomicProcess;
		protected string m_strName = "";
		// Map input names to number of restrictions
		protected Hashtable m_InputRestrictionMap = new Hashtable();
		// Composite processes only have...
		protected enuProcessSubTaskType m_SubTaskType;
		
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
		protected ArrayList m_arrSubProcesses = new ArrayList();
		
		/* Constructor */
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

		// ONLY for Composite processes - should subclass to hide?
		public enuProcessSubTaskType SubTaskType
		{
			get
			{ return m_SubTaskType; }
			set
			{ m_SubTaskType = value; }
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

		public DAMLProcess[] SubProcesses
		{
			get
			{ return (DAMLProcess[]) m_arrSubProcesses.ToArray( typeof(DAMLProcess) ); }
		}
		
		/* Function returns the cardinality restrictions on a given named
		 * input.
		 * 
		 * Input: strInputName - named input
		 * Return value: the cardinality (if the input does not exist 0 returned)
		 */ 
		public int GetInputRestriction( string strInputName )
		{
			if( !m_InputRestrictionMap.ContainsKey( strInputName ) )
				return 0;
			else return (int) m_InputRestrictionMap[strInputName];
		}
		
		/* Procedure Adds an input to a DAMLProcess
		 * 
		 * Inputs: data - RDFProperty details of the input
		 */ 
		public void AddInput( RDFProperty data )
		{
			m_arrInputs.Add( data );
		}

		/* Procedure Adds an array of inputs to a DAMLProcess
		 * 
		 * Inputs: arrData - RDFProperty details of the inputs
		 */ 
		public void AddInput( RDFProperty[] arrData )
		{
			m_arrInputs.AddRange( arrData );
		}

		/* Procedure Adds an output to a DAMLProcess
		 * 
		 * Inputs: data - RDFProperty details of the output
		 */ 
		public void AddOutput( RDFProperty data )
		{
			m_arrOutputs.Add( data );
		}

		/* Procedure Adds an array of outputs to a DAMLProcess
		 * 
		 * Inputs: arrData - RDFProperty details of the outputs
		 */ 
		public void AddOutput( RDFProperty[] arrData )
		{
			m_arrOutputs.AddRange( arrData );
		}
		
		/* Procedure Adds a precondition to a DAMLProcess
		 * 
		 * Inputs: data - RDFProperty details of the precondition
		 */ 
		public void AddPrecondition( RDFProperty data )
		{
			m_arrPreconditions.Add( data );
		}

		/* Procedure Adds an array of preconditions to a DAMLProcess
		 * 
		 * Inputs: arrData - RDFProperty details of the preconditions
		 */ 
		public void AddPrecondition( RDFProperty[] arrData )
		{
			m_arrPreconditions.AddRange( arrData );
		}

		/* Procedure Adds an effect to a DAMLProcess
		 * 
		 * Inputs: data - RDFProperty details of the effect
		 */ 
		public void AddEffect( RDFProperty data )
		{
			m_arrEffects.Add( data );
		}
		
		/* Procedure Adds an array of effects to a DAMLProcess
		 * 
		 * Inputs: arrData - RDFProperty details of the effects
		 */ 
		public void AddEffect( RDFProperty[] arrData )
		{
			m_arrEffects.AddRange( arrData );
		}
		
		/* Procedure Adds a conditional output to a DAMLProcess
		 * 
		 * Inputs: data - RDFProperty details of the conditional output
		 */ 
		public void AddConditionalOutput( RDFProperty data )
		{
			m_arrConditionalOutputs.Add( data );
		}
		
		/* Procedure Adds an array of conditional outputs to a DAMLProcess
		 * 
		 * Inputs: arrData - RDFProperty details of the conditional outputs
		 */ 
		public void AddConditionalOutput( RDFProperty[] arrData )
		{
			m_arrConditionalOutputs.AddRange( arrData );
		}

		/* Procedure Adds a co-condition to a DAMLProcess
		 * 
		 * Inputs: data - RDFProperty details of the co-condition
		 */ 
		public void AddCoCondition( RDFProperty data )
		{
			m_arrCoConditions.Add( data );
		}
		
		/* Procedure Adds an array of co-conditions to a DAMLProcess
		 * 
		 * Inputs: arrData - RDFProperty details of the co-conditions
		 */ 
		public void AddCoCondition( RDFProperty[] arrData )
		{
			m_arrCoConditions.AddRange( arrData );
		}
		
		/* Procedure Adds a co-output to a DAMLProcess
		 * 
		 * Inputs: data - RDFProperty details of the co-output
		 */ 
		public void AddCoOutput( RDFProperty data )
		{
			m_arrCoOutputs.Add( data );
		}

		/* Procedure Adds an array of co-outputs to a DAMLProcess
		 * 
		 * Inputs: arrData - RDFProperty details of the co-outputs
		 */ 
		public void AddCoOutput( RDFProperty[] arrData )
		{
			m_arrCoOutputs.AddRange( arrData );
		}
		
		/* Procedure Adds a parameter to a DAMLProcess
		 * 
		 * Inputs: data - RDFProperty details of the parameter
		 */ 
		public void AddParameter( RDFProperty data )
		{
			m_arrParameters.Add( data );
		}
		
		/* Procedure Adds an array of parameters to a DAMLProcess
		 * 
		 * Inputs: arrData - RDFProperty details of the parameters
		 */ 
		public void AddParameter( RDFProperty[] arrData )
		{
			m_arrParameters.AddRange( arrData );
		}
		
		/* Procedure Adds a subprocess to a (composite) DAMLProcess
		 * 
		 * Inputs: data - DAMLProcess details of the process
		 * 
		 * Exceptions: throws InvalidOperationException if the process is not being
		 *			   added to a Composite DAMLProcess. ONLY Composite DAMLProcesses
		 *			   support subprocesses.
		 */ 
		public void AddSubProcess( DAMLProcess data )
		{
			if( ProcessType == enuProcessType.CompositeProcess )
				m_arrSubProcesses.Add( data );
			else throw new InvalidOperationException( "Only Composite Processes can have SubProcesses" );
		}

		/* Procedure Adds an array of subprocess to a (composite) DAMLProcess
		 * 
		 * Inputs: arrData - DAMLProcess details of the processes
		 * 
		 * Exceptions: throws InvalidOperationException if the processes are not being
		 *			   added to a Composite DAMLProcess. ONLY Composite DAMLProcesses
		 *			   support subprocesses.
		 */ 
		public void AddSubProcess( DAMLProcess[] arrData )
		{
			if( ProcessType == enuProcessType.CompositeProcess )
				m_arrSubProcesses.AddRange( arrData );
			else throw new InvalidOperationException( "Only Composite Processes can have SubProcesses" );
		}

		/* Procedure adds an input (cardinality) restriction 
		 * 
		 * Inputs: strInput - named input
		 *		   nRestriction - cardinality of strInput
		 */ 
		public void AddInputRestriction( string strInput, int nRestriction )
		{
			m_InputRestrictionMap.Add( strInput, nRestriction );
		}
		
		/* Procedure clears all inputs
		 */ 
		public void ClearInputs()
		{
			m_arrInputs.Clear();
		}

		/* Procedure clears all outputs
		 */ 
		public void ClearOutputs()
		{
			m_arrOutputs.Clear();
		}

		/* Procedure clears all preconditions
		 */ 
		public void ClearPreconditons()
		{
			m_arrPreconditions.Clear();
		}

		/* Procedure clears all effects
		 */ 
		public void ClearEffects()
		{
			m_arrEffects.Clear();
		}

		/* Procedure clears all conditional outputs
		 */ 
		public void ClearConditionalOutputs()
		{
			m_arrConditionalOutputs.Clear();
		}

		/* Procedure clears all co-conditions
		 */ 
		public void ClearCoConditions()
		{
			m_arrCoConditions.Clear();
		}

		/* Procedure clears all co-outputs
		 */ 
		public void ClearCoOutputs()
		{
			m_arrCoOutputs.Clear();
		}

		/* Procedure clears all parameters
		 */ 
		public void ClearParameters()
		{
			m_arrParameters.Clear();
		}

		/* Procedure clears all sub processes
		 */ 
		public void ClearSubProcesses()
		{
			m_arrSubProcesses.Clear();
		}
		
		/* Procedure clears the input restriction map
		 */ 
		public void ClearRestrictionMap()
		{
			m_InputRestrictionMap.Clear();
		}

		/* Procedure clears all the constituent data structures used to store
		 * process info
		 */ 
		public void ClearAll()
		{
			this.ClearCoConditions();
			this.ClearConditionalOutputs();
			this.ClearCoOutputs();
			this.ClearEffects();
			this.ClearInputs();
			this.ClearOutputs();
			this.ClearParameters();
			this.ClearPreconditons();
			this.ClearRestrictionMap();
			this.ClearSubProcesses();
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
		
		public bool HasSubProcesses
		{
			get
			{
				// Only Composite Processes can have subprocesses
				if( ProcessType != enuProcessType.CompositeProcess )
					return false;

				return m_arrSubProcesses.Count > 0;
			}
		}

		public bool HasData
		{
			get
			{
				bool bLVal = ( HasInputs || HasOutputs ) || ( HasPreconditions || HasEffects );
				bool bRVal = ( HasConditionalOutputs || HasCoConditions ) || ( HasCoOutputs || HasParameters );
				return ( bLVal || bRVal ) || HasSubProcesses;
			}
		}

		public bool isEmpty
		{
			get
			{ return !HasData; }
		}
	}
}
