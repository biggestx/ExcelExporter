using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using global::ZeroFormatter.Formatters;

public class Initializer : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
        ZeroFormatter.ZeroFormatterInitializer.Register();

        var table = new Table.TestTable();
        table.Deserialize();
        foreach (var data in table.Container)
        {
            Debug.LogError($"{data.Value.ID},{data.Value.Desc},{data.Value.Power}");
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
