using System;
using System.Collections;

namespace PSL.DAML
{
	/// <summary>
	/// Abstract base class for daml:Class elements
	/// </summary>
	public abstract class DamlClass:DamlTypeDefinition
	{
		protected enuDamlClassType m_damlClassType = enuDamlClassType.rdfsSubClassOf;
		protected enuRdfParseType m_parseType = enuRdfParseType.damlCollection;
		protected string m_strValue = "";
		protected Hashtable m_options = new Hashtable();

		/// <summary>
		/// Ctor.
		/// </summary>
		public DamlClass()
		{
			this.m_damlType = enuDamlType.damlClass;
		}
		
		/// <summary>
		/// Property gets/sets the value of this instance
		/// </summary>
		public virtual string Value
		{
			get
			{ return this.m_strValue; }
			set
			{
				if( value == null || value.Length == 0 )
					return;

				if( !this.m_options.Contains( value ) )
					throw new Exception( "Value can only be set to one of the options already added." );

				this.m_strValue = value;
			}
		}

		/// <summary>
		/// Property gets the daml class type of this instance
		/// </summary>
		public virtual enuDamlClassType DamlClassType
		{
			get
			{ return this.m_damlClassType; }
		}
		
		/// <summary>
		/// Property gets/sets the parseType of this instance
		/// </summary>
		public virtual enuRdfParseType ParseType
		{
			get
			{ return this.m_parseType; }
			set
			{ this.m_parseType = value; }
		}

		/// <summary>
		/// Procedure adds an option to the instance (applicable for daml:collection
		/// parseTypes)
		/// </summary>
		/// <param name="strOption"></param>
		public virtual void AddOption( string strOption )
		{
			if( strOption == null || strOption.Length == 0 )
				return;
			
			if( !this.m_options.ContainsKey( strOption ) )
				this.m_options.Add( strOption, strOption.TrimStart( new char[] { '#' } ) );
		}

		/// <summary>
		/// Procedure removes an option from the instance (applicable for daml:collection
		/// parseTypes)
		/// </summary>
		/// <param name="strOption"></param>
		public virtual void RemoveOption( string strOption )
		{
			if( strOption == null || strOption.Length == 0 )
				return;
		
			this.m_options.Remove( strOption );
		}

		/// <summary>
		/// Procedure resets the instance
		/// </summary>
		public override void Clear()
		{
			this.m_strName = "";
			this.m_strValue = "";
			this.m_options.Clear();
		}
	}
}
