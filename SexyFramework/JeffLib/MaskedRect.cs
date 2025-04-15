using SexyFramework.Misc;

namespace JeffLib
{
	public class MaskedRect
	{
		public Rect r;

		public int a;

		public MaskedRect()
		{
			r = default(Rect);
			a = 0;
		}

		public MaskedRect(Rect _r)
		{
			r = new Rect(_r);
			a = 0;
		}

		public MaskedRect(Rect _r, int alpha)
		{
			r = new Rect(_r);
			a = alpha;
		}
	}
}
