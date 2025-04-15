using SexyFramework.Drivers;

namespace SexyFramework
{
	internal class XNAGamepadDriver : IGamepadDriver
	{
		private static XNAGamepad mXNAGamepad;

		private SexyAppBase gApp;

		public static IGamepadDriver CreateGamepadDriver()
		{
			mXNAGamepad = new XNAGamepad();
			return new XNAGamepadDriver();
		}

		public override void Dispose()
		{
		}

		public override int InitGamepadDriver(SexyAppBase app)
		{
			gApp = app;
			return 1;
		}

		public override IGamepad GetGamepad(int theIndex)
		{
			return mXNAGamepad;
		}

		public override void Update()
		{
			mXNAGamepad.Update();
		}
	}
}
