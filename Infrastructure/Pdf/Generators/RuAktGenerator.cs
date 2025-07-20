using Core.Models;
using Infrastructure.Pdf.Interfaces;
using QuestPDF;
using QuestPDF.Drawing;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Infrastructure.Pdf.Generators;

public class RuAktGenerator : IPdfGenerator
{
    public bool CanHandle(DocumentType type)
    {
        return type is DocumentType.RuAkt;
    }
    public byte[] Generate(DocumentData data)
    {
        Settings.License = LicenseType.Community;
        FontManager.RegisterFont(File.OpenRead("Fonts/times.ttf"));
        FontManager.RegisterFont(File.OpenRead("Fonts/times_bold.ttf"));

        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(40);
                page.DefaultTextStyle(x => x.FontFamily("Italic").FontSize(10));
                page.Content().Column(col =>
                {
                    // --- Логотип и заголовок ---
                    col.Item().Row(row =>
                    {
                        row.RelativeItem().Text("BLANC").Bold().FontSize(14);
                    });
                    col.Item().Row(row =>
                    {
                        row.RelativeItem().Text($"Акт № {data.InvoiceNumber} от «{data.Date:dd}» {data.Date:MMMM yyyy} г.")
                            .FontSize(14).Bold().AlignCenter();
                    });

                    col.Item().PaddingTop(10).Text(t =>
                    {
                        t.Span("Заказчик: ").Bold();
                        t.Span($"{data.Buyer.Name}, ИНН: {data.Buyer.INN}, КПП: {data.Buyer.KPP}");
                    });

                    col.Item().Text(t =>
                    {
                        t.Span("Исполнитель: ").Bold();
                        t.Span($"{data.Seller.Name}, ИНН: {data.Seller.INN}, КПП: {data.Seller.KPP}, ОГРН: {data.Seller.OGRN}");
                    });

                    col.Item().Text(t =>
                    {
                        t.Span("Основание: ").Bold();
                        t.Span($"Оплата услуг по договору \u2116 НС/{data.ContractNumber}/24 {data.Date:dd MMMM yyyy} г.");
                    });

                    // --- Таблица работ ---
                    col.Item().PaddingTop(15).Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.ConstantColumn(25);
                            columns.RelativeColumn(4);
                            columns.ConstantColumn(40);
                            columns.ConstantColumn(45);
                            columns.ConstantColumn(60);
                            columns.ConstantColumn(50);
                            columns.ConstantColumn(50);
                        });

                        table.Header(header =>
                        {
                            header.Cell().Element(CellStyle).Text("№").Bold().AlignCenter();
                            header.Cell().Element(CellStyle).Text("Наименование работ (услуг)").Bold().AlignCenter();
                            header.Cell().Element(CellStyle).Text("Кол-во").Bold().AlignCenter();
                            header.Cell().Element(CellStyle).Text("Ед. ").Bold().AlignCenter();
                            header.Cell().Element(CellStyle).Text("Цена за единицу").Bold().AlignCenter();
                            header.Cell().Element(CellStyle).Text("НДС").Bold().AlignCenter();
                            header.Cell().Element(CellStyle).Text("Общая сумма").Bold().AlignLeft();
                        });

                        var i = 1;
                        foreach (var p in data.Products)
                        {
                            table.Cell().Element(CellStyle).Text(i++.ToString()).AlignCenter();
                            table.Cell().Element(CellStyle).Text(p.Name);
                            table.Cell().Element(CellStyle).Text(p.Quantity.ToString()).AlignCenter();
                            table.Cell().Element(CellStyle).Text(p.Unit ?? "").AlignCenter();
                            table.Cell().Element(CellStyle).Text("Без НДС").Bold().AlignCenter();
                            table.Cell().Element(CellStyle).Text($"{p.Price:### ### ##0.00} руб.").AlignRight();
                            table.Cell().Element(CellStyle).Text($"{p.Total:### ### ##0.00} руб.").AlignRight();
                        }

                        // Footer итоги
                        table.Cell().ColumnSpan(6).Element(NoBorderCellStyle).AlignRight().Text("Итого:").Bold();
                        table.Cell().Element(CellStyle).AlignRight().Text($"{data.TotalAmount:### ### ##0.00}").Bold();

                        table.Cell().ColumnSpan(6).Element(NoBorderCellStyle).AlignRight().Text("В том числе НДС:").Bold();
                        table.Cell().Element(CellStyle).AlignRight().Text("0 руб.").Bold();
                    });

                    // --- Сумма прописью и финальный текст ---
                    col.Item().PaddingTop(15).Text(
                        $"Всего выполнено работ (оказано услуг) на сумму: {data.TotalAmount:### ### ##0.00} руб. ({data.TotalAmountText})");

                    col.Item().PaddingTop(5).Text(
                        "Все обязательства выполнены исполнителем полностью и в срок, приняты заказчиком в полном объёме и без замечаний. Заказчик претензий к исполнителю не имеет.");

                    // --- Подписи ---
                    col.Item().PaddingTop(40).Row(row =>
                    {
                        row.RelativeItem().Text($"Исполнитель:                                                               // {data.Seller.Name}").Bold();
                        row.ConstantItem(130).AlignCenter().Image("Stamps/Ru.png");
                    });
                    col.Item().PaddingTop(10).Row(row =>
                    {
                        row.RelativeItem().Text($"Заказчик:                                                                  // {data.Buyer.Name}").Bold();
                    });
                });
            });
        });

        return document.GeneratePdf();
    }

    private IContainer CellStyle(IContainer container)
    {
        return container.Border(1).Padding(2).AlignMiddle().AlignCenter();
    }
    private IContainer NoBorderCellStyle(IContainer container)
    {
        return container.Border(0).Padding(1).AlignMiddle().AlignCenter();
    }
}