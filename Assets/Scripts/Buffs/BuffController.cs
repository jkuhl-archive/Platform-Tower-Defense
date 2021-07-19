using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEditor;
using UnityEngine;

public class BuffLogic : MonoBehaviour
{
    public List<GameObject> buffedObjs;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
       ProcessBuffs(); 
    }

    // Process the buffs every frame
    private void ProcessBuffs()
    {
        if (buffedObjs.Count <= 0)
        {
            return;
        }
        else
        {
            foreach (var i in buffedObjs)
            {
                
            }
        }
    }
    
    public void AddBuff(GameObject item, Buff buff)
    {
        
    }
}
