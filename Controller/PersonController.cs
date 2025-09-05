using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UserManagementAPI.Data;
using UserManagementAPI.Model;
namespace UserManagementAPI.Controller
{
    [Route("api/users")] // base route
    [ApiController]
    public class PersonController : ControllerBase
    {
        private readonly ApplicationDBContext _context; // db context

        public PersonController(ApplicationDBContext context) // constructor
        {
            _context = context;
        }

        [Authorize]
        [HttpGet] // Get all persons
        public IActionResult GetAllPersons()
        {
            var persons = _context.Persons.Select(p => new Dto.Person.getAllPersonsDTO
            {
                Id = p.Id,
                FirstName = p.FirstName,
                LastName = p.LastName,
                Email = p.Email
            }).ToList();
            return Ok(persons);
        }
        // [Authorize] /* sadece login olanlar gorebilir */
        [HttpGet("{id}")] // Get person by id, mesela: api/person/1
        public IActionResult GetPersonById([FromRoute] string id)
        {
            var person = _context.Persons.Find(id);
            if (person == null)
            {
                return NotFound();
            }
            return Ok(new { person.Id, person.FirstName, person.LastName, person.Email });
        }

        // sadece admin guncelleyebilir
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]

        public IActionResult UpdatePerson([FromRoute] string id, [FromBody] Dto.Person.UpdatePersonDTO dto)
        {
            var user = _context.Persons.Find(id);
            if (user == null)
                return NotFound();

            // unique mail controlü
            if (dto.Email != null && _context.Persons.Any(p => p.Email == dto.Email && p.Id != id))
                return BadRequest("Email already exists.");
                
            // update the old person
            user.FirstName = dto.FirstName ?? user.FirstName;
            user.LastName = dto.LastName ?? user.LastName;
            user.Email = dto.Email ?? user.Email;

            _context.Persons.Update(user);
            _context.SaveChanges();

            return Ok(new { user.Id, user.FirstName, user.LastName, user.Email });
        }

        // sadece admin silebilir
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public IActionResult DeletePerson([FromRoute] string id)
        {
            var user = _context.Persons.Find(id);
            if (user == null)
                return NotFound();

            _context.Persons.Remove(user);
            _context.SaveChanges();

            return NoContent();
        }

        // sadece admin yeni user ekleyebilir
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult CreatePerson([FromBody] Dto.Person.CreatePersonDTO dto)
        {
            var user = new Person
            {
                UserName = dto.Email,
                Email = dto.Email,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                RegisterDate = DateOnly.FromDateTime(DateTime.Now),
                IsActive = true
            };
            _context.Persons.Add(user);
            _context.SaveChanges();

            return CreatedAtAction(nameof(GetPersonById), new { id = user.Id }, user);
        }
    }
}