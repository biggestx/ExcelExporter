# Excel Exporter


Description
----
Game developers use excel to manage lots game datas.
Generally they use parsing xlsx to json and then into container, ExcelExporter supports this automatically.
ExcelExporter parses xlsx by using Zeroformatter, it is fater than json.
    - https://github.com/neuecc/ZeroFormatter

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
    
    - This xlsx file will be converted like below C# code.

2. Open ExcelExporter window in Unity and then wirte input/output paths
    
