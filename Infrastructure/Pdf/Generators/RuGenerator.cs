using Core.Models;
using Infrastructure.Pdf.Interfaces;
using QuestPDF;
using QuestPDF.Drawing;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Infrastructure.Pdf.Generators;

public class RuGenerator : IPdfGenerator
{
    public bool CanHandle(DocumentType type)
    {
        return type is DocumentType.Ru;
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
                        // Логотип + заголовок таблицы
                        row.RelativeItem(3).Column(col1 =>
                        {
                            col1.Item().Text("BLANC").FontSize(18).Bold();

                            col1.Item().PaddingTop(5).Text("Образец заполнения платёжного поручения")
                                .FontSize(10).Bold().AlignCenter();

                            // Таблица реквизитов
                            col1.Item().PaddingTop(5).Table(table =>
                            {
                                table.ColumnsDefinition(columns =>
                                {
                                    columns.ConstantColumn(100);
                                    columns.RelativeColumn();
                                });

                                AddRow("Получатель", "");
                                AddRow("ООО \"НОРДСИС\"", "Сч. № 40702810300100216376");
                                AddRow("Банк получателя", "БИК 044525801");
                                AddRow("ООО\"Бланк банк\"", "Сч. № 30101810465250000801");
                                AddRow("ИНН 5420275654", "КПП 542001001");
                                AddRow("Назначение платежа", $"оплата услуг по договору № НС/{data.ContractNumber}/25");
                                return;

                                void AddRow(string left, string right)
                                {
                                    table.Cell().Border(1).Padding(2).Text(left).FontSize(10);
                                    table.Cell().Border(1).Padding(2).Text(right).FontSize(10);
                                }
                            });
                        });

                        // QR-код
                        row.RelativeItem().AlignRight().AlignBottom().Column(c =>
                        {
                            c.Item().Image("Stamps/qrcode.jpg").FitWidth();
                            c.Item().Text("Отсканируйте\nв приложении банка\nдля оплаты")
                                .FontSize(8).AlignCenter();
                        });
                    });
                    col.Item().Row(row =>
                    {
                        row.RelativeItem().Text($"Счёт на оплату № {data.InvoiceNumber} от «{data.Date:dd}» {RussianMonthGenitive[data.Date.Month]} {data.Date:yyyy} г.")
                            .FontSize(14).Bold().AlignCenter();
                    });

                    col.Item().PaddingTop(10).Text(t =>
                    {
                        t.Span("Поставщик: ").Bold();
                        t.Span($"{data.Buyer.Name}");
                    });

                    col.Item().Text(t =>
                    {
                        t.Span("Покупатель: ").Bold();
                        t.Span($"{data.Seller.Name}, ИНН: {data.Seller.INN}, КПП: {data.Seller.KPP}");
                    });

                    var i = 1;
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

                        foreach (var p in data.Products)
                        {
                            table.Cell().Element(CellStyle).Text(i++.ToString()).AlignCenter();
                            table.Cell().Element(CellStyle).Text(p.Name);
                            table.Cell().Element(CellStyle).Text(p.Quantity.ToString()).AlignCenter();
                            table.Cell().Element(CellStyle).Text(p.Unit ?? "").AlignCenter();
                            table.Cell().Element(CellStyle).Text($"{p.Price:### ### ##0.00}").AlignRight();
                            table.Cell().Element(CellStyle).Text("Без НДС").Bold().AlignCenter();
                            table.Cell().Element(CellStyle).Text($"{p.Total:### ### ##0.00}").AlignRight();
                        }

                        // Footer итоги
                        table.Cell().ColumnSpan(6).Element(NoBorderCellStyle).AlignRight().Text("Итого к оплате: ").Bold();
                        table.Cell().Element(CellStyle).AlignRight().Text($"{data.TotalAmount:### ### ##0.00}").Bold();

                        table.Cell().ColumnSpan(6).Element(NoBorderCellStyle).AlignRight().Text("В том числе НДС: ").Bold();
                        table.Cell().Element(CellStyle).AlignRight().Text("0 руб.").Bold();
                    });

                    // --- Сумма прописью и финальный текст ---
                    col.Item().PaddingTop(15).Text(
                        $"Всего наименований {i}, на сумму {data.TotalAmount:### ### ##0.00} руб. ");

                    //Первую букву - с большой
                    if (data.TotalAmountText != null)
                        col.Item().PaddingTop(5).Text(
                            $"{char.ToUpper(data.TotalAmountText[0])}{data.TotalAmountText[1..]}").Bold();

                    // --- Подписи ---
                    col.Item().PaddingTop(40).Row(row =>
                    {
                        row.RelativeItem().Text($"Руководитель:                                                                  Загороднюк К.Е.").Bold();
                        row.ConstantItem(130).AlignCenter().Image("Stamps/Ru.png");
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