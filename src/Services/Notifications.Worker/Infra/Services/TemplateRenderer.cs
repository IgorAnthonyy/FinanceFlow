using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading.Tasks;
using Notifications.Worker.Domain.Interfaces;
using RazorEngineCore;

namespace Notifications.Worker.Infra.Services;

public class TemplateRenderer : ITemplateRenderer
{
    private readonly ConcurrentDictionary<string, IRazorEngineCompiledTemplate> _cache = new();
    private readonly IRazorEngine _razorEngine = new RazorEngine();

    public async Task<string> RenderAsync<T>(string templateName, T model)
    {
        var templatePath = Path.Combine(AppContext.BaseDirectory, "Templates", $"{templateName}.cshtml");
        if (!File.Exists(templatePath))
        {
            throw new FileNotFoundException($"O template '{templateName}' não foi encontrado no caminho: {templatePath}");
        }

        var compiledTemplate = _cache.GetOrAdd(templateName, name =>
        {
            var templateContent = File.ReadAllText(templatePath);
            return _razorEngine.Compile(templateContent);
        });

        var result = compiledTemplate.Run(model);
        return await Task.FromResult(result);
    }
}
