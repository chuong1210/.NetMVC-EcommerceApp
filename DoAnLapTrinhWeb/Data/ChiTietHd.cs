using System;
using System.Collections.Generic;
using DoAnLapTrinhWeb.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.AspNetCore.Http.HttpResults;

namespace DoAnLapTrinhWeb.Data;

public partial class ChiTietHd
{
    public int MaCt { get; set; }

    public int MaHd { get; set; }

    public int MaHh { get; set; }

    public double DonGia { get; set; }

    public int SoLuong { get; set; }

    public double GiamGia { get; set; }

    public virtual HoaDon MaHdNavigation { get; set; } = null!;

    public virtual HangHoa MaHhNavigation { get; set; } = null!;
}


public static class ChuDeEndpoints
{
	public static void MapChuDeEndpoints (this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/ChuDe").WithTags(nameof(ChuDe));

        group.MapGet("/", async (ShopApp2024Context db) =>
        {
            return await db.ChuDes.ToListAsync();
        })
        .WithName("GetAllChuDes")
        .WithOpenApi();

        group.MapGet("/{id}", async Task<Results<Ok<ChuDe>, NotFound>> (int macd, ShopApp2024Context db) =>
        {
            return await db.ChuDes.AsNoTracking()
                .FirstOrDefaultAsync(model => model.MaCd == macd)
                is ChuDe model
                    ? TypedResults.Ok(model)
                    : TypedResults.NotFound();
        })
        .WithName("GetChuDeById")
        .WithOpenApi();

        group.MapPut("/{id}", async Task<Results<Ok, NotFound>> (int macd, ChuDe chuDe, ShopApp2024Context db) =>
        {
            var affected = await db.ChuDes
                .Where(model => model.MaCd == macd)
                .ExecuteUpdateAsync(setters => setters
                  .SetProperty(m => m.MaCd, chuDe.MaCd)
                  .SetProperty(m => m.TenCd, chuDe.TenCd)
                  .SetProperty(m => m.MaNv, chuDe.MaNv)
                  );
            return affected == 1 ? TypedResults.Ok() : TypedResults.NotFound();
        })
        .WithName("UpdateChuDe")
        .WithOpenApi();

        group.MapPost("/", async (ChuDe chuDe, ShopApp2024Context db) =>
        {
            db.ChuDes.Add(chuDe);
            await db.SaveChangesAsync();
            return TypedResults.Created($"/api/ChuDe/{chuDe.MaCd}",chuDe);
        })
        .WithName("CreateChuDe")
        .WithOpenApi();

        group.MapDelete("/{id}", async Task<Results<Ok, NotFound>> (int macd, ShopApp2024Context db) =>
        {
            var affected = await db.ChuDes
                .Where(model => model.MaCd == macd)
                .ExecuteDeleteAsync();
            return affected == 1 ? TypedResults.Ok() : TypedResults.NotFound();
        })
        .WithName("DeleteChuDe")
        .WithOpenApi();
    }
}