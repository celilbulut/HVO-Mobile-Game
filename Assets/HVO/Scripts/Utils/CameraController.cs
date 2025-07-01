using UnityEngine;

/// <summary>
/// Kamera kontrolü: Mobilde parmak hareketi, bilgisayarda mouse ile ekranı kaydırmak için kullanılır.
/// </summary>
/// 
public class CameraController
{
    private float m_PanSpeed; // Masaüstü cihazlar için pan (kaydırma) hızı
    private float m_MobilePanSpeed; // Mobil cihazlar için pan hızı

    public CameraController(float panSpeed, float mobilePanSpeed)
    {
        m_PanSpeed = panSpeed;
        m_MobilePanSpeed = mobilePanSpeed;
    }

    /// <summary>
    /// Her frame'de çağrılır. Mouse veya touch hareketine göre kamera pozisyonunu değiştirir.
    /// </summary>
    public void Update()
    {
        // Mobil cihazda tek parmakla ekran üzerinde hareket varsa
        if (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Moved)
        {
            Vector2 touchDeltaPosition = Input.GetTouch(0).deltaPosition;

            // Ekran çözünürlüğüne göre hareketi normalize et
            Vector2 normalizedDelta = touchDeltaPosition / new Vector2(Screen.width, Screen.height);

            // Kamerayı ters yönde kaydır (scroll hissiyatı gibi)
            Camera.main.transform.Translate(
                -normalizedDelta.x * m_MobilePanSpeed,
                -normalizedDelta.y * m_MobilePanSpeed,
                0
            );
        }
        // Masaüstü: Sol mouse tuşuna basılı tutuluyorsa
        else if (Input.touchCount == 0 && Input.GetMouseButton(0))
        {
            Vector2 mouseDeltaPosition = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));

            // Kamerayı mouse hareketine göre kaydır (delta pozisyon, zamanla çarpılarak smooth hale getirilir)
            Camera.main.transform.Translate(
                mouseDeltaPosition.x * Time.deltaTime * m_PanSpeed,
                mouseDeltaPosition.y * Time.deltaTime * m_PanSpeed,
                0
            );
        }
    }
}