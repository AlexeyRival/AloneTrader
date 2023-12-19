using UnityEngine;
public class Item
{
    public string name;
    public int basePrice;
    public int realPrice;
    public int iconid;
    public Vector3 bornPoint;
    public Item(string name, int basePrice, int realPrice, Vector3 bornPoint, int iconid)
    {
        this.name = name;
        this.basePrice = basePrice;
        this.realPrice = realPrice;
        this.bornPoint = bornPoint;
        this.iconid = iconid;
    }
    public Item Clone() 
    {
        return new Item(name, basePrice, realPrice, bornPoint, iconid);
    }
}
public class Bundle 
{
    public int count;
    public Item item;

    public Bundle(int count, Item item)
    {
        this.count = count;
        this.item = item;
    }
}