using System;
using System.Collections;

namespace PSL.DAML
{
	/// <summary>
	/// Abstract base class of Daml Type Definitions
	/// </summary>
	public abstract class DamlTypeDefinition
	{
		protected string m_strName = "";
		protected enuDamlType m_damlType = enuDamlType.rdfProperty;
		
		/// <summary>
		/// Property gets the Daml Type of an instance
		/// </summary>
		public virtual enuDamlType DamlType
		{
			get
			{ return this.m_damlType; }
		}

		/// <summary>
		/// Property gets/sets the name of a Daml Type
		/// </summary>
		public virtual string Name
		{
			get
			{ return this.m_strName; }
			set
			{
				if( value == null || value.Length == 0 )
					return;

				this.m_strName = value;
			}
		}

		/// <summary>
		/// Ctor.
		/// </summary>
		public DamlTypeDefinition()
		{}

		/// <summary>
		/// Abstract method implemented by derived classes to reset an instance
		/// </summary>
		public abstract void Clear();

		/// <summary>
		/// Abstract method implemented by derived classes to convert an instance
		/// to Xml
		/// </summary>
		/// <returns></returns>
		public abstract string ToXml();
	}
}
