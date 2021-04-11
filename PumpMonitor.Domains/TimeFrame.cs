using System;
using System.Collections.Generic;
using System.Linq;

namespace PumpMonitor.Domains
{
    public enum TimeFrame
    {
        FiveSec,
        TenSec,
        TwentySec,
        ThirtySec,
        FortySec,
        Min,
        Two,
        Five,
        Ten,
        Twenty,
        Thirty,
        Hour,
        TwoHours
    }

    public static class TimeFrameMapper
    {
        private static readonly IDictionary<TimeFrame, TimeSpan> TimeFramesTs = new Dictionary<TimeFrame, TimeSpan>
        {
            {TimeFrame.FiveSec, TimeSpan.Parse("00:00:05")},
            {TimeFrame.TenSec, TimeSpan.Parse("00:00:10")},
            {TimeFrame.TwentySec, TimeSpan.Parse("00:00:20")},
            {TimeFrame.ThirtySec, TimeSpan.Parse("00:00:30")},
            {TimeFrame.FortySec, TimeSpan.Parse("00:00:40")},
            {TimeFrame.Min, TimeSpan.Parse("00:01:00")},
            {TimeFrame.Two, TimeSpan.Parse("00:02:00")},
            {TimeFrame.Five, TimeSpan.Parse("00:05:00")},
            {TimeFrame.Ten, TimeSpan.Parse("00:10:00")},
            {TimeFrame.Twenty, TimeSpan.Parse("00:20:00")},
            {TimeFrame.Thirty, TimeSpan.Parse("00:30:00")},
            {TimeFrame.Hour, TimeSpan.Parse("01:00:00")},
            {TimeFrame.TwoHours, TimeSpan.Parse("02:00:00")}
        };
        
        public static List<TimeFrame> GetTimeFrames()
        {
            return TimeFramesTs.Keys.ToList();
        }

        public static TimeSpan GetTimeSpan(this TimeFrame tf)
        {
            if (!TimeFramesTs.ContainsKey(tf))
                throw new Exception($"TimeFrame: {tf} not found in mapper");

            return TimeFramesTs[tf];
        }
    }
}