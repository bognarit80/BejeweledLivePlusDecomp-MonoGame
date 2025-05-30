using System.Collections.Generic;
using SexyFramework.Graphics;

namespace SexyFramework.Widget
{
	public class BlendSrcData
	{
		public List<PAParticleEffect> mParticleEffectVector = new List<PAParticleEffect>();

		public PATransform mTransform = new PATransform(true);

		public Color mColor = default(Color);
	}
}
