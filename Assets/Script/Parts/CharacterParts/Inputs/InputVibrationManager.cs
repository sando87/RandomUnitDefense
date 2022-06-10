using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum VibrationPattern
{
    FireWeapon,
    ObjectBroken,
    Explosion,
    EarthQuake,
    Pulse,
}

public class InputVibrationManager : SingletonMono<InputVibrationManager>
{
}
