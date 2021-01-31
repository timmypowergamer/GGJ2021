using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class RoomListingsMenu : MonoBehaviourPunCallbacks
{
    [SerializeField] Transform _content = null;
    [SerializeField] RoomListing _roomListingPrefab = null;

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        base.OnRoomListUpdate(roomList);

        foreach (var info in roomList)
        {
            var listing = Instantiate(_roomListingPrefab, _content);
            if (listing != null)
            {
                listing.SetRoomInfo(info);
            }
        }
    }
}
