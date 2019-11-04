using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class RoomController
{    
    private Dictionary<string, RoomInfo> rooms;

    // Use this function to filter all the rooms to give gameObject rooms.
    public void UpdateRooms(Dictionary<string, RoomInfo> new_rooms){
        
        foreach(RoomInfo room in new_rooms.Values)
        {
            Debug.Log(room.Name);
            if(!room.IsOpen || !room.IsVisible || room.RemovedFromList) {
                if(rooms.ContainsKey(room.Name))
                {
                    rooms.Remove(room.Name);
                }
            } else {
                if(rooms.ContainsKey(room.Name)) {
                    rooms[room.Name] = room;
                } else {
                    rooms.Add(room.Name, room);
                }
            }
        }

    }

    public Dictionary<string, RoomInfo> GetRooms(){
        return this.rooms;
    }


}
