using AutoMapper.Attributes;

namespace VIETTEL
{
    public class MapperConfig
    {
        public static void Config()
        {
            AutoMapper.Mapper.Initialize(config =>
            {
                typeof(MapperConfig).Assembly.MapTypes(config);
                //typeof(Email).Assembly.MapTypes(config);
            });
        }
    }
}
