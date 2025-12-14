using CsvHelper.Configuration.Attributes;
namespace DataVizApp.Models;

public class MusicData
{
    [Name("User_ID")]
    public string UserID { get; set; } = string.Empty;

    [Name("Age")]
    public int Age { get; set; }

    [Name("Country")]
    public string Country { get; set; } = string.Empty;

    [Name("Streaming Platform")]
    public string StreamingPlatform { get; set; } = string.Empty;

    [Name("Top Genre")]
    public string TopGenre { get; set; } = string.Empty;

    [Name("Minutes Streamed Per Day")]
    public int MinutesStreamedPerDay { get; set; }

    [Name("Number of Songs Liked")]
    public int NumberOfSongsLiked { get; set; }

    [Name("Most Played Artist")]
    public string MostPlayedArtist { get; set; } = string.Empty;

    [Name("Subscription Type")]
    public string SubscriptionType { get; set; } = string.Empty;

    [Name("Listening Time (Morning/Afternoon/Night)")]
    public string ListeningTime { get; set; } = string.Empty;

    [Name("Discover Weekly Engagement (%)")]
    public double DiscoverWeeklyEngagement { get; set; }

    [Name("Repeat Song Rate (%)")]
    public double RepeatSongRate { get; set; }
}