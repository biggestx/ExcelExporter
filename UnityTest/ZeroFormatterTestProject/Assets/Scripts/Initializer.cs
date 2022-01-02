using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using global::ZeroFormatter.Formatters;

using AsyncOperations = UnityEngine.ResourceManagement.AsyncOperations;

public class Initializer : MonoBehaviour
{

    private void LoadByResourcesLoad()
    {

        // change extension .byte -> text to load
        var testBytes = Resources.Load("Test") as TextAsset;
        var bytes = testBytes.bytes;

        var testTable = new Table.TestTable();
        testTable.DeserializeFromBytes(bytes);

        foreach (var d in testTable.Container)
        {
            Debug.Log($"{d.Key}, {d.Value.Value}, {d.Value.Description}");
        }
    }

    byte[] ObjectToByteArray(object obj)
    {
        if (obj == null)
            return null;
        
        var bf = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
        
        using (var ms = new System.IO.MemoryStream())
        {
            bf.Serialize(ms, obj);
            return ms.ToArray();
        }
    }

    private async void LoadByAddressable()
    {
        var handle = Addressables.LoadAssetAsync<TextAsset>("Test");
        await handle.Task;

        if (handle.Status == AsyncOperations.AsyncOperationStatus.Succeeded)
        {
            var testTable = new Table.TestTable();
            testTable.DeserializeFromBytes(handle.Result.bytes);
            foreach (var d in testTable.Container)
            {
                Debug.LogError($"{d.Key}, {d.Value.Description},");
            }

            Debug.LogError("succeeded");
        }
        else
        {
            Debug.LogError("failed"); 
        }
    }

    //private void LoadByAddressable()
    //{
    //    Addressables.LoadAssetAsync<Object>("Test").Completed
    //        += (handle) =>
    //    {
    //        if (handle.Status == AsyncOperations.AsyncOperationStatus.Succeeded)
    //        {
    //            var bytes = ObjectToByteArray(handle.Result);
    //            Debug.LogError("succeeded");
    //        }
    //        else
    //        {
    //            Debug.LogError("failed");
    //        }



    //    };
    //}

    void Start()
    {
        ZeroFormatter.ZeroFormatterInitializer.Register();

        LoadByAddressable();

    }


}
