using XSDe.Models;
using XSDe.Models.Enums;

namespace XSDe.Services.ButtonMappingServices
{
    public interface IMappingService
    {
        public List<ButtonMapping> GetMappings();
        public ButtonMapping? GetMapping(XButton button, XButton? modifier = null, bool isLongPress = false);
    }
}
