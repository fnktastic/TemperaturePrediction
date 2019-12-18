using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TemperaturePrediction.Model;

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
            cfg.CreateMap<Scene, Model.Scene>();

            cfg.CreateMap<Model.Scene, Scene>();
        })
        {

        }
    }
}
