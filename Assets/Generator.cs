using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Generator : MonoBehaviour
{
    public GameObject[] roads;
    public GameObject[] houses;
    public Sprite[] icons;
    public GameObject NPC;
    public static Generator only;
    public Item[] items =
    {
        new Item("Пиво",15,15,new Vector3(),0),
        new Item("Лягушка",5,5,new Vector3(),1),
        new Item("Металл",10,10,new Vector3(),2),
        new Item("Овощи",5,5,new Vector3(),3),
        new Item("Бумага",10,10,new Vector3(),4),
        new Item("Сплав",25,25,new Vector3(),5),
        new Item("Драгоценность",35,35,new Vector3(),6),
        new Item("Ювелирка",50,50,new Vector3(),7),
        new Item("Рыба",5,5,new Vector3(),8),
        new Item("Птица",5,5,new Vector3(),9),
        new Item("Снаряжение",25,25,new Vector3(),10),
    };
    private static readonly string[] firstVils = { "Канды", "Кузь", "Бары", "Сырь", "Камыш" };
    private static readonly string[] secondVils = { "нки", "минки", "бино", "ёво", "иново" };
    IEnumerator Start()
    {
        only = this;
        Vector3 villageCenter = new Vector3(0, 0, 0);
        int range = 3;
        for (int i = 0; i < 10; ++i)
        {
            GameObject village = new GameObject(firstVils[Random.Range(0, firstVils.Length)] + secondVils[Random.Range(0, secondVils.Length)]);
            village.transform.position = villageCenter;
            Village vil = village.AddComponent<Village>();

            Instantiate(roads[0], villageCenter, Quaternion.identity, village.transform);
            int villagePrice = Random.Range(5, 15);
            List<Vector3> subcenters = new List<Vector3>();
            for (int j = 0; j < villagePrice; ++j)
            {
                subcenters.Add(villageCenter + new Vector3(Random.Range(-range, range + 1), 0, Random.Range(-range, range + 1)) * villagePrice);
            }
            for (int j = 0; j < subcenters.Count; ++j)
            {
                Instantiate(roads[0], subcenters[j], Quaternion.identity, village.transform);
                for (int jj = 0; jj < subcenters.Count; ++jj) if (jj != j)
                    {
                        if (subcenters[j].x == subcenters[jj].x | subcenters[j].z == subcenters[jj].z)
                        {
                            RoadTo(subcenters[j], subcenters[jj], village.transform);
                        }
                    }
            }
            yield return null;
            yield return null;
            int counter = 0;
            int jk = 0;
            while (counter < villagePrice * 5 && jk < villagePrice * 20)
            {
                jk++;
                Vector3 buf = villageCenter + new Vector3(Random.Range(-range, range + 1), 0, Random.Range(-range, range + 1)) * villagePrice;
                RaycastHit[] hits = Physics.BoxCastAll(buf, new Vector3(2.5f, 2.5f, 2.5f), Vector3.up);
                bool can = true;
                if (hits.Length != 0)
                {
                    for (int k = 0; k < hits.Length; ++k)
                    {
                        if (hits[k].transform.CompareTag("Road") || hits[k].transform.CompareTag("House"))
                        {
                            can = false;
                            break;
                        }
                    }
                }
                if (can)
                {
                    ++counter;
                    Instantiate(houses[Random.Range(0, houses.Length)], buf, Quaternion.Euler(0, Random.Range(0, 4) * 90, 0), village.transform);
                    Debug.DrawLine(buf, buf + Vector3.up * 10f, Color.green, 10f);
                    yield return null;
                }
            }
            vil.moneyCap = villagePrice * 11;
            for (int j = 0; j < villagePrice; ++j)
            {
                if (j % 2 == 0)
                {
                    vil.items.Add(items[Random.Range(0, items.Length)].Clone());
                }
                else
                {
                    vil.wishes.Add(items[Random.Range(0, items.Length)].Clone());
                }
            }

            Instantiate(NPC, villageCenter + new Vector3(0, 1f, 0), Quaternion.Euler(0, Random.Range(-180f, 180f), 0)).GetComponent<NPC>().motherLand = vil;

            villageCenter = new Vector3(Random.Range(-50, 50) * 10f, 0, Random.Range(-50, 50) * 10);
        }
        octrees = new List<Octree>();
        octrees.Add(new Octree(new Vector3(0, 0, 0), 100));
        for (int i = 0; i < 10; ++i)
        {
            int k = octrees.Count;
            for (int j = 0; j < k; ++j) if (octrees[j].size > 1 && !octrees[j].splitted)
                {
                    if (Physics.CheckBox(octrees[j].position * 10, new Vector3(5, 5, 5) * octrees[j].size))
                    {
                        octrees[j].Split();
                        octrees.AddRange(octrees[j].childs);
                    }
                }
            Debug.Log(octrees.Count);
            yield return new WaitForSeconds(.1f);
        }
        for (int i = octrees.Count-1; i >=0; --i) 
        {
            if (octrees[i].splitted) 
            {
                //octrees.RemoveAt(i);
            }
        }
        Debug.Log(octrees.Count);
    }
    private List<Octree> octrees;
    private void OnDrawGizmos()
    {
        if (octrees!=null&&octrees.Count>0)for (int i = 0; i < octrees.Count; ++i)
        {
                if (!octrees[i].splitted)
                {
                    Gizmos.color = new Color(1f, 0, 0, .1f);
                    Gizmos.DrawWireCube(octrees[i].position * 10, octrees[i].size * new Vector3(10, 10, 10));
                }
                    if (octrees[i].parent!=null)
                    {
                        Gizmos.color = new Color(0, 1f, 0, .5f);
                        Gizmos.DrawLine(octrees[i].position*10,octrees[i].parent.position*10);
                    }
        }
    }
    private void RoadTo(Vector3 origin, Vector3 target, Transform parent) 
    {
        Vector3 walker = origin;
        if (origin.z == target.z)
        {
            for (int i = 0; i < Mathf.Abs(origin.x - target.x); i+=7) 
            {
                walker += new Vector3(Mathf.Sign(origin.x - target.x), 0, 0)*-7.5f;
                Instantiate(roads[1], walker, Quaternion.Euler(0, 90, 0), parent);
            }
        }
        else
        {
            for (int i = 0; i < Mathf.Abs(origin.z - target.z); i+=7)
            {
                walker += new Vector3(0,0,Mathf.Sign(origin.z - target.z)) * -7.5f;
                Instantiate(roads[1], walker, Quaternion.Euler(0, 0, 0), parent);
            }
        }
    }
    private class Octree 
    {
        public Vector3 position;
        public float size;
        public bool splitted;
        public Octree[] childs;
        public Octree parent;

        public Octree(Vector3 position, float size)
        {
            this.position = position;
            this.size = size;
        }
        public Octree(Vector3 position, float size, Octree parent)
        {
            this.position = position;
            this.size = size;
            this.parent = parent;
        }

        public void Split() 
        {
            splitted = true;
            /*childs = new Octree[]
            {
                new Octree(position+new Vector3(-size*.25f,-size*.25f,-size*.25f),size/2),
                new Octree(position+new Vector3(-size*.25f,-size*.25f,size*.25f),size/2),
                new Octree(position+new Vector3(-size*.25f,size*.25f,-size*.25f),size/2),
                new Octree(position+new Vector3(-size*.25f,size*.25f,size*.25f),size/2),
                new Octree(position+new Vector3(size*.25f,-size*.25f,-size*.25f),size/2),
                new Octree(position+new Vector3(size*.25f,-size*.25f,size*.25f),size/2),
                new Octree(position+new Vector3(size*.25f,size*.25f,-size*.25f),size/2),
                new Octree(position+new Vector3(size*.25f,size*.25f,size*.25f),size/2),
            };*/
            childs = new Octree[]
            {
            new Octree(position+new Vector3(-size*.25f,0,-size*.25f),size/2,this),
            new Octree(position+new Vector3(-size*.25f,0,size*.25f),size/2,this),
            new Octree(position+new Vector3(size*.25f,0,-size*.25f),size/2,this),
            new Octree(position+new Vector3(size*.25f,0,size*.25f),size/2,this),
            };
        }
    }
}
