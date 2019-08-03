using TowerFall;

namespace Mod
{
  class MyXGamepadInput : XGamepadInput
  {
    public MyXGamepadInput (int xGamepadID) : base(xGamepadID)
    {
    }

    public override void StopRumble ()
    {
      // Prevent MacOS freeze with rumble off.
      if (SaveData.Instance.Options.GamepadVibration) {
        base.StopRumble();
      }
    }
  }
}
