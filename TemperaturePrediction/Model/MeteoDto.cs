﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TemperaturePrediction.Model
{
    public class MeteoDto
    {
        public float Min { get; set; }
        public float Max { get; set; }

        public float Now { get; set; }

        public MeteoDto(float min, float max, int now)
        {
            Min = min;
            Max = max;
            Now = now;
        }
    }
}
