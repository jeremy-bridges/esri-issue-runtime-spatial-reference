using Esri.ArcGISRuntime.Data;
using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.Mapping;
using Microsoft.Win32;
using ShowSpatialReferenceIssues.Common;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ShowSpatialReferenceIssues.NetFramework
{
    public class MainWindowViewModel : PropertyChangedBase
    {
        private readonly Action<string> _log;
        private readonly Action<Geometry> _zoom;

        public MainWindowViewModel(Action<string> log, Action<Geometry> zoom)
        {
            _log = log;
            _zoom = zoom;

            AddReplicasCommand = new SimpleCommand(AddReplicas);
            GoToColoradoCommand = new SimpleCommand(GoToColorado);
            GoToNewYorkIslandCommand = new SimpleCommand(GoToNewYorkIsland);

            Map = new Map(ColoradoStatePlanarSpatialReference())
            {
                MinScale = 100000000,
                MaxScale = 0
            };

            Map.Basemap.BaseLayers.Add(new ArcGISMapImageLayer(new Uri(
                "https://services.arcgisonline.com/arcgis/rest/services/World_Street_Map/MapServer"))
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Main Basemap"
            });

            Task.Delay(TimeSpan.FromSeconds(2)).ContinueWith(t =>
            {
                AddReplicas(new[] {".\\miner_fiber.geodatabase"});
            });
        }

        public Map Map { get; }
        public ICommand AddReplicasCommand { get; }
        public ICommand GoToColoradoCommand { get; }
        public ICommand GoToNewYorkIslandCommand { get; }

        public void AddReplicas()
        {
            var dialog = new OpenFileDialog
            {
                Multiselect = true,
                DefaultExt = ".geodatabase"
            };

            if (dialog.ShowDialog() == true)
            {
                AddReplicas(dialog.FileNames);
            }
        }

        private void AddReplicas(string[] fileNames)
        {
            foreach (var fileName in fileNames)
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
                        gdb.GeodatabaseFeatureTables.Select(table => new FeatureLayer(table)));
                    Map.OperationalLayers.AddRange(
                        gdb.GeodatabaseAnnotationTables.Select(table => new AnnotationLayer(table)));

                    _zoom(Colorado());
                });
            }
        }

        private void GoToColorado()
        {
            _zoom(Colorado());
        }

        private void GoToNewYorkIsland()
        {
            _zoom(NewYorkLongIsland());
        }

        private static SpatialReference ColoradoStatePlanarSpatialReference()
        {
            return SpatialReference.Create(26753);
        }

        private static Geometry Colorado()
        {
            return Geometry.FromJson(
                "{\"xmin\":-11682081.737725906,\"ymin\":4907420.5648448579,\"xmax\":-11637351.215794165,\"ymax\":4943740.2194048185,\"spatialReference\":{\"wkid\":102100,\"latestWkid\":3857}}");
        }

        private static SpatialReference NewYorkIslandSpatialReference()
        {
            return SpatialReference.Create(2263);
        }

        private static Geometry NewYorkLongIsland()
        {
            return Geometry.FromJson(
                @"{""xmin"":-8252911.5025626058,""ymin"":4965501.9753929507,""xmax"":-8218343.0578241879,""ymax"":4987128.2633492388,""spatialReference"":{ ""wkid"":102100,""latestWkid"":3857}}");
        }
    }
}
