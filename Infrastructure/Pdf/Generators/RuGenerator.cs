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
        return type is DocumentType.InvoiceRu;
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
                            col1.Item().Text("А").FontSize(45).FontColor(Colors.Red.Medium).Bold();

                            col1.Item().PaddingTop(5).Text("Образец заполнения платёжного поручения")
                                .FontSize(10).Bold().AlignCenter();

                            // Таблица реквизитов
                            col1.Item().PaddingTop(5).Table(table =>
                            {
                                table.ColumnsDefinition(columns =>
                                {
                                    columns.ConstantColumn(140);
                                    columns.RelativeColumn();
                                });

                                AddRow("Получатель", $"{data.Seller.Name}");
                                AddRow("Получателя Сч.\u2116", $"{data.Seller.CoreAcc}");
                                AddRow("Банк получателя", $"{data.Seller.BankDetails}");
                                AddRow("БИК банка", $"{data.Seller.BIK}");
                                AddRow("Корр. счёт банка", $"{data.Seller.CoreAcc}");
                                AddRow("ИНН",  $"{data.Seller.INN}");
                                AddRow("КПП", $"{data.Seller.KPP}");
                                AddRow("Назначение платежа", $"Оплата услуг по договору № {data.ContractNumber}");
                                return;

                                void AddRow(string left, string right)
                                {
                                    table.Cell().Element(RequisiteCellStyle).Text(left).FontSize(10).AlignLeft();
                                    table.Cell().Element(RequisiteCellStyle).Text(right).FontSize(10).AlignLeft();
                                }
                            });
                        });
                        // QR-код
                        //row.RelativeItem().AlignRight().AlignBottom().Column(c =>
                        //{
                        //    c.Item().Image("Stamps/qrcode.jpg").FitWidth();
                        //    c.Item().Text("Отсканируйте\nв приложении банка\nдля оплаты")
                        //        .FontSize(8).AlignCenter();
                        //});
                    });
                    col.Item().PaddingTop(10).Row(row =>
                    {
                        row.RelativeItem().Text($"Счёт на оплату № {data.InvoiceNumber} от «{data.Date:dd}» {RussianMonthGenitive[data.Date.Month]} {data.Date:yyyy} г.")
                            .FontSize(14).Bold().AlignCenter();
                    });

                    col.Item().PaddingTop(10).Text(t =>
                    {
                        t.Span("Поставщик: ").Bold();
                        t.Span($"{data.Seller.Name}");
                    });

                    col.Item().Text(t =>
                    {
                        t.Span("Покупатель: ").Bold();
                        var innKppStr = $"{data.Buyer.Name}, ИНН: {data.Buyer.INN}";
                        if (!string.IsNullOrEmpty(data.Buyer.KPP))
                        {
                            innKppStr += $", КПП: {data.Buyer.KPP}";
                        }
                        t.Span(innKppStr);
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
                            columns.ConstantColumn(60);
                            columns.ConstantColumn(60);
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
                            table.Cell().Element(CellStyle).Text($"{p.Price:### ### ##0.00}").AlignCenter();
                            table.Cell().Element(CellStyle).Text("Без НДС").Bold().AlignCenter();
                            table.Cell().Element(CellStyle).Text($"{p.Total:### ### ##0.00}").AlignCenter();
                        }

                        // Footer итоги
                        table.Cell().ColumnSpan(6).Element(NoBorderCellStyle).AlignRight().Text("Итого к оплате: ").Bold();
                        table.Cell().Element(CellStyle).AlignRight().Text($"{data.TotalAmount:### ### ##0.00}").Bold();

                        table.Cell().ColumnSpan(6).Element(NoBorderCellStyle).AlignRight().Text("В том числе НДС: ").Bold();
                        table.Cell().Element(CellStyle).AlignRight().Text("0 руб.").Bold();
                    });

                    // --- Сумма прописью и финальный текст ---
                    col.Item().PaddingTop(15).Text(
                        $"Всего наименований {i-=1}, на сумму {data.TotalAmount:### ### ##0.00} руб. ");

                    //Первую букву - с большой
                    if (data.TotalAmountText != null)
                        col.Item().PaddingTop(5).Text(
                            $"{char.ToUpper(data.TotalAmountText[0])}{data.TotalAmountText[1..]}").Bold();

                    // --- Подписи ---
                    col.Item().PaddingTop(40).Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.ConstantColumn(90);   // "Руководитель:"
                            columns.ConstantColumn(150);  // подпись
                            columns.ConstantColumn(110);  // ФИО
                            columns.ConstantColumn(150);  // печать
                        });

                        table.Cell().AlignLeft().AlignMiddle().Text("Руководитель:").Bold();
                        table.Cell().AlignCenter().AlignMiddle().Image("Stamps/handwr.png");
                        table.Cell().AlignLeft().AlignMiddle().Text("Загороднюк К.Е.").Bold();
                        table.Cell().AlignCenter().AlignMiddle().Image("Stamps/ru.png");
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
    private IContainer RequisiteCellStyle(IContainer container)
    {
        return container.Border(1).Padding(2).AlignMiddle().AlignLeft();
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