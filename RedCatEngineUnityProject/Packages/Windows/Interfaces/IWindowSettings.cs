using RedCatEngine.DependencyInjection.Containers.Interfaces.Application;

namespace RedCatEngine.Windows.Interfaces
{
	public interface IWindowSettings
	{
		uint ID { get; }
		WindowLayer Layer { get; }
		
		bool TryOpen(IApplicationContainer applicationContainer);
	}
}