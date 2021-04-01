namespace ToolBox.Pools
{
	public interface IPoolable
	{
		void OnGet();
		void OnRelease();
	}
}
