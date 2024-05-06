using pruebaCortaIDWM;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<DataContext>(opt => opt.UseInMemoryDatabase("chairs"));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
var app = builder.Build();

var chairs = app.MapGroup("api/chair");

chairs.MapGet("/",GetChairs);
chairs.MapPost("/",AddChair);
chairs.MapGet("/{id}",GetChairByName);
chairs.MapPut("/{id}",UpdateChair);
chairs.MapDelete("/{id}",DeleteChair);
chairs.MapPut("/{id}/stock", IncChairStock);
chairs.MapPost("/chair/purchase", SellingChair);

app.Run();

static IResult GetChairs(Datacontext db) {
    var result = await db.Chair.ToListAsync();
    return TypedResults.Ok(result);
}

static IResult AddChair(Datacontext db, Chair chair){
    db.Chair.Add(chair);
    await db.SaveChangesAsync();

    return TypedResults.Created($"/api/chair/{chair.id}",chair);
}

static IResult GetChairByName(DataContext db, string name){
    var result = await db.Chair.Where(c => c.Name == name).toListAsync();
    if(result != null){
        return TypedResults.Ok(result);
    }
    return TypedResults.NotFound();
}

static IResult UpdateChair(DataContext db, Chair chair, int id){
    var oldchair = await db.Chair.FindAsync(id);
    if(oldchair is null) return TypedResults.NotFound();

    oldchair.Name = chair.Name;
    oldchair.Type = chair.Type;
    oldchair.Color = chair.Color;


    await db.SaveChangesAsync();

    return TypedResults.NoContent();
}

static IResult DeleteChair(DataContext db, int i){
    var result = await db.Chair.FindAsync(id);
    if(result is null) return TypedResults.NotFound();

    db.Chair.Remove(result);
    await db.SaveChangesAsync();
    return TypedResults.NoContent();
}

static IResult IncChairStock(DataContext db, int i, int newStock){
    var result = await db.Chair.FindAsync(id);
    if(result is null) return TypedResults.NotFound();

    result.Stock = result.Stock + newStock;
    await db.SaveChangesAsync();
    return TypedResults.Ok(result)
}

static IResult SellingChair(DataContext db, int id, int amount, int pay){
    var result = await db.Chair.FindAsync(id);
    if(result is null) return TypedResults.BadRequest();
    if(result.Stock >= amount){
        int totalToPay = result.Price*amount;
        if(totalToPay != pay) return TypedResults.BadRequest();
        result.Stock = result.Stock - amount;
        await db.SaveChangesAsync();
        return TypedRequest.Ok("Se a/han una/s vendido silla/s")
    }
}

