using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class Village : MonoBehaviour 
{
    public List<Item> items;
    public List<Item> wishes;
    public int moneyCap;
    public void Start()
    {
        items = new List<Item>();
        wishes = new List<Item>();
    }
}