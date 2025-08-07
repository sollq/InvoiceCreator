using Core.Interfaces;
using Core.Models;
using Infrastructure.Services.Interfaces;

namespace Infrastructure.Services;

public class MyCompanyInfoProvider : IMyCompanyInfoProvider
{
    public SupplierInfo GetInfo(DocumentType type)
    {
        return type switch
        {
            DocumentType.InvoiceKz => new SupplierInfo
            {
                Name = "TOO \"Евраз-стандарт\"",
                INN = "250140017474",
                Address =
                    "Республика Казахстан, Восточно-Казахстанская область, город Усть-Каменогорск, Проспект Абая, здание 181",
                BankDetails = "АО \"Банк ЦентрКредит\"",
                BankAccount = "KZ678562203143557314",
                BIK = "KCJBKZKX",
                Kbe = "17",
                PaymentCode = "890"
            },
            DocumentType.KzAkt => new SupplierInfo
            {
                Name = "TOO \"Евраз-стандарт\"",
                INN = "250140017474",
                Address =
                    "Республика Казахстан, Восточно-Казахстанская область, город Усть-Каменогорск, Проспект Абая, здание 181",
                BankDetails = "АО \"Банк ЦентрКредит\"",
                BankAccount = "KZ678562203143557314",
                BIK = "KCJBKZKX",
                Kbe = "17",
                PaymentCode = "890"
            },
            DocumentType.InvoiceRu => new SupplierInfo
            {
                Name = "ООО \"НОРДСИС\"",
                KPP = "540201001",
                OGRN = "1225400047267",
                INN = "5402075654",
                Address = "Россия, г. Новосибирск, Красный пр-кт, д. 153Г, помещ 6",
                BankDetails = "ФИЛИАЛ \"НОВОСИБИРСКИЙ\" АО \"АЛЬФА-БАНК\"",
                BankAccount = "40702810900000000001",
                BankCoreAcc = "30101810600000000774",
                CoreAcc = "40702810823670000756",
                BIK = "045004774"
            },
            DocumentType.RuAkt => new SupplierInfo
            {
                Name = "ООО \"НОРДСИС\"",
                KPP = "540201001",
                OGRN = "1225400047267",
                INN = "5402075654",
                Address = "Россия, г. Новосибирск, Красный пр-кт, д. 153Г, помещ 6",
                BankDetails = "ФИЛИАЛ \"НОВОСИБИРСКИЙ\" АО \"АЛЬФА-БАНК\"",
                BankAccount = "40702810900000000001",
                BankCoreAcc = "30101810600000000774",
                CoreAcc = "40702810823670000756",
                BIK = "045004774"
            },
            _ => throw new NotImplementedException()
        };
    }
}