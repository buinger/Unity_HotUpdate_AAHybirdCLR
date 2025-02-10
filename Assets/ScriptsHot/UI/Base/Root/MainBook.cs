using UnityEngine;

public class MainBook : Book
{
    
    protected override void Awake()
    {
        base.Awake();

        if (transform.GetComponent<Canvas>() != null)
        {
            UiManager.NowBook = this;
        }
     
    }

    
}
