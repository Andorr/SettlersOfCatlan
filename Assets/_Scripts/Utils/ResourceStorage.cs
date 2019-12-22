using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceStorage : MonoBehaviour
{
    public int wood;
    public int stone;
    public int clay;
    public int wheat;
    public int wool;

    public bool IsEmpty() {
        return wood == 0 && stone == 0 && clay == 0 && wheat == 0 && wool == 0;
    }

    public override string ToString() {
        return $"{wood} wood, {stone} stone, {clay} clay, {wheat} wheat and {wool} wool";
    }
}
