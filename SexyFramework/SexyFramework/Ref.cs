namespace SexyFramework
{
	public class Ref<STRUCT_TYPE>
	{
		public STRUCT_TYPE value;

		public Ref(STRUCT_TYPE initial)
		{
			value = initial;
		}

		public static implicit operator STRUCT_TYPE(Ref<STRUCT_TYPE> obj)
		{
			return obj.value;
		}
	}
}
