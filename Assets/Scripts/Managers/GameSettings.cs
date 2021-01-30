using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Manager/GameSettings")]
public class GameSettings : ScriptableObject
{
    [SerializeField] string _gameVersion = "0.0.0";
    [SerializeField] string _nickName = "PsychoKiller";

    public string GameVersion { get { return _gameVersion; } }
    public string NickName { get { return $"{_nickName}{Random.Range(0, 9999)}"; } }
}
