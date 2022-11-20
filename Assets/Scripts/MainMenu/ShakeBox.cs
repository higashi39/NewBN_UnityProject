using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class ShakeBox : MonoBehaviour
{
    [field: Header("Property")]
    [field: SerializeField] float RandomTimerWaitMin { set; get; } = 2.0f;
    [field: SerializeField] float RandomTimerWaitMax { set; get; } = 5.0f;
    [field: SerializeField] float ShakePowerMin { set; get; } = 1.5f;
    [field: SerializeField] float ShakePowerMax { set; get; } = 3.5f;
    [field: SerializeField] int ShakePowerFrame { set; get; } = 7;
    [field: SerializeField] float RandomTimerShakeMin { set; get; } = 1.0f;
    [field: SerializeField] float RandomTimerShakeMax { set; get; } = 2.5f;

    float lastShakePower = 0.0f;

    Coroutine animationCoroutine;

    IEnumerator ShakeBoxAnimation()
    {
        bool isWaitTime = true;
        float timer = Random.Range(RandomTimerWaitMin, RandomTimerWaitMax);
        int frameCounter = 0;
        float rotateTime = ShakePowerFrame / 60.0f;

        while (true)
        {
            timer -= Time.deltaTime;
            if (isWaitTime)
            {
                if (timer <= 0.0f)
                {
                    timer = Random.Range(RandomTimerShakeMin, RandomTimerShakeMax);
                    isWaitTime = false;
                }
            }
            //Shake
            else
            {
                if (frameCounter % ShakePowerFrame == 0)
                {
                    float randomPower = Random.Range(ShakePowerMin, ShakePowerMax);
                    if (lastShakePower > 0) randomPower *= -1.0f;
                    lastShakePower = randomPower;
                    Vector3 targetRotation = new Vector3(randomPower, transform.eulerAngles.y, 0.0f);
                    transform.DORotate(targetRotation, rotateTime);
                    frameCounter = 0;
                }

                ++frameCounter;
                if (timer <= 0.0f)
                {
                    transform.eulerAngles = new Vector3(0.0f, transform.eulerAngles.y, 0.0f);
                    timer = timer = Random.Range(RandomTimerWaitMin, RandomTimerWaitMax);
                    isWaitTime = true;
                    frameCounter = 0;
                }
            }

            yield return null;
        }
    }


    private void OnEnable()
    {
        animationCoroutine = StartCoroutine(ShakeBoxAnimation());
    }

    private void OnDisable()
    {
        transform.eulerAngles = new Vector3(0.0f, transform.eulerAngles.y, 0.0f);
        StopCoroutine(animationCoroutine);
    }

}
