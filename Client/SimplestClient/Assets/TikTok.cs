using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TikTok : MonoBehaviour
{
    public GameObject gameOverPanel;
    public Text gameOverText;
    [SerializeField]
    public bool pieces;
    public bool v_a1, v_a2, v_a3, v_b1, v_b2, v_b3, v_c1, v_c2, v_c3;
    public GameObject b_a1, b_a2, b_a3, b_b1, b_b2, b_b3, b_c1, b_c2, b_c3;
    private string side;
    private bool assign;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void SetSpace(GameObject button)
    {
        if (side == "X") { assign = false; }
        else { assign = true; }
       
        this.GetType().GetProperty(button.name).SetValue(this, assign);
        
    }
}
