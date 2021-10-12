namespace ToolBox.Pools
{
	public interface IPoolable
	{
		/// <summary>
		/// This method will be called on 2nd Reuse call.
		/// Use Unity's Awake method for first initialization and this method for others
		/// </summary>
		void OnReuse();
		void OnRelease();
	}
}
