using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// 颜色面板管理器.
/// </summary>
public class ColorPanel : MonoBehaviour, IDragHandler
{
    public static ColorPanel Instance;

    private Transform m_Transform;
    private Transform colorCircle;

    private Image finalColorImage;

    private Texture2D colorTexture;

    [SerializeField]
    private int textureWidth = 512;

    [SerializeField]
    private int textureHeight = 512;

    void Awake()
    {
        Instance = this;
    }    

    private void Start()
    {
        Init();
        CreateColorTexture(Color.red);
    }

    /// <summary>
    /// 初始化工作.
    /// </summary>
    private void Init()
    {
        m_Transform = gameObject.GetComponent<Transform>();
        colorCircle = m_Transform.Find("ColorCircle");
        finalColorImage = GameObject.Find("FinalColor").GetComponent<Image>();
    }

    /// <summary>
    /// 计算出颜色的程序纹理.
    /// </summary>
    /// <param name="endColor">右上角的颜色.</param>
    public void CreateColorTexture(Color endColor)
    {
        colorTexture = new Texture2D(textureWidth, textureHeight);

        // 双线性插值计算.
        Color diff = Color.white - endColor;
        for (int i = 0; i < textureWidth; ++i)
        {
            colorTexture.SetPixel(i, 0, Color.black);
            colorTexture.SetPixel(i, textureHeight - 1, Color.white - diff / textureWidth * i);

            for (int j = 1; j < textureHeight - 1; ++j)
            {
                Color top = colorTexture.GetPixel(i, textureHeight - 1);
                Color bottom = colorTexture.GetPixel(i, 0);

                colorTexture.SetPixel(i, j, bottom + (top - bottom) / textureHeight * j);
            }
        }

        colorTexture.wrapMode = TextureWrapMode.Clamp;
        colorTexture.Apply();

        gameObject.GetComponent<RawImage>().texture = colorTexture;
    }

    void IDragHandler.OnDrag(PointerEventData eventData)
    {
        colorCircle.position = eventData.position;

        // 边界处理.
        if (colorCircle.localPosition.x > 400)
            colorCircle.localPosition = new Vector3(400, colorCircle.localPosition.y, 0);
        if (colorCircle.localPosition.x < -400)
            colorCircle.localPosition = new Vector3(-400, colorCircle.localPosition.y, 0);
        if (colorCircle.localPosition.y > 400)
            colorCircle.localPosition = new Vector3(colorCircle.localPosition.x, 400, 0);
        if (colorCircle.localPosition.y < -400)
            colorCircle.localPosition = new Vector3(colorCircle.localPosition.x, -400, 0);

        // 颜色采样.
        int u = (int)(Mathf.Abs(colorCircle.localPosition.x + 400) / 800 * textureWidth);
        int v = (int)((800 - Mathf.Abs(colorCircle.localPosition.y - 400)) / 800 * textureWidth);

        finalColorImage.color = colorTexture.GetPixel(u, v);
    }
}
