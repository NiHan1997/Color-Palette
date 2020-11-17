using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 颜色选择滑动控制器.
/// </summary>
public class ColorSlider : MonoBehaviour
{
    private Transform m_Transform;
    private Slider m_Slider;
    private RawImage sliderImage;    

    private Texture2D colorTexture;

    [SerializeField]
    private int textureWidth = 800;

    [SerializeField]
    private int textureHeight = 100;

    private void Start()
    {
        Init();
        CreateColorTexture();

        m_Slider.onValueChanged.AddListener(SliderValueChanged);
    }

    /// <summary>
    /// 初始化工作.
    /// </summary>
    private void Init()
    {
        m_Transform = gameObject.GetComponent<Transform>();
        m_Slider = gameObject.GetComponent<Slider>();
        sliderImage = m_Transform.Find("Background").GetComponent<RawImage>();
    }

    /// <summary>
    /// 计算出颜色的程序纹理.
    /// </summary>
    private void CreateColorTexture()
    {
        colorTexture = new Texture2D(textureWidth, textureHeight);

        // 双线性插值计算.
        for (int i = 0; i < textureHeight; ++i)
        {
            colorTexture.SetPixel(0, i, Color.red);
            colorTexture.SetPixel(textureWidth / 3 - 1, i, Color.green);
            colorTexture.SetPixel(textureWidth / 3 * 2 - 1, i, Color.blue);
            colorTexture.SetPixel(textureWidth - 1, i, Color.red);

            for (int j = 1; j < textureWidth / 3 - 1; ++j)
            {
                colorTexture.SetPixel(j, i,
                    new Color(1 - (1.0f / (textureWidth / 3) * j), 1.0f / (textureWidth / 3) * j, 0));
            }

            for (int j = textureWidth / 3; j < textureWidth / 3 * 2 - 1; ++j) 
            {
                colorTexture.SetPixel(j, i,
                    new Color(0, 1 - (1.0f / (textureWidth / 3) * (j - textureWidth / 3)), 
                    1.0f / (textureWidth / 3) * (j - textureWidth / 3)));
            }

            for (int j = textureWidth / 3 * 2; j < textureWidth - 1; ++j)
            {
                colorTexture.SetPixel(j, i,
                    new Color(1.0f / (textureWidth / 3) * (j - textureWidth / 3 * 2), 
                    0, 1 - (1.0f / (textureWidth / 3) * (j - textureWidth / 3 * 2))));
            }
        }

        colorTexture.wrapMode = TextureWrapMode.Clamp;
        colorTexture.Apply();

        sliderImage.texture = colorTexture;
    }

    private void SliderValueChanged(float value)
    {
        int u = (int)(value * textureWidth);
        Color color = colorTexture.GetPixel(u, 1);
        ColorPanel.Instance.CreateColorTexture(color);
        ColorPanel.Instance.ChangeFinalColor();
    }
}
