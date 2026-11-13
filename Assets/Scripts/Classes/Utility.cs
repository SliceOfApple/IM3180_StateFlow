using System.Collections.Generic;
using System.Xml.Serialization;

public class Utility
{
    public class ComboHitAccuracy
    {
        public ArrowDirection dir;
        public HitAccuracy acc;
    }

    /// <summary>
    /// Type of hit. Seven categories.
    /// </summary>
    public enum HitAccuracy
    {
        PERFECT,
        GOOD,
        OK,
        EARLY,
        LATE,
        MISS,
        NA
    }
    public enum ArrowDirection
    {
        [XmlEnum("left")]
        LEFT = 1,
        [XmlEnum("up")]
        UP,
        [XmlEnum("down")]
        DOWN,
        [XmlEnum("right")]
        RIGHT
    }

    // #region Beatmap XML Classes
    [XmlRoot("Beatmap")]
    public class Beatmap
    {
        [XmlAttribute]
        public string name;
        [XmlElement("SongProperty")]
        public SongProperty SongProperty;
        [XmlElement("Structure")]
        public Structure Structure;
    }

    [XmlRoot("SongProperty")]
    public class SongProperty
    {
        [XmlAttribute]
        public int tempo;
        [XmlAttribute]
        public int pickup;
        [XmlAttribute]
        public int beatsPerBar;
    }

    [XmlRoot("Structure")]
    public class Structure
    {
        [XmlElement("Segment")]
        public List<Segment> Segments { get; set; } = new List<Segment>();
    }

    [XmlRoot("Segment")]
    public class Segment
    {
        [XmlAttribute]
        public string name;
        [XmlAttribute]
        public bool isUptime;
        [XmlElement("Beat")]
        public List<Beat> Beats { get; set; } = new List<Beat>();

        // Non-XML attribute properties
        public double TotalLength;
        //{
        //    get
        //    {
        //        double total = 0;
        //        foreach (var beat in Beats)
        //        {
        //            total += beat.length;
        //        }
        //        return total;
        //    }
        //}
    }

    [XmlRoot("Beat")]
    public class Beat
    {
        [XmlAttribute]
        public double length;
        [XmlAttribute]
        public string type;
        [XmlAttribute]
        public string levelEvent;
        [XmlAttribute]
        public string target;
        [XmlAttribute]
        public ArrowDirection dir;
    }

    // #endregion
}
