using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HexTileReviver : MonoBehaviour
{
    [SerializeField] private HexBaseManager _hexBaseManager;
    [SerializeField] private int numberTesting;

    public void ReviveBySlot(int numberToRevive)
    {
        int slotToRevive = 0;

        var hexBases = _hexBaseManager.HexBases;
        var listHexBaseIsValid = hexBases.Where(hexBase => hexBase.HexTileGroup != null).ToList();

        slotToRevive = listHexBaseIsValid.Count <= numberToRevive ? listHexBaseIsValid.Count : numberToRevive;
        
        var randomItems = listHexBaseIsValid.OrderBy(x => Guid.NewGuid()).Take(slotToRevive).ToList();
        
        foreach (var hexBase in randomItems)
        {
            var hexTileGroup = hexBase.HexTileGroup;
            hexBase.RemoveHexTile(hexTileGroup);
        }
    }
}