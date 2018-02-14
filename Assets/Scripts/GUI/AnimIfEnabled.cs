using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired.UI.ControlMapper;

public class AnimIfEnabled : MonoBehaviour
{
    private Animator animator;
    private CustomButton but;

    private void Awake()
    {
        animator = gameObject.GetComponent<Animator>();
        but = gameObject.GetComponent<CustomButton>();
    }
    
    /// <summary>
    /// set l'animator à selected or not
    /// </summary>
    /// <param name="active"></param>
    public void GuiSelected(bool active)
    {
        if (but.interactable && active)
            animator.SetBool("Select", true);
        else
            animator.SetBool("Select", false);
    }
}
