using Bagman.Domain.Models;
using Bagman.Domain.Repositories;
using Bagman.Infrastructure.Data;
using ErrorOr;
using Microsoft.EntityFrameworkCore;

namespace Bagman.Infrastructure.Repositories;

public class EfTableRepository : ITableRepository
{
    private readonly ApplicationDbContext _db;

    public EfTableRepository(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<ErrorOr<Table?>> GetByIdAsync(Guid id)
    {
        try
        {
            var table = await _db.Tables
                .Include(t => t.Members)
                .Include(t => t.Matches)
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Id == id);

            return table;
        }
        catch (Exception ex)
        {
            return Error.Failure("Table.GetByIdError", $"Błąd podczas pobierania stołu: {ex.Message}");
        }
    }

    public async Task<ErrorOr<Table?>> GetByNameAsync(string name)
    {
        try
        {
            var table = await _db.Tables
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Name == name);

            return table;
        }
        catch (Exception ex)
        {
            return Error.Failure("Table.GetByNameError", $"Błąd podczas pobierania stołu: {ex.Message}");
        }
    }

    public async Task<ErrorOr<List<Table>>> GetByUserIdAsync(Guid userId)
    {
        try
        {
            var tables = await _db.Tables
                .Where(t => t.Members.Any(m => m.UserId == userId))
                .Include(t => t.Members)
                .AsNoTracking()
                .ToListAsync();

            return tables;
        }
        catch (Exception ex)
        {
            return Error.Failure("Table.GetByUserIdError", $"Błąd podczas pobierania stołów użytkownika: {ex.Message}");
        }
    }

    public async Task<ErrorOr<Table>> CreateAsync(Table table)
    {
        try
        {
            _db.Tables.Add(table);
            await _db.SaveChangesAsync();
            return table;
        }
        catch (Exception ex)
        {
            return Error.Failure("Table.CreateError", $"Błąd podczas tworzenia stołu: {ex.Message}");
        }
    }

    public async Task<ErrorOr<Table>> UpdateAsync(Table table)
    {
        try
        {
            _db.Tables.Update(table);
            await _db.SaveChangesAsync();
            return table;
        }
        catch (Exception ex)
        {
            return Error.Failure("Table.UpdateError", $"Błąd podczas aktualizacji stołu: {ex.Message}");
        }
    }

    public async Task<ErrorOr<Success>> DeleteAsync(Guid id)
    {
        try
        {
            var table = await _db.Tables.FirstOrDefaultAsync(t => t.Id == id);
            if (table == null)
                return Error.NotFound("Table.NotFound", "Stół nie został znaleziony");

            _db.Tables.Remove(table);
            await _db.SaveChangesAsync();
            return Result.Success;
        }
        catch (Exception ex)
        {
            return Error.Failure("Table.DeleteError", $"Błąd podczas usuwania stołu: {ex.Message}");
        }
    }

    public async Task<ErrorOr<List<TableMember>>> GetMembersAsync(Guid tableId)
    {
        try
        {
            var members = await _db.TableMembers
                .Where(m => m.TableId == tableId)
                .Include(m => m.User)
                .AsNoTracking()
                .ToListAsync();

            return members;
        }
        catch (Exception ex)
        {
            return Error.Failure("Table.GetMembersError", $"Błąd podczas pobierania członków stołu: {ex.Message}");
        }
    }

    public async Task<ErrorOr<TableMember?>> GetMemberAsync(Guid tableId, Guid userId)
    {
        try
        {
            var member = await _db.TableMembers
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.TableId == tableId && m.UserId == userId);

            return member;
        }
        catch (Exception ex)
        {
            return Error.Failure("Table.GetMemberError", $"Błąd podczas pobierania członka stołu: {ex.Message}");
        }
    }

    public async Task<ErrorOr<Success>> AddMemberAsync(TableMember member)
    {
        try
        {
            _db.TableMembers.Add(member);
            await _db.SaveChangesAsync();
            return Result.Success;
        }
        catch (Exception ex)
        {
            return Error.Failure("Table.AddMemberError", $"Błąd podczas dodawania członka stołu: {ex.Message}");
        }
    }

    public async Task<ErrorOr<Success>> RemoveMemberAsync(Guid tableId, Guid userId)
    {
        try
        {
            var member = await _db.TableMembers
                .FirstOrDefaultAsync(m => m.TableId == tableId && m.UserId == userId);

            if (member == null)
                return Error.NotFound("Table.MemberNotFound", "Członek stołu nie został znaleziony");

            _db.TableMembers.Remove(member);
            await _db.SaveChangesAsync();
            return Result.Success;
        }
        catch (Exception ex)
        {
            return Error.Failure("Table.RemoveMemberError", $"Błąd podczas usuwania członka stołu: {ex.Message}");
        }
    }

    public async Task<ErrorOr<Success>> UpdateMemberAdminAsync(Guid tableId, Guid userId, bool isAdmin)
    {
        try
        {
            var member = await _db.TableMembers
                .FirstOrDefaultAsync(m => m.TableId == tableId && m.UserId == userId);

            if (member == null)
                return Error.NotFound("Table.MemberNotFound", "Członek stołu nie został znaleziony");

            member.IsAdmin = isAdmin;
            _db.TableMembers.Update(member);
            await _db.SaveChangesAsync();
            return Result.Success;
        }
        catch (Exception ex)
        {
            return Error.Failure("Table.UpdateMemberError", $"Błąd podczas aktualizacji roli członka: {ex.Message}");
        }
    }
}
