using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using System;

/* 
   This is simple realization of cooldown manager. For comfortable and stable use we will store all cooldowns in one place and 
   indicate about changing in cooldown status by returning its value or sending the subscribers event. We will store cooldown values in Dictionary 
   and use int-id keys (e.g. HashCode) to get them. Changing values action is provided in Update method. We will go through all cooldowns each frame and
   check whether their count comes to end or not.
 */

/// <summary>
/// Common base class for cooldown manager.
/// </summary>
public class CooldownManager : MonoBehaviour {

    private static CooldownManager _instance;

    /// <summary>
    /// On cooldown change event.
    /// </summary>
    public static event Action<int, float> CooldownChangeEvent;

    private Dictionary<int, float> _cooldownArray = new Dictionary<int, float>();

    //Used to get an access to change cooldown values in *foreach* cycle.
    private List<int> _cooldownKeys = new List<int>();

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
    }

    private void Update()
    {
        if ( _instance._cooldownKeys.Count == 0 ) return;

        foreach (int id in _instance._cooldownKeys)
        {
            if (_instance._cooldownArray[id] > 0f)
            {
                _instance._cooldownArray[id] -= Time.deltaTime;
                OnCooldownChange(id, _instance._cooldownArray[id]);
            }
        }
    }

    //Raising the event on cooldown status changing.
    private void OnCooldownChange(int id, float value)
    {
        if (CooldownChangeEvent != null)
        {
            CooldownChangeEvent(id, value);
        }
    }

    /// <summary>
    /// Checks whether cooldown counting is on or not.
    /// </summary>
    /// <param name="id">Cooldown ID</param>
    public static bool IsOnCooldown(int id)
    {
        if (!_instance._cooldownArray.ContainsKey(id))
        {
            return false;
        }

        if ((_instance._cooldownArray[id] > 0.01f))
        {
            _instance.IndicateCooldown();
            return true;
        }
        else
            return false;
    }

    /// <summary>
    /// Setting new cooldown. If it is exist, then it will return its status.
    /// </summary>
    /// <param name="time">Start time of cooldown.</param>
    /// <param name="id">Cooldown ID</param>
    public static bool SetCooldown(float time, int id)
    {
        if (!_instance._cooldownArray.ContainsKey(id))
        {
            _instance._cooldownArray.Add(id, time);
            _instance._cooldownKeys = _instance._cooldownArray.Keys.ToList();
            return false;
        }
        else
        {
            if (_instance._cooldownArray[id] <= 0.01f)
            {
                _instance._cooldownArray[id] = time;
                return false;
            }
            else
            {
                _instance.IndicateCooldown();
                return true;
            }
                
        }
    }

    
    private void IndicateCooldown()
    {
        //Here i am indicating the cooldown status. It can be a message, UI text or sound.
    }

    private void OnDestroy()
    {
        //Unsubscribing by destroy.
        CooldownChangeEvent = null;
    }
}
