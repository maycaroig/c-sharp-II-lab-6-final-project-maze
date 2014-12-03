using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MazeProject
{
	/// <summary>
	/// Interaction logic for MazeCellControl.xaml
	/// </summary>
	public partial class MazeCellControl : UserControl
	{
		#region Fields
		private DirectionEnum m_Borders = DirectionEnum.None;
		private DirectionEnum m_Path = DirectionEnum.None;
		private bool m_IsStart = false;
		public bool m_IsEnd = false;
		#endregion Fields

		#region Properties
		/// <summary>
		/// Path
		/// </summary>
		public DirectionEnum Path
		{
			get
			{
				return m_Path;
			}
			set
			{
				m_Path = value;
				north.Visibility = value.HasFlag(DirectionEnum.North) ? Visibility.Visible : Visibility.Collapsed;
				south.Visibility = value.HasFlag(DirectionEnum.South) ? Visibility.Visible : Visibility.Collapsed;
				east.Visibility = value.HasFlag(DirectionEnum.East) ? Visibility.Visible : Visibility.Collapsed;
				west.Visibility = value.HasFlag(DirectionEnum.West) ? Visibility.Visible : Visibility.Collapsed;
				center.Visibility = value == DirectionEnum.None ? Visibility.Collapsed : Visibility.Visible;
			}
		}

		/// <summary>
		/// Walls
		/// </summary>
		public DirectionEnum Walls
		{
			get
			{
				return m_Borders;
			}
			set
			{
				m_Borders = value;
				const double WALL_THICKNESS = 1;
				double left = value.HasFlag(DirectionEnum.West) ? WALL_THICKNESS : 0.0000001;
				double top = value.HasFlag(DirectionEnum.North) ? WALL_THICKNESS : 0.0000001;
				double right = value.HasFlag(DirectionEnum.East) ? WALL_THICKNESS : 0.0000001;
				double bottom = value.HasFlag(DirectionEnum.South) ? WALL_THICKNESS : 0.0000001;
				border.BorderThickness = new Thickness(left, top, right, bottom);
			}
		}

		/// <summary>
		/// IsStart
		/// </summary>
		public bool IsStart
		{
			get
			{
				return m_IsStart;
			}
			set
			{
				m_IsStart = value;
				start.Visibility = value ? Visibility.Visible : Visibility.Collapsed;
			}
		}

		/// <summary>
		/// IsEnd
		/// </summary>
		public bool IsEnd
		{
			get
			{
				return m_IsEnd;
			}
			set
			{
				m_IsEnd = value;
				end.Visibility = value ? Visibility.Visible : Visibility.Collapsed;
			}
		}
		#endregion Properties

		#region Constructors
		public MazeCellControl()
		{
			InitializeComponent();
		}
		#endregion Constructors
	}
}
