using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// 色环.
/// </summary>
public class ColorRing : MonoBehaviour, IPointerDownHandler, IDragHandler
{
    private RectTransform m_RectTransform;
    private Image m_Image;

    private Transform ringTransform;

    private Texture2D colorTexture;

    private void Start()
    {
        Init();
        CreateColorRingTexture();
    }

    private void Init()
    {
        m_RectTransform = gameObject.GetComponent<RectTransform>();
        m_Image = gameObject.GetComponent<Image>();

        ringTransform = m_RectTransform.Find("Image");
    }

    /// <summary>
    /// 生成色环.
    /// </summary>
    private void CreateColorRingTexture()
    {
        colorTexture = new Texture2D(1024, 1024);

        float center = colorTexture.width / 2.0f;
        for (int i = 0; i < colorTexture.width; ++i)
        {
            for (int j = 0; j < colorTexture.height; ++j)
            {
                if ((i - center) * (i - center) + (j - center) * (j - center) >
                    (colorTexture.width / 2) * (colorTexture.width / 2) ||
                    (i - center) * (i - center) + (j - center) * (j - center) < 400 * 400)
                {
                    colorTexture.SetPixel(i, j, new Color(0, 0, 0, 0));
                }
                else
                {
                    float theta = Mathf.Atan2(j - center, i - center);
                    // Red ---> (1, 1, 0).
                    if (theta >= 0 && theta < Mathf.PI / 3.0f)
                    {
                        Color color = Color.Lerp(Color.red, new Color(1, 1, 0), theta / (Mathf.PI / 3.0f));
                        colorTexture.SetPixel(i, j, color);
                    }
                    // (1, 1, 0) ---> G.
                    else if (theta >= Mathf.PI / 3.0f && theta < Mathf.PI * 2 / 3.0f)
                    {
                        Color color = Color.Lerp(new Color(1, 1, 0), Color.green, theta / (Mathf.PI / 3.0f) - 1);
                        colorTexture.SetPixel(i, j, color);
                    }

                    // R ---> (1, 0, 1).
                    else if (theta < 0 && theta > -Mathf.PI / 3.0f)
                    {
                        Color color = Color.Lerp(Color.red, new Color(1, 0, 1), Mathf.Abs(theta / (Mathf.PI / 3.0f)));
                        colorTexture.SetPixel(i, j, color);
                    }
                    // (1, 0, 1) ---> B.
                    else if (theta <= -Mathf.PI / 3.0f && theta > -Mathf.PI * 2 / 3.0f)
                    {
                        Color color = Color.Lerp(new Color(1, 0, 1), Color.blue, Mathf.Abs(theta / (Mathf.PI / 3.0f) + 1));
                        colorTexture.SetPixel(i, j, color);
                    }

                    // G ---> (0, 1, 1).
                    else if (theta >= Mathf.PI * 2 / 3.0f)
                    {
                        Color color = Color.Lerp(Color.green, new Color(0, 1, 1), Mathf.Abs(theta / (Mathf.PI / 3.0f) - 2));
                        colorTexture.SetPixel(i, j, color);
                    }
                    // B ---> (0, 1, 1).
                    else if (theta <= -Mathf.PI * 2 / 3.0f)
                    {
                        Color color = Color.Lerp(Color.blue, new Color(0, 1, 1), Mathf.Abs(theta / (Mathf.PI / 3.0f) + 2));
                        colorTexture.SetPixel(i, j, color);
                    }
                }
            }
        }

        colorTexture.filterMode = FilterMode.Bilinear;
        colorTexture.Apply();

        m_Image.sprite = Sprite.Create(colorTexture,
            new Rect(0, 0, colorTexture.width, colorTexture.height), Vector2.zero);
        m_Image.alphaHitTestMinimumThreshold = 0.1f;
    }

    void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
    {
        Vector2 localPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(m_RectTransform,
            eventData.position, eventData.enterEventCamera, out localPos);

        float angleRad = Mathf.Atan2(localPos.y, localPos.x);
        float posX = 360 * Mathf.Cos(angleRad);
        float posY = 360 * Mathf.Sin(angleRad);

        ringTransform.localPosition = new Vector3(posX, posY, 0);

        Color color = colorTexture.GetPixel((int)(posX / 400 * (colorTexture.width / 2) + colorTexture.width / 2),
            (int)(posY / 400 * (colorTexture.height / 2) + colorTexture.height / 2));
        ColorPanel.Instance.CreateColorTexture(color);
        ColorPanel.Instance.ChangeFinalColor();
    }

    void IDragHandler.OnDrag(PointerEventData eventData)
    {
        Vector2 localPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(m_RectTransform,
            eventData.position, eventData.enterEventCamera, out localPos);

        float angleRad = Mathf.Atan2(localPos.y, localPos.x);
        float posX = 360 * Mathf.Cos(angleRad);
        float posY = 360 * Mathf.Sin(angleRad);

        ringTransform.localPosition = new Vector3(posX, posY, 0);

        Color color = colorTexture.GetPixel((int)(posX / 400 * (colorTexture.width / 2) + colorTexture.width / 2),
            (int)(posY / 400 * (colorTexture.height / 2) + colorTexture.height / 2));
        ColorPanel.Instance.CreateColorTexture(color);
        ColorPanel.Instance.ChangeFinalColor();
    }
}