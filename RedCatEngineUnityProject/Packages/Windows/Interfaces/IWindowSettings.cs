using RedCatEngine.DependencyInjection.Containers.Interfaces;

namespace RedCatEngine.Windows.Interfaces
{
	public interface IWindowSettings
	{
		uint ID { get; }
		WindowLayer Layer { get; }
		
		bool TryOpen(IApplicationContainer applicationContainer);
	}
}