using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using CustomApplication.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Builder;

namespace CustomApplication
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Чтение строки подключения из appsettings.json
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

            // Регистрация сервисов
            builder.Services.AddDbContext<CustomContext>(options =>
                options.UseSqlServer(connectionString));

            // Регистрация кэширования и сессий
            builder.Services.AddMemoryCache();
            builder.Services.AddScoped<CachedDataService>();

            // Регистрация сессий
            builder.Services.AddDistributedMemoryCache();
            builder.Services.AddSession(options =>
            {
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            var app = builder.Build();

            app.UseSession();

            app.Use(async (context, next) =>
            {
                if (context.Request.Path == "/")
                {
                    context.Response.ContentType = "text/html; charset=utf-8";
                    string strResponse = "<HTML><HEAD><TITLE>Главная страница</TITLE></HEAD>" +
                    "<META http-equiv='Content-Type' content='text/html; charset=utf-8'/>" +
                    "<BODY>";
                    strResponse += "<BR><A href='/table'>Таблицы</A>";
                    strResponse += "<BR><A href='/info'>Информация</A>";
                    strResponse += "<BR><A href='/searchform1'>SearchForm1</A>";
                    strResponse += "<BR><A href='/searchform2'>SearchForm2</A>";
                    strResponse += "</BODY></HTML>";
                    await context.Response.WriteAsync(strResponse);
                    return;
                }
                await next.Invoke();
            });

            app.Map("/searchform1", appBuilder =>
            {
                appBuilder.Run(async context =>
                {
                    context.Response.ContentType = "text/html; charset=utf-8";
                    var dbContext = context.RequestServices.GetService<CustomContext>();

                    if (context.Request.Method == "GET")
                    {
                        var documentNumber = context.Request.Cookies["DocumentNumber"] ?? "";
                        var selectedAgentId = context.Request.Cookies["AgentId"] ?? "";
                        var selectedGoodIds = context.Request.Cookies["GoodIds"] ?? "";

                        var agents = await dbContext.Agents.ToListAsync();
                        var goods = await dbContext.Goods.ToListAsync();

                        var html = "<!DOCTYPE html><html><head><meta charset='UTF-8'><title>Search Form 1</title></head><body>";
                        html += "<h1>Search Fees (Cookie)</h1>";
                        html += "<form method='post'>";
                        html += "<label for='DocumentNumber'>Document Number:</label><br/>";
                        html += $"<input type='text' id='DocumentNumber' name='DocumentNumber' value='{documentNumber}' /><br/><br/>";

                        html += "<label for='AgentId'>Agent:</label><br/>";
                        html += "<select id='AgentId' name='AgentId'>";
                        foreach (var agent in agents)
                        {
                            var selected = agent.AgentId.ToString() == selectedAgentId ? "selected" : "";
                            html += $"<option value='{agent.AgentId}' {selected}>{agent.FullName}</option>";
                        }
                        html += "</select><br/><br/>";

                        html += "<label for='GoodIds'>Goods:</label><br/>";
                        html += "<select id='GoodIds' name='GoodIds' multiple size='3'>";
                        var selectedGoodIdsArray = selectedGoodIds.Split(',');

                        foreach (var good in goods)
                        {
                            var selected = selectedGoodIdsArray.Contains(good.GoodId.ToString()) ? "selected" : "";
                            html += $"<option value='{good.GoodId}' {selected}>{good.Name}</option>";
                        }
                        html += "</select><br/><br/>";

                        html += "<button type='submit'>Search</button>";
                        html += "</form>";
                        html += "</body></html>";

                        await context.Response.WriteAsync(html);
                    }
                    else if (context.Request.Method == "POST")
                    {
                        var formData = await context.Request.ReadFormAsync();

                        var documentNumber = formData["DocumentNumber"];
                        var agentId = formData["AgentId"];
                        var goodIds = formData["GoodIds"];

                        context.Response.Cookies.Append("DocumentNumber", documentNumber);
                        context.Response.Cookies.Append("AgentId", agentId);
                        context.Response.Cookies.Append("GoodIds", string.Join(",", goodIds));

                        var query = dbContext.Fees.Include(c => c.Agent).Include(c => c.Good).AsQueryable();

                        if (!string.IsNullOrEmpty(documentNumber))
                        {
                            query = query.Where(c => c.DocumentNumber.Contains(documentNumber));
                        }

                        if (int.TryParse(agentId, out int agentIdValue))
                        {
                            query = query.Where(c => c.AgentId == agentIdValue);
                        }

                        if (goodIds.Count > 0)
                        {
                            var goodIdValues = goodIds.Select(id => int.Parse(id)).ToList();
                            query = query.Where(c => goodIdValues.Contains(c.GoodId));
                        }

                        var results = await query.ToListAsync();

                        var html = "<!DOCTYPE html><html><head><meta charset='UTF-8'><title>Search Results</title></head><body>";
                        html += "<h1>Search Results</h1>";

                        if (results.Count > 0)
                        {
                            html += "<table border='1' style='border-collapse:collapse'>";
                            html += "<tr><th>ID</th><th>Document Number</th><th>Agent</th><th>Good</th></tr>";
                            foreach (var fee in results)
                            {
                                html += "<tr>";
                                html += $"<td>{fee.FeeId}</td>";
                                html += $"<td>{fee.DocumentNumber}</td>";
                                html += $"<td>{fee.Agent?.FullName}</td>";
                                html += $"<td>{fee.Good?.Name}</td>";
                                html += "</tr>";
                            }
                            html += "</table>";
                        }
                        else
                        {
                            html += "<p>No results found.</p>";
                        }

                        html += "<br/><a href='/searchform1'>Back to Search</a>";
                        html += "</body></html>";

                        await context.Response.WriteAsync(html);
                    }
                });
            });

            // Обработка пути "/searchform2" с использованием сессии
            app.Map("/searchform2", appBuilder =>
            {
                appBuilder.Run(async context =>
                {
                    context.Response.ContentType = "text/html; charset=utf-8";
                    var dbContext = context.RequestServices.GetService<CustomContext>();

                    if (context.Request.Method == "GET")
                    {
                        var documentNumber = context.Session.GetString("DocumentNumber") ?? "";
                        var selectedAgentId = context.Session.GetString("AgentId") ?? "";
                        var selectedGoodIds = context.Session.GetString("GoodIds") ?? "";

                        var agents = await dbContext.Agents.ToListAsync();
                        var goods = await dbContext.Goods.ToListAsync();

                        var html = "<!DOCTYPE html><html><head><meta charset='UTF-8'><title>Search Form 2</title></head><body>";
                        html += "<h1>Search Fees (Session)</h1>";
                        html += "<form method='post'>";
                        html += "<label for='DocumentNumber'>Document Number:</label><br/>";
                        html += $"<input type='text' id='DocumentNumber' name='DocumentNumber' value='{documentNumber}' /><br/><br/>";

                        html += "<label for='AgentId'>Agent:</label><br/>";
                        html += "<select id='AgentId' name='AgentId'>";
                        foreach (var agent in agents)
                        {
                            var selected = agent.AgentId.ToString() == selectedAgentId ? "selected" : "";
                            html += $"<option value='{agent.AgentId}' {selected}>{agent.FullName}</option>";
                        }
                        html += "</select><br/><br/>";

                        html += "<label for='GoodIds'>Goods:</label><br/>";
                        html += "<select id='GoodIds' name='GoodIds' multiple size='3'>";
                        var selectedGoodIdsArray = selectedGoodIds.Split(',');

                        foreach (var good in goods)
                        {
                            var selected = selectedGoodIdsArray.Contains(good.GoodId.ToString()) ? "selected" : "";
                            html += $"<option value='{good.GoodId}' {selected}>{good.Name}</option>";
                        }
                        html += "</select><br/><br/>";

                        html += "<button type='submit'>Search</button>";
                        html += "</form>";
                        html += "</body></html>";

                        await context.Response.WriteAsync(html);
                    }
                    else if (context.Request.Method == "POST")
                    {
                        var formData = await context.Request.ReadFormAsync();

                        var documentNumber = formData["DocumentNumber"];
                        var agentId = formData["AgentId"];
                        var goodIds = formData["GoodIds"];

                        context.Session.SetString("DocumentNumber", documentNumber);
                        context.Session.SetString("AgentId", agentId);
                        context.Session.SetString("GoodIds", string.Join(",", goodIds));

                        var query = dbContext.Fees.Include(c => c.Agent).Include(c => c.Good).AsQueryable();

                        if (!string.IsNullOrEmpty(documentNumber))
                        {
                            query = query.Where(c => c.DocumentNumber.Contains(documentNumber));
                        }

                        if (int.TryParse(agentId, out int agentIdValue))
                        {
                            query = query.Where(c => c.AgentId == agentIdValue);
                        }

                        if (goodIds.Count > 0)
                        {
                            var goodIdValues = goodIds.Select(id => int.Parse(id)).ToList();
                            query = query.Where(c => goodIdValues.Contains(c.GoodId));
                        }

                        var results = await query.ToListAsync();

                        // Формирование HTML для вывода результатов
                        var html = "<!DOCTYPE html><html><head><meta charset='UTF-8'><title>Search Results</title></head><body>";
                        html += "<h1>Search Results</h1>";

                        if (results.Count > 0)
                        {
                            html += "<table border='1' style='border-collapse:collapse'>";
                            html += "<tr><th>ID</th><th>Document Number</th><th>Agent</th><th>Good</th></tr>";
                            foreach (var fee in results)
                            {
                                html += "<tr>";
                                html += $"<td>{fee.FeeId}</td>";
                                html += $"<td>{fee.DocumentNumber}</td>";
                                html += $"<td>{fee.Agent?.FullName}</td>";
                                html += $"<td>{fee.Good?.Name}</td>";
                                html += "</tr>";
                            }
                            html += "</table>";
                        }
                        else
                        {
                            html += "<p>No results found.</p>";
                        }

                        html += "<br/><a href='/searchform2'>Back to Search</a>";
                        html += "</body></html>";

                        await context.Response.WriteAsync(html);
                    }
                });
            });


            app.Use(async (context, next) =>
            {
                if (context.Request.Path == "/table")
                {
                    context.Response.ContentType = "text/html; charset=utf-8";
                    string strResponse = "<HTML><HEAD><TITLE>Таблицы</TITLE></HEAD>" +
                     "<META http-equiv='Content-Type' content='text/html; charset=utf-8'/>" +
                     "<BODY>";
                    strResponse += "<BR><A href='/table/Agents'>Agents</A>";
                    strResponse += "<BR><A href='/table/Fees'>Fees</A>";
                    strResponse += "<BR><A href='/table/Goods'>Goods</A>";
                    strResponse += "<BR><A href='/table/GoodTypes'>GoodTypes</A>";
                    strResponse += "<BR><A href='/table/Warehouses'>Warehouses</A>";
                    strResponse += "</BODY></HTML>";
                    await context.Response.WriteAsync(strResponse);
                    return;
                }
                await next.Invoke();
            });

            app.Use(async (context, next) =>
            {
                if (context.Request.Path.StartsWithSegments("/table", out var remainingPath) && remainingPath.HasValue && remainingPath.Value.StartsWith("/"))
                {
                    context.Response.ContentType = "text/html; charset=utf-8"; // Установка Content-Type
                    var tableName = remainingPath.Value.Substring(1); // Убираем начальный слэш

                    var cachedService = context.RequestServices.GetService<CachedDataService>();

                    if (tableName == "Agents")
                    {
                        var list = cachedService.GetAgents();
                        await RenderTable(context, list);
                    }
                    else if (tableName == "Fees")
                    {
                        var list = cachedService.GetFees();
                        await RenderTable(context, list);
                    }
                    else if (tableName == "Goods")
                    {
                        var list = cachedService.GetGoods();
                        await RenderTable(context, list);
                    }
                    else if (tableName == "GoodTypes")
                    {
                        var list = cachedService.GetGoodTypes();
                        await RenderTable(context, list);
                    }
                    else if (tableName == "Warehouses")
                    {
                        var list = cachedService.GetWarehouses();
                        await RenderTable(context, list);
                    }
                    else
                    {
                        // Если таблица не найдена, возвращаем 404
                        context.Response.StatusCode = 404;
                        await context.Response.WriteAsync("Таблица не найдена");
                    }

                    return; // Завершаем обработку запроса
                }
                await next.Invoke();
            });

            app.Use(async (context, next) =>
            {
                if (context.Request.Path == "/info")
                {
                    context.Response.ContentType = "text/html; charset=utf-8";
                    string strResponse = "<HTML><HEAD><TITLE>Информация</TITLE></HEAD>" +
                    "<META http-equiv='Content-Type' content='text/html; charset=utf-8'/>" +
                    "<BODY><H1>Информация:</H1>";
                    strResponse += "<BR> Сервер: " + context.Request.Host;
                    strResponse += "<BR> Путь: " + context.Request.Path;
                    strResponse += "<BR> Протокол: " + context.Request.Protocol;
                    strResponse += "<BR><A href='/'>Главная</A></BODY></HTML>";
                    await context.Response.WriteAsync(strResponse);
                    return;
                }
                await next.Invoke();
            });

            async Task RenderTable<T>(HttpContext context, IEnumerable<T> data)
            {
                context.Response.ContentType = "text/html; charset=utf-8";
                var html = "<table border='1' style='border-collapse:collapse'>";

                var type = typeof(T);

                // Генерация заголовков таблицы на основе свойств типа
                html += "<tr>";
                foreach (var prop in type.GetProperties())
                {
                    // Пропускаем свойства, которые являются объектами других классов или коллекциями
                    if (!IsSimpleType(prop.PropertyType))
                    {
                        continue;
                    }

                    html += $"<th>{prop.Name}</th>";
                }
                html += "</tr>";

                foreach (var item in data)
                {
                    html += "<tr>";
                    foreach (var prop in type.GetProperties())
                    {
                        if (!IsSimpleType(prop.PropertyType))
                        {
                            continue;
                        }

                        var value = prop.GetValue(item);

                        if (value is DateTime dateValue)
                        {
                            html += $"<td>{dateValue.ToString("dd.MM.yyyy")}</td>";
                        }
                        else
                        {
                            html += $"<td>{value}</td>";
                        }
                    }
                    html += "</tr>";
                }

                html += "</table>";
                await context.Response.WriteAsync(html);
            }

            bool IsSimpleType(Type type)
            {
                // Примитивные типы и типы, которые считаются простыми (string, DateTime и т.д.)
                return type.IsPrimitive ||
                       type.IsValueType ||
                       type == typeof(string) ||
                       type == typeof(DateTime) ||
                       type == typeof(decimal);
            }

            app.Run();
        }
    }
}