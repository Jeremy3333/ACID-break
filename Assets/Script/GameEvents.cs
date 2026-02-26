using System;
using UnityEngine;

public static class GameEvents
{
    public static event Action<HitEvent> OnHit;
    public static event Action<DeathEvent> OnDeath;
    public static System.Action<ChargeEvent> OnCharge;

    public static void RaiseHit(HitEvent e) => OnHit?.Invoke(e);

    public static void RaiseDeath(DeathEvent e) => OnDeath?.Invoke(e);
    public static void RaiseCharge(ChargeEvent e) => OnCharge?.Invoke(e);
}

public struct HitEvent
{
    public Vector2 position;
    public Vector2 direction;
    public GameObject victim;
}

public struct DeathEvent
{
    public Vector2 position;
    public GameObject victim;
}

public enum ChargeState
{
    Started,
    Ended
}

public struct ChargeEvent
{
    public Vector2 position;
    public Vector2 direction;
    public GameObject victim;
    public ChargeState state;
}
