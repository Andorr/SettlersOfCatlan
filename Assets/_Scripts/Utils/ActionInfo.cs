using System;
using State;
using Photon.Pun;

public class ActionInfo
{
    public Player player;
    public ActionType actionType;
    public DateTimeOffset timestamp;
    public object data;

    public static ActionInfo New(ActionType type, Player player) {
        return new ActionInfo {
            actionType = type,
            player = player,
            timestamp = DateTimeOffset.Now,
        };
    }

    public override String ToString() {
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
            case ActionType.GainedResources: {
                ResourceStorage store = (ResourceStorage)data;
                output += $"gained {store.ToString()}";
                break;
            }
            case ActionType.ExchangedResources: {
                ResourceType[] types = (ResourceType[])data;
                output += $"exchanged 3 {ResourceUtil.TypeToString(types[0])} for 1 {ResourceUtil.TypeToString(types[1])}";
                break;
            }
            case ActionType.ThiefStoleResource: {
                var temp = (object[]) data;
                ResourceType type = (ResourceType) temp[0];
                String stealer = (string) temp[1];
                String stealee = (string) temp[2];
                if (stealer == PhotonNetwork.LocalPlayer.NickName) stealer = "Your";
                else if (stealee == PhotonNetwork.LocalPlayer.NickName) { stealee = "you"; stealer += "s"; }
                else stealer += "s";

                output += $"{stealer} thief stole 1 {ResourceUtil.TypeToString(type)} from {stealee}";
                break;
            }
            case ActionType.CannotStealResouce: {
                String playerName = (string) data;
                output += $"{playerName} has no resources to steal";
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
    GainedResources,
    ExchangedResources,
    Traded,
    BuyCard,
    UseCard,
    ThiefStoleResource,
    CannotStealResouce,
}
