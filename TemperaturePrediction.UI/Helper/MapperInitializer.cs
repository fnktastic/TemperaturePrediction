using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TemperaturePrediction.Model;
using TemperaturePrediction.UI.Model;

namespace TemperaturePrediction.UI.Helper
{
    public class MyMapper : Mapper
    {
        public MyMapper(IConfigurationProvider configurationProvider) : base(configurationProvider)
        {

        }
    }

    public class MyConfig : MapperConfiguration
    {
        public MyConfig() : base(cfg =>
        {
            cfg.CreateMap<SceneDto, Scene>();
            cfg.CreateMap<AreaDto, Area>();
            cfg.CreateMap<ChanelValueDto, ChanelValue>();
            cfg.CreateMap<LatLonDto, LatLon>();
            cfg.CreateMap<MapPointDto, MapPoint>();
            cfg.CreateMap<MeteoDto, Meteo>();
            cfg.CreateMap<PointDto, Point>();
            cfg.CreateMap<SceneDto, Scene>();
            cfg.CreateMap<UnitPointDto, UnitPoint>();
            cfg.CreateMap<TimeLinePointDto, TimeLinePoint>();

            cfg.CreateMap<Scene, SceneDto>();
            cfg.CreateMap<Area, AreaDto>();
            cfg.CreateMap<ChanelValue, ChanelValueDto>();
            cfg.CreateMap<LatLon, LatLonDto>();
            cfg.CreateMap<MapPoint, MapPointDto>();
            cfg.CreateMap<Meteo, MeteoDto>();
            cfg.CreateMap<Point, PointDto>();
            cfg.CreateMap<Scene, SceneDto>();
            cfg.CreateMap<UnitPoint, UnitPointDto>();
            cfg.CreateMap<TimeLinePoint, TimeLinePointDto>();
        })
        {

        }
    }
}
