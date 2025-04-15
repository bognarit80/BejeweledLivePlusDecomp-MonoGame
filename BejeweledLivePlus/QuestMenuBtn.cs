using BejeweledLivePlus;
using BejeweledLivePlus.Widget;
using SexyFramework.Graphics;
using SexyFramework.Misc;

public class QuestMenuBtn : Bej3Button
{
	public Rect mBaseImgRect;

	public Image mIconImg;

	public int mIconOffX;

	public int mIconOffY;

	public QuestMenuBtn(int theId, Bej3ButtonListener theListener)
		: base(theId, theListener)
	{
		mIconImg = null;
	}

	public override void Draw(Graphics g)
	{
		if (HaveButtonImage(mButtonImage, mBaseImgRect))
		{
			DrawButtonImage(g, mButtonImage, mBaseImgRect, 0, 0);
		}
		base.Draw(g);
		if (mIconImg != null)
		{
			g.DrawImage(mIconImg, GlobalMembers.S(mIconOffX), GlobalMembers.S(mIconOffY));
		}
	}
}
