using System.Linq;
using State;
using UnityEngine;

public partial class MapController
{
    public Path GetPathById(int id) {
        return map.paths[id];
    }

    public bool GetPathController(Path location, out PathController pc)
    {
        var hasController = paths.TryGetValue(location.id, out GameObject obj);
        if(!hasController) {
            pc = null;
            return false;
        }

        pc = obj.GetComponent<PathController>();
        return pc != null;
    }

    public PathController[] GetAdjecentPaths(Location location, bool mustBeAvailable = false)
    {
        // Get all paths connected to the location
        var adjecentPaths = map.paths.Values.Where(p => p.between.Item1.id == location.id || p.between.Item2.id == location.id);
        
        if(mustBeAvailable)
        {
            adjecentPaths = adjecentPaths.Where(p => p.occupiedBy == null);
        }

        // Get all path controllers
        var controllers = adjecentPaths.Select(p => paths[p.id].GetComponent<PathController>());
        return controllers.ToArray();
    }

}
