using System;
using System.Collections.Generic;
using System.Linq;

namespace BejeweledLivePlus
{
	public class SimpleObjectPool
	{
		private List<object> deadObjects_;

		private Type contentType_;

		public SimpleObjectPool(int size, Type contentType)
		{
			contentType_ = contentType;
			deadObjects_ = new List<object>();
		}

		public object alloc()
		{
			object result;
			if (deadObjects_.Count > 0)
			{
				result = deadObjects_.Last();
				deadObjects_.RemoveAt(deadObjects_.Count - 1);
			}
			else
			{
				result = Activator.CreateInstance(contentType_);
			}
			return result;
		}

		public void release(object p)
		{
			deadObjects_.Add(p);
		}
	}
}
