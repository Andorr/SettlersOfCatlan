using State;


public interface IPlayerAction
{
    WorkerController CreateWorker(Location location);
    void MoveWorker(Worker worker, Location location);
    void BuildPath(Path path);
    void BuildHouse(Location location);
    void BuildCity(Location location);
    void ExchangeResources(ResourceType from, ResourceType to);
    void EndTurn();
}