using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Aka.Library.Data;
using Aka.Library.Data.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Aka.Library.Api.Controllers
{
    [Produces("application/json")]
    [Route("api/libraries/{libraryId:int}/books/{bookId:int}/")]
    public class ManageBookController : BaseController
    {
        public ManageBookController([FromServices] LibraryContext libraryContext)
            : base(libraryContext)
        { }

        // GET: api/LibraryBooks
        [HttpGet]
        public Task<BookTitle> GetBook(int libraryId, int bookId)
        {
            var book = db.LibraryBook.Where(l => l.LibraryId == libraryId && l.BookId == bookId).Include(b => b.Book).Select(lb=> lb.Book).FirstOrDefaultAsync();

            return book;
        }

        /// <summary>
        /// Sign out of a book
        /// </summary>
        /// <param name="memberId">Member Id</param>
        /// <param name="libraryId">Library Id</param>
        /// <param name="bookId">Book Id</param>
        /// <returns></returns>
        [HttpPost("signout/{memberId:int}")]
        public BookSignedOut Post(int libraryId, int bookId, int memberId)
        {
            var bookSid = db.LibraryBook.FirstOrDefault(@lb => @lb.LibraryId == libraryId && @lb.BookId == bookId);

            if (bookSid == null)
            {
                throw new StatusCodeException(HttpStatusCode.NotFound);
            }

            BookSignedOut bookSignedOut = new BookSignedOut() { MemberId = memberId, LibraryBookSid = bookSid.LibraryBookSid, WhenSignedOut = DateTime.Now };
            db.BookSignedOut.Add(bookSignedOut);

            try
            {
                db.SaveChanges();
            }
            catch (Exception)
            {
                throw new StatusCodeException(HttpStatusCode.NotFound);
            }

            return bookSignedOut;
        }


        /// <summary>
        /// Return of a book
        /// </summary>
        /// <param name="memberId">Member Id</param>
        /// <param name="bookId">Book Id to return</param>
        /// <param name="libraryId">Library Id that the book is being returned to</param>
        /// <returns></returns>
        [HttpPut("return/{memberId:int}")]
        public BookSignedOut Put(int memberId, int bookId, int libraryId)
        {
            var bso = db.BookSignedOut.FirstOrDefault(@lb => @lb.MemberId == memberId && @lb.LibraryBook.BookId == bookId && @lb.LibraryBook.LibraryId == libraryId && @lb.WhenReturned == null);

            if (bso == null)
            {
                throw new StatusCodeException(HttpStatusCode.NotFound);
            }

            bso.WhenReturned = DateTime.Now;

            db.Entry(bso).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                bool BookSignedOutExists(int lbid) => db.BookSignedOut.Count(e => e.LibraryBookSid == lbid) > 0;
                if (!BookSignedOutExists(bso.LibraryBookSid))
                {
                    throw new StatusCodeException(HttpStatusCode.NotFound);
                }
                else
                {
                    throw new StatusCodeException(HttpStatusCode.BadRequest);
                }
            }
            catch (Exception)
            {
                throw new StatusCodeException(HttpStatusCode.NotFound);
            }

            return bso;
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