using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MazeProject
{
	/// <summary>
	/// Range represents a range of values
	/// </summary>
	/// <typeparam name="T">Data type for the range (must be a value type and IComparable)</typeparam>
	public struct Range<T> where T: struct, IComparable
	{
		#region Fields
		private T m_Min;
		private T m_Max;
		#endregion Fields

		#region Properties
		/// <summary>
		/// Min
		/// </summary>
		public T Min
		{
			get
			{
				return m_Min;
			}
			set
			{
				int compare = value.CompareTo(Max);
				if (compare <= 0)
				{
					m_Min = value;
				}
				else
				{
					throw new ArgumentOutOfRangeException("Min", "Min must be less than or equal to Max");
				}
			}
		}
		
		/// <summary>
		/// Max
		/// </summary>
		public T Max
		{
			get
			{
				return m_Max;
			}
			set
			{
				int compare = value.CompareTo(Min);
				if (compare >= 0)
				{
					m_Max = value;
				}
				else
				{
					throw new ArgumentOutOfRangeException("Max", "Max must be greater than or equal to Min");
				}
			}
		}
		#endregion Properties

		#region Constructors
		/// <summary>
		/// Constructs a range using the given values
		/// </summary>
		/// <param name="min">Minimum value of range</param>
		/// <param name="max">Maximum value of range</param>
		public Range(T min, T max)
		{
			int compare = min.CompareTo(max);
			if (compare > 0)
			{
				throw new ArgumentOutOfRangeException("max", "max must be greater than or equal to min");
			}
			// Use fields instead of properties since this is a struct
			m_Min = min;
			m_Max = max;
		}
		#endregion Constructors

		#region Methods
		public override string ToString()
		{
			return string.Format("[{0} - {1}]", Min, Max);
		}

		/// <summary>
		/// Determine if the given value lies between the Min and Max values (inclusive)
		/// </summary>
		/// <param name="value">Value to test for containment</param>
		/// <returns>true if contained, false otherwise</returns>
		public bool Contains(T value)
		{
			int compare = Min.CompareTo(value);
			if (compare <= 0)
			{
				compare = Max.CompareTo(value);
				return (compare >= 0);
			}
			return false;
		}
		#endregion Methods
	}
}
