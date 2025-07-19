using Core.Interfaces;
using Core.Models;

namespace Infrastructure.Services;

public class MyCompanyInfoProvider : IMyCompanyInfoProvider
{
    public SupplierInfo GetInfo(OrganizationType type)
    {
        return type switch
        {
            OrganizationType.Kz => new SupplierInfo
            {
                Name = "TOO \"Евраз-стандарт\"",
                INN = "250140017474",
                Address = "Республика Казахстан, Восточно-Казахстанская область, город Усть-Каменогорск, Проспект Абая, здание 181",
                BankDetails = "АО \"Банк ЦентрКредит\"",
                BankAccount = "KZ6785622031435537314",
                BIK = "KCJBKZKX",
                Kbe = "17",
                PaymentCode = "890"
            },
            OrganizationType.Ru => new SupplierInfo
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
            _ => throw new NotImplementedException()
        };
    }
} 