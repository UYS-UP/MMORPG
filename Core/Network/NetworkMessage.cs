using System.Collections;
using System.Collections.Generic;
using MessagePack;
using UnityEngine;

[MessagePackObject]
public struct LoginData
{
    [Key(0)] public string Username { get; set; }
    [Key(1)] public string Password { get; set; }
}
