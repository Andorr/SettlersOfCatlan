using System;
using State;

public class ActionInfo
{
    public Player player;
    public ActionType actionType;
    public DateTimeOffset timestamp;

    public static ActionInfo New(ActionType type, Player player) {
        return new ActionInfo {
            actionType = type,
            player = player,
            timestamp = DateTimeOffset.Now,
        };
    }

    public String ToString() {
        string output = $"[{timestamp.TimeOfDay}] - {player.name} ";
        
        switch(actionType) {
            case ActionType.CreateWorker: {
                output += "created an worker.";
                break;
            }
            case ActionType.MoveWorker: {
                output += "moved a worker.";
                break;
            }
            case ActionType.BuildPath: {
                output += "built a path.";
                break;
            }
            case ActionType.BuildHouse: {
                output += "built a house.";
                break;
            }
            case ActionType.BuildCity: {
                output += "upgraded a house to a city.";
                break;
            }
            case ActionType.EndTurn: {
                output += "ended his/her turn.";
                break;
            }
        }
        return output;
    }
}

public enum ActionType {
    CreateWorker,
    MoveWorker,
    BuildPath,
    BuildHouse,
    BuildCity,
    EndTurn,
}
