﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using IBLTermocasa.Common;
using IBLTermocasa.Types;
using Volo.Abp.Domain.Repositories;

namespace IBLTermocasa.BillOfMaterials
{
    public interface IBillOfMaterialRepository : IRepository<BillOfMaterial, Guid>
    {

        Task DeleteAllAsync(
            string? filterText = null,
            string? bomNumber = null,
            RequestForQuotationProperty? requestForQuotationProperty = null,
            BomStatusType? status = null,
            CancellationToken cancellationToken = default);
        Task<List<BillOfMaterial>> GetListAsync(
                    string? filterText = null,
                    string? bomNumber = null,
                    RequestForQuotationProperty? requestForQuotationProperty = null,
                    BomStatusType? status = null,
                    string? sorting = null,
                    int maxResultCount = int.MaxValue,
                    int skipCount = 0,
                    CancellationToken cancellationToken = default
                );

        Task<long> GetCountAsync(
            string? filterText = null,
            string? bomNumber = null,
            RequestForQuotationProperty? requestForQuotationProperty = null,
            BomStatusType? status = null,
            CancellationToken cancellationToken = default);
    }
}