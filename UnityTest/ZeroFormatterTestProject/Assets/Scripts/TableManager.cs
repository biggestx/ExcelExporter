using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

using UnityEngine;
using UnityEngine.AddressableAssets;
using AsyncOperations = UnityEngine.ResourceManagement.AsyncOperations;

using Table;

public class TableManager : MonoBehaviourSingleton<TableManager>
{
   
    //private static TableManager Inst = new TableManager();
    //public static TableManager Instance => Inst;
    //private TableManager() { }


    public CharacterTable Character;
    public TestTable Test;

    private class Pair
    {
        public ITableDeserialization Table;
        public string Address;
        public Pair(ITableDeserialization table, string address)
        {
            Table = table;
            Address = address;
        }
    }

    public void Initialize()
    {
        StartCoroutine(RoutineInitialize());
    }

    public IEnumerator RoutineInitialize()
    {
        int count = 0;
        int targetCount = 2; // todo

        System.Action doCount = () => ++count;

        LoadByAddressable(Character, "Character", doCount);
        LoadByAddressable(Test, "Test", doCount);

        var until = new WaitUntil(() => count == targetCount);
        yield return until;

        Debug.Log("[TabledManager] : initialized");
    }


    private async void LoadByAddressable<T>
        (T table,string address, System.Action completed) where T : ITableDeserialization, new()
    {
        table = new T();

        var handle = Addressables.LoadAssetAsync<TextAsset>(address);
        await handle.Task;

        if (handle.Status == AsyncOperations.AsyncOperationStatus.Succeeded)
        {
            table.DeserializeFromBytes(handle.Result.bytes);
            Debug.Log("succeded" + address);
        }
        else
        {
            Debug.Log("failed" + address);
        }

        completed?.Invoke();
    }


}