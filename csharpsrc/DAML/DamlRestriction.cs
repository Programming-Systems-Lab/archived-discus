using System;
using System.Xml;

namespace PSL.DAML
{
	/// <summary>
	/// DamlRestriction classes represent a restriction in a Daml Process Model
	/// on some attribute of a process.
	/// </summary>
	public sealed class DamlRestriction:IComparable
	{
		public const int NO_CARDINALITY = -1;
		
		private string m_strOwner = "";
		private int m_nCardinality = NO_CARDINALITY;
		private string m_strHasValue = "";
		private string m_strOnProperty = ""; // May or may not have # pre-pended

		public DamlRestriction()
		{
		}

		/// <summary>
		/// Property gets/sets the owner (process attribute) which this restriction
		/// applies to.
		/// </summary>
		public string Owner
		{
			get
			{ return this.m_strOwner; }
			set
			{
				if( value == null || value.Length == 0 )
					return;

				this.m_strOwner = value;
			}
		}

		/// <summary>
		/// Property gets/sets the cardinality of the restriction. This restriction 
		/// applies to its owner e.g. restricting a method to only expect/allow
		/// 1 instance of a named input
		/// </summary>
		public int Cardinality
		{
			get
			{ return this.m_nCardinality; }
			set
			{ 
				if( value < 0 )
					return;

				this.m_nCardinality = value; 
			}
		}

		/// <summary>
		/// Property gets/sets the HasValue of the restriction. Some restrictions may
		/// specify what value a process attribute must be e.g. an input must have
		/// value = "True".
		/// </summary>
		public string HasValue
		{
			get
			{ return this.m_strHasValue; }
			set
			{ 
				if( value == null || value.Length == 0 )
					return;

				this.m_strHasValue = value; 
			}
		}

		/// <summary>
		/// Property gets/sets the name of the process attribute which this restriction
		/// applies to.
		/// </summary>
		public string OnProperty
		{
			get
			{ return this.m_strOnProperty; }
			set
			{ 
				if( value == null || value.Length == 0 )
					return;
				
				this.m_strOnProperty = value; 
			}
		}

		/// <summary>
		/// Function compares 2 DamlRestriction instances. The OnProperty property
		/// is used for comparisons.
		/// </summary>
		/// <param name="obj"></param>
		/// <returns>0 (equal), -1 (less than this instance) 
		/// or 1 (greater than this instance)</returns>
		public int CompareTo( object obj )
		{
			DamlRestriction temp = (DamlRestriction) obj;

			return this.m_strOnProperty.ToLower().CompareTo( temp.m_strOnProperty.ToLower() );
		}
	}
}
