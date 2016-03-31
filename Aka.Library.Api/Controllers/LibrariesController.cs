using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Aka.Library.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Aka.Library.Api.Controllers
{
    [Produces("application/json")]
    [Route("api/libraries")]
    public class LibrariesController : BaseController
    {
        public LibrariesController([FromServices] LibraryContext libraryContext)
            : base(libraryContext)
        { }
        
        // GET: api/Libraries
        [HttpGet]
        public Task<List<Data.Entities.Library>> Get()
        {
            return db.Library.ToListAsync();
        }

        // GET: api/Libraries/5
        [HttpGet("{id}", Name = "Get")]
        public async Task<Data.Entities.Library> Get(int id)
        {
            return await db.Library.FirstOrDefaultAsync(o => o.LibraryId == id);
        }
        
        // POST: api/Libraries
        [HttpPost]
        public async Task Post([FromBody]Data.Entities.Library library)
        {
            await db.Library.AddAsync(library);
        }
        
        // PUT: api/Libraries/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]Data.Entities.Library library)
        {
        }
        
        // DELETE: api/ApiWithActions/5
        //[HttpDelete("{id}")]
        //public async Task Delete(int id)
        //{
        //    var library = await db.Library.FirstOrDefaultAsync(o => o.LibraryId == id);
        //    db.Library.Remove(library);
        //}

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
