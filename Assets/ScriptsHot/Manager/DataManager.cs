using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager : Manager<DataManager>
{

    public bool iniOver = false;

    protected override void Ini()
    {
        StartCoroutine(IE_Ini());
    }
    IEnumerator IE_Ini()
    {
        yield return new WaitUntil(() => ExcelManager.instance.preLoadOver == true);

        iniOver = true;
    }
  

}

