
namespace DB.Data
{
    public class StudentClass
    {
    public int SerialNo { get; set; }
    public string StudentName { get; set; } = "";
    public string Gender { get; set; } = "";
    public string Religion { get; set; } = "";
    public int? BirthDay { get; set; }
    public int? BirthMonth { get; set; }
    public int? BirthYear { get; set; }
    public string Address { get; set; } = "";
    public string NationalId { get; set; } = "";
    public string? Phone { get; set; }
    public string? Email { get; set; }
    }
}