using ArenaSync.Web.Data;
using ArenaSync.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace ArenaSync.Web.Services
{
    public class VendorService : IVendorService
    {
        private readonly ApplicationDbContext _context;

        public VendorService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Vendor>> GetAllVendorsAsync()
        {
            return await _context.Vendors
                .Include(v => v.SuppliesAt)
                .Include(v => v.VendorAssignments)
                .OrderBy(v => v.Name)
                .ToListAsync();
        }

        public async Task<Vendor?> GetVendorByIdAsync(int id)
        {
            return await _context.Vendors
                .Include(v => v.SuppliesAt)
                    .ThenInclude(sa => sa.Event)
                        .ThenInclude(e => e!.Venue)
                .Include(v => v.VendorAssignments)
                    .ThenInclude(va => va.Event)
                .Include(v => v.VendorAssignments)
                    .ThenInclude(va => va.Booth)
                        .ThenInclude(b => b!.Venue)
                .FirstOrDefaultAsync(v => v.Id == id);
        }

        public async Task<bool> CreateVendorAsync(Vendor vendor)
        {
            var name = vendor.Name.Trim();
            var exists = await _context.Vendors.AnyAsync(v => v.Name == name);
            if (exists)
            {
                return false;
            }

            vendor.Name = name;
            vendor.Type = vendor.Type.Trim();
            vendor.Location = vendor.Location.Trim();
            vendor.Phone = vendor.Phone.Trim();

            _context.Vendors.Add(vendor);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<Vendor?> UpdateVendorAsync(Vendor vendor)
        {
            var name = vendor.Name.Trim();
            var duplicate = await _context.Vendors
                .AnyAsync(v => v.Id != vendor.Id && v.Name == name);

            if (duplicate)
            {
                return null;
            }

            var existingVendor = await _context.Vendors.FindAsync(vendor.Id);
            if (existingVendor is null)
            {
                return null;
            }

            existingVendor.Name = name;
            existingVendor.Type = vendor.Type.Trim();
            existingVendor.Location = vendor.Location.Trim();
            existingVendor.Phone = vendor.Phone.Trim();

            await _context.SaveChangesAsync();
            return existingVendor;
        }

        public async Task<bool> DeleteVendorAsync(int id)
        {
            var vendor = await _context.Vendors.FindAsync(id);
            if (vendor is null)
            {
                return false;
            }

            var hasDependencies = await _context.SuppliesAt.AnyAsync(sa => sa.VendorId == id)
                || await _context.VendorAssignments.AnyAsync(va => va.VendorId == id);

            if (hasDependencies)
            {
                return false;
            }

            _context.Vendors.Remove(vendor);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<Vendor>> GetVendorsForEventAsync(int eventId)
        {
            return await _context.SuppliesAt
                .Where(sa => sa.EventId == eventId)
                .Include(sa => sa.Vendor)
                .Select(sa => sa.Vendor!)
                .OrderBy(v => v.Name)
                .ToListAsync();
        }

        public async Task<List<Vendor>> GetAvailableVendorsForEventAsync(int eventId)
        {
            var assignedVendorIds = await _context.SuppliesAt
                .Where(sa => sa.EventId == eventId)
                .Select(sa => sa.VendorId)
                .ToListAsync();

            return await _context.Vendors
                .Where(v => !assignedVendorIds.Contains(v.Id))
                .OrderBy(v => v.Name)
                .ToListAsync();
        }

        public async Task<bool> AddVendorToEventAsync(int vendorId, int eventId)
        {
            var vendorExists = await _context.Vendors.AnyAsync(v => v.Id == vendorId);
            var eventExists = await _context.Events.AnyAsync(e => e.Id == eventId);
            var alreadyAssigned = await _context.SuppliesAt
                .AnyAsync(sa => sa.VendorId == vendorId && sa.EventId == eventId);

            if (!vendorExists || !eventExists || alreadyAssigned)
            {
                return false;
            }

            _context.SuppliesAt.Add(new SuppliesAt
            {
                VendorId = vendorId,
                EventId = eventId
            });

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RemoveVendorFromEventAsync(int vendorId, int eventId)
        {
            var supplier = await _context.SuppliesAt
                .FindAsync(vendorId, eventId);

            if (supplier is null)
            {
                return false;
            }

            var boothAssignments = await _context.VendorAssignments
                .Where(va => va.VendorId == vendorId && va.EventId == eventId)
                .ToListAsync();

            _context.VendorAssignments.RemoveRange(boothAssignments);
            _context.SuppliesAt.Remove(supplier);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
