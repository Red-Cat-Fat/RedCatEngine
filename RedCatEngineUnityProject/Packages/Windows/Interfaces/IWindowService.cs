using RedCatEngine.Windows.Components.Windows;

namespace RedCatEngine.Windows.Interfaces
{
	public interface IWindowService
	{
		public void Open(BaseWindowConfig windowInfo, params object[] context);
	}
}