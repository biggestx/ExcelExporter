using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using global::ZeroFormatter.Formatters;

using AsyncOperations = UnityEngine.ResourceManagement.AsyncOperations;

public class Initializer : MonoBehaviour
{

    IEnumerator Start()
    {
        //ZeroFormatter.Zerofor.Register();

        yield return TableManager.Instance.RoutineInitialize();


        Debug.Log("[Initializer] : Initialized");

        var table = TableManager.Instance.Character.Container;
        foreach (var d in table)
        {
            Debug.LogError($"{d.Key}, {d.Value.Description}");
        }

    }


}
