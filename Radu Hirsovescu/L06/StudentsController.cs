using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Models;

namespace students.api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class StudentsController : ControllerBase
    {
        private IStudentsRepository _studentsRepository;

        public StudentsController(IStudentsRepository studentsRepository)
        {
            _studentsRepository = studentsRepository;
        }

        [HttpGet]
        public async Task<IEnumerable<StudentEntity>> Get()
        {
            return await _studentsRepository.GetAllStudents();
        }

        [HttpGet("{id}")]
        public async Task<StudentEntity> GetStudent([FromRoute] string id)
        {
            return await _studentsRepository.GetStudent(id);
        }

        [HttpPost]
        public async Task<string> Post([FromBody] StudentEntity student)
        {
            try
            {
                await _studentsRepository.InsertNewStudent(student);

                return "S-a adaugat cu succes!";
            }
            catch (System.Exception e)
            {
                return "Eroare: " + e.Message;
            }
        }

        [HttpDelete("{id}")]
        public async Task<string> Delete([FromRoute] string id)
        {
            try
            {
                await _studentsRepository.DeleteStudent(id);

                return "S-a sters cu succes!";
            }
            catch (System.Exception e)
            {
                return "Eroare: " + e.Message;
            }

        }

        [HttpPut]
        public async Task<string> Edit([FromBody] StudentEntity student) {
            try
            {
                await _studentsRepository.EditStudent(student);

                return "S-a modificat cu succes!";
            }
            catch (System.Exception e)
            {
                return "Eroare: " + e.Message;
            }
        }
    }
}
