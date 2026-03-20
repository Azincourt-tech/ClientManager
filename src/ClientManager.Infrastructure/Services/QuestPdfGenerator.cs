using ClientManager.Domain.Core.Interfaces.Services;
using Microsoft.Extensions.Logging;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using QuestPDF.Previewer;

namespace ClientManager.Infrastructure.Services;

public class QuestPdfGenerator(ILogger<QuestPdfGenerator> logger) : IPdfGenerator
{
    static QuestPdfGenerator()
    {
        // QuestPDF Community License
        QuestPDF.Settings.License = LicenseType.Community;
    }

    public async Task<byte[]> GenerateWelcomeKitAsync(Guid customerId, string name)
    {
        logger.LogInformation("Generating real PDF Welcome Kit for customer: {Name} ({CustomerId})", name, customerId);

        return await Task.Run(() =>
        {
            var document = QuestPDF.Fluent.Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(12).FontFamily(Fonts.Verdana));

                    page.Header().Row(row =>
                    {
                        row.RelativeItem().Column(col =>
                        {
                            col.Item().Text("ClientManager").FontSize(24).SemiBold().FontColor(Colors.Blue.Medium);
                            col.Item().Text("Seu sucesso é nossa prioridade").FontSize(10).Italic();
                        });

                        row.ConstantItem(100).Height(50).Placeholder(); // Espaço para logo
                    });

                    page.Content().PaddingVertical(1, Unit.Centimetre).Column(x =>
                    {
                        x.Spacing(20);

                        x.Item().Text($"Olá, {name}!").FontSize(18).SemiBold();

                        x.Item().Text("É com grande satisfação que lhe damos as boas-vindas ao ClientManager. Estamos entusiasmados por ter você conosco.");

                        x.Item().PaddingTop(10).Text("Seu Kit de Integração").FontSize(14).SemiBold().Underline();

                        x.Item().Text(t =>
                        {
                            t.Span("ID do Cliente: ").SemiBold();
                            t.Span(customerId.ToString());
                        });

                        x.Item().Text("Próximos passos:");
                        x.Item().PaddingLeft(10).Column(list =>
                        {
                            list.Item().Text("• Verifique seus documentos na plataforma.");
                            list.Item().Text("• Complete seu perfil profissional.");
                            list.Item().Text("• Explore nossas ferramentas de gestão.");
                        });

                        x.Item().PaddingTop(20).Background(Colors.Grey.Lighten4).Padding(10).Text(t =>
                        {
                            t.Span("Aviso: ").SemiBold();
                            t.Span("Este é um documento gerado automaticamente. Guarde este arquivo para futuras referências.");
                        });
                    });

                    page.Footer().AlignCenter().Text(x =>
                    {
                        x.Span("Página ");
                        x.CurrentPageNumber();
                    });
                });
            });

            return document.GeneratePdf();
        });
    }
}
