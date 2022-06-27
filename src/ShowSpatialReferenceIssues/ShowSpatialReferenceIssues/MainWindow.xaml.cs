using Esri.ArcGISRuntime.Mapping;
using System;
using System.Windows;

namespace ShowSpatialReferenceIssues
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
                    if (geometry is not null)
                    {
                        MapView.SetViewpoint(new Viewpoint(geometry));
                    }
                });
        }
    }
}
