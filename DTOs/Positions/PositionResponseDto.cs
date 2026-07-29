namespace WebApplication2.DTO.Positions
{
    public class PositionResponseDto
    {
       public int PersonId { get; set; }

       public int VenueId { get; set; }

       public int FloorId { get; set; }

       public int ZoneId { get; set; }

       public int X { get; set; }

       public int Y { get; set; }  

       public DateTime LastUpdate { get; set; }


    }
}