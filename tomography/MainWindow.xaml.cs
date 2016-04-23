﻿using nzy3D.Chart;
using nzy3D.Colors;
using nzy3D.Colors.ColorMaps;
using nzy3D.Maths;
using nzy3D.Plot3D.Builder;
using nzy3D.Plot3D.Builder.Concrete;
using nzy3D.Plot3D.Primitives;
using nzy3D.Plot3D.Primitives.Axes.Layout;
using nzy3D.Plot3D.Rendering.Canvas;
using nzy3D.Plot3D.Rendering.View;
using System.ComponentModel;
using System.Windows;

namespace nzy3d_wpfDemo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {

        private nzy3D.Chart.Controllers.Thread.Camera.CameraThreadController t;
        private nzy3D.Chart.Controllers.Thread.Camera.CameraThreadController t2;
        private IAxeLayout axeLayout;
        private IAxeLayout axeLayout2;

        public MainWindow()
        {
            InitializeComponent();
            //Init console
            //ConsoleManager.Show();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        // Create the OnPropertyChanged method to raise the event
        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            buildPlot(new MyMapper(), this.Main, t, axeLayout);
            //buildPlot(new ExpMapper(), this.Exp, t2, axeLayout2);
        }

        private void buildPlot(nzy3D.Plot3D.Builder.Mapper mapper, System.Windows.Controls.Grid element, nzy3D.Chart.Controllers.Thread.Camera.CameraThreadController t, IAxeLayout axeLayout)
        {

            // Create the interop host control.
        System.Windows.Forms.Integration.WindowsFormsHost host =
                    new System.Windows.Forms.Integration.WindowsFormsHost();

            // Create the Renderer 3D control.
            Renderer3D renderer = new Renderer3D();

            // Assign the Renderer 3D control as the host control's child.
            host.Child = renderer;

            // Add the interop host control to the Grid 
            // control's collection of child controls. 
            element.Children.Add(host);

            // Create a range for the graph generation
            Range range = new Range(0, 499);
            int steps = 50;

            // Build a nice surface to display with cool alpha colors 
            // (alpha 0.8 for surface color and 0.5 for wireframe)
            Shape surface = Builder.buildOrthonomal(new OrthonormalGrid(range, steps, range, steps), mapper);
            surface.ColorMapper = new ColorMapper(new ColorMapRainbow(), surface.Bounds.zmin, surface.Bounds.zmax, new Color(1, 1, 1, 0.8));
            surface.FaceDisplayed = true;
            surface.WireframeDisplayed = true;
            surface.WireframeColor = Color.GRAY;
            surface.WireframeColor.mul(new Color(1, 1, 1, 0.5));

            // Create the chart and embed the surface within
            Chart chart = new Chart(renderer, Quality.Intermediate);
            chart.Scene.Graph.Add(surface);
            axeLayout = chart.AxeLayout;

            // Create a mouse control
            nzy3D.Chart.Controllers.Mouse.Camera.CameraMouseController mouse = new nzy3D.Chart.Controllers.Mouse.Camera.CameraMouseController();
            mouse.addControllerEventListener(renderer);
            chart.addController(mouse);

            // This is just to ensure code is reentrant (used when code is not in Form_Load but another reentrant event)
            DisposeBackgroundThread();

            // Create a thread to control the camera based on mouse movements
            t = new nzy3D.Chart.Controllers.Thread.Camera.CameraThreadController();
            t.addControllerEventListener(renderer);
            mouse.addSlaveThreadController(t);
            chart.addController(t);
            t.Start();

            // Associate the chart with current control
            renderer.setView(chart.View);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            DisposeBackgroundThread();
        }

        private void DisposeBackgroundThread()
        {
            if ((t != null))
            {
                t.Dispose();
            }
        }

    }
}