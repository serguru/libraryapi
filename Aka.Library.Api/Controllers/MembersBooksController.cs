using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Aka.Library.Data;
using Aka.Library.Data.Entities;
using Aka.Library.Data.Models;

namespace Aka.Library.Api.Controllers
{
    [Produces("application/json")]
    [Route("api/members/{mid:int}/books")]
    public class MembersBooksController : BaseController
    {
        public MembersBooksController([FromServices] LibraryContext libraryContext)
            : base(libraryContext)
        {
        }

        // GET: api/Members
        [HttpGet("signedout")]
        public async Task<IActionResult> GetMemberBooks(int mid)
        {
            var sob = await db.BookSignedOut.Where(m => m.MemberId == mid && m.WhenReturned == null).ToListAsync();
            var libraryBooks = 
                from sb in sob
                join lb in db.LibraryBook on sb.LibraryBookSid equals lb.LibraryBookSid into lbs
                from lb in lbs
                select new BookSignedOutDetails
                {
                    LibraryBookSid = sb.LibraryBookSid,
                    BookId = lb.BookId,
                    LibraryId = lb.LibraryId,
                    MemberId = sb.MemberId,
                    WhenReturned = sb.WhenReturned,
                    WhenSignedOut = sb.WhenSignedOut
                };

            return Ok(libraryBooks);
        }

        // GET: api/Members/5
        [HttpGet("history")]
        public async Task<IActionResult> GetMemberBooksHistory(int mid)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var sob = await db.BookSignedOut.Where(m => m.MemberId == mid && m.WhenReturned != null).ToListAsync();
            var libraryBooks =
                from sb in sob
                join lb in db.LibraryBook on sb.LibraryBookSid equals lb.LibraryBookSid into lbs
                from lb in lbs
                select new BookSignedOutDetails
                {
                    LibraryBookSid = sb.LibraryBookSid,
                    BookId = lb.BookId,
                    LibraryId = lb.LibraryId,
                    MemberId = sb.MemberId,
                    WhenReturned = sb.WhenReturned,
                    WhenSignedOut = sb.WhenSignedOut
                };

            return Ok(libraryBooks);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}