using BinarySerialization;
using System;
using ublox.Core.Data;
using ublox.Core.Messages.Enums;

namespace ublox.Core.Messages
{
    public class NavPvat : PacketPayload
    {
        [FieldOrder(0)]
        public GpsTimeOfWeek GpsTimeOfWeek { get; set; }

        [FieldOrder(1)]
        public byte Version { get; set; }

        [FieldOrder(2)]
        public UbloxDateTimeV2 UbloxDateTime { get; set; }

        [FieldOrder(3)]
        public GnssFixType FixType { get; set; }

        [FieldOrder(4)]
        public byte Flags { get; set; }

        [FieldOrder(5)]
        public byte Flags2 { get; set; }

        [FieldOrder(6)]
        public byte SatelliteCount { get; set; }

        [FieldOrder(7)]
        [FieldScale(10000000)]
        [SerializeAs(SerializedType.Int4)]
        public double Longitude { get; set; }

        [FieldOrder(8)]
        [FieldScale(10000000)]
        [SerializeAs(SerializedType.Int4)]
        public double Latitude { get; set; }

        [FieldOrder(9)]
        public SignedDistance Height { get; set; }

        [FieldOrder(10)]
        public SignedDistance HeightAboveMeanSeaLevel { get; set; }

        [FieldOrder(11)]
        public UnsignedDistance HorizontalAccuracyEstimate { get; set; }

        [FieldOrder(12)]
        public UnsignedDistance VerticalAccuracyEstimate { get; set; }

        [FieldOrder(13)]
        public Velocity3 Velocity { get; set; }

        [FieldOrder(14)]
        public SignedVelocity GroundSpeed { get; set; }

        [FieldOrder(15)]
        public UnsignedVelocity SpeedAccuracyEstimate { get; set; }

        [FieldOrder(16)]
        [FieldScale(100000)]
        [SerializeAs(SerializedType.UInt4)]
        public double vehRoll{ get; set; }

        [FieldOrder(17)]
        [FieldScale(100000)]
        [SerializeAs(SerializedType.UInt4)]
        public double vehPitch { get; set; }

        [FieldOrder(18)]
        [FieldScale(100000)]
        [SerializeAs(SerializedType.UInt4)]
        public double vehHeading { get; set; }

        [FieldOrder(19)]
        [FieldScale(100000)]
        [SerializeAs(SerializedType.Int4)]
        public double HeadingOfMotion { get; set; }

        [FieldOrder(20)]
        [FieldScale(100)]
        [SerializeAs(SerializedType.UInt2)]
        public double accRoll { get; set; }

        [FieldOrder(21)]
        [FieldScale(100)]
        [SerializeAs(SerializedType.UInt2)]
        public double accPitch { get; set; }

        [FieldOrder(22)]
        [FieldScale(100)]
        [SerializeAs(SerializedType.UInt2)]
        public double accHeading { get; set; }

        [FieldOrder(23)]
        [FieldScale(100)]
        [SerializeAs(SerializedType.Int2)]
        public double magDec{ get; set; }

        [FieldOrder(24)]
        [FieldScale(100)]
        [SerializeAs(SerializedType.UInt2)]
        public double MagAcc { get; set; }

        [FieldOrder(25)]
        [FieldScale(100)]
        [SerializeAs(SerializedType.UInt2)]
        public double errEllipseOrient{ get; set; }

        [FieldOrder(26)]
        public UInt32 errEllipseMajor { get; set; }

        [FieldOrder(27)]
        public UInt32 errEllipseMinor{ get; set; }

        [FieldOrder(28)]        
        public UInt32 rReserved2 { get; set; }
        [FieldOrder(29)]
        public UInt32 rReserved3 { get; set; }

    }
}
