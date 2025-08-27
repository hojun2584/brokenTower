using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FloatingBar : MonoBehaviour
{
    public static FloatingBar Instance;
    [SerializeField]
    private TextMeshProUGUI textMesh; // 알파값을 줄일 텍스트 컴포넌트
    public float duration = 2.0f; // 페이드 아웃에 걸리는 시간 (초)
    private float startTime;
    private bool isFading = false;


    public string SetTmpText 
    {
        set
        {
            textMesh.text = value;
            SetTextAlpha();
            textMesh.transform.parent.gameObject.SetActive(true);
        }
    }


    private void Awake()
    {
        textMesh = gameObject.GetComponentInChildren<TextMeshProUGUI>();
        if (Instance == null)
            Instance = this;
        else
            Destroy(this);

        DontDestroyOnLoad(this);
        gameObject.SetActive(false);
    }


    private void SetTextAlpha()
    {
        Color color = textMesh.color;
        color.a = 1.0f;
        textMesh.color = color;

        startTime = Time.time;
        isFading = true;
    }


    void Update()
    {
        if (isFading)
        {
            float elapsed = Time.time - startTime;
            float alpha = Mathf.Clamp01(1.0f - (elapsed / duration));

            Color color = textMesh.color;
            color.a = alpha;
            textMesh.color = color;

            // 알파값이 0이 되면 오브젝트를 비활성화
            if (alpha == 0)
            {
                isFading = false;
                gameObject.SetActive(false);
            }
        }
    }

}
