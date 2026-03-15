namespace ClientManager.Application.Dtos;

public class VersioningFileDto
{
    public DateTimeOffset CreateDate { get; set; }
    public required string Name { get; set; }
    public required string Version { get; set; }
    public required string Description { get; set; }
    public required IEnumerable<BuildDto> Builds { get; set; }
}
