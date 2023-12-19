using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : Entity
{
    public GameObject head;
    private Rigidbody rb;
    private float shiftpower = 1;
    private float speed=2;
    private Vector2 mousedelta;
    private float rot, yrot;
    public int money=500;
    public List<Item> inventory;
    public GameObject TradeUI;
    public GameObject ItemButtonPrefab;

    private bool tradeMode;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        inventory = new List<Item>();
    }

    // Update is called once per frame
    void Update()
    {
        EntityUpdate();
        if (!tradeMode)
        {
            mousedelta.x = Input.GetAxis("Mouse X");
            mousedelta.y = Input.GetAxis("Mouse Y");
            transform.Rotate(0, mousedelta.x * Time.deltaTime * 2 * (100f), 0);
            rot += mousedelta.x * Time.deltaTime * 2;
            yrot -= mousedelta.y * Time.deltaTime * 2 * (100f);
            yrot = Mathf.Clamp(yrot, -90, 90);
            head.transform.localRotation = Quaternion.Euler(yrot, 0, 0);

            if (Input.GetKey(KeyCode.A)) { transform.Translate(new Vector3(-speed, 0, 0) * shiftpower * Time.deltaTime); }
            if (Input.GetKey(KeyCode.D)) { transform.Translate(new Vector3(speed, 0, 0) * shiftpower * Time.deltaTime); }
            if (Input.GetKey(KeyCode.W)) { transform.Translate(new Vector3(0, 0, speed) * shiftpower * Time.deltaTime); }
            if (Input.GetKey(KeyCode.S)) { transform.Translate(new Vector3(0, 0, -speed) * shiftpower * Time.deltaTime); }

            if (Input.GetKeyDown(KeyCode.E))
            {
                RaycastHit hit;
                if (Physics.Raycast(head.transform.position, head.transform.forward, out hit, 2f) && hit.transform.CompareTag("NPC"))
                {
                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible = true;
                    tradeMode = true;
                    NPC npc = hit.transform.GetComponent<NPC>();
                    UpdateTradeUI(npc);
                }
            }

            shiftpower = 1;
            if (Input.GetKey(KeyCode.LeftShift))
            {
                shiftpower = 3;
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                rb.AddForce(new Vector3(0, 6, 0), ForceMode.Impulse);
            }
        }
        else 
        {
            if (Input.GetKeyDown(KeyCode.Escape)) 
            {
                TradeUI.SetActive(false);
                tradeMode = false;
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }
    }
    private void UpdateTradeUI(NPC npc) 
    {
        TradeUI.SetActive(true);
        TradeUI.transform.GetChild(1).GetComponent<Text>().text = npc.motherLand.name;
        ClearRoot(TradeUI.transform.GetChild(2).GetChild(0).GetChild(0));
        ClearRoot(TradeUI.transform.GetChild(3).GetChild(0).GetChild(0));
        for (int i = 0; i < npc.motherLand.items.Count; ++i)
        {
            int j = i;
            npc.motherLand.items[i].realPrice = npc.motherLand.items[i].basePrice;
            for (int ii = 0; ii < npc.motherLand.items.Count; ++ii)if(i!=ii)
            {
                if (npc.motherLand.items[i].name == npc.motherLand.items[ii].name)
                {
                        npc.motherLand.items[i].realPrice = npc.motherLand.items[i].realPrice*2/3;
                }
            }
            GameObject ob = Instantiate(ItemButtonPrefab, TradeUI.transform.GetChild(2).GetChild(0).GetChild(0));
            ob.GetComponent<Button>().onClick.AddListener(() => BtnBuy(npc.motherLand.items[j], false,npc));
            ob.transform.GetChild(0).GetComponent<Image>().sprite = Generator.only.icons[npc.motherLand.items[j].iconid];
            ob.transform.GetChild(1).GetComponent<Text>().text = npc.motherLand.items[j].name;
            string st = npc.motherLand.items[j].realPrice + (npc.motherLand.items[j].basePrice == npc.motherLand.items[j].realPrice ? "=" : (npc.motherLand.items[j].realPrice < npc.motherLand.items[j].basePrice ? "+++" : "---"));
            ob.transform.GetChild(2).GetComponent<Text>().text = st;
        }
        for (int i = 0; i < inventory.Count; ++i)
        {
            inventory[i].realPrice = inventory[i].basePrice;
            int j = i;
            for (int ii = 0; ii < npc.motherLand.wishes.Count; ++ii)
            {
                if (inventory[i].name == npc.motherLand.wishes[ii].name)
                {
                    inventory[i].realPrice *= 2;
                }
            }
            for (int ii = 0; ii < npc.motherLand.items.Count; ++ii)
            {
                if (inventory[i].name == npc.motherLand.items[ii].name)
                {
                    inventory[i].realPrice /= 2;
                }
            }
            GameObject ob = Instantiate(ItemButtonPrefab, TradeUI.transform.GetChild(3).GetChild(0).GetChild(0));
            ob.GetComponent<Button>().onClick.AddListener(() => BtnBuy(inventory[j], true, npc));
            ob.transform.GetChild(0).GetComponent<Image>().sprite = Generator.only.icons[inventory[j].iconid];
            ob.transform.GetChild(1).GetComponent<Text>().text = inventory[j].name;
            string st = inventory[j].realPrice + (inventory[j].basePrice == inventory[j].realPrice ? "=" : (inventory[j].realPrice < inventory[j].basePrice ? "---" : "+++"));
            ob.transform.GetChild(2).GetComponent<Text>().text = st;
        }
        TradeUI.transform.GetChild(4).GetComponent<Text>().text = money + " золота";
    }
    private void BtnBuy(Item item, bool sell,NPC npc) 
    {
        if (sell)
        {
            Debug.Log("Продано! " + item.name);
            inventory.Remove(item);
            money += item.realPrice;
        }
        else
        {
            if (money >= item.realPrice && inventory.Count < 10)
            {
                Debug.Log("Куплено! " + item.name);
                inventory.Add(item.Clone());
                money -= item.realPrice;
            }
        }
        UpdateTradeUI(npc);
    }
    private void ClearRoot(Transform root) 
    {
        for (int i = 0; i < root.childCount; ++i)
        {
            Destroy(root.GetChild(i).gameObject);
        }
    }
}
