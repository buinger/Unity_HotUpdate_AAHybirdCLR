using UnityEngine;

public class MainBook : Book
{
    public bool awakeToPage1 = false;
    private void Awake()
    {
        if (transform.GetComponent<Canvas>() != null)
        {
            UiManager.NowBook = this;
        }

        foreach (var item in pages)
        {
            item.SetActive(true);
        }
    }

    private void Start()
    {
        foreach (var item in pages)
        {
            item.SetActive(false);
        }
        if (awakeToPage1)
        {
            ChangePageTo(1);
        }
    }
}
