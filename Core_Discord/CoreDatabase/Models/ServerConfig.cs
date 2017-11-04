﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Core_Discord.CoreDatabase.Models
{
    public class ServerConfig : DbEntity
    {
        public ulong ServerId { get; set; }
        public string Prefix { get; set; }
        public ulong AutoAssignRoleId { get; set; }

        //music settings/voice settings
        public float DefaultMusicVolume = 1.0f;
        public bool AutoDcFromVc { get; set; }

        //server time
        public string Locale { get; set; } = null;
        public string TimeZoneId { get; set; } = null;

        //experience system
        public ExpSettings ExpSettings { get; set; }
    }
}
