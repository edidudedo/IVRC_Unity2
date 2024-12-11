using UnityEngine;

public class SpriteFadeLoop : MonoBehaviour
{
    public float fadeInDuration = 0.1f;   // Duration of fade-in
    public float fadeOutDuration = 0.1f;  // Duration of fade-out
    public float waitDuration = 0.1f;     // Duration to wait at full opacity and full transparency

    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    private float fadeTimer;
    private enum FadeState { FadeIn, WaitVisible, FadeOut, WaitInvisible }
    private FadeState currentState;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;
        fadeTimer = fadeInDuration;
        currentState = FadeState.FadeIn;
    }

    void Update()
    {
        switch (currentState)
        {
            case FadeState.FadeIn:
                fadeTimer -= Time.deltaTime;
                float newAlphaIn = Mathf.Lerp(0f, originalColor.a, 1 - (fadeTimer / fadeInDuration));
                spriteRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, newAlphaIn);

                if (fadeTimer <= 0)
                {
                    // Fade-in complete, switch to visible wait
                    currentState = FadeState.WaitVisible;
                    fadeTimer = waitDuration;
                }
                break;

            case FadeState.WaitVisible:
                fadeTimer -= Time.deltaTime;
                if (fadeTimer <= 0)
                {
                    // Visible wait complete, switch to fade-out
                    currentState = FadeState.FadeOut;
                    fadeTimer = fadeOutDuration;
                }
                break;

            case FadeState.FadeOut:
                fadeTimer -= Time.deltaTime;
                float newAlphaOut = Mathf.Lerp(0f, originalColor.a, fadeTimer / fadeOutDuration);
                spriteRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, newAlphaOut);

                if (fadeTimer <= 0)
                {
                    // Fade-out complete, switch to invisible wait
                    currentState = FadeState.WaitInvisible;
                    fadeTimer = waitDuration;
                }
                break;

            case FadeState.WaitInvisible:
                fadeTimer -= Time.deltaTime;
                if (fadeTimer <= 0)
                {
                    // Invisible wait complete, start fade-in again
                    currentState = FadeState.FadeIn;
                    fadeTimer = fadeInDuration;
                }
                break;
        }
    }
}
