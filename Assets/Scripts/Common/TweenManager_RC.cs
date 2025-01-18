
using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Tween��
/// Tween class
/// </summary>
public class Tween_RC
{
    protected float duration; // �����ĳ���ʱ�� Animation Duration
    protected float elapsed; // �����Ѿ���ȥ��ʱ�� Animation time has passed
    protected Action<float> onTweenUpdate; // �������»ص� Animation update callback
    protected Action onTweenComplete; // ������ɻص� Animation completion callback
    protected Func<float, float> easeFunction; // �������� Easing function
    public bool IsComplete { get; private set; } // �����Ƿ���� Animation is complete

    public Tween_RC(float duration, Action<float> onTweenUpdate, Action onTweenComplete)
    {
        this.duration = duration;
        this.onTweenUpdate = onTweenUpdate;
        this.onTweenComplete = onTweenComplete;
        this.easeFunction = linear;

        this.elapsed = 0;
        this.IsComplete = false;
    }

    public virtual void Update(float deltaTime)
    {
        elapsed += deltaTime;
        float t = Mathf.Clamp01(elapsed / duration);

        // Ӧ�û������� Apply easing function
        t = easeFunction(t);

        onTweenUpdate?.Invoke(t);

        if (elapsed >= duration)
        {
            IsComplete = true;
            onTweenComplete?.Invoke();
        }
    }

    public Tween_RC SetEaseFunction(Func<float, float> easeFunction)
    {
        this.easeFunction = easeFunction;
        return this;
    }

    // �������� Easing function
    public static float linear(float t) => t;
    public static float easeInQuad(float t) => t * t;
    public static float easeOutQuad(float t) => 1 - (1 - t) * (1 - t);
    public static float easeInAndOutQuad(float t)
    {
        if (t < 0.5f)
        {
            return 2 * t * t;
        }
        else
        {
            return 1 - Mathf.Pow(-2 * t + 2, 2) / 2;
        }
    }
}

/// <summary>
/// Tween������
/// Tween manager
/// </summary>
public class TweenManager_RC : Singleton<TweenManager_RC>
{
    List<Tween_RC> tweenList = new List<Tween_RC>();

    protected override void Awake()
    {
        base.Awake();

        DontDestroyOnLoad(this);
    }

    void Update()
    {
        for (int i = tweenList.Count - 1; i >= 0; i--)
        {
            var tween = tweenList[i];
            tween.Update(Time.deltaTime);
            if (tween.IsComplete)
            {
                tweenList.RemoveAt(i);
            }
        }
    }

    public void AddTween(Tween_RC tween)
    {
        tweenList.Add(tween);
    }
}

/// <summary>
/// Tween��չ����
/// Tween extension method
/// </summary>
public static class TweenExtensions_RC
{
    public static Tween_RC rc_To(this float from, float to, float duration, Action<float> onTweenUpdate, Action onTweenComplete = null)
    {
        var tween = new Tween_RC(
            duration,
            (float t) => onTweenUpdate(Mathf.Lerp(from, to, t)),
            onTweenComplete
            );

        TweenManager_RC.Instance.AddTween(tween);
        return tween;
    }

    public static Tween_RC rc_To(this Vector3 from, Vector3 to, float duration, Action<Vector3> onTweenUpdate, Action onTweenComplete = null)
    {
        var tween = new Tween_RC(
            duration,
            (float t) => onTweenUpdate(Vector3.Lerp(from, to, t)),
            onTweenComplete
            );

        TweenManager_RC.Instance.AddTween(tween);
        return tween;
    }

    public static Tween_RC rc_DoMove(this Transform transform, Vector3 endValue, float duration)
    {
        Vector3 startPos = transform.position;
        return startPos.rc_To(endValue, duration, (Vector3 pos) => transform.position = pos);
    }

    public static Tween_RC rc_DoScale(this Transform transform, Vector3 endValue, float duration)
    {
        Vector3 startScale = transform.localScale;
        return startScale.rc_To(endValue, duration, (Vector3 scale) => transform.localScale = scale);
    }
}
