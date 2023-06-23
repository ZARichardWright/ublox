// See https://aka.ms/new-console-template for more information
using BinarySerialization;
using ublox.Core;
using ublox.Core.Messages;

Console.WriteLine("Hello, World!");

string file = @"F:\BirdLogger\20230623_093309_19.ubx";

var filebytes = File.ReadAllBytes(file);

BinarySerializer Serializer = new BinarySerializer();

MemoryStream ms = new MemoryStream(filebytes);


for (int i = 0; i < filebytes.Length; i++)
{
    if ((filebytes[i] == 0xb5) && (filebytes[i + 1] == 0x62))
    {
        if (i + 5 > filebytes.Length)
            break;
        byte Class = filebytes[i + 2];
        byte Id = filebytes[i + 3];
        UInt16 Len = BitConverter.ToUInt16(filebytes, i + 4);
        if (i + Len + 6 > filebytes.Length)
            break;
        var GPSmsg = filebytes.Skip(i + 2).Take(Len + 6).ToArray();
        Len += (2 + 1 + 1 + 2 + 2); //Header,class,Id,Len,Chksum

        if (Id == 0x17) { 
            var packet = Serializer.Deserialize<Packet>(GPSmsg); 
            var Msg = (NavPvat)packet.Content.Payload;            
            if (Msg.FixType >= ublox.Core.Messages.Enums.GnssFixType.TwoD)
            {
                Console.WriteLine("{0},{1},{2},{3}", Msg.GpsTimeOfWeek.TimeOfWeek, Msg.FixType,Msg.accPitch, Msg.vehRoll);
            }
        }
    }
}