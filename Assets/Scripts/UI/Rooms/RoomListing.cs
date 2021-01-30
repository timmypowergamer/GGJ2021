using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomListing : MonoBehaviour
{
    [SerializeField] Text _text = null;

    public void SetRoomInfo(RoomInfo roomInfo)
    {
        Debug.Log($"Room \"{roomInfo.Name}\" listed.");
        if (_text != null)
        {
            _text.text = roomInfo.Name;
        }
    }
}
