using SexyFramework;
using SexyFramework.Graphics;
using SexyFramework.Misc;
using SexyFramework.Widget;

namespace JeffLib
{
	public class DialogEx : Dialog
	{
		public int mFlushPriority;

		public CurvedVal mDrawScale = new CurvedVal();

		public DialogEx(Image theComponentImage, Image theButtonComponentImage, int theId, bool isModal, string theDialogHeader, string theDialogLines, string theDialogFooter, int theButtonMode)
			: base(theComponentImage, theButtonComponentImage, theId, isModal, theDialogHeader, theDialogLines, theDialogFooter, theButtonMode)
		{
			mFlushPriority = -1;
			mDrawScale.SetConstant(1.0);
		}

		public virtual void PreDraw(Graphics g)
		{
			mWidgetManager.FlushDeferredOverlayWidgets(mFlushPriority);
			Graphics3D graphics3D = g?.Get3D();
			if ((double)mDrawScale != 1.0 && graphics3D != null)
			{
				SexyTransform2D theTransform = new SexyTransform2D(false);
				theTransform.Translate(0f - g.mTransX - (float)(mWidth / 2), 0f - g.mTransY - (float)(mHeight / 2));
				theTransform.Scale((float)(double)mDrawScale, (float)(double)mDrawScale);
				theTransform.Translate(g.mTransX + (float)(mWidth / 2), g.mTransY + (float)(mHeight / 2));
				graphics3D.PushTransform(theTransform);
			}
		}

		public override void DrawAll(ModalFlags theFlags, Graphics g)
		{
			PreDraw(g);
			base.DrawAll(theFlags, g);
			PostDraw(g);
		}

		public virtual void PostDraw(Graphics g)
		{
			Graphics3D graphics3D = g?.Get3D();
			if ((double)mDrawScale != 1.0)
			{
				graphics3D?.PopTransform();
			}
		}

		public override void Update()
		{
			base.Update();
			if (!mDrawScale.HasBeenTriggered())
			{
				MarkDirty();
			}
			if (!mDrawScale.IncInVal() && (double)mDrawScale == 0.0)
			{
				GlobalMembers.gSexyAppBase.KillDialog(this);
			}
		}
	}
}
