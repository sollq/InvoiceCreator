using System.ComponentModel;
using System.Reflection.Metadata;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using Core.Models;
using Core.Interfaces;
using Infrastructure.Pdf.Interfaces;

namespace Infrastructure.Pdf;

public class KzInvoicePdfGenerator : IInvoicePdfGenerator
{
    public byte[] Generate(InvoiceData data)
    {
        var document = QuestPDF.Fluent.Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(20);
                page.DefaultTextStyle(x => x.FontSize(10));
                page.Content().Column(col =>
                {
                    // --- Блок с реквизитами ---
                    col.Item().Container().Border(1).Padding(5).Row(row =>
                    {
                        row.RelativeItem().Column(left =>
                        {
                            left.Item().Text(t =>
                            {
                                t.Span("Бенефициар: ").Bold();
                                t.Span(data.Seller.Name);
                            });
                            left.Item().Text($"БИН: {data.Seller.INN}");
                            left.Item().Text(t =>
                            {
                                t.Span("Банк бенефициара: ").Bold();
                                t.Span(data.Seller.BankDetails);
                            });
                        });
                        row.RelativeItem().Column(right =>
                        {
                            right.Item().Text(t =>
                            {
                                t.Span("ИИК").Bold();
                                t.Span($" {data.Seller.BankAccount}");
                            });
                            right.Item().Text(t =>
                            {
                                t.Span("БИК").Bold();
                                t.Span($" {data.Seller.BIK}");
                            });
                        });
                        row.RelativeItem().Column(right2 =>
                        {
                            right2.Item().Text(t =>
                            {
                                t.Span("Кбе").Bold();
                                t.Span($" {data.Seller.Kbe}");
                            });
                            right2.Item().Text(t =>
                            {
                                t.Span("Код назначения платежа").Bold();
                                t.Span($" {data.Seller.PaymentCode}");
                            });
                        });
                    });

                    // --- Заголовок счета ---
                    col.Item().PaddingTop(15).Text($"Счет на оплату №{data.InvoiceNumber} от {data.Date:yyyy-MM-dd}")
                        .Bold().FontSize(16);

                    // --- Поставщик/Покупатель/Договор ---
                    col.Item().PaddingTop(10).Text(t =>
                    {
                        t.Span("Поставщик: ").Bold();
                        t.Span($"ИНН/БИН: {data.Seller.INN}, {data.Seller.Name}, {data.Seller.Address}");
                    });
                    col.Item().Text(t =>
                    {
                        t.Span("Покупатель: ").Bold();
                        t.Span($"ИНН/БИН: {data.Buyer.INN}, {data.Buyer.Name}");
                    });
                    col.Item().Text(t =>
                    {
                        t.Span("Договор: ").Bold();
                        t.Span($"{data.ContractNumber} от {data.Date:dd.MM.yyyy}");
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
                            header.Cell().Element(CellStyle).Text("№").Bold();
                            header.Cell().Element(CellStyle).Text("Код").Bold();
                            header.Cell().Element(CellStyle).Text("Наименование").Bold();
                            header.Cell().Element(CellStyle).Text("Кол-во").Bold();
                            header.Cell().Element(CellStyle).Text("Ед.").Bold();
                            header.Cell().Element(CellStyle).Text("Цена").Bold();
                            header.Cell().Element(CellStyle).Text("Сумма").Bold();
                        });

                        // Products
                        var i = 1;
                        foreach (var p in data.Products)
                        {
                            table.Cell().Element(CellStyle).Text(i++.ToString());
                            table.Cell().Element(CellStyle).Text(p.Code ?? "");
                            table.Cell().Element(CellStyle).Text(p.Name);
                            table.Cell().Element(CellStyle).Text(p.Quantity.ToString());
                            table.Cell().Element(CellStyle).Text(p.Unit ?? "");
                            table.Cell().Element(CellStyle).Text($"{p.Price:0.00}");
                            table.Cell().Element(CellStyle).Text($"{p.Total:0.00}");
                        }

                        // Итог
                        table.Cell().ColumnSpan(6).AlignRight().Text("Итого:").Bold();
                        table.Cell().Text($"{data.TotalAmount:0.00}").Bold();
                    });

                    // --- Итоги и пропись ---
                    col.Item().PaddingTop(10).Text($"Всего наименований {data.Products.Count}, на сумму {data.TotalAmount:### ### ##0.00} KZT");
                    col.Item().Text($"Всего к оплате: {data.TotalAmountText}");

                    // --- Подпись ---
                    col.Item().PaddingTop(20).Text("Исполнитель ________________________________ //").Italic();
                });
            });
        });

        return document.GeneratePdf();
    }

    private QuestPDF.Infrastructure.IContainer CellStyle(QuestPDF.Infrastructure.IContainer container)
    {
        return container.Border(1).Padding(2).AlignMiddle().AlignCenter();
    }
}