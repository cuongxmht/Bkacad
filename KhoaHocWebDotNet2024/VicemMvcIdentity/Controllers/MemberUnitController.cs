using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using VicemMvcIdentity.Data;
using VicemMvcIdentity.Models.Entities;
using VicemMvcIdentity.Models.Process;

namespace VicemMVCIdentity.Controllers
{
    public class MemberUnitController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MemberUnitController(ApplicationDbContext context)
        {
            _context = context;
        }

        [Authorize(Policy = nameof(SystemPermissions.MemberUnitView))]
        public async Task<IActionResult> Index()
        {
            return View(await _context.MemberUnit.ToListAsync());
        }
        [Authorize(Policy = nameof(SystemPermissions.MemberUnitCreate))]
        public IActionResult Create()
        {
            var listMemberUnit = new List<MemberUnit>
            {
                new MemberUnit(1, "Vicem Hạ Long", "Thống Nhất, huyện Hoành Bồ, tỉnh Quảng Ninh", "0203 3699240", "www.ximanghalong.vn"),
                new MemberUnit(2, "Vicem Hoàng Thạch", "Thị trấn Minh Tân - Huyện Kinh Môn - Tỉnh Hải Dương", "0220 3821092", " www.ximanghoangthach.com"),
                new MemberUnit(3, "Vicem Hải Phòng", "Tràng Kênh, Minh Đức, Thủy Nguyên, Hải Phòng", "0225 3875356", "www.ximanghaiphong.com.vn"),
                new MemberUnit(4, "Vicem Sông Thao", "Ninh Dân, Thanh Ba, Phú Thọ", "0210 3663272", "www.ximangsongthao.com"),
                new MemberUnit(5, "Vicem Bút Sơn", "Xã Thanh Sơn, Huyện Kim Bảng, Tỉnh Hà Nam", "0226 3851323", "www.vicembutson.com.vn"),
                new MemberUnit(6, "Vicem Tam Điệp", "Số 27, đường Chi Lăng, xã Quang Sơn, thành phố Tam Điệp, tỉnh Ninh Bình", "0229 3772772", "www.vicemtamdiep.com.vn"),
                new MemberUnit(7, "Vicem Bỉm Sơn", "Phường Ba Đình, Thị xã Bỉm Sơn, Tỉnh Thanh Hóa", "0237 3824242", "www.ximangbimson.com.vn"),
                new MemberUnit(8, "Vicem Hoàng Mai", "Phường Quỳnh Thiện, Thị xã Hoàng Mai, Tỉnh Nghệ An", "0238 866170", "www.ximanghoangmai.com.vn"),
                new MemberUnit(9, "Vicem Hải Vân", "65 Nguyễn Văn Cừ, Quận Liên Triều, Tp.Đà Nẵng", "0236.3842172", "www.haivancement.vn"),
                new MemberUnit(10, "Vicem Hà Tiên 1", "360 Bến Chương Dương, phường Cầu Kho, Quận 1, Tp.Hồ Chí Minh", "028.38 368 363", "https://www.vicemhatien.com.vn/")
            };
            try
            {
                _context.MemberUnit.AddRange(listMemberUnit);
                _context.SaveChanges();
            }
            catch { }
            return View();
        }
        [Authorize(Policy = nameof(SystemPermissions.MemberUnitCreate))]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MemberUnitId,Name,Address,PhoneNumber,WebsiteUrl")] MemberUnit memberUnit)
        {
            if (ModelState.IsValid)
            {
                _context.Add(memberUnit);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(memberUnit);
        }
        [Authorize(Policy = nameof(SystemPermissions.MemberUnitEdit))]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var memberUnit = await _context.MemberUnit.FindAsync(id);
            if (memberUnit == null)
            {
                return NotFound();
            }
            return View(memberUnit);
        }
        [Authorize(Policy = nameof(SystemPermissions.MemberUnitEdit))]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MemberUnitId,Name,Address,PhoneNumber,WebsiteUrl")] MemberUnit memberUnit)
        {
            if (id != memberUnit.MemberUnitId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(memberUnit);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MemberUnitExists(memberUnit.MemberUnitId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(memberUnit);
        }
        [Authorize(Policy = nameof(SystemPermissions.MemberUnitDelete))]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var memberUnit = await _context.MemberUnit
                .FirstOrDefaultAsync(m => m.MemberUnitId == id);
            if (memberUnit == null)
            {
                return NotFound();
            }

            return View(memberUnit);
        }
        [Authorize(Policy = nameof(SystemPermissions.MemberUnitDelete))]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var memberUnit = await _context.MemberUnit.FindAsync(id);
            if (memberUnit != null)
            {
                _context.MemberUnit.Remove(memberUnit);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MemberUnitExists(int id)
        {
            return _context.MemberUnit.Any(e => e.MemberUnitId == id);
        }
    }
}
