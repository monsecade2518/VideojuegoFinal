using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class Atajos : MonoBehaviour
{
   void Update()
   {
      if (Keyboard.current.digit1Key.wasPressedThisFrame) {
        XPpointsManager.Instance.SumarPuntos(60);
      }

      if (Keyboard.current.digit2Key.wasPressedThisFrame) {
        XPpointsManager.Instance.SumarPuntos(120);
      }

      if (Keyboard.current.digit3Key.wasPressedThisFrame) {
        XPpointsManager.Instance.SumarPuntos(210);
      }

      if (Keyboard.current.digit4Key.wasPressedThisFrame) {
        XPpointsManager.Instance.SumarPuntos(310);
      }

      if (Keyboard.current.digit5Key.wasPressedThisFrame) {
        XPpointsManager.Instance.SumarPuntos(460);
      }

      if (Keyboard.current.digit6Key.wasPressedThisFrame) {
        LivesManager.Instance.SumarVidas(3);
        }

      }
   
   
}
