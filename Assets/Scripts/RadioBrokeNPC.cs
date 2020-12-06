using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadioBrokeNPC : MonoBehaviour
{

    bool fix = false;
    private RubyController rubyController;
    Animator animator;
    public ParticleSystem smokeEffect;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        
    }

    // Update is called once per frame
    void Update()
    {
        if(fix == true)
        {
            smokeEffect.Stop();
            animator.SetTrigger("Fix");
        }
    }

    public void Fix ()
    {
        GameObject rubyControllerObject = GameObject.FindWithTag("RubyController");
        fix = true;

        rubyController = rubyControllerObject.GetComponent<RubyController>();
        rubyController.RadioFix(1);
    }
}
