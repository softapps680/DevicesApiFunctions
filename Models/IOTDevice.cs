using Microsoft.Azure.Devices;
using System;
using System.Collections.Generic;
using System.Text;

namespace DevicesApiFunctions.Models
{
    public class IOTDevice
    {

        public string DeviceId { get; set; } 
        public string DeviceType { get; set; }

        public string ConnectionState { get; set; }
        public string Status { get; set; } 
        public string JsonData { get; set; } 
        public DateTime JsonDataLastUpdated { get; set; }

    }
    }

