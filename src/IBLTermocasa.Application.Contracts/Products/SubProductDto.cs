using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using IBLTermocasa.Common;
using Volo.Abp.Application.Dtos;

namespace IBLTermocasa.Products
{
    public class SubProductDto : EntityDto<Guid>
    { 
        [Required]
        public int Order { get; set; }
        [Required]
        public  Guid ParentId { get;  set; }
        public string? ParentPlaceHolder { get; set; } = null;
        public string PlaceHolder=>  PlaceHolderUtils.GetPlaceHolder(PlaceHolderType.PRODUCT, Code, ParentPlaceHolder);
        [Required]
        public virtual string Code { get; set; }
        [Required]
        public string Name { get; set; } = null!;
        public bool IsSingleProduct { get; set; }
        public bool Mandatory { get; set; }
        public Guid ProductId { get; set; } = new();

    }
}