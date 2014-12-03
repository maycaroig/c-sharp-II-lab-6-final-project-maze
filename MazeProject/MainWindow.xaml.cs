using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
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
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		#region Fields
		private Maze m_Maze = null;
		private MazeCellControl[,] m_MazeCellsUI = null;
		#endregion Fields

		#region Constructors
		public MainWindow()
		{
			InitializeComponent();
		}
		#endregion Constructors

		#region Methods
		/// <summary>
		/// Show the generated maze
		/// </summary>
		/// <param name="task">Task that created the maze (if applicable)</param>
		private void ShowMaze(Task task = null)
		{
			try
			{
				// Setup maze grid
				mazeGrid.Children.Clear();
				m_MazeCellsUI = null;

				if (m_Maze != null)
				{
					mazeGrid.Columns = m_Maze.Width;
					mazeGrid.Rows = m_Maze.Height;
					m_MazeCellsUI = new MazeCellControl[m_Maze.Width, m_Maze.Height];

					for (int y = 0; y < m_Maze.Height; y++)
					{
						for (int x = 0; x < m_Maze.Width; x++)
						{
							MazeCell cell = m_Maze[x, y];
							MazeCellControl mazeCellUI = new MazeCellControl();
							mazeGrid.Children.Add(mazeCellUI);
							mazeCellUI.Walls = cell.Walls;
							m_MazeCellsUI[x, y] = mazeCellUI;
						}
					}
					m_MazeCellsUI[m_Maze.StartPoint.X, m_Maze.StartPoint.Y].IsStart = true;
					m_MazeCellsUI[m_Maze.EndPoint.X, m_Maze.EndPoint.Y].IsEnd = true;
				}
			}
			catch (Exception ex)
			{
				System.Reflection.MethodBase mb = System.Reflection.MethodBase.GetCurrentMethod();
				System.Diagnostics.Trace.WriteLine(ex.Message, string.Format("{0}.{1}.{2}", mb.DeclaringType.Namespace, mb.DeclaringType.Name, mb.Name));
				MessageBox.Show("Error showing maze: " + ex.Message, "Error");
			}
			finally
			{
				commandsPanel.IsEnabled = true;
			}
		}

		/// <summary>
		/// Shows the results of solving the maze
		/// </summary>
		/// <param name="result">Task that contains the results</param>
		private void ShowResults(Position start, Position end, List<DirectionEnum> directions)
		{
			try
			{
				m_MazeCellsUI[start.X, start.Y].IsStart = true;
				m_MazeCellsUI[end.X, end.Y].IsEnd = true;
				DirectionEnum previous = DirectionEnum.None;
				Position current = start;
				foreach (DirectionEnum direction in directions)
				{
					Console.WriteLine(direction);
					m_MazeCellsUI[current.X, current.Y].Path = previous.GetOpposite() | direction;
					current = current.MovePosition(direction);
					Console.WriteLine("{0} => {1}", previous, direction);
					previous = direction;
				}
				m_MazeCellsUI[current.X, current.Y].Path = previous.GetOpposite();
			}
			catch (Exception ex)
			{
				System.Reflection.MethodBase mb = System.Reflection.MethodBase.GetCurrentMethod();
				System.Diagnostics.Debug.WriteLine(ex.Message, string.Format("{0}.{1}.{2}", mb.DeclaringType.Namespace, mb.DeclaringType.Name, mb.Name));
				MessageBox.Show("Error showing results: " + ex.Message, "Error");
			}
		}
		#endregion Methods

		#region Event Handlers
		/// <summary>
		/// Generate a maze
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void generateButton_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				int width = 0;
				int height = 0;
				int complexity = 0;

				if ((!int.TryParse(widthTextBox.Text, out width)) || ((width < Constants.MIN_WIDTH) || (width > Constants.MAX_WIDTH)))
				{
					string msg = string.Format("Invalid width.  Value must be a whole number between {0} and {1}.", Constants.MIN_WIDTH, Constants.MAX_WIDTH);
					MessageBox.Show(msg, "Entry Error");
					return;
				}

				if ((!int.TryParse(heightTextBox.Text, out height)) || ((height < Constants.MIN_HEIGHT) || (height > Constants.MAX_HEIGHT)))
				{
					string msg = string.Format("Invalid height.  Value must be a whole number between {0} and {1}.", Constants.MIN_HEIGHT, Constants.MAX_HEIGHT);
					MessageBox.Show(msg, "Entry Error");
					return;
				}

				if ((!int.TryParse(complexityTextBox.Text, out complexity)) || ((complexity < 0) || (complexity > 100)))
				{
					string msg = string.Format("Invalid complexity.  Value must be a whole number between {0} and {1}.", 0, 100);
					MessageBox.Show(msg, "Entry Error");
					return;
				}

				// Create a MazeMaker
				m_Maze = new Maze(width, height);

				// Get handle to UI thread to resynchronize with later...
				var ui = TaskScheduler.FromCurrentSynchronizationContext();

				commandsPanel.IsEnabled = false;

				// Start task to solve maze
				var createMaze = Task.Factory.StartNew(() =>
				{
					m_Maze.CreateMaze((double)complexity / 100);
				});

				// Display results of solving by calling ShowResults on the UI thread
				Task displayResults = createMaze.ContinueWith(ShowMaze, CancellationToken.None, TaskContinuationOptions.OnlyOnRanToCompletion, ui);
			}
			catch (Exception ex)
			{
				System.Reflection.MethodBase mb = System.Reflection.MethodBase.GetCurrentMethod();
				System.Diagnostics.Trace.WriteLine(ex.Message, string.Format("{0}.{1}.{2}", mb.DeclaringType.Namespace, mb.DeclaringType.Name, mb.Name));
			}
		}

		

		/// <summary>
		/// Solve a maze
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void solveButton_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				commandsPanel.IsEnabled = false;
				if (m_Maze != null)
				{
					bool solved = m_Maze.SolveMaze();
					if (solved && m_Maze.Solution != null && m_Maze.Solution.Count > 0)
					{
						ShowResults(m_Maze.StartPoint, m_Maze.EndPoint, m_Maze.Solution);
					}
				}
			}
			catch (Exception ex)
			{
				System.Reflection.MethodBase mb = System.Reflection.MethodBase.GetCurrentMethod();
				System.Diagnostics.Debug.WriteLine(ex.Message, string.Format("{0}.{1}.{2}", mb.DeclaringType.Namespace, mb.DeclaringType.Name, mb.Name));
				MessageBox.Show("Error solving maze: " + ex.Message, "Solve Error");
			}
			finally
			{
				commandsPanel.IsEnabled = true;
			}
		}
		#endregion Event Handlers
	}
}
