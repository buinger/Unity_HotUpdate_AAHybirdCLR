using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Book : MonoBehaviour
{

    public bool awakeToPage1 = false;
    public List<GameObject> pages = new List<GameObject>();

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    protected virtual void Awake()
    {
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

    public void ChangePageByPageName(string _name)
    {
        for (int i = 0; i < pages.Count; i++)
        {
            if (pages[i].name == _name)
            {
                ChangePageTo(i + 1);
                return;
            }
        }
    }

    public void ChangePageByGameobject(GameObject gameObj)
    {
        CloseAllPages();
        if (pages.Contains(gameObj))
        {
            gameObj.SetActive(true);
        }
    }

    public void ChangePageTo(int _pageNum)
    {
        //Debug.Log(_pageNum.ToString() + " "+pages.Count.ToString());

        //Debug.Log("拥有页数:"+pages.Count);
        if (_pageNum > 0 && _pageNum <= pages.Count)
        {
            for (int i = 0; i < pages.Count; i++)
            {
                if (_pageNum - 1 == i)
                {
                    pages[i].SetActive(true);
                    //Debug.Log("设置第" + _pageNum+ pages[i].name + "页:" + pages[i].activeInHierarchy);

                }
                else
                {
                    pages[i].SetActive(false);
                    // Debug.Log("设置第" + (i+1).ToString() + pages[i].name + "页:" + pages[i].activeInHierarchy);
                }
            }
        }
        else
        {
            return;
        }
    }
    public void TwoPageChangeToAnother()
    {
        if (pages.Count == 2)
        {
            if (pages[0].activeSelf == true)
            {
                pages[0].SetActive(false);
                pages[1].SetActive(true);
            }
            else
            {
                pages[1].SetActive(false);
                pages[0].SetActive(true);
            }
        }
        else
        {
            Debug.Log("两个页面没有切换");
            return;
        }
    }

    public void CloseAllPages()
    {
        foreach (GameObject go in pages)
        {
            go.SetActive(false);
        }
    }

    public void AddChangePage()
    {
        for (int i = 0; i < pages.Count; i++)
        {
            if (pages[i].activeInHierarchy == true)
            {
                int nextPage = i + 2;
                if (nextPage != pages.Count + 1)
                {
                    ChangePageTo(nextPage);
                    return;
                }
                else
                {
                    ChangePageTo(1);
                    return;
                }

            }
        }
    }


    public void ReduceChangePage()
    {
        for (int i = 0; i < pages.Count; i++)
        {
            if (pages[i].activeInHierarchy == true)
            {
                int nextPage = i;
                if (nextPage != 0)
                {
                    ChangePageTo(nextPage);
                    return;
                }
                else
                {
                    ChangePageTo(pages.Count);
                    return;
                }

            }
        }

    }
}
