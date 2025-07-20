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
            DocumentType.Kz => new SupplierInfo
            {
                Name = "TOO \"Евраз-стандарт\"",
                INN = "250140017474",
                Address =
                    "Республика Казахстан, Восточно-Казахстанская область, город Усть-Каменогорск, Проспект Абая, здание 181",
                BankDetails = "АО \"Банк ЦентрКредит\"",
                BankAccount = "KZ6785622031435537314",
                BIK = "KCJBKZKX",
                Kbe = "17",
                PaymentCode = "890"
            },
            DocumentType.KzAkt => new SupplierInfo
            {
                Name = "ООО \"Рога и Копыта\"",
                INN = "7701234567",
                Address = "Россия, г. Москва, ул. Примерная, д. 1",
                BankDetails = "ПАО Сбербанк",
                BankAccount = "40702810900000000001",
                BIK = "044525225",
                Kbe = null,
                PaymentCode = null
            },
            DocumentType.Ru => new SupplierInfo
            {
                Name = "ООО \"НОРДСИС\"",
                KPP = "540201001",
                OGRN = "1225400047267",
                INN = "5402075654",
                Address = "Россия, г. Новосибирск, Красный пр-кт, д. 153Г, помещ 6",
                BankDetails = "ПАО Сбербанк",
                BankAccount = "40702810900000000001"
            },
            DocumentType.RuAkt => new SupplierInfo
            {
                Name = "ООО \"НОРДСИС\"",
                KPP = "540201001",
                OGRN = "1225400047267",
                INN = "5402075654",
                Address = "Россия, г. Новосибирск, Красный пр-кт, д. 153Г, помещ 6",
                BankDetails = "ПАО Сбербанк",
                BankAccount = "40702810900000000001"
            },
            _ => throw new NotImplementedException()
        };
    }
}