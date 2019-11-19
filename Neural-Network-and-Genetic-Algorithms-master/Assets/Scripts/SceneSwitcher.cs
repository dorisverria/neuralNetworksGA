using UnityEngine;
using UnityEngine.UI;

public class SceneSwitcher : MonoBehaviour
{
    float   timer;
    bool    appearing;
    bool    loading;

    public void Initialize()
    {
        gameObject.SetActive(true);
        Dissapear();
    }

    public void ManualUpdate()
    {
        if (loading)
        {
            if (appearing)
            {
                Appearing();
            }
            else
            {
                Dissapearing();
            }
        }
    }

    void Appearing()
    {
        if (timer > 0.5f)
        {
            loading = false;
        }
        else
        {
            timer += Time.deltaTime;
        }
    }

    void Dissapearing()
    {
        if (timer < 0)
        {
            loading = false;
            gameObject.SetActive(false);
        }
        else
        {
            timer -= Time.deltaTime;
        }
    }

    public void Appear()
    {
        gameObject.SetActive(true);
        appearing = true;
        loading = true;
        timer = 0;
    }

    public void Dissapear()
    {
        gameObject.SetActive(true);
        appearing = false;
        loading = true;
        timer = 0.5f;
    }

    public bool playingAnimation
    {
        get { return loading; }
    }
}
