using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Volo.Abp.Domain.Entities;

namespace IBLTermocasa.BillOfMaterials;

public class BowItemDto : Entity<Guid>
{
    public virtual Guid IdProfessionalProfile { get; set; }
    public virtual string Code { get; set; }
     public virtual string Name { get; set; }
    public virtual double HourPrice { get; set; }
    public virtual int WorkTime { get; set; }
    public virtual double Price { get; set; }

    protected BowItemDto()
    {
    }

    public BowItemDto(Guid id, Guid idProfessionalProfile, string code, string name, double hourPrice, int workTime, double price) : base(id)
    {
        IdProfessionalProfile = idProfessionalProfile;
        Code = code;
        Name = name;
        HourPrice = hourPrice;
        WorkTime = workTime;
        Price = price;
    }
}