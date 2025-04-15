using BejeweledLivePlus.Misc;
using SexyFramework;
using SexyFramework.Graphics;

namespace BejeweledLivePlus
{
	public class Bej3P3DListener : MeshListener
	{
		public Graphics g;

		public Bej3P3DListener()
		{
			g = null;
		}

		public override void MeshPreLoad(Mesh theMesh)
		{
		}

		public override void MeshHandleProperty(Mesh theMesh, string theMeshName, string theSetName, string thePropertyName, string thePropertyValue)
		{
		}

		public override SharedImageRef MeshLoadTex(Mesh theMesh, string theMeshName, string theSetName, string theTexType, string theFileName)
		{
			bool isNew = false;
			SharedImageRef sharedImage = GlobalMembers.gApp.GetSharedImage($"images\\{GlobalMembers.gApp.mArtRes}\\tex\\{BejeweledLivePlus.Misc.Common.GetFileName(theFileName, true)}", "", ref isNew, false, false);
			if (sharedImage == null)
			{
				sharedImage = GlobalMembers.gApp.GetSharedImage($"images\\NonResize\\tex\\{BejeweledLivePlus.Misc.Common.GetFileName(theFileName, true)}", "", ref isNew, false, false);
			}
			if (sharedImage != null)
			{
				if (theFileName.IndexOf("nebula1") == -1)
				{
					sharedImage.GetImage().ReplaceImageFlags(8u);
				}
				sharedImage.GetMemoryImage().mPurgeBits = true;
			}
			return sharedImage;
		}

		public override void MeshPreDraw(Mesh theMesh)
		{
		}

		public override void MeshPostDraw(Mesh theMesh)
		{
		}

		public override void MeshPreDrawSet(Mesh theMesh, string theMeshName, string theSetName, bool hasBump)
		{
		}

		public override void MeshPostDrawSet(Mesh theMesh, string theMeshName, string theSetName)
		{
		}

		public override void MeshPreDeleted(Mesh theMesh)
		{
		}
	}
}
