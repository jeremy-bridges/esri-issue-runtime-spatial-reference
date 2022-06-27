using Esri.ArcGISRuntime.Data;
using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.Mapping;
using Microsoft.Win32;
using System;
using System.Linq;
using System.Windows.Input;

namespace ShowSpatialReferenceIssues
{
    public class MainWindowViewModel : PropertyChangedBase
    {
        private readonly Action<string> _log;
        private readonly Action<Geometry?> _zoom;

        public MainWindowViewModel(Action<string> log, Action<Geometry?> zoom)
        {
            _log = log;
            _zoom = zoom;
            AddReplicasCommand = new SimpleCommand(AddReplicas);

            // rotated spatial reference, specifically designed for New York long island
            Map = new Map(SpatialReference.Create(2263)); 
        }

        public Map Map { get; }
        public ICommand AddReplicasCommand { get; }

        public void AddReplicas()
        {
            var dialog = new OpenFileDialog
            {
                Multiselect = true,
                DefaultExt = ".geodatabase"
            };

            if (dialog.ShowDialog() == true)
            {
                foreach (var fileName in dialog.FileNames)
                {
                    Geodatabase.OpenAsync(fileName).ContinueWith(t =>
                    {
                        if (t.IsCanceled)
                        {
                            _log($"Opening replica at '{fileName}' cancelled");
                            return;
                        }
                        if (t.IsFaulted)
                        {
                            _log($"Opening replica at '{fileName}' failed because {t.Exception?.Message}");
                            return;
                        }

                        var gdb = t.Result;

                        Map.OperationalLayers.AddRange(
                            gdb.GeodatabaseFeatureTables.Select(t => new FeatureLayer(t)));
                        Map.OperationalLayers.AddRange(
                            gdb.GeodatabaseAnnotationTables.Select(t => new AnnotationLayer(t)));

                        _zoom(NewYorkLongIsland());
                    });
                }
            }
        }

        private static Geometry? NewYorkLongIsland()
        {
            return Geometry.FromJson(
                @"{""xmin"":-8252911.5025626058,""ymin"":4965501.9753929507,""xmax"":-8218343.0578241879,""ymax"":4987128.2633492388,""spatialReference"":{ ""wkid"":102100,""latestWkid"":3857}}");
        }
    }
}
