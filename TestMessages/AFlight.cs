// See https://aka.ms/new-console-template for more information
using BinarySerialization;
using SharpKml.Base;
using SharpKml.Dom;
using SharpKml.Dom.GX;
using System.Reflection.Metadata;
using ublox.Core;
using ublox.Core.Messages;

class DataPoint
{
    public NavPvat GpsAT { get; set; }
    
}

class AFlight
{
    private string Filename="";

    List<DataPoint> Flight = new List<DataPoint>();

    public void LoadUBX(string file)
    {
        Filename = file;
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
                MessageId MId = (MessageId)BitConverter.ToInt16(filebytes.Skip(i + 2).Take(2).Reverse().ToArray());
                UInt16 Len = BitConverter.ToUInt16(filebytes, i + 4);
                if (i + Len + 6 > filebytes.Length)
                    break;
                var GPSmsg = filebytes.Skip(i + 2).Take(Len + 6).ToArray();
                Len += (2 + 1 + 1 + 2 + 2); //Header,class,Id,Len,Chksum

                if (MId == MessageId.NAV_PVAT)
                {
                    var packet = Serializer.Deserialize<Packet>(GPSmsg);
                    var Msg = (NavPvat)packet.Content.Payload;
                    Flight.Add(new DataPoint { GpsAT = Msg }); 
                    //if (Msg.FixType >= ublox.Core.Messages.Enums.GnssFixType.TwoD)
                    {
                        Console.WriteLine("{0},{1},{2},{3}",  Msg.UbloxDateTime.Hour, Msg.FixType, Msg.accPitch, Msg.vehPitch);
                    }
                }
            }
        }
    }
    public void WriteKML(string OutFile = "")
    {
        string outklm;

        if (OutFile == "")
            outklm = Path.Combine(Path.GetDirectoryName(Filename), Path.ChangeExtension(Filename, ".kml"));
        else
            outklm = OutFile;


        LineString ls = new();


        ls.Coordinates = new CoordinateCollection();
        var kml2 = new SharpKml.Dom.Document();

        var style6 = new Style();
        style6.Id = "check-hide-children";

        var listitemtype = new ListItemType();
        listitemtype = ListItemType.CheckHideChildren;
        var liststyle = new ListStyle();
        liststyle.ItemType = listitemtype;
        style6.List = liststyle;

        var style1 = new Style();
        style1.Id = "hl";
        style1.Icon = new IconStyle();
        style1.Icon.Scale = 0.5;
        style1.Icon.Icon = new IconStyle.IconLink(new Uri("http://maps.google.com/mapfiles/kml/shapes/placemark_circle.png"));
        style1.Icon.Color = new Color32(255, 0, 0, 255);

        var fold = new Folder();
        fold.Id = "f0";
        fold.Name = "RawData";
        fold.Visibility = true;
        
        Tour tour = new Tour();
        tour.Name = "First Person View";
        Playlist tourplaylist = new Playlist();

        kml2.AddStyle(style6);
        fold.AddStyle(style1);

        var track = new SharpKml.Dom.GX.Track();
        DateTime lasttime =Flight.FirstOrDefault().GpsAT.UbloxDateTime.DateTime;

        foreach (var m in Flight)
        {
            var Cam = new SharpKml.Dom.Camera();
            if (m.GpsAT.UbloxDateTime.DateTime == new DateTime())
            {
                continue;
            }

            var point = new SharpKml.Dom.Point
            {
                Coordinate = new Vector(m.GpsAT.Latitude, m.GpsAT.Longitude, m.GpsAT.HeightAboveMeanSeaLevel.TotalMeters),
            };
            
            Placemark pm = new Placemark() { };
            point.AltitudeMode = SharpKml.Dom.AltitudeMode.Absolute;
            pm.Geometry = point;
            pm.Visibility = false;
            pm.StyleUrl = new Uri("#hl", UriKind.Relative);
            
            //pm.AddChild(Cam);
            var TS = new SharpKml.Dom.Timestamp();
            TS.When = m.GpsAT.UbloxDateTime.DateTime;
            pm.Time = TS;            
            fold.AddFeature(pm);

            FlyTo flyto = new FlyTo();
            flyto.Duration = (m.GpsAT.UbloxDateTime.DateTime- lasttime).TotalMilliseconds / 1000.0;

            flyto.Mode = FlyToMode.Smooth;
            SharpKml.Dom.Camera cam = new SharpKml.Dom.Camera();
            cam.AltitudeMode = SharpKml.Dom.AltitudeMode.Absolute;
            cam.Latitude = m.GpsAT.Latitude;
            cam.Longitude = m.GpsAT.Longitude;
            cam.Altitude = m.GpsAT.HeightAboveMeanSeaLevel.TotalMeters;
            cam.Heading = m.GpsAT.vehHeading;
            cam.Roll = -m.GpsAT.vehRoll;
            if(Math.Abs(m.GpsAT.vehPitch) < 90)
                cam.Tilt = (90 - (m.GpsAT.vehPitch * -1));
            else
                cam.Tilt = 90;

            cam.GXTimePrimitive = TS;

            flyto.View = cam;
            //if (Math.Abs(flyto.Duration.Value) > 0.1)
            {
                tourplaylist.AddTourPrimitive(flyto);
                
            }

            var vector = new SharpKml.Base.Vector(m.GpsAT.Latitude, m.GpsAT.Longitude, m.GpsAT.HeightAboveMeanSeaLevel.TotalMeters);            
            track.AddCoordinate(vector);
            track.AddWhen(m.GpsAT.UbloxDateTime.DateTime);
            track.AddAngle(new SharpKml.Base.Angle(m.GpsAT.vehPitch, m.GpsAT.vehHeading, m.GpsAT.vehRoll));
            lasttime = m.GpsAT.UbloxDateTime.DateTime;

        }

        Placemark trackPm = new Placemark() {  };
        trackPm.Geometry = track;
        trackPm.Name = "Track";
        trackPm.Visibility = false;

        //kml2.AddFeature(trackPm);
        kml2.AddFeature(fold);
        tour.Playlist = tourplaylist;            
        kml2.AddFeature(tour);

        var serializer = new Serializer();
        serializer.Serialize(kml2);
        File.WriteAllText(outklm, serializer.Xml);
    }


}



