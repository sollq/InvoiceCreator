﻿using Core.Models;
using Infrastructure.Pdf.Interfaces;
using QuestPDF;
using QuestPDF.Drawing;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Infrastructure.Pdf.Generators;

public class KzAktGenerator : IPdfGenerator
{
    public bool CanHandle(DocumentType type)
    {
        return type is DocumentType.KzAkt;
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
                    //col.Item().Row(row =>
                    //{
                    //    row.RelativeItem().Text("BLANC").Bold().FontSize(14);
                    //});
                    col.Item().Row(row =>
                    {
                        row.RelativeItem().Text($"Акт № {data.InvoiceNumber} от «{data.Date:dd}» {RussianMonthGenitive[data.Date.Month]} {data.Date:yyyy} г.")
                            .FontSize(14).Bold().AlignCenter();
                    });

                    col.Item().PaddingTop(10).Text(t =>
                    {
                        t.Span("Заказчик: ").Bold();
                        t.Span($"{data.Buyer.Name}, ИИН/БИН: {data.Buyer.INN}");
                    });

                    col.Item().Text(t =>
                    {
                        t.Span("Исполнитель: ").Bold();
                        t.Span($"{data.Seller.Name}, ИИН/БИН: {data.Seller.INN}");
                    });

                    col.Item().Text(t =>
                    {
                        t.Span("Основание: ").Bold();
                        t.Span($"Оплата услуг по договору \u2116 {data.ContractNumber}");
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
                            //columns.ConstantColumn(50);
                            columns.ConstantColumn(80);
                        });

                        table.Header(header =>
                        {
                            header.Cell().Element(CellStyle).Text("№").Bold().AlignCenter();
                            header.Cell().Element(CellStyle).Text("Наименование работ (услуг)").Bold().AlignCenter();
                            header.Cell().Element(CellStyle).Text("Кол-во").Bold().AlignCenter();
                            header.Cell().Element(CellStyle).Text("Ед. ").Bold().AlignCenter();
                            header.Cell().Element(CellStyle).Text("Цена за единицу").Bold().AlignCenter();
                            //header.Cell().Element(CellStyle).Text("НДС").Bold().AlignCenter();
                            header.Cell().Element(CellStyle).Text("Общая сумма").Bold().AlignLeft();
                        });

                        var i = 1;
                        foreach (var p in data.Products)
                        {
                            table.Cell().Element(CellStyle).Text(i++.ToString()).AlignCenter();
                            table.Cell().Element(CellStyle).Text(p.Name);
                            table.Cell().Element(CellStyle).Text(p.Quantity.ToString()).AlignCenter();
                            table.Cell().Element(CellStyle).Text(p.Unit ?? "").AlignCenter();
                            table.Cell().Element(CellStyle).Text($"{p.Price:### ### ##0.00}").AlignCenter();
                            //table.Cell().Element(CellStyle).Text("Без НДС").Bold().AlignCenter();
                            table.Cell().Element(CellStyle).Text($"{p.Total:### ### ##0.00}").AlignCenter();
                        }

                        // Footer итоги
                        table.Cell().ColumnSpan(5).Element(NoBorderCellStyle).AlignRight().Text($"Итого: ").Bold();
                        table.Cell().Element(CellStyle).AlignRight().Text($"{data.TotalAmount:### ### ##0.00}").Bold();

                        //table.Cell().ColumnSpan(6).Element(NoBorderCellStyle).AlignRight().Text("В том числе НДC: ").Bold();
                        //table.Cell().Element(CellStyle).AlignRight().Text("0 руб.").Bold();
                    });

                    // --- Сумма прописью и финальный текст ---
                    col.Item().PaddingTop(15).Text(
                        $"Всего выполнено работ (оказано услуг) на сумму: {data.TotalAmount:### ### ##0.00} ({data.TotalAmountText})");

                    col.Item().PaddingTop(5).Text(
                        "Все обязательства выполнены исполнителем полностью и в срок, приняты заказчиком в полном объёме и без замечаний. Заказчик претензий к исполнителю не имеет.");

                    // --- Подписи ---
                    col.Item().PaddingTop(40).Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.ConstantColumn(90);   // "Руководитель:"
                            columns.ConstantColumn(150);  // подпись
                            columns.ConstantColumn(110);  // ФИО
                            columns.ConstantColumn(100);  // печать
                        });

                        table.Cell().AlignLeft().AlignMiddle().Text("Исполнитель").Bold();
                        table.Cell().AlignCenter().AlignMiddle().Image("Stamps/kz_handwr.png");
                        table.Cell().AlignLeft().AlignMiddle().Text($"{data.Seller.Name}").Bold();
                        table.Cell().AlignCenter().AlignMiddle().Image("Stamps/kz.png");
                    });
                    col.Item().PaddingTop(40).Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.ConstantColumn(90);   // "Заказчик"
                            columns.ConstantColumn(150);  // подпись
                            columns.ConstantColumn(110);  // ФИО
                            columns.ConstantColumn(110);  // печать
                        });

                        table.Cell().Row(1).Column(1).AlignLeft().AlignMiddle().Text("Заказчик").Bold();

                        table.Cell().Element(container => container
                            .MinHeight(10)
                            .PaddingTop(5)
                            .BorderBottom(1)
                            .BorderColor(Colors.Black));

                        table.Cell().Row(1).Column(3).AlignLeft().AlignMiddle().Text($"{data.Buyer.Name}").Bold();
                        table.Cell().Row(1).Column(4).AlignCenter().AlignMiddle().Text(""); // если будет печать — сюда вставишь
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
    private static readonly Dictionary<int, string> RussianMonthGenitive = new()
    {
        [1] = "января",
        [2] = "февраля",
        [3] = "марта",
        [4] = "апреля",
        [5] = "мая",
        [6] = "июня",
        [7] = "июля",
        [8] = "августа",
        [9] = "сентября",
        [10] = "октября",
        [11] = "ноября",
        [12] = "декабря"
    };
}