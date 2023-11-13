using UnityEngine;

public class Controller : MonoBehaviour
{
  public static void Toggle(bool enable)
  {
    var controllers = FindObjectsOfType<Controller>();
    foreach (var c in controllers)
    {
      c.ToggleInput(enable);
    }
  }

  public PlayerControls Controls;
  public Vector2 MousePosition => Controls.Mouse.Position.ReadValue<Vector2>();
  public Vector2 KeyboardMovement => Controls.Keyboard.Move.ReadValue<Vector2>();
  public float KeyboardRotation => Controls.Keyboard.Rotation.ReadValue<float>();
  public PlayerControls.MouseActions Mouse => Controls.Mouse;
  public PlayerControls.KeyboardActions Keyboard => Controls.Keyboard;

  protected virtual void Awake()
  {
    Controls = new();
    Controls.Enable();
  }

  public virtual void ToggleInput(bool enable)
  {
    if (enable)
      Controls.Enable();
    else
      Controls.Disable();
  }
}
