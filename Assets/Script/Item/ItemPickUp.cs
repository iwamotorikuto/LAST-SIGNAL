using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ItemPickUp : MonoBehaviour
{
    [SerializeField] private ItemData data;
    [SerializeField] private int amount = 1;

    PlayerInventory inventory;

    private void Reset()
    {
        GetComponent<Collider>().isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("接触 : " + other.name);

        PlayerInventory inv =
            other.GetComponent<PlayerInventory>();

        if (inv == null)
            inv = other.GetComponentInParent<PlayerInventory>();

        if (inv == null)
            inv = FindObjectOfType<PlayerInventory>();

        Debug.Log("Inventory : " + inv);

        if (inv == null)
        {
            Debug.Log("PlayerInventory が見つからないよ　NULLです");
            return;
        }

        int added = inv.Add(data, amount);
        Debug.Log($"拾った: {data.itemName} / type={data.type} / +{added} / 所持={inv.Count(data.type)}");

        if (added <= 0) return;

        amount -= added;

        if (amount <= 0)
            Destroy(gameObject);
    }
}