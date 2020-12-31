using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ChooseRoad : MonoBehaviour
{
    public MapGen mapGen;
    private List<string> streetNames = new List<string>();
    public Dropdown dropdown;
    // Start is called before the first frame update
    void Start()
    {
        dropdown.onValueChanged.AddListener(delegate {
            OnMyValueChange(dropdown);
        });
        
        
    }

    // Update is called once per frame
    void Update()
    {
        int TempInt = dropdown.value;
        if (TempInt <= 0) return;
        mapGen.GetStreets()[TempInt].Blink(true);
    }

    public void UpdateSources()
    {
        if (mapGen == null) return;
        List<Street> list = mapGen.GetStreets();
        if (list == null) return;
        streetNames.Clear();
        foreach (Street street in list)
        {
            streetNames.Add(street.Name);
        }
        dropdown.ClearOptions();
        dropdown.AddOptions(streetNames);
    }

    void OnMyValueChange(Dropdown change)
    {
        //dropdown.RefreshShownValue();
        int TempInt = dropdown.value;
        if (TempInt <= 0) return;
        mapGen.GetStreets()[TempInt].StartBlinking();
    }



}
