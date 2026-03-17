using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace RoomService.Models;

public class RoomBooking
{
    public int Id { get; set; }
    public int RoomId { get; set; }
    public string BookedBy { get; set; } = string.Empty;
    public string Purpose { get; set; } = string.Empty;
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }

    [JsonIgnore]
    public Room? Room { get; set; }  
    
    public string Status { get; set; } = "Aktiv";

}
