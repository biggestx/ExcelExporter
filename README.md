# Excel Exporter


Description
----
Game developers use excel to manage lots game datas.
Generally they use parsing xlsx to json and then into container, ExcelExporter supports this automatically.
ExcelExporter parses xlsx by using Zeroformatter, it is faster than json.
(https://github.com/neuecc/ZeroFormatter)

Languages
----
    - C#

Libraries
----
    - ZeroFormatter

        
Installation
----
    - ExcelExporter.unitipackage is released now. just import to unity project.

How to use
----

1. xlsx file form
    - Write name of variable at first row
    - Wirte type of variable at second row
        - ExcelExporter supports int, float, string types for variables
    - 1-A, 1-B is key for container, Currently ExcelExporter only supports int type key

![1](https://user-images.githubusercontent.com/69115321/149321351-8c6cc51b-6e06-43c7-8b61-d6a6adab42dd.png)


- This xlsx file will be converted like below C# code.


![2](https://user-images.githubusercontent.com/69115321/149321363-ca0231d2-f128-4e36-b958-cdee7797c276.png)

2. Open ExcelExporter window in Unity and then wirte input/output paths

![3](https://user-images.githubusercontent.com/69115321/149321367-d9c9490a-21aa-47a0-b47d-4bb53563955e.png)
![4](https://user-images.githubusercontent.com/69115321/149321371-3cd1474e-66bb-45a9-be77-959d8703863e.png)



