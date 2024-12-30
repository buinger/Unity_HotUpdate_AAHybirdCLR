using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class AnimationPlot : Plot
{
    protected static Dictionary<string, Animator> soleEntities = new Dictionary<string, Animator>();

    public bool isSole = true;
    //��������·��
    public PrefabInfo prefabInfo;
    //������ʼλ��
    public Vector3 playPosition = Vector3.zero;
    //������ʼ����
    public Vector3 playScale = Vector3.one;
    //������ʼ��ת
    public Vector3 playEuler = Vector3.one;
    //������
    public string animationName;


    //��������
    //public string animationIntro;
    //����������
    protected Animator targetAnimator;
    //����ģʽ
    //public WrapMode wrapMode = WrapMode.Default;
    //����·��
    //public string animationClipPath;
    //������Դ
    //protected AnimationClip animationClip;


    protected override IEnumerator MainLogic()
    {
        yield return StartCoroutine(GetPlayableDirector());

        yield return StartCoroutine(PlayAnimation());
    }

    public static void ReleaseSoleEntities()
    {
        soleEntities.Clear();
    }




    IEnumerator GetPlayableDirector()
    {
        if (isSole)
        {
            if (!soleEntities.ContainsKey(prefabInfo.path))
            {
                Task<GameObject> getTarget = GetGameObjectEntityAsync(true, prefabInfo.path, playPosition, null, false);
                while (getTarget.IsCompleted == false)
                {
                    yield return null;
                }
                GameObject animationObject = getTarget.Result;
                Animator temp = animationObject.GetComponent<Animator>();
                soleEntities.Add(prefabInfo.path, temp);
            }
            targetAnimator = soleEntities[prefabInfo.path];
        }
        else
        {
            Task<GameObject> getTarget = GetGameObjectEntityAsync(true, prefabInfo.path, playPosition, null, false);
            while (getTarget.IsCompleted == false)
            {
                yield return null;
            }
            GameObject animationObject = getTarget.Result;
            targetAnimator = animationObject.GetComponent<Animator>();
        }
        SetTrans();
    }

    protected virtual void SetTrans()
    {
        targetAnimator.transform.position = playPosition;
        targetAnimator.transform.localScale = playScale;
        targetAnimator.transform.rotation = Quaternion.Euler(playEuler);
    }


    void PlayToEndOfAnimation(Animator animator)
    {
        // ��ȡ��ǰ����״̬����Ϣ
        AnimatorStateInfo currentState = animator.GetCurrentAnimatorStateInfo(0);

        // ����������ڲ����У��� normalizedTime ����Ϊ 1
        if (currentState.normalizedTime < 1.0f)
        {
            animator.Play(currentState.fullPathHash, 0, 1.0f);
        }
    }
    IEnumerator PlayAnimation()
    {
        targetAnimator.gameObject.SetActive(true);
        targetAnimator.Play(animationName);
        yield return null;
        bool isLoop = targetAnimator.GetCurrentAnimatorStateInfo(0).loop;

        if (!isLoop)
        {
            while (targetAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f)
            {
               
                if (Input.GetMouseButtonDown(0))
                {
                    PlayToEndOfAnimation(targetAnimator);
                    break;
                }
                yield return null;
            }
        }
    }

    protected override void Ini(Action onIniOver)
    {
        onIniOver.Invoke();
    }

    protected override void ResetPlot()
    {
        if (!isSole)
        {
            if (targetAnimator != null)
            {
                GameObjectPoolTool.PutInPool(targetAnimator.gameObject);              
            }
        }
        else
        {
            if (targetAnimator != null)
            {
                targetAnimator.gameObject.SetActive(false);
            }
        }
    }
}
