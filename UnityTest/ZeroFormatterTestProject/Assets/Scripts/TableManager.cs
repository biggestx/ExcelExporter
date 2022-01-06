using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

using UnityEngine;
using UnityEngine.AddressableAssets;
using AsyncOperations = UnityEngine.ResourceManagement.AsyncOperations;

using global::ZeroFormatter.Formatters;


using Table;

public class TableManager : MonoBehaviourSingleton<TableManager>
{
    public CharacterTable Character;
    public TestTable Test;

    public void Initialize()
    {
        StartCoroutine(RoutineInitialize());
    }

    public IEnumerator RoutineInitialize()
    {
        ZeroFormatter.ZeroFormatterInitializer.Register();

        int count = 0;
        int targetCount = 2; // todo

        System.Action doCount = () => ++count;

        // TODO : needed simplfy
        LoadByAddressable<CharacterTable>("Table/Character", (p) => { Character = p; doCount(); });
        LoadByAddressable<TestTable>("Table/Test", (p) => { Test = p; doCount(); });

        var until = new WaitUntil(() => count == targetCount);
        yield return until;

        Debug.Log("[TabledManager] : initialized");
    }


    private async void LoadByAddressable<T>
        (string address, System.Action<T> completed) where T : ITableDeserialization, new()
    {
        var result = new T();

        var handle = Addressables.LoadAssetAsync<TextAsset>(address);
        await handle.Task;

        if (handle.Status == AsyncOperations.AsyncOperationStatus.Succeeded)
        {
            result.DeserializeFromBytes(handle.Result.bytes);
            Debug.Log("succeded" + address);
        }
        else
        {
            Debug.Log("failed" + address);
        }

        completed?.Invoke(result);
    }


}