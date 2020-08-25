﻿using Fur.SpecificationDocument;

namespace Microsoft.AspNetCore.Builder
{
    /// <summary>
    /// 规范化文档中间件拓展
    /// </summary>
    public static class SpecificationDocumentApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseSpecificationDocuments(this IApplicationBuilder app)
        {
            // 配置 Swagger 全局参数
            app.UseSwagger(options => SpecificationDocumentBuilder.Build(options));

            // 配置 Swagger UI 参数
            app.UseSwaggerUI(options => SpecificationDocumentBuilder.BuildUI(options));

            return app;
        }
    }
}