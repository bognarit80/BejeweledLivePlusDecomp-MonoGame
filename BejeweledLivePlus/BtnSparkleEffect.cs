using BejeweledLivePlus;
using BejeweledLivePlus.Bej3Graphics;
using SexyFramework;
using SexyFramework.Graphics;

public class BtnSparkleEffect : Effect
{
	public CurvedVal mCurvedRotate;

	public double mBaseRot;

	public double mRotMult;

	public QuestMenuBtn mAnchorBtn;

	public void init()
	{
		mAnchorBtn = null;
		mDAlpha = 0f;
		mCurvedRotate.SetCurve(BejeweledLivePlus.GlobalMembers.MP("b;0,0.8,0.01,2,####     CS,E#    ^~SYE"));
		mCurvedScale.SetCurve(BejeweledLivePlus.GlobalMembers.MP("b;0,0.5,0.01,2,#6JCjB6)8    )~###    X><uQV'<.a"));
		mCurvedAlpha.SetCurve(BejeweledLivePlus.GlobalMembers.MP("b;0,0.75,0.01,2,####    5~### t~###   v'###"));
	}

	public override void Draw(Graphics g)
	{
	}

	public override void Update()
	{
		if (mCurvedAlpha.HasBeenTriggered() || (double)mAnchorBtn.mAlpha == 0.0 || !mAnchorBtn.mVisible)
		{
			mDeleteMe = true;
		}
	}
}
