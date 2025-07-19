using System.ComponentModel;
using System.Reflection.Metadata;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using Core.Models;
using Core.Interfaces;
using Infrastructure.Pdf.Interfaces;
using System.IO;
using QuestPDF.Drawing;

namespace Infrastructure.Pdf;

public class KzInvoicePdfGenerator : IInvoicePdfGenerator
{
    public byte[] Generate(InvoiceData data)
    {
        QuestPDF.Settings.License = LicenseType.Community;
        FontManager.RegisterFont(File.OpenRead("Fonts/times.ttf"));
        FontManager.RegisterFont(File.OpenRead("Fonts/times_bold.ttf"));

        var document = QuestPDF.Fluent.Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(75);
                page.DefaultTextStyle(x => x.FontFamily("Times New Roman").FontSize(11));
                page.Content().Column(col =>
                {
                    // --- Блок с реквизитами ---
                    col.Item().Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn(2); // Левый блок
                            columns.RelativeColumn(2); // Центр
                            columns.RelativeColumn(2); // Правый блок
                        });

                        // Первая строка
                        table.Cell().Row(1).Column(1).Element(CellStyle).Text(t =>
                        {
                            t.AlignLeft();
                            t.Span("Бенефициар: ").Bold();
                            t.Span(data.Seller.Name);
                            t.Line("");
                            t.Span("БИН: ");
                            t.Span(data.Seller.INN);
                        });
                        table.Cell().Row(1).Column(2).Element(CellStyle).Text(t =>
                        {
                            t.AlignCenter();
                            t.Span("ИИК").Bold();
                            t.Line("");
                            t.Span($" {data.Seller.BankAccount}");
                        });
                        table.Cell().Row(1).Column(3).Element(CellStyle).Text(t =>
                        {
                            t.AlignCenter();
                            t.Span("Кбе ").Bold();
                            t.Line("");
                            t.Span($"{data.Seller.Kbe}");
                        });

                        // Вторая строка (банк бенефициара)
                        table.Cell().Row(2).Column(1).Element(CellStyle).Text(t =>
                        {
                            t.AlignLeft();
                            t.Span("Банк бенефициара: ").Bold();
                            t.Line("");
                            t.Span(data.Seller.BankDetails);
                            t.Line("");
                        });
                        table.Cell().Row(2).Column(2).Element(CellStyle).Text(t =>
                        {
                            t.AlignCenter();
                            t.Span("БИК").Bold();
                            t.Line("");
                            t.Span($" {data.Seller.BIK}");
                            t.Line("");
                        });
                        table.Cell().Row(2).Column(3).Element(CellStyle).Text(t =>
                        {
                            t.AlignCenter();
                            t.Span("Код назначения платежа ").Bold();
                            t.Line("");
                            t.Span($"{data.Seller.PaymentCode}");
                            t.Line("");
                        });
                    });

                    // --- Заголовок счета ---
                    col.Item().PaddingTop(15).Text($"Счет на оплату №{data.InvoiceNumber} от {data.Date:yyyy-MM-dd}")
                        .Bold().FontSize(16).AlignLeft().FontFamily("Times New Roman");

                    // --- Поставщик/Покупатель/Договор ---
                    col.Item().PaddingTop(10).Text(t =>
                    {
                        t.Span("Поставщик: ");
                        t.Span($"ИНН/БИН: {data.Seller.INN}, {data.Seller.Name}, {data.Seller.Address}").Bold();
                    });
                    col.Item().Text(t =>
                    {
                        t.Span("Покупатель: ");
                        t.Span($"ИНН/БИН: {data.Buyer.INN}, {data.Buyer.Name}").Bold();
                    });
                    col.Item().Text(t =>
                    {
                        t.Span("Договор: ").Bold();
                        t.Span($"{data.ContractNumber} от {data.Date:dd.MM.yyyy}").Bold();
                    });

                    // --- Таблица товаров/услуг ---
                    col.Item().PaddingTop(10).Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.ConstantColumn(30); // №
                            columns.ConstantColumn(60); // Код
                            columns.RelativeColumn(2);  // Наименование
                            columns.ConstantColumn(40); // Кол-во
                            columns.ConstantColumn(60); // Ед.
                            columns.ConstantColumn(70); // Цена
                            columns.ConstantColumn(80); // Сумма
                        });

                        // Header
                        table.Header(header =>
                        {
                            header.Cell().Element(CellStyle).Text("№").Bold().AlignCenter();
                            header.Cell().Element(CellStyle).Text("Код").Bold().AlignCenter();
                            header.Cell().Element(CellStyle).Text("Наименование").Bold().AlignCenter();
                            header.Cell().Element(CellStyle).Text("Кол-во").Bold().AlignCenter();
                            header.Cell().Element(CellStyle).Text("Ед.").Bold().AlignCenter();
                            header.Cell().Element(CellStyle).Text("Цена").Bold().AlignCenter();
                            header.Cell().Element(CellStyle).Text("Сумма").Bold().AlignCenter();
                        });

                        // Products
                        var i = 1;
                        foreach (var p in data.Products)
                        {
                            table.Cell().Element(CellStyle).Text(i++.ToString()).AlignCenter();
                            table.Cell().Element(CellStyle).Text(p.Code ?? "").AlignCenter();
                            table.Cell().Element(CellStyle).Text(p.Name).AlignCenter();
                            table.Cell().Element(CellStyle).Text(p.Quantity.ToString()).AlignCenter();
                            table.Cell().Element(CellStyle).Text(p.Unit ?? "").AlignCenter();
                            table.Cell().Element(CellStyle).Text($"{p.Price:0.00}").AlignCenter();
                            table.Cell().Element(CellStyle).Text($"{p.Total:0.00}").AlignCenter();
                        }

                        // Итог


                    });

                    // --- Итоги и пропись ---
                    col.Item().PaddingTop(10).Background("#f0f0f0").AlignRight().Text($"\t\tИтого: \t\t{data.TotalAmount:0.00}").Bold().AlignCenter();
                    col.Item().PaddingTop(10).Background("#f0f0f0").Text($"В том числе НДС:").Bold().AlignRight();
                    col.Item().PaddingTop(10).Text($"Всего наименований {data.Products.Count}, на сумму {data.TotalAmount:### ### ##0.00} KZT");
                    col.Item().Text($"Всего к оплате: {data.TotalAmountText}");

                    // --- Подпись ---
                    col.Item().PaddingTop(30).Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn();
                            columns.RelativeColumn();
                        });

                        table.Cell().Text("Исполнитель:").Bold();
                        table.Cell().Text("_________________________");

                        table.Cell().Text("М.П.").Bold();
                        table.Cell().Text(""); // Пустая ячейка под печать
                    });

                });
            });
        });

        return document.GeneratePdf();
    }

    private QuestPDF.Infrastructure.IContainer CellStyle(QuestPDF.Infrastructure.IContainer container)
    {
        return container.Border(1).Padding(1).AlignMiddle().AlignCenter();
    }
}