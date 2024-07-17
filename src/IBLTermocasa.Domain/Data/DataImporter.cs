using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClosedXML.Excel;
using Force.DeepCloner;
using IBLTermocasa.Common;
using IBLTermocasa.Industries;
using IBLTermocasa.Materials;
using IBLTermocasa.Organizations;
using IBLTermocasa.Types;
using Volo.Abp.Identity;

namespace IBLTermocasa;

public class DataImporter
{
    public DataImporter(IOrganizationRepository organizationRepository, 
        IIdentityUserRepository identityUserRepository, 
        IIndustryRepository industryRepository,
        IMaterialRepository materialRepository)
    {
        _organizationRepository = organizationRepository;
        _identityUserRepository = identityUserRepository;
        _industryRepository = industryRepository;
        _materialRepository = materialRepository;
    }

    private IOrganizationRepository _organizationRepository;
    private IIdentityUserRepository _identityUserRepository;
    private IIndustryRepository _industryRepository;
    private IMaterialRepository _materialRepository;
    

    public void ImportCustomerDataFromExcel(string filePath)
    {
        using var workbook = new XLWorkbook(filePath);
        var worksheet = workbook.Worksheet(1); // Assuming data is in the first sheet
        var rows = worksheet.RangeUsed().RowsUsed();

        var industryList  = _industryRepository.GetListAsync("Generico").Result;
        var industry = new Industry(Guid.NewGuid(), "Generico", "Settore Generico");
        if(industryList.Count == 0)
        {
            industry = _industryRepository.InsertAsync(industry).Result;
        }else
        {
            industry = industryList[0];
        }

        var users = _identityUserRepository.GetListAsync(
            userName: "admin"
        );
        var userId = Guid.Empty;
        users.Result.ForEach(u =>
        {
            if (u.UserName == "admin")
            {
                userId = u.Id;
            }
        });
        if (userId.Equals(Guid.Empty))
        {
            Console.WriteLine("Admin user not found.");
            return;
        }
        
        
        var records = new List<Organization>();

        foreach (var row in rows)
        {
            if (row.RowNumber() == 1) continue; // Skip header

            var item = new Organization(Guid.NewGuid());
            var record = new Organization
            {
                Code = ConvertToString(row.Cell("B").Value),
                Name = ConvertToString(row.Cell("A").Value),
                BillingAddress = new Address
                {
                    Street = ConvertToString(row.Cell("E").Value),
                    City = ConvertToString(row.Cell("F").Value),
                    Country = ConvertToString(row.Cell("H").Value),
                    PostalCode = ConvertToString(row.Cell("G").Value),
                    State = "Italia"
                },
                CreationTime = new DateTime(),
                LastModificationTime = new DateTime(),
                LastModifierId = userId,
                CreatorId = userId,
                PhoneInfo = new PhoneInfo
                {
                    PhoneItems = new List<PhoneItem>
                    {
                        new PhoneItem
                        {
                            Number = ConvertToStringAndRemoveChar(row.Cell("I").Value, "/"),
                            Type = PhoneType.PHONE_WORK,
                            Prefix =  CalculatePrefix(ConvertToStringAndRemoveChar(row.Cell("I").Value, "/"))
                        },
                    }
                },
                MailInfo = new MailInfo
                {
                    MailItems = new List<MailItem>
                    {
                        new MailItem
                        {
                            Email = ConvertToString(row.Cell("J").Value),
                            Type = MailType.EMAIL_WORK
                        }
                    }
                },
                Notes = "Imported from Excel",
                OrganizationType = OrganizationType.CUSTOMER,
                IndustryId = industry.Id,
                SourceType = SourceType.MassImport,
                FirstSync = DateTime.Now,
                LastSync = DateTime.Now
            };
            record = this.checkMailAndPhone(record);
            records.Add(Organization.FillPropertiesForInsert(record, item));
        }

        try
        {
            _organizationRepository.InsertManyAsync(records);
            Console.WriteLine("Data imported successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error occurred during data import: " + ex.Message);
        }
    }

    private string? CalculatePrefix(string? convertToStringAndRemoveChar)
    {
        if(convertToStringAndRemoveChar == null || string.IsNullOrWhiteSpace(convertToStringAndRemoveChar))
        {
            return null;
        }
        {
            return "+39";
        }
    }

    private Organization checkMailAndPhone(Organization record)
    {
        var output = record.DeepClone();
        record.PhoneInfo.PhoneItems.ForEach(p =>
        {
            if (string.IsNullOrWhiteSpace(p.Number))
            {
                output.PhoneInfo.PhoneItems.Remove(p);
            }
        });
        record.MailInfo.MailItems.ForEach(m =>
        {
            if (string.IsNullOrWhiteSpace(m.Email))
            {
                output.MailInfo.MailItems.Remove(m);
            }
        });
        return output;
    }

    private string ConvertToString(object input)
    {
        if (input == null || string.IsNullOrWhiteSpace(input.ToString()))
            return null;
        else
            return input.ToString();
    }
    private string ConvertToStringAndRemoveChar(object input, string charToRemove)
    {
        if (input == null || string.IsNullOrWhiteSpace(input.ToString()))
            return null;
        else
            return input.ToString().Replace(charToRemove, "");
    }
    public async Task ImportMaterialDataFromExcel(string filePath)
    {
        using var workbook = new XLWorkbook(filePath);
        var worksheet = workbook.Worksheet(1); // Assuming data is in the first sheet
        var rows = worksheet.RangeUsed().RowsUsed();

        var industryList  = _industryRepository.GetListAsync("Generico").Result;
        var industry = new Industry(Guid.NewGuid(), "Generico", "Settore Generico");
        if(industryList.Count == 0)
        {
            industry = _industryRepository.InsertAsync(industry).Result;
        }else
        {
            industry = industryList[0];
        }

        var users = _identityUserRepository.GetListAsync(
            userName: "admin"
        );
        var userId = Guid.Empty;
        users.Result.ForEach(u =>
        {
            if (u.UserName == "admin")
            {
                userId = u.Id;
            }
        });
        if (userId.Equals(Guid.Empty))
        {
            Console.WriteLine("Admin user not found.");
            return;
        }
        
        
        var records = new List<Material>();
        /*Dictionary<string,decimal> codes = new Dictionary<string,decimal>();
        List<Material> materials = await _materialRepository.GetListAsync();
        foreach (var row in rows)
        {
            var code = ConvertToString(row.Cell("A").Value);
            var price = ConvertStringToDecima(row.Cell("F").Value);
            materials.Where(x => x.Code == code).ToList().ForEach(m =>
            {
                m.StandardPrice = m.StandardPrice/m.Quantity;
            });
        }

        await _materialRepository.UpdateManyAsync(materials);*/
        
        
        
        
        /*
        foreach (var row in rows)
        {
            if (row.RowNumber() == 1) continue; // Skip header
            var record = new Material
            (
                id: Guid.NewGuid(),
                code: ConvertToString(row.Cell("A").Value),
                name: ConvertToString(row.Cell("B").Value),
                measureUnit: ConvertToMeasureUnit(row.Cell("C").Value),
                quantity: ConvertStringToDecima(row.Cell("D").Value),
                lifo: ConvertStringToDecima(row.Cell("E").Value),
                standardPrice: ConvertStringToDecima(row.Cell("F").Value),
                averagePrice: ConvertStringToDecima(row.Cell("G").Value),
                lastPrice: ConvertStringToDecima(row.Cell("H").Value),
                averagePriceSecond: ConvertStringToDecima(row.Cell("I").Value),
                sourceType: SourceType.MassImport,
                firstSync: DateTime.Now,
                lastSync: DateTime.Now
            );
            records.Add(record);
        }
        try
        {
            _materialRepository.InsertManyAsync(records);
            Console.WriteLine("Data imported successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error occurred during data import: " + ex.Message);
        }*/
    }

    private decimal ConvertStringToDecima(object value)
    {
        if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
            return 0;
        else
        {
            if (decimal.TryParse(value.ToString(), out decimal result))
            {
                return result;
            }
            else
            {
                return 0;
            }
        }
    }

    private MeasureUnit ConvertToMeasureUnit(object? input)
    {
        if (input == null || string.IsNullOrWhiteSpace(input.ToString()))
            return MeasureUnit.PZ;
        else
        {
            if(Enum.TryParse(input.ToString(), out MeasureUnit value))
            {
                return value;
            }else
            {
                return MeasureUnit.PZ;
            }
        }
    }
}