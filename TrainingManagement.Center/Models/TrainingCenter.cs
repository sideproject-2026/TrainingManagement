using BuildingBlock.Domain.Core;
using BuildingBlock.Util.Utils;

namespace TrainingManagement.Center.Models;

public class TrainingCenter : AggregateRoot<Guid>
{

    public static TrainingCenter CreateNew(string name,string code,string email, string description, string address, string municipality, string contactNo)
        => new(Guid.NewGuid(), name, code, email, description, address, municipality, contactNo);
    public TrainingCenter()
    {
        
    }
    private TrainingCenter(Guid Id, string name,string code,string email, string description, string address, string municipality, string contactNo) 
        : base(Id)
    {
        Name = name;
        Code = code;
        Email = email;
        Slug = UrlHelper.Slugify(name);
        Description = description;
        Address = address;
        Municipality = municipality;
        ContactNo = contactNo;
    }

    public string Name { get; private set; } = default!;
    public string Email { get; private set; } = default!;
    public string Code { get; private set; } = default!;
    public string Slug { get; private set; } = default!;
    public string Description { get; set; } = default!;
    public string Address { get; set; } = default!;
    public string Municipality { get; set; } = default!;
    public string ContactNo { get; set; } = default!;

}
