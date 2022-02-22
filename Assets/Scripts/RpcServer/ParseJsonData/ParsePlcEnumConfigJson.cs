using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;

public class ValueItem
{
    /// <summary>
    /// S1N主汽开关R
    /// </summary>
    public string ValueName { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public string ModbusAddress { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public string PlcValueAddress { get; set; }
}

public class ParsePlcEnumConfigJson
{
    /// <summary>
    /// 
    /// </summary>
    public List<ValueItem> FirePower { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public List<ValueItem> WindPower { get; set; }
    public List<ValueItem> SolarPower { get; set; }
    public List<ValueItem> IntelligentManufacturing { get; set; }
    public List<ValueItem> WarehouseLogistics { get; set; }
    public List<ValueItem> AutomobileMaking { get; set; }
    public List<ValueItem> CoalToMethanol { get; set; }
    public List<ValueItem> AviationOil { get; set; }
    public List<ValueItem> WaterPower { get; set; }

    public void DebugSelf()
    {
        DebugList(FirePower,"FirePower");
        DebugList(WindPower,"WindPower");
        DebugList(SolarPower,"SolarPower");
        DebugList(IntelligentManufacturing,"IntelligentManufacturing");
        DebugList(WarehouseLogistics,"WarehouseLogistics");
        DebugList(AutomobileMaking,"AutomobileMaking");
        DebugList(CoalToMethanol,"CoalToMethanol");
        DebugList(AviationOil,"AviationOil");
        DebugList(WaterPower,"WaterPower");
    }

    void DebugList(List<ValueItem> list , string name)
    {
        Debug.Log("..................................................................");
        Debug.Log("scene Name : " + name+ " count : " + list.Count);
        for (int i = 0; i < list.Count; i++)
        {
            Debug.Log(" index : " + i + " ValueName : " + list[i].ValueName + " ModbusAddress : " + list[i].ModbusAddress + " PlcValueAddress : " + list[i].PlcValueAddress);
        }
    }
    
    public static string GetMemberName<T>(Expression<Func<T>> memberExpression)
    {
        MemberExpression expressionBody = (MemberExpression)memberExpression.Body;
        return expressionBody.Member.Name;
    }
}