using RedCatEngine.DependencyInjection;

namespace RedCatEngine.Windows.Interfaces
{
	public interface IWindowSettings
	{
		uint ID { get; }
		WindowLayer Layer { get; }
		
		bool TryOpen(IApplicationContainer applicationContainer);
	}
}