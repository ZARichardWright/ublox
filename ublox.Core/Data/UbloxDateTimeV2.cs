﻿using System;
using BinarySerialization;

namespace ublox.Core.Data
{
    public class UbloxDateTimeV2
    {
        public UbloxDateTimeV2()
        {
        }

        public UbloxDateTimeV2(DateTime dateTime)
        {
            DateTime = dateTime;
        }

        public UbloxDateTimeV2(ushort year, byte month, byte day, byte hour, byte minute, byte second, byte validityFlags, uint timeAccuractyNs, int nanosecond)
        {
            Year = year;
            Month = month;
            Day = day;
            Hour = hour;
            Minute = minute;
            Second = second;

            ValidityFlags = validityFlags;
            TimeAccuracyNs = timeAccuractyNs;

            Nanosecond = nanosecond;
        }

        [FieldOrder(0)]
        public byte ValidityFlags { get; set; }  

        [FieldOrder(1)]
        public ushort Year { get; set; }

        [FieldOrder(2)]
        public byte Month { get; set; }

        [FieldOrder(3)]
        public byte Day { get; set; }

        [FieldOrder(4)]
        public byte Hour { get; set; }

        [FieldOrder(5)]
        public byte Minute { get; set; }

        [FieldOrder(6)]
        public byte Second { get; set; }

        [FieldOrder(7)]
        public byte reserved1 { get; set; }

        [FieldOrder(8)]
        public UInt16 reserved2 { get; set; }

        [FieldOrder(9)]
        public uint TimeAccuracyNs { get; set; }

        [FieldOrder(10)]
        public int Nanosecond { get; set; }

        [Ignore]
        public DateTime DateTime
        {
            get
            {
                try
                {
                    var dt = new DateTime(Year, Month, Day, Hour, Minute, Second);
                    var ticks = Nanosecond / Constants.NanosecondsPerTick;
                    return dt.AddTicks(ticks);
                }
                catch
                {   
                    var dt = new DateTime();
                    var ticks = 0;
                    return dt.AddTicks(ticks);
                }
                
            }

            set
            {
                Year = (ushort) value.Year;
                Month = (byte) value.Month;
                Day = (byte) value.Day;
                Hour = (byte) value.Hour;
                Minute = (byte) value.Minute;
                Second = (byte) value.Second;

                // ignore leap seconds so we don't break DateTime
                if (Second > 59)
                {
                    Second = 59;
                }

                var remainder = value - new DateTime(Year, Month, Day, Hour, Minute, Second);
                Nanosecond = (int) (remainder.Ticks * Constants.NanosecondsPerTick);
            }
        }

        [Ignore]
        public TimeSpan TimeAccuracy => TimeSpan.FromTicks(TimeAccuracyNs / Constants.NanosecondsPerTick);
    }
}
