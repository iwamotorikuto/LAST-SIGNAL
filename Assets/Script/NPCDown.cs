using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCDown : MonoBehaviour
{
    static readonly int IsDownHash = Animator.StringToHash("IsDown");

    [SerializeField] Animator animator;
    public bool IsDown { get; private set; } = true; //最初からダウン

    void Awake()
    {
        if(animator == null) animator = GetComponent<Animator>();
        animator.SetBool(IsDownHash, IsDown);
    }

    public void Revive()
    {
        if (!IsDown) return;
        IsDown = false;
        animator.SetBool(IsDownHash, false);
    }
}
