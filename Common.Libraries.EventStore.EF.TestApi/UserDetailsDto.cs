using Common.Libraries.Services.Dtos;

public class UserDetailsDto : IDTO
{
    public int RecordId { get; set; }

    public Guid UserId { get; set; }

    public string UserName { get; set; }

    public Guid SpouseId { get; set; }
}