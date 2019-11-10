using System.Linq;
using State;

public partial class MapController
{
    public PathController[] GetAdjecentPaths(Location location)
    {
        // Get all paths connected to the location
        var adjecentPaths = map.paths.Values.Where(p => p.between.Item1.id == location.id || p.between.Item2.id == location.id).ToArray();
        
        // Get all path controllers
        var controllers = adjecentPaths.Select(p => paths[p.id].GetComponent<PathController>());
        return controllers.ToArray();
    }

}
