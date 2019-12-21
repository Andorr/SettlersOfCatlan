using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GraphicFade : MonoBehaviour
{
    public Graphic graphic;

    public void FadeInAndOut(float timeSpeed = 2f, float duration = 3f) {
        StopAllCoroutines();
        graphic.color = new Color(graphic.color.r, graphic.color.g, graphic.color.b, 0f);
        StartCoroutine(FadeGraphicInAndOut(timeSpeed, duration));
    }

    public void FadeIn(float timeSpeed = 2f) {
        StartCoroutine(FadeInGraphic(timeSpeed));
    }

    public void FadeOut(float timeSpeed = 2f) {
        StartCoroutine(FadeOutGraphic(timeSpeed));
    }

    private IEnumerator FadeInGraphic(float timeSpeed) {
        graphic.color = new Color(graphic.color.r, graphic.color.g, graphic.color.b, 0f);

        while(graphic.color.a < 1f) {
            graphic.color = new Color(graphic.color.r, graphic.color.g, graphic.color.b, graphic.color.a + (Time.deltaTime * timeSpeed));
            yield return null;
        }
    }

    private IEnumerator FadeOutGraphic(float timeSpeed)
    {
        graphic.color = new Color(graphic.color.r, graphic.color.g, graphic.color.b, 1);
        while (graphic.color.a > 0.0f)
        {
            graphic.color = new Color(graphic.color.r, graphic.color.g, graphic.color.b, graphic.color.a - (Time.deltaTime * timeSpeed));
            yield return null;
        }
    }

    private IEnumerator FadeGraphicInAndOut(float timeSpeed, float duration) {
        FadeIn(timeSpeed);
        yield return new WaitForSeconds(duration);
        FadeOut(timeSpeed);
    }
}
