using System.ComponentModel;
using Viettel.Services;

namespace VIETTEL.Application
{
    public class MvcDisplayName : DisplayNameAttribute//, IModelAttribute
    {
        private string _resourceValue = string.Empty;
        private readonly ILocalizationService _localizationService;

        public MvcDisplayName(string resourceKey)
            : base(resourceKey)
        {
            ResourceKey = resourceKey;
            //_localizationService = ServiceFactory.Get<ILocalizationService>();
            _localizationService = LocalizationService.Default;
        }

        public string ResourceKey { get; set; }

        public override string DisplayName
        {
            get
            {
                _resourceValue = _localizationService.Translate(ResourceKey.Trim());
                return _resourceValue;
            }
        }

        public string Name => "MvcDisplayName";
    }
}
