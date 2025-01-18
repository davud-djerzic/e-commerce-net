using QuestPDF.Infrastructure;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Companion;
using QuestPDF.Previewer;
using Ecommerce.Models;
using Ecommerce.Context;
using System;
using Microsoft.EntityFrameworkCore;
using Ecommerce.Services.ServiceInterfaces;
using Ecommerce.Exceptions;
using Ecommerce.Models.ResponseDto;
using Ecommerce.Caches.Interfaces;

namespace Ecommerce.Services
{
    public class GeneratePdfService(ApplicationDbContext _context, IOrderCache orderCache) : IGeneratePdfService
    {
        public async Task GeneratePdfFile(List<OrderProduct> orderProduct)
        {
            try
            {
                string pdfPath = Path.Combine(Directory.GetCurrentDirectory() + "/pdfs/", $"Order {orderProduct[0].OrderId}.pdf");

                Console.WriteLine("Starting PDF generation...");

                QuestPDF.Settings.License = LicenseType.Community;

                //decimal totalPrice = 0;

                // Preuzmi sve podatke o proizvodima iz Redis-a unapred
                List<ProductDataOrder> productDataList = new List<ProductDataOrder>();
                foreach (var item in orderProduct)
                {
                    ProductDataOrder productData = await orderCache.GetOrderProduct(item.OrderId, item.ProductId);
                    if (productData == null) throw new Exception($"Product data for ProductId {item.ProductId} not found in Redis.");
                    productDataList.Add(productData);
                }

                Document.Create(container =>
                {
                    container
                        .Page(page =>
                        {
                            page.Margin(50);

                            page.Margin(50);

                            var titleStyle = TextStyle.Default.FontSize(20).SemiBold().FontColor(Colors.Blue.Medium);

                            page.Header().Row(row =>
                            {
                                row.RelativeItem().Column(column =>
                                {
                                    column.Item().Text($"Invoice {orderProduct[0].OrderId}").Style(titleStyle);

                                    column.Item().Text(text =>
                                    {
                                        text.Span("Issue date: ").SemiBold();
                                        text.Span($"{DateTime.Now.ToShortDateString()}");
                                    });
                                });

                                row.ConstantItem(100).Height(50).Placeholder();
                            });

                            page.Content().PaddingVertical(40).Column(async column =>
                            {
                                column.Spacing(10); 

                                column.Item().Row(async row =>
                                {
                                   
                                    row.RelativeItem().Column(senderColumn =>
                                    {
                                        senderColumn.Item().Text("From:").SemiBold().FontSize(14);
                                        senderColumn.Item().BorderBottom(1).PaddingBottom(5);
                                        senderColumn.Item().Text("Email: djerzicd831@gmail.com");
                                        senderColumn.Item().Text("Comapany: Ecommerce");
                                    });

                                    row.ConstantItem(50);


                                    Order order = _context.Orders.FirstOrDefault(o => o.Id == orderProduct[0].OrderId);
                                    if (order == null) throw new NotFoundException("Order not found");

                                    User user = _context.Users.FirstOrDefault(u => u.Id == order.UserId);
                                    if (user == null) throw new NotFoundException("User not found");

                                    row.RelativeItem().Column(receiverColumn =>
                                    { 
                                        receiverColumn.Item().Text("For:").SemiBold().FontSize(14);
                                        receiverColumn.Item().BorderBottom(1).PaddingBottom(5);
                                        receiverColumn.Item().Text($"Email: {user.Email}");
                                        receiverColumn.Item().Text($"Name: {user.FirstName}");
                                        receiverColumn.Item().Text($"Surname: {user.LastName}");
                                    });
                                });

                                column.Item().Table(async table =>
                                {

                                    table.ColumnsDefinition(columns =>
                                    {
                                        columns.ConstantColumn(25);
                                        columns.RelativeColumn(3);
                                        columns.RelativeColumn();
                                        columns.RelativeColumn();
                                        columns.RelativeColumn();
                                    });

                                    table.Header(header =>
                                    {
                                        header.Cell().Element(CellStyle).Text("#");
                                        header.Cell().Element(CellStyle).Text("Product");
                                        header.Cell().Element(CellStyle).AlignRight().Text("Unit price");
                                        header.Cell().Element(CellStyle).AlignRight().Text("Quantity");
                                        header.Cell().Element(CellStyle).AlignRight().Text("Total");

                                        static IContainer CellStyle(IContainer container)
                                        {
                                            return container.DefaultTextStyle(x => x.SemiBold()).PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Black);
                                        }
                                    });

                                    decimal price = 0;
                                    
                                    for (int i = 0; i < orderProduct.Count; i++)
                                    {
                                        Product product = _context.Products.Find(orderProduct[i].ProductId);
                                        if (product == null) throw new NotFoundException("Product not found");
                                      
                                        price = productDataList[i].Price * orderProduct[i].Quantity;
                                        //price = product.Price * orderProduct[i].Quantity;
                                        //totalPrice += price;

                                        table.Cell().Element(CellStyle).Text($"{i + 1}");
                                        table.Cell().Element(CellStyle).Text(productDataList[i].Name);
                                        table.Cell().Element(CellStyle).AlignRight().Text(productDataList[i].Price.ToString("C"));
                                        table.Cell().Element(CellStyle).AlignRight().Text(productDataList[i].Quantity.ToString());
                                        table.Cell().Element(CellStyle).AlignRight().Text(price.ToString("C"));

                                        static IContainer CellStyle(IContainer container)
                                        {
                                            return container.BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingVertical(5);
                                        }
                                    }
                                });

                                decimal totalPrice = await orderCache.GetTotalPrice(orderProduct[0].OrderId);
                                column.Item().AlignRight().Text($"Grand total: {totalPrice:C}").FontSize(16).Bold();
                            }); 

                            page.Footer().AlignCenter().Text(x =>
                            {
                                x.CurrentPageNumber();
                                x.Span(" / ");
                                x.TotalPages();
                            });
                        });
                }).GeneratePdf(pdfPath);

                Console.WriteLine("PDF generation completed.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error generating PDF: {ex.Message}");
            }
        }
    }
}


