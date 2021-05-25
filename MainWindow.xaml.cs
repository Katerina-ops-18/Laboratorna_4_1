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
using System.Windows.Threading;

namespace A1_Graphs {
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow: Window {
		public MainWindow() {
			InitializeComponent();
		}

		readonly Mutex _mutex = new(false);

		double GraphFunc1(double x) => 224 * Math.Sin(x / 32);
		double GraphFunc2(double x) {
			x /= 32;
			return 4 * (x * x) - 2 * x - 22;  // 4*x2-2*x–22
		}
		double GraphFunc3(double x) {
			x /= 32;
			return Math.Log(x * x) / (x * x * x);  // ln(x2)/x3
		}

		void DrawGraph((Polyline graph_r, Polyline graph_l, Func<double, double> f) state) {
			(Polyline graph_r, Polyline graph_l, Func<double, double> f) = state;
			Delegate plotpoint_r = new PlotPoint(PlotPointAtRight);
			Delegate plotpoint_l = new PlotPoint(PlotPointAtLeft);

			const double step = 1;  // Повинен бути 1/2^n.
			const double width = 1920;

			if(double.IsFinite(f(0))) {
				_mutex.WaitOne();
				Dispatcher.Invoke(plotpoint_r, DispatcherPriority.SystemIdle, graph_r, new Point(0, -f(0)));
				_mutex.ReleaseMutex();
			}
			for(double x = step;x <= width / 2;x += step) {
				_mutex.WaitOne();
				Dispatcher.Invoke(plotpoint_r, DispatcherPriority.SystemIdle, graph_r, new Point(x, -f(x)));
				Dispatcher.Invoke(plotpoint_l, DispatcherPriority.SystemIdle, graph_l, new Point(-x, -f(-x)));
				_mutex.ReleaseMutex();
				Thread.Sleep(1);
			}
		}

		delegate void PlotPoint(Polyline graph, Point point);
		void PlotPointAtRight(Polyline graph, Point point) => graph.Points.Add(point);
		void PlotPointAtLeft(Polyline graph, Point point) => graph.Points.Insert(0, point);

		void Window_Loaded(object sender, RoutedEventArgs e) {
			// Запустити наші графіки.
			ThreadPool.QueueUserWorkItem<(Polyline, Polyline, Func<double, double>)>(DrawGraph, (Graph1, Graph1, GraphFunc1), true);
			ThreadPool.QueueUserWorkItem<(Polyline, Polyline, Func<double, double>)>(DrawGraph, (Graph2, Graph2, GraphFunc2), true);
			ThreadPool.QueueUserWorkItem<(Polyline, Polyline, Func<double, double>)>(DrawGraph, (Graph3, Graph3_L, GraphFunc3), true);
		}

		void Window_SizeChanged(object sender, SizeChangedEventArgs e) {
			// Після зміни розміру вікна треба вирівняти наші графіки посередині вікна.
			double h_half = GraphCanvas.ActualHeight / 2;
			double w_half = GraphCanvas.ActualWidth / 2;
			Canvas.SetTop(Graph1, h_half);
			Canvas.SetTop(Graph2, h_half);
			Canvas.SetTop(Graph3, h_half);
			Canvas.SetTop(Graph3_L, h_half);
			Canvas.SetLeft(Graph1, w_half);
			Canvas.SetLeft(Graph2, w_half);
			Canvas.SetLeft(Graph3, w_half);
			Canvas.SetLeft(Graph3_L, w_half);
		}
	}
}
