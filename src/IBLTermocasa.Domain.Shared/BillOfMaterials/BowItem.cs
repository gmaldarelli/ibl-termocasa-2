using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Volo.Abp.Domain.Entities;

namespace IBLTermocasa.BillOfMaterials;

public class BowItem : Entity<Guid>
{
    [NotNull] public virtual Guid IdProfessionalProfile { get; set; }
    [NotNull] public virtual string Code { get; set; }
    [NotNull]  public virtual string Name { get; set; }
    [NotNull] public virtual double HourPrice { get; set; }
    [NotNull] public virtual int WorkTime { get; set; }
    [NotNull] public virtual double Price { get; set; }

    protected BowItem()
    {
    }

    public BowItem(Guid id, Guid idProfessionalProfile, string code, string name, double hourPrice, int workTime, double price) : base(id)
    {
        IdProfessionalProfile = idProfessionalProfile;
        Code = code;
        Name = name;
        HourPrice = hourPrice;
        WorkTime = workTime;
        Price = price;
    }
}