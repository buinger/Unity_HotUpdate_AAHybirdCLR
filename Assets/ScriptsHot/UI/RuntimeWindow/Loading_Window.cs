using UnityEngine;
using UnityEngine.UI;

public class Loading_Window : RunTimeWindow
{
    float nowPercentValue;
    float targetShowPercent;
    public Text percentShow;

    public void ChangeTargetPercent(float value)
    {
        targetShowPercent = value;
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        targetShowPercent = 0;
        nowPercentValue = 0;
        percentShow.text = "";
    }

    private void Update()
    {
        if (targetShowPercent != 0)
        {
            nowPercentValue = Mathf.Lerp(nowPercentValue, targetShowPercent, 0.1f);
            percentShow.text = nowPercentValue.ToString("0")+"%";
        }
        else
        {
            percentShow.text = "‘ÿ»Î÷–";
        }
    }

}
