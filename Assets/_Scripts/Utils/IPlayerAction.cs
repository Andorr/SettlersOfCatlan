using State;


public interface IPlayerAction
{
    WorkerController CreateWorker(Location location);
    void MoveWorker(WorkerController worker, Location location);
    void BuildPath(PathController controller);
    void BuildHouse(LocationController controller);
    void BuildCity(LocationController controller);
}