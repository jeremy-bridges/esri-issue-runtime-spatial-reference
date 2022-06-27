using Esri.ArcGISRuntime.Mapping;
using System;
using System.Windows;

namespace ShowSpatialReferenceIssues.NetFramework
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent(); 
            DataContext = new MainWindowViewModel(
                message => Log.AppendText(message + Environment.NewLine),
                geometry =>
                {
                    MapView.SetViewpoint(new Viewpoint(geometry));
                });
        }
    }
}
