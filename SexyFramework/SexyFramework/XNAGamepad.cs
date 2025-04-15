using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using SexyFramework.Drivers;

namespace SexyFramework
{
	internal class XNAGamepad : IGamepad
	{
		public static GamepadData mGamepadData = new GamepadData();

		public override bool IsConnected()
		{
			return false;
		}

		public override int GetGamepadIndex()
		{
			return 0;
		}

		public override bool IsButtonDown(GamepadButton button)
		{
			return mGamepadData.mButton[(int)button];
		}

		public override float GetButtonPressure(GamepadButton button)
		{
			return 0f;
		}

		public override float GetAxisXPosition()
		{
			return 0f;
		}

		public override float GetAxisYPosition()
		{
			return 0f;
		}

		public override float GetRightAxisXPosition()
		{
			return 0f;
		}

		public override float GetRightAxisYPosition()
		{
			return 0f;
		}

		public override void Update()
		{
			if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
			{
				mGamepadData.mButton[4] = true;
			}
			if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Released)
			{
				mGamepadData.mButton[4] = false;
			}
		}

		public override void AddRumbleEffect(float theLeft, float theRight, float theFadeTime)
		{
		}
	}
}
