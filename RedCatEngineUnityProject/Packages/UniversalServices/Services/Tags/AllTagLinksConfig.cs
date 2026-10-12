using RedCatEngine.CommonServices.Containers.Storages.Tags;
using RedCatEngine.Configs;
using UnityEngine;

namespace RedCatEngine.CommonServices.Services.Tags
{
	[CreateAssetMenu(menuName = "Configs/Common/Tags/All Tag List Config", fileName = nameof(AllTagLinksConfig))]
	public class AllTagLinksConfig : BaseAllConfigContainerConfig<TagConfig>
	{
	}
}