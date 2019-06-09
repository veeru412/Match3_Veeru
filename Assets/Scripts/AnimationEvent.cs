using UnityEngine;

public class AnimationEvent : MonoBehaviour
{

    void OnAnimationFinished()
    {
        gameObject.SetActive(false);
    }
}
