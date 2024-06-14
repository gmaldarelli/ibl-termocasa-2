﻿using System;
using System.Collections.Generic;
using System.Linq;
using Shouldly;
using System.Threading.Tasks;
using IBLTermocasa.Common;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Modularity;
using Xunit;

namespace IBLTermocasa.BillOfMaterials
{
    public abstract class BillOFMaterialsAppServiceTests<TStartupModule> : IBLTermocasaApplicationTestBase<TStartupModule>
        where TStartupModule : IAbpModule
    {
        private readonly IBillOfMaterialsAppService _billOFMaterialsAppService;
        private readonly IRepository<BillOFMaterial, Guid> _billOFMaterialRepository;

        public BillOFMaterialsAppServiceTests()
        {
            _billOFMaterialsAppService = GetRequiredService<IBillOfMaterialsAppService>();
            _billOFMaterialRepository = GetRequiredService<IRepository<BillOFMaterial, Guid>>();
        }

        [Fact]
        public async Task GetListAsync()
        {
            // Act
            var result = await _billOFMaterialsAppService.GetListAsync(new GetBillOfMaterialsInput());

            // Assert
            result.TotalCount.ShouldBe(2);
            result.Items.Count.ShouldBe(2);
            result.Items.Any(x => x.Id == Guid.Parse("69297841-8d3b-44de-a9dd-87fdbdf73964")).ShouldBe(true);
            result.Items.Any(x => x.Id == Guid.Parse("4b23ffce-c8af-456d-96ad-3db8439a9117")).ShouldBe(true);
        }

        [Fact]
        public async Task GetAsync()
        {
            // Act
            var result = await _billOFMaterialsAppService.GetAsync(Guid.Parse("69297841-8d3b-44de-a9dd-87fdbdf73964"));

            // Assert
            result.ShouldNotBeNull();
            result.Id.ShouldBe(Guid.Parse("69297841-8d3b-44de-a9dd-87fdbdf73964"));
        }

        [Fact]
        public async Task CreateAsync()
        {
            // Arrange
            var input = new BillOfMaterialCreateDto
            {
                Name = "a166121b4be946a58e4be696fdfb2a071bc9e74f7eca4b",
                RequestForQuotationProperty = new RequestForQuotationPropertyDto(),
                ListItems = new List<BOMItemDto>()
            };

            // Act
            var serviceResult = await _billOFMaterialsAppService.CreateAsync(input);

            // Assert
            var result = await _billOFMaterialRepository.FindAsync(c => c.Id == serviceResult.Id);

            result.ShouldNotBe(null);
            result.Name.ShouldBe("a166121b4be946a58e4be696fdfb2a071bc9e74f7eca4b");
            result.RequestForQuotationProperty.ShouldBe(new RequestForQuotationProperty());
            result.ListItems.ShouldBe(new List<BOMItem>());
        }

        [Fact]
        public async Task UpdateAsync()
        {
            // Arrange
            var input = new BillOfMaterialUpdateDto()
            {
                Name = "eb3faf122fa3407f9283db7c6fac7037490b07c82b0d4bf599e12ac52c5cba03b7bc16485f4845f88fcf8",
                RequestForQuotationProperty = new RequestForQuotationPropertyDto(),
                ListItems = new List<BOMItemDto>()
            };

            // Act
            var serviceResult = await _billOFMaterialsAppService.UpdateAsync(Guid.Parse("69297841-8d3b-44de-a9dd-87fdbdf73964"), input);

            // Assert
            var result = await _billOFMaterialRepository.FindAsync(c => c.Id == serviceResult.Id);

            result.ShouldNotBe(null);
            result.Name.ShouldBe("eb3faf122fa3407f9283db7c6fac7037490b07c82b0d4bf599e12ac52c5cba03b7bc16485f4845f88fcf8");
            result.RequestForQuotationProperty.ShouldBe(new RequestForQuotationProperty());
            result.ListItems.ShouldBe(new List<BOMItem>());
        }

        [Fact]
        public async Task DeleteAsync()
        {
            // Act
            await _billOFMaterialsAppService.DeleteAsync(Guid.Parse("69297841-8d3b-44de-a9dd-87fdbdf73964"));

            // Assert
            var result = await _billOFMaterialRepository.FindAsync(c => c.Id == Guid.Parse("69297841-8d3b-44de-a9dd-87fdbdf73964"));

            result.ShouldBeNull();
        }
    }
}